using System.Threading;
using Data;
using Sinks;
using Sources;

namespace MetricAgent
{
    public class Agent
    {
        private IDataSource _source;

        private IDataSink _sink;

        private object _padlock = new object();

        private bool _cancelled = false;

        private int _loopDelay = 5000;

        private Thread _worker;

        private Thread _plotter;

        /// <summary>
        /// Instantiate an Agent
        /// </summary>
        /// <param name="source">The data source to query</param>
        /// <param name="sink">The data sink to update</param>
        /// <param name="loopDelay">Seconds to wait before updating again. This does *not* account for time taken to query and update.</param>
        public Agent(IDataSource source, IDataSink sink, int loopDelay)
        {
            _source = source;
            _sink = sink;
            _loopDelay = loopDelay * 1000;
        }

        internal IMetricData Query()
        {
            return _source.Query();
        }

        internal void Update()
        {
            var metricData = Query();

            _sink.Update(metricData);
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
            // Work in an rrd plotter here -- but we should be able to pass an sink
            // into an object of some interface type here and have it plot the data (will
            // inevitably need to be linked to the sink, as it'll need to query the
            // sink for data?)

            // hack -- have the sink be able to plot for now as it has all the right refs
            while (true)
            {
                Thread.Sleep(_loopDelay * 2);

                lock (_padlock)
                {
                    if (_cancelled)
                    {
                        break;
                    }
                }

                _sink.Plot();
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