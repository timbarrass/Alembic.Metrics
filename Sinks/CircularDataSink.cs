using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace Sinks
{
    public class CircularDataSink : ISnapshotProvider, ISnapshotConsumer
    {
        private object _padlock = new object();

        private int _pointsToKeep = 10;

        private SlidingBuffer<MetricData> _data;

        private MetricSpecification _sourceSpecification;

        private DateTime _lastUpdate = new DateTime();

        /// <summary>
        /// Each sink is configured to accept data from multiple sources (represented by spec) but will
        /// remember the same number of points for each.
        /// </summary>
        public CircularDataSink(int pointsToKeep, MetricSpecification sourceSpecification)
        {
            _pointsToKeep = pointsToKeep;

            _data = CreateSlidingBuffer();

            _lastUpdate = DateTime.MinValue;

            _sourceSpecification = sourceSpecification;
        }

        private SlidingBuffer<MetricData> CreateSlidingBuffer()
        {
            return new SlidingBuffer<MetricData>(_pointsToKeep);
        }

        public MetricSpecification Spec
        {
            get { return _sourceSpecification; }
        }

        public void ResetWith(Snapshot snapshot)
        {
            lock(_padlock)
            {
                _data = CreateSlidingBuffer();

                Update(snapshot);
            }
        }

        public void Update(Snapshot snapshot)
        {
            lock (_padlock)
            {
                foreach (var metric in snapshot)
                {
                    if (metric.Timestamp > _lastUpdate)
                    {
                        _data.Add(metric);

                        _lastUpdate = metric.Timestamp;
                    }
                }
            }
        }

        public Snapshot Snapshot()
        {
            lock(_padlock)
            {
                var snapshot = new Snapshot();
                snapshot.AddRange(_data.ToArray()); // want a deep copy, not a reference

                return snapshot;
            }
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            lock (_padlock)
            {
                var snapshot = new Snapshot();
                snapshot.AddRange(_data.Where(d => d.Timestamp.Ticks >= cutoff.Ticks).ToArray());

                return snapshot;
            }
        }

        [Serializable]
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

                if (value > 1000)
                {
                    return (value / 1000).ToString() + "k";
                }

                return value.ToString();
            }
        }
    }
}