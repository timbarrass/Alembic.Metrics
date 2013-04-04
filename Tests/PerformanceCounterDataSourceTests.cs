using Configuration;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class PerformanceCounterDataSourceTests
    {
        [Test]
        public void PerformanceCounterDataSource_CanBeConfiguredFromAConfigItem()
        {
            var config = new CounterElement("id", "test", "Memory", "Committed Bytes", "", "localhost");

            var source = new PerformanceCounterDataSource(config);

            Assert.AreEqual("test", source.Name);
        }
    }
}