using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public class PlayerModel
    {
        //Info
        public int PlayerID { get; set; }
        public string Nickname { get; set; }
        public byte PlayerAvatar { get; set; }
        //Potition
        public float P_X { get; set; }
        public float P_Y { get; set; }
        public float P_Z { get; set; }
        //Rotation
        public short R_X { get; set; }
        public short R_Y { get; set; }
        public short R_Z { get; set; }

    }
}
