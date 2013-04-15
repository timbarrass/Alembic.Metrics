using System;
using System.Collections.Generic;
using Configuration;
using Data;
using Sinks;

namespace Coordination
{
    public class SimpleComponentBuilder
    {
        public static BuiltComponents BuildStandardSinkSet(IStandardSinkSetConfiguration simpleConfig, IEnumerable<ISnapshotProvider> source)
        {
            var sources = new List<ISnapshotProvider>();
            sources.AddRange(source);

            var bufferElement = new SinkElement(simpleConfig);
            var storeElement = new StoreElement(simpleConfig);
            var plotterElement = new PlotterElement(simpleConfig);

            var bufferConfig = new CircularDataSinkConfiguration(new[] {bufferElement});
            var storeConfig = new FileSystemDataStoreConfiguration(new[] {storeElement});
            var plotterConfig = new PlotterConfiguration(new[] {plotterElement});

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

            var chainConfig = new ChainConfiguration(new[] {bufferChainElement, sinkChainElement, preloadChainElement});

            var preloadScheduleElement = new ScheduleElement(simpleConfig.Id + " Preload Schedule", simpleConfig.Delay,
                                                             preloadChainElement.Id);
            var preloadScheduleConfig = new ScheduleConfiguration(new[] {preloadScheduleElement});

            var chainNames = String.Join(",", new[] {sinkChainElement.Id, bufferChainElement.Id});
            var scheduleElement = new ScheduleElement(simpleConfig.Id + " Schedule", simpleConfig.Delay, chainNames);
            var scheduleConfig = new ScheduleConfiguration(new[] {scheduleElement});

            return new BuiltComponents(sources, sinks, multiSinks, chainConfig, preloadScheduleConfig, scheduleConfig);
        }
    }
}