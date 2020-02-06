namespace Streamer.Model.Topology
{
    public interface ISink<TRecord>
    {
        void Save(TRecord record, OffsetVector<VirtualPartition> offsetVector);
    }
}