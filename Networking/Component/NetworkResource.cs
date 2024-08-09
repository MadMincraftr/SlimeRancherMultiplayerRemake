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
    public class NetworkResource : MonoBehaviour
    {
        void Awake()
        {
            try
            {
                identComp = GetComponent<Identifiable>();
                resource = GetComponent<ResourceCycle>();
            }
            catch { }
        }
        void Start()
        {
            if (resource == null)
            {
                Destroy(this);
            }
        }
        private Identifiable identComp;

        private float updateTimer = 0;

        public ResourceCycle resource;

        public void Update()
        {
            updateTimer -= Time.deltaTime;
            if (updateTimer <= 0)
            {
                if (resource != null)
                {
                    resource.model.progressTime = double.MaxValue;
                    var message = new ResourceStateMessage()
                    {
                        state = resource.model.state,
                        id = identComp.GetActorId()
                    };
                    SRNetworkManager.NetworkSend(message);
                }
                updateTimer = .275f;
            }
        }
    }
}
