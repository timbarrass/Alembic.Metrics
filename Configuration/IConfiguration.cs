using System.Configuration;

namespace Configuration
{
    public interface IConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true)]
        string Name { get; }        
    }
}