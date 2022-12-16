using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public class DeadData : IDarkRiftSerializable
    {
        public int PlayerId { get; set; }
        public ushort DeadType { get; set; }
        public float[] Data { get; set; }
        public DeadData(int _playerId, TypeDead type, float[] _data)
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
            PlayerId = e.Reader.ReadInt32();
            DeadType = e.Reader.ReadUInt16();
            Data = e.Reader.ReadSingles();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerId);
            e.Writer.Write(DeadType);
            e.Writer.Write(Data);
        }
    }
}
