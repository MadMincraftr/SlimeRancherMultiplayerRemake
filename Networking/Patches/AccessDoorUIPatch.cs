using HarmonyLib;
using SRMP.Networking.Packet;

namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(AccessDoorUI), nameof(AccessDoorUI.UnlockDoor))]
    internal class AccessDoorUIUnlockDoor
    {
        public static void Postfix(AccessDoorUI __instance)
        {
            var message = new DoorOpenMessage()
            {
                id = __instance.door.id
            };
            SRNetworkManager.NetworkSend(message);
        }
    }
}
