using System.Collections.Generic;
using Data;

namespace Coordination
{
    public class ScheduleBuilder
    {
        public static ISchedule Build(string scheduleName, int delay, IEnumerable<Chain> chains)
        {
            return new Schedule(scheduleName, delay, chains);
        }
    }
}