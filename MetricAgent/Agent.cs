using System.Collections.Generic;
using System.Threading;
using Data;
using Sinks;
using Sources;
using Plotters;
using Writers;

namespace MetricAgent
{
    public class Agent
    {
        private EventWaitHandle _cancellation = new EventWaitHandle(false, EventResetMode.ManualReset);

        private IList<IDataPlotter> _plotters; 

        private object _padlock = new object(); // locks out cancel message -- rename

        private int _loopDelay = 5000;

        private List<Processor> _workers = new List<Processor>();

        private Thread _plotter;

        private IDictionary<IDataSource, IList<IDataSink<IMetricData>>> _sinksToUpdate;

        private IList<IDataWriter> _writers;

        public Agent(IDictionary<IDataSource, IList<IDataSink<IMetricData>>> sinksToUpdate, IList<IDataPlotter> plotters, IList<IDataWriter> writers, int plotterDelay)
        {
            _sinksToUpdate = sinksToUpdate;

            _plotters = plotters;

            _writers = writers;

            _loopDelay = plotterDelay * 1000;
        }

        internal IEnumerable<IMetricData> Query(IDataSource source)
        {
            return source.Query();
        }

        private bool WaitUnlessCancelled(int duration)
        {
            var ret = _cancellation.WaitOne(duration);
            return !ret;
        }

        private void Graph()
        {
            while (WaitUnlessCancelled(_loopDelay * 10))
            {
                foreach(var plotter in _plotters) 
                {
                    plotter.Plot();
                }

                foreach (var writer in _writers)
                {
                    writer.Write();
                }
            }
        }

        public void Start()
        {
            lock(_padlock)
            {
                foreach (var source in _sinksToUpdate.Keys)
                {
                    var processor = new Processor() { Source = source, Sinks = _sinksToUpdate[source], Delay = source.Delay };
                    processor.Start();
                    _workers.Add(processor);
                }

                _plotter = new Thread(Graph);
                _plotter.IsBackground = true;
                _plotter.Start();
            }
        }

        public void Stop()
        {
            lock(_padlock)
            {
                _cancellation.Set();

                foreach (var worker in _workers)
                {
                    worker.Stop();
                }
            }

            _cancellation.Set();
            _plotter.Join();
        }
    }

    class Processor
    {
        private EventWaitHandle _cancellation = new EventWaitHandle(false, EventResetMode.AutoReset);

        public IDataSource Source { get; set; }

        public IList<IDataSink<IMetricData>> Sinks { get; set; }

        public int Delay { get; set; }

        private Thread _worker;

        public void Stop()
        {
            lock(_padlock)
            {
                _cancellation.Set();
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
            return !_cancellation.WaitOne(duration);
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
            var metricData = source.Query();

            foreach (var sink in sinks)
            {
                sink.Update(source.Spec.Name, metricData);
            }
        }

        private object _padlock = new object();
    }
}