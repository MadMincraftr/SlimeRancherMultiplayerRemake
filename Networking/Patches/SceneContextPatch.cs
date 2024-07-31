using HarmonyLib;
using Mirror;
using SRMP.Networking;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;

namespace SRMP.Patches
{
    [HarmonyPatch(typeof(SceneContext), nameof(SceneContext.Awake))]
    internal class SceneContextAwake
    {
        public static void Postfix(SceneContext __instance)
        {
            if (NetworkServer.active || NetworkClient.active)
            {
            }
        }
    }
}
