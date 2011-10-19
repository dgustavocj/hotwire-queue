using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using FluentAssertions;

namespace Icodeon.Hotwire.TestFramework
{
    public class LMHostsTestHelper
    {
        public void ShouldResolveToLocalhost(string hostname)
        {
            var adresses = System.Net.Dns.GetHostAddresses(hostname);
            var address = adresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            address.Should().Be("localhost");
        }

        public void ShouldNotResolveHostName(string hostname)
        {
            Action action = () => System.Net.Dns.GetHostAddresses(hostname);
            action.ShouldThrow<SocketException>();
        }
    }
}
