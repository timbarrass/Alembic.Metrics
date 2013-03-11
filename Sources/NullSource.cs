using System;
using Data;

namespace Sources
{
    public class NullSource : ISnapshotProvider
    {
        public static readonly MetricSpecification NullSpecification = new MetricSpecification(); // should be non-updateable

        public MetricSpecification Spec
        {
            get { return NullSpecification; }
        }

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
