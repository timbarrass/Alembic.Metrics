using System;

namespace Data
{
    public class NullMetricData : IMetricData
    {
        public double? Data
        {
            get { return null; }
        }

        public DateTime Timestamp
        {
            get { return DateTime.Now; }
        }
    }
}
