using System;
using System.Collections.Generic;
using Configuration;
using Coordination;
using Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests
{
    [TestFixture]
    public class Sandbox
    {
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

            var plotter = new SimplePlotterElement("id", multiplotterName, sources, min, max, outputPath, scale, delay);

            var source1 = MockRepository.GenerateMock<ISnapshotProvider>();
            source1.Expect(s => s.Name).Return(sourceName1).Repeat.Any();
            var source2 = MockRepository.GenerateMock<ISnapshotProvider>();
            source2.Expect(s => s.Name).Return(sourceName2).Repeat.Any();

            var schedules = SimplePlotterBuilder.Build(plotter, new [] { source1, source2 });

           Assert.AreEqual("testPlotter", schedules.Name);
        }



        [Test]
        public void ProvidersShouldProduceAllDataAsSnapshot()
        {
            var snapshot = new Snapshot
                            {
                                new MetricData( 1.0, DateTime.Now, new List<string> { "value" }),
                                new MetricData( 2.0, DateTime.Now, new List<string> { "value" }),
                                new MetricData( 4.0, DateTime.Now, new List<string> { "value" })
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
                                new MetricData( 2.0, DateTime.Parse("2013-02-28"), new List<string> { "value" }),
                                new MetricData( 4.0, DateTime.Parse("2013-03-03"), new List<string> { "value" })
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
                                new MetricData( 1.0, DateTime.Now, new List<string> { "value" }),
                                new MetricData( 2.0, DateTime.Now, new List<string> { "value" }),
                                new MetricData( 4.0, DateTime.Now, new List<string> { "value" })
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
