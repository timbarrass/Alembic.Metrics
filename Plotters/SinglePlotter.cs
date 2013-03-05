using System;
using System.Drawing;
using System.Drawing.Text;
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
    public class SinglePlotter<T> : IDataPlotter
    {
        public SinglePlotter(string outputDirectory, ISnapshotProvider snapshotProvider, MetricSpecification spec)
        {
            _snapshotProvider = snapshotProvider;

            _spec = spec;
            
            _directory = outputDirectory;

            _fontCollection = new PrivateFontCollection();

            _fontCollection.AddFontFile("Apple ][.ttf");
        }

        public void Plot()
        {
            Plot(_snapshotProvider);
        }

        private void Plot(ISnapshotProvider snapshotProvider)
        {
            var spec = snapshotProvider.Spec;

            var snapshot = snapshotProvider.Snapshot();

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
            var path = Path.Combine(_directory, chartName.Replace(":", "-"));
            path = Path.ChangeExtension(path, "png");
            chart.SaveImage(path, ChartImageFormat.Png);
        }

        private ISnapshotProvider _snapshotProvider;

        private MetricSpecification _spec;

        private string _directory;

        private PrivateFontCollection _fontCollection;
    }
}