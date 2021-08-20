using Calypso.TeamsApp123.Common;
using Calypso.TeamsApp123.Config;
using Calypso.TeamsApp123.Repositories;
using Calypso.TeamsApp123.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.TeamsFx.SimpleAuth;
using System;

namespace Calypso.TeamsApp123
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddTeamsFxSimpleAuth(Configuration);

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddHttpClient("LocalApi", client => client.BaseAddress = new Uri("https://localhost:44357"));
           
            var azureStorageSection = Configuration.GetSection("Dependencies:AzureStorage");
            services.Configure<AzureStorageOptions>(azureStorageSection);
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IFeedbackImageRepository, FeedbackImageRepository>();
            services.AddScoped<ITeamsIntegrationService, TeamsIntegrationService>();
            services.Configure<PlannerOptions>(Configuration.GetSection(
                PlannerOptions.OptionName));
            services.Configure<TeamOptions>(Configuration.GetSection(
                TeamOptions.OptionName));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");

                endpoints.MapControllers();
            });
        }
    }
}
