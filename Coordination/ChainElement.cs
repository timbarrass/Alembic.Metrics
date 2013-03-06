using System.Configuration;

namespace Coordination
{
    public class ChainElement : ConfigurationElement
    {
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
}