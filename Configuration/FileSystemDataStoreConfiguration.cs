using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class FileSystemDataStoreConfiguration : ConfigurationSection
    {
        public FileSystemDataStoreConfiguration()
        {
        }

        public FileSystemDataStoreConfiguration(IEnumerable<StoreElement> configs)
        {
            Stores.Add(configs);
        }

        [ConfigurationProperty("stores")]
        public StoreElementCollection Stores
        {
            get { return (StoreElementCollection)base["stores"]; }
        }
    }

    public class StoreElement : ConfigurationElement
    {
        public StoreElement()
        {
        }

        public StoreElement(IStoreConfiguration config)
            : this(config.Id + " Store", config.Name, config.OutputPath)
        {
        }

        public StoreElement(string id, string name, string outputPath)
        {
            Id = id;

            Name = name;

            OutputPath = outputPath;
        }

        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get { return (string)base["id"]; }
            private set { base["id"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            private set { base["name"] = value; }
        }

        [ConfigurationProperty("outputPath", IsRequired = true)]
        public string OutputPath
        {
            get { return (string)base["outputPath"]; }
            private set { base["outputPath"] = value; }
        }
    }

    [ConfigurationCollection(typeof(StoreElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class StoreElementCollection : ConfigurationElementCollection
    {
        public void Add(IEnumerable<StoreElement> configs)
        {
            foreach(var config in configs)
            {
                BaseAdd(config, true);
            }
        }

        public StoreElement this[int index]
        {
            get { return (StoreElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new StoreElement this[string name]
        {
            get { return (StoreElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new StoreElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}", (element as StoreElement).Name);
        }
    }

}