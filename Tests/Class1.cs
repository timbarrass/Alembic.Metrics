using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using MetricAgent;
using Sinks;
using NUnit.Framework;
using Rhino.Mocks;
using Sources;

namespace Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void MetricSpecification_CanBeInstantiated()
        {
            var name = "someMetric";
            var expectedMin = 0f;
            var expectedMax = 100f;

            var specElement = new MetricSpecification(name, expectedMin, expectedMax);
        }

        private Dictionary<string, double?> SamplePerformanceData()
        {
            var firstName = "committed";
            var secondName = "processor";

            return new Dictionary<string, double?> { { firstName, 1.0 }, { secondName, 1.0 }, };
        }

        [Test]
        public void CircularDataSink_CanBeUpdated_WithPerfCounterMetricData()
        {
            var source = new PerformanceCounterDataSource("test", "Memory", "Committed Bytes", null, null);
            var lookup = SamplePerformanceData();
            var mockSource = MockRepository.GenerateMock<IDataSource>();
            mockSource.Expect(x => x.Spec).Return(source.Spec);
            var mockData = MockRepository.GenerateMock<IMetricData>();

            IDataSink sink = new CircularDataSink(10, mockSource.Spec);

            sink.Update(new List<IMetricData> { mockData });

            mockData.VerifyAllExpectations();

            // nothing actually being asserted here!
        }
        
        [Test]
        public void PerformanceCounterDataSource_ProvidesASpec()
        {
            var source = new PerformanceCounterDataSource("test", "Memory", "Committed Bytes", null, null);

            var expectedMetrics = new[] { "test" };

            var i = 0;
            foreach(var expectedMetric in expectedMetrics)
            {
                Assert.AreEqual(expectedMetric, source.Spec.ElementAt(i++).Name);
            }
        }

        [Test]
        public void SqlServerDataSource_CanBeInstantiated()
        {
            var connString =
                string.Join(";",
                    @"Data Source=.\SQLEXPRESS",
                    @"Initial catalog=Alembic.Metrics.Dev",
                    @"Integrated Security=True");

            var source = new SqlServerDataSource(connString);

            Assert.IsInstanceOf<IDataSource>(source);
        }

        [Test]
        public void SqlServerDataSource_QueriesADatabase()
        {
            var connString =
                string.Join(";",
                    @"Data Source=.\SQLEXPRESS",
                    @"Initial catalog=Alembic.Metrics.Dev",
                    @"Integrated Security=True");

            var source = new SqlServerDataSource(connString);

            var timeseries = source.Query();

            Assert.AreEqual(8, timeseries.Count());
        }

        [Test]
        public void ProcessCountingSource_ProvidesASpec()
        {
            var source = new ProcessCountingSource("count", "uptime", "chrome");

            var expectedMetrics = new[] { "count" };

            var i = 0;
            foreach (var expectedMetric in expectedMetrics)
            {
                Assert.AreEqual(expectedMetric, source.Spec.ElementAt(i++).Name);
            }
        }

        [Test]
        public void MetricAgent_ReadsFromSource()
        {
            var underlyingData = SamplePerformanceData();
            var perfMetricData = new MetricData(underlyingData, DateTime.Now);

            var source = MockRepository.GenerateMock<IDataSource>();
            source.Expect(x => x.Query()).Return(new List<IMetricData> { perfMetricData });

            var sink = new BreakingDataSink();

            var agent = new Agent(new List<IDataSource> { source }, new List<IDataSink> { sink }, 10);

            // test an internal method -- not intended to be part of public interface
            var actual = agent.Query(source);

            Assert.That(actual.First().Equals(perfMetricData));

            source.VerifyAllExpectations();
        }

        [Test]
        public void MetricAgent_WritesToSink()
        {
            var underlyingData = SamplePerformanceData();
            var perfMetricData = new MetricData(underlyingData, DateTime.Now);

            var source = MockRepository.GenerateMock<IDataSource>();
            source.Expect(x => x.Query()).Return(new List<IMetricData> { perfMetricData });

            var sink = MockRepository.GenerateMock<IDataSink>();
            sink.Expect(x => x.Update(new List<IMetricData> { perfMetricData }));

            var agent = new Agent(new List<IDataSource> { source }, new List<IDataSink> { sink }, 10);

            // test internal methods -- not intended to be part of public interface
            agent.Update();

            sink.VerifyAllExpectations();
            source.VerifyAllExpectations();
        }

        [Test]
        public void CircularDataSink_IsEnumerable()
        {
            var sink = new CircularDataSink(10, new List<MetricSpecification>());
            sink.Update(new List<IMetricData> { new MetricData(new Dictionary<string, double?> { { "metric", 1.0 } }, DateTime.Now) });
            sink.Update(new List<IMetricData> { new MetricData(new Dictionary<string, double?> { { "metric", 2.0 } }, DateTime.Now) });
            sink.Update(new List<IMetricData> { new MetricData(new Dictionary<string, double?> { { "metric", 4.0 } }, DateTime.Now) });

            var total = 0d;
            var iter = sink.GetEnumerator();
            while(iter.MoveNext())
            {
                total += iter.Current.Values["metric"].Value;
            }

            Assert.AreEqual(7d, total);
        }

        [Test]
        public void GenericCircularDataSink_IsEnumerable()
        {
            var sink = new CircularDataSink<MetricData>(10, new List<MetricSpecification>());
            sink.Update(new[]
                            {
                                new MetricData(new Dictionary<string, double?> {{"metric", 1.0}}, DateTime.Now),
                                new MetricData(new Dictionary<string, double?> {{"metric", 2.0}}, DateTime.Now),
                                new MetricData(new Dictionary<string, double?> {{"metric", 4.0}}, DateTime.Now),
                            });

            var total = 0d;
            var iter = sink.GetEnumerator();
            while (iter.MoveNext())
            {
                total += iter.Current.Values["metric"].Value;
            }

            Assert.AreEqual(7d, total);
        }
    }


    public class BreakingDataSink : IDataSink
    {
        public void Update(IEnumerable<IMetricData> perfMetricData)
        {
            throw new NotImplementedException();
        }

        public void Plot()
        {
            throw new NotImplementedException();
        }
    }


}

/*
 * Some agent that's configurable, talks to a source, updates a sink
 * Some source that can return data and spec the data it's returned
 * (Data should be self describing?)
 * Some sink that can take a set of data and write it out
 * (Wrap specific underlying tech, thinly?)
*/