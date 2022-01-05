using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetTools;
using LiteNetLib;
using static TgenRemoter.NetworkMessages;

namespace TgenRemoter
{
    //The 'Designer.cs' file name is to not open the file in Form Designer mode
    public partial class Controller
    {
        [ClientReceiver]
        public void Disconnected(PartnerLeft a) =>
            Console.WriteLine("Left tcp channel");

        [DgramReceiver]
        public void OnScreenFrameRecive(RemoteControlFrame Frame)
        {
            //TODO: Dispose Frame once done
            ScreenSharePictureBox.BackgroundImage = Frame;
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

        /// <summary>True if already received ConnectionIntializedEvent once</summary>
        bool Initialized = false;
        [DgramReceiver]
        public void ConnectionIntialized(TempUDPConnectAttempt attempt)
        {
            if (Initialized)
                return;
            Initialized = true;

            //Send again if the first call was sent too early
            //ClientManager.Send(new ConnectionIntializedEvent()); //So send again, maximum it will be ignored
            Partner.SendToAll(new NetworkPartnerSettings(RemoteSettings.CanSendFiles), DeliveryMethod.ReliableUnordered);
            //Must be set true IN A THREAD SAFE Environment(Not Network events), otherwise the ticking won't be enabled
            Tick.Enabled = true; //Start control
        }
    }
}
