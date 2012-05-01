using System;
using System.Collections.Generic;
using Data;

namespace Sources
{
    public class NullSource : IDataSource
    {
        public static readonly List<MetricSpecification> EmptySpecList = new List<MetricSpecification>(); // should be non-updateable

        public ICollection<MetricSpecification> Spec
        {
            get { return EmptySpecList; }
        }

        public IMetricData Query()
        {
            return new NullMetricData();
        }
    }
}
