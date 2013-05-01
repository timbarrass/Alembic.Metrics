using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Configuration;
using Data;

namespace Sinks
{
    public class MultiPlotter : IMultipleSnapshotConsumer
    {
        public MultiPlotter(string id, string name, string outputPath, float? expectedMin, float? expectedMax, double scale, string areas)
        {
            Id = id;

            Name = name;

            _directory = outputPath;

            _min = expectedMin;

            _max = expectedMax;

            _scale = scale;

            _areas = areas;
        }

        public MultiPlotter(PlotterElement config)
            : this(config.Id, config.Name, config.OutputDirectory, config.Min, config.Max, config.Scale, config.Areas)
        {
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public void ResetWith(IEnumerable<Snapshot> snapshots)
        {
            Update(snapshots);
        }

        public void Update(IEnumerable<Snapshot> snapshots)
        {
            var areas = _areas.Split(',').ToList();
            
            var smallChart = InitializeSmallChart(Name);
            var largeChart = InitializeLargeChart(Name);

            var index = 0;
            foreach (var snapshot in snapshots)
            {
                var area = "";

                if (areas.Count > 1)
                {
                    area = areas[index];
                }

                Plot(smallChart, snapshot, area);
                Plot(largeChart, snapshot, area);

                index++;
            }

            RenderChart(smallChart, string.Join("-", Name, "small"));
            RenderChart(largeChart, Name);
        }


        private void RenderChart(Chart chart, string chartName)
        {
            chart.Invalidate();

            var path = Path.Combine(_directory, chartName.Replace(":", "-"));
            path = Path.ChangeExtension(path, "png");
            chart.SaveImage(path, ChartImageFormat.Png);
        }

        private void Plot(Chart chart, Snapshot snapshot, string area)
        {
            var xvals = snapshot.Select(x => x.Timestamp).ToArray();

            var yvals = new double?[0];

            if (xvals.Length != 0)
            {
                for (int i = 0; i < snapshot[0].Data.Count(); i++)
                {
                    yvals = snapshot.Select(y => y.Data[i].HasValue ? new double?(y.Data[i].Value*_scale) : null).ToArray();

                    AddSeriesToChart(chart, xvals, yvals, _min, _max, snapshot[0].Labels[i], area);
                }
            }
        }

        private void AddSeriesToChart(Chart chart, DateTime[] xvals, double?[] yvals, float? min, float? max, string seriesName, string area)
        {
            if (min.HasValue)
            {
                if (min.Value < (float?)Decimal.MinValue) min = (float?)Decimal.MinValue + 1;
                chart.ChartAreas[0].AxisY.Minimum = min.Value;
            }
            if (max.HasValue)
            {
                if (max.Value > (float?)Decimal.MaxValue) max = (float?)Decimal.MaxValue - 1;
                chart.ChartAreas[0].AxisY.Maximum = max.Value;
            }

            var series = new Series();
            series.Name = seriesName;
            if (!string.IsNullOrEmpty(area)) series.ChartArea = area;
            series.ChartType = SeriesChartType.FastLine;
            series.XValueType = ChartValueType.DateTime;
            chart.Series.Add(series);

            chart.Series[seriesName].Points.DataBindXY(xvals, yvals);
        }

        private Chart InitializeLargeChart(string chartName)
        {
            var titleFont = new Font(
                "Tahoma",
                16,
                FontStyle.Bold,
                GraphicsUnit.Pixel);

            var labelFont = new Font(
                "Tahoma",
                14,
                FontStyle.Regular,
                GraphicsUnit.Pixel);

            var chart = new Chart();
            
            chart.AntiAliasing = AntiAliasingStyles.None;
            chart.Titles.Add(new Title(chartName, Docking.Top, titleFont, Color.Black));

            var chartAreas = _areas.Split(',').Distinct();

            chart.Size = new Size(1200, 400 + ((chartAreas.Count() - 1) * 360));

            var bigArea = 100 * ((float)400 / (float)chart.Size.Height);
            var smallArea = bigArea * 0.9f;

            int index = 0;
            foreach (var area in chartAreas)
            {
                var chartArea = new ChartArea();
                chartArea.Name = area;
                chartArea.AxisX.LabelStyle.Format = "dd/MMM\nHH:mm";
                chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
                chartArea.AxisX.LabelStyle.Font = labelFont;
                chartArea.AxisY.LabelStyle.Font = labelFont;
                chartArea.IsSameFontSizeForAllAxes = true;

                chartArea.BorderColor = Color.Black;
                chartArea.BorderWidth = 1;
                chartArea.BorderDashStyle = ChartDashStyle.Solid;

                if (area != chartAreas.Last())
                {
                    chartArea.AxisX.LabelStyle = new LabelStyle { Enabled = false };
                    chartArea.AlignWithChartArea = chartAreas.Last();
                    chartArea.Position = new ElementPosition(0, smallArea * index, 100, smallArea);
                }
                else
                {
                    chartArea.Position = new ElementPosition(0, smallArea * index, 100, bigArea);
                }

                chart.ChartAreas.Add(chartArea);

                chart.Legends.Add(new Legend(area));

                index++;
            }

            return chart;
        }

        private Chart InitializeSmallChart(string chartName)
        {
            var titleFont = new Font(
                "Tahoma",
                12,
                FontStyle.Bold,
                GraphicsUnit.Pixel);

            var labelFont = new Font(
                "Tahoma",
                7,
                FontStyle.Regular,
                GraphicsUnit.Pixel);

            var chart = new Chart();
            chart.Size = new Size(300, 150);
            chart.AntiAliasing = AntiAliasingStyles.None;
            chart.Titles.Add(new Title(chartName, Docking.Top, titleFont, Color.Black));

            var chartAreas = _areas.Split(',').Distinct();

            chart.Size = new Size(300, 150 + ((chartAreas.Count() - 1) * 135));

            var bigArea = 100 * ((float)150 / (float)chart.Size.Height);
            var smallArea = bigArea * 0.9f;

            int index = 0;
            foreach (var area in chartAreas)
            {
                var chartArea = new ChartArea();
                chartArea.Name = area;
                chartArea.AxisX.LabelStyle.Format = "dd/MMM\nHH:mm";
                chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
                chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
                chartArea.AxisX.LabelStyle.Font = labelFont;
                chartArea.AxisY.LabelStyle.Font = labelFont;
                chartArea.IsSameFontSizeForAllAxes = true;

                chartArea.BorderColor = Color.Black;
                chartArea.BorderWidth = 1;
                chartArea.BorderDashStyle = ChartDashStyle.Solid;

                if (area != chartAreas.Last())
                {
                    chartArea.AxisX.LabelStyle = new LabelStyle { Enabled = false };
                    chartArea.AlignWithChartArea = chartAreas.Last();
                    chartArea.Position = new ElementPosition(0, smallArea * index, 100, smallArea);
                }
                else
                {
                    chartArea.Position = new ElementPosition(0, smallArea * index, 100, bigArea);
                }

                chart.ChartAreas.Add(chartArea);

                index++;
            }

            return chart;
        }

        private readonly string _directory;

        private readonly float? _min;

        private readonly float? _max;

        private readonly double _scale;

        private readonly string _areas;
    }
}