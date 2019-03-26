using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;

namespace Orleans.WebHostCompatibilityLayer
{
    internal sealed class SiloServiceBuilder : ISiloBuilder
    {
        private readonly Dictionary<object, object> properties = new Dictionary<object, object>();
        private readonly List<Action<HostBuilderContext, ISiloBuilder>> configureSiloDelegates = new List<Action<HostBuilderContext, ISiloBuilder>>();
        private readonly List<Action<HostBuilderContext, IServiceCollection>> configureServicesDelegates = new List<Action<HostBuilderContext, IServiceCollection>>();

        public IDictionary<object, object> Properties
        {
            get { return properties; }
        }

        public void Build(HostBuilderContext context, IServiceCollection serviceCollection)
        {
            foreach (var configurationDelegate in configureSiloDelegates)
            {
                configurationDelegate(context, this);
            }

            serviceCollection.AddHostedService<SiloHost>();

            this.ConfigureDefaults();
            this.ConfigureApplicationParts(parts => parts.ConfigureDefaults());

            foreach (var configurationDelegate in configureServicesDelegates)
            {
                configurationDelegate(context, serviceCollection);
            }
        }

        public ISiloBuilder ConfigureSilo(Action<HostBuilderContext, ISiloBuilder> configureDelegate)
        {
            Guard.NotNull(configureDelegate, nameof(configureDelegate));

            configureSiloDelegates.Add(configureDelegate);

            return this;
        }

        public ISiloBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            Guard.NotNull(configureDelegate, nameof(configureDelegate));

            configureServicesDelegates.Add(configureDelegate);

            return this;
        }
    }
}
