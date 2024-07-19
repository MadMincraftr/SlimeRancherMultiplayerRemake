using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Packet
{
    public struct PlayerUpdateMessage : NetworkMessage
    {
        public int id;
        public Vector3 pos;
        public Quaternion rot;
    }
}
