using System.Collections.Generic;
using System.Linq;

namespace Coordination
{
    public class ScheduleBuilder
    {
        public static IEnumerable<ISchedule> Build(ScheduleConfiguration configs, IEnumerable<Chain> chains)
        {
            var elements = configs.Links.Cast<ScheduleElement>().ToList();

            return Build(elements, chains);
        }

        public static IEnumerable<ISchedule> Build(IEnumerable<ScheduleElement> configs, IEnumerable<Chain> chains)
        {
            var schedules = new List<ISchedule>();

            foreach(var config in configs)
            {
                schedules.Add(new Schedule(config, chains));                
            }

            return schedules;
        }

        public static ISchedule Build(string scheduleName, int delay, IEnumerable<Chain> chains)
        {
            return new Schedule(scheduleName, delay, chains);
        }
    }
}