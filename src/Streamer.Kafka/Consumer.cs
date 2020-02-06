using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confluent.Kafka;
using Streamer.Model.Configuration;
using Streamer.Model.Processing;
using Streamer.Model.State;
using Streamer.Model.Topology;

namespace Streamer.Kafka
{
    public class Consumer : ISource
    {
        private readonly int topicsCount;
        private readonly IStateStore stateStore;
        private readonly CancellationToken cancellationToken;
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly Dictionary<int,int> partitionToTopicsAssigned = new Dictionary<int, int>();
        private readonly HashSet<int> streamPartitions = new HashSet<int>();
        public Consumer(StreamConfiguration streamConfiguration, IStateStore stateStore, ConsumerConfig consumerConfig = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.stateStore = stateStore;
            this.cancellationToken = cancellationToken;
            var config = EnrichConsumerConfig(streamConfiguration, consumerConfig);
            topicsCount = streamConfiguration.Topics.Length;
;            //assume PartitionsAssignedHandler and PartitionsRevokedHandler are called sequentially
            this.consumer = new ConsumerBuilder<byte[], byte[]>(config).SetPartitionsAssignedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                    partitions.ForEach(f =>
                    {
                        int topicsAssigned = partitionToTopicsAssigned.TryGetValue(f.Partition.Value, out topicsAssigned) ? topicsAssigned + 1 : 1;
                        partitionToTopicsAssigned[f.Partition.Value] = topicsAssigned;
                        if (topicsAssigned == topicsCount)
                            AssignStreamPartition(f.Partition.Value);
                    });
                    
                    //todo: Check when partition in all requested topic is assigned
                    
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                    
                    partitions.ForEach(p =>
                    {
                        int topicsAssigned = partitionToTopicsAssigned[p.Partition.Value];
                        
                        if (topicsAssigned == topicsCount)
                            RevokeStreamPartition(p.Partition.Value);

                        partitionToTopicsAssigned[p.Partition.Value] = topicsAssigned - 1;
                        stateStore.SaveTopicPartition(p.Topic, p.Partition.Value);
                    });
                }).Build();
            consumer.Subscribe(streamConfiguration.Topics);

        }

        private void AssignStreamPartition(int streamPartitionIndex)
        {
            
            var offsets = stateStore.GetStreamPartitionOffsets(streamPartitionIndex);
            //todo: check that Assign could be called thread-safe with Consume
            consumer.Assign(offsets.Vectors.Select(s=>new TopicPartitionOffset(new TopicPartition(s.Key.Topic, new Partition(s.Key.TopicPartitionIndex)),new Offset(s.Value))));
            streamPartitions.Add(streamPartitionIndex);
        }

        private void RevokeStreamPartition(int streamPartitionIndex)
        {
            streamPartitions.Remove(streamPartitionIndex);
            stateStore.SaveStreamPartitionState(streamPartitionIndex);
        }

        public ByteRecord GetNext()
        {
            while (true)
            {
                var consumeResult = consumer.Consume(cancellationToken);
                if (streamPartitions.Contains(consumeResult.Partition.Value))
                    return new ByteRecord()
                    {
                        Topic = consumeResult.Topic,
                        Partition = consumeResult.Partition.Value,
                        Offset = consumeResult.Offset.Value,
                        Data = new RecordData<byte[], byte[]>()
                        {
                            Key = consumeResult.Key,
                            Value = consumeResult.Value
                        }
                    };
            }
        }

        private static ConsumerConfig EnrichConsumerConfig(StreamConfiguration streamConfig, ConsumerConfig consumerConfig)
        {
            consumerConfig.AutoOffsetReset = streamConfig.AutoOffsetLatest?AutoOffsetReset.Latest:AutoOffsetReset.Earliest;
            consumerConfig.EnableAutoCommit = false;
            consumerConfig.EnableAutoOffsetStore = false;
            consumerConfig.StatisticsIntervalMs = 0; 
            consumerConfig.GroupId = streamConfig.TopologyId;
            consumerConfig.PartitionAssignmentStrategy = PartitionAssignmentStrategy.Range;
            consumerConfig.BootstrapServers = streamConfig.BrokerConnectionString;
            return consumerConfig;
        }

        public void Dispose()
        {
            consumer.Close();
            consumer?.Dispose();
        }
    }
}
