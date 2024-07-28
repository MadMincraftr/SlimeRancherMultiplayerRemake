using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SRMP.Networking.Component;
using UnityEngine;
namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(SiloStorage), nameof(SiloStorage.InitAmmo))]
    public class SiloStorageInitAmmo
    {
        public static bool Prefix(SiloStorage __instance)
        {
            try
            {
                if (__instance.ammo == null)
                {
                    __instance.ammo = new NetworkAmmo(__instance.transform.GetComponentInParent<LandPlotLocation>().id, __instance.type.GetContents(), __instance.numSlots, __instance.numSlots, new Predicate<Identifiable.Id>[__instance.numSlots], (Identifiable.Id id, int index) => __instance.maxAmmo);
                }
                else if (!(__instance.ammo is NetworkAmmo))
                {
                    __instance.ammo = new NetworkAmmo(__instance.transform.GetComponentInParent<LandPlotLocation>().id, __instance.type.GetContents(), __instance.numSlots, __instance.numSlots, new Predicate<Identifiable.Id>[__instance.numSlots], (Identifiable.Id id, int index) => __instance.maxAmmo);
                }
                return false;
            }
            catch (Exception e)
            {
                if (SRMLConfig.SHOW_SRMP_ERRORS)
                {
                    SRMP.Log($"Error in network ammo!\n{StackTraceUtility.ExtractStackTrace()}");
                }
            }
            return true;
        }
    }
}
