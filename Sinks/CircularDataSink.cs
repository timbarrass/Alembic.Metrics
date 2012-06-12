﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Data;
using Stores;

namespace Sinks
{
    public class CircularDataSink<T> : IDataSink<T>, ISnapshotProvider<T> where T : IMetricData
    {
        private object _padlock = new object();

        private int _pointsToKeep = 10;

        private Dictionary<string, SlidingBuffer<T>> _data = new Dictionary<string, SlidingBuffer<T>>();

        private ICollection<MetricSpecification> _sourceSpecifications;

        private IDataStore<T> _store;

        /// <summary>
        /// Each sink is configured to accept data from multiple sources (represented by spec) but will
        /// remember the same number of points for each.
        /// </summary>
        public CircularDataSink(int pointsToKeep, ICollection<MetricSpecification> sourceSpecifications)
        {
            _pointsToKeep = pointsToKeep;

            foreach (var spec in sourceSpecifications)
            {
                _data[spec.Name] = new SlidingBuffer<T>(_pointsToKeep);
            }

            _sourceSpecifications = sourceSpecifications;
        }

        public CircularDataSink(int pointsToKeep, ICollection<MetricSpecification> sourceSpecifications, IDataStore<T> store) : this(pointsToKeep, sourceSpecifications)
        {
            _store = store;
        }

        public ICollection<MetricSpecification> Spec
        {
            get { return _sourceSpecifications; }
        }

        public void Update(string specName, IEnumerable<T> perfMetricData)
        {
            lock (_padlock)
            {
                foreach (var metric in perfMetricData)
                {
                    _data[specName].Add(metric);
                }
            }
        }


        public void Write()
        {
            lock (_padlock)
            {
                foreach (var specName in _data.Keys)
                {
                    _store.Write(specName, _data[specName]);
                }
            }
        }

        public IEnumerable<T> Snapshot(string label)
        {
            lock(_padlock)
            {
                return _data[label].ToArray(); // want a deep copy, not a reference
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