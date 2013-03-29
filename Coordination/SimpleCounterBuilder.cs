using System;
using System.Collections.Generic;
using System.Linq;
using Configuration;
using Data;
using Sinks;
using Sources;

namespace Coordination
{
    public class SimpleCounterBuilder
    {
        public static IEnumerable<ISchedule> Build(SimplePerformanceCounterSourceConfiguration configs)
        {
            var schedules = new List<ISchedule>();

            foreach(SimpleCounterElement config in configs.Counters)
            {
                schedules.AddRange(Build(config));
            }

            return schedules;
        }

        public static IEnumerable<ISchedule> Build(SimpleCounterElement simpleConfig)
        {
            var counterElement = new CounterElement(simpleConfig);
            var bufferElement = new SinkElement(simpleConfig);
            var storeElement = new StoreElement(simpleConfig);
            var plotterElement = new PlotterElement(simpleConfig);

            var sourceConfig = new PerformanceCounterDataSourceConfiguration(new[] {counterElement});
            var bufferConfig = new CircularDataSinkConfiguration(new[] {bufferElement});
            var storeConfig = new FileSystemDataStoreConfiguration(new[] {storeElement});
            var plotterConfig = new PlotterConfiguration(new[] {plotterElement});

            var sources = new List<ISnapshotProvider>();
            sources.AddRange(PerformanceCounterDataSourceBuilder.Build(sourceConfig));

            var sinks = new List<ISnapshotConsumer>();
            var buffers = CircularDataSinkBuilder.Build(bufferConfig);
            sinks.AddRange(buffers);
            sources.AddRange(buffers);
            sinks.AddRange(FileSystemDataStoreBuilder.Build(storeConfig));

            var multiSinks = new List<IMultipleSnapshotConsumer>();
            multiSinks.AddRange(MultiPlotterBuilder.Build(plotterConfig));

            var bufferChainElement = new ChainElement(
                simpleConfig.Name + " Buffer Chain",
                simpleConfig.Name + " Source",
                simpleConfig.Name + " Buffer",
                "");
            var bufferChain = ChainBuilder.Build(sources, sinks, multiSinks, new[] {bufferChainElement});

            var sinkChainElement = new ChainElement(
                simpleConfig.Name + " Sink Chain",
                simpleConfig.Name + " Buffer",
                simpleConfig.Name + " Store",
                simpleConfig.Name + " Plotter");
            var sinkChain = ChainBuilder.Build(sources, sinks, multiSinks, new[] {sinkChainElement});

            var chains = new List<IChain>();
            chains.AddRange(bufferChain);
            chains.AddRange(sinkChain);

            var chainNames = String.Join((string) ",", (IEnumerable<string>) chains.Select(c => c.Name));
            var scheduleElement = new ScheduleElement(simpleConfig.Name, simpleConfig.Delay, chainNames);
            var scheduleConfig = new ScheduleConfiguration(new[] {scheduleElement});

            var schedules = ScheduleBuilder.Build(scheduleConfig, chains);

            return schedules;
        }
    }
}