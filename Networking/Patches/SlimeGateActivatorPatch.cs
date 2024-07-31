using HarmonyLib;
using SRMP.Networking.Packet;

namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(SlimeGateActivator), nameof(SlimeGateActivator.Activate))]
    internal class SlimeGateActivatorActivate
    {
        public static void Postfix(SlimeGateActivator __instance)
        {
            var message = new DoorOpenMessage()
            {
                id = __instance.gateDoor.id
            };
            SRNetworkManager.NetworkSend(message);
        }
    }
}
