

namespace CNET.FrontOffice_V._7.Forms
{
    partial class frmRateDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRateDetail));
            gcolCode = new DevExpress.XtraGrid.Columns.GridColumn();
            gcolDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            gcolAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            statusStrip1 = new StatusStrip();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            rcRateDetail = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiNew = new DevExpress.XtraBars.BarButtonItem();
            bbiSave = new DevExpress.XtraBars.BarButtonItem();
            bbiDelete = new DevExpress.XtraBars.BarButtonItem();
            bsiOptions = new DevExpress.XtraBars.BarSubItem();
            barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            bbiSearch = new DevExpress.XtraBars.BarButtonItem();
            bbiSummery = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            lciRibbonHolder = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)rcRateDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciRibbonHolder).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            SuspendLayout();
            // 
            // gcolCode
            // 
            gcolCode.Caption = "Id";
            gcolCode.FieldName = "Id";
            gcolCode.Name = "gcolCode";
            gcolCode.Width = 100;
            // 
            // gcolDescription
            // 
            gcolDescription.Caption = "Description";
            gcolDescription.FieldName = "Description";
            gcolDescription.Name = "gcolDescription";
            gcolDescription.Visible = true;
            gcolDescription.VisibleIndex = 0;
            gcolDescription.Width = 466;
            // 
            // gcolAmount
            // 
            gcolAmount.Caption = "Amount";
            gcolAmount.FieldName = "amount";
            gcolAmount.Name = "gcolAmount";
            gcolAmount.Visible = true;
            gcolAmount.VisibleIndex = 1;
            gcolAmount.Width = 150;
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(statusStrip1);
            layoutControl1.Controls.Add(panelControl1);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(713, 545);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // statusStrip1
            // 
            statusStrip1.AutoSize = false;
            statusStrip1.Dock = DockStyle.None;
            statusStrip1.Location = new Point(2, 523);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(709, 20);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(rcRateDetail);
            panelControl1.Location = new Point(2, 2);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(709, 79);
            panelControl1.TabIndex = 4;
            // 
            // rcRateDetail
            // 
            rcRateDetail.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcRateDetail.ExpandCollapseItem.Id = 0;
            rcRateDetail.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcRateDetail.ExpandCollapseItem, rcRateDetail.SearchEditItem, bbiNew, bbiSave, bbiDelete, bsiOptions, barButtonItem1, barButtonItem2, bbiSearch, bbiSummery });
            rcRateDetail.Location = new Point(2, 2);
            rcRateDetail.MaxItemId = 9;
            rcRateDetail.Name = "rcRateDetail";
            rcRateDetail.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcRateDetail.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcRateDetail.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            rcRateDetail.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcRateDetail.Size = new Size(705, 66);
            rcRateDetail.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiNew
            // 
            bbiNew.Caption = "New";
            bbiNew.Id = 1;
            bbiNew.ImageOptions.Image = (Image)resources.GetObject("bbiNew.ImageOptions.Image");
            bbiNew.Name = "bbiNew";
            bbiNew.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bbiSave
            // 
            bbiSave.Caption = "Save";
            bbiSave.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSave.Id = 2;
            bbiSave.ImageOptions.Image = (Image)resources.GetObject("bbiSave.ImageOptions.Image");
            bbiSave.Name = "bbiSave";
            bbiSave.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bbiDelete
            // 
            bbiDelete.Caption = "Delete";
            bbiDelete.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiDelete.Id = 3;
            bbiDelete.ImageOptions.Image = (Image)resources.GetObject("bbiDelete.ImageOptions.Image");
            bbiDelete.Name = "bbiDelete";
            bbiDelete.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bsiOptions
            // 
            bsiOptions.Caption = "Options";
            bsiOptions.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bsiOptions.Id = 4;
            bsiOptions.ImageOptions.Image = (Image)resources.GetObject("bsiOptions.ImageOptions.Image");
            bsiOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem1), new DevExpress.XtraBars.LinkPersistInfo(barButtonItem2) });
            bsiOptions.MenuDrawMode = DevExpress.XtraBars.MenuDrawMode.LargeImagesTextDescription;
            bsiOptions.Name = "bsiOptions";
            bsiOptions.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // barButtonItem1
            // 
            barButtonItem1.Caption = "Option 1";
            barButtonItem1.Description = "Option 1 Description";
            barButtonItem1.Id = 5;
            barButtonItem1.ImageOptions.Image = (Image)resources.GetObject("barButtonItem1.ImageOptions.Image");
            barButtonItem1.Name = "barButtonItem1";
            // 
            // barButtonItem2
            // 
            barButtonItem2.Caption = "Option 2";
            barButtonItem2.Description = "Option 2 Description";
            barButtonItem2.Id = 6;
            barButtonItem2.ImageOptions.Image = (Image)resources.GetObject("barButtonItem2.ImageOptions.Image");
            barButtonItem2.Name = "barButtonItem2";
            // 
            // bbiSearch
            // 
            bbiSearch.Caption = "Search";
            bbiSearch.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSearch.Id = 7;
            bbiSearch.ImageOptions.Image = (Image)resources.GetObject("bbiSearch.ImageOptions.Image");
            bbiSearch.Name = "bbiSearch";
            bbiSearch.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiSearch.ItemClick += bbiSearch_ItemClick;
            // 
            // bbiSummery
            // 
            bbiSummery.Caption = "Summery";
            bbiSummery.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSummery.Id = 8;
            bbiSummery.ImageOptions.Image = (Image)resources.GetObject("bbiSummery.ImageOptions.Image");
            bbiSummery.Name = "bbiSummery";
            bbiSummery.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiSummery.ItemClick += bbiSummery_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, ribbonPageGroup2, ribbonPageGroup3, ribbonPageGroup4, ribbonPageGroup5 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(bbiNew);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "New";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(bbiSave);
            ribbonPageGroup2.ItemLinks.Add(bbiDelete);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            ribbonPageGroup2.Text = "Save and Delete";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.ItemLinks.Add(bsiOptions);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            ribbonPageGroup3.Text = "Options";
            // 
            // ribbonPageGroup4
            // 
            ribbonPageGroup4.ItemLinks.Add(bbiSearch);
            ribbonPageGroup4.Name = "ribbonPageGroup4";
            ribbonPageGroup4.Text = "Search";
            // 
            // ribbonPageGroup5
            // 
            ribbonPageGroup5.ItemLinks.Add(bbiSummery);
            ribbonPageGroup5.Name = "ribbonPageGroup5";
            ribbonPageGroup5.Text = "Summery";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciRibbonHolder, layoutControlItem2, layoutControlItem1 });
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup1.Size = new Size(713, 545);
            layoutControlGroup1.TextVisible = false;
            // 
            // lciRibbonHolder
            // 
            lciRibbonHolder.Control = panelControl1;
            lciRibbonHolder.CustomizationFormText = "layoutControlItem1";
            lciRibbonHolder.Location = new Point(0, 0);
            lciRibbonHolder.Name = "lciRibbonHolder";
            lciRibbonHolder.Size = new Size(713, 83);
            lciRibbonHolder.TextSize = new Size(0, 0);
            lciRibbonHolder.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            layoutControlItem2.Location = new Point(0, 83);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(713, 438);
            layoutControlItem2.TextSize = new Size(0, 0);
            layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = statusStrip1;
            layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            layoutControlItem1.Location = new Point(0, 521);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(713, 24);
            layoutControlItem1.TextSize = new Size(0, 0);
            layoutControlItem1.TextVisible = false;
            // 
            // frmRateDetail
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(713, 545);
            Controls.Add(layoutControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmRateDetail.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmRateDetail";
            Text = "Rate Detail";
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)rcRateDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciRibbonHolder).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcRateDetail;
        private DevExpress.XtraBars.BarButtonItem bbiNew;
        private DevExpress.XtraBars.BarButtonItem bbiSave;
        private DevExpress.XtraBars.BarButtonItem bbiDelete;
        private DevExpress.XtraBars.BarSubItem bsiOptions;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciRibbonHolder;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem bbiSearch;
        private DevExpress.XtraBars.BarButtonItem bbiSummery;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcolDescription;
        private DevExpress.XtraGrid.Columns.GridColumn gcolAmount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private StatusStrip statusStrip1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}