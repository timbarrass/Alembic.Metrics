namespace Data
{
    public interface ISnapshotConsumer
    {
        void ResetWith(Snapshot snapshot);
        
        void Update(Snapshot snapshot);
    }
}