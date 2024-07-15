using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking
{
    public class SRNetworkManager : NetworkManager
    {
        public override void OnStartClient()
        {
            NetworkHandler.Client.Start(false);
        }
        public override void OnStartHost()
        {
            NetworkHandler.Client.Start(true);
        }
        public override void OnStartServer()
        {
            NetworkHandler.Server.Start();
        }
    }
}
