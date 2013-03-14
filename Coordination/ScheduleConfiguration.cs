using System.Configuration;

namespace Coordination
{
    public class ScheduleConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("schedules")]
        public ScheduleElementCollection Links
        {
            get { return (ScheduleElementCollection)base["schedules"]; }
        }
    }

    public class ScheduleElement : ConfigurationElement
    {
        public ScheduleElement()
        {}

        public ScheduleElement(string scheduleName, string chainNames)
            : this(scheduleName, -1, chainNames)
        {}

        public ScheduleElement(string scheduleName, int delay, string chainNames)
        {
            Name = scheduleName;

            Delay = delay;

            Chains = chainNames;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            private set { base["name"] = value; }
        }

        [ConfigurationProperty("delay", IsRequired = false)]
        public int Delay
        {
            get { return (int)base["delay"]; }
            private set { base["delay"] = value; }
        }

        [ConfigurationProperty("chains", IsRequired = true)]
        public string Chains
        {
            get { return (string)base["chains"]; }
            private set { base["chains"] = value; }
        }
    }

    [ConfigurationCollection(typeof(ChainElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ScheduleElementCollection : ConfigurationElementCollection
    {
        public ScheduleElement this[int index]
        {
            get { return (ScheduleElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new ScheduleElement this[string name]
        {
            get { return (ScheduleElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ScheduleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}", (element as ScheduleElement).Name);
        }
    }

}