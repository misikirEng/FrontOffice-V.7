namespace CNET.ERP.EventManagement
{
    partial class frmEventDocumentBrowser
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.tcEventDocuments = new DevExpress.XtraTab.XtraTabControl();
            this.tpEventVoucher = new DevExpress.XtraTab.XtraTabPage();
            this.pcEventVoucher = new DevExpress.XtraEditors.PanelControl();
            this.tpEventRequirement = new DevExpress.XtraTab.XtraTabPage();
            this.pcEventRequirement = new DevExpress.XtraEditors.PanelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcEventDocuments)).BeginInit();
            this.tcEventDocuments.SuspendLayout();
            this.tpEventVoucher.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcEventVoucher)).BeginInit();
            this.tpEventRequirement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcEventRequirement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.tcEventDocuments);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(835, 433);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // tcEventDocuments
            // 
            this.tcEventDocuments.Location = new System.Drawing.Point(2, 2);
            this.tcEventDocuments.Name = "tcEventDocuments";
            this.tcEventDocuments.SelectedTabPage = this.tpEventVoucher;
            this.tcEventDocuments.Size = new System.Drawing.Size(831, 429);
            this.tcEventDocuments.TabIndex = 4;
            this.tcEventDocuments.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpEventVoucher,
            this.tpEventRequirement});
            this.tcEventDocuments.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tcEventDocuments_SelectedPageChanged);
            // 
            // tpEventVoucher
            // 
            this.tpEventVoucher.Controls.Add(this.pcEventVoucher);
            this.tpEventVoucher.Name = "tpEventVoucher";
            this.tpEventVoucher.Size = new System.Drawing.Size(825, 401);
            this.tpEventVoucher.Text = "Event Voucher";
            // 
            // pcEventVoucher
            // 
            this.pcEventVoucher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcEventVoucher.Location = new System.Drawing.Point(0, 0);
            this.pcEventVoucher.Name = "pcEventVoucher";
            this.pcEventVoucher.Size = new System.Drawing.Size(825, 401);
            this.pcEventVoucher.TabIndex = 0;
            // 
            // tpEventRequirement
            // 
            this.tpEventRequirement.Controls.Add(this.pcEventRequirement);
            this.tpEventRequirement.Name = "tpEventRequirement";
            this.tpEventRequirement.Size = new System.Drawing.Size(825, 401);
            this.tpEventRequirement.Text = "Event Requirement Voucher";
            // 
            // pcEventRequirement
            // 
            this.pcEventRequirement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcEventRequirement.Location = new System.Drawing.Point(0, 0);
            this.pcEventRequirement.Name = "pcEventRequirement";
            this.pcEventRequirement.Size = new System.Drawing.Size(825, 401);
            this.pcEventRequirement.TabIndex = 0;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(835, 433);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.tcEventDocuments;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(835, 433);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmEventDocumentBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmEventDocumentBrowser";
            this.Size = new System.Drawing.Size(835, 433);
            this.Load += new System.EventHandler(this.frmEventDocumentBrowser_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tcEventDocuments)).EndInit();
            this.tcEventDocuments.ResumeLayout(false);
            this.tpEventVoucher.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcEventVoucher)).EndInit();
            this.tpEventRequirement.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcEventRequirement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraTab.XtraTabControl tcEventDocuments;
        private DevExpress.XtraTab.XtraTabPage tpEventVoucher;
        private DevExpress.XtraEditors.PanelControl pcEventVoucher;
        private DevExpress.XtraTab.XtraTabPage tpEventRequirement;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.PanelControl pcEventRequirement;
    }
}