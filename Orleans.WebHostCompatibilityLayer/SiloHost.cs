using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Orleans.WebHostCompatibilityLayer
{
    internal sealed class SiloHost : IHostedService
    {
        private ISiloHost siloHost;
        private readonly IApplicationLifetime lifetime;
        private readonly ILogger<SiloHost> logger;
        private ExceptionDispatchInfo startupException;

        public SiloHost(ISiloHost siloHost, IApplicationLifetime lifetime, IEnumerable<IConfigurationValidator> configurationValidators, ILogger<SiloHost> logger)
        {
            ValidateSystemConfiguration(configurationValidators);

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
                startupException = ExceptionDispatchInfo.Capture(ex);

                lifetime.StopApplication();

                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            startupException?.Throw();

            logger.LogInformation("Stopping Orleans Silo");
            await siloHost.StopAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Orleans Silo stopped.");
        }
   
        private void ValidateSystemConfiguration(IEnumerable<IConfigurationValidator> configurationValidators)
        {
            foreach (var validator in configurationValidators)
            {
                validator.ValidateConfiguration();
            }
        }
    }
}
