using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Mirror;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using rail;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using UnityEngine;
using UnityEngine.UIElements;
using static SECTR_AudioSystem;
namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(RegionMember), nameof(RegionMember.UpdateRegionMembership))]
    public class UpdateRegionMembership
    {
        public static bool Prefix(RegionMember __instance)
        {
            if (__instance.setId == RegionRegistry.RegionSetId.UNSET)
            {
                __instance.actorModel.currRegionSetId = RegionRegistry.RegionSetId.HOME;
                return false;
            }
            return true;
        }
    }
}
