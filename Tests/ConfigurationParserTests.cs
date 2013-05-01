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

            var parser = new ConfigurationParser();
            var schedules = parser.Parse(configuration);

            Assert.IsInstanceOf<ParsedSchedules>(schedules);
            Assert.AreEqual(8, schedules.Schedules.Count());
        }

        [Test]
        public void ConfigurationParserBuildsPreloadSchedules()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration("MetricAgent.exe");

            var parser = new ConfigurationParser();
            var schedules = parser.Parse(configuration);

            Assert.IsInstanceOf<ParsedSchedules>(schedules);
            Assert.AreEqual(7, schedules.PreloadSchedules.Count());            
        }
    }
}