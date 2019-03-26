using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

namespace SampleHost
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseOrleans(builder =>
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
                })
                .UseOrleans(builder =>
                {
                    builder.UseDashboard(options =>
                    {
                        options.HostSelf = false;
                    });
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                })
                .Configure(app =>
                {
                    app.UseOrleansDashboard();
                })
                .Build()
                .Run();
        }
    }
}
