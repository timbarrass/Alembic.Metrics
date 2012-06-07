using System.Collections.Generic;
using Data;

namespace Sinks
{
    public interface IDataSink<T>
    {
        void Update(string specName, IEnumerable<T> perfMetricData);

        void Write();

        // hack -- temp
        void Plot(string specName);

        void Plot();
    }
}