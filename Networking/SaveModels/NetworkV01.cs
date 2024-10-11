using MonomiPark.SlimeRancher.Persist;
using System.IO;

namespace SRMP.Networking.SaveModels
{

    // To future self; make sure you use VersionedPersistedDataSet<NetworkV01> for upgrading the network version
    // Also please take changelogs for it as well.
    public class NetworkV01 : PersistedDataSet
    {
        public override string Identifier => "MPNK";
        public override uint Version => 1;

        public bool sharedMoney = true;

        public bool sharedKeys = true;
        
        public bool sharedUpgrades = true;

        public PlayerListV01 savedPlayers = new PlayerListV01();

        public override void LoadData(BinaryReader reader)
        {
            sharedMoney = reader.ReadBoolean();
            sharedKeys = reader.ReadBoolean();
            sharedUpgrades = reader.ReadBoolean();
        
            savedPlayers = PlayerListV01.Load(reader);
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(sharedMoney);
            writer.Write(sharedKeys);
            writer.Write(sharedUpgrades);

            savedPlayers.WriteData(writer);
        }
    }
}
