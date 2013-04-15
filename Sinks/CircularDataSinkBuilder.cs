using System.Collections.Generic;
using Configuration;
using Data;

namespace Sinks
{
    public class CircularDataSinkBuilder
    {
        public static IEnumerable<ISnapshotConsumerAndProvider> Build(CircularDataSinkConfiguration circularDataSinkConfiguration)
        {
            var circularDataSinks = new List<CircularDataSink>();

            foreach (SinkElement config in circularDataSinkConfiguration.Sinks)
            {
                circularDataSinks.Add(new CircularDataSink(config));
            }

            return circularDataSinks;
        }
    }
}