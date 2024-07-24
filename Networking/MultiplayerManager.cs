﻿using kcp2k;
using Mirror;
using Mirror.Discovery;
using MonomiPark.SlimeRancher.Persist;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using SRMP.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRMP.Networking
{
    [DisallowMultipleComponent]
    public class MultiplayerManager : SRBehaviour
    {
        internal static AssetBundle uiBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("SRMP.ui"));


        private NetworkManager networkManager;

        public NetworkingMainMenuUI networkMainMenuHUD;

        public NetworkingClientUI networkConnectedHUD;

        public CustomDiscoveryUI networkDiscoverHUD;

        private NetworkDiscovery discoveryManager;

        public GameObject onlinePlayerPrefab;

        public KcpTransport transport;

        GUIStyle guiStyle;

        public static NetworkManager NetworkManager
        {
            get
            {
                return Instance.networkManager;
            }
        }

        public static NetworkDiscovery DiscoveryManager
        {
            get
            {
                return Instance.discoveryManager;
            }
        }

        public static MultiplayerManager Instance;

        public void Awake()
        {
            Instance = this;
        }


        private void Start()
        {
            var ui = Instantiate(uiBundle.LoadAllAssets<GameObject>()[0]);
            ui.transform.parent = transform;

            foreach (var text in ui.transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                text.alignment = TextAlignmentOptions.Center;
            }

            GUI.backgroundColor = Color.gray;

            GeneratePlayerBean();

            transport = gameObject.AddComponent<KcpTransport>();
            
            WriterBugfix.FixWriters();
            ReaderBugfix.FixReaders();
            
            networkManager = gameObject.AddComponent<SRNetworkManager>();

            networkManager.maxConnections = SRMLConfig.MAX_PLAYERS;
            // networkManager.playerPrefab = onlinePlayerPrefab; need to use asset bundles to fix error
            networkManager.autoCreatePlayer = false;


            networkManager.transport = transport;
            Transport.active = transport;

            networkMainMenuHUD = gameObject.AddComponent<NetworkingMainMenuUI>();
            networkConnectedHUD = gameObject.AddComponent<NetworkingClientUI>();


            discoveryManager = gameObject.AddComponent<NetworkDiscovery>();
            networkDiscoverHUD = gameObject.AddComponent<CustomDiscoveryUI>();

            networkMainMenuHUD.offsetY = Screen.height - 75;
            
            NetworkManager.dontDestroyOnLoad = true;
            discoveryManager.enableActiveDiscovery = true;



            NetworkClient.OnDisconnectedEvent += ClientLeave;
            networkMainMenuHUD.ui.GetChild(4).GetComponent<TMP_InputField>().text = SRMLConfig.DEFAULT_CONNECT_IP;
            networkMainMenuHUD.ui.GetChild(2).GetComponent<TMP_InputField>().text = SRMLConfig.DEFAULT_PORT.ToString();
        }

        void GeneratePlayerBean()
        {
            onlinePlayerPrefab = new GameObject("PlayerDefault");
            var playerModel = GameObject.CreatePrimitive(PrimitiveType.Capsule); // Prototype player.
            var playerFace = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playerFace.transform.parent = playerModel.transform;
            playerFace.transform.localPosition = new Vector3(0f, 0.5f, 0.25f);
            playerFace.transform.localScale = Vector3.one * 0.5f;
            onlinePlayerPrefab.AddComponent<NetworkPlayer>();
            onlinePlayerPrefab.AddComponent<TransformSmoother>();
            onlinePlayerPrefab.GetComponent<NetworkPlayer>().enabled = false;
            onlinePlayerPrefab.DontDestroyOnLoad();
            onlinePlayerPrefab.SetActive(false);
            playerModel.transform.parent = onlinePlayerPrefab.transform;

            var material = Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault((mat) => mat.name == "slimePinkBase");
            playerFace.GetComponent<MeshRenderer>().material = material;
            playerModel.GetComponent<MeshRenderer>().material = material;

            Destroy(playerFace.GetComponent<BoxCollider>());

            playerModel.transform.localPosition = Vector3.up;
        }

        public void OnDestroy()
        {
            SRMP.Log("THIS SHOULD NOT APPEAR!!!!");
        }

        // Hefty code
        public static void PlayerJoin(NetworkConnectionToClient nctc)
        {
            GameContext.Instance.AutoSaveDirector.SaveGame();
            var mem = new MemoryStream();
            GameContext.Instance.AutoSaveDirector.SavedGame.Save(mem);
            var saveMessage = new LoadMessage()
            {
                saveData = mem.ToArray()
            };
            NetworkServer.SRMPSend(saveMessage, nctc);

            try
            {
                var packetNet = new PlayerJoinMessage()
                {
                    id = nctc.connectionId,
                    local = false
                };
                var local = new PlayerJoinMessage()
                {
                    id = nctc.connectionId,
                    local = true
                };

                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {
                        if (SRMLConfig.DEBUG_LOG) SRMP.Log($"Sending join packet for {conn.connectionId} to {nctc.connectionId}.");
                        var playerPacket = new PlayerJoinMessage()
                        {
                            id = conn.connectionId,
                            local = false
                        };

                        NetworkServer.SRMPSend(playerPacket, nctc);
                    }
                }
                if (SRMLConfig.DEBUG_LOG) SRMP.Log($"Broadcasting {nctc.connectionId} join packet.");
                NetworkServer.SRMPSendToConnections(packetNet, NetworkServer.NetworkConnectionListExceptOnly(nctc));
                if (SRMLConfig.DEBUG_LOG) SRMP.Log($"Broadcasting {nctc.connectionId} local join packet.");
                NetworkServer.SRMPSend(local, nctc);


                var player = UnityEngine.Object.Instantiate(Instance.onlinePlayerPrefab);
                player.name = $"Player{packetNet.id}";
                var netPlayer = player.GetComponent<NetworkPlayer>();
                SRNetworkManager.players.Add(packetNet.id, netPlayer);
                netPlayer.id = packetNet.id;
                player.SetActive(true);
                var marker = Instantiate(SRNetworkManager.playerMarkerPrefab);
                SRNetworkManager.playerToMarkerDict.Add(netPlayer, marker.GetComponent<PlayerMapMarker>());
            }
            catch 
            { }
            
        }

        public static void ClientLeave()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void Connect(string ip, ushort port)
        {
            networkManager.OnStartClient();
            transport.port = port;
            NetworkClient.Connect(ip);
        }
        public bool isHosting;
        public void Host(ushort port)
        {
            transport.port = port;
            networkManager.StartHost();
            transport.ServerStart();
            discoveryManager.AdvertiseServer();
            isHosting = true;
        }

        private void Update()
        {
            if (NetworkServer.active)
            {
                networkConnectedHUD.enabled = false;
                networkMainMenuHUD.enabled = false;
                networkDiscoverHUD.enabled = false;
                // Show host ui
            }
            else if (NetworkClient.isConnected)
            {
                networkMainMenuHUD.enabled = false;
                networkDiscoverHUD.enabled = false;
                networkConnectedHUD.enabled = true; // Show connected ui
            }
            else if (NetworkClient.isConnecting)
            {
                networkConnectedHUD.enabled = false;
                networkMainMenuHUD.enabled = false;
                networkDiscoverHUD.enabled = false;
                // Show connecting ui
            }
            else if (Levels.isMainMenu())
            {
                networkConnectedHUD.enabled = false;
                networkMainMenuHUD.enabled = true; // Show connect ui
                networkDiscoverHUD.enabled = true; // Show connect to lan ui
            }
            else
            {
                // Show no ui
                networkConnectedHUD.enabled = false;
                networkMainMenuHUD.enabled = false;
                networkDiscoverHUD.enabled = false;
            }

            // TIcks
            if (NetworkServer.activeHost)
            {
                transport.ServerLateUpdate();
                transport.ServerEarlyUpdate();
            }
            else if (NetworkClient.isConnected || NetworkClient.isConnecting)
            {
                transport.ClientEarlyUpdate();
                transport.ClientLateUpdate();
            }
        }
    }
}
