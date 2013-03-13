using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Data;

namespace Sinks
{
    public class SinglePlotter : ISnapshotConsumer
    {
        public SinglePlotter(PlotterElement config)
            : this(config.OutputDirectory, config.Min, config.Max, config.Name, config.Scale)
        {
        }

        public SinglePlotter(string outputDirectory, float expectedMin, float expectedMax, string name, float scale)
        {
            _directory = outputDirectory;

            _min = expectedMin;

            _max = expectedMax;

            _name = name;

            _scale = scale;

            _fontCollection = new PrivateFontCollection();

            _fontCollection.AddFontFile("Apple ][.ttf");
        }

        public SinglePlotter(string outputDirectory, MetricSpecification spec)
        {
            if(spec.ExpectedMin.HasValue)
                _min = spec.ExpectedMin.Value;

            if(spec.ExpectedMax.HasValue)
                _max = spec.ExpectedMax.Value;

            _name = spec.Name;
            
            _directory = outputDirectory;

            _fontCollection = new PrivateFontCollection();

            _fontCollection.AddFontFile("Apple ][.ttf");
        }

        private void Plot(Snapshot snapshot)
        {
            DateTime[] xvals;
            double?[] yvals = new double?[0];

            xvals = snapshot.Select(x => x.Timestamp).ToArray();

            if (xvals.Length != 0)
            {
                yvals = snapshot.Select(y => y.Data * _scale).ToArray();
            }

            GenerateChart(xvals, yvals, _min, _max, _name);
        }

        private void GenerateChart(DateTime[] xvals, double?[] yvals, double? min, double? max, string chartName)
        {
            chartName = Environment.MachineName + ": " + chartName;

            var titleFont = new Font(
              "Consolas",
              12,
              FontStyle.Regular,
              GraphicsUnit.Pixel);

            var labelFont = new Font(
              "Consolas",
              10,
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

            chartArea.AxisX.LabelAutoFitMaxFontSize = 10;
            chartArea.AxisY.LabelAutoFitMaxFontSize = 10;
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
            var path = Path.Combine(_directory, chartName.Replace(":", "-"));
            path = Path.ChangeExtension(path, "png");
            chart.SaveImage(path, ChartImageFormat.Png);
        }

        private readonly float _scale = 1f;

        private readonly float _min;

        private readonly float _max;

        private readonly string _name;

        private readonly string _directory;

        private readonly PrivateFontCollection _fontCollection;
        
        public string Name
        {
            get { return _name; }
        }

        public void ResetWith(Snapshot snapshot)
        {
            Plot(snapshot);
        }

        public void Update(Snapshot snapshot)
        {
            Plot(snapshot);
        }
    }
}