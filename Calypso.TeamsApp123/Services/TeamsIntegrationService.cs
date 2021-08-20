using System.Linq;
using System.Threading.Tasks;
using Calypso.TeamsApp123.Config;
using Calypso.TeamsApp123.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace Calypso.TeamsApp123.Services
{
    public class TeamsIntegrationService : ITeamsIntegrationService
    {
        private readonly IOptions<PlannerOptions> _plannerOptions;
        private readonly IOptions<TeamOptions> _teamOptions;

        public TeamsIntegrationService(
            IOptions<PlannerOptions> plannerOptions,
            IOptions<TeamOptions> teamOptions)
        {
            _plannerOptions = plannerOptions;
            _teamOptions = teamOptions;
        }

        public async Task<string> CreateTask(string authToken, string title)
        {
            var graphServiceClient = GetGraphServiceClient(authToken);
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
            var graphServiceClient = GetGraphServiceClient(authToken);
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
            var graphServiceClient = GetGraphServiceClient(authToken);
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

        private GraphServiceClient GetGraphServiceClient(string token)
        {
            var client = new GraphServiceClient(new DelegateAuthenticationProvider(async (r) =>
            {
                r.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }));

            return client;
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