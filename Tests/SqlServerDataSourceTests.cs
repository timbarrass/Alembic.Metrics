using System.Data.Linq;
using System.Linq;
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

            var spec = new MetricSpecification("SqlServer", null, null);

            var query = "select * from ExampleData";

            var context = new DataContext(connString);

            var source = new SqlServerDataSource("id", context, spec, query, 1);

            var timeseries = source.Snapshot();

            Assert.AreEqual(8, timeseries.Count());
        }        
    }
}