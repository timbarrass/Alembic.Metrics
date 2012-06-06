using System;
using System.Collections.Generic;
using Data;

namespace Sources
{
    public class NullSource : IDataSource
    {
        public static readonly MetricSpecification NullSpecification = null; // should be non-updateable

        public MetricSpecification Spec
        {
            get { return NullSpecification; }
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
