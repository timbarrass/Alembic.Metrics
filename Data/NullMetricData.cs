using System;

namespace Data
{
    public struct NullMetricData
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
