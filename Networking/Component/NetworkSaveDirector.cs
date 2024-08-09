/*
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Component
{
    public class NetworkSaveDirector : MonoBehaviour
    {
        public AutoSaveDirector saveDirector;
        void Awake()
        {
            saveDirector = GetComponent<AutoSaveDirector>();

            SaveRegistry.RegisterWorldDataLoadDelegate()
        }

        public static void WriteData(CompoundDataPiece data)
        {
            for ()
            data.SetValue<Vector3>("pos", )
        }
        public class NetworkPlayerData
        {
            public Guid playerGUID;
            public int health;
            public int energy;
            public Vector3 position;
            public Quaternion rotation;
        }

    }
}
*/