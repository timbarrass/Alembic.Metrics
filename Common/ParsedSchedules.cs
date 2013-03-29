using System.Collections.Generic;
using Coordination;

namespace Common
{
    public struct ParsedSchedules
    {
        public IEnumerable<ISchedule> Schedules;

        public IEnumerable<ISchedule> PreloadSchedules;
    }
}