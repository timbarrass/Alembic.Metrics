using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Configuration;
using Data;
using Sinks;

namespace Coordination
{
    [Export(typeof(ISimpleSinkBuilder))]
    public class SimplePlotterBuilder : ISimpleSinkBuilder
    {
        private static readonly ISimpleSinkBuilder _instance = new SimplePlotterBuilder();

        public ISimpleSinkBuilder Instance { get { return _instance; } }

        public IEnumerable<ISchedule> Build(System.Configuration.Configuration configuration, IEnumerable<ISnapshotProvider> sources)
        {
            var simplePlotterConfiguration = configuration.GetSection("simplePlotters") as SimplePlotterConfiguration;

            var schedules = new List<ISchedule>();

            if (simplePlotterConfiguration != null)
            {
                var copiedSources = sources;

                foreach (SimplePlotterElement config in simplePlotterConfiguration.Plotters)
                {
                    schedules.Add(Build(config, copiedSources));
                }

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
                sourceStringBuilder.Append(source).Append(" Buffer,");
            }
            var configSources = sourceStringBuilder.ToString().TrimEnd(',');

            var chainConfig = new ChainElement(config.Id + " Chain", config.Name, configSources, "", config.Id + " Plotter");

            var chains = ChainBuilder.Build(sources, new List<ISnapshotConsumer>(), sinks, new [] { chainConfig });

            var schedule = ScheduleBuilder.Build(config.Name, config.Delay, chains);

            return schedule;
        }
    }
}