using System.Collections.Generic;
using Data;
using log4net;

namespace Coordination
{
    public class Chain
    {
        private readonly string _name;

        private readonly ISnapshotProvider _source;

        private readonly IEnumerable<ISnapshotConsumer> _sinks;

        private static readonly ILog Log = LogManager.GetLogger(typeof(Chain).Name);

        public Chain(string name, ISnapshotProvider source, params ISnapshotConsumer[] sinks)
        {
            _name = name;

            _source = source;

            _sinks = sinks;
        }

        public void Update()
        {
            Log.Debug(string.Format("Updating chain '{0}'", _name));

            var snapshot = _source.Snapshot();

            foreach(var sink in _sinks)
            {
                sink.Update(snapshot);
            }
        }
    }
}