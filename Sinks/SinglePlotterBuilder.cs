using System.Collections.Generic;
using Data;

namespace Sinks
{
    public class SinglePlotterBuilder
    {
        public static IEnumerable<ISnapshotConsumer> Build(PlotterConfiguration configs)
        {
            var plotters = new List<ISnapshotConsumer>();

            foreach(PlotterElement config in configs.Plotters)
            {
                plotters.Add(new SinglePlotter(config));
            }

            return plotters;
        }
    }
}