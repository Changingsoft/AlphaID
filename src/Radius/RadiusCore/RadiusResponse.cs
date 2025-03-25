
using System.Net;

namespace RadiusCore;

public class RadiusResponse
{
    public int Length { get; internal set; }
    public IPEndPoint? Remote { get; internal set; }

    internal byte[] ToBytes()
    {
        throw new NotImplementedException();
    }
}