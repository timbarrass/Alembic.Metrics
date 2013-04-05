using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public struct MetricData
    {
        public MetricData(double? value, DateTime timestamp, IList<string> labels)
        {
            _data = new List<double?> { value };

            _timestamp = timestamp;

            _labels = labels;
        }

        public MetricData(List<double?> value, DateTime timestamp, IList<string> labels)
        {
            _data = value;

            _timestamp = timestamp;

            _labels = labels;
        }

        public List<double?> Data { get { return _data; } }

        public DateTime Timestamp { get { return _timestamp; } }

        public IList<string> Labels { get { return _labels; } } 

        private readonly List<double?> _data;

        private readonly DateTime _timestamp;

        private readonly IList<string> _labels;
    }
}