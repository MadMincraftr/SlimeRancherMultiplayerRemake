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
    public class WriterBugfix
    {
        public static void FixWriters()
        {
            Writer<ServerRequest>.write = new Action<NetworkWriter, ServerRequest>((w, v) => NetworkWriterExtensions.Write(w, v));
            Writer<ServerResponse>.write = new Action<NetworkWriter, ServerResponse>((w, v) => NetworkWriterExtensions.Write(w, v));
            Writer<TestLogMessage>.write = new Action<NetworkWriter, TestLogMessage>((w, v) => NetworkWriterExtensions.Write(w, v));
            Writer<NetworkPingMessage>.write = new Action<NetworkWriter, NetworkPingMessage>((w, v) => NetworkWriterExtensions.Write(w, v));
        }
    }
}
