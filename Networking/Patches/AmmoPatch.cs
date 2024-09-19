using HarmonyLib;
using Mirror;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using static PlayerState;

namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(Ammo), nameof(Ammo.MaybeAddToSpecificSlot), typeof(Identifiable.Id), typeof(Identifiable), typeof(int), typeof(int), typeof(bool))]
    public class AmmoMaybeAddToSpecificSlot
    {
        public static void Postfix(Ammo __instance, ref bool __result, Identifiable.Id id, Identifiable identifiable, int slotIdx, int count, bool overflow)
        {
            if (!(NetworkClient.active || NetworkServer.active))
                return;
            
            if (__result)
            {
                if (__instance is NetworkAmmo netAmmo)
                {
                    var packet = new AmmoEditSlotMessage()
                    {
                        ident = id,
                        slot = slotIdx,
                        count = count,
                        id = netAmmo.ammoId
                    };
                    SRNetworkManager.NetworkSend(packet);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Ammo), nameof(Ammo.MaybeAddToSlot), typeof(Identifiable.Id), typeof(Identifiable))]
    public class AmmoMaybeAddToSlot
    {
        private static int GetSlotIDX(Ammo ammo,Identifiable.Id id)
        {

            for (int j = 0; j < ammo.ammoModel.usableSlots; j++)
            {
                if ((ammo.slotPreds[j] == null || ammo.slotPreds[j](id)) && ammo.Slots[j] == null && ammo.potentialAmmo.Contains(id))
                {
                    return j;
                }
            }
            return -1;
        }

        public static void Prefix(Ammo __instance, ref bool __result, Identifiable.Id id, Identifiable identifiable)
        {
            if (!(NetworkClient.active || NetworkServer.active))
                return;

            if (__result)
            {
                if (__instance is NetworkAmmo netAmmo)
                {
                    // Now uses edit slot message; add message is now unused
                    var packet = new AmmoEditSlotMessage()
                    {
                        ident = id,
                        id = netAmmo.ammoId,
                        count = 1,
                        slot = GetSlotIDX(__instance,id)
                    };
                    SRNetworkManager.NetworkSend(packet);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Ammo), nameof(Ammo.Decrement), typeof(int), typeof(int))]
    public class AmmoDecrement
    {
        public static void Postfix(Ammo __instance, int index, int count)
        {
            if (!(NetworkClient.active || NetworkServer.active))
                return;
            if (__instance is NetworkAmmo netAmmo)
            {
                var packet = new AmmoRemoveMessage()
                {
                    index = index,
                    count = count,
                    id = netAmmo.ammoId
                };
                SRNetworkManager.NetworkSend(packet);
            }
        }
    }

    [HarmonyPatch(typeof(Ammo), nameof(Ammo.DecrementSelectedAmmo), typeof(int))]
    public class AmmoDecrementSelectedAmmo
    {
        public static void Postfix(Ammo __instance, int amount)
        {
            if (!(NetworkClient.active || NetworkServer.active))
                return;

            if (__instance is NetworkAmmo netAmmo)
            {
                var packet = new AmmoRemoveMessage()
                {
                    index = netAmmo.selectedAmmoIdx,
                    count = amount,
                    id = netAmmo.ammoId
                };
                SRNetworkManager.NetworkSend(packet);
            }
        }
    }
}
