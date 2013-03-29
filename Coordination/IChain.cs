namespace Coordination
{
    public interface IChain
    {
        string Name { get; }

        void Update();
    }
}