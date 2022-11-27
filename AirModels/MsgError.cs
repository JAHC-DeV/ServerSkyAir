using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public class MsgError : IDarkRiftSerializable
    {
        public int PlayerID { get; set; }
        public string Message { get; set; }
        public MsgError(int playerID, string message)
        {
            PlayerID = playerID;
            Message = message;
        }
        public MsgError()
        {

        }
        public void Deserialize(DeserializeEvent e)
        {
            PlayerID = e.Reader.ReadInt32();
            Message = e.Reader.ReadString();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerID);
            e.Writer.Write(Message);
        }
    }
}
