using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Mirror;
using MonomiPark.SlimeRancher.DataModel;
using rail;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using UnityEngine;
namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(GordoEat), nameof(GordoEat.DoEat))]
    public class GordoDoEat
    {
        public static void Postfix(GordoEat __instance, GameObject obj)
        {
            try
            {
                if ((NetworkServer.active || NetworkClient.active) && __instance.GetComponent<HandledDummy>() == null)
                {
                    var packet = new GordoEatMessage()
                    {
                        id = __instance.id,
                        count = __instance.gordoModel.gordoEatenCount
                    };

                    SRNetworkManager.NetworkSend(packet);
                }
            }
            catch { }
        }

    }
    [HarmonyPatch(typeof(GordoEat), nameof(GordoEat.ImmediateReachedTarget))]
    public class GordoEatImmediateReachedTarget
    {
        public static void Postfix(GordoEat __instance)
        {
            try
            {
                if ((NetworkServer.active || NetworkClient.active) && __instance.GetComponent<HandledDummy>() == null)
                {
                    var packet = new GordoBurstMessage()
                    {
                        id = __instance.id
                    };

                    SRNetworkManager.NetworkSend(packet);
                }
            }
            catch { }
        }

    }
}
