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

            var preloadScheduleElement = new ScheduleElement(simpleConfig.Name + " Preload Schedule", simpleConfig.Delay, simpleConfig.Name + " Preload Chain");
            var preloadScheduleConfig = new ScheduleConfiguration(new [] { preloadScheduleElement });

            var chainNames = String.Join(",", new[] { simpleConfig.Name + " Sink Chain", simpleConfig.Name + " Buffer Chain" });
            var scheduleElement = new ScheduleElement(simpleConfig.Name + " Schedule", simpleConfig.Delay, chainNames);
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