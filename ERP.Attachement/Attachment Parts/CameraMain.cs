using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.IO; 
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using AForge.Video;
using AForge.Video.DirectShow;


namespace CNETCamera
{
    public partial class CameraMain : UserControl
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = new VideoCaptureDevice();

        public CameraMain()
        {         
            InitializeComponent();
        }

        #region Methods

        public FilterInfoCollection getCameraDevices()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            return videoDevices;
        }

        public void connectToCamera(int index)
        {
            if (videoSource.IsRunning)
            {
                videoSource.Stop();
                pictureBox1.Image = null;
                pictureBox1.Invalidate();
                videoSource = new VideoCaptureDevice(videoDevices[index].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                videoSource.Start();
            }
            else
            {
                videoSource = new VideoCaptureDevice(videoDevices[index].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                videoSource.Start();
            }
        }

        public Bitmap capture()
        {
            videoSource.NewFrame -= new NewFrameEventHandler(videoSource_NewFrame);
            Bitmap capturedImage = (Bitmap)pictureBox1.Image;
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
            pictureBox1.Image = capturedImage;
            return capturedImage;
        }

        public void clear()
        {
            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
        }

        public void closeOpenCamera()
        {
            videoSource.Stop();
            videoSource.NewFrame -= new NewFrameEventHandler(videoSource_NewFrame);
        }

        public void showConfiguration(IntPtr handle)
        {
            videoSource.DisplayPropertyPage(handle);
        }

        #endregion

        #region Event Handlers

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;
        }

        #endregion
    }
}
