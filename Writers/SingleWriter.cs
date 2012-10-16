using System;
using System.IO;
using Data;
using Sinks;
using Stores;
using log4net;

namespace Writers
{
    public class SingleWriter<T> : IDataWriter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SingleWriter<T>).Name);

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

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);

                Log.Info("Writer for '" + spec.Name + "' created output directory '" + outputDirectory + "'.");
            } 
        }

        public void Write()
        {
            var snapshot = _snapshotProvider.Snapshot(_spec.Name);

            var path = Path.Combine(_directory, _spec.Name);

            _store.Write(path, snapshot);
        }
    }
}