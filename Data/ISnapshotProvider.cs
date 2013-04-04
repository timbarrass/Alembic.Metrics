using System;

namespace Data
{
    public interface ISnapshotProvider : ISnapshotHandler
    {
        Snapshot Snapshot();

        Snapshot Snapshot(DateTime cutoff);
    }
}