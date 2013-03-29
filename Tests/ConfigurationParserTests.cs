using System.Configuration;
using System.Linq;
using Common;
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

            Assert.IsInstanceOf<ParsedSchedules>(schedules);
            Assert.AreEqual(6, schedules.Schedules.Count());
        }

        [Test]
        public void ConfigurationParserBuildsPreloadSchedules()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration("MetricAgent.exe");

            var schedules = ConfigurationParser.Parse(configuration);

            Assert.IsInstanceOf<ParsedSchedules>(schedules);
            Assert.AreEqual(1, schedules.PreloadSchedules.Count());            
        }
    }
}