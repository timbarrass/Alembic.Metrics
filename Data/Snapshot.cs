using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class Snapshot : List<MetricData>
    {
        public string Name { get; set; }

        public List<string> Labels = new List<string>(); 
    }
}