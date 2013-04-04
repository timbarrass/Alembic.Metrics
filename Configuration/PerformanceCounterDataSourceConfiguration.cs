using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class PerformanceCounterDataSourceConfiguration : ConfigurationSection
    {
        public PerformanceCounterDataSourceConfiguration()
        {
        }

        public PerformanceCounterDataSourceConfiguration(IEnumerable<CounterElement> configs)
        {
            Counters.Add(configs);
        }

        [ConfigurationProperty("counters")]
        public CounterElementCollection Counters
        {
            get { return (CounterElementCollection)base["counters"]; }
        }
    }

    public class CounterElement : ConfigurationElement, ICounterConfiguration
    {
        public CounterElement(string id, string name, string categoryName, string counterName, string instanceName, string machineName)
        {
            base["id"]           = id;
            base["name"]         = name;
            base["categoryName"] = categoryName;
            base["counterName"]  = counterName;
            base["instanceName"] = instanceName;
            base["machineName"]  = machineName;
        }

        public CounterElement()
        {
        }

        public CounterElement(ICounterConfiguration config)
            : this(config.Id + " Source", config.Name, config.CategoryName, config.CounterName, config.InstanceName, config.MachineName)
        {
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

        [ConfigurationProperty("machineName"), DefaultSettingValue(null)]
        public string MachineName
        {
            get { return (string)base["machineName"]; }
        }
    }


    [ConfigurationCollection(typeof(CounterElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class CounterElementCollection : ConfigurationElementCollection
    {
        public void Add(IEnumerable<CounterElement> configs)
        {
            foreach(var config in configs)
            {
                base.BaseAdd(config);
            }
        }

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

        public new CounterElement this[string name]
        {
            get { return (CounterElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CounterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}-{1}-{2}-{3}", (element as CounterElement).MachineName, (element as CounterElement).CategoryName, (element as CounterElement).CounterName, (element as CounterElement).InstanceName);
        }
    }

}
