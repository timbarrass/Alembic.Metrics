using System.Collections.Generic;
using Data;

namespace Sources
{
    public interface IDataSource
    {
        MetricSpecification Spec { get; }
        IEnumerable<IMetricData> Query();
        int Delay { get; }
        string Name { get; }
        string Id { get; }
    }
}