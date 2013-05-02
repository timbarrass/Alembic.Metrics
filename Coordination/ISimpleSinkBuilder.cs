using System.Collections.Generic;
using Data;

namespace Coordination
{
    public interface ISimpleSinkBuilder
    {
        ISimpleSinkBuilder Instance { get; }

        IEnumerable<ISchedule> Build(System.Configuration.Configuration configuration, IEnumerable<ISnapshotProvider> sources);     
    }
}