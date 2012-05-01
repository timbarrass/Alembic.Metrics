using System;
using System.Collections.Generic;

namespace Data
{
    public class NullMetricData : IMetricData
    {
        public static readonly Dictionary<string, double?> EmptyValueMap = new Dictionary<string, double?>();

        public Dictionary<string, double?> Values
        {
            get { return EmptyValueMap; }
        }

        public DateTime Timestamp
        {
            get { return DateTime.Now; }
        }
    }
}
