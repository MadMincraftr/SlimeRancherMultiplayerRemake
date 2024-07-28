using Mirror.Discovery;
using SRMP.Networking;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRMP.Networking.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkDiscovery))]
    public class CustomDiscoveryUI : MonoBehaviour
    {
        public GameObject ui;
        public GameObject foundPrefab;
        readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
        readonly Dictionary<long, string> serverNames = new Dictionary<long, string>(); // Too lazy to write better code for this dictionary
        Vector2 scrollViewPos = Vector2.zero;

        public NetworkDiscovery networkDiscovery;


        public void OnDiscoveryMade(ServerResponse response)
        {
            if (!discoveredServers.ContainsKey(response.serverId))
            {
                discoveredServers.Add(response.serverId, response); 
                var c = discoveredServers.Count;

                serverNames[response.serverId] = response.ServerName;

                var p = Instantiate(foundPrefab).transform;
                p.parent = ui.transform;
                p.localPosition = foundPrefab.transform.localPosition;
                p.localScale = foundPrefab.transform.localScale;
                p.gameObject.name = $"JoinServer{response.serverId}";

                p.GetChild(0).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

                var b = p.GetComponent<Button>();
                for (int i = 0; i < c; i++) // Damn this is crazy
                {
                    p.localPosition = new Vector3(foundPrefab.transform.localPosition.x, p.localPosition.y - 47.064f, p.localPosition.z);
                }
                p.localPosition = new Vector3(foundPrefab.transform.localPosition.x, p.localPosition.y - .5f, p.localPosition.z); 

                b.onClick.AddListener(() => { Connect(response); });

                p.GetChild(0).GetComponent<TextMeshProUGUI>().text = response.ServerName;

                p.gameObject.SetActive(true);
            }
            if (!serverNames.ContainsKey(response.serverId))
            {
                serverNames.Add(response.serverId, response.ServerName);
            }
        }


        public void Awake()
        {
            ui = transform.GetChild(0).Find("Discovery").gameObject;
            foundPrefab = ui.transform.Find("PCTemplate").gameObject;

            ui.GetChild(0).GetComponent<Button>().onClick.AddListener(Discover);

            networkDiscovery = GetComponent<NetworkDiscovery>();

            networkDiscovery.OnServerFound = new ServerFoundUnityEvent<ServerResponse>();
            networkDiscovery.OnServerFound.AddListener(OnDiscoveryMade);

            
        }

        private void Discover()
        {
            Clear();
            networkDiscovery.StartDiscovery();
        }

        public void FixedUpdate()
        {
        }

        public void Clear()
        {
            var c = ui.transform.childCount;

            for (var i = 2; i < c; i++)
            {
                Destroy(ui.transform.GetChild(i).gameObject);
            }
            discoveredServers.Clear();
            serverNames.Clear();
        }

        void Connect(ServerResponse info)
        {
            MultiplayerManager.Instance.Connect(info.uri.Host, (ushort)info.uri.Port);
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            discoveredServers[info.serverId] = info;
            
        }

        void OnDisable()
        {
            ui.SetActive(false);
        }
        void OnEnable()
        {
            ui.SetActive(true);
        }
    }
}
