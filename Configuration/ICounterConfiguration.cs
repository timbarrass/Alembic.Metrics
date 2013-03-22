using System.Configuration;

namespace Configuration
{
    public interface ICounterConfiguration : IConfiguration
    {
        [ConfigurationProperty("categoryName", IsRequired = true)]
        string CategoryName { get; }

        [ConfigurationProperty("counterName", IsRequired = true)]
        string CounterName { get; }

        [ConfigurationProperty("instanceName"), DefaultSettingValue(null)]
        string InstanceName { get; }

        [ConfigurationProperty("machineName"), DefaultSettingValue(null)]
        string MachineName { get; }
    }
}