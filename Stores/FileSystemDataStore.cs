using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Stores
{
    public class FileSystemDataStore<T> : IDataStore<T>
    {
        public void Write(string name, IEnumerable<T> data)
        {
            var allowedAttempts = 3;
            var attempt = 1;

            var zipFileName = ZipFileName(name);

            using (var os = new MemoryStream())
            {
                var bf = new BinaryFormatter();

                bf.Serialize(os, data);

                while (attempt++ <= allowedAttempts)
                {
                    try
                    {
                        using (var gzo = new FileStream(zipFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        using (var gz = new GZipStream(gzo, CompressionMode.Compress))
                        {
                            os.Seek(0, SeekOrigin.Begin);

                            os.CopyTo(gz);
                        }
                    }
                    catch (Exception)
                    {
                        if (attempt == allowedAttempts) throw;

                        // Default to simple block to avoid complication of checking for cancellation.
                        // If something more complex is needed, inject a backoff strategy for use here ...
                        Thread.Sleep(100);
                    }
                }
            }
        }

        public IEnumerable<T> Read(string name)
        {
            var allowedAttempts = 3;
            var attempt = 1;

            var zipFileName = ZipFileName(name);

            IEnumerable<T> ret = new List<T>();

            using (var gzo = new MemoryStream())
            {
                while (attempt++ <= allowedAttempts)
                {
                    try
                    {
                        using (var gzi = new FileStream(zipFileName, FileMode.Open))
                        using (var gz = new GZipStream(gzi, CompressionMode.Decompress))
                        {
                            gz.CopyTo(gzo);

                            gzo.Seek(0, SeekOrigin.Begin);
                        }

                    }
                    catch (Exception ex)
                    {
                        if (attempt == allowedAttempts) throw;
                        
                        // As for Write -- default to simplest backoff
                        Thread.Sleep(100);

                        continue;
                    }

                    var bf = new BinaryFormatter();

                    ret = bf.Deserialize(gzo) as IEnumerable<T>;
                }
            }

            return ret;
        }

        private static string ZipFileName(string fileName)
        {
            if (fileName.EndsWith(".am.gz"))
            {
                return fileName;
            }

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
