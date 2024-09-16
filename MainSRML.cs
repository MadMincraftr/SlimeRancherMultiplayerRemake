using Mirror;
using MonomiPark.SlimeRancher.DataModel;
using Newtonsoft.Json;
using SRML;
using SRML.SR;
using SRMP.Command;
using SRMP.Networking;
using SRMP.Networking.Component;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
            LoadData();
            Application.quitting += SaveData;

            base.PreLoad();
        }

        public static UserData data;

        private void LoadData()
        {
            if (File.Exists(Path.Combine(Application.persistentDataPath, "MultiplayerData.json")))
            {
                try { data = JsonConvert.DeserializeObject<UserData>(File.ReadAllText(Path.Combine(Application.persistentDataPath, "MultiplayerData.json"))); }
                catch { CreateData(); }
            }
            else 
                CreateData();
        }
        private void CreateData()
        {
            var dat = new UserData();

            var rand = new System.Random();

            var i = rand.Next(100000, 999999);

            var guid = Guid.NewGuid();

            dat.Name = $"User{i}";
            dat.Player = guid;
            data = dat;

            SaveData();
        }
        private void SaveData()
        {
            JsonConvert.SerializeObject(data, Formatting.Indented);
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

                    SceneContext.Instance.TimeDirector.worldModel.worldTime = save.time;

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
                            TeleportCommand.playerLookup.Add(TeleportCommand.playerLookup.Count, player.id);
                            netPlayer.id = player.id;
                            playerobj.SetActive(true);
                            UnityEngine.Object.DontDestroyOnLoad(playerobj);
                        }
                        catch { } // Some reason it does happen. // Note found out why, the marker code is completely broken, i forgot that i didnt remove it here so i was wondering why it errored.
                    }
                    foreach (var gordo in save.initGordos)
                    {
                        try
                        {
                            GordoModel gm = SceneContext.Instance.GameModel.gordos[gordo.id];

                            if (gordo.eaten <= -1 || gordo.eaten >= gm.targetCount)
                            {
                                gm.gameObj.SetActive(false);
                            }
                            gm.gordoEatenCount = gordo.eaten;
                        }
                        catch
                        {
                        }
                    }


                    foreach (var plot in save.initPlots)
                    {
                        try
                        {
                            var model = SceneContext.Instance.GameModel.landPlots[plot.id];
                            model.gameObj.BeginHandle();
                            model.gameObj.GetComponent<LandPlotLocation>().Replace(model.gameObj.transform.GetChild(0).GetComponent<LandPlot>(), GameContext.Instance.LookupDirector.plotPrefabDict[plot.type]);
                            model.gameObj.EndHandle();
                            var lp = model.gameObj.transform.GetChild(0).GetComponent<LandPlot>();
                            lp.ApplyUpgrades(plot.upgrades);
                            var silo = model.gameObj.GetComponentInChildren<SiloStorage>();
                            foreach (var ammo in plot.siloData.ammo)
                            {
                                try
                                {
                                    if (!(ammo.count == 0 || ammo.id == Identifiable.Id.NONE))
                                    {
                                        silo.ammo.Slots[ammo.slot] = new Ammo.Slot(ammo.id, ammo.count);
                                    }
                                    else
                                    {
                                        silo.ammo.Slots[ammo.slot] = null;
                                    }
                                }
                                catch { }
                            }

                            GardenCatcher gc = lp.gameObject.GetComponentInChildren<GardenCatcher>(true);
                            if (gc != null)
                            {
                                gc.gameObject.BeginHandle();
                                if (gc.CanAccept(plot.cropIdent))
                                    gc.Plant(plot.cropIdent, true);
                                gc.gameObject.EndHandle();
                            }
                        }
                        catch (Exception e)
                        {
                            SRMP.Log($"Error in world load for plot({plot.id}).\n{e}");
                        }
                    }
                    SceneContext.Instance.PlayerState.model.currency = save.money;
                    SceneContext.Instance.PlayerState.model.keys = save.keys;

                    SceneContext.Instance.PediaDirector.pediaModel.unlocked = save.initPedias;

                    SceneContext.Instance.PlayerState.model.unlockedZoneMaps = save.initMaps;

                    var np = SceneContext.Instance.player.AddComponent<NetworkPlayer>();
                    np.id = save.playerID;

                    foreach (var access in save.initAccess)
                    {
                        GameModel gm = SceneContext.Instance.GameModel;
                        AccessDoorModel adm = gm.doors[access.id];
                        if (access.open)
                        {
                            adm.state = AccessDoor.State.OPEN;
                        }
                        else
                        {
                            adm.state = AccessDoor.State.LOCKED;
                        } // Couldnt figure out the thingy, i tried: access.open ? AccessDoor.State.OPEN : AccessDoor.State.LOCKED
                    }
                }
                else if (NetworkServer.activeHost) // Ignore, impossible to happen.
                {


                    foreach (var a in Resources.FindObjectsOfTypeAll<Identifiable>())
                    {
                        if (a.gameObject.scene.name == "worldGenerated")
                        {
                            if (a.GetComponent<NetworkActor>() == null)
                            {
                                var actor = a.gameObject;
                                actor.AddComponent<NetworkActor>();
                                actor.AddComponent<NetworkActorOwnerToggle>();
                                actor.AddComponent<TransformSmoother>();
                                var ts = actor.GetComponent<TransformSmoother>();
                                ts.interpolPeriod = 0.15f;
                                ts.enabled = false;
                            }
                        }
                    }
                }
            };

            if (m_GameObject != null) return;

            SRMP.Log("Loading SRMP SRML Version");

            //create the mods main game objects and start connecting everything
            string[] args = System.Environment.GetCommandLineArgs();

            m_GameObject = new GameObject("SRMP");
            m_GameObject.AddComponent<MultiplayerManager>();
            
            //mark all mod objects and do not destroy
            GameObject.DontDestroyOnLoad(m_GameObject);

            //get current mod version
            Globals.Version = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            //mark the mod as a background task
            Application.runInBackground = true;

            //initialize connect to the harmony patcher
            HarmonyPatcher.GetInstance().PatchAll(Assembly.GetExecutingAssembly());

            if (args.Contains("--auto-connect"))
            {
                MultiplayerManager.Instance.Connect(SRMLConfig.DEFAULT_CONNECT_IP,SRMLConfig.DEFAULT_PORT);
            }

            SRML.Console.Console.RegisterCommand(new TeleportCommand());
            SRML.Console.Console.RegisterCommand(new PlayerCameraCommand());
        }


        public class UserData
        {
            public string Name;
            public Guid Player;
        }

        // Called after GameContext.Start
        // stuff like gamecontext.lookupdirector are available in this step, generally for when you want to access
        // ingame prefabs and the such
        public override void PostLoad()
        {

        }
    }
}
