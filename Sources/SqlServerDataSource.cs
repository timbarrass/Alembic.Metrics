using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Data;

namespace Sources
{
    /// <summary>
    /// Primarily intended as an example sql data source. More conrete classes should
    /// be implemented for each specific source.
    /// 
    /// Best to wrap in a builder, and have the builder construct sources suitable for
    /// specific purposes.
    /// </summary>
    public class SqlServerDataSource : IDataSource
    {
        private DataContext _context;

        private string _query;

        private ICollection<MetricSpecification> _spec;

        public SqlServerDataSource(DataContext context, IEnumerable<MetricSpecification> spec, string query)
        {
            _context = context;

            _spec = spec.ToArray();

            _query = query;
        }

        public ICollection<MetricSpecification> Spec
        {
            get { return _spec; }
        }

        // want this one to return a list of IMetricData
        public IEnumerable<IMetricData> Query()
        {
            var timeseries = _context.ExecuteQuery<TimeseriesPoint>(_query);

            var returnSeries = new List<MetricData>();

            foreach(var point in timeseries)
            {
                var metricData = new MetricData(new Dictionary<string, double?> {{ "SqlServer", (double?)point.Value }}, point.Timestamp);
                
                returnSeries.Add(metricData);
            }

            return returnSeries;
        }
    }

    /// <summary>
    /// Coupled to SqlServerDataSource specifically as a type safe way of handling
    /// returned metrics
    /// </summary>
    public class TimeseriesPoint
    {
        public DateTime Timestamp;

        public double Value;
    }
}