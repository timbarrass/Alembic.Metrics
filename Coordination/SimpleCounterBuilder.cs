using System.Collections.Generic;
using Configuration;
using Sources;

namespace Coordination
{
    public class SimpleCounterBuilder : ISimpleBuilder
    {
        public static readonly SimpleCounterBuilder _instance = new SimpleCounterBuilder();

        public ISimpleBuilder Instance
        {
            get { return _instance;}
        }

        public IEnumerable<BuiltComponents> Build(System.Configuration.Configuration configuration)
        {
            var components = new List<BuiltComponents>();

            var simpleCounterConfiguration = configuration.GetSection("simplePerformanceCounterSources") as SimplePerformanceCounterSourceConfiguration;

            if (simpleCounterConfiguration != null)
            {
                foreach (SimpleCounterElement config in simpleCounterConfiguration.Counters)
                {
                    components.Add(Build(config));
                }
            }

            return components;
        }

        public static BuiltComponents Build(SimpleCounterElement simpleConfig)
        {
            var counterElement = new CounterElement(simpleConfig);
            var sourceConfig = new PerformanceCounterDataSourceConfiguration(new[] { counterElement });
            var source = PerformanceCounterDataSourceBuilder.Build(sourceConfig);

            return SimpleComponentBuilder.BuildStandardSinkSet(simpleConfig, source);
        }
    }
}