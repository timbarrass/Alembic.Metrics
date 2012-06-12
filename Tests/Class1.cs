﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using Data;
using MetricAgent;
using Plotters;
using Sinks;
using NUnit.Framework;
using Rhino.Mocks;
using Sources;
using Stores;

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

        private IEnumerable<double?> SamplePerformanceData()
        {
            return new List<double?> { { 1.0 }, { 2.0 }, };
        }

        [Test]
        public void CircularDataSink_CanBeUpdated_WithPerfCounterMetricData()
        {
            var source = new PerformanceCounterDataSource("test", "Memory", "Committed Bytes", null, null);
            var lookup = SamplePerformanceData();
            var mockSource = MockRepository.GenerateMock<IDataSource>();
            mockSource.Expect(x => x.Spec).Return(source.Spec);
            var mockData = MockRepository.GenerateMock<IMetricData>();

            var sink = new CircularDataSink<IMetricData>(10, new [] { mockSource.Spec });

            sink.Update("test", new List<IMetricData> { mockData });

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
                Assert.AreEqual(expectedMetric, source.Spec.Name);
            }
        }

        [Test]
        // Very much not a unit test: todo, mock out the datacontext
        public void SqlServerDataSource_QueriesADatabase()
        {
            var connString =
                string.Join(";",
                    @"Data Source=.\SQLEXPRESS",
                    @"Initial catalog=Alembic.Metrics.Dev",
                    @"Integrated Security=True");

             var spec = new MetricSpecification("SqlServer", null, null);

            var query = "select * from ExampleData";

            var context = new DataContext(connString);

            var source = new SqlServerDataSource(context, spec, query, 1);

            var timeseries = source.Query();

            Assert.AreEqual(8, timeseries.Count());
        }

        [Test]
        public void ProcessCountingSource_ProvidesASpec()
        {
            var source = new ProcessCountingSource("count", "chrome", 1);

            var expectedMetrics = new[] { "count" };

            var i = 0;
            foreach (var expectedMetric in expectedMetrics)
            {
                Assert.AreEqual(expectedMetric, source.Spec.Name);
            }

            //Assert.Fail(); // want a pure counting source here
        }

        public void ProcessUptimeSource_ProvidesASpec()
        {
            var source = new ProcessUptimeSource("uptime", "chrome", 1);

            var expectedMetrics = new[] { "count" };

            var i = 0;
            foreach (var expectedMetric in expectedMetrics)
            {
                Assert.AreEqual(expectedMetric, source.Spec.Name);
            }

            //Assert.Fail(); // want a pure uptime source here
        }

        // also -- want source to handle only a single spec, not multiple specs

        [Test]
        public void ProcessCountingSource_CanBeQueriedBySource()
        {
            var source = new ProcessCountingSource("count", "chrome", 1);

            var expectedMetrics = new[] { "count" };

            var res = source.Query();

            // No assert, no test
            //Assert.Fail();
        }

        [Test]
        public void MetricAgent_ReadsFromSource()
        {
            var perfMetricData = new MetricData(1.0, DateTime.Now);

            var source = MockRepository.GenerateMock<IDataSource>();
            source.Expect(x => x.Query()).Return(new List<IMetricData> { perfMetricData });

            var sink = new BreakingDataSink();

            var agent = new Agent(new List<IDataSource> { source }, new List<IDataSink<IMetricData>> { sink }, new List<IDataPlotter>(), 10);

            // test an internal method -- not intended to be part of public interface
            var actual = agent.Query(source);

            Assert.That(actual.First().Equals(perfMetricData));

            source.VerifyAllExpectations();
        }

        //[Test]
        //public void MetricAgent_WritesToSink()
        //{
        //    var underlyingData = SamplePerformanceData();
        //    var perfMetricData = new MetricData(underlyingData, DateTime.Now);

        //    var source = MockRepository.GenerateMock<IDataSource>();
        //    source.Expect(x => x.Query()).Return(new List<IMetricData> { perfMetricData });

        //    var sink = MockRepository.GenerateMock<IDataSink>();
        //    sink.Expect(x => x.Update(new List<IMetricData> { perfMetricData }));

        //    var agent = new Agent(new List<IDataSource> { source }, new List<IDataSink> { sink }, 10);

        //    // test internal methods -- not intended to be part of public interface
        //    agent.Update();

        //    sink.VerifyAllExpectations();
        //    source.VerifyAllExpectations();
        //}

        //[Test]
        //public void CircularDataSink_IsEnumerable()
        //{
        //    var specs = new[]
        //                    {
        //                        new MetricSpecification("test1", null, null),
        //                    };

        //    var sink = new CircularDataSink<IMetricData>(10, specs);
        //    sink.Update("test1", new List<IMetricData> { new MetricData( 1.0, DateTime.Now) });
        //    sink.Update("test1", new List<IMetricData> { new MetricData( 2.0, DateTime.Now) });
        //    sink.Update("test1", new List<IMetricData> { new MetricData( 4.0, DateTime.Now) });

        //    var total = 0d;
        //    var iter = sink.GetEnumerator("test1");
        //    while(iter.MoveNext())
        //    {
        //        if(iter.Current.Data.HasValue)
        //            total += iter.Current.Data.Value;
        //    }

        //    Assert.AreEqual(7d, total);
        //}

        //[Test]
        //public void GenericCircularDataSink_IsEnumerable()
        //{
        //    var specs = new[]
        //                    {
        //                        new MetricSpecification("test1", null, null),
        //                    };

        //    var sink = new CircularDataSink<MetricData>(10, specs);
        //    sink.Update("test1", new[]
        //                    {
        //                        new MetricData( 1.0, DateTime.Now),
        //                        new MetricData( 2.0, DateTime.Now),
        //                        new MetricData( 4.0, DateTime.Now),
        //                    });

        //    var total = 0d;
        //    var iter = sink.GetEnumerator("test1");
        //    while (iter.MoveNext())
        //    {
        //        if(iter.Current.Data.HasValue)
        //            total += iter.Current.Data.Value;
        //    }

        //    Assert.AreEqual(7d, total);
        //}

        [Test, Ignore("Needs updating to use snapshotProvider interface, not enumerator")]
        public void GenericCircularDataSink_SupportsABufferPerSpec()
        {
            var specs = new[]
                            {
                                new MetricSpecification("test1", null, null),
                                new MetricSpecification("test2", null, null),
                            };
            
            var sink = new CircularDataSink<MetricData>(10, specs);

            sink.Update("test1", new []
                            {
                                new MetricData( 1.0, DateTime.Now),
                                new MetricData( 2.0, DateTime.Now),
                                new MetricData( 4.0, DateTime.Now),
                            });

            var total = 0d;
            //var iter = sink.GetEnumerator("test1");
            //while (iter.MoveNext())
            //{
            //    if(iter.Current.Data.HasValue)
            //        total += iter.Current.Data.Value;
            //}

            Assert.AreEqual(7d, total);
        }

        [Test]      
        public void CircularDataSink_CanPersistData()
        {
            var mockStore = MockRepository.GenerateMock<IDataStore<MetricData>>();
            mockStore.Expect(x => x.Write(null, null)).IgnoreArguments();

            var specs = new[]
                            {
                                new MetricSpecification("test1", null, null),
                            };

            var sink = new CircularDataSink<MetricData>(10, specs, mockStore);
            sink.Update("test1", new[]
                            {
                                new MetricData( 1.0, DateTime.Now),
                                new MetricData( 2.0, DateTime.Now),
                                new MetricData( 4.0, DateTime.Now),
                            });

            sink.Write();

            mockStore.VerifyAllExpectations();
        }

        [Test, Category("CollaborationTest"), Ignore("Collaboration test, or higher")]
        public void FileSystemDataStore_CanPersistSimpleData()
        {
            var testData = new[] { new TestSerializable { Message = "Hello" } };

            var store = new FileSystemDataStore<TestSerializable>();

            store.Write("testData", testData);

            var ret = store.Read("testData");

            Assert.AreEqual(testData.First().Message, ret.First().Message);

            File.Delete("testData.am.gz");
        }

        [Test, Category("CollaborationTest"), Ignore("Collaboration test, or higher")]
        public void SinglePlotter_VisitsSinksAndExtractsData()
        {
            var specs = new[]
                            {
                                new MetricSpecification("test1", null, null),
                            };

            var sink = new CircularDataSink<MetricData>(10, specs);

            sink.Update("test1", new[]
                            {
                                new MetricData( 1.0, DateTime.Now),
                                new MetricData( 2.0, DateTime.Now.AddMinutes(5)),
                                new MetricData( 4.0, DateTime.Now.AddMinutes(10)),
                            });

            var visitor = new SinglePlotter<MetricData>(sink, specs[0]);

            visitor.Plot();

            //Assert.AreEqual(7.0d, visitor.Total);
        }

        [Test]
        public void CircularDataSink_SupportsDataSnapshot()
        {
            var specs = new[]
                            {
                                new MetricSpecification("test1", null, null),
                            };

            var sink = new CircularDataSink<MetricData>(10, specs);

            sink.Update("test1", new[]
                            {
                                new MetricData( 1.0, DateTime.Now),
                                new MetricData( 2.0, DateTime.Now.AddMinutes(5)),
                                new MetricData( 4.0, DateTime.Now.AddMinutes(10)),
                            });
            
            var snapshot = sink.Snapshot("test1");

            var total = 0.0d;
            foreach(var dataPoint in snapshot)
            {
                total += dataPoint.Data.Value;
            }

            Assert.AreEqual(7.0d, total);
        }
    }

    [Serializable]
    public class TestSerializable
    {
        public string Message;
    }

    public class BreakingDataSink : IDataSink<IMetricData>
    {

        public void Update(string specName, IEnumerable<IMetricData> perfMetricData)
        {
            throw new NotImplementedException();
        }

        public void Write()
        {
            throw new NotImplementedException();
        }

        public void Plot(string specName)
        {
            throw new NotImplementedException();
        }

        public void Plot()
        {
            throw new NotImplementedException();
        }

        
    }


}