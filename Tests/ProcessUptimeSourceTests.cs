using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class ProcessUptimeSourceTests
    {
        public void ProcessUptimeSource_ProvidesASpec()
        {
            var source = new ProcessUptimeSource("id", "uptime", "chrome", null, 1);

            var expectedMetrics = new[] { "uptime" };

            Assert.AreEqual(expectedMetrics[0], source.Spec.Name);
        }   
    }
}