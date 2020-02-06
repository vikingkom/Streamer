using Streamer.Model.Topology;

namespace Streamer.Model.State
{
    public interface IStateRepository
    {
        void SaveOffset(string topic, int topicPartitionIndex, int streamPartitionIndex, long offset);
        long[] GetTopicPartitionOffsets(string topic, int topicPartitionIndex);
    }

    public interface IStateStore
    {
        OffsetVector<StreamPartition> GetStreamPartitionOffsets(int streamPartitionIndex);
        long GetTopicPartitionOffset(string topic, int partitionIndex);
        void SaveTopicPartition(string topic, int partitionIndex);
        void SaveStreamPartitionState(int streamPartitionIndex);
    }
}