using System;
using System.IO;
using Data;
using NUnit.Framework;
using Sinks;

namespace Tests
{
    [TestFixture]
    public class SinglePlotterTests
    {
        [Test]
        public void CanBeConfiguredWithConfiguration()
        {
            var name = "testPlotter";

            var config = new PlotterElement(name, ".", 0f, 1f, 1f);

            var sink = new SinglePlotter(config);

            Assert.AreEqual(name, sink.Name);
        }

        [Test]
        public void CanBeReset()
        {
            var snapshot = new Snapshot { new MetricData(10, DateTime.Now.AddMinutes(-2)), new MetricData(20, DateTime.Now) };

            var name = "testPlotter";

            var config = new PlotterElement(name, ".", 0f, 1f, 1f);

            var sink = new SinglePlotter(config);

            sink.ResetWith(snapshot);

            var plotFile = Path.Combine(".", Path.ChangeExtension(string.Join("- ", Environment.MachineName, name), "png"));

            Assert.IsTrue(File.Exists(plotFile));

            File.Delete(plotFile);
        }

        [Test]
        public void CanBeUpdated()
        {
            var snapshot = new Snapshot { new MetricData(10, DateTime.Now.AddMinutes(-2)), new MetricData(20, DateTime.Now) };

            var name = "testPlotter";

            var config = new PlotterElement(name, ".", 0f, 1f, 1f);

            var sink = new SinglePlotter(config);

            sink.Update(snapshot);

            var plotFile = Path.Combine(".", Path.ChangeExtension(string.Join("- ", Environment.MachineName, name), "png"));

            Assert.IsTrue(File.Exists(plotFile));

            File.Delete(plotFile);
        }
    }
}