using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class ProcessUptimeSourceConfiguration : ConfigurationSection
    {
        public ProcessUptimeSourceConfiguration()
        {}

        public ProcessUptimeSourceConfiguration(IEnumerable<ProcessElement> configs)
        {
            foreach(var config in configs)
            {
                Processes.Add(config);
            }
        }

        [ConfigurationProperty("processes")]
        public ProcessElementCollection Processes
        {
            get { return (ProcessElementCollection)base["processes"]; }
        }

        [ConfigurationProperty("machineName", DefaultValue = null)]
        public ProcessElementCollection MachineName
        {
            get { return (ProcessElementCollection)base["machineName"]; }
        }

    }
}
