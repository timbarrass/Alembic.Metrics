using System;
using Data;

namespace Tests
{
    public class BreakingDataSink : ISnapshotConsumer
    {
        public string Name { get { return "BreakingDataSink"; } }

        public void ResetWith(Snapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public void Update(Snapshot snapshot)
        {
            throw new NotImplementedException();
        }
    }
}