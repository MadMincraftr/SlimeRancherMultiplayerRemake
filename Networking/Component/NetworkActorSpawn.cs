using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SECTR_AudioSystem;

namespace SRMP.Networking.Component
{
    public class NetworkActorSpawn : MonoBehaviour
    {
        private byte frame = 0;

        public void Update()
        {

            if (frame > 0) // On frame 1
            {
                Identifiable ident = GetComponent<Identifiable>();
                var packet = new ActorSpawnClientMessage()
                {
                    ident = ident.id,
                    position = transform.position,
                    rotation = transform.eulerAngles,
                    velocity = GetComponent<Rigidbody>().velocity,
                    player = SRNetworkManager.playerID
                };
                NetworkClient.SRMPSend(packet);
                Destroyer.DestroyActor(gameObject, "SRMP.CancelActor");
            }
            frame++;
        }
    }
}
