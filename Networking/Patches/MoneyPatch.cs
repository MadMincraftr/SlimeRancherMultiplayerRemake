using HarmonyLib;
using Mirror;
using SRMP.Networking;
using SRMP.Networking.Packet;

namespace SRMP.Patches
{
    [HarmonyPatch(typeof(PlayerState), nameof(PlayerState.AddCurrency))]
    internal class MoneyPatchAdd
    {
        public static void Postfix(PlayerState __instance, int adjust, PlayerState.CoinsType coinsType)
        {
            if (NetworkClient.active)
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
    internal class MoneyPatchSpend
    {
        public static void Postfix(PlayerState __instance, int adjust)
        {
            if (NetworkClient.active)
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
