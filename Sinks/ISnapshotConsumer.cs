using System.Collections.Generic;
using Data;

namespace Sinks
{
    public interface ISnapshotConsumer<in T> where T : IMetricData
    {
        void ResetWith(IEnumerable<T> snapshot, string label);
    }
}