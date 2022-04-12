using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using TgenNetProtocol;
using TgenRemoteCodes;
using TgenSerializer;

namespace TgenRemoteServer
{
    using static NetworkCodes;
    class App : NetworkBehavour
    {
        //Compile command: dotnet build --configuration Release --runtime ubuntu.20.04-x64
        //publish project command: dotnet publish -c release -r ubuntu.20.04-x64 --self-contained
        public class Client
        {
            public ClientInfo info { get; private set; }
            private string code;
            //public IPEndPoint udpEndPoint { get; set; }
            public string Code { get => code; }
            public bool inRoom; //Is in room with another client
            ClientInfo ClientData;
            public Client partner;

            public DateTime LastPing { get; set; }
            public TimeSpan DeltaPing => DateTime.Now - LastPing;

            public IPEndPoint RemoteEndPoint { get {
                    IPEndPoint ep = (IPEndPoint)info.client.RemoteEndPoint;

                    IPAddress ip = ep.Address;

                    //False Ipv6 (Ipv4 that was mapped to Ipv6 starts with this string, example: (::ffff:127.0.0.1)
                    const string falseIpv6 = "::ffff:";
                    bool isIpv4 = ip.ToString().Substring(0, falseIpv6.Length) == falseIpv6;
                    if (isIpv4)
                        ep = new IPEndPoint(ip.MapToIPv4(), ep.Port);

                    return ep;
                } }

            public Client(string code, ClientInfo data)
            {
                this.code = code;
                ClientData = data;
                info = data;
                inRoom = false;
                LastPing = DateTime.Now;
            }

            public static implicit operator ClientInfo(Client client) => client.ClientData;
            public override string ToString()
            {
                return ClientData.ToString();
            }

            public void Close() =>
                info.client.Close();

            public static Client GetClientByData(ClientInfo data, List<Client> clients)
            {
                foreach (var client in clients)
                {
                    if (data.id == client.ClientData.id)
                        return client;
                }
                return null;
            }
        }
        List<Client> clients = new List<Client>();
        ServerManager server;

        const int CONTROLLER_PORT = 7788;
        const int CONTROLLED_PORT = 7799;
        readonly TimeSpan TimeOut = TimeSpan.FromSeconds(25);
        public App()
        {
            server = new ServerManager(7777);
            server.PassKeyStr = "PINK";
            server.ClientConnectedEvent += Server_ClientConnectedEvent;
            server.ClientDisconnectedEvent += Server_ClientDisconnectedEvent;
            server.ClientPendingEvent += Server_ClientPendingEvent; ;
            server.Start();
            TgenLog.Log($"Starting server session at {DateTime.Now}");

            try
            {
                Console.WriteLine($"Public ip: {server.PublicIp} and port: {7777}");
                //Console.WriteLine($"Local ip: {server.LocalIp} and port: {7777}");
            }
            catch (Exception)
            {

                throw;
            }

            RunServer();
        }

        private void Server_ClientPendingEvent(ClientInfo info, byte[] data, ref bool accept)
        {
            TgenLog.Log($"{info.client.RemoteEndPoint} is trying to connect with passCode: '{Bytes.BytesToStr(data)}'.\n" +
                $"Do passwords match? {accept}");
        }

        public void RunServer()
        {
            TimeSpan pingClientsEvery = TimeSpan.FromSeconds(5);
            while (server.Active)
            {
                DateTime timer = DateTime.Now;
                while (DateTime.Now - timer < pingClientsEvery)
                {
                    server.PollEvents();
                    Thread.Sleep(100);
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
                    TgenLog.Log($"Hasn't got a ping from {i} for {clients[i].DeltaPing.TotalSeconds} seconds, removing {i}");
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

            TgenLog.Log($"{sender.Code} has disconnected at {DateTime.Now}");
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

            TgenLog.Log($"{newClient.RemoteEndPoint} has connected and received code: '{code}' at {DateTime.Now}");
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
                Console.WriteLine($"{senderEP} and {clientEP} are from the same address, starting a local session");
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

            Console.WriteLine($"Client {controller} has connected to client {controlled}");
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
    }
}
