using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetTools;
using static TgenRemoter.NetworkMessages;

namespace TgenRemoter
{
    //The 'Designer.cs' file name is to not open the file in Form Designer mode
    public partial class Controller
    {
        [ClientReceiver]
        public void Disconnected(PartnerLeft a) =>
            CloseWindow();

        [DgramReceiver]
        public void OnScreenFrameRecive(RemoteControlFrame Frame)
        {
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
        [ClientReceiver]
        public void GotSettings(NetworkPartnerSettings partnerSettings)
        {
            Console.WriteLine("Can send files? " + partnerSettings.AllowFiles);
            partnerAllowFiles = partnerSettings.AllowFiles;
        }

        /// <summary>True if already received ConnectionIntializedEvent once</summary>
        bool Initialized = false;
        [ClientReceiver]
        public void ConnectionIntialized(ConnectionIntializedEvent connectionIntialized)
        {
            if (Initialized)
                return;

            Initialized = true;
            Partner.Connect(connectionIntialized.partnerEP);
            //Send again if the first call was sent too early
            //ClientManager.Send(new ConnectionIntializedEvent()); //So send again, maximum it will be ignored
            ClientManager.Send(new NetworkPartnerSettings(RemoteSettings.CanSendFiles));
            Tick.Enabled = true; //Start control
        }
    }
}
