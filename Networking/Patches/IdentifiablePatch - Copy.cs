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
using SRMP.Networking;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using UnityEngine;
using static ActorVortexer;
namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(GardenCatcher),nameof(GardenCatcher.Plant))]
    public class GardenCatcherPlant
    {

        public static void Postfix(GardenCatcher __instance, Identifiable.Id cropId, bool isReplacement)
        {
            
        }
    }
}
