using System;
using System.Collections.Generic;
using System.Configuration;
using Configuration;
using Coordination;
using Data;
using NUnit.Framework;
using Rhino.Mocks;
using Sinks;
using Sources;

namespace Tests
{
    [TestFixture]
    public class Sandbox
    {
        [Test]
        public void CanMapASimpleCounterConfigurationToACounterConfiguration()
        {
            const string name = "testCounter";
            const string catgeory = "theCategory";
            const string counter = "theCounter";
            const string instance = "theInstance";
            const string machine = "theMachine";
            float? min = 0.0f;
            float? max = 0.0f;
            const int points = 10;
            const string outputPath = "thePath";
            const double scale = 0.1d;

            var simpleConfig = new SimpleCounterElement(name, catgeory, counter, instance, machine, min, max, points, outputPath, scale);

            var config = new CounterElement(simpleConfig);

            Assert.IsInstanceOf<ICounterConfiguration>(simpleConfig);
            Assert.IsInstanceOf<IConfiguration>(simpleConfig);
            Assert.AreEqual(name, config.Name);
            Assert.AreEqual(counter, config.CounterName);
        }

        [Test, Category("CollaborationTest")]
        public void SimpleConfigurationParserParsesConfigAndGeneratesEndToEndComponents()
        {
 

            Assert.Fail();
        }

        [Test, Category("CollaborationTest")]
        public void ConfigurationParserParsesConfigAndRequestsChainsFromBuilder()
        {
            var sources = new List<ISnapshotProvider>();
            var sinks = new List<ISnapshotConsumer>();
            var chains = new List<IChain>();
            var schedules = new List<ISchedule>();

            var configuration = ConfigurationManager.OpenExeConfiguration("MetricAgent.exe");

            // ProcessCountingSources
            var processCountingSourceConfiguration = configuration.GetSection("processCountingSources") as ProcessCountingSourceConfiguration;

            if (processCountingSourceConfiguration != null)
            {
                sources.AddRange(ProcessCountingSourceBuilder.Build(processCountingSourceConfiguration));
            }

            // ProcessUptimeSources
            var processUptimeSourceConfiguration = configuration.GetSection("processUptimeSources") as ProcessUptimeSourceConfiguration;

            if (processUptimeSourceConfiguration != null)
            {
                sources.AddRange(ProcessUptimeSourceBuilder.Build(processUptimeSourceConfiguration));
            }

            // PerformanceCounterDataSources
            var performanceCounterSourceConfiguration = configuration.GetSection("performanceCounterSources") as PerformanceCounterDataSourceConfiguration;

            if (performanceCounterSourceConfiguration != null)
            {
                sources.AddRange(PerformanceCounterDataSourceBuilder.Build(performanceCounterSourceConfiguration));
            }

            // SqlServerDataSources
            var sqlServerDataSourceConfiguration = configuration.GetSection("databaseSources") as SqlServerDataSourceConfiguration;

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
            var fileSystemDataStoreConfiguration = configuration.GetSection("fileSystemDataStores") as FileSystemDataStoreConfiguration;
            
            if (fileSystemDataStoreConfiguration != null)
            {
                sinks.AddRange(FileSystemDataStoreBuilder.Build(fileSystemDataStoreConfiguration));
            }

            // SinglePlotters
            var plotterConfiguration = configuration.GetSection("singlePlotters") as PlotterConfiguration;

            if (plotterConfiguration != null)
            {
                sinks.AddRange(SinglePlotterBuilder.Build(plotterConfiguration));
            }

            // Chains
            var chainConfiguration = configuration.GetSection("chains") as ChainConfiguration;
            
            if (chainConfiguration != null)
            {
                chains.AddRange(ChainBuilder.Build(sources, sinks, new HashSet<IMultipleSnapshotConsumer>(),  chainConfiguration.Links));
            }

            // Schedules
            var scheduleConfiguration = configuration.GetSection("schedules") as ScheduleConfiguration;

            if (scheduleConfiguration != null)
            {
                schedules.AddRange(ScheduleBuilder.Build(scheduleConfiguration, chains));
            }

            Assert.AreEqual(15, sources.Count);
            Assert.AreEqual(14, sinks.Count);
            Assert.AreEqual(8, chains.Count);
            Assert.AreEqual(5, schedules.Count);
        }

        [Test]
        public void ProvidersShouldProduceAllDataAsSnapshot()
        {
            var snapshot = new Snapshot
                            {
                                new MetricData( 1.0, DateTime.Now),
                                new MetricData( 2.0, DateTime.Now),
                                new MetricData( 4.0, DateTime.Now)
                            };

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot);

            var actual = source.Snapshot();

            Assert.AreEqual(snapshot, actual);
        }

        [Test]
        public void ProviderShouldProduceASubsetOfDataSinceT()
        {
            var snapshot = new Snapshot
                            {
                                new MetricData( 2.0, DateTime.Parse("2013-02-28")),
                                new MetricData( 4.0, DateTime.Parse("2013-03-03"))
                            };

            var cutoff = DateTime.Parse("2013-02-15");

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot(cutoff)).Return(snapshot);

            var actual = source.Snapshot(cutoff);

            // Just establishing interface really ...
            Assert.AreEqual(snapshot, actual);
        }

        [Test]
        public void ConsumerShouldBeUpdateable()
        {
            var snapshot = new Snapshot
                            {
                                new MetricData( 1.0, DateTime.Now),
                                new MetricData( 2.0, DateTime.Now),
                                new MetricData( 4.0, DateTime.Now)
                            };

            var sink = MockRepository.GenerateMock<ISnapshotConsumer>();

            sink.Update(snapshot);
        }
       

        [Test]
        public void UpdateASinkWithASource()
        {
            var source = MockRepository.GenerateMock<ISnapshotProvider>();

            var sink = MockRepository.GenerateMock<ISnapshotConsumer>();

            sink.Update(source.Snapshot());
        }
    }
}
