
using Dapr.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Dapr.Backend
{
    public class Startup
    {
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseCloudEvents();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();

                endpoints.MapGet(DaprConstants.MethodNames.Balance, HttpActions.Balance);
                endpoints
                    .MapPost(DaprConstants.MethodNames.Deposit, HttpActions.Deposit)
                    .WithTopic(DaprConstants.PubsubName, DaprConstants.MethodNames.Deposit);
                endpoints
                    .MapPost(DaprConstants.MethodNames.Withdraw, HttpActions.Withdraw)
                    .WithTopic(DaprConstants.PubsubName, DaprConstants.MethodNames.Withdraw);
            });                
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDaprClient();
            services.AddSingleton(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            });
        }
    }
}
