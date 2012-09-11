using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Data;
using Sinks;

namespace Plotters
{
    public class MultiPlotter<T> : IDataPlotter where T : IMetricData
    {
        public MultiPlotter(ISnapshotProvider<T> snapshotProvider, MetricSpecification[] specs, string name, string outputPath)
        {
            _snapshotProvider = snapshotProvider;

            _specs = specs;

            _name = name;

            _directory = outputPath;
        }

        public MultiPlotter(ISnapshotProvider<T> snapshotProvider, MetricSpecification[] specs, string name)
            : this(snapshotProvider, specs, name, ".")
        {
        }

        public void Plot()
        {
            var chartName = Environment.MachineName + ": " + _name;
            var chart = InitializeChart(chartName);
            
            foreach(var spec in _specs)
            {
                Plot(chart, _snapshotProvider, spec);
            }

            RenderChart(chart, chartName);
        }

        private void RenderChart(Chart chart, string chartName)
        {
            chart.Invalidate();

            var path = Path.Combine(_directory, chartName.Replace(":", "-"));
            path = Path.ChangeExtension(path, "png");
            chart.SaveImage(path, ChartImageFormat.Png);
        }

        private void Plot(Chart chart, ISnapshotProvider<T> snapshotProvider, MetricSpecification spec)
        {
            var snapshot = snapshotProvider.Snapshot(spec.Name);

            DateTime[] xvals;
            double?[] yvals = new double?[0];

            xvals = snapshot.Select(x => x.Timestamp).ToArray();

            if (xvals.Length != 0)
            {
                yvals = snapshot.Select(y => y.Data).ToArray();
            }

            AddSeriesToChart(chart, xvals, yvals, spec.ExpectedMin, spec.ExpectedMax, spec.Name);
        }

        private void AddSeriesToChart(Chart chart, DateTime[] xvals, double?[] yvals, double? min, double? max, string chartName)
        {
            if (min.HasValue)
            {
                if (min.Value < (double?)Decimal.MinValue) min = (double?)Decimal.MinValue + 1;
                chart.ChartAreas[0].AxisY.Minimum = min.Value;
            }
            if (max.HasValue)
            {
                if (max.Value > (double?)Decimal.MaxValue) max = (double?)Decimal.MaxValue - 1;
                chart.ChartAreas[0].AxisY.Maximum = max.Value;
            }

            var series = new Series();
            series.Name = chartName;
            series.ChartType = SeriesChartType.FastLine;
            series.XValueType = ChartValueType.DateTime;
            chart.Series.Add(series);

            chart.Series[chartName].Points.DataBindXY(xvals, yvals);
        }

        private static Chart InitializeChart(string chartName)
        {
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

            chartArea.AxisX.LabelAutoFitMaxFontSize = 7;
            chartArea.AxisY.LabelAutoFitMaxFontSize = 7;
            chart.ChartAreas.Add(chartArea);
            return chart;
        }

        private readonly MetricSpecification[] _specs;

        private readonly ISnapshotProvider<T> _snapshotProvider;

        private readonly string _name;

        private readonly string _directory;
    }
}