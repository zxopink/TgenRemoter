using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetTools;

namespace TgenRemoter
{
    using static NetworkMessages;
    public partial class Controlled : FormNetworkBehavour
    {
        ServerManager serverManager;
        public Controlled(ServerManager serverManager)
        {
            InitializeComponent();
            this.serverManager = serverManager;
        }

        private void Controlled_Load(object sender, EventArgs e)
        {
            AllowDrop = true;
            FormClosed += Controlled_FormClosed;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            serverManager.SendToAll(new ConnectionIntializedEvent());
        }

        [ServerNetworkReciver]
        public void ConnectionIntialized(ConnectionIntializedEvent connectionIntialized )
        {
            serverManager.SendToAll(new NetworkPartnerSettings(RemoteSettings.CanSendFiles));
        }

        private void Controlled_FormClosed(object sender, FormClosedEventArgs e)
        {
            serverManager.SendToAll(new PartnerLeft());
            serverManager.Close();
            Application.Exit();
        }

        [ServerNetworkReciver]
        public void OnMouseRecive(RemoteControlMousePos mousePoint)
        {
            /*
            IntPtr desktopPtr = GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);

            SolidBrush b = new SolidBrush(Color.Red);
            g.FillRectangle(b, new Rectangle((int)(mousePoint.xRatio * Screen.PrimaryScreen.Bounds.Width - 23), (int)(mousePoint.yRatio * Screen.PrimaryScreen.Bounds.Height - 80), 10, 10));

            g.Dispose();
            ReleaseDC(IntPtr.Zero, desktopPtr);
            */
            Cursor.Position = new Point((int)(mousePoint.xRatio * Screen.PrimaryScreen.Bounds.Width - 23), (int)(mousePoint.yRatio * Screen.PrimaryScreen.Bounds.Height - 80));
        }

        //[DllImport("User32.dll")]
        //public static extern IntPtr GetDC(IntPtr hwnd);
        //[DllImport("User32.dll")]
        //public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        [ServerNetworkReciver]
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

        [ServerNetworkReciver]
        public void Disconnected(PartnerLeft a)
        {
            serverManager.Close();
            MessageBox.Show("The other side has disconnected", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
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
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            //CompressImage(bitmap, 5);
            serverManager.SendToAll(new RemoteControlFrame(bitmap)); //PAY ATTENTION TO THIS LINE
        }

        bool partnerAllowFiles;
        [ServerNetworkReciver]
        public void GotSettings(NetworkPartnerSettings partnerSettings)
        {
            partnerAllowFiles = partnerSettings.AllowFiles;
        }

        [ServerNetworkReciver]
        public void GotNetworkFiles(NetworkFile file)
        {
            if(RemoteSettings.CanSendFiles)
                Tools.UnpackFile(RemoteSettings.FolderPath, file);
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
            foreach (string file in files) serverManager.SendToAll(Tools.PackFile(file));
        }
    }
}
