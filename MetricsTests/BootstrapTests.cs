using System;
using Metrics;
using NUnit.Framework;

namespace MetricsTests
{
    [TestFixture]
    public class BootstrapTests
    {
        [Test]
        public void PerfCounterDataSource_CanBeInstantiated()
        {
            IDataSource source = new PerformanceCounterDataSource();

            Assert.IsInstanceOf<PerformanceCounterDataSource>(source);
        }

        [Test]
        public void PerfCounterDataSource_FiresAnEventOnNewMetric()
        {
            IDataSource source = new PerformanceCounterDataSource();

            source.OnNewMetric += NewMetric;

            source.Query();

            Assert.IsTrue(_newMetric);
        }

        [Test]
        public void PerfCounterDataSource_FiresAnEventContainingMetricData()
        {
            IDataSource source = new PerformanceCounterDataSource();

            source.OnNewMetric += NewMetric;

            source.Query();

            Assert.IsTrue(_newMetric);
            Assert.AreEqual("Committed Bytes", _newMetricName);
            /*Assert.AreEqual(1.5, _newMetricData);
            Assert.AreEqual(new DateTime(2011, 10, 21), _newMetricTimestamp);*/
        }

        private bool _newMetric;

        private string _newMetricName;

        private double _newMetricData;

        private DateTime _newMetricTimestamp;

        private void NewMetric(object sender, MetricData e)
        {
            _newMetric = true;
            _newMetricName = e.Name;
            _newMetricData = e.Value;
            _newMetricTimestamp = e.Timestamp;
        }
    }
}
