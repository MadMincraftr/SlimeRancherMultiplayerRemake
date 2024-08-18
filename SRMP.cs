using Mirror;
using SRML.Console;
using System.Collections.Generic;

namespace SRMP
{
    public class SRMP
    {
        public static Dictionary<string,XlateText> localizedTexts = new Dictionary<string,XlateText>();

        private static Dictionary<string,string> revertableTranslations = new Dictionary<string, string>();
        public static void ReplaceTranslation(string bundle, string translated, string key)
        {
            if (!revertableTranslations.ContainsKey($"[{bundle}]-[{key}]"))
                revertableTranslations.Add($"[{bundle}]-[{key}]", GameContext.Instance.MessageDirector.GetBundle(bundle).bundle.dict[key]);

            GameContext.Instance.MessageDirector.GetBundle(bundle).bundle.dict[key] = translated;

            localizedTexts[$"[{bundle}]-[{key}]"].UpdateText();
        }
        public static void RevertTranslation(string bundle, string key)
        {
            if (revertableTranslations.ContainsKey($"[{bundle}]-[{key}]"))
            {
                GameContext.Instance.MessageDirector.GetBundle(bundle).bundle.dict[key] = revertableTranslations[$"[{bundle}]-[{key}]"];

                revertableTranslations.Remove($"[{bundle}]-[{key}]");

                localizedTexts[$"[{bundle}]-[{key}]"].UpdateText();
            }
        }

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
