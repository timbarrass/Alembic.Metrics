using System;
using System.Collections.Generic;

namespace Data
{
    public class MetricData : IMetricData
    {
        public MetricData(Dictionary<string, double?> values, DateTime timestamp)
        {
            Values = values;
            Timestamp = timestamp;
        }

        public Dictionary<string, double?> Values { get; private set; }

        public DateTime Timestamp { get; private set; }

        public virtual ICollection<MetricSpecification> Spec
        {
            get { throw new NotImplementedException(); }
            private set { }
        }
    }
}