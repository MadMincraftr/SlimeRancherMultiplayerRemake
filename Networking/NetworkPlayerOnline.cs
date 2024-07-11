using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking
{
    public class NetworkPlayerOnline : SRBehaviour
    {
        public void Start()
        {
            transform.position = SceneContext.Instance.player.transform.position + new UnityEngine.Vector3(0.55f, 0, 0);
        }
    }
}
