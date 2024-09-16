using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Component
{
    [DisallowMultipleComponent]
    public class TimeSyncer : MonoBehaviour
    {
        TimeDirector dir;
        void Start()
        {
            dir = GetComponent<TimeDirector>();
        }

        public float timer = 0;

        void Update()
        {
            timer += Time.deltaTime;

            if (timer > .08)
            {
                var msg = new TimeSyncMessage()
                {
                    time = dir.worldModel.worldTime
                };
                SRNetworkManager.NetworkSend(msg);
            }
        }
    }
}
