using CNET.FrontOffice_V._7.PMS.Common_Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmDisplay : Form
    {
        private string _url = "";
        public frmDisplay(Screen screen, string url)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;

            _url = url;
            Location = screen.WorkingArea.Location;
            WindowState = FormWindowState.Maximized;

            winPlayerDisplay.uiMode = "none";
            winPlayerDisplay.settings.setMode("loop", true);
            winPlayerDisplay.enableContextMenu = false;
            winPlayerDisplay.Ctlenabled = false;
            winPlayerDisplay.PlayStateChange += winPlayerDisplay_PlayStateChange;
            winPlayerDisplay.settings.volume = 0;
        }


        public AxWMPLib.AxWindowsMediaPlayer GetWindowsMediaPlayer()
        {
            return winPlayerDisplay;
        }

       
        void winPlayerDisplay_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (winPlayerDisplay.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                winPlayerDisplay.fullScreen = true;
            }
        }

        private void frmDisplay_Load(object sender, EventArgs e)
        {
            try
            {
                winPlayerDisplay.URL = @"" + _url;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in playing display video. Detail: " + ex.Message, "ERROR");
            }
        }

        private class MyPlayer : AxWMPLib.AxWindowsMediaPlayer
        {
            public MyPlayer()
            {
                SetStyle(ControlStyles.UserPaint, true);

            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.RotateTransform(90.0f);
            }
        }
        
    }
}
