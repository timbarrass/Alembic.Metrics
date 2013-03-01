using System.Threading;
using System.Collections.Generic;

using Data;
using Sources;
using Sinks;

namespace MetricAgent
{
    class Processor
    {
        public IDataSource Source { get; set; }

        public IList<IDataSink<IMetricData>> Sinks { get; set; }

        public int Delay { get; set; }

        public void Stop()
        {
            lock (_cancellationLock)
            {
                _cancel.Set();
            }

            _worker.Join();
        }

        public void Start()
        {
            _worker = new Thread(Process);
            _worker.IsBackground = true;
            _worker.Start();
        }

        private bool WaitUnlessCancelled(int duration)
        {
            return !_cancel.WaitOne(duration);
        }

        private void Process()
        {
            while (WaitUnlessCancelled(Delay))
            {
                Update(Source, Sinks);
            }
        }

        private void Update(IDataSource source, IList<IDataSink<IMetricData>> sinks)
        {
            IEnumerable<IMetricData> metricData;

            if (!source.TryQuery(out metricData)) return;

            foreach (var sink in sinks)
            {
                sink.Update(source.Spec.Name, metricData);
            }
        }

        private object _cancellationLock = new object();
        private Thread _worker;
        private EventWaitHandle _cancel = new EventWaitHandle(false, EventResetMode.AutoReset);
    }
}