using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class ProcessCountingSourceConfiguration : ConfigurationSection
    {
        public ProcessCountingSourceConfiguration()
        {}

        public ProcessCountingSourceConfiguration(IEnumerable<ProcessElement> configs)
        {
            foreach(var config in configs)
            {
                Processes.Add(config);
            }
        }

        [ConfigurationProperty("processes")]
        public ProcessElementCollection Processes
        {
            get { return (ProcessElementCollection)base["processes"]; }
        }
    }

    public class ProcessElement : ConfigurationElement
    {
        public ProcessElement()
        {
        }

        public ProcessElement(IProcessConfiguration config)
            : this(config.Name + " Source", config.Exe, config.MachineName)
        {
        }

        public ProcessElement(string name, string exe, string machineName)
        {
            base["name"] = name;
            base["exe"] = exe;
            base["machineName"] = machineName;
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

        [ConfigurationProperty("machineName", DefaultValue = null)]
        public string MachineName
        {
            get { return (string)base["machineName"]; }
        }
    }


    [ConfigurationCollection(typeof(ProcessElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ProcessElementCollection : ConfigurationElementCollection
    {
        public void Add(ProcessElement config)
        {
            base.BaseAdd(config, false);
        }

        public ProcessElement this[int index]
        {
            get { return (ProcessElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new ProcessElement this[string name]
        {
            get { return (ProcessElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProcessElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ProcessElement).Name;
        }
    }

}
