using System.Net;
using Xunit;

namespace RadiusCore.Tests;

public class PacketHandlerRepositoryTests
{
    /// <summary>
    /// Test packet handler repository with matching ip
    /// </summary>      
    [Fact]
    public void TestPacketHandlerRepositoryIpSuccess()
    {
        var secret = "derp";
        var repo = new PacketHandlerRepository();
        repo.AddPacketHandler(IPAddress.Parse("127.0.0.1"), new MockPacketHandler(), secret);

        var result = repo.TryGetHandler(IPAddress.Parse("127.0.0.1"), out var handler);

        Assert.True(result);
        Assert.Equal(secret, handler.sharedSecret);
    }


    /// <summary>
    /// Test packet handler repository without matching ip
    /// </summary> 
    [Fact]
    public void TestPacketHandlerRepositoryIpFail()
    {
        var secret = "derp";
        var repo = new PacketHandlerRepository();
        repo.AddPacketHandler(IPAddress.Parse("127.0.0.1"), new MockPacketHandler(), secret);

        var result = repo.TryGetHandler(IPAddress.Parse("127.0.0.100"), out _);

        Assert.False(result);
    }


    /// <summary>
    /// Test packet handler repository with matching range
    /// </summary> 
    [Fact]
    public void TestPacketHandlerRepositoryRangeSuccess()
    {
        var secret = "derp";
        var repo = new PacketHandlerRepository();
        repo.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 24), new MockPacketHandler(), secret);

        var result = repo.TryGetHandler(IPAddress.Parse("10.0.0.254"), out var handler);

        Assert.True(result);
        Assert.Equal(secret, handler.sharedSecret);
    }


    /// <summary>
    /// Test packet handler repository without matching range
    /// </summary> 
    [Fact]
    public void TestPacketHandlerRepositoryRangeFail()
    {
        var secret = "derp";
        var repo = new PacketHandlerRepository();
        repo.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 24), new MockPacketHandler(), secret);

        var result = repo.TryGetHandler(IPAddress.Parse("10.0.1.1"), out _);

        Assert.False(result);
    }


    /// <summary>
    /// Test packet handler repository with catch all handler
    /// </summary> 
    [Fact]
    public void TestPacketHandlerRepositoryCatchAll()
    {
        var secret = "derp";
        var repo = new PacketHandlerRepository();
        repo.AddPacketHandler(IPAddress.Any, new MockPacketHandler(), secret);

        var result = repo.TryGetHandler(IPAddress.Parse("127.0.0.1"), out var handler);

        Assert.True(result);
        Assert.Equal(secret, handler.sharedSecret);
    }
}