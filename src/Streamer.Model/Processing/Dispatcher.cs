using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Streamer.Model.Configuration;

namespace Streamer.Model.Processing
{
    public class Dispatcher
    {
        public Dispatcher(StreamConfiguration streamConfiguration)
        {
        }

        public async Task Process<K, V>(string topic, int partition, IEnumerable<Record<K, V>> records)
        {

        }
    }

}