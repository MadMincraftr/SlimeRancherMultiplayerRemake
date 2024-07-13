using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking
{
    // Custom version of `NetworkManagerHUD`
    public class NetworkingClientUI : SRBehaviour
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
            // If this width is changed, also change offsetX in GUIConsole::OnGUI
            int width = 300;

            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, width, 9999));

            StopButtons();

            GUILayout.EndArea();
        }

        void StopButtons()
        {
            if (GUILayout.Button("Stop Client"))
                manager.StopClient();
            GUILayout.BeginHorizontal();

            TestLogStuff();

            GUILayout.EndHorizontal();
        }

        private string testMSG = "Type here";
        void TestLogStuff()
        {
            testMSG = GUILayout.TextField(testMSG);
            if (GUILayout.Button("Send Test Log"))
            {
                SRMP.Log("Sending");
                var packet = new TestLogMessage() { MessageToLog = testMSG };
                NetworkClient.Send(packet, 1);
            }
        }
    }
}
