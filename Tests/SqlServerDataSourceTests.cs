using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Sources;

namespace Tests
{
    [TestFixture]
    public class SqlServerDataSourceTests
    {
        [Test]
        public void CanPopulateMultivalueSnapshots()
        {
            string query = "testQuery";

            var context = MockRepository.GenerateMock<DataContextWrapper>();
            context.Expect(c => c.ExecuteQuery<TimeseriesPoint>(query)).Return(new [] { new TimeseriesPoint { Value1 = 1, Value2 = 2 } });

            var source = new SqlServerDataSource("id", "name", context, query);

            var result = source.Snapshot();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(5, result[0].Data.Count);
            Assert.AreEqual(1, result[0].Data[0]);
        }

        [Test]
        public void CanLabelMultivalueSnapshots()
        {
            string query = "testQuery";

            var context = MockRepository.GenerateMock<DataContextWrapper>();
            context.Expect(c => c.ExecuteQuery<TimeseriesPoint>(query)).Return(new[] { new TimeseriesPoint { Value1 = 1, Value2 = 2 } });

            var source = new SqlServerDataSource("id", "name", context, query, new List<string> { "firstValue", "secondValue" });

            var result = source.Snapshot();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("firstValue", result.Labels[0]);
            Assert.AreEqual("secondValue", result.Labels[1]);
            Assert.AreEqual(5, result[0].Data.Count);
            Assert.AreEqual(1, result[0].Data[0]);            
        }

        [Test]
        // Very much not a unit test: todo, mock out the datacontext
        public void SqlServerDataSource_QueriesADatabase()
        {
            var query = "testQuery";

            var context = MockRepository.GenerateMock<DataContextWrapper>();
            context.Expect(c => c.ExecuteQuery<TimeseriesPoint>(query)).Return(new[]
                {
                    new TimeseriesPoint { Value1 = 1, Value2 = 2 },
                    new TimeseriesPoint { Value1 = 1, Value2 = 2 },
                    new TimeseriesPoint { Value1 = 1, Value2 = 2 },
                    new TimeseriesPoint { Value1 = 1, Value2 = 2 },
                    new TimeseriesPoint { Value1 = 1, Value2 = 2 },
                    new TimeseriesPoint { Value1 = 1, Value2 = 2 },
                    new TimeseriesPoint { Value1 = 1, Value2 = 2 },
                    new TimeseriesPoint { Value1 = 1, Value2 = 2 }
                });

            var source = new SqlServerDataSource("id", "name", context, query);

            var timeseries = source.Snapshot();

            Assert.AreEqual(8, timeseries.Count());
        }        
    }
}