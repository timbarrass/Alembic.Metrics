using System.Collections.Generic;
using Configuration;
using Sources;

namespace Coordination
{
    public class SimpleProcessUptimeBuilder
    {
        public static IEnumerable<BuiltComponents> Build(SimpleProcessUptimeConfiguration configs)
        {
            var components = new List<BuiltComponents>();

            foreach(SimpleProcessElement config in configs.Processes)
            {
                components.Add(Build(config));
            }

            return components;
        }

        public static BuiltComponents Build(SimpleProcessElement simpleConfig)
        {
            var counterElement = new ProcessElement(simpleConfig);
            var sourceConfig = new ProcessUptimeSourceConfiguration(new[] { counterElement });
            var source = ProcessUptimeSourceBuilder.Build(sourceConfig);

            return SimpleComponentBuilder.BuildStandardSinkSet(simpleConfig, source);
        }
    }
}