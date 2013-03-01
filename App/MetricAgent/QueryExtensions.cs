using System;
using System.Collections.Generic;

using log4net;

using Sources;
using Data;

namespace MetricAgent
{
    public static class QueryExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program).Name);

        public static bool TryQuery(this IDataSource source, out IEnumerable<IMetricData> metricData)
        {
            try
            {
                metricData = source.Query();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Failed to query {0}: {1}", source.Name, ex.FormatMessage()));

                metricData = null;

                return false;
            }
        }
    }

}
