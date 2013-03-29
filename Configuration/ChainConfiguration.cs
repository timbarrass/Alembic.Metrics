using System.Configuration;

namespace Configuration
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

        public ChainElement(string name, string sources, string sinks, string multiSinks)
        {
            Name = name;

            Sources = sources;

            Sinks = sinks;

            MultiSinks = multiSinks;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            private set { base["name"] = value; }
        }

        [ConfigurationProperty("sources", IsRequired = true)]
        public string Sources
        {
            get { return (string)base["sources"]; }
            private set { base["sources"] = value; }
        }

        [ConfigurationProperty("sinks", IsRequired = true)]
        public string Sinks
        {
            get { return (string)base["sinks"]; }
            private set { base["sinks"] = value; }
        }

        [ConfigurationProperty("multiSinks", IsRequired = true)]
        public string MultiSinks
        {
            get { return (string)base["multiSinks"]; }
            private set { base["multiSinks"] = value; }
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
            return string.Format("{0}-{1}-{2}", (element as ChainElement).Name, (element as ChainElement).Sources, (element as ChainElement).Sinks);
        }
    }


}