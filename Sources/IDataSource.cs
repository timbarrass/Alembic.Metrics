﻿using System.Collections.Generic;
using Data;

namespace Sources
{
    public interface IDataSource
    {
        ICollection<MetricSpecification> Spec { get; }
        IMetricData Query();
    }
}