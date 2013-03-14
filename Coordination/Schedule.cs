using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Coordination
{
    public class Schedule : ISchedule
    {
        public Schedule(ScheduleElement config, IEnumerable<Chain> chains)
        {
            Delay = config.Delay * 1000;

            Name = config.Name;

            var chainNames = config.Chains.Split(',');

            Chains = chains.Where(c => chainNames.Contains(c.Name)).ToArray();
        }

        public Schedule(string name, int delay, IEnumerable<Chain> chains)
        {
            Delay = delay * 1000;

            Name = name;

            Chains = chains.ToArray();
        }

        public void Start(CancellationToken cancellationToken)
        {
            while (! cancellationToken.IsCancellationRequested)
            {
                RunOnce();

                var duration = 0.0;
                while (duration < Delay && ! cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(500);

                    duration += 500;
                }
            }
        }

        public string Name { get; private set; }

        public IEnumerable<Chain> Chains { get; private set; }

        public int Delay { get; private set; }

        public void RunOnce()
        {
            foreach (var chain in Chains)
            {
                chain.Update();
            }
        }
    }
}