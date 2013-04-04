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
        public static List<BuiltComponents> Build(SimplePerformanceCounterSourceConfiguration configs)
        {
            var components = new List<BuiltComponents>();

            foreach(SimpleCounterElement config in configs.Counters)
            {
                components.Add(Build(config));
            }

            return components;
        }

        public static BuiltComponents Build(SimpleCounterElement simpleConfig)
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
            var stores = FileSystemDataStoreBuilder.Build(storeConfig);
            sinks.AddRange(stores);
            sources.AddRange(stores);

            var multiSinks = new List<IMultipleSnapshotConsumer>();
            multiSinks.AddRange(MultiPlotterBuilder.Build(plotterConfig));

            var bufferChainElement = new ChainElement(
                simpleConfig.Id + " Buffer Chain",
                simpleConfig.Name,
                simpleConfig.Id + " Source",
                simpleConfig.Id + " Buffer",
                "");

            var sinkChainElement = new ChainElement(
                simpleConfig.Id + " Sink Chain",
                simpleConfig.Name,
                simpleConfig.Id + " Buffer",
                simpleConfig.Id + " Store",
                simpleConfig.Id + " Plotter");

            var preloadChainElement = new ChainElement(
                simpleConfig.Id + " Preload Chain",
                simpleConfig.Name,
                simpleConfig.Id + " Store",
                simpleConfig.Id + " Buffer",
                "");

            var chainConfig = new ChainConfiguration(new [] { bufferChainElement, sinkChainElement, preloadChainElement });

            var preloadScheduleElement = new ScheduleElement(simpleConfig.Id + " Preload Schedule", simpleConfig.Delay, preloadChainElement.Id);
            var preloadScheduleConfig = new ScheduleConfiguration(new [] { preloadScheduleElement });

            var chainNames = String.Join(",", new[] { sinkChainElement.Id, bufferChainElement.Id });
            var scheduleElement = new ScheduleElement(simpleConfig.Id + " Schedule", simpleConfig.Delay, chainNames);
            var scheduleConfig = new ScheduleConfiguration(new[] {scheduleElement});

            return new BuiltComponents(sources, sinks, multiSinks, chainConfig, preloadScheduleConfig, scheduleConfig);
        }
    }

    public struct BuiltComponents
    {
        public BuiltComponents(
            IEnumerable<ISnapshotProvider> sources,
            IEnumerable<ISnapshotConsumer> sinks,
            IEnumerable<IMultipleSnapshotConsumer> multisinks,
            ChainConfiguration chains,
            ScheduleConfiguration preloadSchedules,
            ScheduleConfiguration schedules )
        {
            Sources = sources;

            Sinks = sinks;

            Multisinks = multisinks;

            Chains = chains;

            PreloadSchedules = preloadSchedules;

            Schedules = schedules;
        }

        public IEnumerable<ISnapshotProvider> Sources;

        public IEnumerable<ISnapshotConsumer> Sinks;

        public IEnumerable<IMultipleSnapshotConsumer> Multisinks;

        public ChainConfiguration Chains;

        public ScheduleConfiguration PreloadSchedules;

        public ScheduleConfiguration Schedules;
    }
}