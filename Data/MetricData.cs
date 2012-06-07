using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class MetricData : IMetricData
    {
        public MetricData(double? values, DateTime timestamp)
        {
            Data = values;
            Timestamp = timestamp;
        }

        public double? Data { get; private set; }

        public DateTime Timestamp { get; private set; }

        public virtual ICollection<MetricSpecification> Spec
        {
            get { throw new NotImplementedException(); }
            private set { }
        }
    }
}