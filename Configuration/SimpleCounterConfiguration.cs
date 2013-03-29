using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class SimplePerformanceCounterSourceConfiguration : ConfigurationSection
    {
        public SimplePerformanceCounterSourceConfiguration()
        {
        }

        public SimplePerformanceCounterSourceConfiguration(IEnumerable<SimpleCounterElement> configs)
        {
            Counters.Add(configs);
        }

        [ConfigurationProperty("counters")]
        public SimpleCounterElementCollection Counters
        {
            get { return (SimpleCounterElementCollection)base["counters"]; }
        }
    }

    public class SimpleCounterElement : 
        ConfigurationElement, 
        ICounterConfiguration, 
        ISinkConfiguration, 
        IStoreConfiguration, 
        IPlotterConfiguration,
        IScheduleConfiguration
    {
        public SimpleCounterElement(string name, string categoryName, string counterName, string instanceName, string machineName, float? min, float? max, int points, string outputPath, double scale, int delay)
        {
            base["name"] = name;
            base["categoryName"] = categoryName;
            base["counterName"] = counterName;
            base["instanceName"] = instanceName;
            base["machineName"] = machineName;
            base["min"] = min;
            base["max"] = max;
            base["points"] = points;
            base["outputPath"] = outputPath;
            base["scale"] = scale;
            base["delay"] = delay;
        }

        public SimpleCounterElement()
        {
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

        [ConfigurationProperty("outputPath"), DefaultSettingValue(null)]
        public string OutputPath
        {
            get { return (string)base["outputPath"]; }
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

        [ConfigurationProperty("points"), DefaultSettingValue(null)]
        public int Points
        {
            get { return (int)base["points"]; }
        }

        [ConfigurationProperty("scale"), DefaultSettingValue(null)]
        public double Scale
        {
            get { return (double)base["scale"]; }
        }

        [ConfigurationProperty("delay"), DefaultSettingValue(null)]
        public int Delay
        {
            get { return (int)base["delay"]; }
        }
    }

    [ConfigurationCollection(typeof(SimpleCounterElement),
        CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class SimpleCounterElementCollection : ConfigurationElementCollection
    {
        public void Add(IEnumerable<SimpleCounterElement> configs)
        {
            foreach (var config in configs)
            {
                base.BaseAdd(config);
            }
        }

        public SimpleCounterElement this[int index]
        {
            get { return (SimpleCounterElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new SimpleCounterElement this[string name]
        {
            get { return (SimpleCounterElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SimpleCounterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}-{1}-{2}", (element as SimpleCounterElement).CategoryName, (element as SimpleCounterElement).CounterName, (element as SimpleCounterElement).InstanceName);
        }
    }
}