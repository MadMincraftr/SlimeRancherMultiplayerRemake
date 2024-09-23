using HarmonyLib;
using Mirror;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using static PlayerState;
using static Identifiable.Id;
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
            bool isSlotNull = false;
            bool IsIdentAllowedForAmmo = false;
            bool isSlotEmptyOrSameType = false;
            bool isSlotFull = false;
            for (int j = 0; j < ammo.ammoModel.usableSlots; j++)
            {
                isSlotNull = ammo.Slots[j] == null;

                isSlotEmptyOrSameType = isSlotNull || ammo.Slots[j].id == id;

                IsIdentAllowedForAmmo = ammo.slotPreds[j](id) && ammo.potentialAmmo.Contains(id);

                if (!isSlotNull)
                    isSlotFull = ammo.Slots[j].count >= ammo.ammoModel.slotMaxCountFunction(id, j);
                else
                    isSlotFull = false;

                if (isSlotEmptyOrSameType && isSlotFull) break;

                if (isSlotEmptyOrSameType && IsIdentAllowedForAmmo)
                {
                    return j;
                }
            }
            return -1;
        }

        public static bool Prefix(Ammo __instance, ref bool __result, Identifiable.Id id, Identifiable identifiable)
        {
            if (!(NetworkClient.active || NetworkServer.active))
                return true;

            var slotIDX = GetSlotIDX(__instance, id);
            if (slotIDX == -1) return true;

            __instance.MaybeAddToSpecificSlot(id, null, slotIDX);

            return false;
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
                if (__instance.Slots[index].count <= 0) __instance.Slots[index] = null;

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
                if (__instance.Slots[netAmmo.selectedAmmoIdx].count <= 0) __instance.Slots[netAmmo.selectedAmmoIdx] = null;

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
