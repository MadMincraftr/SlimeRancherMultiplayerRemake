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
        void Start()
        {
            StartLocalLoop();
        }

        public void StartLocalLoop()
        {
            StartCoroutine(Loop());
        }

        System.Collections.IEnumerator Loop()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(.075f);
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
