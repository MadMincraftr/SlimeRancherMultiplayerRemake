using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking
{
    public class NetworkHandler
    {
        public class Server
        {
            internal static void Start()
            {
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, TestLogMessage>(HandleTestLog));
            }
            public static void HandleTestLog(NetworkConnectionToClient nctc, TestLogMessage packet)
            {
                SRMP.Log(packet.MessageToLog);
            }
        }
    }
}
