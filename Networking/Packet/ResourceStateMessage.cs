using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Packet
{
    public struct ResourceStateMessage : NetworkMessage
    {
        public ResourceCycle.State state;
        public long id;
    }
}
