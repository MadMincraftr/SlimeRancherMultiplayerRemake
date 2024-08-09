using HarmonyLib;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
namespace SRMP.Networking.Patches
{
    [HarmonyPatch(typeof(GardenCatcher),nameof(GardenCatcher.Plant))]
    public class GardenCatcherPlant
    {

        public static void Postfix(GardenCatcher __instance, Identifiable.Id cropId, bool isReplacement)
        {
            if (__instance.GetComponent<HandledDummy>() == null)
            {
                string id = __instance.GetComponentInParent<LandPlotLocation>().id;
                var msg = new GardenPlantMessage()
                {
                    ident = cropId,
                    replace = isReplacement,
                    id = id,
                };
                SRNetworkManager.NetworkSend(msg);
            }
        }
    }
}
