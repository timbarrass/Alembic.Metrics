using System;
using Coordination;
using Data;
using NUnit.Framework;
using Rhino.Mocks;
using Sinks;

namespace Tests
{
    [TestFixture]
    public class Sandbox
    {
        [Test]
        public void ChainCanBeConfiguredWithASingleSourceAndMultipleSinks()
        {
            var snapshot = new Snapshot { new MetricData(0.5, DateTime.Parse("12 Aug 2008")) };

            var name = "testChain";

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot);

            var buffer = MockRepository.GenerateMock<ISnapshotConsumer>();
            buffer.Expect(b => b.Update(snapshot));

            var store = MockRepository.GenerateMock<ISnapshotConsumer>();
            store.Expect(s => s.Update(snapshot));

            var chain = new Chain(name, source, buffer, store);   // ignores fact that all must have same spec

            chain.Update();

            store.VerifyAllExpectations();

            buffer.VerifyAllExpectations();

            source.VerifyAllExpectations();
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
