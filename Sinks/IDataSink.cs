using System.Collections.Generic;
using Data;

namespace Sinks
{
    public interface IDataSink<T>
    {
        void Update(string specName, IEnumerable<T> perfMetricData);
    }
}