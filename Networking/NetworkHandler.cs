using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms;

namespace SRMP.Networking
{
    public class NetworkHandler
    {
        public class Server
        {
            internal static void Start()
            {
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, TestLogMessage>(HandleTestLog));
                NetworkServer.RegisterHandler(new Action<NetworkConnectionToClient, SetMoneyMessage>(HandleMoneyChange));

            }
            public static void HandleTestLog(NetworkConnectionToClient nctc, TestLogMessage packet)
            {
                SRMP.Log(packet.MessageToLog);
            }
            public static void HandleMoneyChange(NetworkConnectionToClient nctc, SetMoneyMessage packet)
            {
                int adj = packet.newMoney - SceneContext.Instance.PlayerState.GetCurrency();
                SceneContext.Instance.PlayerState.AddCurrency(adj);

                // Notify others
                NetworkServer.SendToAllExcept(packet, nctc);
            }
        }
        public class Client
        {

            internal static void Start(bool host)
            {
                NetworkClient.RegisterHandler(new Action<SetMoneyMessage>(HandleMoneyChange));

            }
            public static void HandleMoneyChange(SetMoneyMessage packet)
            {
                int adj = packet.newMoney - SceneContext.Instance.PlayerState.GetCurrency();
                SceneContext.Instance.PlayerState.AddCurrency(adj);
            }
        }
    }
}
