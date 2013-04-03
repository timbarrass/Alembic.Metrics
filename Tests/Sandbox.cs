using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Configuration;
using Coordination;
using Data;
using NUnit.Framework;
using Rhino.Mocks;
using Sinks;
using Sources;

namespace Tests
{
    public class SimpleDatabaseBuilderTests
    {
        [Test]
        public void SimpleDatabaseBuilderParsesConfigDownToComponents()
        {
            const string name = "testCounter";
            const string sourceName = name + " Source";
            const string bufferName = name + " Buffer";
            const string storeName = name + " Store";
            const string plotterName = name + " Plotter";
            const string bufferChainName = name + " Buffer Chain";
            const string sinkChainName = name + " Sink Chain";
            const string preloadChainName = name + " Preload Chain";
            const string preloadScheduleName = name + " Preload Schedule";
            const string scheduleName = name + " Schedule";
            const string query = "theQuery";
            const string connectionString = @"Data Source=.\SQLEXPRESS;Initial catalog=Alembic.Metrics.Dev;Integrated Security=True";
            float? min = 0.0f;
            float? max = 0.0f;
            const int points = 10;
            const string outputPath = "thePath";
            const double scale = 0.1d;
            const int delay = 1;

            var simpleConfig = new SimpleDatabaseElement(name, query, connectionString, min, max, points,
                                                        outputPath, scale, delay);

            var components = SimpleDatabaseBuilder.Build(simpleConfig);

            Assert.IsInstanceOf<BuiltComponents>(components);

            Assert.AreEqual(3, components.Sources.Count());
            Assert.IsTrue(components.Sources.Any(s => s.Name == sourceName));
            Assert.IsTrue(components.Sources.Any(s => s.Name == bufferName));
            Assert.IsTrue(components.Sources.Any(s => s.Name == storeName));

            Assert.AreEqual(2, components.Sinks.Count());
            Assert.IsTrue(components.Sinks.Any(s => s.Name == bufferName));
            Assert.IsTrue(components.Sinks.Any(s => s.Name == storeName));

            Assert.AreEqual(1, components.Multisinks.Count());
            Assert.IsTrue(components.Multisinks.Any(s => s.Name == plotterName));

            Assert.IsTrue(components.Chains.Links.Contains(bufferChainName));
            Assert.IsTrue(components.Chains.Links.Contains(sinkChainName));
            Assert.IsTrue(components.Chains.Links.Contains(preloadChainName));

            Assert.IsTrue(components.PreloadSchedules.Links.Contains(preloadScheduleName));
            Assert.IsTrue(components.Schedules.Links.Contains(scheduleName));
        }
    }


    public class SimpleProcessUptimeBuilderTests
    {
        [Test]
        public void SimpleProcessUptimeBuilderParsesConfigDownToComponents()
        {
            const string name = "testCounter";
            const string sourceName = name + " Source";
            const string bufferName = name + " Buffer";
            const string storeName = name + " Store";
            const string plotterName = name + " Plotter";
            const string bufferChainName = name + " Buffer Chain";
            const string sinkChainName = name + " Sink Chain";
            const string preloadChainName = name + " Preload Chain";
            const string preloadScheduleName = name + " Preload Schedule";
            const string scheduleName = name + " Schedule";
            const string exe = "theCategory";
            const string machine = "theMachine";
            float? min = 0.0f;
            float? max = 0.0f;
            const int points = 10;
            const string outputPath = "thePath";
            const double scale = 0.1d;
            const int delay = 1;

            var simpleConfig = new SimpleProcessElement(name, exe, machine, min, max, points,
                                                        outputPath, scale, delay);

            var components = SimpleProcessUptimeBuilder.Build(simpleConfig);

            Assert.IsInstanceOf<BuiltComponents>(components);

            Assert.AreEqual(3, components.Sources.Count());
            Assert.IsTrue(components.Sources.Any(s => s.Name == sourceName));
            Assert.IsTrue(components.Sources.Any(s => s.Name == bufferName));
            Assert.IsTrue(components.Sources.Any(s => s.Name == storeName));

            Assert.AreEqual(2, components.Sinks.Count());
            Assert.IsTrue(components.Sinks.Any(s => s.Name == bufferName));
            Assert.IsTrue(components.Sinks.Any(s => s.Name == storeName));

            Assert.AreEqual(1, components.Multisinks.Count());
            Assert.IsTrue(components.Multisinks.Any(s => s.Name == plotterName));

            Assert.IsTrue(components.Chains.Links.Contains(bufferChainName));
            Assert.IsTrue(components.Chains.Links.Contains(sinkChainName));
            Assert.IsTrue(components.Chains.Links.Contains(preloadChainName));

            Assert.IsTrue(components.PreloadSchedules.Links.Contains(preloadScheduleName));
            Assert.IsTrue(components.Schedules.Links.Contains(scheduleName));
        }
    }

    [TestFixture]
    public class SimpleProcessCountingBuilderTests
    {
        [Test]
        public void SimpleProcessCountingBuilderParsesConfigDownToComponents()
        {
            const string name = "testCounter";
            const string sourceName = name + " Source";
            const string bufferName = name + " Buffer";
            const string storeName = name + " Store";
            const string plotterName = name + " Plotter";
            const string bufferChainName = name + " Buffer Chain";
            const string sinkChainName = name + " Sink Chain";
            const string preloadChainName = name + " Preload Chain";
            const string preloadScheduleName = name + " Preload Schedule";
            const string scheduleName = name + " Schedule";
            const string exe = "theCategory";
            const string machine = "theMachine";
            float? min = 0.0f;
            float? max = 0.0f;
            const int points = 10;
            const string outputPath = "thePath";
            const double scale = 0.1d;
            const int delay = 1;

            var simpleConfig = new SimpleProcessElement(name, exe, machine, min, max, points,
                                                        outputPath, scale, delay);

            var components = SimpleProcessCountingBuilder.Build(simpleConfig);

            Assert.IsInstanceOf<BuiltComponents>(components);

            Assert.AreEqual(3, components.Sources.Count());
            Assert.IsTrue(components.Sources.Any(s => s.Name == sourceName));
            Assert.IsTrue(components.Sources.Any(s => s.Name == bufferName));
            Assert.IsTrue(components.Sources.Any(s => s.Name == storeName));

            Assert.AreEqual(2, components.Sinks.Count());
            Assert.IsTrue(components.Sinks.Any(s => s.Name == bufferName));
            Assert.IsTrue(components.Sinks.Any(s => s.Name == storeName));

            Assert.AreEqual(1, components.Multisinks.Count());
            Assert.IsTrue(components.Multisinks.Any(s => s.Name == plotterName));

            Assert.IsTrue(components.Chains.Links.Contains(bufferChainName));
            Assert.IsTrue(components.Chains.Links.Contains(sinkChainName));
            Assert.IsTrue(components.Chains.Links.Contains(preloadChainName));

            Assert.IsTrue(components.PreloadSchedules.Links.Contains(preloadScheduleName));
            Assert.IsTrue(components.Schedules.Links.Contains(scheduleName));
        }
    }

    [TestFixture]
    public class SimpleCounterBuilderTests
    {
        [Test]
        public void SimpleCounterBuilderParsesConfigDownToComponents()
        {
            const string name = "testCounter";
            const string sourceName = name + " Source";
            const string bufferName = name + " Buffer";
            const string storeName = name + " Store";
            const string plotterName = name + " Plotter";
            const string bufferChainName = name + " Buffer Chain";
            const string sinkChainName = name + " Sink Chain";
            const string preloadChainName = name + " Preload Chain";
            const string preloadScheduleName = name + " Preload Schedule";
            const string scheduleName = name + " Schedule";
            const string catgeory = "theCategory";
            const string counter = "theCounter";
            const string instance = "theInstance";
            const string machine = "theMachine";
            float? min = 0.0f;
            float? max = 0.0f;
            const int points = 10;
            const string outputPath = "thePath";
            const double scale = 0.1d;
            const int delay = 1;

            var simpleConfig = new SimpleCounterElement(name, catgeory, counter, instance, machine, min, max, points,
                                            outputPath, scale, delay);

            var components = SimpleCounterBuilder.Build(simpleConfig);

            Assert.IsInstanceOf<BuiltComponents>(components);

            Assert.AreEqual(3, components.Sources.Count());
            Assert.IsTrue(components.Sources.Any(s => s.Name == sourceName));
            Assert.IsTrue(components.Sources.Any(s => s.Name == bufferName));
            Assert.IsTrue(components.Sources.Any(s => s.Name == storeName));

            Assert.AreEqual(2, components.Sinks.Count());
            Assert.IsTrue(components.Sinks.Any(s => s.Name == bufferName));
            Assert.IsTrue(components.Sinks.Any(s => s.Name == storeName));

            Assert.AreEqual(1, components.Multisinks.Count());
            Assert.IsTrue(components.Multisinks.Any(s => s.Name == plotterName));

            Assert.IsTrue(components.Chains.Links.Contains(bufferChainName));
            Assert.IsTrue(components.Chains.Links.Contains(sinkChainName));
            Assert.IsTrue(components.Chains.Links.Contains(preloadChainName));

            Assert.IsTrue(components.PreloadSchedules.Links.Contains(preloadScheduleName));
            Assert.IsTrue(components.Schedules.Links.Contains(scheduleName));
        }
        
    }

    [TestFixture]
    public class Sandbox
    {

        [Test]
        public void CanMapASimpleCounterConfigurationToCounterBufferStoreAndPlotterConfiguration()
        {
            const string name = "testCounter";
            const string sourceName = name + " Source";
            const string bufferName = name + " Buffer";
            const string storeName = name + " Store";
            const string plotterName = name + " Plotter";
            const string catgeory = "theCategory";
            const string counter = "theCounter";
            const string instance = "theInstance";
            const string machine = "theMachine";
            float? min = 0.0f;
            float? max = 0.0f;
            const int points = 10;
            const string outputPath = "thePath";
            const double scale = 0.1d;
            const int delay = 1;

            var simpleConfig = new SimpleCounterElement(name, catgeory, counter, instance, machine, min, max, points,
                                                        outputPath, scale, delay);

            Assert.IsInstanceOf<ICounterConfiguration>(simpleConfig);
            Assert.IsInstanceOf<IConfiguration>(simpleConfig);

            var counterConfig = new CounterElement(simpleConfig);

            Assert.AreEqual(sourceName, counterConfig.Name);
            Assert.AreEqual(counter, counterConfig.CounterName);

            var bufferConfig = new SinkElement(simpleConfig);

            Assert.AreEqual(bufferName, bufferConfig.Name);

            var storeConfig = new StoreElement(simpleConfig);

            Assert.AreEqual(storeName, storeConfig.Name);

            var plotterConfig = new PlotterElement(simpleConfig);

            Assert.AreEqual(plotterName, plotterConfig.Name);
        }

        [Test]
        public void CanMapASimpleProcessConfigurationToProcessBufferStoreAndPlotterConfiguration()
        {
            const string name = "testCounter";
            const string sourceName = name + " Source";
            const string bufferName = name + " Buffer";
            const string storeName = name + " Store";
            const string plotterName = name + " Plotter";
            const string exe = "testExecutable";
            const string machine = "theMachine";
            float? min = 0.0f;
            float? max = 0.0f;
            const int points = 10;
            const string outputPath = "thePath";
            const double scale = 0.1d;
            const int delay = 11;

            var simpleConfig = new SimpleProcessElement(name, exe, machine, min, max, points, outputPath, scale, delay);

            var uptimeConfig = new ProcessElement(simpleConfig);

            Assert.AreEqual(sourceName, uptimeConfig.Name);
            Assert.AreEqual(exe, uptimeConfig.Exe);

            var bufferConfig = new SinkElement(simpleConfig);

            Assert.AreEqual(bufferName, bufferConfig.Name);

            var storeConfig = new StoreElement(simpleConfig);

            Assert.AreEqual(storeName, storeConfig.Name);

            var plotterConfig = new PlotterElement(simpleConfig);

            Assert.AreEqual(plotterName, plotterConfig.Name);
        }

        [Test]
        public void SimpleMultiPlotterCanConnectToMultipleSources()
        {
            const string sourceName1 = "testSource";
            const string sourceName2 = "testSource2";
            const string multiplotterName = "testPlotter";
            var sources = string.Join(",", sourceName1, sourceName2);
            float? min = 0.0f;
            float? max = 0.0f;
            const string outputPath = "thePath";
            const double scale = 0.1d;
            const int delay = 11;

            var plotter = new SimplePlotterElement(multiplotterName, sources, min, max, outputPath, scale, delay);

            var source1 = MockRepository.GenerateMock<ISnapshotProvider>();
            source1.Expect(s => s.Name).Return(sourceName1).Repeat.Any();
            var source2 = MockRepository.GenerateMock<ISnapshotProvider>();
            source2.Expect(s => s.Name).Return(sourceName2).Repeat.Any();

            var schedules = SimplePlotterBuilder.Build(plotter, new [] { source1, source2 });

           Assert.AreEqual("testPlotter", schedules.Name);
        }



        [Test]
        public void CanMapASimpleDatabaseConfigurationToDatabaseBufferStoreAndPlotterConfiguration()
        {
            const string name = "testDatabase";
            const string sourceName = name + " Source";
            const string bufferName = name + " Buffer";
            const string storeName = name + " Store";
            const string plotterName = name + " Plotter";
            const string query = "testQuery";
            const string connectionString = "theConnectionString";
            float? min = 0.0f;
            float? max = 0.0f;
            const int points = 10;
            const string outputPath = "thePath";
            const double scale = 0.1d;
            const int delay = 11;

            var simpleConfig = new SimpleDatabaseElement(name, query, connectionString, min, max, points, outputPath, scale, delay);

            var databaseConfig = new DatabaseElement(simpleConfig);

            Assert.AreEqual(sourceName, databaseConfig.Name);
            Assert.AreEqual(query, databaseConfig.Query);

            var bufferConfig = new SinkElement(simpleConfig);

            Assert.AreEqual(bufferName, bufferConfig.Name);

            var storeConfig = new StoreElement(simpleConfig);

            Assert.AreEqual(storeName, storeConfig.Name);

            var plotterConfig = new PlotterElement(simpleConfig);

            Assert.AreEqual(plotterName, plotterConfig.Name);
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
