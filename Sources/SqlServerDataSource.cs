using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using Configuration;
using Data;
using log4net;

namespace Sources
{
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

        private DataContext _context;

        private string _query;

        public string Name
        {
            get { return _name; }
        }

        public SqlServerDataSource(DatabaseElement config)
        {
            var conn = new SqlConnection
                {
                    ConnectionString = config.ConnectionString
                };

            var context = new DataContext(conn);

            Initialise(config.Name, context, config.Query);
        }

        public SqlServerDataSource(string id, string name, DataContext context, string query)
        {
            Initialise(name, context, query);
        }

        private void Initialise(string name, DataContext context, string query)
        {
            _context = context;

            _name = name;

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
                timeseries = new List<TimeseriesPoint>() { new TimeseriesPoint { Timestamp = DateTime.Now, Value = 0.0 } };
            }
            
            var queryEnd = DateTime.Now;
            var queryDuration = new TimeSpan(queryEnd.Ticks - start.Ticks).TotalMilliseconds;

            var returnSeries = new Snapshot { Name = Name };

            foreach(var point in timeseries)
            {
                var metricData = new MetricData( point.Value, point.Timestamp);
                
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

        private string _name;
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