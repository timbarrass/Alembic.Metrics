using System.Collections.Generic;
using Configuration;
using Data;

namespace Sources
{
    public class PerformanceCounterDataSourceBuilder
    {
        public static IEnumerable<ISnapshotProvider> Build(PerformanceCounterDataSourceConfiguration performanceCounterSourceConfiguration)
        {
            var performanceCounterDataSinks = new List<ISnapshotProvider>();

            foreach (CounterElement config in performanceCounterSourceConfiguration.Counters)
            {
                performanceCounterDataSinks.Add(new PerformanceCounterDataSource(config));
            }

            return performanceCounterDataSinks;
        }
    }
}