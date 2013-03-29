using System.Data.Linq;
using System.Linq;
using Configuration;
using Data;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class SqlServerDataSourceTests
    {
        [Test]
        // Very much not a unit test: todo, mock out the datacontext
        public void SqlServerDataSource_QueriesADatabase()
        {
            var connString =
                string.Join(";",
                            @"Data Source=.\SQLEXPRESS",
                            @"Initial catalog=Alembic.Metrics.Dev",
                            @"Integrated Security=True");

            var query = "select * from ExampleData";

            var context = new DataContext(connString);

            var source = new SqlServerDataSource("id", "name", context, query);

            var timeseries = source.Snapshot();

            Assert.AreEqual(8, timeseries.Count());
        }        

        [Test]
        public void SqlServerDataSource_CanBeConfiguredWithAConfigElement()
        {
            var connString =
                string.Join(";",
                            @"Data Source=.\SQLEXPRESS",
                            @"Initial catalog=Alembic.Metrics.Dev",
                            @"Integrated Security=True");

            var query = "select * from ExampleData";

            var config = new DatabaseElement("testDb", connString, query);

            var source = new SqlServerDataSource(config);

            Assert.AreEqual("testDb", source.Name);
        }
    }
}