using System.Configuration;

namespace Sources
{
    public class SqlServerDataSourceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("databases")]
        public DatabaseElementCollection Databases
        {
            get { return (DatabaseElementCollection)base["databases"]; }
        }
    }

    public class DatabaseElement : ConfigurationElement
    {
        [ConfigurationProperty("delay", IsRequired = true)]
        public int Delay
        {
            get { return (int)base["delay"]; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)base["connectionString"]; }
        }

        [ConfigurationProperty("query", IsRequired = true)]
        public string Query
        {
            get { return (string)base["query"]; }
        }
    }

    [ConfigurationCollection(typeof(DatabaseElement),
        CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class DatabaseElementCollection : ConfigurationElementCollection
    {
        public DatabaseElement this[int index]
        {
            get { return (DatabaseElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public DatabaseElement this[string name]
        {
            get { return (DatabaseElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DatabaseElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}", (element as DatabaseElement).ConnectionString);
        }
    }

}