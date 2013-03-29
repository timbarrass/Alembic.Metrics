using System.Collections.Generic;
using System.Configuration;
using Configuration;
using Coordination;
using Data;
using Sinks;
using Sources;

namespace Common
{
    public class ConfigurationParser
    {
        public static ParsedSchedules Parse(System.Configuration.Configuration configuration)
        {
            var sources = new List<ISnapshotProvider>();
            var sinks = new List<ISnapshotConsumer>();
            var multiSourceSinks = new List<IMultipleSnapshotConsumer>();
            var chains = new List<IChain>();
            var schedules = new List<ISchedule>();
            var preloadSchedules = new List<ISchedule>();

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
                chains.AddRange(ChainBuilder.Build(sources, sinks, multiSourceSinks, chainConfiguration.Links));
            }

            // PreloadSchedules
            var preloadScheduleConfiguration = configuration.GetSection("preloadSchedules") as ScheduleConfiguration;

            if (preloadScheduleConfiguration != null)
            {
                preloadSchedules.AddRange(ScheduleBuilder.Build(preloadScheduleConfiguration, chains));
            }

            // Schedules
            var scheduleConfiguration = configuration.GetSection("schedules") as ScheduleConfiguration;

            if (scheduleConfiguration != null)
            {
                schedules.AddRange(ScheduleBuilder.Build(scheduleConfiguration, chains));
            }

            // Simple configurations
            var simpleCounterConfiguration = configuration.GetSection("simplePerformanceCounterSources") as SimplePerformanceCounterSourceConfiguration;

            if (simpleCounterConfiguration != null)
            {
                schedules.AddRange(SimpleCounterBuilder.Build(simpleCounterConfiguration));
            }

            var simpleProcessUptimeConfiguration = configuration.GetSection("simpleProcessUptimeSources") as SimpleProcessUptimeConfiguration;

            if (simpleProcessUptimeConfiguration != null)
            {
                schedules.AddRange(SimpleProcessUptimeBuilder.Build(simpleProcessUptimeConfiguration));
            }

            var simpleProcessCounterConfiguration = configuration.GetSection("simpleProcessCountingSources") as SimpleProcessCountingConfiguration;

            if (simpleProcessCounterConfiguration != null)
            {
                schedules.AddRange(SimpleProcessCountingBuilder.Build(simpleProcessCounterConfiguration));
            }

            var simpleDatabaseConfiguration = configuration.GetSection("simpleDatabaseSources") as SimpleDatabaseConfiguration;

            if (simpleDatabaseConfiguration != null)
            {
                schedules.AddRange(SimpleDatabaseBuilder.Build(simpleDatabaseConfiguration));
            }




            return new ParsedSchedules { Schedules = schedules, PreloadSchedules = preloadSchedules };
        }
    }
}