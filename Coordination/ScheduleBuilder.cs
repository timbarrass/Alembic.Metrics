using System.Collections.Generic;
using System.Linq;
using Configuration;
using log4net;

namespace Coordination
{
    public class ScheduleBuilder
    {
        public static IEnumerable<ISchedule> Build(ScheduleConfiguration configs, IEnumerable<IChain> chains)
        {
            var elements = configs.Links.Cast<ScheduleElement>().ToList();

            return Build(elements, chains);
        }

        public static IEnumerable<ISchedule> Build(IEnumerable<ScheduleElement> configs, IEnumerable<IChain> chains)
        {
            var schedules = new List<ISchedule>();

            foreach(var config in configs)
            {
                schedules.Add(new Schedule(config, chains));

                Log.Info(string.Format("Created schedule '{0}'", config.Name));
            }

            return schedules;
        }

        public static ISchedule Build(string scheduleName, int delay, IEnumerable<IChain> chains)
        {
            return new Schedule(scheduleName, delay, chains);
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(ScheduleBuilder).Name);
    }
}