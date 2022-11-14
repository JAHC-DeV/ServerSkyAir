using AirModels;
using DarkRift;
using DarkRift.Server;
using System.Linq.Expressions;
using System.Linq;
using System.Numerics;

namespace FisrtPlugin
{
    public class FisrtPlugin : Plugin
    {
        public override bool ThreadSafe => false;
        public override Version Version => new Version(1, 0);

        public float[] P_1 = new float[3] { -2.11f, -6.09f, 13.11059f};
        public float[] P_2 = new float[3] { 21.2f, -6.09f, 13.11059f };

        private List<PlayerData> playersInLobby;
        public FisrtPlugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            Console.WriteLine("AlRight");
            playersInLobby = new List<PlayerData>();
            ClientManager.ClientConnected += NewClient_Connected;
            ClientManager.ClientDisconnected += Client_DisconnectedEvent;
        }

        private void Client_DisconnectedEvent(object? sender, ClientDisconnectedEventArgs e)
        {
            PlayerData? out_Player = playersInLobby.Find(f => f.PlayerID == e.Client.ID);
            e.Client.MessageReceived -= NewGlobal_MessageReceived;
            DisconnectPlayer(out_Player);
        }

        private void NewClient_Connected(object? sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("New Client Connected\t ID: {0}",e.Client.ID);
            e.Client.MessageReceived += NewGlobal_MessageReceived;
        }

        private void NewGlobal_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("");
            switch ((Tags)e.Tag)
            {
                case Tags.NewPlayerInstantiateData:
                    Console.WriteLine("New MSG: --------------------------------------------------------------");
                    var newPlayer = e.GetMessage().Deserialize<NewPlayerInstantiateData>();                        
                    Console.WriteLine("PlayerID: " + newPlayer.Player.PlayerID);
                    Console.WriteLine("Nickname: " + newPlayer.Player.Nickname);
                    Console.WriteLine("PlayerAvatar: " + newPlayer.Player.PlayerAvatar);
                    AddNewPlayer(newPlayer.Player);
                    Console.WriteLine("-----------------------------------------------------------------------");
                    break;
                case Tags.UpdatePlayerData:
                    var playerData = e.GetMessage().Deserialize<PlayerData>();
                    UpdatePlayerInfoToSend(playerData);
                    Console.WriteLine("Update Recived");
                    break;
                default:
                    break;
            }
        }

        private void UpdatePlayerInfoToSend(PlayerData playerData)
        {
            playersInLobby[playerData.PlayerID] = playerData;
            using (var msg = Message.Create((ushort)Tags.UpdatePlayerData, playerData))
            {
                for (int i = 0; i < ClientManager.GetAllClients().Length; i++)
                {
                    if (playerData.PlayerID == ClientManager.GetAllClients()[i].ID)
                        continue;
                    try
                    {
                        ClientManager.GetAllClients()[i].SendMessage(msg, SendMode.Unreliable);
                    }
                    catch (Exception) { Console.WriteLine("Aqui 1"); continue; }
                   
                }

            }
        }

        private void AddNewPlayer(PlayerModel player)
        {
           Random random = new Random();
           var result =  random.NextInt64(0, 2);
            if (result == 0)
            {
                player.P_X = P_1[0];
                player.P_Y = P_1[1];
                player.P_Z = P_1[2];
            }
            else
            {
                player.P_X = P_2[0];
                player.P_Y = P_2[1];
                player.P_Z = P_2[2];
            }                      
            var playerData = new PlayerData(player);
            playersInLobby.Add(playerData);

            using (var msg = Message.Create((ushort)Tags.NewPlayerSpawnData, playerData))
            {
                DarkRiftWriter allPlayer = DarkRiftWriter.Create();
                foreach (var item in playersInLobby)
                {
                    try
                    {
                        allPlayer.Write(new PlayerData(item));
                    }
                    catch (Exception) { Console.WriteLine("Aqui 2"); continue; }
                }
               
                foreach (var item in ClientManager.GetAllClients())
                {
                    try
                    {
                        if (player.PlayerID == item.ID)
                        {
                            msg.Tag = (ushort)Tags.AllPlayersSpawnData;
                            msg.Serialize(allPlayer);
                            item.SendMessage(msg, SendMode.Reliable);
                            msg.Serialize(playerData);
                            msg.Tag = (ushort)Tags.NewPlayerSpawnData;
                            continue;
                        }
                        item.SendMessage(msg, SendMode.Reliable);
                    }
                    catch (Exception) { Console.WriteLine("Aqui 2"); continue; }
                }
            }          
            Console.WriteLine("Player Add In PointSpawn {0}",result);
        }

        private void DisconnectPlayer(PlayerData? player)
        {
            using (var msg = Message.Create((ushort)Tags.DisconnectedPlayerData,new PlayerData(player)))
            {
                foreach (var item in ClientManager.GetAllClients())
                {
                    try
                    {
                        if (player.PlayerID == item.ID)
                            continue;
                        item.SendMessage(msg, SendMode.Reliable);
                    }
                    catch (Exception) { Console.WriteLine("Aqui 3"); continue; }
                }

                if (playersInLobby.Contains(player))                
                    playersInLobby.Remove(player);                    
            }
        }
    }
}