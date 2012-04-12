using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Data;

namespace Sinks
{
    public class CircularDataSink : IDataSink
    {
        private int _pointsToKeep = 10;

        private SlidingBuffer<IMetricData> _data;

        private ICollection<MetricSpecification> _spec;

        public CircularDataSink(int pointsToKeep, ICollection<MetricSpecification> spec)
        {
            _pointsToKeep = pointsToKeep;

            _data = new SlidingBuffer<IMetricData>(_pointsToKeep);

            _spec = spec;
        }

        public void Update(IMetricData perfMetricData)
        {
            _data.Add(perfMetricData);
        }

        public void Plot()
        {
            var xvals = _data.Select(x => x.Timestamp).ToArray<DateTime>();

            if (xvals.Length != 0)
            {
                foreach (var spec in _spec)
                {
                    var name = spec.Name;

                    var yvals = _data.Select(y => y.Values[name]).ToArray<double?>();

                    GenerateChart(xvals, yvals, spec.ExpectedMin, spec.ExpectedMax, name);
                }
            }
        }

        private void GenerateChart(DateTime[] xvals, double?[] yvals, double? min, double? max, string chartName)
        {
            var chart = new Chart();
            chart.Size = new Size(400, 200);
            chart.AntiAliasing = AntiAliasingStyles.None;

            var chartArea = new ChartArea();
            chartArea.AxisX.LabelStyle.Format = "dd/MMM\nhh:mm";
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
            series.Name = "Series1";
            series.ChartType = SeriesChartType.FastLine;
            series.XValueType = ChartValueType.DateTime;
            chart.Series.Add(series);

            // bind the datapoints
            chart.Series["Series1"].Points.DataBindXY(xvals, yvals);

            // copy the series and manipulate the copy
            //chart.DataManipulator.CopySeriesValues("Series1", "Series2");
            //chart.DataManipulator.FinancialFormula(FinancialFormula.WeightedMovingAverage, "Series2");
            //chart.Series["Series2"].ChartType = SeriesChartType.FastLine;

            // draw!
            chart.Invalidate();

            // write out a file
            chart.SaveImage(Path.ChangeExtension(chartName, "png"), ChartImageFormat.Png);
        }

        private class SlidingBuffer<T> : IEnumerable<T>
        {
            private readonly Queue<T> _queue;
            private readonly int _maxCount;

            public SlidingBuffer(int maxCount)
            {
                _maxCount = maxCount;
                _queue = new Queue<T>(maxCount);
            }

            public void Add(T item)
            {
                if (_queue.Count == _maxCount)
                    _queue.Dequeue();
                _queue.Enqueue(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _queue.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

    }
}