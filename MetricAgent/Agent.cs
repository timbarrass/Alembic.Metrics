using System.Collections.Generic;
using System.Linq;
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
        private IEnumerable<IDataSink<IMetricData>> _sinks;

        private IList<IDataPlotter> _plotters; 

        private object _padlock = new object(); // locks out cancel message -- rename

        private bool _cancelled = false;

        private int _loopDelay = 5000;

        private List<Processor> _workers = new List<Processor>();

        private Thread _plotter;

        private IDictionary<IDataSource, IList<IDataSink<IMetricData>>> _sinksToUpdate;

        private IList<IDataWriter> _writers;

        public Agent(IDictionary<IDataSource, IList<IDataSink<IMetricData>>> sinksToUpdate, IList<IDataPlotter> plotters, IList<IDataWriter> writers, int plotterDelay)
        {
            _sinksToUpdate = sinksToUpdate;

            var sinks = new List<IDataSink<IMetricData>>();

            foreach(var list in _sinksToUpdate.Values)
            {
                foreach (var sink in list)
                {
                    sinks.Add(sink);
                }
            }

            _sinks = sinks.Distinct();

            _plotters = plotters;

            _writers = writers;

            _loopDelay = plotterDelay * 1000;
        }

        internal IEnumerable<IMetricData> Query(IDataSource source)
        {
            return source.Query();
        }

        private void Graph()
        {
            // hack -- have the sink be able to plot for now as it has all the right refs
            while (true)
            {
                Thread.Sleep(_loopDelay * 10);

                lock (_padlock)
                {
                    if (_cancelled)
                    {
                        break;
                    }
                }

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
                _cancelled = false;

                foreach (var source in _sinksToUpdate.Keys)
                {
                    var processor = new Processor() { Source = source, Sinks = _sinksToUpdate[source], Delay = source.Delay };
                    processor.Start();
                    _workers.Add(processor);
                }

                _plotter = new Thread(Graph);
                _plotter.Start();
            }
        }

        public void Stop()
        {
            lock(_padlock)
            {
                _cancelled = true;

                foreach (var worker in _workers)
                {
                    worker.Stop();
                }

                //_plotter.Join();
            }
        }
    }

    class Processor
    {
        public IDataSource Source { get; set; }

        public IList<IDataSink<IMetricData>> Sinks { get; set; }

        public int Delay { get; set; }

        private bool _cancelled;

        private Thread _worker;

        public void Stop()
        {
            lock(_padlock)
            {
                _cancelled = true;
            }

            //_worker.Join();
        }

        public void Start()
        {
            _worker = new Thread(Process);
            _worker.IsBackground = true;
            _worker.Start();
        }

        private void Process()
        {
            bool firstStart = true;

            while (true)
            {
                if(!firstStart)
                    Thread.Sleep(Delay);

                firstStart = false;

                lock (_padlock)
                {
                    if (_cancelled)
                    {
                        break;
                    }
                }

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