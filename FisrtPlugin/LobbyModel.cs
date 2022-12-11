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
        public List<int> playersInLobby;
        private List<Room> rooms = new List<Room>();
        public int PlayersCount { get => players.Count; }
        private long idCounter = 0;
        public int roomsFull = 0;

        public LobbyModel()
        {
            playersInLobby = new List<int>();
            Console.WriteLine("LobbyStatted");
        }

        public void AddClientToLobby(IClient client)
        {
            players.Add(client.ID, client);
            playersInLobby.Add(client.ID);
            client.MessageReceived += Lobby_MessageReceived;
        }
        public void PlayerDisconected(object? sender, ClientDisconnectedEventArgs e)
        {
            if (playersInLobby.Contains(e.Client.ID))
            {
                e.Client.MessageReceived -= Lobby_MessageReceived;
                playersInLobby.Remove(e.Client.ID);
            }
            else
                DeleteOnRoom(e.Client.ID);
        }

        private void DeleteOnRoom(ushort id)
        {
            var room = rooms.Find(r => r.FindPlayer(id));
            room!.QuitPlayer(id);
        }

        private void Lobby_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            switch ((Tags)e.Tag)
            {
                case Tags.JoinGame:
                    Console.WriteLine("New MSG: --------------------------------------------------------------");
                    var newPlayer = e.GetMessage().Deserialize<JoinGameModel>();
                    Console.WriteLine("PlayerID: " + newPlayer.PlayerID);
                    //Console.WriteLine("IdPublic: {0}", newPlayer.IdPublic);
                    Console.WriteLine("Nickname: " + newPlayer.Nickname);
                    Console.WriteLine("PlayerAvatar: " + newPlayer.PlayerAvatar);
                    Console.WriteLine("ModelSkin: {0}", newPlayer.ModelSkin);
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
                Console.WriteLine("RoomCreated");
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
            playersInLobby.Remove(client.ID);
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

        internal void ReturnToLobby(int id)
        {
            if (players.ContainsKey(id))
            {
                players[id].MessageReceived += Lobby_MessageReceived;
                playersInLobby.Add(id);
            }
        }

    }
}
