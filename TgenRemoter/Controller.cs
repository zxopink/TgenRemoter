﻿using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetProtocol.WinForms;
using TgenNetTools;
using LiteNetLib;
using System.Diagnostics;

namespace TgenRemoter
{
    using static NetworkMessages;
    [System.ComponentModel.DesignerCategory("")] //To not view the designer on open
    public partial class Controller : FormNetworkBehavour
    {
        UdpManager Partner { get; set; }
        const int CONTROLLER_PORT = 7788;
        public Controller(IPEndPoint partnerEP)
        {
            Partner = new UdpManager(CONTROLLER_PORT);
            Partner.NatPunchEnabled = true;
            //Register events
            TgenLog.Log("controller connecting to: " + partnerEP + " and my endPoint: " + Partner.LocalEP);
            Partner.NetworkErrorEvent += (peer, error) => { TgenLog.Log("Network Error: " + error); };
            Partner.NetworkLatencyUpdateEvent += (peer, latency) => { TgenLog.Log("New latency: " + latency); };
            Partner.PeerDisconnectedEvent += (ep, info) => { CloseWindow(true); TgenLog.Log("Disconnect reason: " + info.Reason); };
            Partner.PeerConnectedEvent += (ep) => { ConnectionIntialized(); };

            Partner.StartThread();
            Partner.Connect(partnerEP);

            InitializeComponent();
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
        }

        #region CursorController
        private void ScreenSharePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Partner.SendToAll(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.LeftDown), DeliveryMethod.ReliableSequenced);
                    break;
                case MouseButtons.Middle:
                    Partner.SendToAll(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.MiddleDown), DeliveryMethod.ReliableSequenced);
                    break;
                case MouseButtons.Right:
                    Partner.SendToAll(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.RightDown), DeliveryMethod.ReliableSequenced);
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
                    Partner.SendToAll(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.LeftUp), DeliveryMethod.ReliableSequenced);
                    break;
                case MouseButtons.Middle:
                    Partner.SendToAll(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.MiddleUp), DeliveryMethod.ReliableSequenced);
                    break;
                case MouseButtons.Right:
                    Partner.SendToAll(new RemoteControlMousePress(RemoteControlMousePress.MouseEventFlags.RightUp), DeliveryMethod.ReliableSequenced);
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

        private void ScreenSharePictureBox_MouseWheel(object sender, MouseEventArgs e) => Partner.SendToAll(new RemoteControlMousePress(e.Delta), DeliveryMethod.ReliableSequenced);

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

            Partner.SendToAll(mousePos, DeliveryMethod.Unreliable);
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

            Partner.SendToAll(input, DeliveryMethod.ReliableSequenced);
        }
        #endregion

        public void CloseWindow(bool partnerDisconnected)
        {
            Partner.Close();
            if(partnerDisconnected)
                MessageBox.Show("The other side has disconnected", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Application.Exit();
        }

        private void Controller_SizeChanged(object sender, EventArgs e)
        {
            ScreenSharePictureBox.Width = ClientRectangle.Width; //to fit the ratio
            ScreenSharePictureBox.Height = ClientRectangle.Height; //to fit the ratio
        }

        private void Controller_FormClosed(object sender, FormClosedEventArgs e) =>
            CloseWindow(false);

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
            foreach (string file in files) Partner.SendToAll(Tools.PackFile(file), DeliveryMethod.ReliableUnordered);
        }
    }
}
