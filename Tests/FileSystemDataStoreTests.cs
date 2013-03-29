using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using NUnit.Framework;
using Sinks;

namespace Tests
{
    [TestFixture]
    public class FileSystemDataStoreTests
    {
        [Test]
        public void FileSystemDataStoreIsASnapshotConsumerAndProvider()
        {
            var store = new FileSystemDataStore(".", "testStore");

            Assert.IsInstanceOf<ISnapshotConsumer>(store);
            Assert.IsInstanceOf<ISnapshotProvider>(store);
        }

        [Test, Category("IntegrationTest")]
        public void FileSystemDataStore_CanPersistSimpleData()
        {
            var testData = new Snapshot { new MetricData(2.5d, DateTime.Now) };

            var store = new FileSystemDataStore(".", "testStore");

            store.Update(testData);

            var ret = store.Snapshot();

            Assert.AreEqual(testData.First().Data, ret.First().Data);

            File.Delete("testData.am.gz");
        }

        [Test, Category("IntegrationTest")]
        public void FileSystemDataStore_AnswersContainsQueries()
        {
            var testData = new Snapshot { new MetricData(2.5d, DateTime.Now) };

            var store = new FileSystemDataStore(".", "testData");

            store.ResetWith(testData);
            
            Assert.IsTrue(store.Contains("testData"));
            Assert.IsFalse(store.Contains("realData"));

            File.Delete("testData.am.gz");
        }

        [Test, Category("IntegrationTest")]
        public void FileSystemDataStore_HandlesRootDirectory()
        {
            var root = "Store";

            var store = new FileSystemDataStore(root, "testData");

            Assert.IsTrue(Directory.Exists(root));

            var filePath = Path.Combine(root, "testData.am.gz");

            var testData = new Snapshot { new MetricData(2.5d, DateTime.Now) };

            store.ResetWith(testData);

            Assert.IsTrue(File.Exists(filePath));

            File.Delete(filePath);

            Directory.Delete(root);
        }

        [Test]
        public void JustWarnsandReturnsEmptySnapshotIfFileDoesNotExistOnSnapshot()
        {
            var root = "Store";

            var store = new FileSystemDataStore(root, "testData");

            var snapshot = store.Snapshot();

            Assert.AreEqual(0, snapshot.Count());
        }
    }
}