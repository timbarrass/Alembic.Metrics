using System;
using System.Collections.Generic;
using Data;

namespace Sources
{
    public class CompositeSource : IDataSource
    {
        private List<IDataSource> _sources = new List<IDataSource>();

        public CompositeSource(params IDataSource[] sources)
        {
            _sources.AddRange(sources);
        }

        public ICollection<MetricSpecification> Spec
        {
            get 
            {
                List<MetricSpecification> spec = new List<MetricSpecification>();

                foreach(var source in _sources)
                {
                    spec.AddRange(source.Spec);
                }

                return spec;
            }
        }

        public IMetricData Query()
        {
            var values = new Dictionary<string, double?>();

            foreach(var source in _sources)
            {
                var sourceValues = source.Query();

                foreach(var spec in source.Spec)
                {
                    values[spec.Name] = sourceValues.Values[spec.Name];
                }
            }

            return new MetricData(values, DateTime.Now);
        }
    }
}
