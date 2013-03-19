using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Data;

namespace Sinks
{
    public class MultiPlotter : IMultipleSnapshotConsumer
    {
        public MultiPlotter(string name, string outputPath, float? expectedMin, float? expectedMax, float scale)
        {
            Name = name;

            _directory = outputPath;

            _min = expectedMin;

            _max = expectedMax;

            _scale = scale;

            _fontCollection = new PrivateFontCollection();

            _fontCollection.AddFontFile("Apple ][.ttf");
        }

        public MultiPlotter(PlotterElement config)
            : this(config.Name, config.OutputDirectory, config.Min, config.Max, config.Scale)
        {
        }

        public string Name { get; private set; }

        public void ResetWith(IEnumerable<Snapshot> snapshots)
        {
            Update(snapshots);
        }

        public void Update(IEnumerable<Snapshot> snapshots)
        {
            var chartName = Environment.MachineName + ": " + Name;
            var chart = InitializeChart(chartName);

            foreach (var snapshot in snapshots)
            {
                Plot(chart, snapshot);
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

        private void Plot(Chart chart, Snapshot snapshot)
        {
            DateTime[] xvals;
            double?[] yvals = new double?[0];

            xvals = snapshot.Select(x => x.Timestamp).ToArray();

            if (xvals.Length != 0)
            {
                yvals = snapshot.Select(y => y.Data * _scale).ToArray();
            }

            AddSeriesToChart(chart, xvals, yvals, _min, _max, snapshot.Name);
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

        private Chart InitializeChart(string chartName)
        {
            var titleFont = new Font(
                _fontCollection.Families[0].Name,
                8,
                FontStyle.Regular,
                GraphicsUnit.Pixel);

            var labelFont = new Font(
                _fontCollection.Families[0].Name,
                7,
                FontStyle.Regular,
                GraphicsUnit.Pixel);

            var chart = new Chart();
            chart.Size = new Size(400, 200);
            chart.AntiAliasing = AntiAliasingStyles.None;
            chart.Titles.Add(new Title(chartName, Docking.Top, titleFont, Color.Black));

            var chartArea = new ChartArea();
            chartArea.AxisX.LabelStyle.Format = "dd/MMM\nHH:mm";
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.LabelStyle.Font = labelFont;
            chartArea.AxisY.LabelStyle.Font = labelFont;
            chartArea.IsSameFontSizeForAllAxes = true;

            chartArea.AxisX.LabelAutoFitMaxFontSize = 7;
            chartArea.AxisY.LabelAutoFitMaxFontSize = 7;
            chart.ChartAreas.Add(chartArea);

            return chart;
        }

        private readonly string _directory;

        private PrivateFontCollection _fontCollection;

        private readonly float? _min;

        private readonly float? _max;

        private readonly float _scale;
    }
}