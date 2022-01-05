﻿using LiteNetLib;
using RUDPSharp;
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
    //[System.ComponentModel.DesignerCategory("")] //To not view the designer on open
    public partial class Controlled : FormNetworkBehavour
    {
        ClientManager ClientManager { get; set; }
        UdpManager Partner { get; set; }
        public Controlled(ClientManager clientManager, UdpManager partner, IPEndPoint partnerEP)
        {
            ClientManager = clientManager;
            Partner = partner;
            Partner.NatPunchEnabled = true;
            Partner.DisconnectedEvent += (ep, info) => { Console.WriteLine("my ip: " + partner.LocalEP + " Disconnected from: " + ep.EndPoint + ", because: " + info.Reason); CloseWindow(); };
            Partner.ConnectedEvent += (ep) => { Partner.SendToAll(new TempUDPConnectAttempt(), DeliveryMethod.ReliableUnordered); };
            partner.Start();

            InitializeComponent();
        }

        private void Controlled_Load(object sender, EventArgs e)
        {
            AllowDrop = true;
            FormClosed += Controlled_FormClosed;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void Controlled_FormClosed(object sender, FormClosedEventArgs e)
        {
            //clientManager.Send(new PartnerLeft()); //Server handles that
            ClientManager.Close();
            Application.Exit();
        }

        public void CloseWindow()
        {
            ClientManager.Close();
            Partner.Close();
            MessageBox.Show("The other side has disconnected", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Application.Exit();
        }

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
            Console.WriteLine("Controlled_Tick");
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            //A single frame can get as big as ~200,000 bytes
            Partner.SendToAll(new RemoteControlFrame(bitmap), DeliveryMethod.ReliableOrdered);
            g.Dispose();
            bitmap.Dispose();
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
            foreach (string file in files) ClientManager.Send(Tools.PackFile(file));
        }
    }
}
