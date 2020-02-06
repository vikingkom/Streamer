namespace Streamer.Model.Topology
{
    public interface IStatelessTransformation<TRecord, TState, TResult> where TState : IStreamPartitionState
    {
        TRecord Transform(TRecord record);
    }
}