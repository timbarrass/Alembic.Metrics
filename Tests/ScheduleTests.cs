using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using Coordination;
using Data;
using NUnit.Framework;
using Sources;

namespace Tests
{
    [TestFixture]
    public class ScheduleTests
    {
        [Test]
        public void SchedulesCanBeRunOnce()
        {
            var scheduleName = "testSchedule";
            var delay = 10;
            var chainName1 = "testChain";
            var chainName2 = "testChain2";

            var chains = new[]
                {
                    new MultipleSinkChain("id1", chainName1, new NullSource(), new ISnapshotConsumer[] {}),
                    new MultipleSinkChain("id2", chainName2, new NullSource(), new ISnapshotConsumer[] {})
                };

            var config = new ScheduleElement(scheduleName, delay, string.Join(",", chainName1, chainName2));

            var schedule = new Schedule(config, chains);

            schedule.RunOnce();

            // mock out source, sink and assert
        }

        [Test]
        public void ScheduleCAnBeStartedAndStopped()
        {
            var scheduleName = "testSchedule";
            var delay = 10;
            var chainName1 = "testChain";
            var chainName2 = "testChain2";

            var chains = new[]
                {
                    new MultipleSinkChain("id1", chainName1, new NullSource(), new ISnapshotConsumer[] {}),
                    new MultipleSinkChain("id2", chainName2, new NullSource(), new ISnapshotConsumer[] {})
                };

            var config = new ScheduleElement(scheduleName, delay, string.Join(",", chainName1, chainName2));

            var schedule = new Schedule(config, chains);

            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;

            var task = Task.Factory.StartNew(() => schedule.Start(cancellationToken), cancellationToken);

            Thread.Sleep(1000);

            tokenSource.Cancel();

            task.Wait();
        }

        [Test]
        public void ScheduleCanBeInstantiatedUsingConfiguration()
        {
            var scheduleName = "testSchedule";
            var delay = 10;
            var chainName1 = "testChain";
            var chainName2 = "testChain2";

            var chains = new[]
                {
                    new MultipleSinkChain("id1", chainName1, new NullSource(), new ISnapshotConsumer[] {}),
                    new MultipleSinkChain("id2", chainName2, new NullSource(), new ISnapshotConsumer[] {})
                };

            var config = new ScheduleElement(scheduleName, delay, string.Join(",", "id1", "id2"));

            var schedule = new Schedule(config, chains);

            Assert.AreEqual(scheduleName, schedule.Name);
            Assert.AreEqual(delay * 1000, schedule.Delay);
            Assert.AreEqual(2, schedule.Chains.Count());
        }    
    }
}