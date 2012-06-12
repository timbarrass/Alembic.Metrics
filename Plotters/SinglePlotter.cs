using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Data;
using Sinks;

namespace Plotters
{
    /// <summary>
    /// Intent here is to have the agent configured with a set of plotters, each of
    /// which is configured with a set of sinks mapped to a set of specs to request
    /// for each sink.
    /// </summary>
    public class SinglePlotter<T> : IDataPlotter where T : IMetricData
    {
        public double Total { get; set; }

        public SinglePlotter(ISnapshotProvider<T> snapshotProvider, MetricSpecification spec)
        {
            _snapshotProvider = snapshotProvider;
            _spec = spec;
        }

        public void Plot()
        {
            Plot(_snapshotProvider, _spec);
        }

        private void Plot(ISnapshotProvider<T> snapshotProvider, MetricSpecification spec)
        {
            var snapshot = snapshotProvider.Snapshot(spec.Name);

            DateTime[] xvals;
            double?[] yvals = new double?[0];

            xvals = snapshot.Select(x => x.Timestamp).ToArray();

            if (xvals.Length != 0)
            {
                yvals = snapshot.Select(y => y.Data).ToArray();
            }

            GenerateChart(xvals, yvals, spec.ExpectedMin, spec.ExpectedMax, spec.Name);
        }

        private void GenerateChart(DateTime[] xvals, double?[] yvals, double? min, double? max, string chartName)
        {
            chartName = Environment.MachineName + ": " + chartName;

            var chart = new Chart();
            chart.Size = new Size(400, 200);
            chart.AntiAliasing = AntiAliasingStyles.None;
            chart.Titles.Add(new Title(chartName, Docking.Top, new Font("Tahoma", 8), Color.Black));

            var chartArea = new ChartArea();
            chartArea.AxisX.LabelStyle.Format = "dd/MMM\nHH:mm";
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.LabelStyle.Font = new Font("Tahoma", 7);
            chartArea.AxisY.LabelStyle.Font = new Font("Tahoma", 7);
            chartArea.IsSameFontSizeForAllAxes = true;

            if (min.HasValue)
            {
                if (min.Value < (double?)Decimal.MinValue) min = (double?)Decimal.MinValue + 1;
                chartArea.AxisY.Minimum = min.Value;
            }
            if (max.HasValue)
            {
                if (max.Value > (double?)Decimal.MaxValue) max = (double?)Decimal.MaxValue - 1;
                chartArea.AxisY.Maximum = max.Value;
            }

            chartArea.AxisX.LabelAutoFitMaxFontSize = 7;
            chartArea.AxisY.LabelAutoFitMaxFontSize = 7;
            chart.ChartAreas.Add(chartArea);

            var series = new Series();
            series.Name = chartName;
            series.ChartType = SeriesChartType.FastLine;
            series.XValueType = ChartValueType.DateTime;
            chart.Series.Add(series);

            // bind the datapoints
            chart.Series[chartName].Points.DataBindXY(xvals, yvals);

            // copy the series and manipulate the copy
            //chart.DataManipulator.CopySeriesValues("Series1", "Series2");
            //chart.DataManipulator.FinancialFormula(FinancialFormula.WeightedMovingAverage, "Series2");
            //chart.Series["Series2"].ChartType = SeriesChartType.FastLine;

            // draw!
            chart.Invalidate();

            // write out a file
            chart.SaveImage(Path.ChangeExtension(chartName.Replace(":", "-"), "png"), ChartImageFormat.Png);
        }

        private ISnapshotProvider<T> _snapshotProvider;

        private MetricSpecification _spec;
    }
}