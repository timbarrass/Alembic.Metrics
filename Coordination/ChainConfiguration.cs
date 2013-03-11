using System.Configuration;

namespace Coordination
{
    public class ChainConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("links")]
        public ChainElementCollection Links
        {
            get { return (ChainElementCollection)base["links"]; }
        }
    }

    public class ChainElement : ConfigurationElement
    {
        public ChainElement()
        {
        }

        public ChainElement(string name, string source, string sinks)
        {
            Name = name;

            Source = source;

            Sinks = sinks;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            private set { base["name"] = value; }
        }

        [ConfigurationProperty("source", IsRequired = true)]
        public string Source
        {
            get { return (string)base["source"]; }
            private set { base["source"] = value; }
        }

        [ConfigurationProperty("sinks", IsRequired = true)]
        public string Sinks
        {
            get { return (string)base["sinks"]; }
            private set { base["sinks"] = value; }
        }
    }

    [ConfigurationCollection(typeof(ChainElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ChainElementCollection : ConfigurationElementCollection
    {
        public ChainElement this[int index]
        {
            get { return (ChainElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new ChainElement this[string name]
        {
            get { return (ChainElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ChainElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}-{1}-{2}", (element as ChainElement).Name, (element as ChainElement).Source, (element as ChainElement).Sinks);
        }
    }


}