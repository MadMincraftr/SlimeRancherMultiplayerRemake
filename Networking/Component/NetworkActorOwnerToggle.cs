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

        /// <summary>
        /// Use this to drop the current largo held for this client.
        /// </summary>
        public void LoseGrip()
        {
            WeaponVacuum vac = SceneContext.Instance.player.GetComponentInChildren<WeaponVacuum>();

            if (vac.held == gameObject)
            {
                // SLIGHTLY MODIFIED SR CODE
                Vacuumable vacuumable = vac.held.GetComponent<Vacuumable>();

                if (vacuumable != null)
                {
                    vacuumable.release();
                }

                vac.lockJoint.connectedBody = null;
                Identifiable ident = vac.held.GetComponent<Identifiable>();
                vac.held = null;
                vac.SetHeldRad(0f);

                vac.heldStartTime = double.NaN;
            }
        }
        /// <summary>
        /// This is for transfering actor ownership to another player. Recommended for when you want a client to control a feature on the actor. 
        /// </summary>
        public void OwnActor()
        {
            
            // Owner change
            GetComponent<NetworkActor>().enabled = true;
            GetComponent<NetworkActor>().IsOwned = true;
            GetComponent<TransformSmoother>().enabled = false;

            // Inform server of owner change.
            var packet = new ActorUpdateOwnerMessage()
            {
                id = GetComponent<Identifiable>().model.actorId,
                player = SRNetworkManager.playerID
            };
            SRNetworkManager.NetworkSend(packet);

            // Combining with normal owner packet.
            // Change the 'LARGE' vacuumable to not be held by player.
            /*var packet2 = new ActorChangeHeldOwnerMessage()
            {
                id = GetComponent<Identifiable>().model.actorId
            };
            SRNetworkManager.NetworkSend(packet2);*/
        }
    }
}
