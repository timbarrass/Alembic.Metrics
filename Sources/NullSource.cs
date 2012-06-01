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

        public IEnumerable<IMetricData >Query()
        {
            return new List<NullMetricData>();
        }

        public int Delay
        {
            get { return 1000; }
        }

        public string Name
        {
            get { return "NullSource"; }
        }
    }
}
