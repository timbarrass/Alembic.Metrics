using System.Collections.Generic;
using System.Configuration;

namespace Sources
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

        [ConfigurationProperty("id")]
        public ProcessElementCollection Id
        {
            get { return (ProcessElementCollection)base["id"]; }
        }

        [ConfigurationProperty("machineName", DefaultValue = null)]
        public ProcessElementCollection MachineName
        {
            get { return (ProcessElementCollection)base["machineName"]; }
        }

    }
}
