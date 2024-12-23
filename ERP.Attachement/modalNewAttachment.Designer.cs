namespace ERP.Attachement
{
    partial class modalNewAttachment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(modalNewAttachment));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.bbiSave = new DevExpress.XtraBars.BarButtonItem();
            this.bbiClose = new DevExpress.XtraBars.BarButtonItem();
            this.bbiBrowse = new DevExpress.XtraBars.BarButtonItem();
            this.bbiScan = new DevExpress.XtraBars.BarButtonItem();
            this.bbiCamera = new DevExpress.XtraBars.BarButtonItem();
            this.bbiEditor = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.rpgSave = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rpgClose = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.pcPreview = new DevExpress.XtraEditors.PanelControl();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.spreadsheetControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
            this.pdfViewer1 = new DevExpress.XtraPdfViewer.PdfViewer();
            this.richEditControl1 = new DevExpress.XtraRichEdit.RichEditControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txtUrl = new DevExpress.XtraEditors.TextEdit();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bbiCapture = new DevExpress.XtraEditors.SimpleButton();
            this.bbiConfig = new DevExpress.XtraEditors.SimpleButton();
            this.cbDevices = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cbScannerDevices = new DevExpress.XtraEditors.ComboBoxEdit();
            this.bbiScanDocumnet = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcPreview)).BeginInit();
            this.pcPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbDevices.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbScannerDevices.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.bbiSave,
            this.bbiClose,
            this.bbiBrowse,
            this.bbiScan,
            this.bbiCamera,
            this.bbiEditor});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 9;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            this.ribbonControl1.ShowToolbarCustomizeItem = false;
            this.ribbonControl1.Size = new System.Drawing.Size(670, 96);
            this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiSave
            // 
            this.bbiSave.Caption = "Save";
            this.bbiSave.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            this.bbiSave.Glyph = ((System.Drawing.Image)(resources.GetObject("bbiSave.Glyph")));
            this.bbiSave.Id = 2;
            this.bbiSave.Name = "bbiSave";
            this.bbiSave.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiSave_ItemClick);
            // 
            // bbiClose
            // 
            this.bbiClose.Caption = "Close";
            this.bbiClose.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            this.bbiClose.Glyph = ((System.Drawing.Image)(resources.GetObject("bbiClose.Glyph")));
            this.bbiClose.Id = 3;
            this.bbiClose.Name = "bbiClose";
            this.bbiClose.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiClose_ItemClick);
            // 
            // bbiBrowse
            // 
            this.bbiBrowse.Caption = "Browse";
            this.bbiBrowse.Glyph = ((System.Drawing.Image)(resources.GetObject("bbiBrowse.Glyph")));
            this.bbiBrowse.Id = 4;
            this.bbiBrowse.Name = "bbiBrowse";
            this.bbiBrowse.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiBrowse.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiBrowse_ItemClick);
            // 
            // bbiScan
            // 
            this.bbiScan.Caption = "Scan";
            this.bbiScan.Glyph = global::CNETAttachment.Properties.Resources.Scanner;
            this.bbiScan.Id = 5;
            this.bbiScan.Name = "bbiScan";
            this.bbiScan.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiScan.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // bbiCamera
            // 
            this.bbiCamera.Caption = "Camera";
            this.bbiCamera.Glyph = global::CNETAttachment.Properties.Resources.Camera;
            this.bbiCamera.Id = 6;
            this.bbiCamera.Name = "bbiCamera";
            this.bbiCamera.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiCamera.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiCamera_ItemClick);
            // 
            // bbiEditor
            // 
            this.bbiEditor.Caption = "Edit";
            this.bbiEditor.Enabled = false;
            this.bbiEditor.Glyph = ((System.Drawing.Image)(resources.GetObject("bbiEditor.Glyph")));
            this.bbiEditor.Id = 8;
            this.bbiEditor.Name = "bbiEditor";
            this.bbiEditor.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiEditor.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiEditor_ItemClick_1);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.rpgSave,
            this.rpgClose});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // rpgSave
            // 
            this.rpgSave.AllowTextClipping = false;
            this.rpgSave.ItemLinks.Add(this.bbiSave);
            this.rpgSave.Name = "rpgSave";
            this.rpgSave.ShowCaptionButton = false;
            // 
            // rpgClose
            // 
            this.rpgClose.AllowTextClipping = false;
            this.rpgClose.ItemLinks.Add(this.bbiBrowse);
            this.rpgClose.ItemLinks.Add(this.bbiScan);
            this.rpgClose.ItemLinks.Add(this.bbiCamera);
            this.rpgClose.ItemLinks.Add(this.bbiEditor);
            this.rpgClose.ItemLinks.Add(this.bbiClose);
            this.rpgClose.Name = "rpgClose";
            this.rpgClose.ShowCaptionButton = false;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.pcPreview);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 96);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(670, 337);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "Preview";
            // 
            // pcPreview
            // 
            this.pcPreview.Controls.Add(this.pictureEdit1);
            this.pcPreview.Controls.Add(this.axWindowsMediaPlayer1);
            this.pcPreview.Controls.Add(this.spreadsheetControl1);
            this.pcPreview.Controls.Add(this.pdfViewer1);
            this.pcPreview.Controls.Add(this.richEditControl1);
            this.pcPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcPreview.Location = new System.Drawing.Point(2, 21);
            this.pcPreview.Name = "pcPreview";
            this.pcPreview.Size = new System.Drawing.Size(666, 314);
            this.pcPreview.TabIndex = 0;
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureEdit1.Location = new System.Drawing.Point(2, 2);
            this.pictureEdit1.MenuManager = this.ribbonControl1;
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.ShowScrollBars = true;
            this.pictureEdit1.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
            this.pictureEdit1.Size = new System.Drawing.Size(662, 310);
            this.pictureEdit1.TabIndex = 6;
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(2, 2);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(662, 310);
            this.axWindowsMediaPlayer1.TabIndex = 5;
            // 
            // spreadsheetControl1
            // 
            this.spreadsheetControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetControl1.Location = new System.Drawing.Point(2, 2);
            this.spreadsheetControl1.MenuManager = this.ribbonControl1;
            this.spreadsheetControl1.Name = "spreadsheetControl1";
            this.spreadsheetControl1.Size = new System.Drawing.Size(662, 310);
            this.spreadsheetControl1.TabIndex = 4;
            this.spreadsheetControl1.Text = "spreadsheetControl1";
            // 
            // pdfViewer1
            // 
            this.pdfViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pdfViewer1.Location = new System.Drawing.Point(2, 2);
            this.pdfViewer1.Name = "pdfViewer1";
            this.pdfViewer1.Size = new System.Drawing.Size(662, 310);
            this.pdfViewer1.TabIndex = 3;
            // 
            // richEditControl1
            // 
            this.richEditControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richEditControl1.Location = new System.Drawing.Point(2, 2);
            this.richEditControl1.MenuManager = this.ribbonControl1;
            this.richEditControl1.Name = "richEditControl1";
            this.richEditControl1.Options.Fields.UseCurrentCultureDateTimeFormat = false;
            this.richEditControl1.Options.MailMerge.KeepLastParagraph = false;
            this.richEditControl1.ReadOnly = true;
            this.richEditControl1.Size = new System.Drawing.Size(662, 310);
            this.richEditControl1.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.Transparent;
            this.statusStrip1.Location = new System.Drawing.Point(0, 433);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(670, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(359, 13);
            this.txtUrl.MenuManager = this.ribbonControl1;
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Properties.ReadOnly = true;
            this.txtUrl.Size = new System.Drawing.Size(122, 20);
            this.txtUrl.TabIndex = 6;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(359, 47);
            this.txtDescription.MenuManager = this.ribbonControl1;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(122, 20);
            this.txtDescription.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(293, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Url";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(293, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Description";
            // 
            // bbiCapture
            // 
            this.bbiCapture.Location = new System.Drawing.Point(487, 11);
            this.bbiCapture.Name = "bbiCapture";
            this.bbiCapture.Size = new System.Drawing.Size(59, 23);
            this.bbiCapture.TabIndex = 12;
            this.bbiCapture.Text = "Capture";
            this.bbiCapture.Visible = false;
            this.bbiCapture.Click += new System.EventHandler(this.bbiCapture_Click);
            // 
            // bbiConfig
            // 
            this.bbiConfig.Location = new System.Drawing.Point(487, 45);
            this.bbiConfig.Name = "bbiConfig";
            this.bbiConfig.Size = new System.Drawing.Size(59, 23);
            this.bbiConfig.TabIndex = 14;
            this.bbiConfig.Text = "Configure";
            this.bbiConfig.Visible = false;
            this.bbiConfig.Click += new System.EventHandler(this.bbiConfig_Click);
            // 
            // cbDevices
            // 
            this.cbDevices.Location = new System.Drawing.Point(360, 13);
            this.cbDevices.MenuManager = this.ribbonControl1;
            this.cbDevices.Name = "cbDevices";
            this.cbDevices.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbDevices.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbDevices.Size = new System.Drawing.Size(121, 20);
            this.cbDevices.TabIndex = 18;
            this.cbDevices.Visible = false;
            this.cbDevices.SelectedIndexChanged += new System.EventHandler(this.cbDevices_SelectedIndexChanged);
            // 
            // cbScannerDevices
            // 
            this.cbScannerDevices.Location = new System.Drawing.Point(360, 13);
            this.cbScannerDevices.MenuManager = this.ribbonControl1;
            this.cbScannerDevices.Name = "cbScannerDevices";
            this.cbScannerDevices.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbScannerDevices.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbScannerDevices.Size = new System.Drawing.Size(121, 20);
            this.cbScannerDevices.TabIndex = 20;
            this.cbScannerDevices.Visible = false;
            this.cbScannerDevices.SelectedIndexChanged += new System.EventHandler(this.cbScannerDevices_SelectedIndexChanged);
            // 
            // bbiScanDocumnet
            // 
            this.bbiScanDocumnet.Location = new System.Drawing.Point(487, 12);
            this.bbiScanDocumnet.Name = "bbiScanDocumnet";
            this.bbiScanDocumnet.Size = new System.Drawing.Size(59, 23);
            this.bbiScanDocumnet.TabIndex = 22;
            this.bbiScanDocumnet.Text = "Scan";
            this.bbiScanDocumnet.Visible = false;
            this.bbiScanDocumnet.Click += new System.EventHandler(this.bbiScanDocumnet_Click);
            // 
            // modalNewAttachment
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.BorderColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseBorderColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 455);
            this.ControlBox = false;
            this.Controls.Add(this.bbiScanDocumnet);
            this.Controls.Add(this.cbScannerDevices);
            this.Controls.Add(this.cbDevices);
            this.Controls.Add(this.bbiConfig);
            this.Controls.Add(this.bbiCapture);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ribbonControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "modalNewAttachment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Attachment";
            this.Load += new System.EventHandler(this.modalNewAttachment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcPreview)).EndInit();
            this.pcPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbDevices.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbScannerDevices.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        public DevExpress.XtraBars.BarButtonItem bbiSave;
        public DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.BarButtonItem bbiBrowse;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgSave;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgClose;
        private DevExpress.XtraSpreadsheet.SpreadsheetControl spreadsheetControl1;
        private DevExpress.XtraPdfViewer.PdfViewer pdfViewer1;
        private DevExpress.XtraRichEdit.RichEditControl richEditControl1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private DevExpress.XtraBars.BarButtonItem bbiScan;
        private DevExpress.XtraBars.BarButtonItem bbiCamera;
        private DevExpress.XtraEditors.TextEdit txtUrl;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.PanelControl pcPreview;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private DevExpress.XtraEditors.SimpleButton bbiConfig;
        public  DevExpress.XtraEditors.ComboBoxEdit cbDevices;
        public DevExpress.XtraEditors.ComboBoxEdit cbScannerDevices;
        private DevExpress.XtraEditors.SimpleButton bbiScanDocumnet;
        private DevExpress.XtraEditors.SimpleButton bbiCapture;
        private DevExpress.XtraBars.BarButtonItem bbiEditor;
        public DevExpress.XtraEditors.PictureEdit pictureEdit1;
    }
}