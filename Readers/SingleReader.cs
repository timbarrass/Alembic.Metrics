using System.Collections.Generic;
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
    public class SingleReader<T> : ISnapshotProvider<T> where T : IMetricData
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SingleReader<T>).Name);
        
        private IDataStore<T> _store;
        
        private string _directory;

        public SingleReader(string directory, IDataStore<T> store)
        {
            _directory = directory;

            _store = store;
        }

        public IEnumerable<T> Snapshot(string label)
        {
            var path = Path.Combine(_directory, label);

            Log.Debug("Reading from " + path);

            return _store.Read(path);
        }
    }
}
