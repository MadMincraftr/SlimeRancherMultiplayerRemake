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
    public class SRNetworkManager : SRSingleton<SRNetworkManager>
    {
        public NetworkManager networkManager;

        internal NetworkingMainMenuUI networkMainMenuHUD;
        
        internal NetworkingClientUI networkConnectedHUD;

        public GameObject onlinePlayerPrefab;

        public void Awake()
        {
            NetworkHandler.Server.Start();
        }

        private void Start()
        {
            onlinePlayerPrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule); // Prototype player.
            onlinePlayerPrefab.AddComponent<NetworkPlayerOnline>();
            onlinePlayerPrefab.DontDestroyOnLoad();
            onlinePlayerPrefab.SetActive(false);

            networkManager = gameObject.AddComponent<NetworkManager>();

            networkManager.maxConnections = SRMLConfig.MAX_PLAYERS;
            networkManager.playerPrefab = onlinePlayerPrefab;

            networkMainMenuHUD = gameObject.AddComponent<NetworkingMainMenuUI>();
            networkConnectedHUD = gameObject.AddComponent<NetworkingClientUI>();
        }
        
        private void Update()
        {
            networkMainMenuHUD.enabled = false;
            networkConnectedHUD.enabled = false;
            if (NetworkServer.active)
            {
                // Show host ui
            }
            else if (NetworkClient.isConnected)
            {
                networkConnectedHUD.enabled = true; // Show connected ui
            }
            else if (NetworkClient.isConnecting)
            {
                // Show connecting ui
            }
            else if (Levels.isMainMenu())
            {
                networkMainMenuHUD.enabled = true; // Show connect ui
            }
            else
            {
                // Show no ui
            }
        }
    }
}
