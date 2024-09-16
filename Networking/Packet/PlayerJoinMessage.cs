using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Packet
{
    public struct PlayerJoinMessage : NetworkMessage
    {
        public int id;
        public bool local;
    }
    public struct PlayerLeaveMessage : NetworkMessage
    {
        public int id;
    }
}
