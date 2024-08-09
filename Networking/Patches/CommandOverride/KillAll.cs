using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRML.Console.Commands;
using Mirror;
using UnityEngine;

namespace SRMP.Networking.Patches.CommandOverride
{
    [HarmonyPatch(typeof(KillAllCommand), nameof(KillAllCommand.Execute))]
    public class KillAllCommandExecute
    {
        public static bool Prefix(ref bool __result, string[] args)
        {
            if (NetworkClient.active || NetworkServer.active)
            {

                int radius = -1;
                List<Identifiable.Id> list = new List<Identifiable.Id>();
                string[] array = args ?? new string[0];
                foreach (string text in array)
                {
                    if (uint.TryParse(text, out var result))
                    {
                        radius = (int)result;
                        continue;
                    }

                    try
                    {
                        list.Add((Identifiable.Id)Enum.Parse(typeof(Identifiable.Id), text, ignoreCase: true));
                    }
                    catch
                    {
                    }
                }

                List<GameObject> list2 = new List<GameObject>();
                foreach (Identifiable item in from x in Resources.FindObjectsOfTypeAll<Identifiable>()
                                                                where radius == -1 || Vector3.Distance(x.transform?.position ?? SRSingleton<SceneContext>.Instance.PlayerState.model.position, SRSingleton<SceneContext>.Instance.PlayerState.model.position) < (float)radius
                                                                select x)
                {
                    if (list.Count == 0 || list.Contains(item.id))
                    {
                        list2.Add(item.gameObject);
                    }
                }

                int num = 0;
                foreach (GameObject item2 in list2)
                {
                    if (item2)
                    {
                        num++;
                        DeathHandler.Kill(item2, DeathHandler.Source.UNDEFINED, SRSingleton<SceneContext>.Instance.Player, "SRMP.KillAllCommand.Execute");
                    }
                }
                __result = true;
                return false;
            }
            return true;
        }
    }
}
