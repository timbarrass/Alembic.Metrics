using System;
using System.Collections.Generic;
using System.Configuration;
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
        [Test, Category("CollaborationTest")]
        public void ConfigurationParserParsesConfigAndRequestsChainsFromBuilder()
        {
            var sources = new List<ISnapshotProvider>();
            var sinks = new List<ISnapshotConsumer>();
            var chains = new List<Chain>();

            var configuration = ConfigurationManager.OpenExeConfiguration("MetricAgent.exe");

            // processCountingSource builder ...
            var processCountingSourceConfiguration = configuration.GetSection("processCountingSources") as ProcessCountingSourceConfiguration;

            if (processCountingSourceConfiguration != null)
            {
                foreach (ProcessElement config in processCountingSourceConfiguration.Processes)
                {
                    sources.Add(new ProcessCountingSource(config));
                }
            }

            // processUptimeSource builder
            var processUptimeSourceConfiguration = configuration.GetSection("processUptimeSources") as ProcessUptimeSourceConfiguration;

            if (processUptimeSourceConfiguration != null)
            {
                foreach (ProcessElement config in processUptimeSourceConfiguration.Processes)
                {
                    sources.Add(new ProcessUptimeSource(config));
                }
            }

            // performanceCounterDataSource builder
            var performanceCounterSourceConfiguration = configuration.GetSection("performanceCounterSources") as PerformanceCounterDataSourceConfiguration;

            if (performanceCounterSourceConfiguration != null)
            {
                foreach (CounterElement config in performanceCounterSourceConfiguration.Counters)
                {
                    sources.Add(new PerformanceCounterDataSource(config));
                }
            }

            // sqlServerDataSource builder
            var sqlServerDataSourceConfiguration = configuration.GetSection("databaseSources") as SqlServerDataSourceConfiguration;

            if (sqlServerDataSourceConfiguration != null)
            {
                foreach (DatabaseElement config in sqlServerDataSourceConfiguration.Databases)
                {
                    sources.Add(new SqlServerDataSource(config));
                }
            }

            // circularDataSink builder
            var circularDataSinkConfiguration = configuration.GetSection("circularDataSinks") as CircularDataSinkConfiguration;

            if (circularDataSinkConfiguration != null)
            {
                foreach (SinkElement config in circularDataSinkConfiguration.Sinks)
                {
                    var sink = new CircularDataSink(config);

                    sources.Add(sink);
                    sinks.Add(sink);
                }
            }

            // fileSystemDataStore builder
            var fileSystemDataStoreConfiguration = configuration.GetSection("fileSystemDataStores") as FileSystemDataStoreConfiguration;

            if (fileSystemDataStoreConfiguration != null)
            {
                foreach (StoreElement config in fileSystemDataStoreConfiguration.Stores)
                {
                    sinks.Add(new FileSystemDataStore(config));
                }
            }

            // chain ... builder?
            var chainConfiguration = configuration.GetSection("chains") as ChainConfiguration;

            if (chainConfiguration != null)
            {
                chains.AddRange(ChainBuilder.Build(sources, sinks, chainConfiguration.Links));
            }

            Assert.AreEqual(14, sources.Count);
            Assert.AreEqual(12, sinks.Count);
            Assert.AreEqual(1, chains.Count);
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
