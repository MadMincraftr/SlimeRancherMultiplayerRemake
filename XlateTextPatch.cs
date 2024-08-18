using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
namespace SRMP
{
    [HarmonyPatch(typeof(XlateText), "Awake")]
    public class XlateTextAwake
    {
        public static void Prefix(XlateText __instance)
        {
            SRMP.localizedTexts.Add($"[{__instance.bundlePath}]-[{__instance.key}]", __instance);
        }
    }
    [HarmonyPatch(typeof(XlateText), "OnDestroy")]
    public class XlateTextOnDestroy
    {
        public static void Prefix(XlateText __instance)
        {
            SRMP.localizedTexts.Remove($"[{__instance.bundlePath}]-[{__instance.key}]");
        }
    }
}
