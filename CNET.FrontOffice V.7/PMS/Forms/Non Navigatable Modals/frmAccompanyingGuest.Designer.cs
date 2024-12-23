using CNET.ERP.ResourceProvider;


namespace CNET.FrontOffice_V._7.Non_Navigatable_Modals
{
    partial class frmAccompanyingGuest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAccompanyingGuest));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            rcAccompanyGuest = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiNew = new DevExpress.XtraBars.BarButtonItem();
            bbiSave = new DevExpress.XtraBars.BarButtonItem();
            bbiDelete = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            bbiIssueCard = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            gcAccGuest = new DevExpress.XtraGrid.GridControl();
            gv_accGuest = new DevExpress.XtraGrid.Views.Grid.GridView();
            gCol_guest = new DevExpress.XtraGrid.Columns.GridColumn();
            risle_accGuest = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            gCol_address = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            lc_gcAccGuest = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)rcAccompanyGuest).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gcAccGuest).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gv_accGuest).BeginInit();
            ((System.ComponentModel.ISupportInitialize)risle_accGuest).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lc_gcAccGuest).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(rcAccompanyGuest);
            layoutControl1.Controls.Add(gcAccGuest);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(615, 428);
            layoutControl1.TabIndex = 12;
            layoutControl1.Text = "layoutControl1";
            // 
            // rcAccompanyGuest
            // 
            rcAccompanyGuest.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcAccompanyGuest.ExpandCollapseItem.Id = 0;
            rcAccompanyGuest.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcAccompanyGuest.ExpandCollapseItem, rcAccompanyGuest.SearchEditItem, bbiNew, bbiSave, bbiDelete, bbiClose, bbiIssueCard });
            rcAccompanyGuest.Location = new Point(12, 12);
            rcAccompanyGuest.MaxItemId = 20;
            rcAccompanyGuest.Name = "rcAccompanyGuest";
            rcAccompanyGuest.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcAccompanyGuest.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcAccompanyGuest.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            rcAccompanyGuest.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
            rcAccompanyGuest.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            rcAccompanyGuest.ShowMoreCommandsButton = DevExpress.Utils.DefaultBoolean.False;
            rcAccompanyGuest.ShowPageHeadersInFormCaption = DevExpress.Utils.DefaultBoolean.False;
            rcAccompanyGuest.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcAccompanyGuest.ShowQatLocationSelector = false;
            rcAccompanyGuest.ShowToolbarCustomizeItem = false;
            rcAccompanyGuest.Size = new Size(591, 92);
            rcAccompanyGuest.Toolbar.ShowCustomizeItem = false;
            rcAccompanyGuest.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            rcAccompanyGuest.TransparentEditorsMode = DevExpress.Utils.DefaultBoolean.False;
            // 
            // bbiNew
            // 
            bbiNew.Caption = "New";
            bbiNew.Id = 1;
            bbiNew.ImageOptions.Image = (Image)resources.GetObject("bbiNew.ImageOptions.Image");
            bbiNew.Name = "bbiNew";
            bbiNew.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiNew.ItemClick += bbiNew_ItemClick;
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
            // bbiDelete
            // 
            bbiDelete.Caption = "Remove";
            bbiDelete.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiDelete.Id = 3;
            bbiDelete.ImageOptions.Image = (Image)resources.GetObject("bbiDelete.ImageOptions.Image");
            bbiDelete.Name = "bbiDelete";
            bbiDelete.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiDelete.ItemClick += bbiDelete_ItemClick;
            // 
            // bbiClose
            // 
            bbiClose.Caption = "Close";
            bbiClose.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiClose.Id = 9;
            bbiClose.ImageOptions.Image = (Image)resources.GetObject("bbiClose.ImageOptions.Image");
            bbiClose.ImageOptions.LargeImage = (Image)resources.GetObject("bbiClose.ImageOptions.LargeImage");
            bbiClose.Name = "bbiClose";
            bbiClose.ItemClick += bbiClose_ItemClick;
            // 
            // bbiIssueCard
            // 
            bbiIssueCard.Caption = "Issue Card";
            bbiIssueCard.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiIssueCard.Id = 10;
            bbiIssueCard.ImageOptions.Image = (Image)resources.GetObject("bbiIssueCard.ImageOptions.Image");
            bbiIssueCard.ImageOptions.LargeImage = (Image)resources.GetObject("bbiIssueCard.ImageOptions.LargeImage");
            bbiIssueCard.Name = "bbiIssueCard";
            bbiIssueCard.ItemClick += bbiIssueCard_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup2, ribbonPageGroup4, ribbonPageGroup3 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup2.ItemLinks.Add(bbiSave);
            ribbonPageGroup2.ItemLinks.Add(bbiDelete);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // ribbonPageGroup4
            // 
            ribbonPageGroup4.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup4.ItemLinks.Add(bbiIssueCard);
            ribbonPageGroup4.Name = "ribbonPageGroup4";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup3.ImageOptions.Image = (Image)resources.GetObject("ribbonPageGroup3.ImageOptions.Image");
            ribbonPageGroup3.ItemLinks.Add(bbiClose);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            // 
            // gcAccGuest
            // 
            gcAccGuest.Location = new Point(12, 109);
            gcAccGuest.MainView = gv_accGuest;
            gcAccGuest.MenuManager = rcAccompanyGuest;
            gcAccGuest.Name = "gcAccGuest";
            gcAccGuest.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { risle_accGuest });
            gcAccGuest.Size = new Size(591, 307);
            gcAccGuest.TabIndex = 4;
            gcAccGuest.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gv_accGuest });
            // 
            // gv_accGuest
            // 
            gv_accGuest.Appearance.FocusedCell.BackColor = SystemColors.Highlight;
            gv_accGuest.Appearance.FocusedCell.ForeColor = Color.White;
            gv_accGuest.Appearance.FocusedCell.Options.UseBackColor = true;
            gv_accGuest.Appearance.FocusedCell.Options.UseForeColor = true;
            gv_accGuest.Appearance.FocusedRow.BackColor = SystemColors.Highlight;
            gv_accGuest.Appearance.FocusedRow.ForeColor = Color.White;
            gv_accGuest.Appearance.FocusedRow.Options.UseBackColor = true;
            gv_accGuest.Appearance.FocusedRow.Options.UseForeColor = true;
            gv_accGuest.Appearance.OddRow.BackColor = SystemColors.ControlLight;
            gv_accGuest.Appearance.OddRow.Options.UseBackColor = true;
            gv_accGuest.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gCol_guest, gCol_address, gridColumn1 });
            gv_accGuest.GridControl = gcAccGuest;
            gv_accGuest.Name = "gv_accGuest";
            gv_accGuest.OptionsSelection.EnableAppearanceFocusedCell = false;
            gv_accGuest.OptionsSelection.EnableAppearanceHideSelection = false;
            gv_accGuest.OptionsView.EnableAppearanceOddRow = true;
            gv_accGuest.OptionsView.ShowGroupPanel = false;
            gv_accGuest.OptionsView.ShowIndicator = false;
            gv_accGuest.RowStyle += gv_accGuest_RowStyle;
            // 
            // gCol_guest
            // 
            gCol_guest.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_guest.AppearanceHeader.Options.UseFont = true;
            gCol_guest.Caption = "Guest";
            gCol_guest.ColumnEdit = risle_accGuest;
            gCol_guest.FieldName = "consignee";
            gCol_guest.Name = "gCol_guest";
            gCol_guest.OptionsColumn.FixedWidth = true;
            gCol_guest.Visible = true;
            gCol_guest.VisibleIndex = 1;
            gCol_guest.Width = 256;
            // 
            // risle_accGuest
            // 
            risle_accGuest.AutoHeight = false;
            risle_accGuest.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            risle_accGuest.DisplayMember = "fullName";
            risle_accGuest.Name = "risle_accGuest";
            risle_accGuest.NullText = "<Add New>";
            risle_accGuest.PopupView = gridView1;
            risle_accGuest.ShowAddNewButton = true;
            risle_accGuest.ValueMember = "consignee";
            risle_accGuest.ViewType = DevExpress.XtraEditors.Repository.GridLookUpViewType.GridView;
            // 
            // gridView1
            // 
            gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView1.Name = "gridView1";
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // gCol_address
            // 
            gCol_address.AppearanceCell.Options.UseTextOptions = true;
            gCol_address.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            gCol_address.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_address.AppearanceHeader.Options.UseFont = true;
            gCol_address.Caption = "Address";
            gCol_address.FieldName = "address";
            gCol_address.Name = "gCol_address";
            gCol_address.OptionsColumn.AllowEdit = false;
            gCol_address.Visible = true;
            gCol_address.VisibleIndex = 2;
            gCol_address.Width = 293;
            // 
            // gridColumn1
            // 
            gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            gridColumn1.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gridColumn1.AppearanceHeader.Options.UseFont = true;
            gridColumn1.Caption = "SN";
            gridColumn1.FieldName = "SN";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 0;
            gridColumn1.Width = 36;
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lc_gcAccGuest, layoutControlItem1 });
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(615, 428);
            layoutControlGroup1.TextVisible = false;
            // 
            // lc_gcAccGuest
            // 
            lc_gcAccGuest.Control = gcAccGuest;
            lc_gcAccGuest.CustomizationFormText = "lc_gcAccGuest";
            lc_gcAccGuest.Location = new Point(0, 97);
            lc_gcAccGuest.Name = "lc_gcAccGuest";
            lc_gcAccGuest.Size = new Size(595, 311);
            lc_gcAccGuest.TextSize = new Size(0, 0);
            lc_gcAccGuest.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = rcAccompanyGuest;
            layoutControlItem1.Location = new Point(0, 0);
            layoutControlItem1.MaxSize = new Size(0, 97);
            layoutControlItem1.MinSize = new Size(209, 97);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(595, 97);
            layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem1.TextSize = new Size(0, 0);
            layoutControlItem1.TextVisible = false;
            // 
            // frmAccompanyingGuest
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(615, 428);
            Controls.Add(layoutControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmAccompanyingGuest.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            IsMdiContainer = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmAccompanyingGuest";
            Text = "Accompanying Guest";
            Load += frmAccompanyingGuest_Load;
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            layoutControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)rcAccompanyGuest).EndInit();
            ((System.ComponentModel.ISupportInitialize)gcAccGuest).EndInit();
            ((System.ComponentModel.ISupportInitialize)gv_accGuest).EndInit();
            ((System.ComponentModel.ISupportInitialize)risle_accGuest).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)lc_gcAccGuest).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private DevExpress.XtraBars.Ribbon.RibbonControl rcAccompanyGuest;
        private DevExpress.XtraBars.BarButtonItem bbiNew;
        private DevExpress.XtraBars.BarButtonItem bbiSave;
        private DevExpress.XtraBars.BarButtonItem bbiDelete;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.BarButtonItem bbiIssueCard;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gcAccGuest;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_accGuest;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_guest;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit risle_accGuest;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_address;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lc_gcAccGuest;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}