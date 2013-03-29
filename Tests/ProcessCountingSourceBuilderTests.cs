using System.Linq;
using Configuration;
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
                    new ProcessElement(name, "exe", "machine")
                };

            var configCollection = new ProcessCountingSourceConfiguration(configs);

            var sources = ProcessCountingSourceBuilder.Build(configCollection);

            Assert.AreEqual(1, sources.Count());
            Assert.AreEqual(name, sources.First().Name);
        }
    }
}