namespace CNET_ImageEditor
{
    partial class imageEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(imageEditor));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.bbiSave = new DevExpress.XtraBars.BarButtonItem();
            this.bbiClose = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.bbiRotate = new DevExpress.XtraEditors.SimpleButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.tbContrast = new DevExpress.XtraEditors.TrackBarControl();
            this.pbEdit = new System.Windows.Forms.PictureBox();
            this.lblContrast = new System.Windows.Forms.Label();
            this.lblContrastValue = new System.Windows.Forms.Label();
            this.bbiCrop = new DevExpress.XtraEditors.SimpleButton();
            this.rbGrayscale = new System.Windows.Forms.RadioButton();
            this.rbNegative = new System.Windows.Forms.RadioButton();
            this.rbSepia = new System.Windows.Forms.RadioButton();
            this.rbTransparency = new System.Windows.Forms.RadioButton();
            this.lblBrightnessValue = new System.Windows.Forms.Label();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.tbBrightness = new DevExpress.XtraEditors.TrackBarControl();
            this.cmbSelectedAspectRatio = new System.Windows.Forms.ComboBox();
            this.lbAspect = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSelectedCropBoxSize = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbContrast.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.bbiSave,
            this.bbiClose});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 3;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            this.ribbonControl1.Size = new System.Drawing.Size(817, 96);
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiSave
            // 
            this.bbiSave.Caption = "Save";
            this.bbiSave.Glyph = ((System.Drawing.Image)(resources.GetObject("bbiSave.Glyph")));
            this.bbiSave.Id = 1;
            this.bbiSave.Name = "bbiSave";
            this.bbiSave.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiSave_ItemClick);
            // 
            // bbiClose
            // 
            this.bbiClose.Caption = "Close";
            this.bbiClose.Glyph = ((System.Drawing.Image)(resources.GetObject("bbiClose.Glyph")));
            this.bbiClose.Id = 2;
            this.bbiClose.Name = "bbiClose";
            this.bbiClose.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.bbiClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiClose_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.bbiSave);
            this.ribbonPageGroup1.ItemLinks.Add(this.bbiClose);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            // 
            // bbiRotate
            // 
            this.bbiRotate.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.bbiRotate.ImageIndex = 6;
            this.bbiRotate.ImageList = this.imageList1;
            this.bbiRotate.Location = new System.Drawing.Point(184, 5);
            this.bbiRotate.Name = "bbiRotate";
            this.bbiRotate.Size = new System.Drawing.Size(37, 34);
            this.bbiRotate.TabIndex = 1;
            this.bbiRotate.Click += new System.EventHandler(this.bbiRotate_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.png");
            this.imageList1.Images.SetKeyName(1, "refresh.png");
            this.imageList1.Images.SetKeyName(2, "window_view.png");
            this.imageList1.Images.SetKeyName(3, "redo.png");
            this.imageList1.Images.SetKeyName(4, "redo.png");
            this.imageList1.Images.SetKeyName(5, "palette.png");
            this.imageList1.Images.SetKeyName(6, "rotateclockw16.gif");
            this.imageList1.Images.SetKeyName(7, "contrast_high.png");
            this.imageList1.Images.SetKeyName(8, "color_wheel.png");
            this.imageList1.Images.SetKeyName(9, "shape_move_backwards.png");
            this.imageList1.Images.SetKeyName(10, "picture_empty.png");
            // 
            // simpleButton3
            // 
            this.simpleButton3.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.simpleButton3.ImageIndex = 1;
            this.simpleButton3.ImageList = this.imageList1;
            this.simpleButton3.Location = new System.Drawing.Point(143, 5);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(35, 32);
            this.simpleButton3.TabIndex = 1;
            this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
            // 
            // tbContrast
            // 
            this.tbContrast.EditValue = -100;
            this.tbContrast.Location = new System.Drawing.Point(504, 5);
            this.tbContrast.MenuManager = this.ribbonControl1;
            this.tbContrast.Name = "tbContrast";
            this.tbContrast.Properties.AutoSize = false;
            this.tbContrast.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tbContrast.Properties.LabelAppearance.Options.UseTextOptions = true;
            this.tbContrast.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tbContrast.Properties.Maximum = 100;
            this.tbContrast.Properties.Minimum = -100;
            this.tbContrast.Properties.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbContrast.Size = new System.Drawing.Size(254, 30);
            this.tbContrast.TabIndex = 2;
            this.tbContrast.Value = -100;
            this.tbContrast.ValueChanged += new System.EventHandler(this.tbContrast_ValueChanged);
            // 
            // pbEdit
            // 
            this.pbEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbEdit.Location = new System.Drawing.Point(0, 96);
            this.pbEdit.Name = "pbEdit";
            this.pbEdit.Size = new System.Drawing.Size(817, 362);
            this.pbEdit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbEdit.TabIndex = 3;
            this.pbEdit.TabStop = false;
            this.pbEdit.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbEdit_MouseDown);
            this.pbEdit.MouseLeave += new System.EventHandler(this.pbEdit_MouseLeave);
            this.pbEdit.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbEdit_MouseMove);
            // 
            // lblContrast
            // 
            this.lblContrast.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContrast.Location = new System.Drawing.Point(433, 5);
            this.lblContrast.Name = "lblContrast";
            this.lblContrast.Size = new System.Drawing.Size(65, 30);
            this.lblContrast.TabIndex = 26;
            this.lblContrast.Text = "Contrast";
            this.lblContrast.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblContrastValue
            // 
            this.lblContrastValue.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContrastValue.Location = new System.Drawing.Point(764, 5);
            this.lblContrastValue.Name = "lblContrastValue";
            this.lblContrastValue.Size = new System.Drawing.Size(41, 30);
            this.lblContrastValue.TabIndex = 27;
            this.lblContrastValue.Text = "0";
            this.lblContrastValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bbiCrop
            // 
            this.bbiCrop.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.bbiCrop.Location = new System.Drawing.Point(100, 5);
            this.bbiCrop.Name = "bbiCrop";
            this.bbiCrop.Size = new System.Drawing.Size(37, 33);
            this.bbiCrop.TabIndex = 28;
            this.bbiCrop.Text = "Crop";
            this.bbiCrop.Click += new System.EventHandler(this.bbiCrop_Click);
            // 
            // rbGrayscale
            // 
            this.rbGrayscale.AutoSize = true;
            this.rbGrayscale.Location = new System.Drawing.Point(262, 5);
            this.rbGrayscale.Name = "rbGrayscale";
            this.rbGrayscale.Size = new System.Drawing.Size(72, 17);
            this.rbGrayscale.TabIndex = 30;
            this.rbGrayscale.Text = "Grayscale";
            this.rbGrayscale.UseVisualStyleBackColor = true;
            this.rbGrayscale.CheckedChanged += new System.EventHandler(this.OnCheckChangedEventHandler);
            // 
            // rbNegative
            // 
            this.rbNegative.AutoSize = true;
            this.rbNegative.Location = new System.Drawing.Point(356, 5);
            this.rbNegative.Name = "rbNegative";
            this.rbNegative.Size = new System.Drawing.Size(68, 17);
            this.rbNegative.TabIndex = 30;
            this.rbNegative.Text = "Negative";
            this.rbNegative.UseVisualStyleBackColor = true;
            this.rbNegative.CheckedChanged += new System.EventHandler(this.OnCheckChangedEventHandler);
            // 
            // rbSepia
            // 
            this.rbSepia.AutoSize = true;
            this.rbSepia.Location = new System.Drawing.Point(356, 23);
            this.rbSepia.Name = "rbSepia";
            this.rbSepia.Size = new System.Drawing.Size(78, 17);
            this.rbSepia.TabIndex = 30;
            this.rbSepia.Text = "Sepia Tone";
            this.rbSepia.UseVisualStyleBackColor = true;
            this.rbSepia.CheckedChanged += new System.EventHandler(this.OnCheckChangedEventHandler);
            // 
            // rbTransparency
            // 
            this.rbTransparency.AutoSize = true;
            this.rbTransparency.Location = new System.Drawing.Point(262, 23);
            this.rbTransparency.Name = "rbTransparency";
            this.rbTransparency.Size = new System.Drawing.Size(91, 17);
            this.rbTransparency.TabIndex = 30;
            this.rbTransparency.Text = "Transparency";
            this.rbTransparency.UseVisualStyleBackColor = true;
            this.rbTransparency.CheckedChanged += new System.EventHandler(this.OnCheckChangedEventHandler);
            // 
            // lblBrightnessValue
            // 
            this.lblBrightnessValue.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBrightnessValue.Location = new System.Drawing.Point(764, 40);
            this.lblBrightnessValue.Name = "lblBrightnessValue";
            this.lblBrightnessValue.Size = new System.Drawing.Size(41, 30);
            this.lblBrightnessValue.TabIndex = 27;
            this.lblBrightnessValue.Text = "0";
            this.lblBrightnessValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBrightness
            // 
            this.lblBrightness.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBrightness.Location = new System.Drawing.Point(430, 39);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(68, 30);
            this.lblBrightness.TabIndex = 26;
            this.lblBrightness.Text = "Brightness";
            this.lblBrightness.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbBrightness
            // 
            this.tbBrightness.EditValue = -100;
            this.tbBrightness.Location = new System.Drawing.Point(504, 40);
            this.tbBrightness.Name = "tbBrightness";
            this.tbBrightness.Properties.AutoSize = false;
            this.tbBrightness.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tbBrightness.Properties.LabelAppearance.Options.UseTextOptions = true;
            this.tbBrightness.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tbBrightness.Properties.Maximum = 100;
            this.tbBrightness.Properties.Minimum = -100;
            this.tbBrightness.Properties.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbBrightness.Size = new System.Drawing.Size(254, 30);
            this.tbBrightness.TabIndex = 2;
            this.tbBrightness.Value = -100;
            this.tbBrightness.ValueChanged += new System.EventHandler(this.tbBrightness_ValueChanged);
            // 
            // cmbSelectedAspectRatio
            // 
            this.cmbSelectedAspectRatio.FormattingEnabled = true;
            this.cmbSelectedAspectRatio.Location = new System.Drawing.Point(171, 46);
            this.cmbSelectedAspectRatio.Name = "cmbSelectedAspectRatio";
            this.cmbSelectedAspectRatio.Size = new System.Drawing.Size(122, 21);
            this.cmbSelectedAspectRatio.TabIndex = 33;
            this.cmbSelectedAspectRatio.SelectedIndexChanged += new System.EventHandler(this.cmbSelectedAspectRatio_SelectedIndexChanged);
            // 
            // lbAspect
            // 
            this.lbAspect.AutoSize = true;
            this.lbAspect.Location = new System.Drawing.Point(97, 49);
            this.lbAspect.Name = "lbAspect";
            this.lbAspect.Size = new System.Drawing.Size(68, 13);
            this.lbAspect.TabIndex = 34;
            this.lbAspect.Text = "Aspect Ratio";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(299, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Crop Box";
            // 
            // cmbSelectedCropBoxSize
            // 
            this.cmbSelectedCropBoxSize.FormattingEnabled = true;
            this.cmbSelectedCropBoxSize.Location = new System.Drawing.Point(356, 46);
            this.cmbSelectedCropBoxSize.Name = "cmbSelectedCropBoxSize";
            this.cmbSelectedCropBoxSize.Size = new System.Drawing.Size(68, 21);
            this.cmbSelectedCropBoxSize.TabIndex = 37;
            this.cmbSelectedCropBoxSize.SelectedIndexChanged += new System.EventHandler(this.cmbSelectedCropBoxSize_SelectedIndexChanged);
            // 
            // imageEditor
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.BorderColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseBorderColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 458);
            this.ControlBox = false;
            this.Controls.Add(this.cmbSelectedCropBoxSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbAspect);
            this.Controls.Add(this.cmbSelectedAspectRatio);
            this.Controls.Add(this.rbTransparency);
            this.Controls.Add(this.rbSepia);
            this.Controls.Add(this.rbNegative);
            this.Controls.Add(this.rbGrayscale);
            this.Controls.Add(this.bbiCrop);
            this.Controls.Add(this.lblBrightnessValue);
            this.Controls.Add(this.lblBrightness);
            this.Controls.Add(this.lblContrastValue);
            this.Controls.Add(this.lblContrast);
            this.Controls.Add(this.pbEdit);
            this.Controls.Add(this.tbBrightness);
            this.Controls.Add(this.tbContrast);
            this.Controls.Add(this.simpleButton3);
            this.Controls.Add(this.bbiRotate);
            this.Controls.Add(this.ribbonControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "imageEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Editor";
            this.Load += new System.EventHandler(this.imageEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbContrast.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraEditors.SimpleButton bbiRotate;
        private System.Windows.Forms.ImageList imageList1;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private DevExpress.XtraEditors.TrackBarControl tbContrast;
        private System.Windows.Forms.Label lblContrast;
        private System.Windows.Forms.Label lblContrastValue;
        private DevExpress.XtraEditors.SimpleButton bbiCrop;
        private System.Windows.Forms.RadioButton rbGrayscale;
        private System.Windows.Forms.RadioButton rbNegative;
        private System.Windows.Forms.RadioButton rbSepia;
        private System.Windows.Forms.RadioButton rbTransparency;
        private System.Windows.Forms.Label lblBrightnessValue;
        private System.Windows.Forms.Label lblBrightness;
        private DevExpress.XtraEditors.TrackBarControl tbBrightness;
        public System.Windows.Forms.PictureBox pbEdit;
        public DevExpress.XtraBars.BarButtonItem bbiSave;
        private System.Windows.Forms.ComboBox cmbSelectedAspectRatio;
        private System.Windows.Forms.Label lbAspect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSelectedCropBoxSize;

    }
}

