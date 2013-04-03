using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class PlotterConfiguration : ConfigurationSection
    {
        public PlotterConfiguration()
        {
        }

        public PlotterConfiguration(IEnumerable<IPlotterConfiguration> configs)
        {
            foreach(var config in configs)
            {
                Plotters.Add(new PlotterElement(config));
            }
        }

        public PlotterConfiguration(IEnumerable<PlotterElement> configs)
        {
            Plotters.Add(configs);
        }

        [ConfigurationProperty("plotters")]
        public PlotterElementCollection Plotters
        {
            get { return (PlotterElementCollection)base["plotters"]; }
        }
    }

    public class PlotterElement : ConfigurationElement
    {
        public PlotterElement()
        {
        }

        public PlotterElement(IPlotterConfiguration config)
            : this(config.Name + " Plotter", config.OutputPath, config.Min, config.Max, config.Scale)
        {
        }

        public PlotterElement(string name, string outputDirectory, float? min, float? max, double scale)
        {
            Name = name;

            OutputDirectory = outputDirectory;

            Min = min;

            Max = max;

            Scale = scale;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            private set { base["name"] = value; }
        }

        [ConfigurationProperty("outputDirectory", IsRequired = true)]
        public string OutputDirectory
        {
            get { return (string)base["outputDirectory"]; }
            private set { base["outputDirectory"] = value; }
        }

        [ConfigurationProperty("min", IsRequired = true)]
        public float? Min
        {
            get { return (float?)base["min"]; }
            private set { base["min"] = value; }
        }

        [ConfigurationProperty("max", IsRequired = true)]
        public float? Max
        {
            get { return (float?)base["max"]; }
            private set { base["max"] = value; }
        }

        [ConfigurationProperty("scale", IsRequired = true)]
        public double Scale
        {
            get { return (double)base["scale"]; }
            private set { base["scale"] = value; }
        }
    }

    [ConfigurationCollection(typeof(StoreElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class PlotterElementCollection : ConfigurationElementCollection
    {
        public void Add(PlotterElement config)
        {
            BaseAdd(config, true);
        }

        public void Add(IEnumerable<PlotterElement> configs)
        {
            foreach (var config in configs)
            {
                Add(config);
            }
        }

        public PlotterElement this[int index]
        {
            get { return (PlotterElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new PlotterElement this[string name]
        {
            get { return (PlotterElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PlotterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return string.Format("{0}", (element as PlotterElement).Name);
        }
    }
}
