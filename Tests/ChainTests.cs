using System;
using System.Collections.Generic;
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
            var snapshot = new Snapshot { new MetricData(0.5, DateTime.Parse("12 Aug 2008"), new List<string>()) };

            var name = "testChain";

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot);

            var buffer = MockRepository.GenerateMock<ISnapshotConsumer>();
            buffer.Expect(b => b.Update(snapshot));

            var store = MockRepository.GenerateMock<ISnapshotConsumer>();
            store.Expect(s => s.Update(snapshot));

            var chain = new MultipleSinkChain("id", name, source, buffer, store);

            chain.Update();

            store.VerifyAllExpectations();

            buffer.VerifyAllExpectations();

            source.VerifyAllExpectations();
        }

        [Test]
        public void ChainCanBeConfiguredWithMultipleSourcesAndASingleSink()
        {
            var firstSnapshot = new Snapshot { new MetricData(0.5, DateTime.Parse("12 Aug 2008"), new List<string> { "value" }) };
            var secondSnapshot = new Snapshot { new MetricData(0.5, DateTime.Parse("12 Aug 2008"), new List<string> { "value" }) };

            var name = "testChain";

            var firstSource = MockRepository.GenerateMock<ISnapshotProvider>();
            firstSource.Expect(s => s.Snapshot()).Return(firstSnapshot);

            var secondSource = MockRepository.GenerateMock<ISnapshotProvider>();
            secondSource.Expect(s => s.Snapshot()).Return(secondSnapshot);

            var sink = MockRepository.GenerateMock<IMultipleSnapshotConsumer>();
            sink.Expect(b => b.Update(null)).IgnoreArguments();

            var chain = new MultipleSourceChain("id", name, sink, firstSource, secondSource); 

            chain.Update();

            sink.VerifyAllExpectations();

            secondSource.VerifyAllExpectations();

            firstSource.VerifyAllExpectations();
        }   

    }
}