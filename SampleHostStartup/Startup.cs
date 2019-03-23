using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace SampleHostStartup
{
    public sealed class Startup
    {
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment environment;

        public Startup(IConfiguration config, IWebHostEnvironment environment)
        {
            this.config = config;

            this.environment = environment;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddOrleans(config, environment, builder =>
            {
                var siloPort = 11111;
                var siloAddress = IPAddress.Loopback;

                int gatewayPort = 30000;

                builder.UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(siloAddress, siloPort));
                builder.UseInMemoryReminderService();
                builder.ConfigureEndpoints(siloAddress, siloPort, gatewayPort);
                builder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "helloworldcluster";
                    options.ServiceId = "1";
                });

                builder.UseDashboard(options =>
                {
                    options.HostSelf = false;
                });
            });

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOrleansDashboard();
        }
    }
}
