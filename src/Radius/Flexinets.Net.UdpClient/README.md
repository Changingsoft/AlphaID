Flexinets.Net.IUdpClient

The purpose of the mock UdpClient is to test receiveing and sending UDP packets.
Currently it is only designed for limited use cases where a server receive an UDP packet, and then responds to it using the same UdpClient.

Usage
``` 
 var expected = "test";
var bytes = Encoding.UTF8.GetBytes(expected);

var client = new UdpClientMock();
var factory = new UdpClientMockFactory(client);

var task = factory.CreateClient(null).ReceiveAsync();

// This will wait until some other thread sends a response to the same udpclient
var response = await client.SendMock(new UdpReceiveResult(bytes, new IPEndPoint(IPAddress.Any, 1812)));

var actual = Encoding.UTF8.GetString(response.Buffer);

Assert.AreEqual(expected, actual);