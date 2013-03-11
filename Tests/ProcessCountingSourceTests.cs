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
            var source = new ProcessCountingSource("count", "count", "chrome", null);

            var expectedMetrics = new[] { "count" };

            Assert.AreEqual(expectedMetrics[0], source.Spec.Name);
        }

        [Test]
        public void ProcessingCountingSource_CanBeConfiguredWithAConfigElement()
        {
            var config = new ProcessElement("id", "testCounter", "exe", "machine");

            var source = new ProcessCountingSource(config);

            Assert.AreEqual("testCounter", source.Name);
        }
    }
}