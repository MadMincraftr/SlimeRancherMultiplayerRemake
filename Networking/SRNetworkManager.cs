using Mirror;
using MonomiPark.SlimeRancher.Persist;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using SRMP.Networking.Patches;
using SRMP.Networking.SaveModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerState;
using static SECTR_AudioSystem;

namespace SRMP.Networking
{
    public struct NetGameInitialSettings
    {
        public NetGameInitialSettings(bool defaultValueForAll = true) // Would not use paramater here but this version of c# is ehh...
        {
            shareMoney = defaultValueForAll;
            shareKeys = defaultValueForAll;
            shareUpgrades = defaultValueForAll;
        }

        public bool shareMoney;
        public bool shareKeys;
        public bool shareUpgrades;
    }
    public class SRNetworkManager : NetworkManager
    {
        public static NetGameInitialSettings initialWorldSettings = new NetGameInitialSettings();

        internal static void CheckForMPSavePath()
        {
            if (!Directory.Exists(Path.Combine(((FileStorageProvider)GameContext.Instance.AutoSaveDirector.StorageProvider).SavePath(), "MultiplayerSaves")))
            {
                Directory.CreateDirectory(Path.Combine(((FileStorageProvider)GameContext.Instance.AutoSaveDirector.StorageProvider).SavePath(), "MultiplayerSaves"));
            }
        }

        public static Dictionary<int, Guid> clientToGuid = new Dictionary<int, Guid>();

        public static NetworkV01 savedGame;
        public static string savedGamePath;


        public static Dictionary<int, Vector3> playerRegionCheckValues = new Dictionary<int, Vector3>();

        public static Dictionary<string, Ammo> ammos => NetworkAmmo.all; // this was unused so i decided to make it actually usable.

        public static LoadMessage latestSaveJoined;

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

            foreach (var a in Resources.FindObjectsOfTypeAll<Identifiable>())
            {
                try
                {
                    if (a.gameObject.scene.name == "worldGenerated")
                    {
                        var actor = a.gameObject;
                        actor.AddComponent<NetworkActor>();
                        actor.AddComponent<NetworkActorOwnerToggle>();
                        actor.AddComponent<TransformSmoother>();
                        actor.AddComponent<NetworkResource>();
                        var ts = actor.GetComponent<TransformSmoother>();
                        ts.interpolPeriod = 0.15f;
                        ts.enabled = false;
                        actors.Add(a.GetActorId(), a.GetComponent<NetworkActor>());
                    }
                }
                catch { }
            }
            SceneContext.Instance.gameObject.AddComponent<TimeSyncer>();

            SRMP.ReplaceTranslation("ui", "End Game", "b.save_and_quit");

        }
        public override void OnStopHost()
        {
            NetworkAmmo.all.Clear();
            MultiplayerManager.Instance.isHosting = false;

            SRMP.RevertTranslation("ui", "b.save_and_quit");
        }
        public override void OnStartServer()
        {
            NetworkHandler.Server.Start();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            DoNetworkSave();

            try
            {
                players[conn.connectionId].enabled = true;
                Destroy(players[conn.connectionId].gameObject);
                players.Remove(conn.connectionId);
                clientToGuid.Remove(conn.connectionId);
                ammos.Remove($"player_{playerID}_normal");
                ammos.Remove($"player_{playerID}_nimble");
            }
            catch { }

        }
        public override void OnClientDisconnect()
        {
            SRMP.RevertTranslation("ui", "b.save_and_quit");

            NetworkAmmo.all.Clear();
            try
            {
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
        }

        public override void OnClientConnect()
        {
            var joinMsg = new ClientUserMessage()
            {
                guid = MainSRML.data.Player,
                name = MainSRML.data.Name,
            };
            NetworkClient.SRMPSend(joinMsg);
        }

        /// <summary>
        /// The send function common to both server and client. By default uses 'SRMPSendToAll' for server and 'SRMPSend' for client.
        /// </summary>
        /// <typeparam name="M">Message struct type. Ex: 'PlayerJoinMessage'</typeparam>
        /// <param name="message">The actual message itself. Should automatically set the M type paramater.</param>
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

        internal static (bool, ArraySegment<byte>) SRDataTransport(ArraySegment<byte> buffer)
        {
            using (NetworkReaderPooled reader = NetworkReaderPool.Get(buffer))
            {
                if (reader.ReadBool())
                    return (true, reader.ReadBytesSegment(reader.Remaining));
                else
                    return (false, reader.ReadBytesSegment(reader.Remaining));
            }
        }

        /// <summary>
        /// Erases sync values.
        /// </summary>
        public static void EraseValues()
        {
            foreach (var actor in actors.Values)
            {
                Destroyer.DestroyActor(actor.gameObject, "SRMP.EraseValues");
            }
            actors = new Dictionary<long, NetworkActor>();

            foreach (var player in players.Values)
            {
                Destroy(player.gameObject);
            }
            players = new Dictionary<int, NetworkPlayer>();
            playerRegionCheckValues = new Dictionary<int, Vector3>();

            clientToGuid = new Dictionary<int, Guid>();

            NetworkAmmo.all = new Dictionary<string, Ammo>();

            MapDataEntryStart.entries = new List<MapDataEntry>();

            latestSaveJoined = new LoadMessage();
            savedGame = new NetworkV01();
            savedGamePath = null;
        }


        public static void DoNetworkSave()
        {

            foreach (var player in players)
            {
                Guid playerID = clientToGuid[player.Key];
                NetworkAmmo normalAmmo = (NetworkAmmo)ammos[$"player_{playerID}_normal"];
                NetworkAmmo nimbleAmmo = (NetworkAmmo)ammos[$"player_{playerID}_nimble"];
                Dictionary<AmmoMode, List<AmmoDataV02>> ammoData = new Dictionary<AmmoMode, List<AmmoDataV02>>();
                ammoData.Add(AmmoMode.DEFAULT, GameContext.Instance.AutoSaveDirector.SavedGame.AmmoDataFromSlots(normalAmmo.Slots));
                ammoData.Add(AmmoMode.NIMBLE_VALLEY, GameContext.Instance.AutoSaveDirector.SavedGame.AmmoDataFromSlots(nimbleAmmo.Slots));
                savedGame.savedPlayers.playerList[playerID].ammo = ammoData;
                var playerPos = new Vector3V02();
                playerPos.value = player.Value.transform.position;
                var playerRot = new Vector3V02();
                playerRot.value = player.Value.transform.eulerAngles;
                savedGame.savedPlayers.playerList[playerID].position = playerPos;
                savedGame.savedPlayers.playerList[playerID].rotation = playerRot;
            }
            using (FileStream fs = File.Open(savedGamePath, FileMode.Create))
            {
                savedGame.Write(fs);
            }
        }
    }

    /// <summary>
    /// Server send type for NetworkSend
    /// </summary>
    public enum ServerSendType
    {
        ALL,
        TO_CONNECTION,
        ALL_EXCEPT_CONNECTION,
        TO_MULTIPLE_CONNECTIONS,
    }
}
