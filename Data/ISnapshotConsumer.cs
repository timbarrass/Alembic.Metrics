namespace Data
{
    public interface ISnapshotConsumer
    {
        string Name { get; }

        void ResetWith(Snapshot snapshot);
        
        void Update(Snapshot snapshot);
    }
}