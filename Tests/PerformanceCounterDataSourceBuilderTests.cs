using System.Linq;
using Configuration;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class PerformanceCounterDataSourceBuilderTests
    {
        [Test]
        public void CanBuildWhenGivenConfiguration()
        {
            var name = "testSource";

            var configs = new[]
                {
                    new CounterElement("id", name, "category", "counter", "instance", "machine")
                };

            var configCollection = new PerformanceCounterDataSourceConfiguration(configs);

            var sources = PerformanceCounterDataSourceBuilder.Build(configCollection);

            Assert.AreEqual(1, sources.Count());
            Assert.AreEqual(name, sources.First().Name);
        }
    }
}