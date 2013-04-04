using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configuration;
using Data;

namespace Sinks
{
    public class CircularDataSink : ISnapshotProvider, ISnapshotConsumer
    {
        private readonly object _padlock = new object();

        private readonly int _pointsToKeep = 10;

        private SlidingBuffer<MetricData> _data;

        private DateTime _lastUpdate = new DateTime();

        private readonly string _name;

        private readonly string _id;

        public CircularDataSink(ISinkConfiguration config)
        {
            _id = config.Id;

            _pointsToKeep = config.Points;

            _data = CreateSlidingBuffer();

            _lastUpdate = DateTime.MinValue;

            _name = config.Name;
        }

        public string Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        private SlidingBuffer<MetricData> CreateSlidingBuffer()
        {
            return new SlidingBuffer<MetricData>(_pointsToKeep);
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
                var snapshot = new Snapshot { Name = Name };
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