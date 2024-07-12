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

            Reader<ReadyMessage>.read = new Func<NetworkReader, ReadyMessage>((r) => NetworkReaderExtensions.ReadReadyMessage(r));
            Reader<NotReadyMessage>.read = new Func<NetworkReader, NotReadyMessage>((r) => NetworkReaderExtensions.ReadUnreadyMessage(r));
            Reader<TimeSnapshotMessage>.read = new Func<NetworkReader, TimeSnapshotMessage>((r) => NetworkReaderExtensions.ReadTimeSnapshotMessage(r));
            Reader<AddPlayerMessage>.read = new Func<NetworkReader, AddPlayerMessage>((r) => NetworkReaderExtensions.ReadAddPlayerMessage(r));
        }
    }
}
