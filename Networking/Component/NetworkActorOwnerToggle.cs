using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Component
{
    // Just a toggle thing
    [DisallowMultipleComponent]
    public class NetworkActorOwnerToggle : MonoBehaviour
    {
        public void OwnActor()
        {
            if (GetComponent<NetworkActor>().isOwned) return;

            // Inform server of owner change.
            var packet = new ActorUpdateOwnerMessage()
            {
                id = GetComponent<Identifiable>().model.actorId,
                player = SRNetworkManager.playerID
            };
            SRNetworkManager.NetworkSend(packet);
            
            // Owner change
            GetComponent<NetworkActor>().enabled = true;
            GetComponent<NetworkActor>().isOwned = true;
            GetComponent<TransformSmoother>().enabled = false;

            // Change the 'LARGE' vacuumable to not be held by player.
            var packet2 = new ActorChangeHeldOwnerMessage()
            {
                id = GetComponent<Identifiable>().model.actorId
            };
            SRNetworkManager.NetworkSend(packet2);
        }
    }
}
