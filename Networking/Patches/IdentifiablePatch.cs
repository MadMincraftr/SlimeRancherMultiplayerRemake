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
    [HarmonyPatch(typeof(Identifiable),nameof(Identifiable.SetModel))]
    public class IdentifiableSetModel
    {
        public static void Prefix(Identifiable __instance) 
        {
            if (MultiplayerManager.Instance.isHosting)
            {
                var t = __instance.gameObject.AddComponent<TransformSmoother>();
                __instance.gameObject.AddComponent<NetworkActorOwnerToggle>();
                t.enabled = false;
                if (__instance.gameObject.GetComponent<NetworkActor>() == null) __instance.gameObject.AddComponent<NetworkActor>();

            }
        }

        public static void Postfix(Identifiable __instance)
        {

            if (NetworkClient.active && !NetworkServer.activeHost && __instance.id != Identifiable.Id.PLAYER)
            {
                
                if (Globals.isLoaded)
                {
                    if (__instance.GetComponent<NetworkActor>() == null)
                    {
                        var packet = new ActorSpawnClientMessage()
                        {
                            ident = __instance.id,
                            position = __instance.transform.position,
                            rotation = __instance.transform.eulerAngles,
                            velocity = __instance.GetComponent<Rigidbody>().velocity,
                            player = SRNetworkManager.playerID
                        };
                        SRNetworkManager.NetworkSend(packet);
                        Destroyer.DestroyActor(__instance.gameObject, "SRMPCancelActor");
                        return;
                    }
                }
            }
            else if (NetworkServer.activeHost)
            {
                if (__instance.id != Identifiable.Id.PLAYER)
                {
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
            if (NetworkServer.activeHost)
            {
                if (__instance.id != Identifiable.Id.PLAYER)
                {
                    var id = __instance.GetActorId();
                    var packet = new ActorDestroyGlobalMessage()
                    {
                        id = id
                    };
                    SRNetworkManager.NetworkSend(packet);

                }
            }
        }
    }
}
