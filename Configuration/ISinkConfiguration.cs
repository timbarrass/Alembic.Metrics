using System.Configuration;

namespace Configuration
{
    public interface ISinkConfiguration : IConfiguration
    {
        [ConfigurationProperty("points", IsRequired = true)]
        int Points { get; }
    }
}