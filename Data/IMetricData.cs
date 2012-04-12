using System;
using System.Collections.Generic;

namespace Data
{
    public interface IMetricData
    {
        Dictionary<string, double?> Values { get; }
        DateTime Timestamp { get; }
    }
}