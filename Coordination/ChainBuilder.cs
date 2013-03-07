using System.Collections.Generic;
using System.Linq;
using Data;
using log4net;

namespace Coordination
{
    public class ChainBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChainBuilder).Name);

        public static IEnumerable<Chain> Build(IEnumerable<ISnapshotProvider> sources, IEnumerable<ISnapshotConsumer> sinks, ChainElementCollection configs)
        {
            var enumerableConfigs = new List<ChainElement>();

            foreach(ChainElement config in configs)
            {
                enumerableConfigs.Add(config);
            }

            return Build(sources, sinks, enumerableConfigs);
        }

        public static IEnumerable<Chain> Build(IEnumerable<ISnapshotProvider> sources, IEnumerable<ISnapshotConsumer> sinks, IEnumerable<ChainElement> configs)
        {
            var chains = new List<Chain>();

            sources = sources.ToArray();
            sinks = sinks.ToArray();

            
            foreach(var config in configs)
            {
                if (!sources.Any(s => s.Name.Equals(config.Source)))
                {
                    Log.Warn(string.Format("Couldn't find source '{0}' in the set of sources supplied.", config.Name));

                    continue;
                }

                if (!sinks.Any(s => config.Sinks.Split(',').Any(i => i.Equals(s.Name))))
                {
                    Log.Warn(string.Format("Couldn't find sink '{0}' in the set of sinks supplied.", config.Name));

                    continue;
                }

                var chosenSource = sources.First(s => s.Name.Equals(config.Source));

                var chosenSinks = new List<ISnapshotConsumer>();

                foreach(var sinkName in config.Sinks.Split(','))
                {
                    chosenSinks.Add(sinks.First(s => s.Name.Equals(sinkName)));
                }

                chains.Add(new Chain(config.Name, chosenSource, chosenSinks.ToArray()));
            }

            return chains;
        }
    }
}