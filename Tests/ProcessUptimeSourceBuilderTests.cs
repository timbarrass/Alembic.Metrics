using System.Linq;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class ProcessUptimeSourceBuilderTests
    {
        [Test]
        public void CanBuildSourcesBasedOnConfig()
        {
            var name = "testSource";

            var configs = new[]
                {
                    new ProcessElement("id", name, "exe", "machine")
                };

            var configCollection = new ProcessUptimeSourceConfiguration(configs); 

            var sources = ProcessUptimeSourceBuilder.Build(configCollection);

            Assert.AreEqual(1, sources.Count());
            Assert.AreEqual(name, sources.First().Name);
        }
    }
}