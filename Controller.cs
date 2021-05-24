using System;
using System.Drawing;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetTools;

namespace TgenRemoter
{
    using static NetworkMessages;
    public partial class Controller : FormNetworkBehavour
    {
        ClientManager clientManager;
        public Controller(ClientManager clientManager)
        {
            InitializeComponent();
            this.clientManager = clientManager;
        }

        private void Controller_Load(object sender, EventArgs e)
        {
            FormClosed += Controller_FormClosed;
            AllowDrop = true;
            Tick.Enabled = true;

            ScreenSharePictureBox.Location = Point.Empty;
            ScreenSharePictureBox.Width = ClientRectangle.Width;
            ScreenSharePictureBox.Height = ClientRectangle.Height;
            SizeChanged += Controller_SizeChanged;
            clientManager.Send(new ConnectionIntializedEvent());
        }

        private void ScreenSharePictureBox_MouseClick(object sender, MouseEventArgs e) {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    clientManager.Send(new RemoteControlMousePress(RemoteControlMousePress.PressType.leftClick));
                    break;
                case MouseButtons.Middle:
                    clientManager.Send(new RemoteControlMousePress(RemoteControlMousePress.PressType.middleClick));
                    break;
                case MouseButtons.Right:
                    clientManager.Send(new RemoteControlMousePress(RemoteControlMousePress.PressType.rightClick));
                    break;
                case MouseButtons.None:
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    break;
            }
        }

        private void Controller_KeyDown(object sender, KeyEventArgs e)
        {
            if (!RectangleToScreen(ClientRectangle).Contains(Cursor.Position)) //detects if the controller is in the monitor window
                return;

            RemoteControlKeyboard input = e.KeyCode.ToString();
            clientManager.Send(input);
        }

        private void Controller_SizeChanged(object sender, EventArgs e)
        {
            ScreenSharePictureBox.Width = ClientRectangle.Width; //to fit the ratio
            ScreenSharePictureBox.Height = ClientRectangle.Height; //to fit the ratio
        }

        private void Controller_FormClosed(object sender, FormClosedEventArgs e)
        {
            clientManager.Send(new PartnerLeft());
            clientManager.Close();
            Application.Exit();
        }

        private void Tick_Tick(object sender, EventArgs e)
        {
            if (!RectangleToScreen(ClientRectangle).Contains(Cursor.Position)) //detects if the controller is in the monitor window
                return;

            double xPos = (Cursor.Position.X - Location.X) / (double)ClientRectangle.Width;
            double yPos = (Cursor.Position.Y - Location.Y) / (double)ClientRectangle.Height;

            RemoteControlMousePos mousePos = new RemoteControlMousePos(xPos, yPos);

            clientManager.Send(mousePos);
        }

        [ClientNetworkReciver]
        public void Disconnected(PartnerLeft a)
        {
            clientManager.Close();
            MessageBox.Show("The other side has disconnected", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            Application.Exit();
        }

        [ClientNetworkReciver]
        public void OnScreenFrameRecive(RemoteControlFrame Frame)
        {
            ScreenSharePictureBox.BackgroundImage = Frame;
        }

        [ClientNetworkReciver]
        public void GotNetworkFile(NetworkFile file)
        {
            if (!RemoteSettings.CanSendFiles) return;

            Tools.UnpackFile(RemoteSettings.FolderPath, file);
            FormTaskbarFlash.FlashWindowEx(this);
        }

        bool partnerAllowFiles;
        [ClientNetworkReciver]
        public void GotSettings(NetworkPartnerSettings partnerSettings)
        {
            Console.WriteLine("Can send files? " + partnerSettings.AllowFiles);
            partnerAllowFiles = partnerSettings.AllowFiles;
        }
        [ClientNetworkReciver]
        public void GotSettings(ConnectionIntializedEvent connectionIntialized)
        {
            clientManager.Send(new NetworkPartnerSettings(RemoteSettings.CanSendFiles));
        }

        private void Controller_DragEnter(object sender, DragEventArgs e)
        {
            if (!partnerAllowFiles)
            {
                e.Effect = DragDropEffects.Scroll; //None effect blocks the Controller_DragDrop event
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Controller_DragDrop(object sender, DragEventArgs e)
        {
            if (!partnerAllowFiles)
            {
                MessageBox.Show("Partner doesn't allow for files transformation", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) clientManager.Send(Tools.PackFile(file));
        }
    }
}
