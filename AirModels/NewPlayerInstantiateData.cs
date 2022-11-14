using System;
using DarkRift;

namespace AirModels
{
    public class NewPlayerInstantiateData : IDarkRiftSerializable
    {
       public PlayerModel Player { get; set; }

        public NewPlayerInstantiateData(ushort id,string nickname,byte avatar)
        {
            Player = new PlayerModel()
            { 
                PlayerID= id,
                Nickname = nickname,    
                PlayerAvatar= avatar
            };

        }
        public NewPlayerInstantiateData()
        {

        }

        public void Deserialize(DeserializeEvent e)
        {
            if (Player == null)
                Player = new PlayerModel();
            Player.PlayerID = e.Reader.ReadInt32();
            Player.Nickname = e.Reader.ReadString();
            Player.PlayerAvatar = e.Reader.ReadByte();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Player.PlayerID);
            e.Writer.Write(Player.Nickname);
            e.Writer.Write(Player.PlayerAvatar);
        }
    }
}
