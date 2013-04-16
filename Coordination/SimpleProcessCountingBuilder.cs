using System.Collections.Generic;
using System.ComponentModel.Composition;
using Configuration;
using Sources;

namespace Coordination
{
    [Export(typeof(ISimpleBuilder))]
    public class SimpleProcessCountingBuilder : ISimpleBuilder
    {
        public static readonly SimpleProcessCountingBuilder _instance = new SimpleProcessCountingBuilder();

        public ISimpleBuilder Instance
        {
            get { return _instance; }
        }

        public IEnumerable<BuiltComponents> Build(System.Configuration.Configuration configuration)
        {
            var schedules = new List<BuiltComponents>();

            var simpleProcessCounterConfiguration = configuration.GetSection("simpleProcessCountingSources") as SimpleProcessCountingConfiguration;

            if (simpleProcessCounterConfiguration != null)
            {
                foreach (SimpleProcessElement config in simpleProcessCounterConfiguration.Processes)
                {
                    schedules.Add(Build(config));
                }
            }

            return schedules;
        }

        public static BuiltComponents Build(SimpleProcessElement simpleConfig)
        {
            var counterElement = new ProcessElement(simpleConfig);
            var sourceConfig = new ProcessCountingSourceConfiguration(new[] { counterElement });
            var source = ProcessCountingSourceBuilder.Build(sourceConfig);

            return SimpleComponentBuilder.BuildStandardSinkSet(simpleConfig, source);
        }
    }
}