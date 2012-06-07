using System;
using System.Collections.Generic;

namespace Data
{
    public interface IMetricData
    {
        double? Data { get; }
        DateTime Timestamp { get; }
    }
}