namespace Configuration
{
    public interface IScheduleConfiguration : IConfiguration
    {
        int Delay { get; }
    }
}