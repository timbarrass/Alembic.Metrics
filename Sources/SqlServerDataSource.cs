using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using Configuration;
using Data;
using log4net;

namespace Sources
{
    public class DataContextWrapper
    {
        private readonly DataContext _context;

        public DataContextWrapper()
        {
            _context = null;
        }

        public DataContextWrapper(DataContext context)
        {
            _context = context;
        }

        public virtual IEnumerable<T> ExecuteQuery<T>(string query)
        {
            if (_context != null) return _context.ExecuteQuery<T>(query);

            return null;
        }
    }

    /// <summary>
    /// Primarily intended as an example sql data source. More concrete classes should
    /// be implemented for each specific source.
    /// 
    /// Best to wrap in a builder, and have the builder construct sources suitable for
    /// specific purposes.
    /// </summary>
    public class SqlServerDataSource : ISnapshotProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SqlServerDataSource).Name);

        private DataContextWrapper _context;

        private string _query;

        public string Name { get; private set; }

        public string Id { get; private set; }

        public IList<string> Labels { get; private set; } 

        public SqlServerDataSource(DatabaseElement config)
        {
            var conn = new SqlConnection
                {
                    ConnectionString = config.ConnectionString
                };

            var context = new DataContextWrapper(new DataContext(conn));

            Initialise(config.Id, config.Name, context, config.Query, new List<string>());
        }

        public SqlServerDataSource(string id, string name, DataContextWrapper context, string query)
        {
            Initialise(id, name, context, query, new List<string>());
        }

        public SqlServerDataSource(string id, string name, DataContextWrapper context, string query, IList<string> labels)
        {
            Initialise(id, name, context, query, labels);
        }

        private void Initialise(string id, string name, DataContextWrapper context, string query, IList<string> labels)
        {
            _context = context;

            Name = name;

            Id = id;

            Labels = labels;

            _query = query;
        }

        public Snapshot Snapshot()
        {
            Log.Debug("Querying " + Name);

            var start = DateTime.Now;

            IEnumerable<TimeseriesPoint> timeseries;

            try
            {
                timeseries = _context.ExecuteQuery<TimeseriesPoint>(_query);
            }
            catch (SqlException ex)
            {
                Log.Warn(Name + ": SqlException thrown: " + ex.Message);
                timeseries = new List<TimeseriesPoint>() { new TimeseriesPoint { Timestamp = DateTime.Now, Value1 = 0.0 } };
            }
            
            var queryEnd = DateTime.Now;
            var queryDuration = new TimeSpan(queryEnd.Ticks - start.Ticks).TotalMilliseconds;

            var returnSeries = new Snapshot { Name = Name, Labels = Labels };

            foreach(var point in timeseries)
            {
                var values = new[] {point.Value1, point.Value2, point.Value3, point.Value4, point.Value5}.ToList();

                var metricData = new MetricData(values, point.Timestamp);
                
                returnSeries.Add(metricData);
            }

            var processEnd = DateTime.Now;
            var processDuration = new TimeSpan(processEnd.Ticks - queryEnd.Ticks).TotalMilliseconds;
            var totalDuration = new TimeSpan(processEnd.Ticks - start.Ticks).TotalMilliseconds;

            var perfLogMessage = string.Format("Query started {0}, ended {1} [{2}] {3} [{4}] [{5}]", start, queryEnd, queryDuration, processEnd, processDuration, totalDuration);
            Log.Debug(perfLogMessage);

            return returnSeries;
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Coupled to SqlServerDataSource specifically as a type safe way of handling
    /// returned metrics
    /// </summary>
    public class TimeseriesPoint
    {
        public DateTime Timestamp;

        public double? Value1;

        public double? Value2;

        public double? Value3;

        public double? Value4;

        public double? Value5;
    }
}