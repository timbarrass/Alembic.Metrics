using System.Collections.Generic;
using Data;

namespace Sources
{
    public class ProcessCountingSourceBuilder
    {
        public static IEnumerable<ISnapshotProvider> Build(ProcessCountingSourceConfiguration processCountingSourceConfiguration)
        {
            var processCountingSources = new List<ISnapshotProvider>();

            foreach (ProcessElement config in processCountingSourceConfiguration.Processes)
            {
                processCountingSources.Add(new ProcessCountingSource(config));
            }

            return processCountingSources;
        }
    }
}