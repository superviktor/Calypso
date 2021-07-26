using Calypso.FunctionApp.Options;
using Calypso.FunctionApp.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Calypso.FunctionApp.Startup))]
namespace Calypso.FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<FeedbacksStorageOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Feedbacks").Bind(settings);
                });
            builder.Services.AddOptions<AzureAdOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("AzureAd").Bind(settings);
                });

            builder.Services.AddTransient<ISyncService, SyncService>();
        }
    }
}