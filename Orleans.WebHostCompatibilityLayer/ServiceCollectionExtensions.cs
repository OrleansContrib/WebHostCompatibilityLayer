using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using Orleans.WebHostCompatibilityLayer;
using System;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to add orleans to a service collection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the service collection to host an Orleans silo.
        /// </summary>
        /// <param name="services">The services. Cannot be null.</param>
        /// <param name="environment">The host environment. Cannot be null.</param>
        /// <param name="configuration">The configuration object. Cannot be null.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo. Cannot be null.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Do not call this method multiple times.
        /// </remarks>
        public static IServiceCollection AddOrleans(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment, Action<HostBuilderContext, ISiloBuilder> configureDelegate)
        {
            Guard.NotNull(environment, nameof(environment));
            Guard.NotNull(configuration, nameof(configuration));
            Guard.NotNull(configureDelegate, nameof(configureDelegate));

            var siloBuilder = new SiloServiceBuilder();

            var context = ContextBuilder.Build(siloBuilder, configuration, environment);

            siloBuilder.ConfigureSilo(configureDelegate);
            siloBuilder.Build(context, services);

            return services;
        }

        /// <summary>
        /// Configures the service collection to host an Orleans silo.
        /// </summary>
        /// <param name="services">The services. Cannot be null.</param>
        /// <param name="context">The host builder context. Cannot be null.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo. Cannot be null.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Do not call this method multiple times.
        /// </remarks>
        public static IServiceCollection AddOrleans(this IServiceCollection services, WebHostBuilderContext context, Action<HostBuilderContext, ISiloBuilder> configureDelegate)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(configureDelegate, nameof(configureDelegate));

            return AddOrleans(services, context.Configuration, context.HostingEnvironment, configureDelegate);
        }

        /// <summary>
        /// Configures the service collection to host an Orleans silo.
        /// </summary>
        /// <param name="services">The services. Cannot be null.</param>
        /// <param name="context">The host builder context. Cannot be null.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo. Cannot be null.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Do not call this method multiple times.
        /// </remarks>
        public static IServiceCollection AddOrleans(this IServiceCollection services, WebHostBuilderContext context, Action<ISiloBuilder> configureDelegate)
        {
            Guard.NotNull(configureDelegate, nameof(configureDelegate));

            return AddOrleans(services, context.Configuration, context.HostingEnvironment, (c, b) => configureDelegate(b));
        }

        /// <summary>
        /// Configures the service collection to host an Orleans silo.
        /// </summary>
        /// <param name="services">The services. Cannot be null.</param>
        /// <param name="environment">The host environment. Cannot be null.</param>
        /// <param name="configuration">The configuration object. Cannot be null.</param>
        /// <param name="configureDelegate">The delegate used to configure the silo. Cannot be null.</param>
        /// <returns>The host builder.</returns>
        /// <remarks>
        /// Do not call this method multiple times.
        /// </remarks>
        public static IServiceCollection AddOrleans(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment, Action<ISiloBuilder> configureDelegate)
        {
            Guard.NotNull(configureDelegate, nameof(configureDelegate));

            return AddOrleans(services, configuration, environment, (c, b) => configureDelegate(b));
        }
    }
}
