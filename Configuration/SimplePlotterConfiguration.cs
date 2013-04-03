using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class SimplePlotterConfiguration : ConfigurationSection
    {
        public SimplePlotterConfiguration()
        {
        }

        public SimplePlotterConfiguration(IEnumerable<SimplePlotterElement> configs)
        {
            Plotters.Add(configs);
        }

        [ConfigurationProperty("plotters")]
        public SimplePlotterElementCollection Plotters
        {
            get { return (SimplePlotterElementCollection)base["plotters"]; }
        }
    }

    public class SimplePlotterElement :
        ConfigurationElement,
        IPlotterConfiguration,
        IScheduleConfiguration
    {
        public SimplePlotterElement(string name, string sources, float? min, float? max,
                                    string outputPath, double scale, int delay)
        {
            base["name"] = name;
            base["sources"] = sources;
            base["min"] = min;
            base["max"] = max;
            base["outputPath"] = outputPath;
            base["scale"] = scale;
            base["delay"] = delay;
        }

        public SimplePlotterElement()
        {
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("sources", IsRequired = true)]
        public string Sources
        {
            get { return (string)base["sources"]; }
        }

        [ConfigurationProperty("outputPath", IsRequired = true)]
        public string OutputPath
        {
            get { return (string)base["outputPath"]; }
        }

        [ConfigurationProperty("min")]
        public float? Min
        {
            get { return (float?)base["min"]; }
        }

        [ConfigurationProperty("max")]
        public float? Max
        {
            get { return (float?)base["max"]; }
        }

        [ConfigurationProperty("scale")]
        public double Scale
        {
            get { return (double)base["scale"]; }
        }

        [ConfigurationProperty("delay", IsRequired = true)]
        public int Delay
        {
            get { return (int)base["delay"]; }
        }
    }

    [ConfigurationCollection(typeof(SimplePlotterElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class SimplePlotterElementCollection : ConfigurationElementCollection
    {
        public void Add(IEnumerable<SimplePlotterElement> configs)
        {
            foreach (var config in configs)
            {
                base.BaseAdd(config);
            }
        }

        public SimplePlotterElement this[int index]
        {
            get { return (SimplePlotterElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new SimplePlotterElement this[string name]
        {
            get { return (SimplePlotterElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SimplePlotterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}", (element as SimplePlotterElement).Name);
        }
    }
}