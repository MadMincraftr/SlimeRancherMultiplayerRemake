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
    public class NetworkPlayer : MonoBehaviour
    {
        internal int id;
        float transformTimer = 0.1f;
        public Camera cam;
        void Start()
        {
            cam = gameObject.GetComponentInChildren<Camera>();
        }
        public void Update()
        {
            transformTimer -= Time.deltaTime;
            if (transformTimer < 0)
            {
                transformTimer = 0.1f;


                var packet = new PlayerUpdateMessage()
                {
                    id = id,
                    pos = transform.position,
                    rot = transform.rotation
                };
                SRNetworkManager.NetworkSend(packet);
            }
            
        }
    }
}
