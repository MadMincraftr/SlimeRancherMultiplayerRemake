using HarmonyLib;
using Mirror;
using SRMP.Networking;
using SRMP.Networking.Packet;

namespace SRMP.Patches
{
    [HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.Quit))]
    internal class PauseMenuQuit
    {
        public static void Postfix(PauseMenu __instance)
        {
            if (NetworkServer.active || NetworkClient.active)
            {
                NetworkServer.Shutdown();
                NetworkClient.Shutdown();
            }
        }
    }
}
