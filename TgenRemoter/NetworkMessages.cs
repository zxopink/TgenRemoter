using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TgenRemoter
{
    public class NetworkMessages
    {
        #region Basic Messages
        [Serializable]
        [Obsolete]
        public class RemoteStartedMessage { }

        [Serializable]
        public class ConnectionIntializedEvent { }

        [Serializable]
        public class NetworkPartnerSettings {

            /// <summary>
            /// Allow files tranformation
            /// </summary>
            bool allowFiles;
            public bool AllowFiles { get => allowFiles; }
            public NetworkPartnerSettings(bool AllowFiles)
            {
                this.allowFiles = AllowFiles;
            }
        }
        #endregion

        #region Control Messages
        [Serializable]
        public class RemoteControlMousePos
        {
            int x; int y;
            public double xRatio; public double yRatio;
            public RemoteControlMousePos(int x, int y) { this.x = x; this.y = y; }
            public RemoteControlMousePos(double xRatio, double yRatio) { this.xRatio = xRatio; this.yRatio = yRatio; }

            public static implicit operator RemoteControlMousePos(Point mousePos) => new RemoteControlMousePos(mousePos.X, mousePos.Y); //turns point class into RemoteControlMousePos class
            public static implicit operator Point(RemoteControlMousePos mousePos) => new Point(mousePos.x, mousePos.y); //turns RemoteControlMousePos class into point class
        }

        [Serializable]
        public class RemoteControlMousePress
        {
            [Flags]
            [Serializable]
            public enum MouseEventFlags
            {
                LeftDown = 0x00000002,
                LeftUp = 0x00000004,
                MiddleDown = 0x00000020,
                MiddleUp = 0x00000040,
                Move = 0x00000001,
                Wheel = 0x00000800,
                Absolute = 0x00008000,
                RightDown = 0x00000008,
                RightUp = 0x00000010
            }
            private MouseEventFlags mouseEvent;
            private int wheelDelta;

            public RemoteControlMousePress(MouseEventFlags pressType) => this.mouseEvent = pressType;
            public RemoteControlMousePress(int delta)
            {
                mouseEvent = MouseEventFlags.Wheel;
                this.wheelDelta = delta;
            }

            public static implicit operator MouseEventFlags(RemoteControlMousePress mousePress) => mousePress.mouseEvent;

            public void SignMouse()
            {
                MouseEvent(mouseEvent, wheelDelta);
            }

            [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool SetCursorPos(int x, int y);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetCursorPos(out MousePoint lpMousePoint);

            [DllImport("user32.dll")]
            private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

            private static void SetCursorPosition(int x, int y)
            {
                SetCursorPos(x, y);
            }

            private static void SetCursorPosition(MousePoint point)
            {
                SetCursorPos(point.X, point.Y);
            }

            private static MousePoint GetCursorPosition()
            {
                MousePoint currentMousePoint;
                var gotPoint = GetCursorPos(out currentMousePoint);
                if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
                return currentMousePoint;
            }

            private static void MouseEvent(MouseEventFlags value, int wheelDelta = 0)
            {
                MousePoint position = GetCursorPosition();

                mouse_event
                    ((int)value,
                     position.X,
                     position.Y,
                     wheelDelta,
                     0)
                    ;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MousePoint
            {
                public int X;
                public int Y;

                public MousePoint(int x, int y)
                {
                    X = x;
                    Y = y;
                }
            }
        }

        [Serializable]
        public class RemoteControlKeyboard
        {
            public string input;
            public bool ctrl;
            public bool alt;
            public bool shift;

            public RemoteControlKeyboard(string input) => this.input = input;
            public void SignKey()
            {
                string output = string.Empty;
                if (ctrl)
                    output += "^";
                if (shift)
                    output += "+";
                if (alt)
                    output += "%";

                if (input.Equals("Back", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Windows.Forms.SendKeys.Send(output+"{BACKSPACE}");
                    return;
                }
                if (input.Equals("Space", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Windows.Forms.SendKeys.Send(output+" ");
                    return;
                }
                if (input.Equals("Delete", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Windows.Forms.SendKeys.Send(output+"{DELETE}");
                    return;
                }
                if (input.Equals("Return", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Windows.Forms.SendKeys.Send(output+"{ENTER}");
                    return;
                }
                if (input.Equals("Up", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Windows.Forms.SendKeys.Send(output+"{UP}");
                    return;
                }
                if (input.Equals("Down", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Windows.Forms.SendKeys.Send(output+"{DOWN}");
                    return;
                }
                if (input.Equals("capital", StringComparison.InvariantCultureIgnoreCase))
                {
                    //System.Windows.Forms.SendKeys.Send("{CAPSLOCK}"); //Kinda useless
                    return;
                }
                System.Windows.Forms.SendKeys.Send(output+input);
            }
            public static implicit operator RemoteControlKeyboard(string input) => new RemoteControlKeyboard(input);
        }

        [Serializable]
        public class RemoteControlFrame
        {
            byte[] frameData;
            public RemoteControlFrame(Bitmap screenFrame) => frameData = ToByteArray(screenFrame, ImageFormat.Jpeg);

            public static implicit operator RemoteControlFrame(Bitmap frame) => new RemoteControlFrame(frame);
            public static implicit operator Bitmap(RemoteControlFrame imageData) => FromBytes(imageData.frameData);

            private static byte[] ToByteArray(Image image, ImageFormat format)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, format);
                    return ms.ToArray();
                }
            }

            private static Bitmap FromBytes(byte[] bytes)
            {
                Bitmap bmp;
                using (var ms = new MemoryStream(bytes))
                {
                    bmp = new Bitmap(ms);
                }
                return bmp;
            }
        }
        #endregion

        #region Notifications
        [Serializable]
        public class PartnerLeft { }
        #endregion
    }
}
