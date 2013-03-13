using System;
using Data;
using NUnit.Framework;
using Sinks;

namespace Tests
{
    [TestFixture]
    public class Class1
    {
        [Test, Category("CollaborationTest"), Ignore("Collaboration test, or higher")]
        public void SinglePlotter_VisitsSinksAndExtractsData()
        {
            var spec = new MetricSpecification("test1", null, null);

            var snapshot = new Snapshot
                            {
                                new MetricData( 1.0, DateTime.Now),
                                new MetricData( 2.0, DateTime.Now.AddMinutes(5)),
                                new MetricData( 4.0, DateTime.Now.AddMinutes(10)),
                            };

            var plotter = new SinglePlotter("", spec);

            plotter.Update(snapshot);

            //Assert.AreEqual(7.0d, visitor.Total);
        }

        //[Test, Category("CollaborationTest"), Ignore("Collaboration test, or higher")]
        //public void MultiPlotter_VisitsSinksAndExtractsData()
        //{
        //    var specs = new[]
        //                    {
        //                        new MetricSpecification("test1", null, null),
        //                        new MetricSpecification("test2", null, null),
        //                    };

        //    var sink = new CircularDataSink(10, specs);

        //    var firstTimestamp = DateTime.Now;

        //    sink.Update("test1", new Snapshot
        //                    {
        //                        new MetricData( 1.0, firstTimestamp),
        //                        new MetricData( 2.0, firstTimestamp.AddMinutes(5)),
        //                        new MetricData( 4.0, firstTimestamp.AddMinutes(10)),
        //                    });
        //    sink.Update("test2", new Snapshot
        //                    {
        //                        new MetricData( 2.0, firstTimestamp),
        //                        new MetricData( 2.5, firstTimestamp.AddMinutes(5)),
        //                        new MetricData( 3.0, firstTimestamp.AddMinutes(10)),
        //                    });


        //    var visitor = new MultiPlotter<MetricData>(sink, specs, "aggregated test data");

        //    visitor.Plot();
        //}
    }
}