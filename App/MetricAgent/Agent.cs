using System;
using System.Collections.Generic;
using System.Threading;
using Data;
using Sinks;
using Sources;
using Plotters;
using Writers;
using log4net;

namespace MetricAgent
{
    public class Agent
    {
        private EventWaitHandle _cancel = new EventWaitHandle(false, EventResetMode.ManualReset);

        private IList<IDataPlotter> _plotters; 

        private object _cancellationLock = new object();

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
            var ret = _cancel.WaitOne(duration);
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
            lock(_cancellationLock)
            {
                foreach (var source in _sinksToUpdate.Keys)
                {
                    var processor = new Processor { Source = source, Sinks = _sinksToUpdate[source], Delay = source.Delay };
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
            lock(_cancellationLock)
            {
                _cancel.Set();

                foreach (var worker in _workers)
                {
                    worker.Stop();
                }
            }

            _cancel.Set();
            _plotter.Join();
        }
    }

    static class ExceptionExtensions
    {
        public static string FormatMessage(this Exception ex)
        {
            return ex.Message.Replace('\r', ' ').Replace('\n', ' ');
        }
    }
}