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
        //Contiene Todos Los Datos de los players en la Sala
        private Dictionary<int, PlayerData> playersData = new Dictionary<int, PlayerData>();
        /// <summary>
        /// Id de la Sala 
        /// </summary>
        private long id;
        /// <summary>
        /// Propiedad Id de la Sala
        /// </summary>
        public long ID { get => id; }
        /// <summary>
        /// Propiedad cantidad de clientes en la sala
        /// </summary>
        public int Count { get => playersData.Count; }
        /// <summary>
        /// Maximo de clientes en la sala
        /// </summary>
        private int maxPlayers = 20;
        /// <summary>
        /// Propiedad Maximo de clientes en la sala
        /// </summary>
        public int MaxPlayers { get => maxPlayers; }
        /// <summary>
        /// Referencia al lobby principal
        /// </summary>
        private LobbyModel lobby;

        /// <summary>
        /// Contructor de la sala
        /// </summary>
        /// <param name="_lobby">Referencia al lobby principal</param>
        /// <param name="_id">Id de la sala</param>
        public Room(LobbyModel _lobby, long _id)
        {
            id = _id;
            lobby = _lobby;
        }

        /// <summary>
        /// Agrega un nuevo cliente a la sala
        /// </summary>
        /// <param name="netClient">Interfas referente a la comunicacion con el cliente remoto</param>
        /// <param name="playerData">Datos actuales del cliente</param>
        public void AddToRoom(IClient netClient, PlayerData playerData)
        {
            playersData.Add(netClient.ID, playerData);
            netClient.MessageReceived += Room_MessageRecived;

            GeneratePointSpawn(playerData);
        }
        /// <summary>
        /// Metodo que ejecuta el evento de lectura de los msg enviados por los clientes
        /// </summary>
        /// <param name="sender">Objeto mismo</param>
        /// <param name="e">Elemento que contiene la informacion recivida</param>
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
                case Tags.EnemyDead:
                    var deadData = e.GetMessage().Deserialize<DeadData>();
                    PlayerDead(deadData);
                    break;
                case Tags.PlayerLeave:
                    var playerLeave = e.GetMessage().Deserialize<PlayerLeave>();
                    QuitPlayer(playerLeave.PlayerID,!playerLeave.IsAlive);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Command: {0} Unsopported...", e.Tag);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }
        /// <summary>
        /// Metodo que se ejecuta cuando se recive que un cliente a muerto
        /// </summary>
        /// <param name="deadData">Define el tipo de muerte que tubo el cliente</param>
        private void PlayerDead(DeadData deadData)
        {
            SendToAllInRoom(Tags.EnemyDead, deadData, SendMode.Reliable, deadData.PlayerId);
            if (playersData.ContainsKey(deadData.PlayerId))
            {
                var playerImpact = playersData[deadData.PlayerId].LastPlayerImpact;
                if (playerImpact != -1 && playersData.ContainsKey(playerImpact))
                {
                    playersData[playerImpact].Kills++;
                    using (var msg = Message.Create((ushort)Tags.UpdatePlayerData, playersData[playerImpact]))
                       lobby.SendMessageToPlayer(playerImpact,msg,SendMode.Reliable);
                }
            }
        }
        
        /// <summary>
        /// Metodo que se ejecuta cuando se recive que un cliente esta disparando
        /// </summary>
        /// <param name="shoot">Datos de la rafaga envida</param>
        private void PlayerShoot(ShootModel shoot)
        {
            Console.WriteLine("Player Shoot: " + shoot.PlayerID);
            SendToAllInRoom(Tags.PlayerShoot, shoot, SendMode.Reliable, shoot.PlayerID, true);
        }
        /// <summary>
        /// Metodo que se ejecuta cuando se recive una actualizacion de los datos del player
        /// </summary>
        /// <param name="playerData"></param>
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

        /// <summary>
        /// Genera la posicion donde se Intanciara el cliente una vez unido a la sala
        /// </summary>
        /// <param name="playerData">Datos del cliete a editar la posicion</param>
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
        /// <summary>
        /// Busca un player en la sala
        /// </summary>
        /// <param name="id">Id del cliente a buscar</param>
        /// <returns>Retorna si el cliente vive en esta sala o no</returns>
        public bool FindPlayer(int id)
        {
            return playersData.ContainsKey(id);
        }
        /// <summary>
        /// Envia un msg a todos los clientes de la sala
        /// </summary>
        /// <typeparam name="T">Tipo de dato que se va a enviar</typeparam>
        /// <param name="tag">Tipo de msg a enviar</param>
        /// <param name="data">Dato a enviar al cliente</param>
        /// <param name="mode">Modo en el cual se enviaran los datos</param>
        /// <param name="myId">Id del cliente que solicita enviar los datos</param>
        /// <param name="sendToMy">Se enviara al mismo cliente que solicita enviar??</param>
        private void SendToAllInRoom<T>(Tags tag, T data, SendMode mode, int myId, bool sendToMy = false) where T : IDarkRiftSerializable
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
        /// <summary>
        /// Elimina un cliente de la sala
        /// </summary>
        /// <param name="id">Id del cliente a eliminar</param>
        /// <param name="isDead">Define si se eliminara por que el cliente murio</param>
        public void QuitPlayer(int id,bool isDead = false)
        {
            if (playersData.ContainsKey(id))
            {
                SendToAllInRoom(Tags.PlayerLeave, new PlayerLeave(playersData[id]), SendMode.Reliable, id);
                Console.WriteLine("Send PlayerLeave: {0}", id);
                playersData.Remove(id);
                lobby.players[id].MessageReceived -= Room_MessageRecived;
                if (isDead)
                    lobby.ReturnToLobby(id);
            }
        }
    }
}
