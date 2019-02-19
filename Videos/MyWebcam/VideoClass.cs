using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MyWebcam
{
    public class VideoAPI
    {
        //视频调用API
        [DllImport("avicap32.dll")]
        public static extern IntPtr capCreateCaptureWindow(byte[] strWindowName, int dwStyle, int x, int y, int width, int height, IntPtr hwdParent, int nID);

        [DllImport("avicap32.dll")]
        public static extern bool capGetDriverDescription(short wDriver, byte[] lpszName, int cbName, byte[] lpszVer, int cbVer);

        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, short wParam, int lParam);

        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, string lParam);

        [DllImport("Kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        //常量
        public const int WM_USER = 0x400; 
        public const int WS_CHILD = 0x40000000; 
        public const int WS_VISIBLE = 0x10000000; 
        public const int SWP_NOMOVE = 0x2; 
        public const int SWP_NOZORDER = 0x4;  
        public const int WM_CAP_DRIVER_CONNECT = WM_USER + 10; 
        public const int WM_CAP_DRIVER_DISCONNECT = WM_USER + 11; 
        public const int WM_CAP_SET_CALLBACK_FRAME = WM_USER + 5; 
        public const int WM_CAP_SET_PREVIEW = WM_USER + 50; 
        public const int WM_CAP_SET_PREVIEWRATE = WM_USER + 52; 
        public const int WM_CAP_SET_VIDEOFORMAT = WM_USER + 45; 
        public const int WM_CAP_START = WM_USER;  
        public const int WM_CAP_SAVEDIB = WM_CAP_START + 25;
        public const int WM_CAP_SET_SCALE = WM_USER + 53;
        public const int WM_COPYTOCLIPBORAD = WM_USER + 30;
        public const int WM_CAP_SEQUENCE = WM_USER + 62;
        public const int WM_CAP_FILE_SET_CAPTURE_FILE = WM_USER + 20;
        public const int WM_CAP_STOP = WM_USER + 68;

    }

    public class VideoClass
    {
        private IntPtr caphwnd;
        private IntPtr controlhwnd;
        private int height;
        private int width;
        private bool isstart = false;

        public VideoClass()
        {
            
        }

        public void Initialize(IntPtr controlhwnd, int width, int height)
        {
            this.controlhwnd = controlhwnd;
            this.width = width;
            this.height = height;
        }

        ~VideoClass()
        {
            if (isstart)
            {
                this.StopWebcam();
            }
        }

        /// <summary>
        /// 检查是否有视频设备
        /// </summary>
        /// <returns></returns>
        public bool CheckWebcamExist()
        {
            byte[] lpszName = new byte[100];
            byte[] lpszVer = new byte[100];
            VideoAPI.capGetDriverDescription(0, lpszName, 100, lpszVer, 100);

            caphwnd = VideoAPI.capCreateCaptureWindow(lpszName, VideoAPI.WS_CHILD | VideoAPI.WS_VISIBLE, 0, 0, this.width, this.height, this.controlhwnd, 1);

            if (caphwnd == null)
            {
                return false;
            }

            int index = 0;
            bool isconnect = VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_DRIVER_CONNECT, 0, 0);
            while (!isconnect)
            {
                isconnect = VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_DRIVER_CONNECT, 0, 0);
                if (++index <= 10)
                    continue;
            }
            if (isconnect == false)
            {
                VideoAPI.CloseHandle(caphwnd);
                return false;
            }
            else
            {
                VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_DRIVER_DISCONNECT, 0, 0);
                return true;
            }
        }

        /// <summary>
        /// 启动视频设备
        /// </summary>
        /// <returns></returns>
        public bool StartWebcam()
        {
            byte[] lpszName = new byte[100];
            byte[] lpszVer = new byte[100];
            VideoAPI.capGetDriverDescription(0, lpszName, 100, lpszVer, 100);

            caphwnd = VideoAPI.capCreateCaptureWindow(lpszName, VideoAPI.WS_CHILD | VideoAPI.WS_VISIBLE, 0, 0, this.width, this.height, this.controlhwnd, 1);

            if (caphwnd == null)
            {
                return false;
            }

            //解决在Win7系统下摄像头获取不到问题
            int index=0;
            bool isconnect = VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_DRIVER_CONNECT, 0, 0);
            while (!isconnect)
            {
                isconnect = VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_DRIVER_CONNECT, 0, 0);
                if (++index <= 10)
                    continue;
            }
            if(isconnect == false)
            {
                VideoAPI.CloseHandle(caphwnd);
                return false;
            }

            if (VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_SET_PREVIEWRATE, 66, 0) == false)
            {
                return false;
            }

            if (VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_SET_PREVIEW, true, 0) == false)
            {
                return false;
            }

            if (VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_SET_SCALE, 1, 0) == false)
            {
                return false;
            }

            isstart = true;
            return true;
        }

        /// <summary>
        /// 关闭视频设备
        /// </summary>
        /// <returns></returns>
        public bool StopWebcam()
        {
            if (caphwnd != null)
            {
                isstart = false;
                return VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_DRIVER_DISCONNECT, 0, 0);
            }
            else
                return false;
        }

        public bool GrabImage(IntPtr hWndC, string path)
        {
            if (caphwnd != null)
                return VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_SAVEDIB, 0, path);
            else
                return false;
        }

        public bool SaveImage(string path)
        {
            return GrabImage(this.caphwnd, path);
        }

        public void CopyToClipBorad()
        {
            VideoAPI.SendMessage(caphwnd, VideoAPI.WM_COPYTOCLIPBORAD, 0, 0);
        }

        public System.Drawing.Image getCaptureImage()
        {
            System.Windows.Forms.IDataObject iData = System.Windows.Forms.Clipboard.GetDataObject();
            System.Drawing.Image retImage = null;
            if (iData != null)
            {
                if (iData.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
                {
                    retImage = (System.Drawing.Image)iData.GetData(System.Windows.Forms.DataFormats.Bitmap);
                }
                else if (iData.GetDataPresent(System.Windows.Forms.DataFormats.Dib))
                {
                    retImage = (System.Drawing.Image)iData.GetData(System.Windows.Forms.DataFormats.Dib);
                }
            }
            return retImage;
        }

        public bool StartRecordVideo(string path)
        {
            VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_FILE_SET_CAPTURE_FILE, 0, path);
            VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_SEQUENCE, 0, 0);
            return true;
        }

        public bool StopRecordVideo()
        {
            return VideoAPI.SendMessage(caphwnd, VideoAPI.WM_CAP_STOP, 0, 0);
        }

    }
}
