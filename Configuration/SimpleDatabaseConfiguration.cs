using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class SimpleDatabaseConfiguration : ConfigurationSection
    {
        public SimpleDatabaseConfiguration()
        {
        }

        public SimpleDatabaseConfiguration(IEnumerable<SimpleDatabaseElement> configs)
        {
            Databases.Add(configs);
        }

        [ConfigurationProperty("databases")]
        public SimpleDatabaseElementCollection Databases
        {
            get { return (SimpleDatabaseElementCollection)base["databases"]; }
        }
    }

    public class SimpleDatabaseElement :
        ConfigurationElement,
        IDatabaseConfiguration,
        ISinkConfiguration,
        IStoreConfiguration,
        IPlotterConfiguration,
        IScheduleConfiguration
    {
        public SimpleDatabaseElement(string name, string query, string connectionString, float? min, float? max,
                                    int pointsToKeep, string outputPath, double scale, int delay)
        {
            base["name"] = name;
            base["query"] = query;
            base["connectionString"] = connectionString;
            base["min"] = min;
            base["max"] = max;
            base["points"] = pointsToKeep;
            base["outputPath"] = outputPath;
            base["scale"] = scale;
            base["delay"] = delay;
        }

        public SimpleDatabaseElement()
        {
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("query", IsRequired = true)]
        public string Query
        {
            get { return (string)base["query"]; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)base["connectionString"]; }
        }

        [ConfigurationProperty("points", IsRequired = true)]
        public int Points
        {
            get { return (int)base["points"]; }
        }

        [ConfigurationProperty("outputPath", IsRequired = true)]
        public string OutputPath
        {
            get { return (string)base["outputPath"]; }
        }

        [ConfigurationProperty("min")]
        public float? Min
        {
            get { return (float?)base["min"]; }
        }

        [ConfigurationProperty("max")]
        public float? Max
        {
            get { return (float?)base["max"]; }
        }

        [ConfigurationProperty("scale")]
        public double Scale
        {
            get { return (double)base["scale"]; }
        }

        [ConfigurationProperty("delay", IsRequired = true)]
        public int Delay
        {
            get { return (int)base["delay"]; }
        }
    }

    [ConfigurationCollection(typeof(SimpleDatabaseElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class SimpleDatabaseElementCollection : ConfigurationElementCollection
    {
        public void Add(IEnumerable<SimpleDatabaseElement> configs)
        {
            foreach (var config in configs)
            {
                base.BaseAdd(config);
            }
        }

        public SimpleDatabaseElement this[int index]
        {
            get { return (SimpleDatabaseElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new SimpleDatabaseElement this[string name]
        {
            get { return (SimpleDatabaseElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SimpleDatabaseElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}", (element as SimpleDatabaseElement).Name);
        }
    }
}