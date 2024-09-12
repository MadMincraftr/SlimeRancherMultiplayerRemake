using HarmonyLib;
using Mirror;
using SRMP.Networking;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;

namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(PediaDirector), nameof(PediaDirector.MaybeShowPopup), typeof(PediaDirector.Id))]
    internal class PediaDirectorMaybeShowPopup
    {
        public static void Postfix(PediaDirector __instance, PediaDirector.Id id)
        {
            if ((NetworkClient.active || NetworkServer.active) && !__instance.IsHandling())
            {
                PediaMessage message = new PediaMessage()
                {
                    id = id
                };
                SRNetworkManager.NetworkSend(message);
            }
        }
    }
}
