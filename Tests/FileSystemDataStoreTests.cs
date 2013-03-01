using System.IO;
using System.Linq;
using NUnit.Framework;
using Stores;

namespace Tests
{
    [TestFixture]
    public class FileSystemDataStoreTests
    {
        [Test, Category("IntegrationTest")]
        public void FileSystemDataStore_CanPersistSimpleData()
        {
            var testData = new[] { new TestSerializable { Message = "Hello" } };

            var store = new FileSystemDataStore<TestSerializable>();

            store.Write("testData", testData);

            var ret = store.Read("testData");

            Assert.AreEqual(testData.First().Message, ret.First().Message);

            File.Delete("testData.am.gz");
        }

        [Test, Category("IntegrationTest")]
        public void FileSystemDataStore_AnswersContainsQueries()
        {
            var testData = new[] { new TestSerializable { Message = "Hello" } };

            var store = new FileSystemDataStore<TestSerializable>();

            store.Write("testData", testData);
            
            Assert.IsTrue(store.Contains("testData"));
            Assert.IsFalse(store.Contains("realData"));

            File.Delete("testData.am.gz");
        }

        [Test, Category("IntegrationTest")]
        public void FileSystemDataStore_HandlesRootDirectory()
        {
            var root = "Store";

            var store = new FileSystemDataStore<TestSerializable>(root);

            Assert.IsTrue(Directory.Exists(root));

            var filePath = Path.Combine(root, "testData.am.gz");

            var testData = new[] { new TestSerializable { Message = "Hello" } };

            store.Write("testData", testData);

            Assert.IsTrue(File.Exists(filePath));

            File.Delete(filePath);

            Directory.Delete(root);
        }
    }
}