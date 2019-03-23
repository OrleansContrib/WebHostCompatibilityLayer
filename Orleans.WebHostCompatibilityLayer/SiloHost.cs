using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orleans.WebHostCompatibilityLayer
{
    public sealed class SiloHost : IHostedService
    {
        private ISiloHost siloHost;
        private readonly IApplicationLifetime lifetime;
        private readonly ILogger<SiloHost> logger;

        public SiloHost(ISiloHost siloHost, IApplicationLifetime lifetime, ILogger<SiloHost> logger)
        {
            this.siloHost = siloHost;
            this.lifetime = lifetime;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Starting Orleans Silo.");
                await siloHost.StartAsync(cancellationToken).ConfigureAwait(false);
                logger.LogInformation("Orleans Silo started.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Orleans Silo failed to start, stopping application.");

                lifetime.StopApplication();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping Orleans Silo");
            await siloHost.StopAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Orleans Silo stopped.");
        }
    }
}
