using System.Configuration;
using System.Linq;
using Coordination;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ConfigurationParserTests
    {
        [Test]
        public void ConfigurationParserParsesConfigAndBuildsSchedules()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration("MetricAgent.exe");

            var schedules = ConfigurationParser.Parse(configuration);

            Assert.AreEqual(4, schedules.Count());
        }
    }
}