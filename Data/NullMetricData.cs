using System;
using System.Collections.Generic;

namespace Data
{
    public struct NullMetricData
    {
        public List<double?> Data
        {
            get { return new List<double?>(); }
        }

        public DateTime Timestamp
        {
            get { return DateTime.Now; }
        }
    }
}
