using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Data;

namespace Sinks
{
    public class FileSystemDataStore : ISnapshotConsumer, ISnapshotProvider
    {
        public FileSystemDataStore(string root, MetricSpecification specification)
        {
            _root = root;

            Directory.CreateDirectory(root);

            _spec = specification;
        }

        public void ResetWith(Snapshot snapshot)
        {
            var zipFileName = ZipFileName(_spec.Name);

            if (File.Exists(zipFileName))
            {
                File.Delete(zipFileName);
            }

            Update(snapshot);
        }

        public void Update(Snapshot snapshot)
        {
            var allowedAttempts = 3;
            var attempt = 1;

            var zipFileName = ZipFileName(_spec.Name);

            using (var os = new MemoryStream())
            {
                var bf = new BinaryFormatter();

                bf.Serialize(os, snapshot);

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

        public MetricSpecification Spec { get { return _spec; }}

        public Snapshot Snapshot()
        {
            var allowedAttempts = 3;
            var attempt = 1;

            var zipFileName = ZipFileName(_spec.Name);

            var snapshot = new Snapshot();

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
                    catch(FileNotFoundException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        if (attempt == allowedAttempts) throw;
                        
                        // As for Write -- default to simplest backoff
                        Thread.Sleep(100);

                        continue;
                    }

                    var bf = new BinaryFormatter();

                    snapshot = bf.Deserialize(gzo) as Snapshot;
                }
            }

            return snapshot;
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string name)
        {
            var fileName = ZipFileName(name);

            return File.Exists(fileName);
        }


        private string ZipFileName(string name)
        {
            name = Path.Combine(_root, name);

            if (name.EndsWith(".am.gz"))
            {
                return name;
            }

            var zipFileName = Path.ChangeExtension(name, ".am.gz");
            
            return zipFileName;
        }

        private string _root = ".";

        private readonly MetricSpecification _spec;
    }
}
