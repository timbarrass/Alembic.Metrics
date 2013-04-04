using Configuration;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class ProcessUptimeSourceTests
    {
        [Test]
        public void ProcessUptimeSource_CanBeConfiguredWithAconfigElement()
        {
            var config = new ProcessElement("testConterId", "testCounter", "exe", "machine");

            var source = new ProcessUptimeSource(config);

            Assert.AreEqual("testCounter", source.Name);
        }
    }
}