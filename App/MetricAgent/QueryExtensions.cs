using System;
using log4net;
using Data;

namespace MetricAgent
{
    public static class QueryExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program).Name);

        public static bool TrySnapshot(this ISnapshotProvider source, out Snapshot metricData)
        {
            try
            {
                metricData = source.Snapshot();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Failed to query {0}: {1}", source.Spec.Name, ex.Message));

                metricData = null;

                return false;
            }
        }
    }

}
