using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public class BulletInpactData : IDarkRiftSerializable
    {
        public ushort PlayerId { get; set; }
        public ushort BulletId { get; set; }
        public ushort BulletDamage { get; set; }
        public BulletInpactData(ushort _playerId, ushort _bulletId,ushort _bulletDamage)
        {
            PlayerId = _playerId;
            BulletId = _bulletId;
            BulletDamage = _bulletDamage;
        }
        public BulletInpactData()
        {

        }
        public void Deserialize(DeserializeEvent e)
        {
            PlayerId = e.Reader.ReadUInt16();
            BulletId = e.Reader.ReadUInt16();
            BulletDamage = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerId);
            e.Writer.Write(BulletId);
            e.Writer.Write(BulletDamage);
        }
    }
}
