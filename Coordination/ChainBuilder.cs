using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using log4net;

namespace Coordination
{
    public class ChainBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChainBuilder).Name);

        public static IEnumerable<IChain> Build(IEnumerable<ISnapshotProvider> sources, IEnumerable<ISnapshotConsumer> sinks, IEnumerable<IMultipleSnapshotConsumer> multiSinks, ChainElementCollection configs)
        {
            var enumerableConfigs = new List<ChainElement>();

            foreach(ChainElement config in configs)
            {
                enumerableConfigs.Add(config);
            }

            return Build(sources, sinks, multiSinks, enumerableConfigs);
        }

        public static IEnumerable<IChain> Build(IEnumerable<ISnapshotProvider> sources, IEnumerable<ISnapshotConsumer> sinks, IEnumerable<IMultipleSnapshotConsumer> multiSinks, IEnumerable<ChainElement> configs)
        {
            var chains = new List<IChain>();

            sources = sources.ToArray();
            sinks = sinks.ToArray();
            multiSinks = multiSinks.ToArray();

            foreach(var config in configs)
            {
                try
                {
                    if (!sources.Any(s => config.Sources.Split(',').Any(i => i.Equals(s.Name))))
                    {
                        Log.Warn(string.Format("Couldn't find source '{0}' in the set of sources supplied for chain '{1}'.", config.Sources, config.Name));

                        continue;
                    }

                    if (!sinks.Any(s => config.Sinks.Split(',').Any(i => i.Equals(s.Name))) && !multiSinks.Any(s => config.MultiSinks.Split(',').Any(i => i.Equals(s.Name))))
                    {
                        Log.Warn(string.Format("Couldn't find one of sinks '{0}' in the set of sinks and multisinks supplied for chain '{1}'.", config.Sinks, config.Name));

                        continue;
                    }

                    if (config.Sources.Split(',').Count().Equals(1))
                    {                      
                        var chosenSource = sources.First(s => s.Name.Equals(config.Sources));

                        var chosenSinks = new List<ISnapshotConsumer>();

                        foreach (var sinkName in config.Sinks.Split(','))
                        {
                            chosenSinks.Add(sinks.First(s => s.Name.Equals(sinkName)));
                        }

                        chains.Add(new MultipleSinkChain(config.Name, chosenSource, chosenSinks.ToArray()));
                    }
                    else
                    {
                        var chosenSink = multiSinks.First(s => s.Name.Equals(config.MultiSinks));

                        var chosenSources = new List<ISnapshotProvider>();

                        foreach(var _ in config.Sources.Split(','))
                        {
                            var sourceName = _.TrimStart(' ');

                            chosenSources.Add(sources.First(s => s.Name.Equals(sourceName)));
                        }

                        chains.Add(new MultipleSourceChain(config.Name, chosenSink, chosenSources.ToArray()));
                    }
                }
                catch (InvalidOperationException)
                {
                    Log.Warn(string.Format("Couldn't construct chain: '{0}' '{1}' '{2}'", config.Name, config.Sources, config.Sinks));
                }
            }

            return chains;
        }
    }
}