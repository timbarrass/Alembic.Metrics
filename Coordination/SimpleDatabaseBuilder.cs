using System.Collections.Generic;
using System.ComponentModel.Composition;
using Configuration;
using Sources;

namespace Coordination
{
    [Export(typeof(ISimpleBuilder))]
    public class SimpleDatabaseBuilder : ISimpleBuilder
    {
        public static readonly SimpleDatabaseBuilder _instance = new SimpleDatabaseBuilder();

        public ISimpleBuilder Instance
        {
            get { return _instance; }
        }

        public IEnumerable<BuiltComponents> Build(System.Configuration.Configuration configuration)
        {
            var components = new List<BuiltComponents>();

            var simpleDatabaseConfiguration = configuration.GetSection("simpleDatabaseSources") as SimpleDatabaseConfiguration;

            if (simpleDatabaseConfiguration != null)
            {
                foreach (SimpleDatabaseElement config in simpleDatabaseConfiguration.Databases)
                {
                    components.Add(Build(config));
                }
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