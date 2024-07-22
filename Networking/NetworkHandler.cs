using Mirror;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using SRMP.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static SECTR_AudioSystem;

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
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, ActorSpawnClientMessage>(HandleClientActorSpawn));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, ActorUpdateClientMessage>(HandleClientActor));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, ActorDestroyGlobalMessage>(HandleDestroyActor));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, ActorUpdateOwnerMessage>(HandleActorOwner));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, LandPlotMessage>(HandleLandPlot));
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
            public static void HandleClientActorSpawn(NetworkConnectionToClient nctc, ActorSpawnClientMessage packet)
            {
                try
                {
                    Quaternion quat = Quaternion.Euler(packet.rotation.x, packet.rotation.y, packet.rotation.z);
                    var identObj = GameContext.Instance.LookupDirector.identifiablePrefabDict[packet.ident];
                    identObj.AddComponent<NetworkActor>();
                    identObj.AddComponent<NetworkActorOwnerToggle>();
                    identObj.AddComponent<TransformSmoother>();
                    var obj = SRBehaviour.InstantiateActor(identObj, packet.region, packet.position, quat, false);
                    obj.GetComponent<TransformSmoother>().enabled = false;
                    UnityEngine.Object.Destroy(identObj.GetComponent<TransformSmoother>());
                    UnityEngine.Object.Destroy(identObj.GetComponent<NetworkActorOwnerToggle>());
                    UnityEngine.Object.Destroy(identObj.GetComponent<NetworkActor>());
                    SRNetworkManager.actors.Add(obj.GetComponent<Identifiable>().GetActorId(), obj.GetComponent<NetworkActor>());
                    obj.GetComponent<Rigidbody>().velocity = packet.velocity;
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in spawning actor(no id)! Stack Trace:\n{e}");
                }
            }
            public static void HandleClientActor(NetworkConnectionToClient nctc, ActorUpdateClientMessage packet)
            {
                try
                {
                    var actor = SRNetworkManager.actors[packet.id];
                    var t = actor.GetComponent<TransformSmoother>();
                    t.nextPos = packet.position;
                    t.nextRot = packet.rotation;
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in handling actor({packet.id})! Stack Trace:\n{e}");
                }
            }

            public static void HandleActorOwner(NetworkConnectionToClient nctc, ActorUpdateOwnerMessage packet)
            {
                try
                {
                    var actor = SRNetworkManager.actors[packet.id];
                    actor.GetComponent<TransformSmoother>().enabled = true;
                    actor.GetComponent<NetworkActor>().enabled = false;
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in transfering actor({packet.id})! Stack Trace:\n{e}");
                }
            }
            public static void HandleDestroyActor(NetworkConnectionToClient nctc, ActorDestroyGlobalMessage packet)
            {
                try
                {
                    UnityEngine.Object.Destroy(SRNetworkManager.actors[packet.id].gameObject);
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in handling actor({packet.id})! Stack Trace:\n{e}");
                }
            }
            public static void HandlePlayer(NetworkConnectionToClient nctc, PlayerUpdateMessage packet)
            {
                try
                {
                    var player = SRNetworkManager.players[packet.id];

                    player.GetComponent<TransformSmoother>().nextPos = packet.pos;
                    player.GetComponent<TransformSmoother>().nextRot = packet.rot.eulerAngles;

                    var marker = SRNetworkManager.playerToMarkerDict[player];

                    // Stolen from saty :evil:
                    var coefficients = SRSingleton<Map>.Instance.mapUI.mainCoefficients;
                    var minPoint = SRSingleton<Map>.Instance.mapUI.worldMarkerPositionMin;
                    var maxPoint = SRSingleton<Map>.Instance.mapUI.worldMarkerPositionMax;
                    var num = SRSingleton<Map>.Instance.mapUI.mainRotationAdjustment;

                    Vector3 eulerAngles = packet.rot.eulerAngles;
                    marker.Rotate(Quaternion.Euler(eulerAngles.x + num, eulerAngles.y, eulerAngles.z));
                    marker.SetAnchoredPosition(SRSingleton<Map>.Instance.mapUI.GetMapPosClamped(packet.pos, coefficients, minPoint, maxPoint));

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
                catch { }

            }
            public static void HandleLandPlot(NetworkConnectionToClient nctc, LandPlotMessage packet)
            {
                try
                {
                    var plot = SceneContext.Instance.GameModel.landPlots[packet.id].gameObj;

                    if (packet.messageType == LandplotUpdateType.SET)
                    {
                        plot.AddComponent<HandledDummy>();

                        plot.GetComponent<LandPlotLocation>().Replace(plot.transform.GetChild(0).GetComponent<LandPlot>(), GameContext.Instance.LookupDirector.plotPrefabDict[packet.type]);

                        UnityEngine.Object.Destroy(plot.GetComponent<HandledDummy>());
                    }
                    else
                    {

                        var lp = plot.transform.GetChild(0).GetComponent<LandPlot>();
                        lp.gameObject.AddComponent<HandledDummy>();

                        lp.AddUpgrade(packet.upgrade);

                        UnityEngine.Object.Destroy(lp.GetComponent<HandledDummy>());

                    }
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in handling landplot({packet.id})! Stack Trace:\n{e}");
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
                NetworkClient.RegisterHandler(new Action<ActorSpawnMessage>(HandleActorSpawn));
                NetworkClient.RegisterHandler(new Action<ActorUpdateMessage>(HandleActor));
                NetworkClient.RegisterHandler(new Action<ActorDestroyGlobalMessage>(HandleDestroyActor));
                NetworkClient.RegisterHandler(new Action<ActorUpdateOwnerMessage>(HandleActorOwner));
                NetworkClient.RegisterHandler(new Action<LandPlotMessage>(HandleLandPlot));
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
                        SRNetworkManager.playerID = localPlayer.id;
                    }
                    else
                    {
                        var player = UnityEngine.Object.Instantiate(MultiplayerManager.Instance.onlinePlayerPrefab);
                        player.name = $"Player{packet.id}";
                        var netPlayer = player.GetComponent<NetworkPlayer>();
                        SRNetworkManager.players.Add(packet.id, netPlayer);
                        netPlayer.id = packet.id;
                        player.SetActive(true);
                        var marker = UnityEngine.Object.Instantiate(Map.Instance.mapUI.transform.GetComponentInChildren<PlayerMapMarker>().gameObject);
                        SRNetworkManager.playerToMarkerDict.Add(netPlayer, marker.GetComponent<PlayerMapMarker>());
                    }
                }
                catch { } // Some reason it does happen.
            }
            public static void HandlePlayer(PlayerUpdateMessage packet)
            {
                try
                {
                    var player = SRNetworkManager.players[packet.id];

                    player.GetComponent<TransformSmoother>().nextPos = packet.pos;
                    player.GetComponent<TransformSmoother>().nextRot = packet.rot.eulerAngles;

                    var marker = SRNetworkManager.playerToMarkerDict[player];

                    // Stolen from saty :evil:
                    var coefficients = SRSingleton<Map>.Instance.mapUI.mainCoefficients;
                    var minPoint = SRSingleton<Map>.Instance.mapUI.worldMarkerPositionMin;
                    var maxPoint = SRSingleton<Map>.Instance.mapUI.worldMarkerPositionMax;
                    var num = SRSingleton<Map>.Instance.mapUI.mainRotationAdjustment;

                    Vector3 eulerAngles = packet.rot.eulerAngles;
                    marker.Rotate(Quaternion.Euler(eulerAngles.x + num, eulerAngles.y, eulerAngles.z));
                    marker.SetAnchoredPosition(SRSingleton<Map>.Instance.mapUI.GetMapPosClamped(packet.pos, coefficients, minPoint, maxPoint));

                }
                catch (KeyNotFoundException e)
                {
                    try
                    {
                        var player = UnityEngine.Object.Instantiate(MultiplayerManager.Instance.onlinePlayerPrefab);
                        player.name = $"Player{packet.id}";
                        var netPlayer = player.GetComponent<NetworkPlayer>();
                        SRNetworkManager.players.Add(packet.id, netPlayer);
                        netPlayer.id = packet.id;
                        player.SetActive(true);
                        SRMP.Log(e.ToString());

                        var marker = UnityEngine.Object.Instantiate(Map.Instance.mapUI.transform.GetComponentInChildren<PlayerMapMarker>().gameObject);
                        SRNetworkManager.playerToMarkerDict.Add(netPlayer, marker.GetComponent<PlayerMapMarker>());
                    }
                    catch { }
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

            public static void HandleDestroyActor(ActorDestroyGlobalMessage packet)
            {
                try
                {
                    UnityEngine.Object.Destroy(SRNetworkManager.actors[packet.id].gameObject);
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in handling actor({packet.id})! Stack Trace:\n{e}");
                }
            }
            public static void HandleActorSpawn(ActorSpawnMessage packet)
            {
                try
                {
                    Quaternion quat = Quaternion.Euler(packet.rotation.x, packet.rotation.y, packet.rotation.z);
                    var identObj = GameContext.Instance.LookupDirector.identifiablePrefabDict[packet.ident];
                    identObj.AddComponent<NetworkActor>();
                    identObj.gameObject.AddComponent<NetworkActorOwnerToggle>();
                    identObj.AddComponent<TransformSmoother>();
                    var obj = SceneContext.Instance.GameModel.InstantiateActor(packet.id, identObj, packet.region, packet.position, quat, false, false);
                    obj.GetComponent<NetworkActor>().enabled = false;
                    UnityEngine.Object.Destroy(identObj.GetComponent<TransformSmoother>());
                    UnityEngine.Object.Destroy(identObj.GetComponent<NetworkActor>());
                    UnityEngine.Object.Destroy(identObj.GetComponent<NetworkActorOwnerToggle>());
                    SRNetworkManager.actors.Add(packet.id, obj.GetComponent<NetworkActor>());
                    obj.GetComponent<NetworkActor>().trueID = packet.id;
                    if (Identifiable.GetActorId(obj) != packet.id)
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception spawning actor({packet.id})! Stack trace: \n{e}");
                }
            }
            public static void HandleActor(ActorUpdateMessage packet)
            {
                try
                {
                    var actor = SRNetworkManager.actors[packet.id];
                    var t = actor.GetComponent<TransformSmoother>();
                    t.nextPos = packet.position;
                    t.nextRot = packet.rotation;
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in handling actor({packet.id})! Stack Trace:\n{e}");
                }
            }
            public static void HandleActorOwner(ActorUpdateOwnerMessage packet)
            {
                try
                {
                    var actor = SRNetworkManager.actors[packet.id];
                    actor.GetComponent<TransformSmoother>().enabled = true;
                    actor.GetComponent<NetworkActor>().enabled = false;
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in transfering actor({packet.id})! Stack Trace:\n{e}");
                }
            }
            public static void HandleLandPlot(LandPlotMessage packet)
            {
                try
                {
                    var plot = SceneContext.Instance.GameModel.landPlots[packet.id].gameObj;

                    if (packet.messageType == LandplotUpdateType.SET)
                    {
                        plot.AddComponent<HandledDummy>();

                        plot.GetComponent<LandPlotLocation>().Replace(plot.transform.GetChild(0).GetComponent<LandPlot>(), GameContext.Instance.LookupDirector.plotPrefabDict[packet.type]);

                        UnityEngine.Object.Destroy(plot.GetComponent<HandledDummy>());
                    }
                    else
                    {

                        var lp = plot.transform.GetChild(0).GetComponent<LandPlot>();
                        lp.gameObject.AddComponent<HandledDummy>();

                        lp.AddUpgrade(packet.upgrade);

                        UnityEngine.Object.Destroy(lp.GetComponent<HandledDummy>());

                    }
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in handling landplot({packet.id})! Stack Trace:\n{e}");
                }
            }
        }
    }
}
