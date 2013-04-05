namespace Configuration
{
    public interface IDatabaseConfiguration : IConfiguration
    {
        string Query { get; }

        string ConnectionString { get; }

        string Labels { get; }
    }
}