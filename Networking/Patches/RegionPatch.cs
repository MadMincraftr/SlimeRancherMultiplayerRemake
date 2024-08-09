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
namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(Region), nameof(Region.Unproxy))]
    public class RegionUnproxy
    {
        public static void Postfix(Region __instance)
        {
            try
            {
                if (NetworkClient.active && !NetworkServer.activeHost)
                {

                }
            }
            catch { }
        }
    }
}
