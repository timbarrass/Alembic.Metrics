using System.Collections.Generic;

namespace Coordination
{
    public interface ISchedule
    {
        string Name { get; }

        IEnumerable<Chain> Chains { get; }
    }
}