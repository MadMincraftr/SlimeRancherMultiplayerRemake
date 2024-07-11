using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking.Packet
{
    public struct TestLogMessage : NetworkMessage
    {
        public string MessageToLog;
    }
}
