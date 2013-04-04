using System;
using System.Diagnostics;
using Configuration;
using Data;
using log4net;

namespace Sources
{
    public class ProcessCountingSource : ISnapshotProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProcessCountingSource).Name);

        private readonly string _processToMonitor;

        private readonly string _processCountName;

        private readonly string _machineName = Environment.MachineName;

        private readonly string _id;

        public string Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _processCountName; }
        }

        public ProcessCountingSource(ProcessElement config)
            : this(config.Id, config.Name, config.Exe, config.MachineName)
        {
        }

        public ProcessCountingSource(string id, string processCountFriendlyName, string processToMonitor, string machine)
        {
            _id = id;

            _processToMonitor = processToMonitor;

            _processCountName = processCountFriendlyName;

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

            var snapshot = new Snapshot { Name = Name };
            snapshot.Add(new MetricData( processes.Length, DateTime.Now));

            return snapshot;
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            throw new NotImplementedException();
        }
    }
}