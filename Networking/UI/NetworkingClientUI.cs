using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using UnityEngine;
using UnityEngine.UI;

namespace SRMP.Networking.UI
{
    public class NetworkingClientUI : SRBehaviour
    {
        public NetworkManager manager;

        public GameObject ui;

        public int offsetX;
        public int offsetY;

        void OnDisable()
        {
            ui.SetActive(false);
        }
        void OnEnable()
        {
            ui.SetActive(true);
        }

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
            ui = transform.GetChild(0).Find("ClientIngame").gameObject;
            ui.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
                NetworkServer.Shutdown();
                NetworkClient.Shutdown();
                MultiplayerManager.ClientLeave();
            });
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
