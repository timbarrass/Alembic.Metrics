using System.IO;
using Data;
using Sinks;
using Stores;
using log4net;

namespace Readers
{
    /// <summary>
    /// Reader is plossibly a poor name for this class, as is Writer for SingleWriter. Each forms a
    /// link between a Sink and a persistent medium.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleReader<T>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SingleReader<T>).Name);
        
        private IDataStore<IMetricData> _store;
        
        private string _directory;

        private MetricSpecification _spec;

        private ISnapshotConsumer<IMetricData> _consumer;

        public SingleReader(string directory, ISnapshotConsumer<IMetricData> consumer,  MetricSpecification spec, IDataStore<IMetricData> store)
        {
            _directory = directory;

            _store = store;

            _consumer = consumer;

            _spec = spec;
        }

        public void Read()
        {
            var path = Path.Combine(_directory, _spec.Name);

            var snapshot = _store.Read(path);

            _consumer.ResetWith(snapshot, _spec.Name);
        }
    }
}
