using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public class PlayerModel
    {
        //Control
      
        
        //Info
        public int PlayerID { get; set; }
        public string IdPublic { get; set; }
        public ushort IdRoom { get; set; }
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
        //Extra
        private float currentSpeed = 0.0f;
        public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
        public float CurrentRollSpeed { get; set; }
        public float CurrentPitchSpeed { get; set; }
        public float CurrentYawSpeed { get; set; }
        public float Input_H { get; set; }
        public float Input_V { get; set;}
    }
}
