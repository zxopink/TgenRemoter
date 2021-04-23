using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TgenNetProtocol;

namespace TgenRemoter
{
    public partial class Menu : FormNetworkBehavour
    {
        ServerManager serverManager;
        ClientManager clientManager;
        int port = 7777; //7777


        bool AllowNetworkFiles { get => RemoteSettings.CanSendFiles; set => RemoteSettings.CanSendFiles = value; }
        string FolderPath { get => RemoteSettings.FolderPath;  set => RemoteSettings.FolderPath = value; }
        public Menu()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TgenLog.Reset();
            FormClosed += Form1_FormClosed;
            clientManager = new ClientManager();
            publicIpLabel.Text = "Public IP: " + clientManager.PublicIp;
            localIpLabel.Text = "Local IP: " + clientManager.LocalIp;
            portLabel.Text = "Port: " + port;
            serverManager = new ServerManager(port);

            this.MouseClick += Form1_MouseClick1;

            TgenLog.Reset();

            FolderPath = KnownFolders.GetPath(KnownFolder.Downloads);
            AllowNetworkFiles = CheckFileTransformation.Checked;
        }

        private void Form1_MouseClick1(object sender, MouseEventArgs e)
        {
            Console.WriteLine("CLICKING");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            serverManager.Close();
            clientManager.Close();
            Application.Exit();
        }

        private void copyPublicIpButton_Click(object sender, EventArgs e) => Clipboard.SetText(clientManager.PublicIp);
        private void copyLocalIpButton_Click(object sender, EventArgs e) => Clipboard.SetText(clientManager.LocalIp);

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (serverListenCheckBox.Checked)
                serverManager.Start();
            else
                serverManager.Close();
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            if (partnerIp.Text == "")
                return;

            serverListenCheckBox.Checked = false;
            serverManager.Close();
            bool connected = clientManager.Connect(partnerIp.Text, port);
            Console.WriteLine(connected);
            if (!connected)
            {
                MessageBox.Show("Wasn't able to connect", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //serverManager.Start();
            }
            else
            {
                Controller controllerForm = new Controller(clientManager);
                controllerForm.Show();
                Hide();
                clientManager.Send(new NetworkMessages.RemoteStartedMessage());
            }
        }
        /// <summary>
        /// Function is called when a client connects
        /// it starts the remote control connection
        /// </summary>
        /// <param name="a"></param>
        [ServerNetworkReciver]
        public void OnControllerConnected(NetworkMessages.RemoteStartedMessage a)
        {
            clientManager.Close();
            Controlled controlledForm = new Controlled(serverManager);
            controlledForm.Show();
            //controlledForm.WindowState = FormWindowState.Minimized;
            Hide();
        }

        public void DiagnoseBitmap()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            ISerializable serializable = bitmap;
            SerializationInfo info = new SerializationInfo(serializable.GetType(), new FormatterConverter());
            StreamingContext context = new StreamingContext(StreamingContextStates.All);
            serializable.GetObjectData(info, context);
            var Node = info.GetEnumerator();
            Console.WriteLine(info.MemberCount);
            byte[] data = null;
            while (Node.MoveNext())
            {
                data = (byte[])Node.Value;
                Console.WriteLine("Item: " + Node.Name + " Type: " + Node.ObjectType + " value? " + Node.Value);
            }
            Console.WriteLine(data is IEnumerable);
            Console.WriteLine(data.Length);
        }

        #region Mouse Press Event
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        public void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
        #endregion

        private void CheckFileTransformation_CheckedChanged(object sender, EventArgs e)
        {
            AllowNetworkFiles = CheckFileTransformation.Checked;
            ButtonFilePath.Enabled = CheckFileTransformation.Checked;
        }

        private void ButtonFilePath_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog folderBrowser = new CommonOpenFileDialog())
            {
                Console.WriteLine(FolderPath);
                folderBrowser.Title = "Path For Network Files";
                folderBrowser.IsFolderPicker = true;
                folderBrowser.InitialDirectory = FolderPath;
                CommonFileDialogResult result = folderBrowser.ShowDialog();

                if (result == CommonFileDialogResult.Ok) // Test result.
                {
                    FolderPath = folderBrowser.FileName;
                }
            }
        }
    }
}
