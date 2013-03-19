using System;
using System.Collections.Generic;
using System.Linq;
using Coordination;
using Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests
{
    [TestFixture]
    public class ScheduleBuilderTests
    {
        [Test]
        // ChainWorkerBuilder -> ChainThreadBuilder -> ChainTaskBuilder -> SnapshotUpdaterBuilder
        // -> TimedSnapshotUpdaterBuilder -> SnapshotScheduleBuilder -> DelayedScheduleBuilder
        // -> ScheduleBuilder
        public void ScheduleBuilderCanBuildAScheduleForMultipleChains()
        {
            var scheduleName = "testSchedule";
            var delay = 10;

            var snapshot = new Snapshot { new MetricData(0.5, DateTime.Parse("12 Aug 2008")) };

            var configs = new List<ChainElement>
                {
                    new ChainElement("firstTestChain", "testSource", "testBuffer", ""),
                    new ChainElement("secondTestChain", "testSource", "testBuffer", "")
                };

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot).Repeat.Any();
            source.Expect(s => s.Name).Return("testSource").Repeat.Any();

            var buffer = MockRepository.GenerateMock<ISnapshotConsumer>();
            buffer.Expect(b => b.Update(snapshot));
            buffer.Expect(s => s.Name).Return("testBuffer").Repeat.Any();

            var sources = new HashSet<ISnapshotProvider> { source };

            var sinks = new HashSet<ISnapshotConsumer> { buffer };

            var chains = ChainBuilder.Build(sources, sinks, new HashSet<IMultipleSnapshotConsumer>(), configs);

            var schedule = ScheduleBuilder.Build(scheduleName, delay, chains);

            Assert.IsInstanceOf<ISchedule>(schedule);
            Assert.AreEqual(scheduleName, schedule.Name);
            Assert.AreEqual(2, schedule.Chains.Count());
        }

        [Test]
        public void ScheduleBuilderCanBuildAScheduleForMultipleChainsFromConfig()
        {
            var scheduleName = "testSchedule";
            var delay = 10;

            var snapshot = new Snapshot { new MetricData(0.5, DateTime.Parse("12 Aug 2008")) };

            var configs = new List<ChainElement>
                {
                    new ChainElement("firstTestChain", "testSource", "testBuffer", ""),
                    new ChainElement("secondTestChain", "testSource", "testBuffer", "")
                };

            var source = MockRepository.GenerateMock<ISnapshotProvider>();
            source.Expect(s => s.Snapshot()).Return(snapshot).Repeat.Any();
            source.Expect(s => s.Name).Return("testSource").Repeat.Any();

            var buffer = MockRepository.GenerateMock<ISnapshotConsumer>();
            buffer.Expect(b => b.Update(snapshot));
            buffer.Expect(s => s.Name).Return("testBuffer").Repeat.Any();

            var sources = new HashSet<ISnapshotProvider> { source };

            var sinks = new HashSet<ISnapshotConsumer> { buffer };

            var chains = ChainBuilder.Build(sources, sinks, new HashSet<IMultipleSnapshotConsumer>(), configs);

            var scheduleConfig = new ScheduleElement(scheduleName, delay, string.Join(",", "firstTestChain", "secondTestChain"));

            var schedules = ScheduleBuilder.Build(new [] { scheduleConfig }, chains);

            Assert.IsInstanceOf<IEnumerable<ISchedule>>(schedules);
            Assert.AreEqual(scheduleName, schedules.First().Name);
            Assert.AreEqual(2, schedules.First().Chains.Count());
        }
    }
}