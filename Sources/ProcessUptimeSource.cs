using System;
using System.Collections.Generic;
using System.Diagnostics;
using Configuration;
using Data;
using log4net;

namespace Sources
{
    public class ProcessUptimeSource : ISnapshotProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProcessUptimeSource).Name);

        private readonly string _processToMonitor;

        private readonly string _machineName;

        public string Name { get; private set; }

        public string Id { get; private set; }

        public ProcessUptimeSource(ProcessElement config)
            : this(config.Id, config.Name, config.Exe, config.MachineName)
        {
        }

        public ProcessUptimeSource(string id, string processUptimeFriendlyName, string processToMonitor, string machine)
        {
            Id = id;

            _processToMonitor = processToMonitor;
;
            Name = processUptimeFriendlyName;

            _machineName = machine;
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