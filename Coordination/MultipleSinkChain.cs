using System.Collections.Generic;
using Data;
using log4net;

namespace Coordination
{
    public class MultipleSinkChain : IChain
    {
        private readonly ISnapshotProvider _source;

        private readonly IEnumerable<ISnapshotConsumer> _sinks; 

        private static readonly ILog Log = LogManager.GetLogger(typeof(MultipleSinkChain).Name);

        public MultipleSinkChain(string id, string name, ISnapshotProvider source, params ISnapshotConsumer[] sinks)
        {
            Name = name;

            Id = id;

            _source = source;

            _sinks = sinks;
        }

        public string Name { get; private set; }

        public string Id { get; private set; }

        public void Update()
        {
            Log.Debug(string.Format("Updating chain '{0}'", Id));

            var snapshot = _source.Snapshot();

            foreach(var sink in _sinks)
            {
                sink.Update(snapshot);
            }
        }
    }
}