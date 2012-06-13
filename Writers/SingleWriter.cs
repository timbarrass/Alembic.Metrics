using Data;
using Sinks;
using Stores;

namespace Writers
{
    public class SingleWriter<T> : IDataWriter
    {
        private ISnapshotProvider<IMetricData> _snapshotProvider;

        private MetricSpecification _spec;

        private IDataStore<IMetricData> _store;

        public SingleWriter(ISnapshotProvider<IMetricData> snapshotProvider, MetricSpecification spec, IDataStore<IMetricData> store )
        {
            _snapshotProvider = snapshotProvider;

            _spec = spec;

            _store = store;
        }

        public void Write()
        {
            var snapshot = _snapshotProvider.Snapshot(_spec.Name);

            _store.Write(_spec.Name, snapshot);
        }
    }
}