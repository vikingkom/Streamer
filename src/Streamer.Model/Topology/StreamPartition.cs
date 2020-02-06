namespace Streamer.Model.Topology
{
    public class StreamPartition
    {
        public string Topic { get; set; }
        public int TopicPartitionIndex { get; set; }
    }

    public class VirtualPartition
    {
        public int TopicPartitionIndex { get; set; }
        public int StreamPartitionIndex { get; set; }
    }
}