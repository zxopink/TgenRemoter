using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TgenNetProtocol;
using TgenSerializer;
using static TgenRemoteCodes.NetworkCodes;

namespace TgenRemoteServer
{
    internal class Lobby : NetworkBehavour
    {
        List<Client> clients = new List<Client>();
        ServerManager server;

        const int SERVER_PORT = 7777;
        const int CONTROLLER_PORT = 7788;
        const int CONTROLLED_PORT = 7799;
        readonly TimeSpan TimeOut = TimeSpan.FromSeconds(25);

        private readonly ILogger<Worker> _logger;

        public Lobby(ILogger<Worker> logger)
        {
            _logger = logger;
            Setup();
        }

        public Task ExecuteAsync(CancellationToken stoppingToken) =>
            RunServer(stoppingToken);

        public void StopAsync(CancellationToken cancellationToken) =>
            server.Close();

        private void Setup()
        {
            server = new ServerManager(SERVER_PORT);
            server.PassKeyStr = "PINK";
            server.ClientConnectedEvent += Server_ClientConnectedEvent;
            server.ClientDisconnectedEvent += Server_ClientDisconnectedEvent;
            server.ClientPendingEvent += Server_ClientPendingEvent; ;
            server.Start();
            Log($"Starting server session at {DateTime.Now}");

            try
            {
                Log($"Public ip: {server.PublicIp.Trim()} and port: {SERVER_PORT}");
                //Console.WriteLine($"Local ip: {server.LocalIp} and port: {SERVER_PORT}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Server_ClientPendingEvent(ClientInfo info, byte[] data, ref bool accept)
        {
            Log($"{info.client.RemoteEndPoint} is trying to connect with passCode: '{Bytes.BytesToStr(data)}'.\n" +
                $"Do passwords match? {accept}");
        }

        public async Task RunServer(CancellationToken stoppingToken)
        {
            TimeSpan pingClientsEvery = TimeSpan.FromSeconds(5);
            while (!stoppingToken.IsCancellationRequested && server.Active)
            {
                DateTime timer = DateTime.Now;
                while (DateTime.Now - timer < pingClientsEvery)
                {
                    server.PollEvents();
                    await Task.Delay(100, stoppingToken); //Throws a TaskCanceledException on cancel
                }

                PingClients();
            }
        }

        public void PingClients()
        {
            server.SendToAll(new Ping());

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].DeltaPing > TimeOut)
                {
                    Log($"Hasn't got a ping from {i} for {clients[i].DeltaPing.TotalSeconds} seconds, removing {i}");
                    RemoveClient(clients[i]);
                }
            }
        }

        private void RemoveClient(Client client)
        {
            client.Close();
            clients.Remove(client);
        }

        private void Server_ClientDisconnectedEvent(ClientInfo client)
        {
            Client sender = Client.GetClientByData(client, clients);
            if (sender == null)
                return; //Removed already

            Log($"{sender.Code} has disconnected at {DateTime.Now}");
            RemoveClient(sender);
        }

        private string GetCode(ClientInfo client)
        {
            int rnd = new Random().Next(100, 999);
            return rnd.ToString() + "-" + client.id.ToString();
        }

        private void Server_ClientConnectedEvent(ClientInfo client)
        {
            string code = GetCode(client);
            server.Send(new PassCode(code), client);
            Client newClient = new Client(code, client);
            clients.Add(newClient);

            Log($"{newClient.RemoteEndPoint} has connected and received code: '{code}' at {DateTime.Now}");
        }

        [ServerReceiver]
        public void GetPassCode(PassCode pass, ClientInfo senderData)
        {
            Client sender = Client.GetClientByData(senderData, clients);
            for (int i = 0; i < clients.Count; i++)
            {
                Client client = clients[i];
                if (client.Code == pass.passCode && client != sender && !client.inRoom)
                {
                    client.partner = sender;
                    sender.partner = client;

                    OpenSession(sender, client);
                    return;
                }
            }
            server.Send(new PassCode("Failed to find partner"), sender);
        }

        public void OpenSession(Client controller, Client controlled)
        {
            controller.inRoom = true;
            controlled.inRoom = true;

            IPEndPoint senderEP = GetPeerEndPoint(controller, Mode.Controller);
            IPEndPoint clientEP = GetPeerEndPoint(controlled, Mode.Controlled);

            object senderSession = null;
            object receiverSession = null;

            //Both clients use the same external address (use the same rauter)
            //Unless the rauter supports loopback (most don't) connection will fail
            //Solution: make them connect using their internal address
            if (senderEP.Address.Equals(clientEP.Address))
            {
                Log($"{senderEP} and {clientEP} are from the same address, starting a local session");
                //throw new NotImplementedException("Both clients use the same address");
                senderSession = new LocalSession(Mode.Controller, clientEP.Port); //The sender's mode and his partner's EndPoint
                receiverSession = new LocalSession(Mode.Controlled, senderEP.Port); //The receiver's mode and his partner's EndPoint
            }
            else
            {
                senderSession = new Session(Mode.Controller, clientEP); //The sender's mode and his partner's EndPoint
                receiverSession = new Session(Mode.Controlled, senderEP); //The receiver's mode and his partner's EndPoint
            }

            server.Send(senderSession, controller);
            server.Send(receiverSession, controlled);

            RemoveClient(controller);
            RemoveClient(controlled);

            Log($"Client {controller} has connected to client {controlled}");
        }

        /// <summary>Get EndPoint for peer to peer connection</summary>
        public IPEndPoint GetPeerEndPoint(Client client, Mode mode)
        {
            IPEndPoint senderTcpEp = client.RemoteEndPoint;

            int port = mode == Mode.Controlled ? CONTROLLED_PORT : CONTROLLER_PORT;
            IPAddress senderIP = senderTcpEp.Address;

            return new IPEndPoint(senderIP, port);
        }

        [ServerReceiver]
        public void GetPing(Ping ping, ClientInfo senderData)
        {
            Client client = Client.GetClientByData(senderData, clients);
            client.LastPing = DateTime.Now;
        }

        private void Log(string data, LogLevel level = LogLevel.Information)
        {
            _logger.Log(level, data);
            TgenLog.Log(data);
        }
    }
}
