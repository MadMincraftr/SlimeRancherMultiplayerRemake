using HarmonyLib;
using Mirror;
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
            if (NetworkClient.active) return;
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
            networkGame.sharedKeys = SRNetworkManager.initialWorldSettings.shareKeys;
            networkGame.sharedUpgrades = SRNetworkManager.initialWorldSettings.shareUpgrades;
            networkGame.sharedMoney
                = SRNetworkManager.initialWorldSettings.shareMoney;

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
            if (NetworkClient.active && !NetworkServer.activeHost) return;
            SRNetworkManager.DoNetworkSave();
        }
    }
}
