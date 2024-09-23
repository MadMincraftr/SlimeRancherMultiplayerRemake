using HarmonyLib;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.Persist;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using SRMP.Networking.SaveModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using static PlayerState;

namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.LoadSave))]
    public class AutoSaveDirectorLoadSave
    {
        public static void Postfix(AutoSaveDirector __instance, string gameName, string saveName, bool promptDLCPurgedException, Action onError)
        {
            SRNetworkManager.CheckForMPSavePath();
            var path = Path.Combine(((FileStorageProvider)GameContext.Instance.AutoSaveDirector.StorageProvider).SavePath(), "MultiplayerSaves", $"{gameName}.srmp");
            var networkGame = new NetworkV01();
            try
            {
                using (FileStream fs = File.Open(path, FileMode.OpenOrCreate))
                {
                    networkGame.Load(fs);
                }
            }
            catch { }

            SRNetworkManager.savedGame = networkGame;
            SRNetworkManager.savedGamePath = path;
        }
    }
    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.LoadNewGame))]
    public class AutoSaveDirectorLoadNewGame
    {
        public static void Postfix(AutoSaveDirector __instance, string displayName, Identifiable.Id gameIconId, GameMode gameMode, Action onError)
        {
            SRNetworkManager.CheckForMPSavePath();
            var path = Path.Combine(((FileStorageProvider)GameContext.Instance.AutoSaveDirector.StorageProvider).SavePath(), "MultiplayerSaves", $"{displayName}.srmp");
            var networkGame = new NetworkV01();
            try
            {
                using (FileStream fs = File.Create(path))
                {
                    networkGame.Write(fs);
                }
            }
            catch { }

            SRNetworkManager.savedGame = networkGame;
            SRNetworkManager.savedGamePath = path;
        }
    }
    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGame))]
    public class AutoSaveDirectorSaveGame
    {
        public static void Postfix(AutoSaveDirector __instance)
        { 
            foreach (var player in SRNetworkManager.players)
            {
                Guid playerID = SRNetworkManager.clientToGuid[player.Key];
                NetworkAmmo normalAmmo = (NetworkAmmo)SRNetworkManager.ammos[$"player_{playerID}_normal"];
                NetworkAmmo nimbleAmmo = (NetworkAmmo)SRNetworkManager.ammos[$"player_{playerID}_nimble"];
                Dictionary<AmmoMode, List<AmmoDataV02>> ammoData = new Dictionary<AmmoMode, List<AmmoDataV02>>();
                ammoData.Add(AmmoMode.DEFAULT, GameContext.Instance.AutoSaveDirector.SavedGame.AmmoDataFromSlots(normalAmmo.Slots));
                ammoData.Add(AmmoMode.NIMBLE_VALLEY, GameContext.Instance.AutoSaveDirector.SavedGame.AmmoDataFromSlots(nimbleAmmo.Slots));
                SRNetworkManager.savedGame.savedPlayers.playerList[playerID].ammo = ammoData;
                var playerPos = new Vector3V02();
                playerPos.value = player.Value.transform.position;
                var playerRot = new Vector3V02();
                playerRot.value = player.Value.transform.eulerAngles;
                SRNetworkManager.savedGame.savedPlayers.playerList[playerID].position = playerPos;
                SRNetworkManager.savedGame.savedPlayers.playerList[playerID].rotation = playerRot;
            }
            using (FileStream fs = File.Open(SRNetworkManager.savedGamePath, FileMode.Create))
            {
                SRNetworkManager.savedGame.Write(fs);
            }
        }
    }
}
