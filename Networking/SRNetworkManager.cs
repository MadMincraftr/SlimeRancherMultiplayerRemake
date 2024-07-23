﻿using Mirror;
using SRMP.Networking.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRMP.Networking
{
    public class SRNetworkManager : NetworkManager
    {
        public static int playerID;
        public static Dictionary<int, NetworkPlayer> players = new Dictionary<int, NetworkPlayer>();
        public static Dictionary<NetworkPlayer, PlayerMapMarker> playerToMarkerDict = new Dictionary<NetworkPlayer, PlayerMapMarker>();
        public static GameObject playerMarkerPrefab;
        public static Dictionary<long, NetworkActor> actors = new Dictionary<long, NetworkActor>();
        public static Dictionary<long, long> actorIDLocals = new Dictionary<long, long>();
        public static Dictionary<int, NetworkRegion> regions = new Dictionary<int, NetworkRegion>();
        public override void OnStartClient()
        {
            NetworkHandler.Client.Start(false);
        }
        public override void OnStartHost()
        {
            NetworkHandler.Client.Start(true);
            var localPlayer = SceneContext.Instance.player.AddComponent<NetworkPlayer>();
            localPlayer.id = 0;
        }
        public override void OnStopHost()
        {
            MultiplayerManager.Instance.isHosting = false;
        }
        public override void OnStartServer()
        {
            NetworkHandler.Server.Start();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            try
            {
                Destroy(players[conn.connectionId].gameObject);
                players.Remove(conn.connectionId);
            }
            catch { }
        }
        public override void OnClientDisconnect()
        {
            try
            {
                foreach (var player in players.Values)
                {
                    Destroy(player.gameObject);
                }
                players = new Dictionary<int, NetworkPlayer>();
                SceneManager.LoadScene("MainMenu");
            }
            catch { }
        }
        public override void OnStopClient()
        {
            SceneManager.LoadScene("MainMenu");
        }
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            MultiplayerManager.PlayerJoin(conn);
        }

        public override void OnClientConnect()
        {
        }

        public static void NetworkSend<M>(M message) where M : struct, NetworkMessage
        {
            if (NetworkServer.activeHost)
            {
                NetworkServer.SRMPSendToAll(message);
            }
            else if (NetworkClient.active)
            {
                NetworkClient.SRMPSend(message);
            }
        }

        public static (bool, ArraySegment<byte>) SRDataTransport(ArraySegment<byte> buffer)
        {
            using (NetworkReaderPooled reader = NetworkReaderPool.Get(buffer))
            {
                if (reader.ReadBool())
                    return (true, reader.ReadBytesSegment(reader.Remaining));
                else
                    return (false, reader.ReadBytesSegment(reader.Remaining));
            }
        }

    }
}
