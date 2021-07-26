using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Calypso.FunctionApp.Models;
using Calypso.FunctionApp.Options;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Calypso.FunctionApp.Services
{
    public class SyncService : ISyncService
    {
        private const string TableName = "Feedback";
        private const string TaskReadWriteScope = "https://graph.microsoft.com/Tasks.ReadWrite";
        private const string UserAssertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer";

        private readonly TableClient _tableClient;
        private readonly IOptions<AzureAdOptions> _azureAdOptions;
        public SyncService(IOptions<FeedbacksStorageOptions> feedbacksStorageOptions, IOptions<AzureAdOptions> azureAdOptions)
        {
            _azureAdOptions = azureAdOptions;
            _tableClient = new TableClient(feedbacksStorageOptions.Value.ConnectionString, TableName);
        }
        public async Task SyncStatuses()
        {
            var feedbacks = _tableClient.Query<Feedback>().ToList();
            var graphClient = await GetGraphServiceClient("tokenHere", TaskReadWriteScope);
            foreach (var feedback in feedbacks)
            {
                var task = await graphClient.Planner.Tasks[feedback.TaskId]
                    .Request()
                    .GetAsync();
                var taskStatus = MapProgressToStatus(task.PercentComplete.Value);
                if (taskStatus != feedback.Status)
                {
                    feedback.Status = taskStatus;
                    await _tableClient.UpdateEntityAsync(feedback, feedback.ETag);
                }
            }
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

        private string MapProgressToStatus(int progress)
        {
            return progress switch
            {
                0 => "New",
                50 => "InProgress",
                100 => "Completed",
                _ => throw new ArgumentException("Invalid progress")
            };
        }
    }
}