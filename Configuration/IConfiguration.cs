using System.Configuration;

namespace Configuration
{
    public interface IConfiguration
    {
        [ConfigurationProperty("id", IsRequired = true)]
        string Id { get; }        

        [ConfigurationProperty("name", IsRequired = true)]
        string Name { get; }        
    }
}