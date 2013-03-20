using System;
using System.IO;
using Data;
using NUnit.Framework;
using Sinks;

namespace Tests
{
    [TestFixture]
    public class MultiPlotterTests
    {
        [Test]
        public void CanBeConfiguredwithConfiguration()
        {
            var name = "testPlotter";

            var config = new PlotterElement(name, ".", 0, 1, 1);

            var plotter = new MultiPlotter(config);

            Assert.IsInstanceOf<IMultipleSnapshotConsumer>(plotter);
            Assert.AreEqual(name, plotter.Name);
        }

        [Test]
        public void CanBeReset()
        {
            var firstSnapshot = new Snapshot { Name = "first" };
            firstSnapshot.Add(new MetricData(10, DateTime.Now.AddMinutes(-2)));
            firstSnapshot.Add(new MetricData(11, DateTime.Now.AddMinutes(-1.5)));
            firstSnapshot.Add(new MetricData(15, DateTime.Now.AddMinutes(-1)));

            var secondSnapshot = new Snapshot { Name = "second" };
            secondSnapshot.Add(new MetricData(10, DateTime.Now.AddMinutes(-2)));
            secondSnapshot.Add(new MetricData(5, DateTime.Now.AddMinutes(-1.5)));
            secondSnapshot.Add(new MetricData(6, DateTime.Now.AddMinutes(-1)));

            var name = "testPlotter";

            var config = new PlotterElement(name, ".", 0, 15, 1);

            var sink = new MultiPlotter(config);

            sink.ResetWith(new [] { firstSnapshot, secondSnapshot });

            var plotFile = Path.Combine(".", Path.ChangeExtension(name, "png"));

            Assert.IsTrue(File.Exists(plotFile));

            File.Delete(plotFile);
        }
    }
}
