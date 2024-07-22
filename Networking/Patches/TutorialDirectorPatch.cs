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
    [HarmonyPatch(typeof(TutorialDirector), nameof(TutorialDirector.MaybeShowPopup))]
    public class TutorialDirectorShowPopup
    {
        public static bool Prefix(TutorialDirector __instance, TutorialDirector.Id id) => !SRMLConfig.DEBUG_STOP_TUTORIALS;
    }
}
