using Data;

namespace Sinks
{
    public interface IDataSink<T>
    {
        void Update(T perfMetricData);

        // hack -- temp
        void Plot();
    }

    public interface IDataSink
    {
        void Update(IMetricData perfMetricData);

        // hack -- temp
        void Plot();
    }
}