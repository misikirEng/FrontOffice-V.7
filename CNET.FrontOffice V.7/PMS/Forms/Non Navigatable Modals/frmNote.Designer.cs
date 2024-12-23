namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    partial class frmNote
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNote));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            statusStrip1 = new StatusStrip();
            rtbNote = new RichTextBox();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            rcNote = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiClear = new DevExpress.XtraBars.BarButtonItem();
            bbiSave = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            lciRibbonHolder = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)rcNote).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciRibbonHolder).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(statusStrip1);
            layoutControl1.Controls.Add(rtbNote);
            layoutControl1.Controls.Add(panelControl1);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(259, 152, 250, 350);
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(752, 405);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // statusStrip1
            // 
            statusStrip1.AutoSize = false;
            statusStrip1.Dock = DockStyle.None;
            statusStrip1.Location = new Point(12, 373);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(728, 20);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // rtbNote
            // 
            rtbNote.BorderStyle = BorderStyle.None;
            rtbNote.Location = new Point(12, 88);
            rtbNote.Name = "rtbNote";
            rtbNote.Size = new Size(728, 281);
            rtbNote.TabIndex = 5;
            rtbNote.Text = "";
            rtbNote.TextChanged += rtbNote_TextChanged;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(rcNote);
            panelControl1.Location = new Point(2, 2);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(748, 70);
            panelControl1.TabIndex = 4;
            // 
            // rcNote
            // 
            rcNote.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcNote.ExpandCollapseItem.Id = 0;
            rcNote.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcNote.ExpandCollapseItem, rcNote.SearchEditItem, bbiClear, bbiSave, bbiClose });
            rcNote.Location = new Point(2, 2);
            rcNote.MaxItemId = 4;
            rcNote.Name = "rcNote";
            rcNote.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcNote.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcNote.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcNote.Size = new Size(744, 66);
            rcNote.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiClear
            // 
            bbiClear.Caption = "Clear";
            bbiClear.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiClear.Id = 1;
            bbiClear.ImageOptions.Image = (Image)resources.GetObject("bbiClear.ImageOptions.Image");
            bbiClear.ImageOptions.LargeImage = (Image)resources.GetObject("bbiClear.ImageOptions.LargeImage");
            bbiClear.Name = "bbiClear";
            bbiClear.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiClear.ItemClick += bbiClear_ItemClick;
            // 
            // bbiSave
            // 
            bbiSave.Caption = "Save";
            bbiSave.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSave.Id = 2;
            bbiSave.ImageOptions.Image = (Image)resources.GetObject("bbiSave.ImageOptions.Image");
            bbiSave.ImageOptions.LargeImage = (Image)resources.GetObject("bbiSave.ImageOptions.LargeImage");
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
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, ribbonPageGroup2, ribbonPageGroup3 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(bbiClear);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Clear";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(bbiSave);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            ribbonPageGroup2.Text = "Save";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.ItemLinks.Add(bbiClose);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            ribbonPageGroup3.Text = "Close";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciRibbonHolder, layoutControlGroup2 });
            layoutControlGroup1.Name = "Root";
            layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup1.Size = new Size(752, 405);
            layoutControlGroup1.TextVisible = false;
            // 
            // lciRibbonHolder
            // 
            lciRibbonHolder.Control = panelControl1;
            lciRibbonHolder.CustomizationFormText = "lciRibbonHolder";
            lciRibbonHolder.Location = new Point(0, 0);
            lciRibbonHolder.MaxSize = new Size(0, 74);
            lciRibbonHolder.MinSize = new Size(5, 74);
            lciRibbonHolder.Name = "lciRibbonHolder";
            lciRibbonHolder.Size = new Size(752, 74);
            lciRibbonHolder.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            lciRibbonHolder.TextSize = new Size(0, 0);
            lciRibbonHolder.TextVisible = false;
            // 
            // layoutControlGroup2
            // 
            layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
            layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem2, layoutControlItem1 });
            layoutControlGroup2.Location = new Point(0, 74);
            layoutControlGroup2.Name = "layoutControlGroup2";
            layoutControlGroup2.Size = new Size(752, 331);
            layoutControlGroup2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 2, 0);
            layoutControlGroup2.TextLocation = DevExpress.Utils.Locations.Bottom;
            layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = rtbNote;
            layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            layoutControlItem2.Location = new Point(0, 0);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(732, 285);
            layoutControlItem2.TextSize = new Size(0, 0);
            layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = statusStrip1;
            layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            layoutControlItem1.Location = new Point(0, 285);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(732, 24);
            layoutControlItem1.TextSize = new Size(0, 0);
            layoutControlItem1.TextVisible = false;
            // 
            // frmNote
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(752, 405);
            Controls.Add(layoutControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmNote.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmNote";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Note";
            Load += frmNote_Load;
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)rcNote).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciRibbonHolder).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion


        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcNote;
        private DevExpress.XtraBars.BarButtonItem bbiClear;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciRibbonHolder;
        private RichTextBox rtbNote;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        public DevExpress.XtraBars.BarButtonItem bbiSave;
        public DevExpress.XtraBars.BarButtonItem bbiClose;
        private StatusStrip statusStrip1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}