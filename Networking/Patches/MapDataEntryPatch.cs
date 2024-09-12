using HarmonyLib;
using Mirror;
using SRMP;
using SRMP.Networking;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;




namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(MapDataEntry), nameof(MapDataEntry.Start))]
    public class MapDataEntryStart
    {
        public static List<MapDataEntry> entries = new List<MapDataEntry>();
        public static void Postfix(MapDataEntry __instance)
        {
            entries.Add(__instance);
        }
    }
}
