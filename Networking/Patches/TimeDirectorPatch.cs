using HarmonyLib;
using Mirror;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking.Patches
{
    internal class TimeDirectorPatch
    {
        [HarmonyPatch(typeof(TimeDirector), nameof(TimeDirector.FastForwardTo))]
        internal class SceneContextAwake
        {
            public static bool Prefix (TimeDirector __instance, double fastForwardUntil)
            {
                if (NetworkClient.active && !NetworkServer.activeHost)
                {
                    var packet = new SleepMessage()
                    {
                        time = fastForwardUntil
                    };
                    NetworkClient.SRMPSend(packet);
                    return false;
                }
                return true;
            }
        }
    }
}
