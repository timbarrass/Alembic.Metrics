namespace Data
{
    public interface ISnapshotConsumer : ISnapshotHandler
    {
        void ResetWith(Snapshot snapshot);
        
        void Update(Snapshot snapshot);
    }
}