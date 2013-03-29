using System.Collections.Generic;
using Configuration;

namespace Sources
{
    public class SqlServerDataSourceBuilder
    {
        public static IEnumerable<SqlServerDataSource> Build(SqlServerDataSourceConfiguration sqlServerDataSourceConfiguration)
        {
            var sqlServerDataSources = new List<SqlServerDataSource>();

            foreach (DatabaseElement config in sqlServerDataSourceConfiguration.Databases)
            {
                sqlServerDataSources.Add(new SqlServerDataSource(config));
            }

            return sqlServerDataSources;
        }
    }
}