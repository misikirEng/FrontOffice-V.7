namespace CNET.FrontOffice_V._7.Group_Registration
{
    partial class frmNewGuest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNewGuest));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiSave = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            lukNationality = new DevExpress.XtraEditors.LookUpEdit();
            lukGender = new DevExpress.XtraEditors.LookUpEdit();
            teTelephone = new DevExpress.XtraEditors.TextEdit();
            teIdNumber = new DevExpress.XtraEditors.TextEdit();
            teName = new DevExpress.XtraEditors.TextEdit();
            layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl2).BeginInit();
            layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lukNationality.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lukGender.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)teTelephone.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)teIdNumber.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)teName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(ribbonControl1);
            layoutControl1.Dock = DockStyle.Top;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(328, 82);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // ribbonControl1
            // 
            ribbonControl1.Dock = DockStyle.None;
            ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, bbiSave, bbiClose, ribbonControl1.SearchEditItem });
            ribbonControl1.Location = new Point(12, 12);
            ribbonControl1.MaxItemId = 3;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.OptionsPageCategories.ShowCaptions = false;
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            ribbonControl1.ShowToolbarCustomizeItem = false;
            ribbonControl1.Size = new Size(304, 66);
            ribbonControl1.Toolbar.ShowCustomizeItem = false;
            ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiSave
            // 
            bbiSave.Caption = "Save";
            bbiSave.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSave.Id = 1;
            bbiSave.ImageOptions.Image = (Image)resources.GetObject("bbiSave.ImageOptions.Image");
            bbiSave.ImageOptions.LargeImage = (Image)resources.GetObject("bbiSave.ImageOptions.LargeImage");
            bbiSave.Name = "bbiSave";
            bbiSave.ItemClick += bbiSave_ItemClick;
            // 
            // bbiClose
            // 
            bbiClose.Caption = "Close";
            bbiClose.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiClose.Id = 2;
            bbiClose.ImageOptions.Image = (Image)resources.GetObject("bbiClose.ImageOptions.Image");
            bbiClose.ImageOptions.LargeImage = (Image)resources.GetObject("bbiClose.ImageOptions.LargeImage");
            bbiClose.Name = "bbiClose";
            bbiClose.ItemClick += bbiClose_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, ribbonPageGroup2 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(bbiSave);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup2.ItemLinks.Add(bbiClose);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1 });
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(328, 82);
            layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = ribbonControl1;
            layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            layoutControlItem1.Location = new Point(0, 0);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(308, 62);
            layoutControlItem1.TextSize = new Size(0, 0);
            layoutControlItem1.TextVisible = false;
            // 
            // layoutControl2
            // 
            layoutControl2.Controls.Add(lukNationality);
            layoutControl2.Controls.Add(lukGender);
            layoutControl2.Controls.Add(teTelephone);
            layoutControl2.Controls.Add(teIdNumber);
            layoutControl2.Controls.Add(teName);
            layoutControl2.Dock = DockStyle.Fill;
            layoutControl2.Location = new Point(0, 82);
            layoutControl2.Name = "layoutControl2";
            layoutControl2.Root = layoutControlGroup2;
            layoutControl2.Size = new Size(328, 206);
            layoutControl2.TabIndex = 1;
            layoutControl2.Text = "layoutControl2";
            // 
            // lukNationality
            // 
            lukNationality.Location = new Point(84, 68);
            lukNationality.MenuManager = ribbonControl1;
            lukNationality.Name = "lukNationality";
            lukNationality.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            lukNationality.Properties.NullText = "";
            lukNationality.Size = new Size(232, 20);
            lukNationality.StyleController = layoutControl2;
            lukNationality.TabIndex = 11;
            // 
            // lukGender
            // 
            lukGender.Location = new Point(86, 42);
            lukGender.MenuManager = ribbonControl1;
            lukGender.Name = "lukGender";
            lukGender.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            lukGender.Properties.NullText = "";
            lukGender.Size = new Size(228, 20);
            lukGender.StyleController = layoutControl2;
            lukGender.TabIndex = 10;
            // 
            // teTelephone
            // 
            teTelephone.Location = new Point(86, 122);
            teTelephone.MenuManager = ribbonControl1;
            teTelephone.Name = "teTelephone";
            teTelephone.Properties.Mask.EditMask = "((\\+\\d{3}-)?|\\({0,1}(\\d{3})\\)-)?(\\d{3}-\\d{6})";
            teTelephone.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            teTelephone.Size = new Size(228, 20);
            teTelephone.StyleController = layoutControl2;
            teTelephone.TabIndex = 8;
            // 
            // teIdNumber
            // 
            teIdNumber.Location = new Point(86, 94);
            teIdNumber.MenuManager = ribbonControl1;
            teIdNumber.Name = "teIdNumber";
            teIdNumber.Size = new Size(228, 20);
            teIdNumber.StyleController = layoutControl2;
            teIdNumber.TabIndex = 7;
            // 
            // teName
            // 
            teName.Location = new Point(86, 14);
            teName.MenuManager = ribbonControl1;
            teName.Name = "teName";
            teName.Size = new Size(228, 20);
            teName.StyleController = layoutControl2;
            teName.TabIndex = 4;
            // 
            // layoutControlGroup2
            // 
            layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
            layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup2.GroupBordersVisible = false;
            layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem2, layoutControlItem5, layoutControlItem6, emptySpaceItem1, layoutControlItem8, layoutControlItem3 });
            layoutControlGroup2.Name = "layoutControlGroup2";
            layoutControlGroup2.Size = new Size(328, 206);
            layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem2.Control = teName;
            layoutControlItem2.CustomizationFormText = "Fist Name";
            layoutControlItem2.Location = new Point(0, 0);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(308, 28);
            layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem2.Text = "Full Name *";
            layoutControlItem2.TextSize = new Size(60, 13);
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem5.Control = teIdNumber;
            layoutControlItem5.CustomizationFormText = "ID Number";
            layoutControlItem5.Location = new Point(0, 80);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Size = new Size(308, 28);
            layoutControlItem5.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem5.Text = "ID Number *";
            layoutControlItem5.TextSize = new Size(60, 13);
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem6.Control = teTelephone;
            layoutControlItem6.CustomizationFormText = "Telephone";
            layoutControlItem6.Location = new Point(0, 108);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new Size(308, 28);
            layoutControlItem6.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem6.Text = "Telephone";
            layoutControlItem6.TextSize = new Size(60, 13);
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            emptySpaceItem1.Location = new Point(0, 136);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(308, 50);
            emptySpaceItem1.TextSize = new Size(0, 0);
            // 
            // layoutControlItem8
            // 
            layoutControlItem8.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem8.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem8.Control = lukGender;
            layoutControlItem8.CustomizationFormText = "Gender";
            layoutControlItem8.Location = new Point(0, 28);
            layoutControlItem8.Name = "layoutControlItem8";
            layoutControlItem8.Size = new Size(308, 28);
            layoutControlItem8.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem8.Text = "Gender *";
            layoutControlItem8.TextSize = new Size(60, 13);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem3.Control = lukNationality;
            layoutControlItem3.CustomizationFormText = "Nationality *";
            layoutControlItem3.Location = new Point(0, 56);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(308, 24);
            layoutControlItem3.Text = "Nationality *";
            layoutControlItem3.TextSize = new Size(60, 13);
            // 
            // frmNewGuest
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(328, 288);
            ControlBox = false;
            Controls.Add(layoutControl2);
            Controls.Add(layoutControl1);
            IconOptions.ShowIcon = false;
            Name = "frmNewGuest";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "New Guest";
            TopMost = true;
            Load += frmNewGuest_Load;
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            layoutControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl2).EndInit();
            layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)lukNationality.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)lukGender.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)teTelephone.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)teIdNumber.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)teName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem8).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem bbiSave;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraEditors.TextEdit teTelephone;
        private DevExpress.XtraEditors.TextEdit teIdNumber;
        private DevExpress.XtraEditors.TextEdit teName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.LookUpEdit lukGender;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.LookUpEdit lukNationality;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
    }
}