using System;
using Data;

namespace Sources
{
    public class NullSource : ISnapshotProvider
    {
        public Snapshot Snapshot()
        {
            return new Snapshot();
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            return new Snapshot();
        }

        public string Name
        {
            get { return "NullSource"; }
        }

        public string Id
        {
            get { return Name; }
        }
    }
}
