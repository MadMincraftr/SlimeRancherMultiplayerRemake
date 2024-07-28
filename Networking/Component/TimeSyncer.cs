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
                yield return new WaitForSecondsRealtime(.1f);
                if (NetworkServer.activeHost)
                {
                    var packet = new TimeSyncMessage
                    {
                        time = dir.worldModel.worldTime
                    };
                    SRNetworkManager.NetworkSend(packet);

                }
            }
        }
    }
}
