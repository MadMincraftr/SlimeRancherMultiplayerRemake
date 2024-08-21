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
    [HarmonyPatch(typeof(Vacuumable), nameof(Vacuumable.SetCaptive))]
    public class VacuumableSetCaptive
    {
        public static void Prefix(Vacuumable __instance, Joint toJoint)
        {
            if (NetworkServer.active || NetworkClient.active)
            {
                var actor = __instance.gameObject.GetComponent<NetworkActorOwnerToggle>();
                if (actor != null)
                {
                    actor.OwnActor();
                }
            }
        }
    }
    [HarmonyPatch(typeof(Vacuumable), nameof(Vacuumable.SetHeld))]
    public class VacuumableSetHeld
    {
        public static void Prefix(Vacuumable __instance, bool held)
        {
            if (__instance.GetComponent<HandledDummy>() != null) return;

            if (NetworkServer.active || NetworkClient.active)
            {
                var actor = __instance.gameObject.GetComponent<NetworkActorOwnerToggle>();
                if (actor != null)
                {
                    actor.OwnActor();
                }
            }
        }
    }
}
