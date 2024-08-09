using SRML.Console;
using SRMP.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Command
{
    internal class TeleportCommand : ConsoleCommand
    {
        public static Dictionary<int, int> playerLookup = new Dictionary<int, int>(); 
        public override string ID => "tp";

        public override string Usage => "tp <playernum>";

        public override string Description => "Teleport to a player.";

        public override bool Execute(string[] args)
        {
            if (args.Length != 1)
            {
                SRML.Console.Console.Instance.LogError("Error processing teleport command! Incorrect number of args.");
                return false;
            }
            else
            {
                int arg = int.Parse(args[0]);
                if (playerLookup.ContainsKey(arg))
                {
                    int player = playerLookup[arg];

                    SceneContext.Instance.player.transform.position = SRNetworkManager.players[player].transform.position;

                    return true;
                }
                else
                    SRML.Console.Console.Instance.LogError("Error processing teleport command! Invalid player.");
                
                return false;
            }
        }
    }
}
