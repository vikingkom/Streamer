namespace Streamer.Model.Topology
{
    public interface IStatefulTransformation<TRecord, TState, TResult> where TState: IStreamPartitionState
    {
        TRecord Transform(TState state, TRecord record);
    }
}
