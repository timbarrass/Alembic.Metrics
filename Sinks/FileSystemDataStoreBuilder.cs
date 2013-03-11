using System.Collections.Generic;

namespace Sinks
{
    public class FileSystemDataStoreBuilder
    {
        public static IEnumerable<FileSystemDataStore> Build(FileSystemDataStoreConfiguration fileSystemDataStoreConfiguration)
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