using System;
using System.IO;
using Data;
using Mono.Options;
using Stores;

namespace SnapshotConverter
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: snapshotconverter -f|-file=<filename>");
                return;
            }

            var fileName = string.Empty;

            var p = new OptionSet()
                .Add("file=|f=", f => fileName = f);

            var unparsed = p.Parse(args);

            var theApp = new Program();
            theApp.Run(fileName);
        }

        private void Run(string snapshotFile )
        {
            var store = new FileSystemDataStore<IMetricData>();

            var snapshot = store.Read(snapshotFile);

            Console.WriteLine("Timestamp\tValue");

            foreach(var metricData in snapshot)
            {
                Console.WriteLine(metricData.Timestamp + "\t" + metricData.Data);
            }
        }
    }
}
