using System.Collections.Generic;
using System.Threading;
using Data;
using Sinks;
using Sources;

namespace MetricAgent
{
    public class Agent
    {
        private IList<IDataSource> _sources;

        private IList<IDataSink<IMetricData>> _sinks;

        private object _padlock = new object(); // locks out cancel message -- rename

        private bool _cancelled = false;

        private int _loopDelay = 5000;

        private List<Processor> _workers = new List<Processor>();

        private Thread _plotter;

        /// <summary>
        /// Instantiate an Agent
        /// </summary>
        /// <param name="sources">The data sources to query</param>
        /// <param name="sinks">The data sinks to update</param>
        public Agent(IList<IDataSource> sources, IList<IDataSink<IMetricData>> sinks, int plotterDelay)
        {
            _sources = sources;
            _sinks = sinks;
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

                foreach (var sink in _sinks)
                {
                    sink.Plot();
                }
            }
        }

        public void Start()
        {
            lock(_padlock)
            {
                _cancelled = false;

                int index = 0;

                foreach(var source in _sources)
                {
                    var sink = _sinks[index++]; // simple sink lookup

                    var processor = new Processor() { Source = source, Sink = sink, Delay = source.Delay };
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

                _plotter.Join();
            }
        }
    }

    class Processor
    {
        public IDataSource Source { get; set; }

        public IDataSink<IMetricData> Sink { get; set; }

        public int Delay { get; set; }

        private bool _cancelled;

        private Thread _worker;

        public void Stop()
        {
            lock(_padlock)
            {
                _cancelled = true;
            }

            _worker.Join();
        }

        public void Start()
        {
            _worker = new Thread(Process);
            _worker.Start();
        }

        private void Process()
        {
            while (true)
            {
                Thread.Sleep(Delay);

                lock (_padlock)
                {
                    if (_cancelled)
                    {
                        break;
                    }
                }

                Update(Source, Sink);
            }
        }

        private void Update(IDataSource source, IDataSink<IMetricData> sink)
        {
            var metricData = source.Query();

            sink.Update(source.Spec.Name, metricData);
        }

        private object _padlock = new object();
    }
}