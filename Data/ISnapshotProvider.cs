using System;

namespace Data
{
    public interface ISnapshotProvider
    {
        string Name { get; }

        Snapshot Snapshot();

        Snapshot Snapshot(DateTime cutoff);
    }
}