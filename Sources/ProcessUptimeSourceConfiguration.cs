using System.Configuration;

namespace Sources
{
    public class ProcessUptimeSourceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("processes")]
        public ProcessElementCollection Processes
        {
            get { return (ProcessElementCollection)base["processes"]; }
        }

        [ConfigurationProperty("id")]
        public ProcessElementCollection Id
        {
            get { return (ProcessElementCollection)base["id"]; }
        }

    }
}
