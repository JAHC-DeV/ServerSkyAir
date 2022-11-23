using AirModels;
using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FisrtPlugin
{
    public class Room
    {
        private Dictionary<int, PlayerData> playersData = new Dictionary<int, PlayerData>();
        private long id;
        public long ID { get => id; }
        public int Count { get => playersData.Count; }
        private int maxPlayers = 20;
        public int MaxPlayers { get => maxPlayers; }
        private LobbyModel lobby;
        public Room(LobbyModel _lobby,long _id)
        {
            id = _id;
            lobby = _lobby;
        }


        public void AddToRoom(IClient netClient, PlayerData playerData)
        {
            playersData.Add(netClient.ID, playerData);
            netClient.MessageReceived += Room_MessageRecived;
        }

        private void Room_MessageRecived(object? sender, MessageReceivedEventArgs e)
        {
            switch ((Tags)e.Tag)
            {
                case Tags.UpdatePlayerData:
                    var playerData = e.GetMessage().Deserialize<PlayerData>();
                    UpdatePlayerInfoToSend(playerData);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Command: {0} Unsopported...", e.Tag);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }
        private void UpdatePlayerInfoToSend(PlayerData playerData)
        {
            if (playersData.ContainsKey(playerData.PlayerID))
                playersData[playerData.PlayerID] = playerData;

            using (var msg = Message.Create((ushort)Tags.UpdatePlayerData, playerData))
            {
                for (int i = 0; i < playersData.Count; i++)
                {
                    try
                    {
                        if (playerData.PlayerID == playersData.Keys.ToArray()[i])
                            continue;

                        lobby.SendMessageToPlayer(playerData.PlayerID, msg, SendMode.Unreliable);
                    }
                    catch (Exception) { Console.WriteLine("Aqui 1"); continue; }

                }

            }
        }

        private void GeneratePointSpawn(PlayerData playerData)
        {
            float x, y;
            x=  MathF.Cos(Random.Shared.Next(0, 35));
            y = MathF.Sin(Random.Shared.Next(0, 35));
            playerData.P_X = (int)x * 7500;
            playerData.P_Y = (int)y * 7500;
            using (var msg = Message.Create((ushort)Tags.PlayerEnter, playerData))
            {
                DarkRiftWriter allPlayer = DarkRiftWriter.Create();
                try
                {
                    Console.WriteLine("playersInLobby: {0}", playersData.Count);
                    if (playersData.Count != 0)
                        allPlayer.Write(playersData.Values.ToArray());
                    else
                        allPlayer.Write(new PlayerData[1] { playerData });
                }
                catch (Exception) { Console.WriteLine("Aqui 2"); }

                foreach (var item in playersData)
                {
                    try
                    {
                        if (playerData.PlayerID == item.Key)
                        {
                            msg.Tag = (ushort)Tags.PlayersList;
                            msg.Serialize(allPlayer);
                            lobby.SendMessageToPlayer(item.Key, msg, SendMode.Reliable);
                            msg.Serialize(playerData);
                            msg.Tag = (ushort)Tags.PlayerEnter;
                            continue;
                        }
                        lobby.SendMessageToPlayer(item.Key, msg, SendMode.Reliable);
                    }
                    catch (Exception) { Console.WriteLine("Aqui 2"); continue; }
                }
            }
        }
    }
}
