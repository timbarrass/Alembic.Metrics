using System.Collections.Generic;
using System.Linq;

namespace Coordination
{
    public class Schedule : ISchedule
    {
        public Schedule(ScheduleElement config, IEnumerable<Chain> chains)
        {
            Delay = config.Delay;

            Name = config.Name;

            var chainNames = config.Chains.Split(',');

            Chains = chains.Where(c => chainNames.Contains(c.Name)).ToArray();
        }

        public Schedule(string name, int delay, IEnumerable<Chain> chains)
        {
            Delay = delay;

            Name = name;

            Chains = chains.ToArray();
        }

        public string Name { get; private set; }

        public IEnumerable<Chain> Chains { get; private set; }

        public int Delay { get; private set; }
    }
}