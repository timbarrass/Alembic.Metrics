using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class SimpleProcessUptimeConfiguration : ConfigurationSection
    {
        public SimpleProcessUptimeConfiguration()
        {
        }

        public SimpleProcessUptimeConfiguration(IEnumerable<SimpleProcessElement> configs)
        {
            Processes.Add(configs);
        }

        [ConfigurationProperty("processes")]
        public SimpleProcessElementCollection Processes
        {
            get { return (SimpleProcessElementCollection)base["processes"]; }
        }
    }

    public class SimpleProcessCountingConfiguration : ConfigurationSection
    {
        public SimpleProcessCountingConfiguration()
        {
        }

        public SimpleProcessCountingConfiguration(IEnumerable<SimpleProcessElement> configs)
        {
            Processes.Add(configs);
        }

        [ConfigurationProperty("processes")]
        public SimpleProcessElementCollection Processes
        {
            get { return (SimpleProcessElementCollection)base["processes"]; }
        }
    }

    public class SimpleProcessElement :
        ConfigurationElement,
        IProcessConfiguration,
        IStandardSinkSetConfiguration
    {
        public SimpleProcessElement(string id, string name, string executableName, string machineName, float? min, float? max,
                                    int pointsToKeep, string outputPath, double scale, int delay, string areas)
        {
            base["id"] = id;
            base["name"] = name;
            base["exe"] = executableName;
            base["machineName"] = machineName;
            base["min"] = min;
            base["max"] = max;
            base["points"] = pointsToKeep;
            base["outputPath"] = outputPath;
            base["scale"] = scale;
            base["delay"] = delay;
            base["areas"] = areas;
        }

        public SimpleProcessElement()
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

        [ConfigurationProperty("exe", IsRequired = true)]
        public string Exe
        {
            get { return (string)base["exe"]; }
        }

        [ConfigurationProperty("machineName")]
        public string MachineName
        {
            get { return (string)base["machineName"]; }
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

        [ConfigurationProperty("min", IsRequired = true)]
        public float? Min
        {
            get { return (float?)base["min"]; }
        }

        [ConfigurationProperty("max", IsRequired = true)]
        public float? Max
        {
            get { return (float?)base["max"]; }
        }

        [ConfigurationProperty("scale", IsRequired = true)]
        public double Scale
        {
            get { return (double)base["scale"]; }
        }

        [ConfigurationProperty("delay", IsRequired = true)]
        public int Delay
        {
            get { return (int)base["delay"]; }
        }

        [ConfigurationProperty("areas", IsRequired = false), DefaultSettingValue("")]
        public string Areas
        {
            get { return (string)base["areas"]; }
        }
    }

    [ConfigurationCollection(typeof(SimpleProcessElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class SimpleProcessElementCollection : ConfigurationElementCollection
    {
        public void Add(IEnumerable<SimpleProcessElement> configs)
        {
            foreach (var config in configs)
            {
                base.BaseAdd(config);
            }
        }

        public SimpleProcessElement this[int index]
        {
            get { return (SimpleProcessElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new SimpleProcessElement this[string name]
        {
            get { return (SimpleProcessElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SimpleProcessElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}-{1}", (element as SimpleProcessElement).MachineName, (element as SimpleProcessElement).Exe);
        }
    }
}