using System;
using Coordination;
using Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests
{
    [TestFixture]
    public class ChainTests
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
    }
}