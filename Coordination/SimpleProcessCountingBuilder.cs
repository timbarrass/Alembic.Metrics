using System;
using System.Collections.Generic;
using Configuration;
using Data;
using Sinks;
using Sources;

namespace Coordination
{
    public class SimpleProcessCountingBuilder
    {
        public static IEnumerable<BuiltComponents> Build(SimpleProcessCountingConfiguration configs)
        {
            var schedules = new List<BuiltComponents>();

            foreach(SimpleProcessElement config in configs.Processes)
            {
                schedules.Add(Build(config));
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