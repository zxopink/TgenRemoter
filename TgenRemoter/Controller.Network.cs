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
using System.Drawing;

namespace TgenRemoter
{
    //The 'Designer.cs' file name is to not open the file in Form Designer mode
    public partial class Controller
    {
        Image lastFrame;
        [DgramReceiver]
        public void OnScreenFrameRecive(RemoteControlFrame Frame)
        {
            //TODO: Dispose Frame once done
            lastFrame = ScreenSharePictureBox.BackgroundImage;
            ScreenSharePictureBox.BackgroundImage = Frame;
            lastFrame?.Dispose();
        }

        [DgramReceiver]
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

        public void ConnectionIntialized()
        {
            //Send again if the first call was sent too early
            //ClientManager.Send(new ConnectionIntializedEvent()); //So send again, maximum it will be ignored
            Partner.SendToAll(new NetworkPartnerSettings(RemoteSettings.CanSendFiles), DeliveryMethod.ReliableUnordered);

            //Must be set true IN A THREAD SAFE Environment(Not Network events), otherwise the ticking won't be enabled
            Invoke(new Action(() => { Tick.Enabled = true; }));  //Tick.Enabled = true; //Start control
        }
    }
}
