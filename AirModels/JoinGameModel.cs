﻿using System;
using DarkRift;

namespace AirModels
{
    public class JoinGameModel : PlayerModel
    {
        public JoinGameModel(ushort id,string idPublic, string nickname, ushort avatar,ushort modelSkin)
        {
            PlayerID = id;
            IdPublic = idPublic;
            Nickname = nickname;
            PlayerAvatar = avatar;
            ModelSkin = modelSkin;
        }
        public JoinGameModel()
        {

        }

        public override void Deserialize(DeserializeEvent e)
        {

            PlayerID = e.Reader.ReadInt32();
            IdPublic = e.Reader.ReadString();
            Nickname = e.Reader.ReadString();
            PlayerAvatar = e.Reader.ReadUInt16();
            ModelSkin = e.Reader.ReadUInt16();
        }

        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerID);
            e.Writer.Write(IdPublic);
            e.Writer.Write(Nickname);
            e.Writer.Write(PlayerAvatar);
            e.Writer.Write(ModelSkin);
        }
    }
}
