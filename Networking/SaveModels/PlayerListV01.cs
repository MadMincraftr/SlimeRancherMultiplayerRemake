using MonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking.SaveModels
{
    public class PlayerListV01 : PersistedDataSet
    {

        public override string Identifier => "MPPL";
        public override uint Version => 1;

        public Dictionary<Guid,NetPlayerV01> playerList = new Dictionary<Guid, NetPlayerV01>();

        public override void LoadData(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            var set = new Dictionary<Guid, NetPlayerV01>();

            for (int i = 0; i < count; i++)
            {
                var guid = Guid.Parse(reader.ReadString());
                var player = NetPlayerV01.Load(reader);

                set.Add(guid,player);
            }
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(playerList.Count);
            foreach (var player in playerList)
            {
                writer.Write(player.Key.ToString());
                player.Value.WriteData(writer);
            }
        }


        public static PlayerListV01 Load(BinaryReader reader)
        {
            var list = new PlayerListV01();
            list.LoadData(reader);
            return list;
        }
    }
}
