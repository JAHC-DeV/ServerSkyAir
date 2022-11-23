using System;
using DarkRift;

namespace AirModels
{
    public class JoinGameModel : PlayerModel, IDarkRiftSerializable
    {
        public JoinGameModel(ushort id,string idPublic, string nickname, byte avatar)
        {
            PlayerID = id;
            IdPublic = idPublic;
            Nickname = nickname;
            PlayerAvatar = avatar;
        }
        public JoinGameModel()
        {

        }

        public void Deserialize(DeserializeEvent e)
        {

            PlayerID = e.Reader.ReadInt32();
            IdPublic = e.Reader.ReadString();
            Nickname = e.Reader.ReadString();
            PlayerAvatar = e.Reader.ReadByte();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerID);
            e.Writer.Write(IdPublic);
            e.Writer.Write(Nickname);
            e.Writer.Write(PlayerAvatar);
        }
    }
}
