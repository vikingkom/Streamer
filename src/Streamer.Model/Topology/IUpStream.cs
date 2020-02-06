namespace Streamer.Model.Topology
{
    public interface IUpStream
    {
        IStreamNode[] DownStreamNodes { get; set; }
    }
}