using System.Configuration;

namespace Sources
{
    public class PerformanceCounterDataSourceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("counters")]
        public CounterElementCollection Counters
        {
            get { return (CounterElementCollection)base["counters"]; }
        }
    }

    public class CounterElement : ConfigurationElement
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

        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get { return (string)base["id"]; }
        }

        [ConfigurationProperty("categoryName", IsRequired = true)]
        public string CategoryName
        {
            get { return (string)base["categoryName"]; }
        }

        [ConfigurationProperty("counterName", IsRequired = true)]
        public string CounterName
        {
            get { return (string)base["counterName"]; }
        }

        [ConfigurationProperty("instanceName"), DefaultSettingValue(null)]
        public string InstanceName
        {
            get { return (string)base["instanceName"]; }
        }

        [ConfigurationProperty("min"), DefaultSettingValue(null)]
        public float? Min
        {
            get { return (float?)base["min"]; }
        }

        [ConfigurationProperty("max"), DefaultSettingValue(null)]
        public float? Max
        {
            get { return (float?)base["max"]; }
        }
    }


    [ConfigurationCollection(typeof(CounterElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class CounterElementCollection : ConfigurationElementCollection
    {
        public CounterElement this[int index]
        {
            get { return (CounterElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public CounterElement this[string name]
        {
            get { return (CounterElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CounterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}-{1}-{2}-{3}", (element as CounterElement).Id, (element as CounterElement).CategoryName, (element as CounterElement).CounterName, (element as CounterElement).InstanceName);
        }
    }

}
