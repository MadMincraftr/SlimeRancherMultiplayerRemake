using SRMP.Networking;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Discovery
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkDiscovery))]
    public class CustomDiscoveryUI : MonoBehaviour
    {
        readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
        readonly Dictionary<long, string> serverNames = new Dictionary<long, string>(); // Too lazy to write better code for this dictionary
        Vector2 scrollViewPos = Vector2.zero;

        public NetworkDiscovery networkDiscovery;


        public void OnDiscoveryMade(ServerResponse response)
        {
            if (!discoveredServers.ContainsKey(response.serverId))
            {
                discoveredServers.Add(response.serverId, response);
                serverNames.Add(response.serverId, response.ServerName);
            }
        }


        public void Start()
        {
            networkDiscovery = GetComponent<NetworkDiscovery>();

            networkDiscovery.OnServerFound = new ServerFoundUnityEvent<ServerResponse>();
            networkDiscovery.OnServerFound.AddListener(OnDiscoveryMade);
        }
        void OnGUI()
        {
            if (NetworkManager.singleton == null)
                return;

            if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
                DrawGUI();

            if (NetworkServer.active || NetworkClient.active)
                StopButtons();
        }

        void DrawGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 500));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Find Servers"))
            {
                discoveredServers.Clear();
                networkDiscovery.StartDiscovery();
            }
            GUILayout.EndHorizontal();

            // show list of found server

            GUILayout.Label($"Discovered Servers [{discoveredServers.Count}]:");

            // servers
            scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);

            foreach (ServerResponse info in discoveredServers.Values)
                if (GUILayout.Button($"{serverNames[info.serverId]} ({info.EndPoint.Address.ToString()})"))
                    Connect(info);

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void StopButtons()
        {
            GUILayout.BeginArea(new Rect(10, 40, 100, 25));

            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Host"))
                {
                    NetworkManager.singleton.StopHost();
                    networkDiscovery.StopDiscovery();
                }
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Client"))
                {
                    NetworkManager.singleton.StopClient();
                    networkDiscovery.StopDiscovery();
                }
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                if (GUILayout.Button("Stop Server"))
                {
                    NetworkManager.singleton.StopServer();
                    networkDiscovery.StopDiscovery();
                }
            }

            GUILayout.EndArea();
        }

        void Connect(ServerResponse info)
        {
            MultiplayerManager.Instance.Connect(info.uri.Host, (ushort)info.uri.Port);
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info;
        }
    }
}
