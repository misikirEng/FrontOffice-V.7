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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(modalNewAttachment));
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiSave = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            bbiBrowse = new DevExpress.XtraBars.BarButtonItem();
            bbiScan = new DevExpress.XtraBars.BarButtonItem();
            bbiCamera = new DevExpress.XtraBars.BarButtonItem();
            bbiEditor = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgSave = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgClose = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            groupControl1 = new DevExpress.XtraEditors.GroupControl();
            pcPreview = new DevExpress.XtraEditors.PanelControl();
            pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            spreadsheetControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
            pdfViewer1 = new DevExpress.XtraPdfViewer.PdfViewer();
            richEditControl1 = new DevExpress.XtraRichEdit.RichEditControl();
            statusStrip1 = new StatusStrip();
            txtUrl = new DevExpress.XtraEditors.TextEdit();
            txtDescription = new DevExpress.XtraEditors.TextEdit();
            label1 = new Label();
            label2 = new Label();
            bbiCapture = new DevExpress.XtraEditors.SimpleButton();
            bbiConfig = new DevExpress.XtraEditors.SimpleButton();
            cbDevices = new DevExpress.XtraEditors.ComboBoxEdit();
            cbScannerDevices = new DevExpress.XtraEditors.ComboBoxEdit();
            bbiScanDocumnet = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)groupControl1).BeginInit();
            groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pcPreview).BeginInit();
            pcPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)axWindowsMediaPlayer1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtUrl.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtDescription.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cbDevices.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cbScannerDevices.Properties).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, ribbonControl1.SearchEditItem, bbiSave, bbiClose, bbiBrowse, bbiScan, bbiCamera, bbiEditor });
            ribbonControl1.Location = new Point(0, 0);
            ribbonControl1.MaxItemId = 9;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowMoreCommandsButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowPageHeadersInFormCaption = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            ribbonControl1.ShowQatLocationSelector = false;
            ribbonControl1.ShowToolbarCustomizeItem = false;
            ribbonControl1.Size = new Size(670, 83);
            ribbonControl1.Toolbar.ShowCustomizeItem = false;
            ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            ribbonControl1.TransparentEditorsMode = DevExpress.Utils.DefaultBoolean.False;
            // 
            // bbiSave
            // 
            bbiSave.Caption = "Save";
            bbiSave.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSave.Id = 2;
            bbiSave.ImageOptions.Image = (Image)resources.GetObject("bbiSave.ImageOptions.Image");
            bbiSave.Name = "bbiSave";
            bbiSave.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiSave.ItemClick += bbiSave_ItemClick;
            // 
            // bbiClose
            // 
            bbiClose.Caption = "Close";
            bbiClose.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiClose.Id = 3;
            bbiClose.ImageOptions.Image = (Image)resources.GetObject("bbiClose.ImageOptions.Image");
            bbiClose.Name = "bbiClose";
            bbiClose.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiClose.ItemClick += bbiClose_ItemClick;
            // 
            // bbiBrowse
            // 
            bbiBrowse.Caption = "Browse";
            bbiBrowse.Id = 4;
            bbiBrowse.ImageOptions.Image = (Image)resources.GetObject("bbiBrowse.ImageOptions.Image");
            bbiBrowse.Name = "bbiBrowse";
            bbiBrowse.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiBrowse.ItemClick += bbiBrowse_ItemClick;
            // 
            // bbiScan
            // 
            bbiScan.Caption = "Scan";
            bbiScan.Id = 5;
            bbiScan.ImageOptions.Image = (Image)resources.GetObject("bbiScan.ImageOptions.Image");
            bbiScan.ImageOptions.LargeImage = (Image)resources.GetObject("bbiScan.ImageOptions.LargeImage");
            bbiScan.Name = "bbiScan";
            bbiScan.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiScan.ItemClick += barButtonItem1_ItemClick;
            // 
            // bbiCamera
            // 
            bbiCamera.Caption = "Camera";
            bbiCamera.Id = 6;
            bbiCamera.ImageOptions.Image = (Image)resources.GetObject("bbiCamera.ImageOptions.Image");
            bbiCamera.ImageOptions.LargeImage = (Image)resources.GetObject("bbiCamera.ImageOptions.LargeImage");
            bbiCamera.Name = "bbiCamera";
            bbiCamera.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiCamera.ItemClick += bbiCamera_ItemClick;
            // 
            // bbiEditor
            // 
            bbiEditor.Caption = "Edit";
            bbiEditor.Enabled = false;
            bbiEditor.Id = 8;
            bbiEditor.ImageOptions.Image = (Image)resources.GetObject("bbiEditor.ImageOptions.Image");
            bbiEditor.Name = "bbiEditor";
            bbiEditor.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiEditor.ItemClick += bbiEditor_ItemClick_1;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgSave, rpgClose });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // rpgSave
            // 
            rpgSave.AllowTextClipping = false;
            rpgSave.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgSave.ItemLinks.Add(bbiSave);
            rpgSave.Name = "rpgSave";
            // 
            // rpgClose
            // 
            rpgClose.AllowTextClipping = false;
            rpgClose.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgClose.ItemLinks.Add(bbiBrowse);
            rpgClose.ItemLinks.Add(bbiScan);
            rpgClose.ItemLinks.Add(bbiCamera);
            rpgClose.ItemLinks.Add(bbiEditor);
            rpgClose.ItemLinks.Add(bbiClose);
            rpgClose.Name = "rpgClose";
            // 
            // groupControl1
            // 
            groupControl1.Controls.Add(pcPreview);
            groupControl1.Dock = DockStyle.Fill;
            groupControl1.Location = new Point(0, 83);
            groupControl1.Name = "groupControl1";
            groupControl1.Size = new Size(670, 350);
            groupControl1.TabIndex = 0;
            groupControl1.Text = "Preview";
            // 
            // pcPreview
            // 
            pcPreview.Controls.Add(pictureEdit1);
            pcPreview.Controls.Add(axWindowsMediaPlayer1);
            pcPreview.Controls.Add(spreadsheetControl1);
            pcPreview.Controls.Add(pdfViewer1);
            pcPreview.Controls.Add(richEditControl1);
            pcPreview.Dock = DockStyle.Fill;
            pcPreview.Location = new Point(2, 23);
            pcPreview.Name = "pcPreview";
            pcPreview.Size = new Size(666, 325);
            pcPreview.TabIndex = 0;
            // 
            // pictureEdit1
            // 
            pictureEdit1.Dock = DockStyle.Fill;
            pictureEdit1.Location = new Point(2, 2);
            pictureEdit1.MenuManager = ribbonControl1;
            pictureEdit1.Name = "pictureEdit1";
            pictureEdit1.Properties.ShowScrollBars = true;
            pictureEdit1.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
            pictureEdit1.Size = new Size(662, 321);
            pictureEdit1.TabIndex = 6;
            // 
            // axWindowsMediaPlayer1
            // 
            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            axWindowsMediaPlayer1.Enabled = true;
            axWindowsMediaPlayer1.Location = new Point(2, 2);
            axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            axWindowsMediaPlayer1.OcxState = (AxHost.State)resources.GetObject("axWindowsMediaPlayer1.OcxState");
            axWindowsMediaPlayer1.Size = new Size(662, 321);
            axWindowsMediaPlayer1.TabIndex = 5;
            // 
            // spreadsheetControl1
            // 
            spreadsheetControl1.Dock = DockStyle.Fill;
            spreadsheetControl1.Location = new Point(2, 2);
            spreadsheetControl1.MenuManager = ribbonControl1;
            spreadsheetControl1.Name = "spreadsheetControl1";
            spreadsheetControl1.Size = new Size(662, 321);
            spreadsheetControl1.TabIndex = 4;
            spreadsheetControl1.Text = "spreadsheetControl1";
            // 
            // pdfViewer1
            // 
            pdfViewer1.Dock = DockStyle.Fill;
            pdfViewer1.Location = new Point(2, 2);
            pdfViewer1.MenuManager = ribbonControl1;
            pdfViewer1.Name = "pdfViewer1";
            pdfViewer1.Size = new Size(662, 321);
            pdfViewer1.TabIndex = 3;
            // 
            // richEditControl1
            // 
            richEditControl1.Dock = DockStyle.Fill;
            richEditControl1.Location = new Point(2, 2);
            richEditControl1.MenuManager = ribbonControl1;
            richEditControl1.Name = "richEditControl1";
            richEditControl1.ReadOnly = true;
            richEditControl1.Size = new Size(662, 321);
            richEditControl1.TabIndex = 1;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = Color.Transparent;
            statusStrip1.Location = new Point(0, 433);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(670, 22);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // txtUrl
            // 
            txtUrl.Location = new Point(372, 12);
            txtUrl.MenuManager = ribbonControl1;
            txtUrl.Name = "txtUrl";
            txtUrl.Properties.ReadOnly = true;
            txtUrl.Size = new Size(122, 20);
            txtUrl.TabIndex = 6;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(371, 43);
            txtDescription.MenuManager = ribbonControl1;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(122, 20);
            txtDescription.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(305, 19);
            label1.Name = "label1";
            label1.Size = new Size(20, 13);
            label1.TabIndex = 8;
            label1.Text = "Url";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(305, 46);
            label2.Name = "label2";
            label2.Size = new Size(60, 13);
            label2.TabIndex = 8;
            label2.Text = "Description";
            // 
            // bbiCapture
            // 
            bbiCapture.Location = new Point(511, 16);
            bbiCapture.Name = "bbiCapture";
            bbiCapture.Size = new Size(59, 23);
            bbiCapture.TabIndex = 12;
            bbiCapture.Text = "Capture";
            bbiCapture.Visible = false;
            bbiCapture.Click += bbiCapture_Click;
            // 
            // bbiConfig
            // 
            bbiConfig.Location = new Point(511, 45);
            bbiConfig.Name = "bbiConfig";
            bbiConfig.Size = new Size(59, 23);
            bbiConfig.TabIndex = 14;
            bbiConfig.Text = "Configure";
            bbiConfig.Visible = false;
            bbiConfig.Click += bbiConfig_Click;
            // 
            // cbDevices
            // 
            cbDevices.Location = new Point(372, 13);
            cbDevices.MenuManager = ribbonControl1;
            cbDevices.Name = "cbDevices";
            cbDevices.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cbDevices.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cbDevices.Size = new Size(121, 20);
            cbDevices.TabIndex = 18;
            cbDevices.Visible = false;
            cbDevices.SelectedIndexChanged += cbDevices_SelectedIndexChanged;
            // 
            // cbScannerDevices
            // 
            cbScannerDevices.Location = new Point(372, 13);
            cbScannerDevices.MenuManager = ribbonControl1;
            cbScannerDevices.Name = "cbScannerDevices";
            cbScannerDevices.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cbScannerDevices.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cbScannerDevices.Size = new Size(121, 20);
            cbScannerDevices.TabIndex = 20;
            cbScannerDevices.Visible = false;
            cbScannerDevices.SelectedIndexChanged += cbScannerDevices_SelectedIndexChanged;
            // 
            // bbiScanDocumnet
            // 
            bbiScanDocumnet.Location = new Point(511, 12);
            bbiScanDocumnet.Name = "bbiScanDocumnet";
            bbiScanDocumnet.Size = new Size(59, 23);
            bbiScanDocumnet.TabIndex = 22;
            bbiScanDocumnet.Text = "Scan";
            bbiScanDocumnet.Visible = false;
            bbiScanDocumnet.Click += bbiScanDocumnet_Click;
            // 
            // modalNewAttachment
            // 
            Appearance.BackColor = Color.White;
            Appearance.BorderColor = Color.White;
            Appearance.Options.UseBackColor = true;
            Appearance.Options.UseBorderColor = true;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(670, 455);
            ControlBox = false;
            Controls.Add(bbiScanDocumnet);
            Controls.Add(cbScannerDevices);
            Controls.Add(cbDevices);
            Controls.Add(bbiConfig);
            Controls.Add(bbiCapture);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtDescription);
            Controls.Add(txtUrl);
            Controls.Add(groupControl1);
            Controls.Add(statusStrip1);
            Controls.Add(ribbonControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("modalNewAttachment.IconOptions.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "modalNewAttachment";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "New Attachment";
            Load += modalNewAttachment_Load;
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)groupControl1).EndInit();
            groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pcPreview).EndInit();
            pcPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)axWindowsMediaPlayer1).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtUrl.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtDescription.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cbDevices.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cbScannerDevices.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
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