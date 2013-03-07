using System.Configuration;

namespace Sinks
{
    public class FileSystemDataStoreConfiguration : ConfigurationSection
    {
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

        public StoreElement(string name, string outputPath, float? min, float? max)
        {
            Name = name;

            OutputPath = outputPath;

            Min = min.HasValue ? min.Value : float.MinValue;

            Max = max.HasValue ? max.Value : float.MaxValue;
        }

        [ConfigurationProperty("min", IsRequired = false)]
        public float Min
        {
            get { return (float)base["min"]; }
            private set { base["min"] = value; }
        }

        [ConfigurationProperty("max", IsRequired = false)]
        public float Max
        {
            get { return (float)base["max"]; }
            private set { base["max"] = value; }
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

        public StoreElement this[string name]
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