using System.Collections.Generic;
using System.Configuration;
using Data;
using Sinks;
using Sources;

namespace Coordination
{
    public class ConfigurationParser
    {
        public static IEnumerable<ISchedule> Parse(Configuration configuration)
        {
            var sources = new List<ISnapshotProvider>();
            var sinks = new List<ISnapshotConsumer>();
            var chains = new List<Chain>();
            var schedules = new List<ISchedule>();

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
                sinks.AddRange(FileSystemDataStoreBuilder.Build(fileSystemDataStoreConfiguration));
            }

            // Chains
            var chainConfiguration = configuration.GetSection("chains") as ChainConfiguration;

            if (chainConfiguration != null)
            {
                chains.AddRange(ChainBuilder.Build(sources, sinks, chainConfiguration.Links));
            }

            // Schedules
            var scheduleConfiguration = configuration.GetSection("schedules") as ScheduleConfiguration;

            if (scheduleConfiguration != null)
            {
                schedules.AddRange(ScheduleBuilder.Build(scheduleConfiguration, chains));
            }

            return schedules;
        }
    }
}