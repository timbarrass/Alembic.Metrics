using System.Linq;
using Configuration;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class SqlServerDataSourceBuilderTests
    {
        [Test]
        public void CanBuildWithConfigurationObject()
        {
            var name = "testSource";

            var configs = new[]
                {
                    new DatabaseElement(name, @"Data Source=.\SQLEXPRESS;Initial catalog=Alembic.Metrics.Dev;Integrated Security=True", "select 1")
                };

            var configCollection = new SqlServerDataSourceConfiguration(configs);

            var sources = SqlServerDataSourceBuilder.Build(configCollection);

            Assert.AreEqual(1, sources.Count());
            Assert.AreEqual(name, sources.First().Name);
        }
    }
}