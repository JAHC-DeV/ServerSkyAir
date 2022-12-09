using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public class PlayerLeave: PlayerModel
    {
        public PlayerLeave()
        {

        }
        public PlayerLeave(PlayerModel model)
        {
            PlayerID = model.PlayerID;
            Nickname = model.Nickname;           
        }
        public override void Deserialize(DeserializeEvent e)
        {
            PlayerID = e.Reader.ReadInt32();
            Nickname = e.Reader.ReadString();          
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerID);
            e.Writer.Write(Nickname);         
        }
    }
}
