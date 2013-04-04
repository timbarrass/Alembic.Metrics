using System.Linq;
using Configuration;
using Coordination;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class SimpleCounterBuilderTests
    {
        [Test]
        public void SimpleCounterBuilderParsesConfigDownToComponents()
        {
            const string id = "testId";
            const string name = "testCounter";
            const string sourceId = id + " Source";
            const string bufferId = id + " Buffer";
            const string storeId = id + " Store";
            const string plotterId = id + " Plotter";
            const string bufferChainId = id + " Buffer Chain";
            const string sinkChainId = id + " Sink Chain";
            const string preloadChainId = id + " Preload Chain";
            const string preloadScheduleId = id + " Preload Schedule";
            const string scheduleId = id + " Schedule";
            const string catgeory = "theCategory";
            const string counter = "theCounter";
            const string instance = "theInstance";
            const string machine = "theMachine";
            float? min = 0.0f;
            float? max = 0.0f;
            const int points = 10;
            const string outputPath = "thePath";
            const double scale = 0.1d;
            const int delay = 1;

            var simpleConfig = new SimpleCounterElement(id, name, catgeory, counter, instance, machine, min, max, points,
                                                        outputPath, scale, delay);

            var components = SimpleCounterBuilder.Build(simpleConfig);

            Assert.IsInstanceOf<BuiltComponents>(components);

            Assert.AreEqual(3, components.Sources.Count());
            Assert.IsTrue(components.Sources.All(s => s.Name == name));
            Assert.IsTrue(components.Sources.Any(s => s.Id == sourceId));
            Assert.IsTrue(components.Sources.Any(s => s.Id == bufferId));
            Assert.IsTrue(components.Sources.Any(s => s.Id == storeId));

            Assert.AreEqual(2, components.Sinks.Count());
            Assert.IsTrue(components.Sinks.All(s => s.Name == name));
            Assert.IsTrue(components.Sinks.Any(s => s.Id == bufferId));
            Assert.IsTrue(components.Sinks.Any(s => s.Id == storeId));

            Assert.AreEqual(1, components.Multisinks.Count());
            Assert.IsTrue(components.Multisinks.All(s => s.Name == name));
            Assert.IsTrue(components.Multisinks.Any(s => s.Id == plotterId));

            Assert.IsTrue(components.Chains.Links.Contains(bufferChainId));
            Assert.IsTrue(components.Chains.Links.Contains(sinkChainId));
            Assert.IsTrue(components.Chains.Links.Contains(preloadChainId));

            Assert.IsTrue(components.PreloadSchedules.Links.Contains(preloadScheduleId));
            Assert.IsTrue(components.Schedules.Links.Contains(scheduleId));
        }
        
    }
}