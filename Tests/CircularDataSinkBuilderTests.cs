using System.Linq;
using Configuration;
using NUnit.Framework;
using Sinks;

namespace Tests
{
    [TestFixture]
    public class CircularDataSinkBuilderTests
    {
        [Test]
        public void BuildsUsingConfigurationObject()
        {
            var name = "testSink";

            var configs = new[]
                {
                    new SinkElement("id", name, 10, null, null)
                };

            var configCollection = new CircularDataSinkConfiguration(configs);

            var sinks = CircularDataSinkBuilder.Build(configCollection);

            Assert.AreEqual(1, sinks.Count());
        }
    }
}