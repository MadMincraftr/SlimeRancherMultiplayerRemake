using HarmonyLib;
using Mirror;
using SRMP.Networking;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;

namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(SceneContext), nameof(SceneContext.Start))]
    internal class SceneContextStart
    {
        public static void Postfix(SceneContext __instance)
        {
            if (!NetworkServer.active && NetworkClient.active)
            {
                MainSRML.OnClientSaveLoaded(__instance);
            }
        }
    }
}
