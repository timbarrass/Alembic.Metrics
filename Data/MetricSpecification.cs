namespace Data
{
    public class MetricSpecification
    {
        public MetricSpecification(string name, float? expectedMin, float? expectedMax)
        {
            Name = name;
            ExpectedMin = expectedMin;
            ExpectedMax = expectedMax;
        }

        public float? ExpectedMax { get; private set; }

        public float? ExpectedMin { get; private set; }

        public string Name { get; private set; }
    }
}