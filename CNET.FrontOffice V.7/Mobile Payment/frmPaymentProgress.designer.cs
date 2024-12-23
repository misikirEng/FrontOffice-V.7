namespace CNET.Mobile.Payments
{
    partial class frmPaymentProgress
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
            this.tcNavigation = new DevExpress.XtraTab.XtraTabControl();
            this.tpNotification = new DevExpress.XtraTab.XtraTabPage();
            this.pbNotification = new System.Windows.Forms.PictureBox();
            this.tpProgress = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.PictureBox();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.tcNavigation)).BeginInit();
            this.tcNavigation.SuspendLayout();
            this.tpNotification.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbNotification)).BeginInit();
            this.tpProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // tcNavigation
            // 
            this.tcNavigation.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tcNavigation.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tcNavigation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcNavigation.Location = new System.Drawing.Point(0, 0);
            this.tcNavigation.Margin = new System.Windows.Forms.Padding(0);
            this.tcNavigation.Name = "tcNavigation";
            this.tcNavigation.SelectedTabPage = this.tpNotification;
            this.tcNavigation.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.tcNavigation.Size = new System.Drawing.Size(268, 268);
            this.tcNavigation.TabIndex = 0;
            this.tcNavigation.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpProgress,
            this.tpNotification});
            // 
            // tpNotification
            // 
            this.tpNotification.Controls.Add(this.pbNotification);
            this.tpNotification.Margin = new System.Windows.Forms.Padding(0);
            this.tpNotification.Name = "tpNotification";
            this.tpNotification.Size = new System.Drawing.Size(262, 262);
            this.tpNotification.Text = "Notification";
            // 
            // pbNotification
            // 
            this.pbNotification.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbNotification.Image =  FrontOffice_V._7.Properties.Resources.PyamentConfirmed;
            this.pbNotification.Location = new System.Drawing.Point(0, 0);
            this.pbNotification.Margin = new System.Windows.Forms.Padding(0);
            this.pbNotification.Name = "pbNotification";
            this.pbNotification.Size = new System.Drawing.Size(262, 262);
            this.pbNotification.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbNotification.TabIndex = 0;
            this.pbNotification.TabStop = false;
            // 
            // tpProgress
            // 
            this.tpProgress.Controls.Add(this.layoutControl1);
            this.tpProgress.Margin = new System.Windows.Forms.Padding(0);
            this.tpProgress.Name = "tpProgress";
            this.tpProgress.Size = new System.Drawing.Size(262, 262);
            this.tpProgress.Text = "Progress";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Controls.Add(this.lblProgress);
            this.layoutControl1.Controls.Add(this.pbProgress);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(262, 262);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Brown;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft New Tai Lue", 13F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(172, 192);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 70);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.BackColor = System.Drawing.Color.White;
            this.lblProgress.Font = new System.Drawing.Font("Tahoma", 18F);
            this.lblProgress.Location = new System.Drawing.Point(0, 192);
            this.lblProgress.Margin = new System.Windows.Forms.Padding(0);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(172, 70);
            this.lblProgress.TabIndex = 4;
            this.lblProgress.Text = "Processing Payment...";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbProgress
            // 
            this.pbProgress.Image = FrontOffice_V._7.Properties.Resources.Progress;
            this.pbProgress.Location = new System.Drawing.Point(0, 0);
            this.pbProgress.Margin = new System.Windows.Forms.Padding(0);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(262, 192);
            this.pbProgress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbProgress.TabIndex = 0;
            this.pbProgress.TabStop = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(262, 262);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.pbProgress;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(262, 192);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.lblProgress;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 192);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(1, 1);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Size = new System.Drawing.Size(172, 70);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnCancel;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(172, 192);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(90, 70);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(90, 70);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem3.Size = new System.Drawing.Size(90, 70);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "layoutControlItem3";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // frmPaymentProgress
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 268);
            this.ControlBox = false;
            this.Controls.Add(this.tcNavigation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPaymentProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPaymentProgress";
            this.Load += new System.EventHandler(this.frmPaymentProgress_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tcNavigation)).EndInit();
            this.tcNavigation.ResumeLayout(false);
            this.tpNotification.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbNotification)).EndInit();
            this.tpProgress.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl tcNavigation;
        private DevExpress.XtraTab.XtraTabPage tpProgress;
        private DevExpress.XtraTab.XtraTabPage tpNotification;
        private System.Windows.Forms.PictureBox pbProgress;
        private System.Windows.Forms.PictureBox pbNotification;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblProgress;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;

    }
}