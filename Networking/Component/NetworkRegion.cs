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
    public class NetworkRegion : MonoBehaviour
    {
        public Region region;
        public int cell;
        void Awake()
        {
            cell = gameObject.name.GetStableHashCode();
            region = GetComponent<Region>();
        }

        public void OwnRegion()
        {

        }

    }
}
