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

        private int _delay;

        public int Delay
        {
            get
            {
                return _delay;
            }
        }

        public string Name
        {
            get { return _spec.First().Name; }
        }

        public SqlServerDataSource(DataContext context, IEnumerable<MetricSpecification> spec, string query, int delay)
        {
            _context = context;

            _spec = spec.ToArray();

            _query = query;

            _delay = delay * 1000;
        }

        public ICollection<MetricSpecification> Spec
        {
            get { return _spec; }
        }

        // want this one to return a list of IMetricData
        public IEnumerable<IMetricData> Query()
        {
            var start = DateTime.Now;

            var timeseries = _context.ExecuteQuery<TimeseriesPoint>(_query);

            var queryEnd = DateTime.Now;
            var queryDuration = new TimeSpan(queryEnd.Ticks - start.Ticks).TotalMilliseconds;

            var returnSeries = new List<MetricData>();

            foreach(var point in timeseries)
            {
                var metricData = new MetricData(new Dictionary<string, double?> {{ "SqlServer", (double?)point.Value }}, point.Timestamp);
                
                returnSeries.Add(metricData);
            }

            var processEnd = DateTime.Now;
            var processDuration = new TimeSpan(processEnd.Ticks - queryEnd.Ticks).TotalMilliseconds;
            var totalDuration = new TimeSpan(processEnd.Ticks - start.Ticks).TotalMilliseconds;

            Console.WriteLine("Query started {0}, ended {1} [{2}] {3} [{4}] [{5}]", start, queryEnd, queryDuration, processEnd, processDuration, totalDuration);

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