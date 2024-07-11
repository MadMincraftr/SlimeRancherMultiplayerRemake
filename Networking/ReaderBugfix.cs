using Mirror;
using Mirror.Discovery;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking
{
    public class ReaderBugfix
    {
        public static void FixReaders()
        {
            Reader<ServerRequest>.read = new Func<NetworkReader, ServerRequest>((r) => NetworkReaderExtensions.ReadDiscoveryRequestMessage(r)); 
            Reader<ServerResponse>.read = new Func<NetworkReader, ServerResponse>((r) => NetworkReaderExtensions.ReadDiscoveryResponseMessage(r));
            Reader<TestLogMessage>.read = new Func<NetworkReader, TestLogMessage>((r) => NetworkReaderExtensions.ReadTestLogMessage(r));
            Reader<NetworkPingMessage>.read = new Func<NetworkReader, NetworkPingMessage>((r) => NetworkReaderExtensions.ReadPingMessage(r));
        }
    }
}
