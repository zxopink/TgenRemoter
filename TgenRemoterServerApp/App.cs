using System;
using System.Collections.Generic;
using System.Text;
using TgenNetProtocol;
using TgenRemoter;

namespace TgenRemoterServer
{
    class App : NetworkBehavour
    {
        public class Client
        {
            private string code;
            public string Code { get => code; }
            public bool ready; //Is handle ready
            public bool inRoom; //Is in room with another client
            ClientData ClientData;
            public Client partner;
            public Client(string code, ClientData data)
            {
                this.code = code;
                ClientData = data;
                inRoom = false;
            }

            public static implicit operator ClientData(Client client) => client.ClientData;
            public override string ToString()
            {
                return ClientData.ToString();
            }

            public static Client GetClientByData(ClientData data, List<Client> clients)
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
            server.Start();
            server.ClientConnectedEvent += Server_ClientConnectedEvent;
            server.ClientDisconnectedEvent += Server_ClientDisconnectedEvent;

            Console.WriteLine($"Local ip: {server.LocalIp} and port: {7777}");
        }

        private void Server_ClientDisconnectedEvent(ClientData client)
        {
            Client sender = Client.GetClientByData(client, clients);
            Console.Write(sender.Code + " has disconnected");
            if (sender.partner != null)
            {
                server.Send(new NetworkMessages.PartnerLeft(), sender.partner);
                Console.WriteLine("and left client: " + sender.partner.Code);
            }
        }

        private string GetCode(ClientData client)
        {
            int rnd = new Random().Next(100, 999);
            return rnd.ToString() + "-" + client.id.ToString();
        }

        private void Server_ClientConnectedEvent(ClientData client)
        {
            string code = GetCode(client);
            server.Send(new NetworkCodes.PassCode(code), client);
            Client newClient = new Client(code, client);
            clients.Add(newClient);
        }

        [ServerReceiver]
        public void GetPassCode(NetworkCodes.PassCode pass, ClientData senderData)
        {
            Client sender = Client.GetClientByData(senderData, clients);
            for (int i = 0; i < clients.Count; i++)
            {
                Client client = clients[i];
                if (client.Code == pass.passCode && client != sender && !client.inRoom)
                {
                    client.partner = sender;
                    sender.partner = client;

                    server.Send(new NetworkCodes.PassCode("SuccessController"), sender);
                    server.Send(new NetworkCodes.PassCode("SuccessControlled"), client);

                    //Wait for 'ConnectionIntializedEvent' (for the forms to be created)
                    //client.inRoom = true;
                    //sender.inRoom = true;

                    Console.WriteLine($"Client {sender} has connected to client {client}");
                    return;
                }
            }
            server.Send(new NetworkCodes.PassCode("Failed to find partner"), sender);
        }

        [ServerReceiver]
        public void GetPassCode(NetworkMessages.ConnectionIntializedEvent obj, ClientData senderData)
        {
            Client sender = Client.GetClientByData(senderData, clients);
            sender.ready = true;

            Client partner = sender.partner;

            if (sender.partner.ready)
            {
                sender.inRoom = true;
                partner.inRoom = true;
            }

            server.Send(new NetworkMessages.ConnectionIntializedEvent(), sender);
            server.Send(new NetworkMessages.ConnectionIntializedEvent(), partner);
        }

        [ServerReceiver]
        public void GetPassCode(object obj, ClientData senderData)
        {
            Client sender = Client.GetClientByData(senderData, clients);
            if (!sender.inRoom) return;

            server.Send(obj, sender.partner);
        }
    }
}
