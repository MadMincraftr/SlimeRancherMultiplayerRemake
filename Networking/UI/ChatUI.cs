using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.UI
{
    // Custom version of `NetworkManagerHUD`
    public class ChatUI : SRBehaviour
    {
        public NetworkManager manager;

        public int offsetX;
        public int offsetY;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        void OnGUI()
        {

        }

        void StopButtons()
        {
            if (GUILayout.Button("Stop Client"))
                manager.StopClient();
            
        }

        private string testMSG = "Type here";
        void TestLogStuff()
        {
            testMSG = GUILayout.TextField(testMSG);
            if (GUILayout.Button("Send Test Log"))
            {
                SRMP.Log("Sending");
                var packet = new TestLogMessage() { MessageToLog = testMSG };
                NetworkClient.SRMPSend(packet, 1);
            }
        }
    }
}
