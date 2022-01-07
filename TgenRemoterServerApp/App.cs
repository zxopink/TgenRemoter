using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TgenNetProtocol;
using TgenRemoter;

namespace TgenRemoterServer
{
    using static NetworkCodes;
    using static NetworkMessages;
    class App : NetworkBehavour
    {
        public class Client
        {
            public TgenNetProtocol.Client Socket { get; private set; }
            private string code;
            public IPEndPoint udpEndPoint { get; set; }
            public string Code { get => code; }
            public bool ready; //Is handle ready
            public bool inRoom; //Is in room with another client
            ClientInfo ClientData;
            public Client partner;
            public Client(string code, ClientInfo data)
            {
                this.code = code;
                ClientData = data;
                Socket = data.client;
                inRoom = false;
                udpEndPoint = null;
            }

            public static implicit operator ClientInfo(Client client) => client.ClientData;
            public override string ToString()
            {
                return ClientData.ToString();
            }

            public IPEndPoint GetIP()
            {
                IPEndPoint endPoint = Socket;
                return new IPEndPoint(endPoint.Address, 7778);
            }

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
        public App()
        {
            server = new ServerManager(7777);
            server.ClientConnectedEvent += Server_ClientConnectedEvent;
            server.ClientDisconnectedEvent += Server_ClientDisconnectedEvent;
            server.Start();

            Console.WriteLine($"Local ip: {server.LocalIp} and port: {7777}");
        }

        private void Server_ClientDisconnectedEvent(ClientInfo client)
        {
            Client sender = Client.GetClientByData(client, clients);
            Console.Write(sender.Code + " has disconnected");
            if (sender.partner != null)
            {
                server.Send(new PartnerLeft(), sender.partner);
                Console.WriteLine("and left client: " + sender.partner.Code);
            }
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

                    client.udpEndPoint.Port++; //DEBUGGING SAKE ONLY

                    var senderSession = new OpenSession(Mode.Controller, client.udpEndPoint);
                    var receiverSession = new OpenSession(Mode.Controlled, sender.udpEndPoint);

                    server.Send(senderSession, sender);
                    server.Send(receiverSession, client);

                    //Wait for 'ConnectionIntializedEvent' (for the forms to be created)
                    client.inRoom = true;
                    sender.inRoom = true;

                    Console.WriteLine($"Client {sender} has connected to client {client}");
                    return;
                }
            }
            server.Send(new PassCode("Failed to find partner"), sender);
        }

        [ServerReceiver]
        public void GetEndPoint(NetworkEndPoint ep, ClientInfo senderData)
        {
            Client sender = Client.GetClientByData(senderData, clients);

            IPEndPoint senderTcpEp = sender.Socket.RemoteEndPoint as IPEndPoint;
            IPAddress senderIP = senderTcpEp.Address.MapToIPv4(); //For now, we're using IPv4
            IPEndPoint senderEP = new IPEndPoint(senderIP, ep.port);
            sender.udpEndPoint = senderEP;
        }
    }
}
