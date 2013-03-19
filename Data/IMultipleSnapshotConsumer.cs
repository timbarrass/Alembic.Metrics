using System.Collections.Generic;

namespace Data
{
    public interface IMultipleSnapshotConsumer
    {
        string Name { get; }

        void ResetWith(IEnumerable<Snapshot> snapshot);

        void Update(IEnumerable<Snapshot> snapshot);
    }
}