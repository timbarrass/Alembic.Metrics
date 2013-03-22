using System.Configuration;

namespace Configuration
{
    public class SimpleCounterElement : ConfigurationElement, ICounterConfiguration
    {
        public SimpleCounterElement(string name, string categoryName, string counterName, string instanceName, string machineName, float? min, float? max, int points, string outputPath, double scale)
        {
            base["name"] = name;
            base["categoryName"] = categoryName;
            base["counterName"] = counterName;
            base["instanceName"] = instanceName;
            base["machineName"] = machineName;
            base["min"] = min;
            base["max"] = max;
            base["points"] = points;
            base["outputPath"] = outputPath;
            base["scale"] = scale;
        }

        public SimpleCounterElement()
        {
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("categoryName", IsRequired = true)]
        public string CategoryName
        {
            get { return (string)base["categoryName"]; }
        }

        [ConfigurationProperty("counterName", IsRequired = true)]
        public string CounterName
        {
            get { return (string)base["counterName"]; }
        }

        [ConfigurationProperty("instanceName"), DefaultSettingValue(null)]
        public string InstanceName
        {
            get { return (string)base["instanceName"]; }
        }

        [ConfigurationProperty("machineName"), DefaultSettingValue(null)]
        public string MachineName
        {
            get { return (string)base["machineName"]; }
        }

        [ConfigurationProperty("outputPath"), DefaultSettingValue(null)]
        public string OutputPath
        {
            get { return (string)base["outputPath"]; }
        }

        [ConfigurationProperty("min"), DefaultSettingValue(null)]
        public float? Min
        {
            get { return (float?)base["min"]; }
        }

        [ConfigurationProperty("max"), DefaultSettingValue(null)]
        public float? Max
        {
            get { return (float?)base["max"]; }
        }

        [ConfigurationProperty("points"), DefaultSettingValue(null)]
        public int Points
        {
            get { return (int)base["points"]; }
        }

        [ConfigurationProperty("scale"), DefaultSettingValue(null)]
        public double Scale
        {
            get { return (double)base["scale"]; }
        }
    }
}