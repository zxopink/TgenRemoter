using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TgenNetProtocol;

namespace TgenRemoteServer
{
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

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                IPEndPoint ep = (IPEndPoint)info.client.RemoteEndPoint;

                IPAddress ip = ep.Address;

                //False Ipv6 (Ipv4 that was mapped to Ipv6 starts with this string, example: (::ffff:127.0.0.1)
                const string falseIpv6 = "::ffff:";
                bool isIpv4 = ip.ToString().Substring(0, falseIpv6.Length) == falseIpv6;
                if (isIpv4)
                    ep = new IPEndPoint(ip.MapToIPv4(), ep.Port);

                return ep;
            }
        }

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
}
