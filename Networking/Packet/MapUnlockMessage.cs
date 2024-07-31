using Mirror;

namespace SRMP.Networking.Packet
{
    public struct MapUnlockMessage : NetworkMessage
    {
        public ZoneDirector.Zone id;
    }
}
