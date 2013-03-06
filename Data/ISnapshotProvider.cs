using System;

namespace Data
{
    public interface ISnapshotProvider
    {
        MetricSpecification Spec { get; }

        string Name { get; }

        Snapshot Snapshot();

        Snapshot Snapshot(DateTime cutoff);
    }
}