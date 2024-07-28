using Mirror;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using SRMP.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using static ActorVortexer;
using static PlayerState;
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
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, GordoEatMessage>(HandleGordoEat));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, GordoBurstMessage>(HandleGordoBurst));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, PediaMessage>(HandlePedia));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, AmmoAddMessage>(HandleAmmo));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, AmmoEditSlotMessage>(HandleAmmoSlot));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, AmmoRemoveMessage>(HandleAmmoReverse));
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

                        NetworkServer.SRMPSend(packet, conn);
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
                    SRMP.Log($"Actor spawned with velocity {packet.velocity}.");
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
                    obj.GetComponent<NetworkActor>().startingVel = packet.velocity;
                    obj.GetComponent<TransformSmoother>().interpolPeriod = .15f;
                    obj.GetComponent<Vacuumable>().launched = true;
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in spawning actor(no id)! Stack Trace:\n{e}");
                }
                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {

                        NetworkServer.SRMPSend(packet, conn);
                    }
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
                ActorUpdateMessage packetS2C = new ActorUpdateMessage()
                {
                    id = packet.id,
                    position = packet.position,
                    rotation = packet.rotation,
                };

                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {

                        NetworkServer.SRMPSend(packetS2C, conn);
                    }
                }
            }

            public static void HandleActorOwner(NetworkConnectionToClient nctc, ActorUpdateOwnerMessage packet)
            {
                try
                {
                    var actor = SRNetworkManager.actors[packet.id];
                    actor.GetComponent<TransformSmoother>().enabled = true;
                    actor.GetComponent<NetworkActor>().enabled = false;
                    actor.GetComponent<NetworkActor>().isOwned = false;
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in transfering actor({packet.id})! Stack Trace:\n{e}");
                }

                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {

                        NetworkServer.SRMPSend(packet, conn);
                    }
                }
            }
            public static void HandleDestroyActor(NetworkConnectionToClient nctc, ActorDestroyGlobalMessage packet)
            {
                try
                {
                    UnityEngine.Object.Destroy(SRNetworkManager.actors[packet.id].gameObject);
                    SRNetworkManager.actors.Remove(packet.id);
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in handling actor({packet.id})! Stack Trace:\n{e}");
                }
                NetworkServer.SRMPSendToConnections(packet, NetworkServer.NetworkConnectionListExcept(nctc));
            }
            public static void HandlePlayer(NetworkConnectionToClient nctc, PlayerUpdateMessage packet)
            {
                try
                {
                    var player = SRNetworkManager.players[packet.id];

                    player.GetComponent<TransformSmoother>().nextPos = packet.pos;
                    player.GetComponent<TransformSmoother>().nextRot = packet.rot.eulerAngles;
                }
                catch { }

                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {

                        NetworkServer.SRMPSend(packet, conn);
                    }
                }
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
                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {

                        NetworkServer.SRMPSend(packet, conn);
                    }
                }
            }

            public static void HandleGordoEat(NetworkConnectionToClient nctc, GordoEatMessage packet)
            {
                try
                {
                    SceneContext.Instance.GameModel.gordos[packet.id].gordoEatenCount = packet.count;
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in feeding gordo({packet.id})! Stack Trace:\n{e}");
                }
                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {

                        NetworkServer.SRMPSend(packet, conn);
                    }
                }
            }
            public static void HandlePedia(NetworkConnectionToClient nctc, PediaMessage packet)
            {
                SceneContext.Instance.gameObject.AddComponent<HandledDummy>();
                SceneContext.Instance.PediaDirector.MaybeShowPopup(packet.id);
                UnityEngine.Object.Destroy(SceneContext.Instance.gameObject.GetComponent<HandledDummy>());

                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {

                        NetworkServer.SRMPSend(packet, conn);
                    }
                }
            }
            public static void HandleGordoBurst(NetworkConnectionToClient nctc, GordoBurstMessage packet)
            {
                try
                {
                    var gordo = SceneContext.Instance.GameModel.gordos[packet.id].gameObj;
                    gordo.AddComponent<HandledDummy>();
                    gordo.GetComponent<GordoEat>().ImmediateReachedTarget();
                    UnityEngine.Object.Destroy(gordo.GetComponent<HandledDummy>());
                }
                catch (Exception e)
                {
                    if (SRMLConfig.SHOW_SRMP_ERRORS)
                        SRMP.Log($"Exception in feeding gordo({packet.id})! Stack Trace:\n{e}");
                }

                foreach (var conn in NetworkServer.connections.Values)
                {
                    if (conn.connectionId != nctc.connectionId)
                    {

                        NetworkServer.SRMPSend(packet, conn);
                    }
                }
            }
            public static void HandleAmmoSlot(NetworkConnectionToClient nctc, AmmoEditSlotMessage packet)
            {
                try
                {
                    var ammo = SRNetworkManager.ammos[packet.id];
                    if (ammo.Slots[packet.slot] != null)
                    {
                        ammo.Slots[packet.slot].count += packet.count;
                    }
                    else
                    {
                        ammo.Slots[packet.slot] = new Ammo.Slot(packet.ident, packet.count);
                    }
                }
                catch { }
            }

            public static void HandleAmmo(NetworkConnectionToClient nctc, AmmoAddMessage packet)
            {
                try
                {
                    var ammo = SRNetworkManager.ammos[packet.id];
                    int slot = -1;
                    for (int i = 0; i < ammo.ammoModel.usableSlots; i++)
                    {
                        if (ammo.Slots[i].count + 1 <= ammo.ammoModel.GetSlotMaxCount(packet.ident, i))
                        {
                            slot = i;
                            continue;
                        }
                    }
                    if (ammo.Slots[slot] != null)
                    {
                        ammo.Slots[slot].count++;
                    }
                    else
                    {
                        ammo.Slots[slot] = new Ammo.Slot(packet.ident, 1);
                    }
                }
                catch { }
            }
            public static void HandleAmmoReverse(NetworkConnectionToClient nctc, AmmoRemoveMessage packet)
            {
                try
                {
                    Ammo ammo = NetworkAmmo.all[packet.id];
                    if (ammo.Slots[packet.index] != null)
                    {
                        if (ammo.Slots[packet.index].count <= packet.count)
                        {
                            ammo.Slots[packet.index] = null;
                        }
                        else
                            ammo.Slots[packet.index].count -= packet.count;
                    }
                }
                catch { }
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
                NetworkClient.RegisterHandler(new Action<GordoBurstMessage>(HandleGordoBurst));
                NetworkClient.RegisterHandler(new Action<GordoEatMessage>(HandleGordoEat));
                NetworkClient.RegisterHandler(new Action<PediaMessage>(HandlePedia));
                NetworkClient.RegisterHandler(new Action<LoadMessage>(HandleSave));
                NetworkClient.RegisterHandler(new Action<LoadMessage>(HandleSave));
                NetworkClient.RegisterHandler(new Action<AmmoAddMessage>(HandleAmmo));
                NetworkClient.RegisterHandler(new Action<AmmoEditSlotMessage>(HandleAmmoSlot));
                NetworkClient.RegisterHandler(new Action<AmmoRemoveMessage>(HandleAmmoReverse));
            }
            public static void HandleMoneyChange(SetMoneyMessage packet)
            {
                SceneContext.Instance.PlayerState.model.currency = packet.newMoney;
            }
            public static void HandleSave(LoadMessage save)
            {
                SRNetworkManager.latestSaveJoined = save;
                SceneManager.LoadScene("worldGenerated");
            }
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
                    UnityEngine.Object.DontDestroyOnLoad(player);
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
            }
            catch (Exception e)
            {
                if (SRMLConfig.SHOW_SRMP_ERRORS)
                    SRMP.Log($"Exception handling player({packet.id})! Stack trace: \n{e}");
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
                SRNetworkManager.actors.Remove(packet.id);
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
                obj.GetComponent<NetworkActor>().isOwned = false;
                obj.GetComponent<TransformSmoother>().interpolPeriod = .15f;
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

                    foreach (var silo in plot.GetComponentsInChildren<SiloStorage>())
                    {
                        silo.InitAmmo();
                    }
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
        public static void HandleGordoEat(GordoEatMessage packet)
        {
            try
            {
                SceneContext.Instance.GameModel.gordos[packet.id].gordoEatenCount = packet.count;
            }
            catch (Exception e)
            {
                if (SRMLConfig.SHOW_SRMP_ERRORS)
                    SRMP.Log($"Exception in feeding gordo({packet.id})! Stack Trace:\n{e}");
            }
        }
        public static void HandlePedia(PediaMessage packet)
        {
            SceneContext.Instance.gameObject.AddComponent<HandledDummy>();
            SceneContext.Instance.PediaDirector.MaybeShowPopup(packet.id);
            UnityEngine.Object.Destroy(SceneContext.Instance.gameObject.GetComponent<HandledDummy>());

        }
        public static void HandleGordoBurst(GordoBurstMessage packet)
        {
            try
            {
                var gordo = SceneContext.Instance.GameModel.gordos[packet.id].gameObj;
                gordo.AddComponent<HandledDummy>();
                gordo.GetComponent<GordoEat>().ImmediateReachedTarget();
                UnityEngine.Object.Destroy(gordo.GetComponent<HandledDummy>());
            }
            catch (Exception e)
            {
                if (SRMLConfig.SHOW_SRMP_ERRORS)
                    SRMP.Log($"Exception in feeding gordo({packet.id})! Stack Trace:\n{e}");
            }
        }
        public static void HandleAmmoSlot(AmmoEditSlotMessage packet)
        {
            try
            {
                Ammo ammo = NetworkAmmo.all[packet.id];
                if (ammo.Slots[packet.slot] != null)
                {
                    ammo.Slots[packet.slot].count += packet.count;
                }
                else
                {
                    ammo.Slots[packet.slot] = new Ammo.Slot(packet.ident, packet.count);
                }
            }
            catch
            {
                if (SRMLConfig.SHOW_SRMP_ERRORS)
                    SRMP.Log($"Error adding to silo slot({packet.id}_{packet.slot})\n{StackTraceUtility.ExtractStackTrace()}");
            }
        }
        public static void HandleAmmo(AmmoAddMessage packet)
        {
            try
            {
                Ammo ammo = NetworkAmmo.all[packet.id];
                int slot = -1;
                for (int i = 0; i < ammo.ammoModel.usableSlots; i++)
                {
                    if (ammo.Slots[i].count + 1 <= ammo.ammoModel.GetSlotMaxCount(packet.ident, i))
                    {
                        slot = i;
                        continue;
                    }
                }
                if (ammo.Slots[slot] != null)
                {
                    ammo.Slots[slot].count++;
                }
                else
                {
                    ammo.Slots[slot] = new Ammo.Slot(packet.ident, 1);
                }
            }
            catch
            {
                if (SRMLConfig.SHOW_SRMP_ERRORS)
                    SRMP.Log($"Error adding to silo slot({packet.id})\n{StackTraceUtility.ExtractStackTrace()}");
            }
        }
        public static void HandleAmmoReverse(AmmoRemoveMessage packet)
        {
            SRMP.Log("recieve");

            try
            {
                Ammo ammo = NetworkAmmo.all[packet.id];
                if (ammo.Slots[packet.index] != null)
                {
                    if (ammo.Slots[packet.index].count <= packet.count)
                    {
                        ammo.Slots[packet.index] = null;
                    }
                    else
                        ammo.Slots[packet.index].count -= packet.count;
                }
            }
            catch
            {
                if (SRMLConfig.SHOW_SRMP_ERRORS)
                    SRMP.Log($"Error taking from silo slot({packet.id}_{packet.index})\n{StackTraceUtility.ExtractStackTrace()}");
            }
        }
    }
}
