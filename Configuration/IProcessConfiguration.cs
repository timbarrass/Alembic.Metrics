namespace Configuration
{
    public interface IProcessConfiguration : IConfiguration
    {
        string Exe { get; }

        string MachineName { get; }
    }
}