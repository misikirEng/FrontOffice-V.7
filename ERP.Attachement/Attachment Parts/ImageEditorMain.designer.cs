namespace CNETImageEditor
{
    partial class CNET_Image_Editor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CNET_Image_Editor));
            this.lcMain = new DevExpress.XtraLayout.LayoutControl();
            this.sbSelectedAspectRatio = new System.Windows.Forms.ComboBox();
            this.sbGray = new DevExpress.XtraEditors.SimpleButton();
            this.sbInvert = new DevExpress.XtraEditors.SimpleButton();
            this.sbRotateImage = new DevExpress.XtraEditors.SimpleButton();
            this.sbCenter = new DevExpress.XtraEditors.SimpleButton();
            this.sbRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.cmbSelectedCropBoxSize = new System.Windows.Forms.ComboBox();
            this.sbSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.pbEdit = new System.Windows.Forms.PictureBox();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.lcMain)).BeginInit();
            this.lcMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            this.SuspendLayout();
            // 
            // lcMain
            // 
            this.lcMain.Controls.Add(this.pbEdit);
            this.lcMain.Controls.Add(this.sbSelectedAspectRatio);
            this.lcMain.Controls.Add(this.sbGray);
            this.lcMain.Controls.Add(this.sbInvert);
            this.lcMain.Controls.Add(this.sbRotateImage);
            this.lcMain.Controls.Add(this.sbCenter);
            this.lcMain.Controls.Add(this.sbRefresh);
            this.lcMain.Controls.Add(this.cmbSelectedCropBoxSize);
            this.lcMain.Controls.Add(this.sbSave);
            this.lcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lcMain.Location = new System.Drawing.Point(0, 0);
            this.lcMain.Name = "lcMain";
            this.lcMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-562, 172, 250, 350);
            this.lcMain.Root = this.layoutControlGroup1;
            this.lcMain.Size = new System.Drawing.Size(745, 670);
            this.lcMain.TabIndex = 0;
            this.lcMain.Text = "layoutControl1";
            // 
            // sbSelectedAspectRatio
            // 
            this.sbSelectedAspectRatio.FormattingEnabled = true;
            this.sbSelectedAspectRatio.Location = new System.Drawing.Point(4, 20);
            this.sbSelectedAspectRatio.Name = "sbSelectedAspectRatio";
            this.sbSelectedAspectRatio.Size = new System.Drawing.Size(233, 21);
            this.sbSelectedAspectRatio.TabIndex = 10;
            // 
            // sbGray
            // 
            this.sbGray.Image = ((System.Drawing.Image)(resources.GetObject("sbGray.Image")));
            this.sbGray.Location = new System.Drawing.Point(454, 628);
            this.sbGray.Name = "sbGray";
            this.sbGray.Size = new System.Drawing.Size(151, 38);
            this.sbGray.StyleController = this.lcMain;
            this.sbGray.TabIndex = 7;
            this.sbGray.Text = "gray";
            this.sbGray.Click += new System.EventHandler(this.sbGrayScaling_Click);
            // 
            // sbInvert
            // 
            this.sbInvert.Image = ((System.Drawing.Image)(resources.GetObject("sbInvert.Image")));
            this.sbInvert.Location = new System.Drawing.Point(286, 628);
            this.sbInvert.Name = "sbInvert";
            this.sbInvert.Size = new System.Drawing.Size(164, 38);
            this.sbInvert.StyleController = this.lcMain;
            this.sbInvert.TabIndex = 7;
            this.sbInvert.Text = "Invert";
            this.sbInvert.Click += new System.EventHandler(this.sbInverting_Click);
            // 
            // sbRotateImage
            // 
            this.sbRotateImage.Image = ((System.Drawing.Image)(resources.GetObject("sbRotateImage.Image")));
            this.sbRotateImage.Location = new System.Drawing.Point(136, 628);
            this.sbRotateImage.Name = "sbRotateImage";
            this.sbRotateImage.Size = new System.Drawing.Size(146, 38);
            this.sbRotateImage.StyleController = this.lcMain;
            this.sbRotateImage.TabIndex = 6;
            this.sbRotateImage.Text = "Rotate";
            this.sbRotateImage.Click += new System.EventHandler(this.sbRotateImage_Click);
            // 
            // sbCenter
            // 
            this.sbCenter.Image = ((System.Drawing.Image)(resources.GetObject("sbCenter.Image")));
            this.sbCenter.Location = new System.Drawing.Point(609, 628);
            this.sbCenter.Name = "sbCenter";
            this.sbCenter.Size = new System.Drawing.Size(132, 38);
            this.sbCenter.StyleController = this.lcMain;
            this.sbCenter.TabIndex = 6;
            this.sbCenter.Text = "Center";
            this.sbCenter.Click += new System.EventHandler(this.sbCenter_Click);
            // 
            // sbRefresh
            // 
            this.sbRefresh.Image = ((System.Drawing.Image)(resources.GetObject("sbRefresh.Image")));
            this.sbRefresh.Location = new System.Drawing.Point(4, 628);
            this.sbRefresh.Name = "sbRefresh";
            this.sbRefresh.Size = new System.Drawing.Size(128, 38);
            this.sbRefresh.StyleController = this.lcMain;
            this.sbRefresh.TabIndex = 6;
            this.sbRefresh.Text = "Refresh";
            // 
            // cmbSelectedCropBoxSize
            // 
            this.cmbSelectedCropBoxSize.FormattingEnabled = true;
            this.cmbSelectedCropBoxSize.Location = new System.Drawing.Point(241, 20);
            this.cmbSelectedCropBoxSize.Name = "cmbSelectedCropBoxSize";
            this.cmbSelectedCropBoxSize.Size = new System.Drawing.Size(248, 21);
            this.cmbSelectedCropBoxSize.TabIndex = 8;
            this.cmbSelectedCropBoxSize.SelectedIndexChanged += new System.EventHandler(this.cmbSelectedCropBoxSize_SelectedIndexChanged);
            // 
            // sbSave
            // 
            this.sbSave.Image = ((System.Drawing.Image)(resources.GetObject("sbSave.Image")));
            this.sbSave.Location = new System.Drawing.Point(626, 4);
            this.sbSave.Name = "sbSave";
            this.sbSave.Size = new System.Drawing.Size(115, 38);
            this.sbSave.StyleController = this.lcMain;
            this.sbSave.TabIndex = 6;
            this.sbSave.Text = "Save";
            this.sbSave.Click += new System.EventHandler(this.sbSave_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4,
            this.layoutControlItem3,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.layoutControlItem6,
            this.layoutControlItem5,
            this.emptySpaceItem1,
            this.layoutControlItem1,
            this.layoutControlItem10});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlGroup1.Size = new System.Drawing.Size(745, 670);
            this.layoutControlGroup1.Text = "Root";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.cmbSelectedCropBoxSize;
            this.layoutControlItem4.ControlAlignment = System.Drawing.ContentAlignment.TopRight;
            this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
            this.layoutControlItem4.Location = new System.Drawing.Point(237, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(252, 42);
            this.layoutControlItem4.Text = "Crop Size";
            this.layoutControlItem4.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(45, 13);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.sbSelectedAspectRatio;
            this.layoutControlItem3.ControlAlignment = System.Drawing.ContentAlignment.TopRight;
            this.layoutControlItem3.CustomizationFormText = "Ratio";
            this.layoutControlItem3.ImageToTextDistance = 1;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(237, 42);
            this.layoutControlItem3.Text = "Ratio";
            this.layoutControlItem3.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(45, 13);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.sbRotateImage;
            this.layoutControlItem7.CustomizationFormText = "layoutControlItem7";
            this.layoutControlItem7.Location = new System.Drawing.Point(132, 624);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(150, 42);
            this.layoutControlItem7.Text = "layoutControlItem7";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextToControlDistance = 0;
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.sbInvert;
            this.layoutControlItem8.CustomizationFormText = "layoutControlItem8";
            this.layoutControlItem8.Location = new System.Drawing.Point(282, 624);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(168, 42);
            this.layoutControlItem8.Text = "layoutControlItem8";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextToControlDistance = 0;
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.sbGray;
            this.layoutControlItem9.CustomizationFormText = "layoutControlItem9";
            this.layoutControlItem9.Location = new System.Drawing.Point(450, 624);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(155, 42);
            this.layoutControlItem9.Text = "layoutControlItem9";
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextToControlDistance = 0;
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.sbCenter;
            this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
            this.layoutControlItem6.Location = new System.Drawing.Point(605, 624);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(136, 42);
            this.layoutControlItem6.Text = "layoutControlItem6";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextToControlDistance = 0;
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.sbRefresh;
            this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 624);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(132, 42);
            this.layoutControlItem5.Text = "layoutControlItem5";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextToControlDistance = 0;
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(489, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(133, 42);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.sbSave;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(622, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(119, 42);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // pbEdit
            // 
            this.pbEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbEdit.Location = new System.Drawing.Point(4, 46);
            this.pbEdit.Name = "pbEdit";
            this.pbEdit.Size = new System.Drawing.Size(737, 578);
            this.pbEdit.TabIndex = 11;
            this.pbEdit.TabStop = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.pbEdit;
            this.layoutControlItem10.CustomizationFormText = "layoutControlItem10";
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 42);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(741, 582);
            this.layoutControlItem10.Text = "layoutControlItem10";
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextToControlDistance = 0;
            this.layoutControlItem10.TextVisible = false;
            // 
            // CNET_Image_Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lcMain);
            this.Name = "CNET_Image_Editor";
            this.Size = new System.Drawing.Size(745, 670);
            ((System.ComponentModel.ISupportInitialize)(this.lcMain)).EndInit();
            this.lcMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton sbGray;
        private DevExpress.XtraEditors.SimpleButton sbInvert;
        private DevExpress.XtraEditors.SimpleButton sbRotateImage;
        private DevExpress.XtraEditors.SimpleButton sbCenter;
        private DevExpress.XtraEditors.SimpleButton sbRefresh;
        private System.Windows.Forms.ComboBox cmbSelectedCropBoxSize;
        private DevExpress.XtraEditors.SimpleButton sbSave;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private System.Windows.Forms.ComboBox sbSelectedAspectRatio;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        public DevExpress.XtraLayout.LayoutControl lcMain;
        public System.Windows.Forms.PictureBox pbEdit;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;

    }
}
