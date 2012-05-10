﻿using System.Configuration;

namespace Sources
{
    public class ProcessCountingSourceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("processes")]
        public ProcessElementCollection Processes
        {
            get { return (ProcessElementCollection)base["processes"]; }
        }
    }

    public class ProcessElement : ConfigurationElement
    {
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
    }


    [ConfigurationCollection(typeof(ProcessElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ProcessElementCollection : ConfigurationElementCollection
    {
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

        public ProcessElement this[string name]
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