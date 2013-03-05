using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class ProcessUptimeSourceTests
    {
        [Test]
        public void ProcessUptimeSource_ProvidesASpec()
        {
            var source = new ProcessUptimeSource("id", "uptime", "chrome", null, 1);

            var expectedMetrics = new[] { "uptime" };

            Assert.AreEqual(expectedMetrics[0], source.Spec.Name);
        }   

        [Test]
        public void ProcessUptimeSource_CanBeConfiguredWithAconfigElement()
        {
            var config = new ProcessElement("id", "testCounter", "exe", "machine", 1);

            var source = new ProcessUptimeSource(config);

            Assert.AreEqual("testCounter", source.Name);
        }
    }
}