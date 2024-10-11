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
    [HarmonyPatch(typeof(ActorModel), nameof(ActorModel.GetPos))]
    public class ActorModelGetPos
    {
        public static bool Prefix(ActorModel __instance, ref Vector3 __result)
        {
            try
            {
                __result = __instance.transform.position;
                return false;
            }
            catch
            {
                if (SRMLConfig.SHOW_SRMP_ERRORS)
                {
                    SRMP.Log($"Error when getting actor position (probably during saving!)\n{StackTraceUtility.ExtractStackTrace()}");
                }
            }
            __result = Vector3.zero;
            return false;
        }

    }
    [HarmonyPatch(typeof(ActorModel), nameof(ActorModel.GetRot))]
    public class ActorModelGetRot
    {
        public static bool Prefix(ActorModel __instance, ref Quaternion __result)
        {
            try
            {
                __result = __instance.transform.rotation;
            }
            catch
            {
                if (SRMLConfig.SHOW_SRMP_ERRORS)
                {
                    SRMP.Log($"Error when getting actor position (probably during saving!)\n{StackTraceUtility.ExtractStackTrace()}");
                }
            }
            return false;
        }

    }
}
