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

        private MetricSpecification _spec;

        private int _delay;

        private string _id;

        public int Delay
        {
            get
            {
                return _delay;
            }
        }

        public string Name
        {
            get { return _spec.Name; }
        }

        public string Id
        {
            get { return _id; }
        }

        public SqlServerDataSource(string id, DataContext context, MetricSpecification spec, string query, int delay)
        {
            _id = id;

            _context = context;

            _spec = spec;

            _query = query;

            _delay = delay * 1000;
        }

        public MetricSpecification Spec
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
                var metricData = new MetricData( point.Value, point.Timestamp);
                
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