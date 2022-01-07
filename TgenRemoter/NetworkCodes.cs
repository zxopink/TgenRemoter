using System;
using System.Net;
using System.Net.Sockets;

namespace TgenRemoter
{
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

        [Serializable]
        public class OpenSession
        {
            enum Mode : byte
            {
                Controlled,
                Controller
            }
            Mode mode;
            NetworkEndPoint partnerEP;

            public OpenSession(Mode mode, NetworkEndPoint ep)
            {
                this.mode = mode;
                this.partnerEP = ep;
            }
        }

        [Serializable]
        public class NetworkEndPoint
        {
            public byte[] address;
            public int port;
            AddressFamily family;
            
            public NetworkEndPoint(IPEndPoint endPoint)
            {
                address = endPoint.Address.GetAddressBytes();
                port = endPoint.Port;
                family = endPoint.AddressFamily;
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
