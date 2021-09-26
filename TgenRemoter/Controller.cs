using System;
using System.Drawing;
using System.Runtime.InteropServices;
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

            ScreenSharePictureBox.MouseWheel += ScreenSharePictureBox_MouseWheel;

            SizeChanged += Controller_SizeChanged;
            clientManager.Send(new ConnectionIntializedEvent());
        }

        #region CursorController
        private void ScreenSharePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    clientManager.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.LeftDown));
                    break;
                case MouseButtons.Middle:
                    clientManager.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.MiddleDown));
                    break;
                case MouseButtons.Right:
                    clientManager.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.RightDown));
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

        private void ScreenSharePictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    clientManager.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.LeftUp));
                    break;
                case MouseButtons.Middle:
                    clientManager.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.MiddleUp));
                    break;
                case MouseButtons.Right:
                    clientManager.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.RightUp));
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

        private void ScreenSharePictureBox_MouseWheel(object sender, MouseEventArgs e) => clientManager.Send(new RemoteControlMousePress(e.Delta));

        /// <summary>
        /// Called every tick
        /// </summary>
        private void ShareMousePos()
        {
            if (!RectangleToScreen(ClientRectangle).Contains(Cursor.Position)) //detects if the controller is in the monitor window
                return;

            Point relativePoint = PointToClient(Cursor.Position);

            double xPos = (relativePoint.X - ScreenSharePictureBox.Location.X) / (double)ScreenSharePictureBox.Width; //ClientRectangle
            double yPos = (relativePoint.Y - ScreenSharePictureBox.Location.Y) / (double)ScreenSharePictureBox.Height;

            RemoteControlMousePos mousePos = new RemoteControlMousePos(xPos, yPos);

            clientManager.Send(mousePos);
        }

        private void Controller_KeyDown(object sender, KeyEventArgs e)
        {
            if (!RectangleToScreen(ClientRectangle).Contains(Cursor.Position)) //detects if the controller is in the monitor window
                return;

            string key = e.KeyCode.ToString();
            if (!IsKeyLocked(Keys.CapsLock))
                key = key.ToLower();

            RemoteControlKeyboard input = key;
            input.ctrl = e.Control;
            input.alt = e.Alt;
            input.shift = e.Shift;

            clientManager.Send(input);
        }
    #endregion

    private void Controller_SizeChanged(object sender, EventArgs e)
        {
            ScreenSharePictureBox.Width = ClientRectangle.Width; //to fit the ratio
            ScreenSharePictureBox.Height = ClientRectangle.Height; //to fit the ratio
        }

        private void Controller_FormClosed(object sender, FormClosedEventArgs e)
        {
            //clientManager.Send(new PartnerLeft()); //Server handles that
            clientManager.Close();
            Application.Exit();
        }

        private void Tick_Tick(object sender, EventArgs e)
        {
            ShareMousePos();
        }

        [ClientReceiver]
        public void Disconnected(PartnerLeft a)
        {
            clientManager.Close();
            MessageBox.Show("The other side has disconnected", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            Application.Exit();
        }

        [ClientReceiver]
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
        [ClientReceiver]
        public void ConnectionIntialized(ConnectionIntializedEvent connectionIntialized)
        {
            clientManager.Send(new NetworkPartnerSettings(RemoteSettings.CanSendFiles));
            Tick.Enabled = true; //Start control
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
