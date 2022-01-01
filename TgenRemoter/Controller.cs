using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetProtocol.WinForms;
using TgenNetTools;
using LiteNetLib;

namespace TgenRemoter
{
    using static NetworkMessages;
    [System.ComponentModel.DesignerCategory("")] //To not view the designer on open
    public partial class Controller : FormNetworkBehavour
    {
        ClientManager ClientManager { get; set; }
        UdpManager Partner { get; set; }
        public Controller(ClientManager clientManager, UdpManager partner)
        {
            
            InitializeComponent();
            ClientManager = clientManager;
            ClientManager.OnDisconnect += CloseWindow;
            Partner = partner;
            partner.Listen();
        }

        private void Controller_Load(object sender, EventArgs e)
        {
            FormClosed += Controller_FormClosed;
            AllowDrop = true;

            ScreenSharePictureBox.Location = Point.Empty;
            ScreenSharePictureBox.Width = ClientRectangle.Width;
            ScreenSharePictureBox.Height = ClientRectangle.Height;

            ScreenSharePictureBox.MouseWheel += ScreenSharePictureBox_MouseWheel;

            SizeChanged += Controller_SizeChanged;
            ClientManager.Send(new ConnectionIntializedEvent(Partner.LocalEP));
        }

        #region CursorController
        private void ScreenSharePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Partner.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.LeftDown), DeliveryMethod.ReliableOrdered);
                    break;
                case MouseButtons.Middle:
                    Partner.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.MiddleDown), DeliveryMethod.ReliableOrdered);
                    break;
                case MouseButtons.Right:
                    Partner.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.RightDown), DeliveryMethod.ReliableOrdered);
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
                    Partner.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.LeftUp), DeliveryMethod.ReliableOrdered);
                    break;
                case MouseButtons.Middle:
                    Partner.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.MiddleUp), DeliveryMethod.ReliableOrdered);
                    break;
                case MouseButtons.Right:
                    Partner.Send(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.RightUp), DeliveryMethod.ReliableOrdered);
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

        private void ScreenSharePictureBox_MouseWheel(object sender, MouseEventArgs e) => Partner.Send(new RemoteControlMousePress(e.Delta), DeliveryMethod.ReliableUnordered);

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

            Partner.Send(mousePos, DeliveryMethod.Unreliable);
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

            Partner.Send(input, DeliveryMethod.ReliableOrdered);
        }
        #endregion

        public void CloseWindow()
        {
            ClientManager.Close();
            Partner.Close();
            MessageBox.Show("The other side has disconnected", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Application.Exit();
        }

        private void Controller_SizeChanged(object sender, EventArgs e)
        {
            ScreenSharePictureBox.Width = ClientRectangle.Width; //to fit the ratio
            ScreenSharePictureBox.Height = ClientRectangle.Height; //to fit the ratio
        }

        private void Controller_FormClosed(object sender, FormClosedEventArgs e)
        {
            //clientManager.Send(new PartnerLeft()); //Server handles that
            ClientManager.Close();
            Partner.Close();
            Application.Exit();
        }

        private void Tick_Tick(object sender, EventArgs e)
        {
            ShareMousePos();
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
            foreach (string file in files) ClientManager.Send(Tools.PackFile(file));
        }
    }
}
