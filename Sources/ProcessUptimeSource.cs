﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Sources
{
    public class ProcessUptimeSource : IDataSource
    {
        private MetricSpecification _spec;

        private string _processToMonitor;

        private string _processUptimeName;

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
            get { return _processToMonitor + "-metrics"; }
        }

        public string Id
        {
            get { return _id; }
        }

        public ProcessUptimeSource(string id, string processUptimeFriendlyName, string processToMonitor, int delay)
        {
            _processToMonitor = processToMonitor;
;
            _processUptimeName = processUptimeFriendlyName;

            _delay = delay * 1000;

            _id = id;

            _spec = new MetricSpecification(_processUptimeName, 0, null);
        }

        public MetricSpecification Spec
        {
            get { return _spec; }
        }

        public IEnumerable<IMetricData> Query()
        {
            var processes = Process.GetProcessesByName(_processToMonitor);

            var count = 0;
            var uptime = 0d;
            foreach(var process in processes)
            {
                try
                {
                    uptime += new TimeSpan(DateTime.Now.Ticks - process.StartTime.Ticks).TotalSeconds;
                }
                catch(InvalidOperationException ex)
                {
                    // _assume_ this is because the process has gone away between getting the process 
                    // list and making the query. Ignore.
                }

                count++;
            }

            return new List<IMetricData> { new MetricData( count == 0 ? 0 : uptime / count, DateTime.Now) };
        }
    }
}