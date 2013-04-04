namespace Coordination
{
    public interface IChain
    {
        string Name { get; }

        string Id { get; }

        void Update();
    }
}