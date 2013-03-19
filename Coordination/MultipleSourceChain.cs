using System.Collections.Generic;
using Data;
using log4net;

namespace Coordination
{
    public class MultipleSourceChain : IChain
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MultipleSourceChain).Name);

        public MultipleSourceChain(string name, IMultipleSnapshotConsumer sink, params ISnapshotProvider[] sources)
        {
            Name = name;

            _sources = sources;

            _sink = sink;
        }

        public string Name { get; private set; }

        public void Update()
        {
            Log.Debug(string.Format("Updating chain '{0}'", Name));

            var snapshots = new List<Snapshot>();

            foreach (var source in _sources)
            {
                snapshots.Add(source.Snapshot());
            }

            _sink.Update(snapshots);
        }

        private readonly IEnumerable<ISnapshotProvider> _sources;

        private readonly IMultipleSnapshotConsumer _sink;
    }
}