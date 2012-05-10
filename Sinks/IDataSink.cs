using System.Collections.Generic;
using Data;

namespace Sinks
{
    public interface IDataSink<T>
    {
        void Update(IEnumerable<T> perfMetricData);

        // hack -- temp
        void Plot();
    }

    public interface IDataSink
    {
        void Update(IEnumerable<IMetricData> perfMetricData);

        // hack -- temp
        void Plot();
    }
}