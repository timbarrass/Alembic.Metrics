using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class CircularDataSinkConfiguration : ConfigurationSection
    {
        public CircularDataSinkConfiguration()
        {
        }

        public CircularDataSinkConfiguration(IEnumerable<SinkElement> configs)
        {
            Sinks.Add(configs);
        }

        [ConfigurationProperty("sinks")]
        public SinkElementCollection Sinks
        {
            get { return (SinkElementCollection)base["sinks"]; }
        }
    }

    public class SinkElement : ConfigurationElement, ISinkConfiguration
    {
        public SinkElement()
        {
        }

        public SinkElement(ISinkConfiguration config)
            : this(config.Id + " Buffer", config.Name, config.Points)
        {
        }

        public SinkElement(string id, string name, int points, float? min = null, float? max = null)
        {
            Id = id;

            Name = name;

            Points = points;

            Min = min.HasValue ? min.Value : float.MinValue;

            Max = max.HasValue ? max.Value : float.MaxValue;
        }

        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get { return (string)base["id"]; }
            private set { base["id"] = value; }
        }

        [ConfigurationProperty("min", IsRequired = false)]
        public float Min
        {
            get { return (float)base["min"]; }
            private set { base["min"] = value; }
        }

        [ConfigurationProperty("max", IsRequired = false)]
        public float Max
        {
            get { return (float)base["max"]; }
            private set { base["max"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            private set { base["name"] = value; }
        }

        [ConfigurationProperty("points", IsRequired = true)]
        public int Points
        {
            get { return (int)base["points"]; }
            private set { base["points"] = value; }
        }
    }

    [ConfigurationCollection(typeof(SinkElement),
        CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class SinkElementCollection : ConfigurationElementCollection
    {
        public void Add(IEnumerable<SinkElement> configs)
        {
            foreach(var config in configs)
            {
                base.BaseAdd(config, false);
            }
        }

        public SinkElement this[int index]
        {
            get { return (SinkElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new SinkElement this[string name]
        {
            get { return (SinkElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SinkElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}", (element as SinkElement).Name);
        }
    }

}