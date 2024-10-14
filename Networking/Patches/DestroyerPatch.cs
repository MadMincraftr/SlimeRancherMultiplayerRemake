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
    [HarmonyPatch(typeof(Destroyer), nameof(Destroyer.Destroy),typeof(UnityEngine.Object), typeof(string))]
    public class DestroyerDestroy
    {
        public static void Postfix(UnityEngine.Object instance, string source)
        {
            if (SRMLConfig.DEBUG_LOG)
            {

                if (source.Contains("GifRecorder")) return;
                if (source.Contains("ObjectPool")) return;
                if (NetworkServer.active || NetworkClient.active)
                {
                    if (instance is GameObject)
                    {
                        GameObject obj = (GameObject)instance;
                        SRMP.Log($"[Destroyer] Destroyed a object with the source being \"{source}\", more details: Object name: [{instance.name}]");
                    }
                    else
                    {
                        SRMP.Log($"[Destroyer] Destroyed a [{instance.GetType().FullName}] with the source being \"{source}\"");
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(Destroyer), nameof(Destroyer.DestroyActor), typeof(GameObject), typeof(string), typeof(bool))]
    public class DestroyerDestroyActor
    {
        public static void Postfix(GameObject actorObj, string source, bool okIfNonActor)
        {
            if (NetworkServer.active || NetworkClient.active)
            {


                if (SRMLConfig.DEBUG_LOG)
                {
                    SRMP.Log($"[Destroyer] Destroyed a actor with the source being \"{source}\", more details: Object name: [{actorObj.name}] Is Resource: [{actorObj.GetComponent<ResourceCycle>() != null}]");
                }
            }
        }
        public static bool Prefix(GameObject actorObj, string source, bool okIfNonActor)
        {
            if (NetworkServer.active || NetworkClient.active)
            {
                if (source.Equals("ResourceCycle.RegistryUpdate#1"))
                {

                    if (SRMLConfig.DEBUG_LOG)
                    {
                        SRMP.Log("[Destroyer] canceled a destroy from a resource missing a joint (most likely on a client.)");
                    }
                    return false;
                }
            }
            return true;
        }
    }
}
