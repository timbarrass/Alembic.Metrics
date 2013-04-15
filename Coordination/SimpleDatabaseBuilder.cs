using System;
using System.Collections.Generic;
using Configuration;
using Data;
using Sinks;
using Sources;

namespace Coordination
{
    public class SimpleDatabaseBuilder
    {
        public static IEnumerable<BuiltComponents> Build(SimpleDatabaseConfiguration configs)
        {
            var components = new List<BuiltComponents>();

            foreach(SimpleDatabaseElement config in configs.Databases)
            {
                components.Add(Build(config));
            }

            return components;
        }

        public static BuiltComponents Build(SimpleDatabaseElement simpleConfig)
        {
            var counterElement = new DatabaseElement(simpleConfig);
            var sourceConfig = new SqlServerDataSourceConfiguration(new[] { counterElement });
            var source = SqlServerDataSourceBuilder.Build(sourceConfig);

            return SimpleComponentBuilder.BuildStandardSinkSet(simpleConfig, source);
        }
    }
}