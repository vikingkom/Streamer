using System.Collections.Generic;

namespace Streamer.Model.Topology
{
    public class OffsetVector<TKey>
    {
        public Dictionary<TKey, long> Vectors;
    }
}