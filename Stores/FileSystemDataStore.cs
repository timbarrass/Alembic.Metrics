using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace Stores
{
    public class FileSystemDataStore<T> : IDataStore<T>
    {
        public void Write(string name, IEnumerable<T> data)
        {
            var zipFileName = ZipFileName(name);

            using(var os = new MemoryStream())
            using (var gzo = new FileStream(zipFileName, FileMode.Create))
            using (var gz = new GZipStream(gzo, CompressionMode.Compress))
            {
                var bf = new BinaryFormatter();

                bf.Serialize(os, data);

                os.Seek(0, SeekOrigin.Begin);

                os.CopyTo(gz);
            }
        }

        public IEnumerable<T> Read(string name)
        {
            var zipFileName = ZipFileName(name);

            IEnumerable<T> ret;

            using (var gzo = new MemoryStream())
            using (var gzi = new FileStream(zipFileName, FileMode.Open))
            using (var gz = new GZipStream(gzi, CompressionMode.Decompress))
            {
                gz.CopyTo(gzo);

                gzo.Seek(0, SeekOrigin.Begin);

                var bf = new BinaryFormatter();

                ret = bf.Deserialize(gzo) as IEnumerable<T>;
            }

            return ret;
        }

        private static string ZipFileName(string fileName)
        {
            var zipFileName = Path.ChangeExtension(fileName, ".am.gz");
            return zipFileName;
        }

        private static string FileName(string name)
        {
            var fileName = Path.ChangeExtension(name, "am");
            return fileName;
        }
    }
}
