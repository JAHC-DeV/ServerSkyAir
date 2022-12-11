using DarkRift;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AirModels
{
    public class ShootModel : ObjectModel
    {
        public ushort PlayerID { get; set; }
        public ushort BulletCount { get; set; }
        public short PlayerImpact { get; set; }
        public ushort Damage { get; set; }
        public ShootModel(ushort _playerId,ushort _bulletCount,ushort _damage,short _playerImpact)
        {
            PlayerImpact = _playerImpact;
            PlayerID = _playerId;
            BulletCount = _bulletCount;
            Damage = _damage;
        }
        public ShootModel() { }

        public override void Deserialize(DeserializeEvent e)
        {
            PlayerID = e.Reader.ReadUInt16();
            BulletCount = e.Reader.ReadUInt16();
            PlayerImpact = e.Reader.ReadInt16();
            Damage = e.Reader.ReadUInt16();
            P_X = e.Reader.ReadSingle();
            P_Y = e.Reader.ReadSingle();
            P_Z = e.Reader.ReadSingle();
            DestroyIn = e.Reader.ReadSingle();
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerID);
            e.Writer.Write(BulletCount);
            e.Writer.Write(PlayerImpact);
            e.Writer.Write(Damage);
            e.Writer.Write(P_X);
            e.Writer.Write(P_Y);
            e.Writer.Write(P_Z);
            e.Writer.Write(DestroyIn);
        }
    }
}
