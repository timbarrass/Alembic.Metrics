using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class ProcessCountingSourceTests
    {
        [Test]
        public void ProcessCountingSource_ProvidesASpec()
        {
            var source = new ProcessCountingSource("count", "count", "chrome", null, 1);

            var expectedMetrics = new[] { "count" };

            Assert.AreEqual(expectedMetrics[0], source.Spec.Name);
        }

        [Test]
        public void ProcessingCountingSource_CanBeConfiguredWithAConfigElement()
        {
            var config = new ProcessElement("id", "testCounter", "exe", "machine", 1);

            var source = new ProcessCountingSource(config);

            Assert.AreEqual("testCounter", source.Name);
        }
    }
}