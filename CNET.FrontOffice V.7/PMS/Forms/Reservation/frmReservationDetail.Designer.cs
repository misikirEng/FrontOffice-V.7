
namespace CNET.FrontOffice_V._7.Forms
{
    partial class frmReservationDetail
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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReservationDetail));
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            gCol_pkgName = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_amount = new DevExpress.XtraGrid.Columns.GridColumn();
            gc_resDetail = new DevExpress.XtraGrid.GridControl();
            gv_resDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            gcol_SN = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_date = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_weekDay = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_adult = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_child = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_roomCount = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_roomType = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_actualRTC = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_room = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_rateDetail = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_rateAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_market = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_Source = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_isFixed = new DevExpress.XtraGrid.Columns.GridColumn();
            repItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            lc_gvResDetail = new DevExpress.XtraLayout.LayoutControlItem();
            rb_regDetail = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiRefresh = new DevExpress.XtraBars.BarButtonItem();
            bbiEdit = new DevExpress.XtraBars.BarButtonItem();
            bbiPdfExport = new DevExpress.XtraBars.BarButtonItem();
            bbiExcelReport = new DevExpress.XtraBars.BarButtonItem();
            bbiPrint = new DevExpress.XtraBars.BarButtonItem();
            bci_expandAll = new DevExpress.XtraBars.BarCheckItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            beiExpandCheckBx = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gc_resDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gv_resDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repItemCheckEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl2).BeginInit();
            layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lc_gvResDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rb_regDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)beiExpandCheckBx).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            SuspendLayout();
            // 
            // gridView1
            // 
            gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gCol_pkgName, gCol_amount });
            gridView1.GridControl = gc_resDetail;
            gridView1.Name = "gridView1";
            gridView1.OptionsBehavior.Editable = false;
            gridView1.ViewCaption = "Package To Post";
            // 
            // gCol_pkgName
            // 
            gCol_pkgName.Caption = "Package";
            gCol_pkgName.FieldName = "packageHeader";
            gCol_pkgName.Name = "gCol_pkgName";
            gCol_pkgName.Visible = true;
            gCol_pkgName.VisibleIndex = 0;
            // 
            // gCol_amount
            // 
            gCol_amount.Caption = "Amount";
            gCol_amount.FieldName = "amount";
            gCol_amount.Name = "gCol_amount";
            gCol_amount.Visible = true;
            gCol_amount.VisibleIndex = 1;
            // 
            // gc_resDetail
            // 
            gridLevelNode1.LevelTemplate = gridView1;
            gridLevelNode1.RelationName = "Level1";
            gc_resDetail.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
            gc_resDetail.Location = new Point(12, 12);
            gc_resDetail.MainView = gv_resDetail;
            gc_resDetail.Name = "gc_resDetail";
            gc_resDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repItemCheckEdit });
            gc_resDetail.Size = new Size(1072, 382);
            gc_resDetail.TabIndex = 4;
            gc_resDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gv_resDetail, gridView1 });
            gc_resDetail.ViewRegistered += gc_resDetail_ViewRegistered;
            gc_resDetail.Click += gc_resDetail_Click;
            // 
            // gv_resDetail
            // 
            gv_resDetail.Appearance.FocusedCell.BackColor = SystemColors.Highlight;
            gv_resDetail.Appearance.FocusedCell.Options.UseBackColor = true;
            gv_resDetail.Appearance.FocusedRow.BackColor = SystemColors.Highlight;
            gv_resDetail.Appearance.FocusedRow.Options.UseBackColor = true;
            gv_resDetail.Appearance.OddRow.BackColor = Color.MintCream;
            gv_resDetail.Appearance.OddRow.Options.UseBackColor = true;
            gv_resDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gcol_SN, gCol_date, gCol_weekDay, gCol_adult, gCol_child, gCol_roomCount, gCol_roomType, gCol_actualRTC, gCol_room, gCol_rateDetail, gCol_rateAmount, gCol_market, gCol_Source, gCol_isFixed });
            gv_resDetail.GridControl = gc_resDetail;
            gv_resDetail.Name = "gv_resDetail";
            gv_resDetail.OptionsBehavior.Editable = false;
            gv_resDetail.OptionsDetail.AllowExpandEmptyDetails = true;
            gv_resDetail.OptionsDetail.ShowDetailTabs = false;
            gv_resDetail.OptionsPrint.PrintDetails = true;
            gv_resDetail.OptionsSelection.EnableAppearanceHideSelection = false;
            gv_resDetail.OptionsView.EnableAppearanceOddRow = true;
            gv_resDetail.OptionsView.ShowGroupPanel = false;
            gv_resDetail.OptionsView.ShowIndicator = false;
            gv_resDetail.RowStyle += gv_resDetail_RowStyle;
            gv_resDetail.DoubleClick += gv_resDetail_DoubleClick;
            // 
            // gcol_SN
            // 
            gcol_SN.Name = "gcol_SN";
            gcol_SN.Visible = true;
            gcol_SN.VisibleIndex = 0;
            gcol_SN.Width = 34;
            // 
            // gCol_date
            // 
            gCol_date.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_date.AppearanceHeader.Options.UseFont = true;
            gCol_date.Caption = "Date";
            gCol_date.FieldName = "Date";
            gCol_date.Name = "gCol_date";
            gCol_date.Visible = true;
            gCol_date.VisibleIndex = 1;
            gCol_date.Width = 78;
            // 
            // gCol_weekDay
            // 
            gCol_weekDay.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_weekDay.AppearanceHeader.Options.UseFont = true;
            gCol_weekDay.Caption = "Week Day";
            gCol_weekDay.FieldName = "DayWeek";
            gCol_weekDay.Name = "gCol_weekDay";
            gCol_weekDay.Visible = true;
            gCol_weekDay.VisibleIndex = 2;
            gCol_weekDay.Width = 78;
            // 
            // gCol_adult
            // 
            gCol_adult.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_adult.AppearanceHeader.Options.UseFont = true;
            gCol_adult.Caption = "Adult";
            gCol_adult.FieldName = "Adult";
            gCol_adult.Name = "gCol_adult";
            gCol_adult.Visible = true;
            gCol_adult.VisibleIndex = 3;
            gCol_adult.Width = 46;
            // 
            // gCol_child
            // 
            gCol_child.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_child.AppearanceHeader.Options.UseFont = true;
            gCol_child.Caption = "Child";
            gCol_child.FieldName = "Child";
            gCol_child.Name = "gCol_child";
            gCol_child.Visible = true;
            gCol_child.VisibleIndex = 4;
            gCol_child.Width = 43;
            // 
            // gCol_roomCount
            // 
            gCol_roomCount.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_roomCount.AppearanceHeader.Options.UseFont = true;
            gCol_roomCount.Caption = "Room Count";
            gCol_roomCount.FieldName = "RoomCount";
            gCol_roomCount.Name = "gCol_roomCount";
            gCol_roomCount.Visible = true;
            gCol_roomCount.VisibleIndex = 5;
            gCol_roomCount.Width = 77;
            // 
            // gCol_roomType
            // 
            gCol_roomType.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_roomType.AppearanceHeader.Options.UseFont = true;
            gCol_roomType.Caption = "Room Type";
            gCol_roomType.FieldName = "roomTypeDesc";
            gCol_roomType.Name = "gCol_roomType";
            gCol_roomType.Visible = true;
            gCol_roomType.VisibleIndex = 6;
            gCol_roomType.Width = 133;
            // 
            // gCol_actualRTC
            // 
            gCol_actualRTC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_actualRTC.AppearanceHeader.Options.UseFont = true;
            gCol_actualRTC.Caption = "Actual RTC";
            gCol_actualRTC.FieldName = "actualRTCDesc";
            gCol_actualRTC.Name = "gCol_actualRTC";
            gCol_actualRTC.Visible = true;
            gCol_actualRTC.VisibleIndex = 7;
            gCol_actualRTC.Width = 100;
            // 
            // gCol_room
            // 
            gCol_room.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_room.AppearanceHeader.Options.UseFont = true;
            gCol_room.Caption = "Room";
            gCol_room.FieldName = "roomDesc";
            gCol_room.Name = "gCol_room";
            gCol_room.Visible = true;
            gCol_room.VisibleIndex = 8;
            gCol_room.Width = 42;
            // 
            // gCol_rateDetail
            // 
            gCol_rateDetail.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_rateDetail.AppearanceHeader.Options.UseFont = true;
            gCol_rateDetail.Caption = "Rate";
            gCol_rateDetail.FieldName = "rateCodeDetailDesc";
            gCol_rateDetail.Name = "gCol_rateDetail";
            gCol_rateDetail.Visible = true;
            gCol_rateDetail.VisibleIndex = 9;
            gCol_rateDetail.Width = 101;
            // 
            // gCol_rateAmount
            // 
            gCol_rateAmount.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_rateAmount.AppearanceHeader.Options.UseFont = true;
            gCol_rateAmount.Caption = "Rate Amount";
            gCol_rateAmount.FieldName = "RateAmount";
            gCol_rateAmount.Name = "gCol_rateAmount";
            gCol_rateAmount.Visible = true;
            gCol_rateAmount.VisibleIndex = 10;
            gCol_rateAmount.Width = 95;
            // 
            // gCol_market
            // 
            gCol_market.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_market.AppearanceHeader.Options.UseFont = true;
            gCol_market.Caption = "Market";
            gCol_market.FieldName = "marketDesc";
            gCol_market.Name = "gCol_market";
            gCol_market.Visible = true;
            gCol_market.VisibleIndex = 11;
            gCol_market.Width = 93;
            // 
            // gCol_Source
            // 
            gCol_Source.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_Source.AppearanceHeader.Options.UseFont = true;
            gCol_Source.Caption = "Source";
            gCol_Source.FieldName = "sourceDesc";
            gCol_Source.Name = "gCol_Source";
            gCol_Source.Visible = true;
            gCol_Source.VisibleIndex = 12;
            gCol_Source.Width = 91;
            // 
            // gCol_isFixed
            // 
            gCol_isFixed.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_isFixed.AppearanceHeader.Options.UseFont = true;
            gCol_isFixed.Caption = "Is Fixed";
            gCol_isFixed.ColumnEdit = repItemCheckEdit;
            gCol_isFixed.FieldName = "IsFixedRate";
            gCol_isFixed.Name = "gCol_isFixed";
            gCol_isFixed.Visible = true;
            gCol_isFixed.VisibleIndex = 13;
            gCol_isFixed.Width = 59;
            // 
            // repItemCheckEdit
            // 
            repItemCheckEdit.AutoHeight = false;
            repItemCheckEdit.Caption = "Check";
            repItemCheckEdit.Name = "repItemCheckEdit";
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(layoutControl2);
            layoutControl1.Controls.Add(rb_regDetail);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(2, 2);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(239, 184, 250, 350);
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(1106, 487);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl2
            // 
            layoutControl2.Controls.Add(gc_resDetail);
            layoutControl2.Location = new Point(5, 76);
            layoutControl2.Name = "layoutControl2";
            layoutControl2.Root = Root;
            layoutControl2.Size = new Size(1096, 406);
            layoutControl2.TabIndex = 4;
            layoutControl2.Text = "layoutControl2";
            // 
            // Root
            // 
            Root.CustomizationFormText = "Root";
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lc_gvResDetail });
            Root.Name = "Root";
            Root.Size = new Size(1096, 406);
            Root.TextVisible = false;
            // 
            // lc_gvResDetail
            // 
            lc_gvResDetail.Control = gc_resDetail;
            lc_gvResDetail.CustomizationFormText = "lc_gvResDetail";
            lc_gvResDetail.Location = new Point(0, 0);
            lc_gvResDetail.Name = "lc_gvResDetail";
            lc_gvResDetail.Size = new Size(1076, 386);
            lc_gvResDetail.TextSize = new Size(0, 0);
            lc_gvResDetail.TextVisible = false;
            // 
            // rb_regDetail
            // 
            rb_regDetail.Dock = DockStyle.None;
            rb_regDetail.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rb_regDetail.ExpandCollapseItem.Id = 0;
            rb_regDetail.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rb_regDetail.ExpandCollapseItem, rb_regDetail.SearchEditItem, bbiRefresh, bbiEdit, bbiPdfExport, bbiExcelReport, bbiPrint, bci_expandAll, bbiClose });
            rb_regDetail.Location = new Point(5, 5);
            rb_regDetail.MaxItemId = 10;
            rb_regDetail.Name = "rb_regDetail";
            rb_regDetail.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rb_regDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { beiExpandCheckBx, repositoryItemCheckEdit2 });
            rb_regDetail.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rb_regDetail.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            rb_regDetail.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
            rb_regDetail.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            rb_regDetail.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rb_regDetail.ShowToolbarCustomizeItem = false;
            rb_regDetail.Size = new Size(1096, 66);
            rb_regDetail.Toolbar.ShowCustomizeItem = false;
            rb_regDetail.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiRefresh
            // 
            bbiRefresh.Caption = "Refresh";
            bbiRefresh.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiRefresh.Id = 1;
            bbiRefresh.ImageOptions.Image = (Image)resources.GetObject("bbiRefresh.ImageOptions.Image");
            bbiRefresh.ImageOptions.LargeImage = (Image)resources.GetObject("bbiRefresh.ImageOptions.LargeImage");
            bbiRefresh.Name = "bbiRefresh";
            // 
            // bbiEdit
            // 
            bbiEdit.Caption = "Edit";
            bbiEdit.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiEdit.Id = 2;
            bbiEdit.ImageOptions.Image = (Image)resources.GetObject("bbiEdit.ImageOptions.Image");
            bbiEdit.ImageOptions.LargeImage = (Image)resources.GetObject("bbiEdit.ImageOptions.LargeImage");
            bbiEdit.Name = "bbiEdit";
            bbiEdit.ItemClick += bbiEdit_ItemClick;
            // 
            // bbiPdfExport
            // 
            bbiPdfExport.Caption = "To PDF";
            bbiPdfExport.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiPdfExport.Id = 3;
            bbiPdfExport.ImageOptions.Image = (Image)resources.GetObject("bbiPdfExport.ImageOptions.Image");
            bbiPdfExport.ImageOptions.LargeImage = (Image)resources.GetObject("bbiPdfExport.ImageOptions.LargeImage");
            bbiPdfExport.Name = "bbiPdfExport";
            bbiPdfExport.ItemClick += bbiPdfExport_ItemClick;
            // 
            // bbiExcelReport
            // 
            bbiExcelReport.Caption = "To Excel";
            bbiExcelReport.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiExcelReport.Id = 4;
            bbiExcelReport.ImageOptions.Image = (Image)resources.GetObject("bbiExcelReport.ImageOptions.Image");
            bbiExcelReport.ImageOptions.LargeImage = (Image)resources.GetObject("bbiExcelReport.ImageOptions.LargeImage");
            bbiExcelReport.Name = "bbiExcelReport";
            bbiExcelReport.ItemClick += bbiExcelReport_ItemClick;
            // 
            // bbiPrint
            // 
            bbiPrint.Caption = "Print";
            bbiPrint.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiPrint.Id = 5;
            bbiPrint.ImageOptions.Image = (Image)resources.GetObject("bbiPrint.ImageOptions.Image");
            bbiPrint.ImageOptions.LargeImage = (Image)resources.GetObject("bbiPrint.ImageOptions.LargeImage");
            bbiPrint.Name = "bbiPrint";
            bbiPrint.ItemClick += bbiPrint_ItemClick;
            // 
            // bci_expandAll
            // 
            bci_expandAll.Caption = "Expand All";
            bci_expandAll.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bci_expandAll.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.AfterText;
            bci_expandAll.Id = 8;
            bci_expandAll.ImageOptions.Image = (Image)resources.GetObject("bci_expandAll.ImageOptions.Image");
            bci_expandAll.ImageOptions.LargeImage = (Image)resources.GetObject("bci_expandAll.ImageOptions.LargeImage");
            bci_expandAll.Name = "bci_expandAll";
            bci_expandAll.CheckedChanged += bci_expandAll_CheckedChanged;
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
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, ribbonPageGroup2, ribbonPageGroup3, ribbonPageGroup4, ribbonPageGroup5 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(bbiRefresh);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup2.ItemLinks.Add(bbiEdit);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup3.ItemLinks.Add(bbiPdfExport);
            ribbonPageGroup3.ItemLinks.Add(bbiExcelReport);
            ribbonPageGroup3.ItemLinks.Add(bbiPrint);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            // 
            // ribbonPageGroup4
            // 
            ribbonPageGroup4.ItemLinks.Add(bci_expandAll);
            ribbonPageGroup4.Name = "ribbonPageGroup4";
            ribbonPageGroup4.Text = "ribbonPageGroup4";
            // 
            // ribbonPageGroup5
            // 
            ribbonPageGroup5.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup5.ItemLinks.Add(bbiClose);
            ribbonPageGroup5.Name = "ribbonPageGroup5";
            // 
            // beiExpandCheckBx
            // 
            beiExpandCheckBx.AutoHeight = false;
            beiExpandCheckBx.Caption = "Check";
            beiExpandCheckBx.Name = "beiExpandCheckBx";
            beiExpandCheckBx.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            beiExpandCheckBx.ValueUnchecked = true;
            // 
            // repositoryItemCheckEdit2
            // 
            repositoryItemCheckEdit2.AutoHeight = false;
            repositoryItemCheckEdit2.Caption = "Check";
            repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "Root";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem2 });
            layoutControlGroup1.Name = "Root";
            layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
            layoutControlGroup1.Size = new Size(1106, 487);
            layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = layoutControl2;
            layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            layoutControlItem1.Location = new Point(0, 71);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(1100, 410);
            layoutControlItem1.TextSize = new Size(0, 0);
            layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = rb_regDetail;
            layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            layoutControlItem2.Location = new Point(0, 0);
            layoutControlItem2.MaxSize = new Size(0, 71);
            layoutControlItem2.MinSize = new Size(209, 71);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(1100, 71);
            layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem2.TextSize = new Size(0, 0);
            layoutControlItem2.TextVisible = false;
            // 
            // gridColumn1
            // 
            gridColumn1.Name = "gridColumn1";
            // 
            // frmReservationDetail
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1110, 491);
            Controls.Add(layoutControl1);
            IconOptions.Icon = (Icon)resources.GetObject("frmReservationDetail.IconOptions.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmReservationDetail";
            Padding = new Padding(2);
            Text = "Reservation Detail";
            FormClosing += frmReservationDetail_FormClosing;
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gc_resDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)gv_resDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)repItemCheckEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            layoutControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl2).EndInit();
            layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)lc_gvResDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)rb_regDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)beiExpandCheckBx).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemCheckEdit2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.Columns.GridColumn gcolCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcolAmountMoreFields;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl gc_resDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_resDetail;
        private DevExpress.XtraLayout.LayoutControlItem lc_gvResDetail;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_date;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_weekDay;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_adult;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_child;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_roomCount;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_roomType;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_actualRTC;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_room;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_rateDetail;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_rateAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_market;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_Source;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_isFixed;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repItemCheckEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gcol_SN;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_pkgName;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_amount;
        private DevExpress.XtraBars.Ribbon.RibbonControl rb_regDetail;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem bbiRefresh;
        private DevExpress.XtraBars.BarButtonItem bbiEdit;
        private DevExpress.XtraBars.BarButtonItem bbiPdfExport;
        private DevExpress.XtraBars.BarButtonItem bbiExcelReport;
        private DevExpress.XtraBars.BarButtonItem bbiPrint;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit beiExpandCheckBx;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraBars.BarCheckItem bci_expandAll;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;

    }
}