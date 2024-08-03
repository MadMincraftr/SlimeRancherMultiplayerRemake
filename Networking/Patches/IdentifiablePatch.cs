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
    [HarmonyPatch(typeof(Identifiable),nameof(Identifiable.SetModel))]
    public class IdentifiableSetModel
    {

        public static void Postfix(Identifiable __instance)
        {

            if (NetworkClient.active && !NetworkServer.activeHost && __instance.id != Identifiable.Id.PLAYER && __instance.GetComponent<NetworkActor>() == null)
            {
                
                if (Globals.isLoaded)
                {
                    if (__instance.GetComponent<NetworkActor>() == null)
                    {
                        try
                        {

                            __instance.transform.GetChild(0).gameObject.SetActive(false);
                            __instance.GetComponent<Collider>().isTrigger = true;
                            __instance.gameObject.AddComponent<NetworkActorSpawn>();
                            return;
                        }
                        catch { }
                    }
                }
            }
            else if (NetworkServer.activeHost)
            {
                if (__instance.id != Identifiable.Id.PLAYER)
                {
                    var actor = __instance.gameObject;
                    actor.AddComponent<NetworkActor>();
                    actor.AddComponent<NetworkActorOwnerToggle>();
                    actor.AddComponent<TransformSmoother>();
                    var ts = actor.GetComponent<TransformSmoother>();
                    SRNetworkManager.actors.Add(__instance.GetActorId(), actor.GetComponent<NetworkActor>());

                    ts.interpolPeriod = 0.15f;
                    ts.enabled = false;
                    var id = __instance.GetActorId();
                    var packet = new ActorSpawnMessage()
                    {
                        id = id,
                        ident = __instance.id,
                        position = __instance.transform.position,
                        rotation = __instance.transform.eulerAngles

                    };
                    SRNetworkManager.NetworkSend(packet);

                }
            }
        }
    }


    [HarmonyPatch(typeof(Identifiable),nameof(Identifiable.OnDestroy))]
    public class IdentifiableDestroy
    {
        public static void Postfix(Identifiable __instance)
        {
            if (NetworkServer.active || NetworkClient.active)
            {
                if (__instance.id != Identifiable.Id.PLAYER)
                {
                    var id = __instance.GetActorId();
                    var packet = new ActorDestroyGlobalMessage()
                    {
                        id = id
                    };
                    SRNetworkManager.NetworkSend(packet);

                    SRNetworkManager.actors.Remove(id);

                }
            }
        }
    }
}
