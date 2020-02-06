using System;
using Streamer.Model.Processing;

namespace Streamer.Model.Topology
{
    public interface ISource : IDisposable
    {
        ByteRecord GetNext();
    }
}