using System.Collections.Generic;

namespace Sinks
{
    public interface ISnapshotProvider<T>
    {
        IEnumerable<T> Snapshot(string label);
    }
}