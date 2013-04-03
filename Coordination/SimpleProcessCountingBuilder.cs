using System;
using System.Collections.Generic;
using System.Linq;
using Configuration;
using Data;
using Sinks;
using Sources;

namespace Coordination
{
    public class SimpleProcessCountingBuilder
    {
        public static IEnumerable<BuiltComponents> Build(SimpleProcessCountingConfiguration configs)
        {
            var schedules = new List<BuiltComponents>();

            foreach(SimpleProcessElement config in configs.Processes)
            {
                schedules.Add(Build(config));
            }

            return schedules;
        }

        public static BuiltComponents Build(SimpleProcessElement simpleConfig)
        {
            var counterElement = new ProcessElement(simpleConfig);
            var bufferElement = new SinkElement(simpleConfig);
            var storeElement = new StoreElement(simpleConfig);
            var plotterElement = new PlotterElement(simpleConfig);

            var sourceConfig = new ProcessCountingSourceConfiguration(new[] {counterElement});
            var bufferConfig = new CircularDataSinkConfiguration(new[] {bufferElement});
            var storeConfig = new FileSystemDataStoreConfiguration(new[] {storeElement});
            var plotterConfig = new PlotterConfiguration(new[] {plotterElement});

            var sources = new List<ISnapshotProvider>();
            sources.AddRange(ProcessCountingSourceBuilder.Build(sourceConfig));

            var sinks = new List<ISnapshotConsumer>();
            var buffers = CircularDataSinkBuilder.Build(bufferConfig);
            sinks.AddRange(buffers);
            sources.AddRange(buffers);
            var stores = FileSystemDataStoreBuilder.Build(storeConfig);
            sinks.AddRange(stores);
            sources.AddRange(stores);

            var multiSinks = new List<IMultipleSnapshotConsumer>();
            multiSinks.AddRange(MultiPlotterBuilder.Build(plotterConfig));

            var bufferChainElement = new ChainElement(
                simpleConfig.Name + " Buffer Chain",
                simpleConfig.Name + " Source",
                simpleConfig.Name + " Buffer",
                "");

            var sinkChainElement = new ChainElement(
                simpleConfig.Name + " Sink Chain",
                simpleConfig.Name + " Buffer",
                simpleConfig.Name + " Store",
                simpleConfig.Name + " Plotter");

            var preloadChainElement = new ChainElement(
                simpleConfig.Name + " Preload Chain",
                simpleConfig.Name + " Store",
                simpleConfig.Name + " Buffer",
                "");

            var chainConfig = new ChainConfiguration(new [] { bufferChainElement, sinkChainElement, preloadChainElement });

            var preloadScheduleElement = new ScheduleElement(simpleConfig.Name + " Preload Schedule", simpleConfig.Delay, preloadChainElement.Name);
            var preloadScheduleConfig = new ScheduleConfiguration(new [] { preloadScheduleElement });

            var chainNames = String.Join(",", new [] { bufferChainElement.Name, sinkChainElement.Name });
            var scheduleElement = new ScheduleElement(simpleConfig.Name + " Schedule", simpleConfig.Delay, chainNames);
            var scheduleConfig = new ScheduleConfiguration(new[] {scheduleElement});

            return new BuiltComponents(sources, sinks, multiSinks, chainConfig, preloadScheduleConfig, scheduleConfig);
        }
    }
}