using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Stores
{
    public class FileSystemDataStore<T> : IDataStore<T>
    {
        public void Write(string name, IEnumerable<T> data)
        {
            var fileName = FileName(name);

            using(var os = new FileStream(fileName, FileMode.Create))
            {
                var bf = new BinaryFormatter();

                foreach(var item in data)
                {
                    bf.Serialize(os, data);
                }
            }
        }

        private static string FileName(string name)
        {
            var fileName = Path.ChangeExtension(name, "am");
            return fileName;
        }

        public IEnumerable<T> Read(string name)
        {
            var fileName = FileName(name);

            IEnumerable<T> ret;

            using (var ins = new FileStream(fileName, FileMode.Open))
            {
                var bf = new BinaryFormatter();

                ret = bf.Deserialize(ins) as IEnumerable<T>;
            }

            return ret;
        }
    }
}
