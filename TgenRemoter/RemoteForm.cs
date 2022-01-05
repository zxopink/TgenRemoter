using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgenNetProtocol;
using TgenNetTools;
using TgenNetProtocol.WinForms;
using static TgenRemoter.NetworkMessages;
using LiteNetLib;

namespace TgenRemoter
{
    //Remote base form
    public abstract class RemoteForm : FormNetworkBehavour
    {
        public UdpManager Peer { get; private set; }
        public RemoteForm(UdpManager udpConnection)
        {
            Peer = udpConnection;
        }

        [ClientReceiver]
        public void GotNetworkFile(NetworkFile file)
        {
            if (!RemoteSettings.CanSendFiles) return;

            Tools.UnpackFile(RemoteSettings.FolderPath, file);
            FormTaskbarFlash.FlashWindowEx(this);
        }

        bool partnerAllowFiles;
        [DgramReceiver]
        public void GotSettings(NetworkPartnerSettings partnerSettings)
        {
            Console.WriteLine("Can send files? " + partnerSettings.AllowFiles);
            partnerAllowFiles = partnerSettings.AllowFiles;
        }

        /// <summary>Start Session</summary>
        private void StartTicking()
        { 
        
        }
    }
}
