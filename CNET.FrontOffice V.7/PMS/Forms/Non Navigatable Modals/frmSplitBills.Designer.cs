namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    partial class frmSplitBills
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSplitBills));
            rcNightAudit = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiNew = new DevExpress.XtraBars.BarButtonItem();
            bbiSave = new DevExpress.XtraBars.BarButtonItem();
            bbiBreakItem = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            bbiFolio = new DevExpress.XtraBars.BarButtonItem();
            bbiCheckOut = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgBreak = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgCancel = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repositoryItemCheckEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repositoryItemCheckEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)rcNightAudit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit4).BeginInit();
            xtraScrollableControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            SuspendLayout();
            // 
            // rcNightAudit
            // 
            rcNightAudit.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcNightAudit.ExpandCollapseItem.Id = 0;
            rcNightAudit.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcNightAudit.ExpandCollapseItem, rcNightAudit.SearchEditItem, bbiNew, bbiSave, bbiBreakItem, bbiClose, bbiFolio, bbiCheckOut });
            rcNightAudit.Location = new Point(0, 0);
            rcNightAudit.MaxItemId = 56;
            rcNightAudit.Name = "rcNightAudit";
            rcNightAudit.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcNightAudit.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repositoryItemCheckEdit1, repositoryItemCheckEdit2, repositoryItemCheckEdit3, repositoryItemCheckEdit4 });
            rcNightAudit.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
            rcNightAudit.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            rcNightAudit.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.ShowOnMultiplePages;
            rcNightAudit.Size = new Size(934, 83);
            rcNightAudit.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiNew
            // 
            bbiNew.Caption = "Add Window";
            bbiNew.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiNew.Id = 1;
            bbiNew.ImageOptions.Image = (Image)resources.GetObject("bbiNew.ImageOptions.Image");
            bbiNew.ImageOptions.LargeImage = (Image)resources.GetObject("bbiNew.ImageOptions.LargeImage");
            bbiNew.Name = "bbiNew";
            bbiNew.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiNew.ItemClick += bbiNew_ItemClick;
            // 
            // bbiSave
            // 
            bbiSave.Caption = "Save";
            bbiSave.Id = 49;
            bbiSave.ImageOptions.Image = (Image)resources.GetObject("bbiSave.ImageOptions.Image");
            bbiSave.ImageOptions.LargeImage = (Image)resources.GetObject("bbiSave.ImageOptions.LargeImage");
            bbiSave.Name = "bbiSave";
            bbiSave.ItemClick += bbiSave_ItemClick;
            // 
            // bbiBreakItem
            // 
            bbiBreakItem.Caption = "Break Item";
            bbiBreakItem.Id = 52;
            bbiBreakItem.ImageOptions.Image = (Image)resources.GetObject("bbiBreakItem.ImageOptions.Image");
            bbiBreakItem.ImageOptions.LargeImage = (Image)resources.GetObject("bbiBreakItem.ImageOptions.LargeImage");
            bbiBreakItem.Name = "bbiBreakItem";
            bbiBreakItem.ItemClick += bbiBreakItem_ItemClick;
            // 
            // bbiClose
            // 
            bbiClose.Caption = "Close";
            bbiClose.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiClose.Id = 53;
            bbiClose.ImageOptions.Image = (Image)resources.GetObject("bbiClose.ImageOptions.Image");
            bbiClose.ImageOptions.LargeImage = (Image)resources.GetObject("bbiClose.ImageOptions.LargeImage");
            bbiClose.Name = "bbiClose";
            bbiClose.ItemClick += bbiCancel_ItemClick;
            // 
            // bbiFolio
            // 
            bbiFolio.Caption = "View Folio";
            bbiFolio.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiFolio.Id = 54;
            bbiFolio.ImageOptions.Image = (Image)resources.GetObject("bbiFolio.ImageOptions.Image");
            bbiFolio.ImageOptions.LargeImage = (Image)resources.GetObject("bbiFolio.ImageOptions.LargeImage");
            bbiFolio.Name = "bbiFolio";
            bbiFolio.ItemClick += bbiFolio_ItemClick;
            // 
            // bbiCheckOut
            // 
            bbiCheckOut.Caption = "Check Out";
            bbiCheckOut.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiCheckOut.Id = 55;
            bbiCheckOut.ImageOptions.Image = (Image)resources.GetObject("bbiCheckOut.ImageOptions.Image");
            bbiCheckOut.ImageOptions.LargeImage = (Image)resources.GetObject("bbiCheckOut.ImageOptions.LargeImage");
            bbiCheckOut.Name = "bbiCheckOut";
            bbiCheckOut.ItemClick += bbiCheckOut_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup2, ribbonPageGroup4, rpgBreak, ribbonPageGroup1, ribbonPageGroup3, rpgCancel });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.AllowTextClipping = false;
            ribbonPageGroup2.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup2.ImageOptions.Image = (Image)resources.GetObject("ribbonPageGroup2.ImageOptions.Image");
            ribbonPageGroup2.ItemLinks.Add(bbiNew);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // ribbonPageGroup4
            // 
            ribbonPageGroup4.AllowTextClipping = false;
            ribbonPageGroup4.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup4.ItemLinks.Add(bbiSave);
            ribbonPageGroup4.Name = "ribbonPageGroup4";
            // 
            // rpgBreak
            // 
            rpgBreak.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgBreak.ItemLinks.Add(bbiBreakItem);
            rpgBreak.Name = "rpgBreak";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(bbiFolio);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup3.ItemLinks.Add(bbiCheckOut);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            // 
            // rpgCancel
            // 
            rpgCancel.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgCancel.ItemLinks.Add(bbiClose);
            rpgCancel.Name = "rpgCancel";
            // 
            // repositoryItemCheckEdit1
            // 
            repositoryItemCheckEdit1.AutoHeight = false;
            repositoryItemCheckEdit1.Caption = "Check";
            repositoryItemCheckEdit1.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // repositoryItemCheckEdit2
            // 
            repositoryItemCheckEdit2.AutoHeight = false;
            repositoryItemCheckEdit2.Caption = "Check";
            repositoryItemCheckEdit2.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            // 
            // repositoryItemCheckEdit3
            // 
            repositoryItemCheckEdit3.AutoHeight = false;
            repositoryItemCheckEdit3.Caption = "Check";
            repositoryItemCheckEdit3.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            repositoryItemCheckEdit3.Name = "repositoryItemCheckEdit3";
            // 
            // repositoryItemCheckEdit4
            // 
            repositoryItemCheckEdit4.AutoHeight = false;
            repositoryItemCheckEdit4.Caption = "Check";
            repositoryItemCheckEdit4.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            repositoryItemCheckEdit4.Name = "repositoryItemCheckEdit4";
            // 
            // xtraScrollableControl1
            // 
            xtraScrollableControl1.AutoScrollMinSize = new Size(100, 1000);
            xtraScrollableControl1.Controls.Add(layoutControl1);
            xtraScrollableControl1.Dock = DockStyle.Fill;
            xtraScrollableControl1.Location = new Point(0, 83);
            xtraScrollableControl1.Name = "xtraScrollableControl1";
            xtraScrollableControl1.Size = new Size(934, 452);
            xtraScrollableControl1.TabIndex = 4;
            // 
            // layoutControl1
            // 
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(54, 248, 250, 350);
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(917, 1000);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.AppearanceGroup.BackColor = Color.Transparent;
            layoutControlGroup1.AppearanceGroup.Options.UseBackColor = true;
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(917, 1000);
            layoutControlGroup1.TextVisible = false;
            // 
            // frmSplitBills
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            AutoScrollMargin = new Size(5, 10);
            AutoScrollMinSize = new Size(40, 50);
            ClientSize = new Size(934, 535);
            Controls.Add(xtraScrollableControl1);
            Controls.Add(rcNightAudit);
            IconOptions.Icon = (Icon)resources.GetObject("frmSplitBills.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            Name = "frmSplitBills";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bill Split";
            Load += SplitBills_Load;
            ((System.ComponentModel.ISupportInitialize)rcNightAudit).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit2).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit3).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit4).EndInit();
            xtraScrollableControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl rcNightAudit;
        private DevExpress.XtraBars.BarButtonItem bbiNew;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgCancel;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit3;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit4;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.BarButtonItem bbiSave;
        private DevExpress.XtraBars.BarButtonItem bbiBreakItem;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgBreak;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.BarButtonItem bbiFolio;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem bbiCheckOut;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
    }
}