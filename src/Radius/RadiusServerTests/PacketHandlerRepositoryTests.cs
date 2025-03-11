using Microsoft.AspNetCore.HttpOverrides;
using NUnit.Framework;
using System.Net;

namespace Flexinets.Radius.Tests
{
    [TestFixture]
    public class PacketHandlerRepositoryTests
    {
        /// <summary>
        /// Test packet handler repository with matching ip
        /// </summary>      
        [TestCase]
        public void TestPacketHandlerrepositoryIpSuccess()
        {
            var secret = "derp";
            var repo = new PacketHandlerRepository();
            repo.AddPacketHandler(IPAddress.Parse("127.0.0.1"), new MockPacketHandler(), secret);

            var result = repo.TryGetHandler(IPAddress.Parse("127.0.0.1"), out var handler);

            Assert.IsTrue(result);
            Assert.AreEqual(secret, handler.sharedSecret);
        }


        /// <summary>
        /// Test packet handler repository without matching ip
        /// </summary> 
        [TestCase]
        public void TestPacketHandlerrepositoryIpFail()
        {
            var secret = "derp";
            var repo = new PacketHandlerRepository();
            repo.AddPacketHandler(IPAddress.Parse("127.0.0.1"), new MockPacketHandler(), secret);

            var result = repo.TryGetHandler(IPAddress.Parse("127.0.0.100"), out var handler);

            Assert.IsFalse(result);
        }


        /// <summary>
        /// Test packet handler repository with matching range
        /// </summary> 
        [TestCase]
        public void TestPacketHandlerrepositoryRangeSuccess()
        {
            var secret = "derp";
            var repo = new PacketHandlerRepository();
            repo.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 24), new MockPacketHandler(), secret);

            var result = repo.TryGetHandler(IPAddress.Parse("10.0.0.254"), out var handler);

            Assert.IsTrue(result);
            Assert.AreEqual(secret, handler.sharedSecret);
        }


        /// <summary>
        /// Test packet handler repository without matching range
        /// </summary> 
        [TestCase]
        public void TestPacketHandlerrepositoryRangeFail()
        {
            var secret = "derp";
            var repo = new PacketHandlerRepository();
            repo.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 24), new MockPacketHandler(), secret);

            var result = repo.TryGetHandler(IPAddress.Parse("10.0.1.1"), out var handler);

            Assert.IsFalse(result);
        }


        /// <summary>
        /// Test packet handler repository with catch all handler
        /// </summary> 
        [TestCase]
        public void TestPacketHandlerrepositoryCatchAll()
        {
            var secret = "derp";
            var repo = new PacketHandlerRepository();
            repo.AddPacketHandler(IPAddress.Any, new MockPacketHandler(), secret);

            var result = repo.TryGetHandler(IPAddress.Parse("127.0.0.1"), out var handler);

            Assert.IsTrue(result);
            Assert.AreEqual(secret, handler.sharedSecret);
        }
    }
}
