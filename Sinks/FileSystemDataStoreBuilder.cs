using System.Collections.Generic;
using Configuration;
using Data;

namespace Sinks
{
    public class FileSystemDataStoreBuilder
    {
        public static IEnumerable<ISnapshotConsumerAndProvider> Build(FileSystemDataStoreConfiguration fileSystemDataStoreConfiguration)
        {
            var fileSystemDataStores = new List<FileSystemDataStore>();

            foreach (StoreElement config in fileSystemDataStoreConfiguration.Stores)
            {
                fileSystemDataStores.Add(new FileSystemDataStore(config));
            }

            return fileSystemDataStores;
        }
    }
}