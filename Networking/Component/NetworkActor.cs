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
    [DisallowMultipleComponent]
    public class NetworkActor : MonoBehaviour
    {
        public bool isOwned = true;

        private Identifiable identComp;
        void Awake()
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

        public Vector3 startingVel = Vector3.zero;

        private bool appliedVel;

        public ResourceCycle resource;

        private ResourceCycle.State prevResState;

        void Start()
        {
            resource = GetComponent<ResourceCycle>();
            if (resource != null &&  NetworkServer.activeHost)
            {
                resource.model.progressTime = double.MaxValue;
                prevResState = resource.model.state;
                var message = new ResourceStateMessage()
                {
                    state = prevResState,
                    id = identComp.GetActorId()
                };
                SRNetworkManager.NetworkSend(message);
            }
            if (startingVel != Vector3.zero)
                GetComponent<Rigidbody>().velocity = startingVel;
            appliedVel = true;
        }
        uint frame;
        bool appliedLaunch;
        bool appliedCollider;
        public void Update()
        {
            try
            {
                if (frame > 8 && !appliedLaunch)
                {
                    GetComponent<Vacuumable>().launched = true;
                    appliedLaunch = true;
                }
                if (frame > 12 && !appliedCollider)
                {
                    GetComponent<Collider>().isTrigger = false;
                    appliedCollider = true;
                }
            }
            catch { }


            if (!isOwned)
            {
                GetComponent<TransformSmoother>().enabled = true;
                enabled = false;
                return;
            }
            transformTimer -= Time.deltaTime;
            if (transformTimer <= 0)
            {

                if (resource != null &&resource.model.state != prevResState && NetworkServer.activeHost)
                {
                    prevResState = resource.model.state;

                    var message = new ResourceStateMessage()
                    {
                        state = prevResState,
                        id = identComp.GetActorId()
                    };
                    SRNetworkManager.NetworkSend(message);
                }

                transformTimer = .15f;

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
            frame++;
        }
        public void OnDisable()
        {
            GetComponent<TransformSmoother>().enabled = true;

        }
    }
}
