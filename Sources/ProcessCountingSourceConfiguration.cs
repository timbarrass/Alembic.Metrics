using System.Configuration;

namespace Sources
{
    public class ProcessCountingSourceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("processes")]
        public ExampleThingElementCollection Processes
        {
            get { return (ExampleThingElementCollection)base["processes"]; }
        }
    }

    public class ProcessElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }

        }

        [ConfigurationProperty("exe")]
        public string Exe
        {
            get { return (string)base["exe"]; }
        }
    }


    [ConfigurationCollection(typeof(ProcessElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ExampleThingElementCollection : ConfigurationElementCollection
    {
        //#region Constructors
        //static ExampleThingElementCollection()
        //{
        //    m_properties = new ConfigurationPropertyCollection();
        //}

        //public ExampleThingElementCollection()
        //{
        //}
        //#endregion

        //#region Fields
        //private static ConfigurationPropertyCollection m_properties;
        //#endregion

        //#region Properties
        //protected override ConfigurationPropertyCollection Properties
        //{
        //    get { return m_properties; }
        //}

        //public override ConfigurationElementCollectionType CollectionType
        //{
        //    get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        //}
        //#endregion

        #region Indexers
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
        #endregion

        #region Overrides
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProcessElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ProcessElement).Name;
        }
        #endregion
    }

}
