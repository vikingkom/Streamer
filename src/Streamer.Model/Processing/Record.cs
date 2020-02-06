namespace Streamer.Model.Processing
{
    public class Record<K,V>
    {
        public RecordData<K,V> Data { get; set; }
        public long Offset { get; set; }
        public string Topic { get; set; }
        public int Partition { get; set; }
    }

    public class ByteRecord: Record<byte[], byte[]>
    {
        
    }

    public class RecordData<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }
}