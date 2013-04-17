using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Configuration;
using Coordination;
using Data;
using NUnit.Framework;
using Rhino.Mocks;
using Sinks;

namespace Tests
{
    [TestFixture]
    public class ChainBuilderTests
    {
        [Test]
        public void ChainBuilderTakesSourcesAndSinksAndBuildsSetOfChainsBasedOnConfig()
        {
            var snapshot = new Snapshot { new MetricData(0.5, DateTime.Parse("12 Aug 2008"), new List<string>()) };

            var configs = new List<ChainElement>
                {
                    new ChainElement("id1", "firstTestChain", "testSourceId", "testBufferId", ""),
                    new ChainElement("id2", "secondTestChain", "testSourceId", "testBufferId,testStoreId", ""),
                    new ChainElement("id3", "thirdTestChain", "testSourceId,testSourceId", "", "testMultiStoreId")
                };

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot).Repeat.Any();
            source.Expect(s => s.Name).Return("testSource").Repeat.Any();
            source.Expect(s => s.Id).Return("testSourceId").Repeat.Any();

            var buffer = MockRepository.GenerateMock<ISnapshotConsumer>();
            buffer.Expect(b => b.Update(snapshot));
            buffer.Expect(s => s.Name).Return("testBuffer").Repeat.Any();
            buffer.Expect(s => s.Id).Return("testBufferId").Repeat.Any();

            var store = MockRepository.GenerateMock<ISnapshotConsumer>();
            store.Expect(s => s.Update(snapshot));
            store.Expect(s => s.Name).Return("testStore").Repeat.Any();
            store.Expect(s => s.Id).Return("testStoreId").Repeat.Any();

            var multiStore = MockRepository.GenerateMock<IMultipleSnapshotConsumer>();
            multiStore.Expect(s => s.Update(null)).IgnoreArguments();
            multiStore.Expect(s => s.Name).Return("testMultiStore").Repeat.Any();
            multiStore.Expect(s => s.Id).Return("testMultiStoreId").Repeat.Any();

            var sources = new HashSet<ISnapshotProvider> { source };

            var sinks = new HashSet<ISnapshotConsumer> { buffer, store };

            var multiSinks = new HashSet<IMultipleSnapshotConsumer> { multiStore };

            var chains = ChainBuilder.Build(sources, sinks, multiSinks, configs);

            Assert.AreEqual(configs.Count, chains.Count());
        }

        [Test]
        public void ChainBuilderContinuesIfYouAskForAStoreOrSinkThatDoesntExist()
        {
            var snapshot = new Snapshot { new MetricData(0.5, DateTime.Parse("12 Aug 2008"), new List<string> { "value" }) };

            var configs = new List<ChainElement>
                {
                    new ChainElement("id", "storageChain", "testSourceId", "testBufferId", ""),
                };

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot).Repeat.Any();
            source.Expect(s => s.Name).Return("testSource").Repeat.Any();
            source.Expect(s => s.Id).Return("testSourceId").Repeat.Any();

            var sources = new HashSet<ISnapshotProvider> { source };

            var sourceChains = ChainBuilder.Build(sources, new HashSet<ISnapshotConsumer>(), new HashSet<IMultipleSnapshotConsumer>(), configs);

            Assert.AreEqual(0, sourceChains.Count());

            var buffer = MockRepository.GenerateMock<ISnapshotConsumer>();
            buffer.Expect(b => b.Update(snapshot));
            buffer.Expect(s => s.Name).Return("testBuffer").Repeat.Any();
            buffer.Expect(s => s.Id).Return("testBufferId").Repeat.Any();

            var sinks = new HashSet<ISnapshotConsumer> { buffer };

            var sinkChains = ChainBuilder.Build(new HashSet<ISnapshotProvider>(), sinks, new HashSet<IMultipleSnapshotConsumer>(), configs);

            Assert.AreEqual(0, sinkChains.Count());
        }

        [Test, Category("CollaborationTest")]
        public void ChainBuilderCanBuildAMainAndAPlottingChain()
        {
            var snapshot = new Snapshot
                {
                    new MetricData(0.5, DateTime.Parse("12 Aug 2008"), new List<string> { "value" }),
                    new MetricData(0.8, DateTime.Parse("13 Aug 2008"), new List<string> { "value" }),
                    new MetricData(0.9, DateTime.Parse("14 Aug 2008"), new List<string> { "value" })
                };

            var configs = new List<ChainElement>
                {
                    new ChainElement("chain1", "storageChain", "testSourceId", "testBufferId,testStoreId", ""),
                    new ChainElement("chain2", "plottingChain", "testBufferId", "", "testPlotterId"),
                };                   

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot).Repeat.Any();
            source.Expect(s => s.Name).Return("testSource").Repeat.Any();
            source.Expect(s => s.Id).Return("testSourceId").Repeat.Any();

            var bufferConfig = new SinkElement("testBufferId", "testBuffer", 10, 0, 1);

            var buffer = new CircularDataSink(bufferConfig);

            var store = new FileSystemDataStore(".", "testStore", "testStoreId");

            var config = new PlotterElement("testPlotterId", "testPlotter", ".", 0, 1, 1, "");

            var plotter = new MultiPlotter(config);

            var sources = new HashSet<ISnapshotProvider> { source, buffer };

            var sinks = new HashSet<ISnapshotConsumer> { buffer, store };

            var multiSinks = new HashSet<IMultipleSnapshotConsumer> { plotter };

            var chains = ChainBuilder.Build(sources, sinks, multiSinks, configs);

            Assert.AreEqual(configs.Count, chains.Count());

            foreach(var chain in chains)
            {
                chain.Update();
            }

            var chartName = Path.Combine(@".", plotter.Name + ".png");
            var storeName = store.Name + ".am";

            Assert.IsTrue(File.Exists(chartName));
            Assert.IsTrue(File.Exists(storeName));

            File.Delete(chartName);

            File.Delete(storeName);

            source.VerifyAllExpectations();
        }
    }
}