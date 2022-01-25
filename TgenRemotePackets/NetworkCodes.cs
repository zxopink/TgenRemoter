using System;
using System.Net;
using System.Net.Sockets;

namespace TgenRemoteCodes
{
    /// <summary>
    /// Packets exchange between the server (.Net 5 (core) and .Net Framework 4.5) by using .Net 2 (standard )
    /// This Library lets the two communicate using the same types but in different frameworks
    /// </summary>
    public class NetworkCodes
    {
        [Serializable]
        public class PassCode
        {
            public string passCode;

            public PassCode(string passCode)
            {
                this.passCode = passCode;
            }
        }

        public enum Mode : byte
        {
            Controlled,
            Controller
        }

        [Serializable]
        public class Session
        {
            Mode mode;
            NetworkEndPoint partnerEP;

            public Mode Mode { get => mode; }
            public NetworkEndPoint PartnerEP { get => partnerEP; }

            public Session(Mode setMode, NetworkEndPoint ep)
            {
                mode = setMode;
                partnerEP = ep;
            }
        }

        [Serializable]
        public class LocalSession
        {
            Mode mode;
            int partnerPort;

            public Mode Mode { get => mode; }
            public int PartnerPort { get => partnerPort; }

            public LocalSession(Mode setMode, int port)
            {
                mode = setMode;
                partnerPort = port;
            }
        }

        [Serializable]
        public class Ping
        {
            //Ping client, if he doesn't reply within X seconds, kick the client
        }

        [Serializable]
        public class Req
        {
            //Get premission from controlled (Checkbox) before opening a session
            public enum Result : byte
            {
                Succeeded,
                Denied //Controlled side denied the request, use this to inform the controller side
            }

            public Result result;
        }

        [Serializable]
        public class NetworkEndPoint
        {
            public byte[] address;
            public int port;

            public NetworkEndPoint(IPEndPoint endPoint)
            {
                address = endPoint.Address.GetAddressBytes();
                port = endPoint.Port;
            }

            /// <summary>Get partner endpoint</summary>
            public IPEndPoint GetEP()
            {
                if (address == null)
                    return null;

                IPAddress ipAddress = new IPAddress(address);
                return new IPEndPoint(ipAddress, port);
            }

            public static implicit operator NetworkEndPoint(IPEndPoint endPoint) => new NetworkEndPoint(endPoint);
            public static implicit operator NetworkEndPoint(EndPoint endPoint) => new NetworkEndPoint((IPEndPoint)endPoint);
            public static implicit operator IPEndPoint(NetworkEndPoint endPoint) => endPoint.GetEP();
        }

        [Serializable]
        public class StartEvent
        {
            public string message;
            public StartEvent(string message)
            {
                this.message = message;
            }
        }
    }
}
