using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public struct MetricData
    {
        public MetricData(double? value, DateTime timestamp)
        {
            _data = new List<double?> { value };
            _timestamp = timestamp;
        }

        public MetricData(List<double?> value, DateTime timestamp)
        {
            _data = value;
            _timestamp = timestamp;
        }

        public List<double?> Data { get { return _data; } }

        public DateTime Timestamp { get { return _timestamp; } }

        private readonly List<double?> _data;

        private readonly DateTime _timestamp;
    }
}