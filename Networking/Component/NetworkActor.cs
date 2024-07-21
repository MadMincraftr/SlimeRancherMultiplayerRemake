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
    public class NetworkActor : MonoBehaviour
    {
        private Identifiable identComp;
        void Start()
        {
            try
            {
                identComp = GetComponent<Identifiable>();
            }
            catch { }
        }

        private float transformTimer = 0;

        internal int startingOwner = 0;

        public long trueID = -1;

        public void Update()
        {
            transformTimer -= Time.deltaTime;
            if (transformTimer <= 0)
            {
                GetComponent<TransformSmoother>().enabled = false;
                transformTimer = .1f;

                if (NetworkClient.active && !NetworkServer.activeHost)
                {
                    var packet = new ActorUpdateClientMessage()
                    {
                        id = identComp.GetActorId(),
                        position = transform.position,
                        rotation = transform.eulerAngles,
                    };
                    SRNetworkManager.NetworkSend(packet);
                }
                else if (NetworkServer.activeHost)
                {
                    var packet = new ActorUpdateMessage()
                    {
                        id = identComp.GetActorId(),
                        position = transform.position,
                        rotation = transform.eulerAngles,
                    };
                    SRNetworkManager.NetworkSend(packet);
                }


            }
        }
        public void OnDisable()
        {
            GetComponent<TransformSmoother>().enabled = true;

        }
    }
}
