using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class SqlServerDataSourceConfiguration : ConfigurationSection
    {
        public SqlServerDataSourceConfiguration()
        {}

        public SqlServerDataSourceConfiguration(IEnumerable<DatabaseElement> configs)
        {
            Databases.Add(configs);
        }

        [ConfigurationProperty("databases")]
        public DatabaseElementCollection Databases
        {
            get { return (DatabaseElementCollection)base["databases"]; }
        }
    }

    public class DatabaseElement : ConfigurationElement
    {
        public DatabaseElement()
        {
        }

        public DatabaseElement(IDatabaseConfiguration config)
            : this(config.Id + " Source", config.Name, config.ConnectionString, config.Query, config.Labels)
        {
        }

        public DatabaseElement(string id, string name, string connString, string query, string labels)
        {
            base["id"] = id;
            base["name"] = name;
            base["connectionString"] = connString;
            base["query"] = query;
            base["labels"] = labels;
        }

        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get { return (string)base["id"]; }
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

        [ConfigurationProperty("labels", IsRequired = true)]
        public string Labels
        {
            get { return (string)base["labels"]; }
        }
    }

    [ConfigurationCollection(typeof(DatabaseElement),
        CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class DatabaseElementCollection : ConfigurationElementCollection
    {
        public void Add(IEnumerable<DatabaseElement> configs)
        {
            foreach(var config in configs)
            {
                base.BaseAdd(config);
            }
        }

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

        public new DatabaseElement this[string name]
        {
            get { return (DatabaseElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DatabaseElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //return base.GetElementKey(element);
            return string.Format("{0}", (element as DatabaseElement).Name);
        }
    }

}