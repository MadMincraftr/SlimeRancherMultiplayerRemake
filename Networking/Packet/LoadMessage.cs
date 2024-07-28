using Mirror;
using MonomiPark.SlimeRancher.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
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
        // public List<InitGadgetData> initGadgets;

        public HashSet<PediaDirector.Id> initPedias;

        public int playerID;
        public int money;
    }

    public struct InitActorData
    {
        public long id;
        public Identifiable.Id ident;
        public Vector3 pos;
    }
    public struct InitGadgetData
    {
        public string id;
        public Gadget.Id gadget;
    }
    public struct InitPlotData
    {
        public string id;
        public LandPlot.Id type;
        public HashSet<LandPlot.Upgrade> upgrades;

        public InitSiloData siloData;
    }

    public struct InitSiloData
    {
        public int slots;

        HashSet<AmmoData> ammo;
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
}
