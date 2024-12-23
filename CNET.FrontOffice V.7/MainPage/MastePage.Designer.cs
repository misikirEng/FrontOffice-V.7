namespace CNET.FrontOffice_V._7
{
    partial class MasterPageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MasterPageForm));
            badNavigator = new DevExpress.XtraBars.BarAndDockingController(components);
            alertControl1 = new DevExpress.XtraBars.Alerter.AlertControl(components);
            timer1 = new System.Windows.Forms.Timer(components);
            acHome = new DevExpress.XtraBars.Alerter.AlertControl(components);
            TabManager = new CNETTabbedMdiManager();
            ((System.ComponentModel.ISupportInitialize)badNavigator).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TabManager).BeginInit();
            SuspendLayout();
            // 
            // badNavigator
            // 
            badNavigator.PropertiesBar.AllowLinkLighting = false;
            // 
            // acHome
            // 
            acHome.AutoFormDelay = 3000;
            acHome.FormLocation = DevExpress.XtraBars.Alerter.AlertFormLocation.TopRight;
            // 
            // TabManager
            // 
            TabManager.AppearancePage.Header.Options.UseTextOptions = true;
            TabManager.AppearancePage.Header.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            TabManager.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover;
            TabManager.Controller = badNavigator;
            TabManager.FloatOnDrag = DevExpress.Utils.DefaultBoolean.True;
            TabManager.HeaderButtons = DevExpress.XtraTab.TabButtons.Prev | DevExpress.XtraTab.TabButtons.Next | DevExpress.XtraTab.TabButtons.Default;
            TabManager.HeaderButtonsShowMode = DevExpress.XtraTab.TabButtonShowMode.WhenNeeded;
            TabManager.MdiParent = this;
            TabManager.ShowHeaderFocus = DevExpress.Utils.DefaultBoolean.True;
            TabManager.ShowToolTips = DevExpress.Utils.DefaultBoolean.True;
            TabManager.TabPageWidth = 150;
            // 
            // MasterPageForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(896, 443);
            DoubleBuffered = true;
            IconOptions.Icon = (Icon)resources.GetObject("MasterPageForm.IconOptions.Icon");
            IconOptions.Image = (Image)resources.GetObject("MasterPageForm.IconOptions.Image");
            IconOptions.ShowIcon = false;
            IsMdiContainer = true;
            Name = "MasterPageForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FrontOffice Management";
            WindowState = FormWindowState.Maximized;
            FormClosing += MasterPageForm_FormClosing;
            FormClosed += Form1_FormClosed;
            Load += MasterPageForm_Load;
            Shown += MasterPageForm_Shown;
            ((System.ComponentModel.ISupportInitialize)badNavigator).EndInit();
            ((System.ComponentModel.ISupportInitialize)TabManager).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraBars.BarAndDockingController badNavigator;
        private DevExpress.XtraBars.Alerter.AlertControl alertControl1;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraBars.Alerter.AlertControl acHome;
        private CNETTabbedMdiManager TabManager;
    }
}

