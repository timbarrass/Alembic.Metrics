using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coordination;
using Data;
using NUnit.Framework;
using Plotters;
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
            var snapshot = new Snapshot { new MetricData(0.5, DateTime.Parse("12 Aug 2008")) };

            var configs = new List<ChainElement>
                {
                    new ChainElement("firstTestChain", "testSource", "testBuffer"),
                    new ChainElement("secondTestChain", "testSource", "testBuffer,testStore")
                };

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot).Repeat.Any();
            source.Expect(s => s.Name).Return("testSource").Repeat.Any();

            var buffer = MockRepository.GenerateMock<ISnapshotConsumer>();
            buffer.Expect(b => b.Update(snapshot));
            buffer.Expect(s => s.Name).Return("testBuffer").Repeat.Any();

            var store = MockRepository.GenerateMock<ISnapshotConsumer>();
            store.Expect(s => s.Update(snapshot));
            store.Expect(s => s.Name).Return("testStore").Repeat.Any();

            var sources = new HashSet<ISnapshotProvider> { source };

            var sinks = new HashSet<ISnapshotConsumer> { buffer, store };

            var chains = ChainBuilder.Build(sources, sinks, configs);

            Assert.AreEqual(configs.Count, chains.Count());
        }

        [Test, Category("CollaborationTest")]
        public void ChainBuilderCanBuildAMainAndAPlottingChain()
        {
            var bufferSpec = new MetricSpecification("testBuffer", 0, 1);
            var storeSpec = new MetricSpecification("testStore", 0, 1);
            var plotterSpec = new MetricSpecification("testPlotter", 0, 1);

            var snapshot = new Snapshot
                {
                    new MetricData(0.5, DateTime.Parse("12 Aug 2008")),
                    new MetricData(0.8, DateTime.Parse("13 Aug 2008")),
                    new MetricData(0.9, DateTime.Parse("14 Aug 2008"))
                };

            var configs = new List<ChainElement>
                {
                    new ChainElement("storageChain", "testSource", "testBuffer,testStore"),
                    new ChainElement("plottingChain", "testBuffer", "testPlotter"),
                };

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot).Repeat.Any();
            source.Expect(s => s.Name).Return("testSource").Repeat.Any();

            var buffer = new CircularDataSink(10, bufferSpec);

            var store = new FileSystemDataStore(".", storeSpec);

            var plotter = new SinglePlotter(".", plotterSpec);

            var sources = new HashSet<ISnapshotProvider> { source, buffer };

            var sinks = new HashSet<ISnapshotConsumer> { buffer, store, plotter };

            var chains = ChainBuilder.Build(sources, sinks, configs);

            Assert.AreEqual(configs.Count, chains.Count());

            foreach(var chain in chains)
            {
                chain.Update();
            }

            var chartName = @".\" + Environment.MachineName + "- " + plotterSpec.Name + ".png";
            var storeName = store.Name + ".am.gz";

            Assert.IsTrue(File.Exists(chartName));
            Assert.IsTrue(File.Exists(storeName));

            File.Delete(chartName);

            File.Delete(storeName);

            source.VerifyAllExpectations();
        }
    }
}