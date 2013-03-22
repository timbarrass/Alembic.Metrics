using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
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
            return string.Format("{0}-{1}-{2}-{3}", (element as SimpleCounterElement).CategoryName, (element as SimpleCounterElement).CounterName, (element as SimpleCounterElement).InstanceName);
        }
    }
}