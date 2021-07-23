using System.Collections.Generic;
using System.Threading.Tasks;
using Calypso.Api.Config;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Calypso.Api.Services
{
    public class TeamsIntegrationService : ITeamsIntegrationService
    {
        private const string TaskReadWriteScope = "https://graph.microsoft.com/Tasks.ReadWrite";
        private const string ChannelMessageSendScope = "https://graph.microsoft.com/ChannelMessage.Send";
        private const string UserAssertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer";

        private readonly IOptions<AzureAdOptions> _azureAdOptions;
        private readonly IOptions<PlannerOptions> _plannerOptions;
        private readonly IOptions<TeamOptions> _teamOptions;

        public TeamsIntegrationService(IOptions<AzureAdOptions> azureAdOptions,
            IOptions<PlannerOptions> plannerOptions,
            IOptions<TeamOptions> teamOptions)
        {
            _azureAdOptions = azureAdOptions;
            _plannerOptions = plannerOptions;
            _teamOptions = teamOptions;
        }

        public async Task<string> CreateTask(string authToken, string title)
        {
            var graphServiceClient = await GetGraphServiceClient(authToken, TaskReadWriteScope);
            var task = await graphServiceClient.Planner.Tasks.Request()
                .AddAsync(new PlannerTask
                {
                    PlanId = _plannerOptions.Value.PlanId,
                    Title = title,
                    Assignments = new PlannerAssignments()
                });
            return task.Id;
        }

        public async Task AddTaskDetails(string authToken, string taskId, string description, string attachmentUrl = null)
        {
            var graphServiceClient = await GetGraphServiceClient(authToken, TaskReadWriteScope);
            var taskDetails = await graphServiceClient.Planner.Tasks[taskId].Details.Request()
               .GetAsync();
            var plannerTaskDetailsToUpdate = new PlannerTaskDetails
            {
                Description = description,
                References = new PlannerExternalReferences()
            };
            if (attachmentUrl != null)
                plannerTaskDetailsToUpdate.References.AddReference(attachmentUrl, "attachment.jpg");

            await graphServiceClient.Planner.Tasks[taskId].Details.Request()
                .Header("If-Match", taskDetails.GetEtag())
                .UpdateAsync(plannerTaskDetailsToUpdate);
        }

        public async Task SendChannelMessage(string authToken, string taskTitle)
        {
            var graphServiceClient = await GetGraphServiceClient(authToken, ChannelMessageSendScope);
            var chatMessage = new ChatMessage
            {
                Body = new ItemBody
                {
                    Content = $"Task {taskTitle} created"
                }
            };

            await graphServiceClient.Teams[_teamOptions.Value.TeamId].Channels[_teamOptions.Value.ChannelId].Messages
                .Request()
                .AddAsync(chatMessage);
        }

        private async Task<GraphServiceClient> GetGraphServiceClient(string authToken, string scope)
        {
            var scopes = new List<string> { scope };
            var userAssertion = new UserAssertion(authToken, UserAssertionType);
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(_azureAdOptions.Value.ClientId)
                .WithTenantId(_azureAdOptions.Value.TenantId)
                .WithClientSecret(_azureAdOptions.Value.ClientSecret)
                .Build();
            var authProvider = new OnBehalfOfProvider(confidentialClientApplication, scopes);
            await authProvider.ClientApplication.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync();
            var graphServiceClient = new GraphServiceClient(authProvider);
            return graphServiceClient;
        }
    }
}