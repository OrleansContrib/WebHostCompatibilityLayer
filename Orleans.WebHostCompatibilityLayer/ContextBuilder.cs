using Microsoft.Extensions.Configuration;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Orleans.WebHostCompatibilityLayer
{
    internal static class ContextBuilder
    {
        public static HostBuilderContext Build(SiloServiceBuilder builder, IConfiguration configuration, IHostingEnvironment environment)
        {
            return new HostBuilderContext(builder.Properties)
            {
                Configuration = configuration,
                HostingEnvironment = new EnvironmentWrapper(environment)
            };
        }
    }
}
