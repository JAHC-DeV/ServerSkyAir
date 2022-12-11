using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public class DeadData : IDarkRiftSerializable
    {
        public ushort PlayerId { get; set; }
        public ushort DeadType { get; set; }
        public int[] Data { get; set; }
        public DeadData(ushort _playerId, TypeDead type, int[] _data)
        {
            PlayerId = _playerId;
            DeadType = (ushort)type;
            Data = _data;

        }
        public DeadData()
        {

        }
        public void Deserialize(DeserializeEvent e)
        {
            PlayerId = e.Reader.ReadUInt16();
            DeadType = e.Reader.ReadUInt16();
            Data = e.Reader.ReadInt32s();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerId);
            e.Writer.Write(DeadType);
            e.Writer.Write(Data);
        }
    }
}
