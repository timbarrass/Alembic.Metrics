using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;
using log4net;

namespace Sources
{
    public class ProcessUptimeSource : ISnapshotProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProcessUptimeSource).Name);

        private MetricSpecification _spec;

        private string _processToMonitor;

        private string _processUptimeName;

        private string _id;

        private string _machineName;

        public string Name
        {
            get { return _processUptimeName; }
        }

        public string Id
        {
            get { return _id; }
        }

        public ProcessUptimeSource(ProcessElement config)
            : this(config.Id, config.Name, config.Exe, config.MachineName)
        {
        }

        public ProcessUptimeSource(string id, string processUptimeFriendlyName, string processToMonitor, string machine)
        {
            _processToMonitor = processToMonitor;
;
            _processUptimeName = processUptimeFriendlyName;

            _id = id;

            _machineName = machine;

            _spec = new MetricSpecification(_processUptimeName, 0, null);
        }

        public MetricSpecification Spec
        {
            get { return _spec; }
        }

        public Snapshot Snapshot()
        {
            Log.Debug("Querying " + Name);

            Process[] processes;

            if (string.IsNullOrEmpty(_machineName))
            {
                processes = Process.GetProcessesByName(_processToMonitor);
            }
            else
            {
                processes = Process.GetProcessesByName(_processToMonitor, _machineName);
            }
            var count = 0;
            var uptime = 0d;
            foreach(var process in processes)
            {
                try
                {
                    uptime += new TimeSpan(DateTime.Now.Ticks - process.StartTime.Ticks).TotalSeconds;
                }
                catch(InvalidOperationException)
                {
                    // _assume_ this is because the process has gone away between getting the process 
                    // list and making the query. Ignore.
                }

                count++;
            }

            var snapshot = new Snapshot { Name = Name };
            snapshot.Add(new MetricData( count == 0 ? 0 : uptime / count, DateTime.Now));

            return snapshot;
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            throw new NotImplementedException();
        }
    }
}