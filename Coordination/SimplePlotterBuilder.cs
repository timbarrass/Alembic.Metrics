using System.Collections.Generic;
using System.Text;
using Configuration;
using Data;
using Sinks;

namespace Coordination
{
    public class SimplePlotterBuilder
    {
        public static IEnumerable<ISchedule> Build(SimplePlotterConfiguration configs, IEnumerable<ISnapshotProvider> sources)
        {
            var copiedSources = sources;

            var schedules = new List<ISchedule>();

            foreach(SimplePlotterElement config in configs.Plotters)
            {
                schedules.Add(Build(config, copiedSources));
            }

            return schedules;
        }

        public static ISchedule Build(SimplePlotterElement config, IEnumerable<ISnapshotProvider> sources)
        {
            var plotterConfig = new PlotterConfiguration(new [] { config });

            var sinks = MultiPlotterBuilder.Build(plotterConfig);

            var sourceStringBuilder = new StringBuilder();
            foreach(var source in config.Sources.Split(','))
            {
                sourceStringBuilder.Append(source).Append(" Source,");
            }
            var configSources = sourceStringBuilder.ToString().TrimEnd(',');

            var chainConfig = new ChainElement(config.Id + " Chain", config.Name, configSources, "", config.Id + " Plotter");

            var chains = ChainBuilder.Build(sources, new List<ISnapshotConsumer>(), sinks, new [] { chainConfig });

            var schedule = ScheduleBuilder.Build(config.Name, config.Delay, chains);

            return schedule;
        }
    }
}