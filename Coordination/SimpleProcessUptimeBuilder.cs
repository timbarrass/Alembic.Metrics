using System.Collections.Generic;
using System.ComponentModel.Composition;
using Configuration;
using Sources;

namespace Coordination
{
    [Export(typeof(ISimpleBuilder))]
    public class SimpleProcessUptimeBuilder : ISimpleBuilder
    {
        public static readonly SimpleProcessUptimeBuilder _instance = new SimpleProcessUptimeBuilder();

        public ISimpleBuilder Instance
        {
            get { return _instance; }
        }

        public IEnumerable<BuiltComponents> Build(System.Configuration.Configuration configuration)
        {
            var components = new List<BuiltComponents>();

            var simpleProcessUptimeConfiguration = configuration.GetSection("simpleProcessUptimeSources") as SimpleProcessUptimeConfiguration;

            if (simpleProcessUptimeConfiguration != null)
            {
                foreach (SimpleProcessElement config in simpleProcessUptimeConfiguration.Processes)
                {
                    components.Add(Build(config));
                }
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