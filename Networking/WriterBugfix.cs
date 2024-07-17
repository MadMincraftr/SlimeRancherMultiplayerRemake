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
            Writer<SceneMessage>.write = new Action<NetworkWriter, SceneMessage>((w, v) => NetworkWriterExtensions.Write(w, v));
            Writer<AddPlayerMessage>.write = new Action<NetworkWriter, AddPlayerMessage>((w, v) => NetworkWriterExtensions.Write(w, v));
            Writer<ReadyMessage>.write = new Action<NetworkWriter, ReadyMessage>((w, v) => NetworkWriterExtensions.Write(w, v));
            Writer<NotReadyMessage>.write = new Action<NetworkWriter, NotReadyMessage>((w, v) => NetworkWriterExtensions.Write(w, v));
            Writer<TimeSnapshotMessage>.write = new Action<NetworkWriter, TimeSnapshotMessage>((w, v) => NetworkWriterExtensions.Write(w, v));
        }
    }
}
