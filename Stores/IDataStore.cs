using Data;

namespace Stores
{
    public interface IDataStore<T>
    {
        void Write(string name, Snapshot<T> data);

        Snapshot<T> Read(string name);

        bool Contains(string name);
    }
}