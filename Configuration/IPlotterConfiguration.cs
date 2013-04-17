namespace Configuration
{
    public interface IPlotterConfiguration : IConfiguration
    {
        string OutputPath { get; }

        float? Min { get; }

        float? Max { get; }

        double Scale { get; }

        string Areas { get; }
    }
}