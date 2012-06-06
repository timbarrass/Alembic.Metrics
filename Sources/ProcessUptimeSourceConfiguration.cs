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
    }
}
