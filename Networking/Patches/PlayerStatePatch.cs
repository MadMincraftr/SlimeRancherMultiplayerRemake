using HarmonyLib;
using Mirror;
using SRMP.Networking;
using SRMP.Networking.Packet;

namespace SRMP.Patches
{
    [HarmonyPatch(typeof(PlayerState), nameof(PlayerState.AddCurrency))]
    internal class PlayerStateAddCurrency
    {
        public static void Postfix(PlayerState __instance, int adjust, PlayerState.CoinsType coinsType)
        {
            if (NetworkClient.active || NetworkServer.active)
            {
                SetMoneyMessage message = new SetMoneyMessage()
                {
                    newMoney = __instance.GetCurrency()
                };
                SRNetworkManager.NetworkSend(message);
            }
        }
    }
    [HarmonyPatch(typeof(PlayerState), nameof(PlayerState.SpendCurrency))]
    internal class PlayerStateSpendCurrency
    {
        public static void Postfix(PlayerState __instance, int adjust)
        {
            if (NetworkClient.active || NetworkServer.active)
            {
                SetMoneyMessage message = new SetMoneyMessage()
                {
                    newMoney = __instance.GetCurrency()
                };
                SRNetworkManager.NetworkSend(message);
            }
        }
    }
}
