using Mirror;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms;

namespace SRMP.Networking
{
    public class NetworkHandler
    {
        public class Server
        {
            internal static void Start()
            {
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, TestLogMessage>(HandleTestLog));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, SetMoneyMessage>(HandleMoneyChange));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, PlayerJoinMessage>(HandlePlayerJoin));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, PlayerUpdateMessage>(HandlePlayer));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, SleepMessage>(HandleClientSleep));

            }
            public static void HandleTestLog(NetworkConnectionToClient nctc, TestLogMessage packet)
            {
                SRMP.Log(packet.MessageToLog);
            }
            public static void HandleMoneyChange(NetworkConnectionToClient nctc, SetMoneyMessage packet)
            {
                SceneContext.Instance.PlayerState.model.currency = packet.newMoney;

                // Notify others
                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {
                        var playerPacket = new PlayerJoinMessage()
                        {
                            id = conn.connectionId,
                            local = false
                        };

                        NetworkServer.SRMPSend(playerPacket, conn);
                    }
                }
            }
            public static void HandlePlayerJoin(NetworkConnectionToClient nctc, PlayerJoinMessage packet)
            {
                // Do nothing, everything is already handled anyways.
            }
            public static void HandleClientSleep(NetworkConnectionToClient nctc, SleepMessage packet)
            {
                SceneContext.Instance.TimeDirector.FastForwardTo(packet.time);
            }
            public static void HandlePlayer(NetworkConnectionToClient nctc, PlayerUpdateMessage packet)
            {
                var player = SRNetworkManager.players[packet.id];

                player.transform.position = packet.pos;
                player.transform.rotation = packet.rot;

                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {
                        var playerPacket = new PlayerJoinMessage()
                        {
                            id = conn.connectionId,
                            local = false
                        };

                        NetworkServer.SRMPSend(playerPacket, conn);
                    }
                }

            }
        }
        public class Client
        {

            internal static void Start(bool host)
            {
                NetworkClient.RegisterHandler(new Action<SetMoneyMessage>(HandleMoneyChange));
                NetworkClient.RegisterHandler(new Action<PlayerJoinMessage>(HandlePlayerJoin));
                NetworkClient.RegisterHandler(new Action<PlayerUpdateMessage>(HandlePlayer));
                NetworkClient.RegisterHandler(new Action<TimeSyncMessage>(HandleTime));
            }
            public static void HandleMoneyChange(SetMoneyMessage packet)
            {
                SceneContext.Instance.PlayerState.model.currency = packet.newMoney;
            }
            public static void HandlePlayerJoin(PlayerJoinMessage packet)
            {
                try
                {
                    if (packet.local)
                    {
                        var localPlayer = SceneContext.Instance.player.AddComponent<NetworkPlayer>();
                        localPlayer.id = packet.id;
                    }
                    else
                    {
                        var player = UnityEngine.Object.Instantiate(MultiplayerManager.Instance.onlinePlayerPrefab);
                        player.name = $"Player{packet.id}";
                        var netPlayer = player.GetComponent<NetworkPlayer>();
                        SRNetworkManager.players.Add(packet.id, netPlayer);
                        netPlayer.id = packet.id;
                        player.SetActive(true);
                    }
                }
                catch { } // Some reason it does happen.
            }
            public static void HandlePlayer(PlayerUpdateMessage packet)
            {
                try
                {
                    var player = SRNetworkManager.players[packet.id];

                    player.transform.position = packet.pos;
                    player.transform.rotation = packet.rot;
                }
                catch (KeyNotFoundException e)
                {
                    var player = UnityEngine.Object.Instantiate(MultiplayerManager.Instance.onlinePlayerPrefab);
                    player.name = $"Player{packet.id}";
                    var netPlayer = player.GetComponent<NetworkPlayer>();
                    SRNetworkManager.players.Add(packet.id, netPlayer);
                    netPlayer.id = packet.id;
                    player.SetActive(true);
                    SRMP.Log(e.ToString());
                }
            }
            public static void HandleTime(TimeSyncMessage packet)
            {
                try
                {
                    SceneContext.Instance.TimeDirector.worldModel.worldTime = packet.time;
                }
                catch { }
            }
        }
    }
}
