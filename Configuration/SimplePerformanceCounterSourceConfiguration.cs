using System.Collections.Generic;
using System.Configuration;

namespace Configuration
{
    public class SimplePerformanceCounterSourceConfiguration : ConfigurationSection
    {
        public SimplePerformanceCounterSourceConfiguration()
        {
        }

        public SimplePerformanceCounterSourceConfiguration(IEnumerable<SimpleCounterElement> configs)
        {
            Counters.Add(configs);
        }

        [ConfigurationProperty("counters")]
        public SimpleCounterElementCollection Counters
        {
            get { return (SimpleCounterElementCollection)base["counters"]; }
        }
    }
}