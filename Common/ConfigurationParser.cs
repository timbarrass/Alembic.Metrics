using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Configuration;
using Coordination;
using Data;
using Sinks;
using Sources;

namespace Common
{
    public class ConfigurationParser
    {
        [ImportMany]
        private IEnumerable<Lazy<ISimpleBuilder>> DiscoveredBuilders;

        private readonly CompositionContainer _container;

        public ConfigurationParser()
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(SimpleCounterBuilder).Assembly));

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public ParsedSchedules Parse(System.Configuration.Configuration configuration)
        {
            var sources = new List<ISnapshotProvider>();
            var sinks = new List<ISnapshotConsumer>();
            var multiSourceSinks = new List<IMultipleSnapshotConsumer>();
            var chains = new List<IChain>();
            var schedules = new List<ISchedule>();
            var preloadSchedules = new List<ISchedule>();

            var chainConfigs = new List<ChainConfiguration>();
            var preloadScheduleConfigs = new List<ScheduleConfiguration>();
            var scheduleConfigs = new List<ScheduleConfiguration>();

            // Simple configurations
            var builders = new List<ISimpleBuilder>
                {
                    new SimpleCounterBuilder(),
                    new SimpleProcessUptimeBuilder(),
                    new SimpleProcessCountingBuilder(),
                    new SimpleDatabaseBuilder()
                };
            
            foreach(var builder in DiscoveredBuilders)
            {
                var allComponentSets = builder.Value.Instance.Build(configuration);

                foreach (var components in allComponentSets)
                {
                    sources.AddRange(components.Sources);
                    sinks.AddRange(components.Sinks);
                    multiSourceSinks.AddRange(components.Multisinks);
                    chainConfigs.Add(components.Chains);
                    preloadScheduleConfigs.Add(components.PreloadSchedules);
                    scheduleConfigs.Add(components.Schedules);
                }
            }

            var simplePlotterConfiguration = configuration.GetSection("simplePlotters") as SimplePlotterConfiguration;

            if (simplePlotterConfiguration != null)
            {
                schedules.AddRange(SimplePlotterBuilder.Build(simplePlotterConfiguration, sources)); // this won't work, as everything else is hidden behind schedules
            }


            // ProcessCountingSources
            var processCountingSourceConfiguration =
                configuration.GetSection("processCountingSources") as ProcessCountingSourceConfiguration;

            if (processCountingSourceConfiguration != null)
            {
                sources.AddRange(ProcessCountingSourceBuilder.Build(processCountingSourceConfiguration));
            }

            // ProcessUptimeSources
            var processUptimeSourceConfiguration =
                configuration.GetSection("processUptimeSources") as ProcessUptimeSourceConfiguration;

            if (processUptimeSourceConfiguration != null)
            {
                sources.AddRange(ProcessUptimeSourceBuilder.Build(processUptimeSourceConfiguration));
            }

            // PerformanceCounterDataSources
            var performanceCounterSourceConfiguration =
                configuration.GetSection("performanceCounterSources") as PerformanceCounterDataSourceConfiguration;

            if (performanceCounterSourceConfiguration != null)
            {
                sources.AddRange(PerformanceCounterDataSourceBuilder.Build(performanceCounterSourceConfiguration));
            }

            // SqlServerDataSources
            var sqlServerDataSourceConfiguration =
                configuration.GetSection("databaseSources") as SqlServerDataSourceConfiguration;

            if (sqlServerDataSourceConfiguration != null)
            {
                sources.AddRange(SqlServerDataSourceBuilder.Build(sqlServerDataSourceConfiguration));
            }

            // CircularDataSinks
            var circularDataSinkConfiguration = configuration.GetSection("circularDataSinks") as CircularDataSinkConfiguration;

            if (circularDataSinkConfiguration != null)
            {
                var circularDataSinks = CircularDataSinkBuilder.Build(circularDataSinkConfiguration);

                sources.AddRange(circularDataSinks);
                sinks.AddRange(circularDataSinks);
            }

            // FileSystemDataStores
            var fileSystemDataStoreConfiguration =
                configuration.GetSection("fileSystemDataStores") as FileSystemDataStoreConfiguration;

            if (fileSystemDataStoreConfiguration != null)
            {
                var stores = FileSystemDataStoreBuilder.Build(fileSystemDataStoreConfiguration);

                sinks.AddRange(stores);
                sources.AddRange(stores);
            }

            // MultiPlotters
            var multiPlotterConfiguration = configuration.GetSection("multiPlotters") as PlotterConfiguration;

            if (multiPlotterConfiguration != null)
            {
                multiSourceSinks.AddRange(MultiPlotterBuilder.Build(multiPlotterConfiguration));
            }

            // Chains
            var chainConfiguration = configuration.GetSection("chains") as ChainConfiguration;

            if (chainConfiguration != null)
            {
                chainConfigs.Add(chainConfiguration);
            }
            
            foreach(var config in chainConfigs)
            {
                chains.AddRange(ChainBuilder.Build(sources, sinks, multiSourceSinks, config.Links));
            }

            // PreloadSchedules
            var preloadScheduleConfiguration = configuration.GetSection("preloadSchedules") as ScheduleConfiguration;

            if (preloadScheduleConfiguration != null)
            {
                preloadScheduleConfigs.Add(preloadScheduleConfiguration);
            }

            foreach(var config in preloadScheduleConfigs)
            {
                preloadSchedules.AddRange(ScheduleBuilder.Build(config, chains));
            }

            // Schedules
            var scheduleConfiguration = configuration.GetSection("schedules") as ScheduleConfiguration;

            if (scheduleConfiguration != null)
            {
                scheduleConfigs.Add(scheduleConfiguration);
            }

            foreach(var config in scheduleConfigs)
            {
                schedules.AddRange(ScheduleBuilder.Build(config, chains));
            }
            


            return new ParsedSchedules { Schedules = schedules, PreloadSchedules = preloadSchedules };
        }
    }
}