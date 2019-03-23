using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using Orleans.WebHostCompatibilityLayer;
using System;
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddOrleans(this IServiceCollection services, IConfiguration config, IWebHostEnvironment environment, Action<ISiloBuilder> builder)
        {
            var hostBuilder = new SiloServiceBuilder(config, environment);

            builder?.Invoke(hostBuilder);

            hostBuilder.Build(services);

            return services;
        }

        public static IServiceCollection AddOrleans(this IServiceCollection services, WebHostBuilderContext context, Action<ISiloBuilder> builder)
        {
            return AddOrleans(services, context, builder);
        }
    }
}
