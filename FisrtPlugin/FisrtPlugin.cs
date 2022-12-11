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
        private LobbyModel lobby;

        public FisrtPlugin(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            Console.WriteLine("Plugin Loader");
            lobby = new LobbyModel();            
            ClientManager.ClientConnected += NewClient_Connected;
            ClientManager.ClientDisconnected += lobby.PlayerDisconected;
        }

        private void NewClient_Connected(object? sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("New Client Connected\t ID: {0}",e.Client.ID);
            lobby.AddClientToLobby(e.Client);
        }
    }
}