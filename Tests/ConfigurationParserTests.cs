using System.Collections.Generic;
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

            Assert.IsInstanceOf<ParsedSchedules>(schedules);
            Assert.AreEqual(5, schedules.Schedules.Count());
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