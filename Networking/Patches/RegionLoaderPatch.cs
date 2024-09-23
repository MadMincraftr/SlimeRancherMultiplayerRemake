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
    [HarmonyPatch(typeof(RegionLoader), nameof(RegionLoader.UpdateProxied))]
    public class RegionLoaderUpdateProxied
    {
        private static void CombineRegionList(List<Region> load, List<Region> unload)
        {
            RegionLoader.loadRegions = CombineRegionListInternal(load, RegionLoader.loadRegions).ToList();
            RegionLoader.unloadRegions = CombineRegionListInternal(unload, RegionLoader.unloadRegions).ToList();
        }

        static IEnumerable<Region> CombineRegionListInternal(List<Region> a, List<Region> b)
        {

            foreach (var item in a)
            {
                yield return item;
            }
            foreach (var item in b)
            {
                yield return item;
            }
        }

        /// <summary>
        /// CODE IS PROPERTY OF MONOMI PARK
        /// DO NOT COPY ANYWHERE
        /// </summary>
        public static bool Prefix(RegionLoader __instance, Vector3 position)
        {
            RegionLoader.loadRegions.Clear();
            RegionLoader.unloadRegions.Clear();

            foreach (var player in SRNetworkManager.players.Values)
            {
                if (player.id == 0 && NetworkServer.activeHost) continue;

                Vector3 networkPos = player.transform.position;

                List<Region> load = new List<Region>();
                List<Region> unload = new List<Region>();

                Bounds bounds = new Bounds(networkPos, __instance.LoadSize / 4);
                Bounds bounds2 = new Bounds(networkPos, (__instance.LoadSize * (1f + __instance.UnloadBuffer)) / 4);
                
                __instance.regionReg.GetContaining(ref load, bounds);
                __instance.regionReg.GetContaining(ref load, bounds);


                CombineRegionList(load, unload);
            }


            List<Region> load2 = new List<Region>();
            List<Region> unload2 = new List<Region>();

            Bounds bounds3 = new Bounds(position, __instance.LoadSize );
            Bounds bounds4 = new Bounds(position, __instance.LoadSize * (1f + __instance.UnloadBuffer));

            __instance.regionReg.GetContaining(ref load2, bounds3);
            __instance.regionReg.GetContaining(ref unload2, bounds4);

            CombineRegionList(load2, unload2);

            int num = 0;
            int num2 = __instance.nonProxiedRegions.Count;
            while (num < num2)
            {
                Region region = __instance.nonProxiedRegions[num];
                if (RegionLoader.loadRegions.Contains(region))
                {
                    RegionLoader.loadRegions.Remove(region);
                    num++;
                }
                else if (!RegionLoader.unloadRegions.Contains(region))
                {
                    region.RemoveNonProxiedReference();
                    __instance.nonProxiedRegions.RemoveAt(num);
                    num2--;
                }
                else
                {
                    num++;
                }
            }

            num2 = RegionLoader.loadRegions.Count;
            if (num2 <= 0)
            {
                return false;
            }

            for (num = 0; num < num2; num++)
            {
                Region region2 = RegionLoader.loadRegions[num];
                if (!__instance.nonProxiedRegions.Contains(region2))
                {
                    region2.AddNonProxiedReference();
                    __instance.nonProxiedRegions.Add(region2);
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(RegionLoader), nameof(RegionLoader.Update))]
    public class RegionLoaderUpdate
    {
        private static bool CheckPlayerPositions(RegionLoader rl)
        {
            foreach (var player in SRNetworkManager.players.Values)
            {
                var checkVal = (player.transform.position - SRNetworkManager.playerRegionCheckValues[player.id]).sqrMagnitude >= 1f;

                SRNetworkManager.playerRegionCheckValues[player.id] = player.transform.position;

                if (checkVal == true)
                {
                    return true;
                }
            }

            var localCheckVal = (rl.transform.position - rl.lastRegionCheckPos).sqrMagnitude >= 1f;

            if (localCheckVal == true)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// CODE IS PROPERTY OF MONOMI PARK
        /// DO NOT COPY ANYWHERE
        /// </summary>
        public static bool Prefix(RegionLoader __instance)
        {
            if (NetworkServer.active || NetworkClient.active)
            {

                if (CheckPlayerPositions(__instance))
                {
                    __instance.ForceUpdate();


                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(RegionLoader), nameof(RegionLoader.UpdateHibernated))]
    public class RegionLoaderUpdateHibernated
    {
        private static void CombineRegionList(List<Region> load, List<Region> unload)
        {
            RegionLoader.loadRegions = CombineRegionListInternal(load, RegionLoader.loadRegions).ToList();
            RegionLoader.unloadRegions = CombineRegionListInternal(unload, RegionLoader.unloadRegions).ToList();
        }

        static IEnumerable<Region> CombineRegionListInternal(List<Region> a, List<Region> b)
        {

            foreach (var item in a)
            {
                yield return item;
            }
            foreach (var item in b)
            {
                yield return item;
            }
        }

        /// <summary>
        /// CODE IS PROPERTY OF MONOMI PARK
        /// DO NOT COPY ANYWHERE
        /// </summary>
        public static bool Prefix(RegionLoader __instance, Vector3 position)
        {
            RegionLoader.loadRegions.Clear();
            RegionLoader.unloadRegions.Clear();
            foreach (var player in SRNetworkManager.players.Values)
            {
                if (player.id == 0 && NetworkServer.activeHost) continue;

                Vector3 networkPos = player.transform.position;

                List<Region> load = new List<Region>();
                List<Region> unload = new List<Region>();

                Bounds bounds = new Bounds(networkPos, __instance.WakeSize / 5);
                Bounds bounds2 = new Bounds(networkPos, (__instance.WakeSize * (1f + __instance.UnloadBuffer)) / 5);

                __instance.regionReg.GetContaining(ref load, bounds);
                __instance.regionReg.GetContaining(ref unload, bounds2);


                CombineRegionList(load, unload);
            }


            List<Region> load2 = new List<Region>();
            List<Region> unload2 = new List<Region>();

            Bounds bounds3 = new Bounds(position, __instance.WakeSize);
            Bounds bounds4 = new Bounds(position, __instance.WakeSize * (1f + __instance.UnloadBuffer));

            __instance.regionReg.GetContaining(ref load2, bounds3);
            __instance.regionReg.GetContaining(ref unload2, bounds4);

            CombineRegionList(load2, unload2);


            int num = 0;
            int num2 = __instance.nonHibernatedRegions.Count;
            while (num < num2)
            {
                Region region = __instance.nonHibernatedRegions[num];
                if (RegionLoader.loadRegions.Contains(region))
                {
                    RegionLoader.loadRegions.Remove(region);
                    num++;
                }
                else if (!RegionLoader.unloadRegions.Contains(region))
                {
                    region.RemoveNonHibernateReference();
                    __instance.nonHibernatedRegions.RemoveAt(num);
                    num2--;
                }
                else
                {
                    num++;
                }
            }

            num2 = RegionLoader.loadRegions.Count;
            if (num2 <= 0)
            {
                return false;
            }

            for (num = 0; num < num2; num++)
            {
                Region region2 = RegionLoader.loadRegions[num];
                if (!__instance.nonHibernatedRegions.Contains(region2))
                {
                    region2.AddNonHibernateReference();
                    __instance.nonHibernatedRegions.Add(region2);
                }
            }
            return false;
        }
    }
}
