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
        private object _padlock = new object();

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
            lock (_padlock)
            {
                _data.Add(perfMetricData);
            }
        }

        public void Plot()
        {
            DateTime[] xvals;
            var yvals = new Dictionary<MetricSpecification, double?[]>();

            lock (_padlock)
            {
                xvals = _data.Select(x => x.Timestamp).ToArray<DateTime>();

                if (xvals.Length != 0)
                {
                    foreach (var spec in _spec)
                    {
                        yvals[spec] = _data.Select(y => y.Values[spec.Name]).ToArray<double?>();
                    }
                }
            }

            foreach(var spec in yvals.Keys)
            {
                GenerateChart(xvals, yvals[spec], spec.ExpectedMin, spec.ExpectedMax, spec.Name);
            }
        }

        private void GenerateChart(DateTime[] xvals, double?[] yvals, double? min, double? max, string chartName)
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


        private class KiloFormatter : ICustomFormatter, IFormatProvider
        {
            public object GetFormat(Type formatType)
            {
                return (formatType == typeof(ICustomFormatter)) ? this : null;
            }

            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (format == null || !format.Trim().StartsWith("K"))
                {
                    if (arg is IFormattable)
                    {
                        return ((IFormattable)arg).ToString(format, formatProvider);
                    }
                    return arg.ToString();
                }

                decimal value = Convert.ToDecimal(arg);

                //  Here's is where you format your number

                if (value > 1000)
                {
                    return (value / 1000).ToString() + "k";
                }

                return value.ToString();
            }
        }
    }
}