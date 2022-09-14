using LiteNetLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetProtocol.WinForms;
using TgenNetTools;

namespace TgenRemoter
{
    using static NetworkMessages;
    [System.ComponentModel.DesignerCategory("")] //To not view the designer on open
    public partial class Controlled : FormNetworkBehavour
    {
        UdpManager Partner { get; set; }
        const int CONTROLLED_PORT = 7799;
        public Controlled(IPEndPoint partnerEP)
        {
            Partner = new UdpManager(CONTROLLED_PORT);
            Partner.NatPunchEnabled = true;
            //Register events
            TgenLog.Log("controller connecting to: " + partnerEP + " and my endPoint: " + Partner.LocalEP);
            Partner.NetworkErrorEvent += (peer, error) => { TgenLog.Log("Network Error: " + error); };
            Partner.NetworkLatencyUpdateEvent += (peer, latency) => { TgenLog.Log("New latency: " + latency); };
            Partner.PeerDisconnectedEvent += (ep, info) => { var reason = "Disconnect reason: " + info.Reason; CloseWindow(true, reason); TgenLog.Log(reason); };
            Partner.PeerConnectedEvent += (ep) => { ConnectionIntialized(); };
            Partner.StartThread();

            var timer = new Timer();
            timer.Interval = Partner.DisconnectTimeout;
            timer.Start();

            timer.Tick += (sender, args) => { if (Tick.Enabled) return; TgenLog.Log("Partner failed to connect, aborting"); timer.Stop(); timer.Dispose(); CloseWindow(true); };
            //The controller will attempt connection, just wait for the connection

            InitializeComponent();
        }

        private void Controlled_Load(object sender, EventArgs e)
        {
            AllowDrop = true;
            FormClosed += Controlled_FormClosed;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void Controlled_FormClosed(object sender, FormClosedEventArgs e) =>
            CloseWindow(false);

        public void CloseWindow(bool partnerDisconnected, string reason = null)
        {
            Partner.Close();
            if (partnerDisconnected)
                MessageBox.Show(reason ?? "The other side has disconnected", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            Bitmap bitmap = GetScreenBitmap();
            if (bitmap == null)
                return;

            Partner.SendToAll(new RemoteControlFrame(bitmap), DeliveryMethod.ReliableUnordered); //Unreliable can't be fragmanted
            bitmap.Dispose();
        }

        private Bitmap GetScreenBitmap()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            try
            {
                Graphics g = Graphics.FromImage(bitmap);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                //A single frame can get as big as ~200,000 bytes
                g.Dispose();
            }
            catch (Exception)
            {
                bitmap.Dispose();
                return null;
            }
            return bitmap;
            //Don't forget to dispose bitmap at the end!
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
            foreach (string file in files) Partner.SendToAll(Tools.PackFile(file), DeliveryMethod.ReliableUnordered);
        }
    }
}
