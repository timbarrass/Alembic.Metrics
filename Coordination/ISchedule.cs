using System.Collections.Generic;
using System.Threading;

namespace Coordination
{
    public interface ISchedule
    {
        string Name { get; }

        IEnumerable<IChain> Chains { get; }
        
        void Start(CancellationToken token);

        void RunOnce();
    }
}