using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Configuration;
using Data;
using log4net;

namespace Sinks
{
    public class FileSystemDataStore : ISnapshotConsumerAndProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileSystemDataStore).Name);

        public FileSystemDataStore(StoreElement config)
            : this(config.OutputPath, config.Name, config.Id)
        {
        }

        public FileSystemDataStore(string root, string name, string id)
        {
            _id = id;

            _root = root;

            _name = name;

            Directory.CreateDirectory(root);
        }

        public string Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public void ResetWith(Snapshot snapshot)
        {
            var fileName = FileName(Name);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            Update(snapshot);
        }

        public void Update(Snapshot snapshot)
        {
            CheckDataStructure(snapshot);

            using(var file = File.Create(FileName(Name)))
            using(var w = new StreamWriter(file))
            {
                w.WriteLine(string.Join(",", snapshot[0].Labels));

                foreach(var dataPoint in snapshot)
                {
                    w.Write(string.Format("{0}", dataPoint.Timestamp));
                    
                    foreach(var value in dataPoint.Data)
                    {
                        w.Write(string.Format(",{0}", value.HasValue ? value.Value.ToString() : "null"));
                    }

                    w.WriteLine(); w.Flush();
                }
            }
        }

        private IList<string> _labels;

        private void CheckDataStructure(Snapshot snapshot)
        {
            foreach (var metric in snapshot)
            {
                if (_labels == null) _labels = metric.Labels;

                if (!metric.Labels.SequenceEquals(_labels))
                {
                    throw new InvalidOperationException(
                        string.Format("Data structure for {0} has changed '{1}':'{2}'",
                                      Name,
                                      string.Join(",", _labels),
                                      string.Join(",", metric.Labels)));
                }
            }
        }

        public Snapshot Snapshot()
        {
            var snapshot = new Snapshot();

            if(! File.Exists(FileName(Name)))
            {
                Log.Warn(string.Format("Attempted to snapshot '{0}', but no underlying file was found.", Name));

                return snapshot;
            }

            using (var file = File.OpenRead(FileName(Name)))
            using (var r = new StreamReader(file))
            {
                string line = r.ReadLine();

                if (line == null) return new Snapshot();

                var labels = line.Split(',').ToList();

                while ((line = r.ReadLine()) != null)
                {
                    var data = line.Split(',').ToList();

                    var dataPoints = new List<double?>();

                    for (int i = 1; i < data.Count; i++)
                    {
                        dataPoints.Add(data[i].Equals("null") ? new double?() : double.Parse(data[i]));
                    }

                    snapshot.Add(new MetricData(dataPoints, DateTime.Parse(data[0]), labels));
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
            var fileName = FileName(name);

            return File.Exists(fileName);
        }

        private string FileName(string name)
        {
            name = Path.Combine(_root, name);

            if (name.EndsWith(".am"))
            {
                return name;
            }

            var zipFileName = Path.ChangeExtension(name, ".am");

            return zipFileName;
        }

        private readonly string _root = ".";

        private readonly string _name;

        private readonly string _id;
    }
}
