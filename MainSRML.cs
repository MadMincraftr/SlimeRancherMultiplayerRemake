using Mirror;
using Newtonsoft.Json;
using SRML;
using SRML.SR;
using SRMP.DebugConsole;
using SRMP.Networking;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SECTR_AudioSystem;

namespace SRMP
{
    public class MainSRML : ModEntryPoint
    {
        private static GameObject m_GameObject;

        // Called before GameContext.Awake
        // this is where you want to register stuff (like custom enum values or identifiable id's)
        // and patch anything you want to patch with harmony
        public override void PreLoad()
        {
            base.PreLoad();
        }

        // Called right before PostLoad
        // Used to register stuff that needs lookupdirector access
        public override void Load()
        {
            SRCallbacks.OnSaveGameLoaded += (s) =>
            {
                Globals.isLoaded = true;
                if (NetworkClient.active && !NetworkServer.activeHost)
                {
                    LoadMessage save = SRNetworkManager.latestSaveJoined;

                    foreach (var a in Resources.FindObjectsOfTypeAll<Identifiable>())
                    {
                        if (a.gameObject.scene.name == "worldGenerated")
                        {
                            try
                            {

                                if (!Identifiable.SCENE_OBJECTS.Contains(a.id) && a.id != Identifiable.Id.PLAYER)
                                    Destroyer.DestroyActor(a.gameObject, "SRMP.LoadWorld", true);
                            }
                            catch { }
                        }
                    }

                    for (int i = 0; i < save.initActors.Count; i++)
                    {
                        try
                        {
                            InitActorData newActor = save.initActors[i];
                            if (!Identifiable.SCENE_OBJECTS.Contains(newActor.ident) && newActor.ident != Identifiable.Id.PLAYER)
                            {
                                SRMP.Log(newActor.ident.ToString());
                                var obj = GameContext.Instance.LookupDirector.identifiablePrefabDict[newActor.ident];
                                if (obj.GetComponent<NetworkActor>() == null)
                                    obj.AddComponent<NetworkActor>();
                                if (obj.GetComponent<TransformSmoother>() == null)
                                    obj.AddComponent<TransformSmoother>();
                                var obj2 = SceneContext.Instance.GameModel.InstantiateActor(newActor.id, obj, MonomiPark.SlimeRancher.Regions.RegionRegistry.RegionSetId.HOME, Vector3.zero, Quaternion.identity, false, false);
                                UnityEngine.Object.Destroy(obj.GetComponent<NetworkActor>());
                                UnityEngine.Object.Destroy(obj.GetComponent<TransformSmoother>());

                                obj2.transform.position = newActor.pos;

                                SRNetworkManager.actors.Add(newActor.id, obj2.GetComponent<NetworkActor>());
                            }
                        }
                        catch (Exception e)
                        {
                            if (SRMLConfig.SHOW_SRMP_ERRORS) SRMP.Log($"Error in loading world actor(index {i}) {e}");
                        }
                    }
                    foreach (var player in save.initPlayers)
                    {
                        try
                        {
                            var playerobj = UnityEngine.Object.Instantiate(MultiplayerManager.Instance.onlinePlayerPrefab);
                            playerobj.name = $"Player{player.id}";
                            var netPlayer = playerobj.GetComponent<NetworkPlayer>();
                            SRNetworkManager.players.Add(player.id, netPlayer);
                            netPlayer.id = player.id;
                            playerobj.SetActive(true);
                            UnityEngine.Object.DontDestroyOnLoad(playerobj);
                            var marker = UnityEngine.Object.Instantiate(Map.Instance.mapUI.transform.GetComponentInChildren<PlayerMapMarker>().gameObject);
                            SRNetworkManager.playerToMarkerDict.Add(netPlayer, marker.GetComponent<PlayerMapMarker>());
                        }
                        catch { } // Some reason it does happen.
                    }

                    foreach (var plot in save.initPlots)
                    {
                        try
                        {
                            var model = SceneContext.Instance.GameModel.landPlots[plot.id];
                            model.gameObj.AddComponent<HandledDummy>();
                            model.gameObj.GetComponent<LandPlotLocation>().Replace(model.gameObj.transform.GetChild(0).GetComponent<LandPlot>(), GameContext.Instance.LookupDirector.plotPrefabDict[plot.type]);
                            model.gameObj.RemoveComponent<HandledDummy>();
                            model.gameObj.transform.GetChild(0).GetComponent<LandPlot>().ApplyUpgrades(plot.upgrades);

                        }
                        catch { }
                    }

                    SceneContext.Instance.PlayerState.model.currency = save.money;

                    SceneContext.Instance.PediaDirector.pediaModel.unlocked = save.initPedias;

                    var np = SceneContext.Instance.player.AddComponent<NetworkPlayer>();
                    np.id = save.playerID;
                }
                else if (NetworkServer.activeHost) // Ignore, impossible to happen.
                {


                    /*foreach (var a in Resources.FindObjectsOfTypeAll<Identifiable>())
                    {
                        if (a.gameObject.scene.name == "worldGenerated")
                        {
                            var actor = a.gameObject;
                            actor.AddComponent<NetworkActor>();
                            actor.AddComponent<NetworkActorOwnerToggle>();
                            actor.AddComponent<TransformSmoother>();
                            var ts = actor.GetComponent<TransformSmoother>();
                            ts.interpolPeriod = 0.15f;
                            ts.enabled = false;
                        }
                    }*/
                }
            };

            if (m_GameObject != null) return;

            SRMP.Log("Loading SRMP SRML Version");

            //create the mods main game objects and start connecting everything
            string[] args = System.Environment.GetCommandLineArgs();

            m_GameObject = new GameObject("SRMP");
            m_GameObject.AddComponent<MultiplayerManager>();
            if(args.Contains("-console"))
            {
                m_GameObject.AddComponent<SRMPConsole>();
            }
            //mark all mod objects and do not destroy
            GameObject.DontDestroyOnLoad(m_GameObject);

            //get current mod version
            Globals.Version = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            //mark the mod as a background task
            Application.runInBackground = true;

            //initialize connect to the harmony patcher
            HarmonyPatcher.GetInstance().PatchAll(Assembly.GetExecutingAssembly());
        }


        // Called after GameContext.Start
        // stuff like gamecontext.lookupdirector are available in this step, generally for when you want to access
        // ingame prefabs and the such
        public override void PostLoad()
        {

        }
    }
}
