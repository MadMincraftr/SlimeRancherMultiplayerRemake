using Mirror;
using MonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlayerState;
using UnityEngine;
namespace SRMP.Networking.SaveModels
{
    public class NetPlayerV01 : PersistedDataSet
    {
        public override string Identifier => "MPP";
        public override uint Version => 1;

        public Vector3V02 position;
        public Vector3V02 rotation;

        public NetPlayerV01()
        {
            position = new Vector3V02();
            position.value = new Vector3(89.29f, 14.9064f, -144.46f);
            rotation = new Vector3V02();
            rotation.value = Vector3.up * 24.9084f;

            Ammo.Slot[] normalSlots = new Ammo.Slot[]
            {
                null,
                null,
                null,
                null,
                null
            };Ammo.Slot[] nimbleSlots = new Ammo.Slot[]
            {
                null,
                null,
                null
            };

            ammo.Add(AmmoMode.DEFAULT, GameContext.Instance.AutoSaveDirector.SavedGame.AmmoDataFromSlots(normalSlots));
            ammo.Add(AmmoMode.NIMBLE_VALLEY, GameContext.Instance.AutoSaveDirector.SavedGame.AmmoDataFromSlots(nimbleSlots));
        }

        public int keys = 0;
        public int money = 250;

        public Dictionary<PlayerState.AmmoMode, List<AmmoDataV02>> ammo = new Dictionary<AmmoMode, List<AmmoDataV02>>();

        public List<PlayerState.Upgrade> upgrades = new List<Upgrade>();

        public Guid playerID;


        public static NetPlayerV01 Load(BinaryReader reader)
        {
            var netPlayer = new NetPlayerV01();
            netPlayer.LoadData(reader);
            return netPlayer;
        }

        public override void LoadData(BinaryReader reader)
        {
            position = Vector3V02.Load(reader);
            rotation = Vector3V02.Load(reader);

            keys = reader.ReadInt32();
            money = reader.ReadInt32();

            var ammoModeC = reader.ReadInt32();

            ammo = new Dictionary<PlayerState.AmmoMode, List<AmmoDataV02>>();

            for (int i = 0; i < ammoModeC; i++)
            {
                var ammoDataC = reader.ReadInt32();

                var ammoData = new List<AmmoDataV02>();

                for (int x = 0; x < ammoDataC; x++)
                {
                    var ammoSlot = new AmmoDataV02();
                    ammoSlot.LoadData(reader);

                    ammoData.Add(ammoSlot);
                }
                var ammoMode = reader.ReadByte();

                ammo.Add((AmmoMode)ammoMode, ammoData);
            }

            var upgradeC = reader.ReadInt32();

            upgrades = new List<Upgrade>();

            for (int i = 0; i < upgradeC; i++)
            {
                upgrades.Add((Upgrade)reader.ReadByte());
            }

            playerID = Guid.Parse(reader.ReadString());
        }

        public override void WriteData(BinaryWriter writer)
        {
            position.WriteData(writer);
            rotation.WriteData(writer);

            writer.Write(keys);
            writer.Write(money);

            writer.Write(ammo.Count);

            foreach (var amm in ammo)
            {
                writer.Write(amm.Value.Count);
                foreach (var ammoSlot in amm.Value)
                {
                    ammoSlot.WriteData(writer);
                }
                writer.Write((byte)amm.Key);
            }

            writer.Write(upgrades.Count);

            foreach (var upgrade in upgrades)
            {
                writer.Write((byte)upgrade);
            }
        }
    }
}
