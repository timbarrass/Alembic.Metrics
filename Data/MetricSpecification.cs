namespace Data
{
    public struct MetricSpecification
    {
        public MetricSpecification(string name, float? expectedMin, float? expectedMax)
        {
            _name = name;
            _expectedMin = expectedMin;
            _expectedMax = expectedMax;
        }

        public float? ExpectedMax { get { return _expectedMax; } }

        public float? ExpectedMin { get { return _expectedMin; } }

        public string Name { get { return _name; } }

        private readonly float? _expectedMax;

        private readonly float? _expectedMin;

        private readonly string _name;
    }
}