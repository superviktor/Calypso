using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calypso.Api.Config;
using Calypso.Api.Exceptions;
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
        private const string TeamSettingsScope = "https://graph.microsoft.com/TeamSettings.ReadWrite.All";
        private const string ChannelSettingsScope = "https://graph.microsoft.com/ChannelSettings.ReadWrite.All";

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
            var graphServiceClient = await GetGraphServiceClient(authToken);
            var planId = await GetPlanId(graphServiceClient, _teamOptions.Value.Team, _plannerOptions.Value.Plan);
            var task = await graphServiceClient.Planner.Tasks.Request()
                .AddAsync(new PlannerTask
                {
                    PlanId = planId,
                    Title = title,
                    Assignments = new PlannerAssignments()
                });
            return task.Id;
        }

        public async Task AddTaskDetails(string authToken, string taskId, string description, string attachmentUrl = null)
        {
            var graphServiceClient = await GetGraphServiceClient(authToken);
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
            var graphServiceClient = await GetGraphServiceClient(authToken);
            var chatMessage = new ChatMessage
            {
                Body = new ItemBody
                {
                    Content = $"Task {taskTitle} created"
                }
            };
            var teamId = await GetTeamId(graphServiceClient, _teamOptions.Value.Team);
            var channelId = await GetChannelId(graphServiceClient, teamId, _teamOptions.Value.Channel);
            await graphServiceClient.Teams[teamId].Channels[channelId].Messages
                .Request()
                .AddAsync(chatMessage);
        }

        private async Task<GraphServiceClient> GetGraphServiceClient(string authToken)
        {
            var scopes = new List<string> { TaskReadWriteScope, ChannelMessageSendScope, TeamSettingsScope, ChannelSettingsScope };
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

        private async Task<string> GetTeamId(GraphServiceClient graphServiceClient, string team)
        {
            var joinedTeams = await graphServiceClient.Me.JoinedTeams
                .Request()
                .GetAsync();
            var selectedTeam =
                joinedTeams.SingleOrDefault(t => t.DisplayName.ToLower().Trim() == team.ToLower().Trim());

            if (selectedTeam is null)
                throw new InvalidConfigurationException("Team is invalid (observe appsettings.json)");

            return selectedTeam.Id;
        }

        private async Task<string> GetChannelId(GraphServiceClient graphServiceClient, string teamId, string channel)
        {
            var channels = await graphServiceClient.Teams[teamId].Channels
                .Request()
                .GetAsync();
            var selectedChannel =
                channels.SingleOrDefault(c => c.DisplayName.ToLower().Trim() == channel.ToLower().Trim());

            if (selectedChannel is null)
                throw new InvalidConfigurationException("Channel is invalid (observe appsettings.json)");

            return selectedChannel.Id;
        }

        private async Task<string> GetPlanId(GraphServiceClient graphServiceClient, string team, string plan)
        {
            var teamId = await GetTeamId(graphServiceClient, team);
            var plans = await graphServiceClient.Groups[teamId].Planner.Plans
                .Request()
                .GetAsync();
            var selectedPlan =
                plans.SingleOrDefault(p => p.Title.ToLower().Trim() == plan.ToLower().Trim());

            if (selectedPlan is null)
                throw new InvalidConfigurationException("Plan is invalid (observe appsettings.json)");

            return selectedPlan.Id;
        }
    }
}