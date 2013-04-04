using System.Collections.Generic;

namespace Data
{
    public interface IMultipleSnapshotConsumer : ISnapshotHandler
    {
        void ResetWith(IEnumerable<Snapshot> snapshot);

        void Update(IEnumerable<Snapshot> snapshot);
    }
}