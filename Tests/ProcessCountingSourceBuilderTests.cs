using System.Linq;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class ProcessCountingSourceBuilderTests
    {
        [Test]
        public void CanBuildSourceFromConfiguration()
        {
            var name = "testSource";

            var configs = new[]
                {
                    new ProcessElement("id", name, "exe", "machine", 10)
                };

            var configCollection = new ProcessCountingSourceConfiguration(configs);

            var sources = ProcessCountingSourceBuilder.Build(configCollection);

            Assert.AreEqual(1, sources.Count());
            Assert.AreEqual(name, sources.First().Name);
        }
    }
}