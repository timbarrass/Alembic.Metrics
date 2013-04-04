using System.Linq;
using Configuration;
using NUnit.Framework;
using Sinks;

namespace Tests
{
    [TestFixture]
    public class FileSystemDataStoreBuilderTests
    {
        [Test]
        public void BuildsUsingAConfigurationObject()
        {
            var name = "firstStore";

            var configs = new []
                {
                    new StoreElement("id", name, ".")
                };

            var configCollection = new FileSystemDataStoreConfiguration(configs);

            var stores = FileSystemDataStoreBuilder.Build(configCollection);

            Assert.AreEqual(1, stores.Count());
            Assert.AreEqual(name, stores.First().Name);
        }
    }
}