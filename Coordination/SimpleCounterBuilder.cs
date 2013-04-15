using System.Collections.Generic;
using Configuration;
using Sources;

namespace Coordination
{
    public class SimpleCounterBuilder
    {
        public static List<BuiltComponents> Build(SimplePerformanceCounterSourceConfiguration configs)
        {
            var components = new List<BuiltComponents>();

            foreach(SimpleCounterElement config in configs.Counters)
            {
                components.Add(Build(config));
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