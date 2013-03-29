namespace Configuration
{
    public interface IStoreConfiguration : IConfiguration
    {
        string OutputPath { get; }
    }
}