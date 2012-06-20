using System.Configuration;

namespace Sinks
{
    public class CircularDataSinkConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("sources")]
        public SourceElementCollection Sinks
        {
            get { return (SourceElementCollection)base["sources"]; }
        }
    }

    public class SourceElement : ConfigurationElement
    {

        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get { return (string)base["id"]; }
        }

        [ConfigurationProperty("points", IsRequired = true)]
        public int Points
        {
            get { return (int)base["points"]; }
        }
    }

    [ConfigurationCollection(typeof(SourceElement),
        CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class SourceElementCollection : ConfigurationElementCollection
    {
        public SourceElement this[int index]
        {
            get { return (SourceElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public SourceElement this[string name]
        {
            get { return (SourceElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SourceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}", (element as SourceElement).Id);
        }
    }

}