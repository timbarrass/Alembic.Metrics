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

        private IList<IDataSink> _sinks;

        private object _padlock = new object();

        private bool _cancelled = false;

        private int _loopDelay = 5000;

        private Thread _worker;

        private Thread _plotter;

        /// <summary>
        /// Instantiate an Agent
        /// </summary>
        /// <param name="sources">The data sources to query</param>
        /// <param name="sinks">The data sinks to update</param>
        /// <param name="loopDelay">Seconds to wait before updating again. This does *not* account for time taken to query and update.</param>
        public Agent(IList<IDataSource> sources, IList<IDataSink> sinks, int loopDelay)
        {
            _sources = sources;
            _sinks = sinks;
            _loopDelay = loopDelay * 1000;
        }

        internal IEnumerable<IMetricData> Query(IDataSource source)
        {
            return source.Query();
        }

        internal void Update()
        {
            var index = 0;

            foreach (var source in _sources)
            {
                var metricData = Query(source);

                _sinks[index++].Update(metricData);
            }
        }

        private void Process()
        {
            while(true)
            {
                Thread.Sleep(_loopDelay);

                lock(_padlock)
                {
                    if(_cancelled)
                    {
                        break;
                    }
                }

                Update();
            }
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

                _worker = new Thread(Process);
                _worker.Start();

                _plotter = new Thread(Graph);
                _plotter.Start();
            }
        }

        public void Stop()
        {
            lock(_padlock)
            {
                _cancelled = true;

                _worker.Join();
                _plotter.Join();
            }
        }

        public int LoopDelay { get { return _loopDelay; } }
    }
}