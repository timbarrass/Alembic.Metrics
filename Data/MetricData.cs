using System;

namespace Data
{
    [Serializable]
    public struct MetricData
    {
        public MetricData(double? value, DateTime timestamp)
        {
            _data = value;
            _timestamp = timestamp;
        }

        public double? Data { get { return _data; } }

        public DateTime Timestamp { get { return _timestamp; } }

        private readonly double? _data;

        private readonly DateTime _timestamp;
    }
}