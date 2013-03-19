using System.Collections.Generic;
using System.Configuration;
using Data;
using Sinks;
using Sources;

namespace Coordination
{
    public struct ParsedSchedules
    {
        public IEnumerable<ISchedule> Schedules;

        public IEnumerable<ISchedule> PreloadSchedules;
    }

    public class ConfigurationParser
    {
        public static ParsedSchedules Parse(Configuration configuration)
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

            // SinglePlotters
            var plotterConfiguration = configuration.GetSection("singlePlotters") as PlotterConfiguration;

            if (plotterConfiguration != null)
            {
                sinks.AddRange(SinglePlotterBuilder.Build(plotterConfiguration));
            }

            // MultiPlotters
            var multiPlotterConfiguration = configuration.GetSection("multiPlotters") as PlotterConfiguration;

            if (multiPlotterConfiguration != null)
            {
                multiSourceSinks.AddRange(MultiPlotterBuilder.Build(plotterConfiguration));
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

            return new ParsedSchedules { Schedules = schedules, PreloadSchedules = preloadSchedules };
        }
    }
}