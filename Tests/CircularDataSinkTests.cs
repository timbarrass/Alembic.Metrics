using System;
using System.Linq;
using Data;
using NUnit.Framework;
using Rhino.Mocks;
using Sinks;
using Sources;

namespace Tests
{
    [TestFixture]
    public class CircularDataSinkTests
    {
        [Test]
        public void CircularDataSink_CanBeReset()
        {
            var spec = new MetricSpecification("testData", float.MinValue, float.MaxValue);

            var sink = new CircularDataSink(10, spec);

            var snapshot = new Snapshot { new MetricData(10, DateTime.Now), new MetricData(20, DateTime.Now) };

            sink.Update(snapshot);

            var actual = sink.Snapshot();

            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual(10, actual.First().Data);

            snapshot = new Snapshot { new MetricData(20, DateTime.Now) };

            sink.ResetWith(snapshot);

            actual = sink.Snapshot();

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(20, actual.First().Data);
        }

        [Test]
        public void CircularDataSink_CanProvideASnapshotOfDataAfterSomeCutoff()
        {
            var spec = new MetricSpecification("testData", float.MinValue, float.MaxValue);

            var sink = new CircularDataSink(10, spec);

            var snapshot = new Snapshot { new MetricData(10, DateTime.Parse("7 Feb 2013")), new MetricData(20, DateTime.Parse("9 Feb 2013")) };

            sink.Update(snapshot);

            var actual = sink.Snapshot(DateTime.Parse("8 Feb 2013"));

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(20, actual.First().Data);
        }

        [Test]
        public void CircularDataSink_CanBeUpdatedWithMetricData()
        {
            var spec = new MetricSpecification("testData", float.MinValue, float.MaxValue);

            var sink = new CircularDataSink(10, spec);

            var snapshot = new Snapshot { new MetricData(10, DateTime.Now) };

            sink.Update(snapshot);

            var actual = sink.Snapshot();

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(10, actual.First().Data);
        }

        [Test]
        public void CircularDataSink_SupportsDataSnapshot()
        {
            var spec = new MetricSpecification("test1", null, null);

            var sink = new CircularDataSink(10, spec);

            sink.Update(new Snapshot
                            {
                                new MetricData( 1.0, DateTime.Now),
                                new MetricData( 2.0, DateTime.Now.AddMinutes(5)),
                                new MetricData( 4.0, DateTime.Now.AddMinutes(10)),
                            });

            var snapshot = sink.Snapshot();

            var total = 0.0d;
            foreach (var dataPoint in snapshot)
            {
                if(dataPoint.Data.HasValue)
                    total += dataPoint.Data.Value;
            }

            Assert.AreEqual(7.0d, total);
        }

        [Test]
        public void CircularDataSink_IgnoreDataOlderThanMostRecentLastUpdate()
        {
            var spec = new MetricSpecification("test1", null, null);

            var sink = new CircularDataSink(10, spec);

            sink.Update(new Snapshot
                            {
                                new MetricData( 1.0, DateTime.Now),
                                new MetricData( 2.0, DateTime.Now.AddMinutes(5)),
                                new MetricData( 4.0, DateTime.Now.AddMinutes(10)),
                                new MetricData( 1.0, DateTime.Now.AddMinutes(5))
                            });

            var snapshot = sink.Snapshot();

            var total = 0.0d;
            foreach (var dataPoint in snapshot)
            {
                total += dataPoint.Data.Value;
            }

            Assert.AreEqual(7.0d, total);
        }
    }
}