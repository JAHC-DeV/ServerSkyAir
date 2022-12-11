using AirModels;
using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
        public Room(LobbyModel _lobby, long _id)
        {
            id = _id;
            lobby = _lobby;
        }


        public void AddToRoom(IClient netClient, PlayerData playerData)
        {
            playersData.Add(netClient.ID, playerData);
            netClient.MessageReceived += Room_MessageRecived;

            GeneratePointSpawn(playerData);
        }

        private void Room_MessageRecived(object? sender, MessageReceivedEventArgs e)
        {
            switch ((Tags)e.Tag)
            {
                case Tags.UpdatePlayerData:
                    var playerData = e.GetMessage().Deserialize<PlayerData>();
                    UpdatePlayerInfoToSend(playerData);
                    break;
                case Tags.PlayerShoot:
                    var playerShoot = e.GetMessage().Deserialize<ShootModel>();
                    PlayerShoot(playerShoot);
                    break;
                case Tags.BulletImpact:
                    var bulletimpactData = e.GetMessage().Deserialize<BulletInpactData>();
                    PlayerReciveInpact(bulletimpactData);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Command: {0} Unsopported...", e.Tag);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }
        void PlayerReciveInpact(BulletInpactData data)
        {
            if (playersData.ContainsKey(data.PlayerId))
            {
                playersData[data.PlayerId].Fuselage -= data.BulletDamage;
                playersData[data.PlayerId].LastPlayerInpact = data.BulletId;
                if (playersData[data.PlayerId].Fuselage <= 0)
                {
                    var deadData = new DeadData(data.PlayerId, TypeDead.ImpactBullet, new int[2] { data.BulletId, 0 });
                    SendToAllInRoom(Tags.EnemyDead, deadData, SendMode.Reliable, data.PlayerId,true);
                    return;
                }
                using (var msg = Message.Create((ushort)Tags.BulletImpact, playersData[data.PlayerId]))
                    lobby.SendMessageToPlayer(data.PlayerId, msg, SendMode.Reliable);
            }

        }

        private void PlayerShoot(ShootModel shoot)
        {
            SendToAllInRoom(Tags.PlayerShoot, shoot, SendMode.Reliable, shoot.PlayerID, true);
        }
        private void UpdatePlayerInfoToSend(PlayerData playerData)
        {
            if (playersData.ContainsKey(playerData.PlayerID))
            {
                playersData[playerData.PlayerID].P_X = playerData.P_X;
                playersData[playerData.PlayerID].P_Y = playerData.P_Y;
                playersData[playerData.PlayerID].P_Z = playerData.P_Z;
                playersData[playerData.PlayerID].CurrentSpeed = playerData.CurrentSpeed;
                playersData[playerData.PlayerID].R_X = playerData.R_X;
                playersData[playerData.PlayerID].R_Y = playerData.R_Y;
                playersData[playerData.PlayerID].R_Z = playerData.R_Z;
            }
            SendToAllInRoom(Tags.UpdatePlayerData, playerData, SendMode.Unreliable, (ushort)playerData.PlayerID);

        }

        private void GeneratePointSpawn(PlayerData playerData)
        {
            float x, z;
            x = MathF.Cos(Random.Shared.Next(0, 35)) * 1000;
            z = MathF.Sin(Random.Shared.Next(0, 35)) * 1000;
            playerData.P_X = (int)(x);
            playerData.P_Z = (int)(z);
            playerData.P_Y = 20;
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
                    catch (Exception) { Console.WriteLine("Aqui 3"); continue; }
                }
            }
        }

        public bool FindPlayer(int id)
        {
            return playersData.ContainsKey(id);
        }

        private void SendToAllInRoom<T>(Tags tag, T data, SendMode mode, ushort myId, bool sendToMy = false) where T : IDarkRiftSerializable
        {
            using (var msg = Message.Create((ushort)tag, data))
            {
                for (int i = 0; i < playersData.Count; i++)
                {
                    try
                    {
                        if (myId == playersData.Keys.ToArray()[i] && !sendToMy)
                            continue;

                        lobby.SendMessageToPlayer(playersData[playersData.Keys.ToArray()[i]].PlayerID, msg, mode);
                    }
                    catch (Exception e) { Console.WriteLine("Aqui 1: {0}---- CountPlayers: {1}", e.Message, playersData.Count); continue; }

                }

            }
        }

        public void QuitPlayer(ushort id)
        {
            if (playersData.ContainsKey(id))
            {
                SendToAllInRoom(Tags.PlayerLeave, new PlayerLeave(playersData[id]), SendMode.Reliable, id);
                Console.WriteLine("Send PlayerLeave: {0}", id);
                playersData.Remove(id);
                lobby.players[id].MessageReceived -= Room_MessageRecived;
            }

        }
    }
}
