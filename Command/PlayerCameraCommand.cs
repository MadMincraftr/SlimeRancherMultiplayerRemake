using SRML.Console;
using SRMP.Networking;
using SRMP.Networking.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Command
{
    internal class PlayerCameraCommand : ConsoleCommand
    {
        public override string ID => "mppreview";

        public override string Usage => "mppreview <playernum>";

        public override string Description => "Preview a player's camera.";

        private static bool isOn = false;
        private static int prevPlayer = -1;

        public override bool Execute(string[] args)
        {
            if (args.Length != 1)
            {
                SRML.Console.Console.Instance.LogError("Error processing preview command! Incorrect number of args.");
                return false;
            }
            else
            {
                int arg = int.Parse(args[0]);
                if (TeleportCommand.playerLookup.ContainsKey(arg))
                {
                    if (isOn && prevPlayer == arg)
                    {
                        MultiplayerManager.Instance.EndPlayerPreview();
                        isOn = false;
                        prevPlayer = -1;
                        return true;
                    }

                    int player = TeleportCommand.playerLookup[arg];

                    NetworkPlayer playerObj = SRNetworkManager.players[player];

                    playerObj.StartCamera();

                    prevPlayer = player;
                    isOn = true;
                    return true;
                }
                else
                    SRML.Console.Console.Instance.LogError("Error processing preview command! Invalid player.");
                
                return false;
            }
        }
    }
}
