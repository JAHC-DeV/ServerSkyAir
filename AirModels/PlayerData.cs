using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public class PlayerData : PlayerModel
    {
        public PlayerData(PlayerModel model)
        {
            PlayerID = model.PlayerID;
            Nickname = model.Nickname;
            PlayerAvatar = model.PlayerAvatar;
            //Potition
            P_X = model.P_X;
            P_Y = model.P_Y;
            P_Z = model.P_Z;
            //Rotation
            R_X = model.R_X;
            R_Y = model.R_Y;
            R_Z = model.R_Z;
            //Extra
            CurrentSpeed = model.CurrentSpeed;
        }
        public PlayerData()
        {

        }
        public override void Deserialize(DeserializeEvent e)
        {
            PlayerID = e.Reader.ReadInt32();
            Nickname = e.Reader.ReadString();
            PlayerAvatar = e.Reader.ReadByte();
            //Potition
            P_X = e.Reader.ReadSingle();
            P_Y = e.Reader.ReadSingle();
            P_Z = e.Reader.ReadSingle();
            //Rotation
            R_X = e.Reader.ReadInt16();
            R_Y = e.Reader.ReadInt16();
            R_Z = e.Reader.ReadInt16();
            //Extra
            CurrentSpeed = e.Reader.ReadSingle();        
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerID);
            e.Writer.Write(Nickname);
            e.Writer.Write(PlayerAvatar);
            //Potition
            e.Writer.Write(P_X);
            e.Writer.Write(P_Y);
            e.Writer.Write(P_Z);
            //Rotation
            e.Writer.Write(R_X);
            e.Writer.Write(R_Y);
            e.Writer.Write(R_Z);
            //Extra
            e.Writer.Write(CurrentSpeed);
        }
    }
}
