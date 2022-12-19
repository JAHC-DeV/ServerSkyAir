using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public class PlayerModel : IDarkRiftSerializable
    {
        //Control
      
        
        //Info
        public int PlayerID { get; set; }
        public string IdPublic { get; set; }
        public ushort IdRoom { get; set; }
        public string Nickname { get; set; }
        public ushort PlayerAvatar { get; set; }
        public ushort ModelSkin { get; set; }
        public ushort Fuselage { get; set; }
        public bool IsAlive { get; set; }
        public ushort Kills { get; set; }
        //Potition
        public float P_X { get; set; }
        public float P_Y { get; set; }
        public float P_Z { get; set; }
        //Rotation
        public short R_X { get; set; }
        public short R_Y { get; set; }
        public short R_Z { get; set; }
        //Extra
        public float CurrentSpeed { get; set; }
        public int BulletCount { get; set; }

        public virtual void Deserialize(DeserializeEvent e){}

        public virtual void Serialize(SerializeEvent e) { }

    }
}
