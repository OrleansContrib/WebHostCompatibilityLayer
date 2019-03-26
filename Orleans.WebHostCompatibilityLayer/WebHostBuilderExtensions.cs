using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;
using Orleans.WebHostCompatibilityLayer;
using System;
using System.Globalization;
using System.Linq;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// Extension to configure orleans for the WebHost.
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        private static readonly string ConfigurationMarker = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Configures the host builder to host an Orleans silo.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Calling this method multiple times on the same <see cref="IHostBuilder"/> instance will result in one silo being configured.
        /// However, the effects of <paramref name="configureDelegate"/> will be applied once for each call.
        /// </remarks>
        public static IWebHostBuilder UseOrleans(this IWebHostBuilder hostBuilder, Action<HostBuilderContext, ISiloBuilder> configureDelegate)
        {
            if (configureDelegate == null) throw new ArgumentNullException(nameof(configureDelegate));

            // Track how many times this method is called.
            var thisCallNumber = GetCallCount(hostBuilder) + 1;

            hostBuilder.UseSetting(ConfigurationMarker, thisCallNumber.ToString(CultureInfo.InvariantCulture));

            return hostBuilder.ConfigureServices((hostContext, services) =>
            {
                var siloBuilder = GetOrAddSiloBuilder(services);

                siloBuilder.ConfigureSilo(configureDelegate);

                // Check if this is the final call to this method.
                var callNumber = GetCallCount(hostBuilder);

                if (callNumber == thisCallNumber)
                {
                    var context = ContextBuilder.Build(siloBuilder, hostContext.Configuration, hostContext.HostingEnvironment);

                    siloBuilder.Build(context, services);
                }
            });
        }

        private static SiloServiceBuilder GetOrAddSiloBuilder(IServiceCollection services)
        {
            var registration = services.FirstOrDefault(s => s.ServiceType.Equals(typeof(SiloServiceBuilder)));

            SiloServiceBuilder siloBuilder;

            if (registration == null)
            {
                siloBuilder = new SiloServiceBuilder();

                services.AddSingleton(siloBuilder);
            }
            else
            {
                siloBuilder = (SiloServiceBuilder)registration.ImplementationInstance;
            }

            return siloBuilder;
        }

        private static int GetCallCount(IWebHostBuilder hostBuilder)
        {
            int.TryParse(hostBuilder.GetSetting(ConfigurationMarker), NumberStyles.None, CultureInfo.InvariantCulture, out var thisCallNumber);

            return thisCallNumber;
        }

        /// <summary>
        /// Configures the host builder to host an Orleans silo.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Calling this method multiple times on the same <see cref="IHostBuilder"/> instance will result in one silo being configured.
        /// However, the effects of <paramref name="configureDelegate"/> will be applied once for each call.
        /// </remarks>
        public static IWebHostBuilder UseOrleans(this IWebHostBuilder hostBuilder, Action<ISiloBuilder> configureDelegate)
        {
            Guard.NotNull(configureDelegate, nameof(configureDelegate));

            return hostBuilder.UseOrleans((ctx, siloBuilder) => configureDelegate(siloBuilder));
        }
    }
}