namespace CNET.FrontOffice_V._7.HouseKeeping
{
    partial class frmTaskSheetGrid
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTaskSheetGrid));
            rcNightAudit = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiNew = new DevExpress.XtraBars.BarButtonItem();
            bbiSave = new DevExpress.XtraBars.BarButtonItem();
            bbiAmendDate = new DevExpress.XtraBars.BarButtonItem();
            bbiCheckOut = new DevExpress.XtraBars.BarButtonItem();
            bbiCheckIn = new DevExpress.XtraBars.BarButtonItem();
            bbiCancel = new DevExpress.XtraBars.BarButtonItem();
            bbiPrint = new DevExpress.XtraBars.BarButtonItem();
            bbiPush = new DevExpress.XtraBars.BarButtonItem();
            bbiVoid = new DevExpress.XtraBars.BarButtonItem();
            bbiPostAll = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            barSubItem1 = new DevExpress.XtraBars.BarSubItem();
            barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
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
            rcNightAudit.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcNightAudit.ExpandCollapseItem, rcNightAudit.SearchEditItem, bbiNew, bbiSave, bbiAmendDate, bbiCheckOut, bbiCheckIn, bbiCancel, bbiPrint, bbiPush, bbiVoid, bbiPostAll, barButtonItem1, barButtonItem2, barSubItem1, barButtonItem3, barButtonItem4 });
            rcNightAudit.Location = new Point(0, 0);
            rcNightAudit.MaxItemId = 49;
            rcNightAudit.Name = "rcNightAudit";
            rcNightAudit.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcNightAudit.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repositoryItemCheckEdit1, repositoryItemCheckEdit2, repositoryItemCheckEdit3, repositoryItemCheckEdit4 });
            rcNightAudit.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcNightAudit.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            rcNightAudit.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.ShowOnMultiplePages;
            rcNightAudit.Size = new Size(929, 66);
            rcNightAudit.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiNew
            // 
            bbiNew.Caption = "Add";
            bbiNew.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiNew.Id = 1;
            bbiNew.ImageOptions.Image = (Image)resources.GetObject("bbiNew.ImageOptions.Image");
            bbiNew.ImageOptions.LargeImage = (Image)resources.GetObject("bbiNew.ImageOptions.LargeImage");
            bbiNew.Name = "bbiNew";
            bbiNew.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiNew.ItemClick += AddEmployeeClick;
            // 
            // bbiSave
            // 
            bbiSave.Caption = "Save";
            bbiSave.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSave.Id = 2;
            bbiSave.ImageOptions.Image = (Image)resources.GetObject("bbiSave.ImageOptions.Image");
            bbiSave.Name = "bbiSave";
            bbiSave.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiSave.ItemClick += SaveNewTask;
            // 
            // bbiAmendDate
            // 
            bbiAmendDate.Caption = "Amend Date";
            bbiAmendDate.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiAmendDate.Id = 6;
            bbiAmendDate.ImageOptions.Image = (Image)resources.GetObject("bbiAmendDate.ImageOptions.Image");
            bbiAmendDate.ImageOptions.LargeImage = (Image)resources.GetObject("bbiAmendDate.ImageOptions.LargeImage");
            bbiAmendDate.Name = "bbiAmendDate";
            // 
            // bbiCheckOut
            // 
            bbiCheckOut.Caption = "Check Out";
            bbiCheckOut.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiCheckOut.Id = 7;
            bbiCheckOut.ImageOptions.Image = (Image)resources.GetObject("bbiCheckOut.ImageOptions.Image");
            bbiCheckOut.ImageOptions.LargeImage = (Image)resources.GetObject("bbiCheckOut.ImageOptions.LargeImage");
            bbiCheckOut.Name = "bbiCheckOut";
            // 
            // bbiCheckIn
            // 
            bbiCheckIn.Caption = "Check In";
            bbiCheckIn.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiCheckIn.Id = 8;
            bbiCheckIn.ImageOptions.Image = (Image)resources.GetObject("bbiCheckIn.ImageOptions.Image");
            bbiCheckIn.ImageOptions.LargeImage = (Image)resources.GetObject("bbiCheckIn.ImageOptions.LargeImage");
            bbiCheckIn.Name = "bbiCheckIn";
            // 
            // bbiCancel
            // 
            bbiCancel.Caption = "Cancel";
            bbiCancel.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiCancel.Id = 9;
            bbiCancel.ImageOptions.Image = (Image)resources.GetObject("bbiCancel.ImageOptions.Image");
            bbiCancel.ImageOptions.LargeImage = (Image)resources.GetObject("bbiCancel.ImageOptions.LargeImage");
            bbiCancel.Name = "bbiCancel";
            bbiCancel.ItemClick += bbiCancel_ItemClick;
            // 
            // bbiPrint
            // 
            bbiPrint.Caption = "Print";
            bbiPrint.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiPrint.Id = 10;
            bbiPrint.ImageOptions.Image = (Image)resources.GetObject("bbiPrint.ImageOptions.Image");
            bbiPrint.ImageOptions.LargeImage = (Image)resources.GetObject("bbiPrint.ImageOptions.LargeImage");
            bbiPrint.Name = "bbiPrint";
            // 
            // bbiPush
            // 
            bbiPush.Caption = "Push";
            bbiPush.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiPush.Id = 11;
            bbiPush.ImageOptions.Image = (Image)resources.GetObject("bbiPush.ImageOptions.Image");
            bbiPush.ImageOptions.LargeImage = (Image)resources.GetObject("bbiPush.ImageOptions.LargeImage");
            bbiPush.Name = "bbiPush";
            // 
            // bbiVoid
            // 
            bbiVoid.Caption = "Void";
            bbiVoid.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiVoid.Id = 12;
            bbiVoid.ImageOptions.Image = (Image)resources.GetObject("bbiVoid.ImageOptions.Image");
            bbiVoid.ImageOptions.LargeImage = (Image)resources.GetObject("bbiVoid.ImageOptions.LargeImage");
            bbiVoid.Name = "bbiVoid";
            // 
            // bbiPostAll
            // 
            bbiPostAll.Caption = "Post All";
            bbiPostAll.Id = 14;
            bbiPostAll.Name = "bbiPostAll";
            // 
            // barButtonItem1
            // 
            barButtonItem1.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            barButtonItem1.Caption = "Print";
            barButtonItem1.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            barButtonItem1.Id = 35;
            barButtonItem1.ImageOptions.Image = (Image)resources.GetObject("barButtonItem1.ImageOptions.Image");
            barButtonItem1.ImageOptions.LargeImage = (Image)resources.GetObject("barButtonItem1.ImageOptions.LargeImage");
            barButtonItem1.Name = "barButtonItem1";
            // 
            // barButtonItem2
            // 
            barButtonItem2.Caption = "Print ";
            barButtonItem2.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            barButtonItem2.Id = 44;
            barButtonItem2.ImageOptions.Image = (Image)resources.GetObject("barButtonItem2.ImageOptions.Image");
            barButtonItem2.ImageOptions.LargeImage = (Image)resources.GetObject("barButtonItem2.ImageOptions.LargeImage");
            barButtonItem2.Name = "barButtonItem2";
            // 
            // barSubItem1
            // 
            barSubItem1.Caption = "Print";
            barSubItem1.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            barSubItem1.Id = 45;
            barSubItem1.ImageOptions.Image = (Image)resources.GetObject("barSubItem1.ImageOptions.Image");
            barSubItem1.ImageOptions.LargeImage = (Image)resources.GetObject("barSubItem1.ImageOptions.LargeImage");
            barSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem3), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem4) });
            barSubItem1.Name = "barSubItem1";
            // 
            // barButtonItem3
            // 
            barButtonItem3.Caption = "Print Selected";
            barButtonItem3.Id = 46;
            barButtonItem3.Name = "barButtonItem3";
            barButtonItem3.ItemClick += PrintSelected;
            // 
            // barButtonItem4
            // 
            barButtonItem4.Caption = "Print All";
            barButtonItem4.Id = 47;
            barButtonItem4.Name = "barButtonItem4";
            barButtonItem4.ItemClick += PrintAll;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup2, ribbonPageGroup3, ribbonPageGroup4, rpgCancel });
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
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.AllowTextClipping = false;
            ribbonPageGroup3.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup3.ItemLinks.Add(bbiSave);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            // 
            // ribbonPageGroup4
            // 
            ribbonPageGroup4.AllowTextClipping = false;
            ribbonPageGroup4.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup4.ItemLinks.Add(barSubItem1);
            ribbonPageGroup4.Name = "ribbonPageGroup4";
            // 
            // rpgCancel
            // 
            rpgCancel.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgCancel.ItemLinks.Add(bbiCancel);
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
            xtraScrollableControl1.Location = new Point(0, 66);
            xtraScrollableControl1.Name = "xtraScrollableControl1";
            xtraScrollableControl1.Size = new Size(929, 595);
            xtraScrollableControl1.TabIndex = 4;
            // 
            // layoutControl1
            // 
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(54, 248, 250, 350);
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(912, 1000);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(912, 1000);
            layoutControlGroup1.TextVisible = false;
            // 
            // frmTaskSheetGrid
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            AutoScrollMargin = new Size(5, 10);
            AutoScrollMinSize = new Size(40, 50);
            ClientSize = new Size(929, 661);
            Controls.Add(xtraScrollableControl1);
            Controls.Add(rcNightAudit);
            IconOptions.Icon = (Icon)resources.GetObject("frmTaskSheetGrid.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            Name = "frmTaskSheetGrid";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Task Sheet Grid";
            Load += frmTaskSheetGrid_Load;
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
        private DevExpress.XtraBars.BarButtonItem bbiSave;
        private DevExpress.XtraBars.BarButtonItem bbiAmendDate;
        private DevExpress.XtraBars.BarButtonItem bbiCheckOut;
        private DevExpress.XtraBars.BarButtonItem bbiCheckIn;
        private DevExpress.XtraBars.BarButtonItem bbiCancel;
        private DevExpress.XtraBars.BarButtonItem bbiPrint;
        private DevExpress.XtraBars.BarButtonItem bbiPush;
        private DevExpress.XtraBars.BarButtonItem bbiVoid;
        private DevExpress.XtraBars.BarButtonItem bbiPostAll;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgCancel;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit3;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit4;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.BarSubItem barSubItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraBars.BarButtonItem barButtonItem4;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
    }
}