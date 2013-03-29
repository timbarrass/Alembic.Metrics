using System.Collections.Generic;
using Configuration;
using Data;

namespace Sinks
{
    public class MultiPlotterBuilder
    {
        public static IEnumerable<IMultipleSnapshotConsumer> Build(PlotterConfiguration configs)
        {
            var plotters = new List<IMultipleSnapshotConsumer>();

            foreach (PlotterElement config in configs.Plotters)
            {
                plotters.Add(new MultiPlotter(config));
            }

            return plotters;
        }
    }
}