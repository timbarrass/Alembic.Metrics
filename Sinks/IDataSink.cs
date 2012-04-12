using Data;

namespace Sinks
{
    public interface IDataSink
    {
        void Update(IMetricData perfMetricData);

        // hack -- temp
        void Plot();
    }
}