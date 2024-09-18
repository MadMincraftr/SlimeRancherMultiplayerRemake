using HarmonyLib;
using MonomiPark.SlimeRancher;
using SRMP.Networking.Packet;
using SRMP.Networking.SaveModels;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.LoadSave))]
    public class AutoSaveDirectorLoadSave
    {
        public static void Postfix(AutoSaveDirector __instance, string gameName, string saveName, bool promptDLCPurgedException, Action onError)
        {
            var path = Path.Combine(((FileStorageProvider)GameContext.Instance.AutoSaveDirector.StorageProvider).SavePath(), "MultiplayerSaves", $"{saveName}.srmp");
            var networkGame = new NetworkV01();
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    networkGame.Load(fs);
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
            var networkGame = new NetworkV01();
            try
            {
                using (FileStream fs = File.Open(SRNetworkManager.savedGamePath, FileMode.Open))
                {
                    networkGame.Write(fs);
                }
            }
            catch { }
        }
    }
}
