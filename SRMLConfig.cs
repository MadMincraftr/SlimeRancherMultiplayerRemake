using SRML.Config.Attributes;
using Steamworks;
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
        /*
        [ConfigComment("Use steam servers for hosting.\n; Only works if all players use steam.")]
        public static readonly bool USE_PLATFORM = false;

        [ConfigComment("(Only use if \"USE_PLATFORM\" is set to true.)\n; Steam Lobby visibility\n; -- Values --\n; 0 -- Invite Only (UNIMPLEMENTED)\n; 1 -- Friends Only (Friends can join anytime while hosting.)\n; 2 -- Joinable by anyone, while hosting.\n; 3 -- Invisible (do not use, untested, and idk if you can even join people)\n; 4 -- Private + Unique (same as Invite Only (UNIMPLEMENTED), but lobby id is unique.)\n; \"\"But wait! if Invite Only and Private+Unique are \"UNIMPLEMENTED\" why is Invisible just listed as \"untested\".\"\" Good question! UNIMPLEMENTED means i havent coded a full invite system, which also means that some methods of inviting like normal dont work for those settings. untested means i havent checked if thats the case for that ")]
        public static readonly byte STEAM_LOBBY_MODE = (byte)ELobbyType.k_ELobbyTypeFriendsOnly;
        */
        [ConfigComment("Max players in lobby (untested)")]
        public static readonly byte MAX_PLAYERS = 4;

        [ConfigComment("Log debug information.\n; [[WARNING, FILLS UP LOG WITH RANDOM INFO THAT IS UNIMPORTANT]]")]
        public static readonly bool DEBUG_LOG = false;

        [ConfigComment("Logs SRMP Errors.\n; Use if you are having problems with the mod, which you then send the ENTIRE log file to a developer.")]
        public static readonly bool SHOW_SRMP_ERRORS = false;

        [ConfigComment("Removes tutorials.")]
        public static readonly bool DEBUG_STOP_TUTORIALS = false;

        [ConfigComment("Default IP address to connect to.")]
        public static readonly string DEFAULT_CONNECT_IP = "localhost";

        [ConfigComment("Default port to use.")]
        public static readonly ushort DEFAULT_PORT = 7777;
    }
}
