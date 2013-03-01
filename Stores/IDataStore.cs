using System.Collections.Generic;

namespace Stores
{
    public interface IDataStore<T>
    {
        void Write(string name, IEnumerable<T> data);

        IEnumerable<T> Read(string name);

        bool Contains(string name);
    }
}