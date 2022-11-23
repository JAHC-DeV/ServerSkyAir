using AirModels;
using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisrtPlugin
{
    public class LobbyModel
    {

        public Dictionary<int, IClient> players = new Dictionary<int, IClient>();
        private List<Room> rooms = new List<Room>();
        public int PlayersCount { get => players.Count; }
        private long idCounter = 0;
        public int roomsFull = 0;
        public void AddClientToLobby(IClient client)
        {
            players.Add(client.ID, client);
            client.MessageReceived += Lobby_MessageReceived;
        }

        private void Lobby_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            switch ((Tags)e.Tag)
            {
                case Tags.JoinGame:
                    Console.WriteLine("New MSG: --------------------------------------------------------------");
                    var newPlayer = e.GetMessage().Deserialize<JoinGameModel>();
                    Console.WriteLine("PlayerID: " + newPlayer.PlayerID);
                    Console.WriteLine("IdPublic: {0}", newPlayer.IdPublic);
                    Console.WriteLine("Nickname: " + newPlayer.Nickname);
                    Console.WriteLine("PlayerAvatar: " + newPlayer.PlayerAvatar);
                    JoinGame(e.Client, newPlayer);
                    Console.WriteLine("-----------------------------------------------------------------------");
                    break;
            }
        }

        private void JoinGame(IClient client, JoinGameModel data)
        {
            if (rooms.Count == 0)
            {
                var room = new Room(this, UpdateCounter());
                client.MessageReceived -= Lobby_MessageReceived;
                room.AddToRoom(client, new PlayerData(data));
                rooms.Add(room);
            }
            else
            {

                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].Count >= rooms[i].MaxPlayers)
                    {
                        roomsFull++;
                        continue;
                    }
                    client.MessageReceived -= Lobby_MessageReceived;
                    rooms[i].AddToRoom(client, new PlayerData(data));
                }
                if (roomsFull >= rooms.Count)
                {
                    var room = new Room(this, UpdateCounter());
                    client.MessageReceived -= Lobby_MessageReceived;
                    room.AddToRoom(client, new PlayerData(data));
                    rooms.Add(room);
                }
            }
        }


        public void SendMessageToAll(Message msg)
        {
            for (int i = 0; i < PlayersCount; i++)
            {
                players[i].SendMessage(msg, SendMode.Reliable);
                msg.Dispose();
            }
        }

        public void SendMessageToPlayer(int id, Message msg, SendMode mode)
        {
            IClient player = players[id];
            if (player != null)
            {
                player.SendMessage(msg, mode);
                msg.Dispose();
            }
        }



        private long UpdateCounter()
        {
            if (idCounter >= 1000000)
                idCounter = 0;
            else
                idCounter++;
            return idCounter;
        }
    }
}
