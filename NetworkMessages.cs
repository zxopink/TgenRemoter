using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgenRemoter
{
    public class NetworkMessages
    {
        #region Basic Messages
        [Serializable]
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
            [Serializable]
            public enum PressType
            {
                leftClick,
                middleClick,
                rightClick
                //doubleClick not used
            }
            public PressType pressType;
            public RemoteControlMousePress(PressType pressType) => this.pressType = pressType;
            public static implicit operator PressType(RemoteControlMousePress mousePress) => mousePress.pressType;
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
