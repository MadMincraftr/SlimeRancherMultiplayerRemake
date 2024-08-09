using Mirror;
using MonomiPark.SlimeRancher.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Component
{
    [DisallowMultipleComponent]
    public class NetworkRegion : MonoBehaviour
    {
        public List<int> players = new List<int>();

        public static Dictionary<string, NetworkRegion> all = new Dictionary<string, NetworkRegion>();

        void Awake()
        {
            all.Add(gameObject.name, this);
        }

        public void AddPlayer(int player)
        {
            players.Add(player);
        }

        public void RemovePlayer(int player)
        {
            players.Remove(player);
        }

    }
}
