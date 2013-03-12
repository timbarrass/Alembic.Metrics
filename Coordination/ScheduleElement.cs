using System.Configuration;

namespace Coordination
{
    public class ScheduleElement : ConfigurationElement
    {
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

        [ConfigurationProperty("delay", IsRequired = true)]
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
}