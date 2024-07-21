using Mirror;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

namespace SRMP.Networking.Packet
{
    public struct ActorSpawnClientMessage : NetworkMessage
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 velocity;
        public Identifiable.Id ident;
        public RegionRegistry.RegionSetId region;
        public int player;
    }
}
