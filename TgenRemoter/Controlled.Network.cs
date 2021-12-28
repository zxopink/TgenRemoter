using System.Drawing;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetTools;
using static TgenRemoter.NetworkMessages;

namespace TgenRemoter
{
    //The 'Designer.cs' file name is to not open the file in Form Designer mode
    partial class Controlled
    {
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
            Tick.Enabled = true; //Start sharing screen
        }

        [DgramReceiver]
        public void OnMouseRecive(RemoteControlMousePos mousePoint)
        {
            Cursor.Position = new Point((int)(mousePoint.xRatio * Screen.PrimaryScreen.Bounds.Width), (int)(mousePoint.yRatio * Screen.PrimaryScreen.Bounds.Height));
        }

        [DgramReceiver]
        public void OnKeyboardRecive(RemoteControlKeyboard keyboardInput) => keyboardInput.SignKey();

        [DgramReceiver]
        public void OnMousePress(RemoteControlMousePress mousePress) => mousePress.SignMouse();

        [ClientReceiver]
        public void Disconnected(PartnerLeft a) => CloseWindow();

        bool partnerAllowFiles;
        [ClientReceiver]
        public void GotSettings(NetworkPartnerSettings partnerSettings)
        {
            partnerAllowFiles = partnerSettings.AllowFiles;
        }

        //TODO: figure out what to do here
        [ClientReceiver]
        public void GotNetworkFiles(NetworkFile file)
        {
            if (!RemoteSettings.CanSendFiles) return;

            Tools.UnpackFile(RemoteSettings.FolderPath, file);
            FormTaskbarFlash.FlashWindowEx(this);
        }
    }
}
