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
    public struct GardenPlantMessage : NetworkMessage
    {
        public string id;
        public Identifiable.Id ident;
        public bool replace;
    }
}
