using System;
using System.Collections.Generic;
using System.Linq;
using Coordination;
using Data;
using NUnit.Framework;
using Rhino.Mocks;

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
    }
}