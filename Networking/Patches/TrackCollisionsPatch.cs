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
    [HarmonyPatch(typeof(TrackCollisions),nameof(TrackCollisions.OnTriggerEnter))]
    public class TrackCollisionsOnTriggerEnter
    {
        public static void Prefix(TrackCollisions __instance, Collider other) 
        {
            if (NetworkServer.active || NetworkClient.active)
            {
                if (__instance.gameObject.name == "vac shape")
                {
                    if (__instance.transform.GetChild(0).gameObject.activeInHierarchy)
                    {
                        var actor = other.gameObject.GetComponent<NetworkActorOwnerToggle>();
                        if (actor != null)
                        {
                            actor.OwnActor();
                        }
                    }
                }
            }
        }
    }
}
