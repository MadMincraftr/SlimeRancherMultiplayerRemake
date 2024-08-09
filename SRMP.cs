using Mirror;
using SRML.Console;

namespace SRMP
{
    public class SRMP
    {
        private static Console.ConsoleInstance conInstance = new Console.ConsoleInstance("SRMP");
        // I need to be able to look at stuff in UE.
        internal static ushort MessageId<M>() where M: struct, NetworkMessage => NetworkMessageId<M>.Id; 
        // For a debug log on Transport Data Recieved on server.
        internal static ushort DebugUShortFromBytes(byte byte1, byte byte2)
        {
            return (ushort)((byte1 << 8) | byte2);
        }
        public static void Log(string message)
        {
            conInstance.Log(message);
        }
    }
}
