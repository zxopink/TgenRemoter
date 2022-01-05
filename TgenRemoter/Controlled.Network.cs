using System.Diagnostics;
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
        [DgramReceiver]
        public void ConnectionIntialized(TempUDPConnectAttempt attempt)
        {
            if (Initialized)
                return;
            Initialized = true;

            //Send again if the first call was sent too early
            //ClientManager.Send(new ConnectionIntializedEvent()); //So send again, maximum it will be ignored
            Partner.SendToAll(new NetworkPartnerSettings(RemoteSettings.CanSendFiles), LiteNetLib.DeliveryMethod.ReliableUnordered);
            //Must be set true IN A THREAD SAFE Environment(Not Network events), otherwise the ticking won't be enabled
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
        public void OnMousePress(RemoteControlMousePress mousePress) =>
            mousePress.SignMouse();

        [ClientReceiver]
        public void Disconnected(PartnerLeft a) => System.Console.WriteLine("Left tcp channel");

        bool partnerAllowFiles;
        [DgramReceiver]
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
