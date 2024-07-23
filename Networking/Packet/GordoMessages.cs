using Mirror;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

namespace SRMP.Networking.Packet
{
    public struct GordoEatMessage : NetworkMessage
    {
        public string id;
        public int count;
    }
    public struct GordoBurstMessage : NetworkMessage
    {
        public string id;
    }
}
