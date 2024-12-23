namespace CNET.ERP.Client.Common.UI.Flyouts
{
    partial class MesssageFlyouts
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
            this.components = new System.ComponentModel.Container();
            this.dmCloseFlyout = new DevExpress.XtraBars.Docking2010.DocumentManager(this.components);
            this.wuiViewMessage = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.WindowsUIView(this.components);
            this.wuiFlyouts = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.TileContainer(this.components);
            this.closeERPFlyout = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.Flyout(this.components);
            this.document1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.Document(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dmCloseFlyout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wuiViewMessage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wuiFlyouts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeERPFlyout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.document1)).BeginInit();
            this.SuspendLayout();
            // 
            // dmCloseFlyout
            // 
            this.dmCloseFlyout.ContainerControl = this;
            this.dmCloseFlyout.View = this.wuiViewMessage;
            this.dmCloseFlyout.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[] {
            this.wuiViewMessage});
            // 
            // wuiViewMessage
            // 
            this.wuiViewMessage.AppearanceCaption.ForeColor = System.Drawing.Color.Navy;
            this.wuiViewMessage.AppearanceCaption.Options.UseForeColor = true;
            this.wuiViewMessage.ContentContainers.AddRange(new DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainer[] {
            this.wuiFlyouts,
            this.closeERPFlyout});
            this.wuiViewMessage.Documents.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseDocument[] {
            this.document1});
            // 
            // wuiFlyouts
            // 
            this.wuiFlyouts.Caption = "";
            //this.wuiFlyouts.Image = global::CNET.ERP.EventManagement.Properties.Resources.aiga_information;
            this.wuiFlyouts.Name = "tileContainer1";
            // 
            // closeERPFlyout
            // 
            this.closeERPFlyout.AppearanceSubtitle.BackColor = System.Drawing.Color.SteelBlue;
            this.closeERPFlyout.AppearanceSubtitle.Options.UseBackColor = true;
            this.closeERPFlyout.Name = "closeERPFlyout";
            // 
            // document1
            // 
            this.document1.Caption = "document1";
            this.document1.ControlName = "document1";
            // 
            // MesssageFlyouts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MesssageFlyouts";
            this.Text = "MesssageFlyouts";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.dmCloseFlyout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wuiViewMessage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wuiFlyouts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeERPFlyout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.document1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Docking2010.DocumentManager dmCloseFlyout;
        private DevExpress.XtraBars.Docking2010.Views.WindowsUI.WindowsUIView wuiViewMessage;
        private DevExpress.XtraBars.Docking2010.Views.WindowsUI.TileContainer wuiFlyouts;
        private DevExpress.XtraBars.Docking2010.Views.WindowsUI.Flyout closeERPFlyout;
        private DevExpress.XtraBars.Docking2010.Views.WindowsUI.Document document1;

    }
}