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
            var packet = new ActorUpdateOwnerMessage()
            {
                id = GetComponent<Identifiable>().model.actorId,
                player = SRNetworkManager.playerID
            };
            SRNetworkManager.NetworkSend(packet);
            GetComponent<NetworkActor>().enabled = true;
            GetComponent<NetworkActor>().isOwned = true;
            GetComponent<TransformSmoother>().enabled = false;
        }
    }
}
