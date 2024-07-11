using Newtonsoft.Json;
using SRML;
using SRMP.DebugConsole;
using SRMP.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
            if (m_GameObject != null) return;

            SRMP.Log("Loading SRMP SRML Version");

            //create the mods main game objects and start connecting everything
            string[] args = System.Environment.GetCommandLineArgs();

            m_GameObject = new GameObject("SRMP");
            m_GameObject.AddComponent<SRNetworkManager>();
            if(args.Contains("-console"))
            {
                m_GameObject.AddComponent<SRMPConsole>();
            }

            m_GameObject.AddComponent<SRNetworkManager>();
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
