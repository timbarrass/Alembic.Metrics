using System.Collections.Generic;
using Configuration;
using Data;

namespace Coordination
{
    public struct BuiltComponents
    {
        public BuiltComponents(
            IEnumerable<ISnapshotProvider> sources,
            IEnumerable<ISnapshotConsumer> sinks,
            IEnumerable<IMultipleSnapshotConsumer> multisinks,
            ChainConfiguration chains,
            ScheduleConfiguration preloadSchedules,
            ScheduleConfiguration schedules )
        {
            Sources = sources;

            Sinks = sinks;

            Multisinks = multisinks;

            Chains = chains;

            PreloadSchedules = preloadSchedules;

            Schedules = schedules;
        }

        public IEnumerable<ISnapshotProvider> Sources;

        public IEnumerable<ISnapshotConsumer> Sinks;

        public IEnumerable<IMultipleSnapshotConsumer> Multisinks;

        public ChainConfiguration Chains;

        public ScheduleConfiguration PreloadSchedules;

        public ScheduleConfiguration Schedules;
    }
}