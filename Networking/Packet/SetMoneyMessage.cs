using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking.Packet
{
    public struct SetMoneyMessage : NetworkMessage
    {
        public int newMoney;
        // public PlayerState.CoinsType type;
    }
    public struct SetKeysMessage : NetworkMessage
    {
        public int newMoney;
        // public PlayerState.CoinsType type;
    }
}
