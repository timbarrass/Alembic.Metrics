using System;
using System.Linq;
using Configuration;
using Data;
using NUnit.Framework;
using Sinks;

namespace Tests
{
    [TestFixture]
    public class CircularDataSinkTests
    {
        [Test]
        public void CircularDataSink_CanBeConfiguredWithASinkElement()
        {
            var name = "testBuffer";
            var points = 1;

            var config = new SinkElement(name, points);

            var sink = new CircularDataSink(config);

            Assert.AreEqual(name, sink.Name);
        }

        [Test]
        public void CircularDataSink_ProvidesAName()
        {
            var config = new SinkElement("testData", 10, float.MinValue, float.MaxValue);

            var sink = new CircularDataSink(config);
            
            Assert.AreEqual(config.Name, sink.Name);
        }

        [Test]
        public void CircularDataSink_CanBeReset()
        {
            var config = new SinkElement("testData", 10, float.MinValue, float.MaxValue);

            var sink = new CircularDataSink(config);

            var snapshot = new Snapshot { new MetricData(10, DateTime.Now.AddMinutes(-2)), new MetricData(20, DateTime.Now.AddMinutes(-1)) };

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
            var config = new SinkElement("testData", 10, float.MinValue, float.MaxValue);

            var sink = new CircularDataSink(config);

            var snapshot = new Snapshot { new MetricData(10, DateTime.Parse("7 Feb 2013")), new MetricData(20, DateTime.Parse("9 Feb 2013")) };

            sink.Update(snapshot);

            var actual = sink.Snapshot(DateTime.Parse("8 Feb 2013"));

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(20, actual.First().Data);
        }

        [Test]
        public void CircularDataSink_CanBeUpdatedWithMetricData()
        {
            var config = new SinkElement("testData", 10, float.MinValue, float.MaxValue);

            var sink = new CircularDataSink(config);

            var snapshot = new Snapshot { new MetricData(10, DateTime.Now) };

            sink.Update(snapshot);

            var actual = sink.Snapshot();

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(10, actual.First().Data);
        }

        [Test]
        public void CircularDataSink_SupportsDataSnapshot()
        {
            var config = new SinkElement("testData", 10, float.MinValue, float.MaxValue);

            var sink = new CircularDataSink(config);

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
            var config = new SinkElement("testData", 10, float.MinValue, float.MaxValue);

            var sink = new CircularDataSink(config);

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