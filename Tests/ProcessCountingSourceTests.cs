using Configuration;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class ProcessCountingSourceTests
    {
        [Test]
        public void ProcessingCountingSource_CanBeConfiguredWithAConfigElement()
        {
            var config = new ProcessElement("testCounter", "exe", "machine");

            var source = new ProcessCountingSource(config);

            Assert.AreEqual("testCounter", source.Name);
        }
    }
}