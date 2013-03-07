﻿using System.Configuration;

namespace Sinks
{
    public class CircularDataSinkConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("sinks")]
        public SinkElementCollection Sinks
        {
            get { return (SinkElementCollection)base["sinks"]; }
        }
    }

    public class SinkElement : ConfigurationElement
    {
        public SinkElement()
        {
        }

        public SinkElement(string name, int points, float? min = null, float? max = null)
        {
            Name = name;

            Points = points;

            Min = min.HasValue ? min.Value : float.MinValue;

            Max = max.HasValue ? max.Value : float.MaxValue;
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

        public SinkElement this[string name]
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