using Mirror;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Packet
{
    public struct LoadMessage : NetworkMessage
    {
        public List<InitActorData> initActors;
        public List<InitPlayerData> initPlayers;
        public List<InitPlotData> initPlots;
        public HashSet<InitGordoData> initGordos;
        // public List<InitGadgetData> initGadgets;

        public HashSet<PediaDirector.Id> initPedias;
        public HashSet<ZoneDirector.Zone> initMaps;

        public HashSet<InitAccessData> initAccess;

        public LocalPlayerData localPlayerSave;
        public int playerID;
        public int money;
        public int keys;
        public List<PlayerState.Upgrade> upgrades;
        public double time;

        public bool sharedMoney;
        public bool sharedKeys;
        public bool sharedUpgrades;

        public List<InitPlotData> DebugGetAllGardenPlots()
        {
            List<InitPlotData> _return = new List<InitPlotData>();
            foreach (var plot in initPlots)
            {
                if (plot.type == LandPlot.Id.GARDEN) _return.Add(plot);
            }
            return _return;
        }
    }

    public struct InitActorData
    {
        public long id;
        public Identifiable.Id ident;
        public Vector3 pos;
    }
    public struct InitGordoData
    {
        public string id;
        public int eaten;
    }
    public struct InitGadgetData
    {
        public string id;
        public Gadget.Id gadget;
    }
    public struct InitAccessData
    {
        public string id;
        public bool open;
    }
    public struct InitPlotData
    {
        public string id;
        public LandPlot.Id type;
        public HashSet<LandPlot.Upgrade> upgrades;
        public Identifiable.Id cropIdent;

        public InitSiloData siloData;
    }

    public struct InitSiloData
    {
        public int slots;

        public HashSet<AmmoData> ammo;
    }

    public struct AmmoData
    {
        public Identifiable.Id id;
        public int count;
        public int slot;
    }

    public struct InitPlayerData
    {
        public int id;
    }
    public struct LocalPlayerData
    {
        public Vector3 pos;
        public Vector3 rot;

        public Dictionary<PlayerState.AmmoMode, List<AmmoData>> ammo;
    }
}
