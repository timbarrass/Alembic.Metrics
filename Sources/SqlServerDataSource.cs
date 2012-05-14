using System;
using System.Collections.Generic;
using System.Data.Linq;
using Data;

namespace Sources
{
    /// <summary>
    /// Primarily intended as an example sql data source. More conrete classes should
    /// be implemented for each specific source
    /// </summary>
    public class SqlServerDataSource : IDataSource
    {
        private DataContext _context;

        private ICollection<MetricSpecification> _spec;

        public SqlServerDataSource(string connectionString)
        {
            _context = new DataContext(connectionString);

            _spec = new[]
                        {
                            new MetricSpecification("SqlServer", null, null)
                        };
        }

        public ICollection<MetricSpecification> Spec
        {
            get { return _spec; }
        }

        // want this one to return a list of IMetricData
        public IEnumerable<IMetricData> Query()
        {
            var timeseries = _context.ExecuteQuery<TimeseriesPoint>("select * from ExampleData");

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

        public decimal Value;
    }
}