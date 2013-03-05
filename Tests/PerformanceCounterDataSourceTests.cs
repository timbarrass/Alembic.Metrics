using System.Linq;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class PerformanceCounterDataSourceTests
    {
        [Test]
        public void PerformanceCounterDataSource_ProvidesASpec()
        {
            var source = new PerformanceCounterDataSource("id", "test", "Memory", "Committed Bytes", null, null);

            var expectedMetrics = new[] { "test" };

                Assert.AreEqual(expectedMetrics[0], source.Spec.Name);
        }
    }
}