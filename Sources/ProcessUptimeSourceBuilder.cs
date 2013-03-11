using System.Collections.Generic;
using Data;

namespace Sources
{
    public class ProcessUptimeSourceBuilder
    {
        public static IEnumerable<ISnapshotProvider> Build(ProcessUptimeSourceConfiguration processUptimeSourceConfiguration)
        {
            var processUptimeSources = new List<ISnapshotProvider>();

            foreach (ProcessElement config in processUptimeSourceConfiguration.Processes)
            {
                processUptimeSources.Add(new ProcessUptimeSource(config));
            }

            return processUptimeSources;
        }
    }
}