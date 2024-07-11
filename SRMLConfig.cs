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
    }
}
