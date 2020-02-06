
namespace Streamer.Model.Configuration
{
    public class StreamConfiguration
    {
        public string BrokerConnectionString { get; set; }
        /// <summary>
        /// If true process only new messages, else process from the beginning
        /// </summary>
        public bool AutoOffsetLatest { get; set; }
        public string TopologyId { get; set; }
        public int VirtualPartitionsPerStreamPartition { get; set; }
        public int ThreadsCount { get; set; }
        public string[] Topics { get; set; }
    }
}