using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetTools;

namespace TgenRemoter
{
    using static NetworkMessages;
    public partial class Controlled : FormNetworkBehavour
    {
        ClientManager clientManager;
        public Controlled(ClientManager clientManager)
        {
            InitializeComponent();
            this.clientManager = clientManager;
        }

        private void Controlled_Load(object sender, EventArgs e)
        {
            AllowDrop = true;
            FormClosed += Controlled_FormClosed;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            clientManager.Send(new ConnectionIntializedEvent());
        }

        [ClientNetworkReciver]
        public void ConnectionIntialized(ConnectionIntializedEvent connectionIntialized)
        {
            clientManager.Send(new NetworkPartnerSettings(RemoteSettings.CanSendFiles));
            Tick.Enabled = true; //Start sharing screen
        }

        private void Controlled_FormClosed(object sender, FormClosedEventArgs e)
        {
            //clientManager.Send(new PartnerLeft()); //Server handles that
            clientManager.Close();
            Application.Exit();
        }

        [ClientNetworkReciver]
        public void OnMouseRecive(RemoteControlMousePos mousePoint)
        {
            Cursor.Position = new Point((int)(mousePoint.xRatio * Screen.PrimaryScreen.Bounds.Width), (int)(mousePoint.yRatio * Screen.PrimaryScreen.Bounds.Height));
        }

        [ClientNetworkReciver]
        public void OnKeyboardRecive(RemoteControlKeyboard keyboardInput) => keyboardInput.SignKey();

        /*
        [ClientNetworkReciver]
        public void OnMousePress(RemoteControlMousePress mousePress)
        {
            switch (mousePress.pressType)
            {
                case RemoteControlMousePress.PressType.leftClick:
                    DoLeftMouseClick();
                    break;
                case RemoteControlMousePress.PressType.middleClick:
                    break;
                case RemoteControlMousePress.PressType.rightClick:
                    DoRightMouseClick();
                    break;
                default:
                    break;
            }
        }
        */
        [ClientNetworkReciver]
        public void OnMousePress(RemoteControlMousePress mousePress) => mousePress.SignMouse();

        [ClientNetworkReciver]
        public void Disconnected(PartnerLeft a)
        {
            clientManager.Close();
            MessageBox.Show("The other side has disconnected", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            Application.Exit();
        }

        #region Mouse Press Event
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        public void DoLeftMouseClick()
        {
            //Call the imported function with the cursor's current position
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
        public void DoRightMouseClick()
        {
            //Call the imported function with the cursor's current position
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }
        #endregion

        private void Tick_Tick(object sender, EventArgs e)
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            clientManager.Send(new RemoteControlFrame(bitmap), true);
        }

        bool partnerAllowFiles;
        [ClientNetworkReciver]
        public void GotSettings(NetworkPartnerSettings partnerSettings)
        {
            partnerAllowFiles = partnerSettings.AllowFiles;
        }

        [ClientNetworkReciver]
        public void GotNetworkFiles(NetworkFile file)
        {
            if (!RemoteSettings.CanSendFiles) return;

            Tools.UnpackFile(RemoteSettings.FolderPath, file);
            FormTaskbarFlash.FlashWindowEx(this);
        }

        private void Controlled_DragEnter(object sender, DragEventArgs e)
        {
            if (!partnerAllowFiles)
            {
                e.Effect = DragDropEffects.Scroll; //None effect blocks the Controlled_DragDrop event
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Controlled_DragDrop(object sender, DragEventArgs e)
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
