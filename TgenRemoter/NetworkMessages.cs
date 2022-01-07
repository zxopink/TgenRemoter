using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
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
        public class ExchangePartners 
        {
            public NetworkCodes.NetworkEndPoint partnerEP;
            public ExchangePartners()
            {
                
            }

            public ExchangePartners(NetworkCodes.NetworkEndPoint EP)
            {
                partnerEP = EP;
            }
        }

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
            public double xRatio; public double yRatio;
            public RemoteControlMousePos(double xRatio, double yRatio) { this.xRatio = xRatio; this.yRatio = yRatio; }

            public static implicit operator RemoteControlMousePos(Point mousePos) => new RemoteControlMousePos(mousePos.X, mousePos.Y); //turns point class into RemoteControlMousePos class
        }

        [Serializable]
        public class RemoteControlMousePress
        {
            [Flags]
            [Serializable]
            public enum MouseEventFlags : ushort
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
            private float xPos;
            private float yPos;

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

            static RemoteControlFrame()
            {
                QualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);
                ImageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                Parameters = new EncoderParameters(1);
            }
            public RemoteControlFrame(Bitmap screenFrame) => frameData = ToByteArray(screenFrame);


            public static implicit operator RemoteControlFrame(Bitmap frame) => new RemoteControlFrame(frame);
            public static implicit operator Bitmap(RemoteControlFrame imageData)
            {
                return FromBytes(imageData.frameData);
            }

            private static byte[] ToByteArray(Bitmap image)
            {
                return GetCompressedBitmap(image, 50);
            }

            private static EncoderParameter QualityParam;
            private static ImageCodecInfo ImageCodec;
            private static EncoderParameters Parameters;

            private static byte[] GetCompressedBitmap(Bitmap bmp, long quality)
            {
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
                {
                    //EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    //ImageCodecInfo imageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                    //EncoderParameters parameters = new EncoderParameters(1);
                    Parameters.Param[0] = QualityParam;
                    bmp.Save(dstream, ImageCodec, Parameters);
                }
                return output.ToArray();
            }

            private static Bitmap FromBytes(byte[] bytes)
            {
                MemoryStream input = new MemoryStream(bytes);
                Bitmap bmp;
                using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                {
                    bmp = new Bitmap(dstream);
                }
                return bmp;
            }

            public static byte[] Compress(byte[] data)
            {
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
                {
                    dstream.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }

            public static byte[] Decompress(byte[] data)
            {
                MemoryStream input = new MemoryStream(data);
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                {
                    dstream.CopyTo(output);
                }
                return output.ToArray();
            }
        }
        #endregion

        #region Notifications
        [Serializable]
        public class PartnerLeft { }
        #endregion
    }
}
