using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public struct MetricData
    {
        public MetricData(double? value, DateTime timestamp, IList<string> labels)
        {
            Data = new List<double?> { value };

            Timestamp = timestamp;

            Labels = labels;
        }

        public MetricData(List<double?> value, DateTime timestamp, IList<string> labels)
        {
            Data = value;

            Timestamp = timestamp;

            Labels = labels;
        }

        public List<double?> Data;

        public DateTime Timestamp;

        public IList<string> Labels;
    }
}