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

        private string _directory;

        public SingleWriter(string outputDirectory, ISnapshotProvider<IMetricData> snapshotProvider, MetricSpecification spec, IDataStore<IMetricData> store )
        {
            _snapshotProvider = snapshotProvider;

            _spec = spec;

            _store = store;

            _directory = outputDirectory;
        }

        public void Write()
        {
            var snapshot = _snapshotProvider.Snapshot(_spec.Name);

            var path = System.IO.Path.Combine(_directory, _spec.Name);

            _store.Write(path, snapshot);
        }
    }
}