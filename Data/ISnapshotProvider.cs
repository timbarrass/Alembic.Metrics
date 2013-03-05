using System;
using System.Collections.Generic;

namespace Data
{
    public interface ISnapshotProvider
    {
        MetricSpecification Spec { get; }

        Snapshot Snapshot();

        Snapshot Snapshot(DateTime cutoff);
    }
}