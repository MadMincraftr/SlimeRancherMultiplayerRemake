using SRML.Config.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP
{
    [ConfigFile("srmp")]
    internal class SRMLConfig
    {
        public static readonly int MAX_PLAYERS = 16;
        public static readonly bool EXPERIMENTAL = true;
        public static readonly bool DEBUG_LOG = false;
        public static readonly bool SHOW_SRMP_ERRORS = false;
        public static readonly bool DEBUG_STOP_TUTORIALS = false;
        public static readonly string DEFAULT_CONNECT_IP = "localhost";
        public static readonly ushort DEFAULT_CONNECT_PORT = 7777;
    }
}
