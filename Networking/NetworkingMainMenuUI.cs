using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking
{
    // Custom version of `NetworkManagerHUD`
    internal class NetworkingMainMenuUI : SRBehaviour
    {
        NetworkManager manager;

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

            StartButtons();

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (!NetworkClient.active)
            {
                // Host
                if (GUILayout.Button("Host"))
                    manager.StartHost();

                // Client + IP (+ PORT)
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Client"))
                    manager.StartClient();

                manager.networkAddress = GUILayout.TextField(manager.networkAddress);
                // only show a port field if we have a port transport
                // we can't have "IP:PORT" in the address field since this only
                // works for IPV4:PORT.
                // for IPV6:PORT it would be misleading since IPV6 contains ":":
                // 2001:0db8:0000:0000:0000:ff00:0042:8329
                if (Transport.active is PortTransport portTransport)
                {
                    // use TryParse in case someone tries to enter non-numeric characters
                    if (ushort.TryParse(GUILayout.TextField(portTransport.Port.ToString()), out ushort port))
                        portTransport.Port = port;
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                // Connecting
                GUILayout.Label($"Connecting to {manager.networkAddress}..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                    manager.StopClient();
            }
        }
    }
}
