
namespace CNET.FrontOffice_V._7.Forms
{
    partial class frmRateSummery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRateSummery));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            gc_rateSummary = new DevExpress.XtraGrid.GridControl();
            gv_rateSummary = new DevExpress.XtraGrid.Views.Grid.GridView();
            gCol_SN = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_day = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_date = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_rateCode = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_roomRevenue = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_pkg = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_subTotal = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_serCharge = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_VAT = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_grandTotal = new DevExpress.XtraGrid.Columns.GridColumn();
            rcRateSummery = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiRefresh = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            bbiPrintExport = new DevExpress.XtraBars.BarButtonItem();
            bbiDetail = new DevExpress.XtraBars.BarButtonItem();
            bbiPrint = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            statusStrip1 = new StatusStrip();
            teOutstandingCost = new DevExpress.XtraEditors.TextEdit();
            teDeposit = new DevExpress.XtraEditors.TextEdit();
            teTotalCost = new DevExpress.XtraEditors.TextEdit();
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            lciRibbonHolder = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gc_rateSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gv_rateSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rcRateSummery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)teOutstandingCost.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)teDeposit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)teTotalCost.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciRibbonHolder).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(gc_rateSummary);
            layoutControl1.Controls.Add(statusStrip1);
            layoutControl1.Controls.Add(teOutstandingCost);
            layoutControl1.Controls.Add(teDeposit);
            layoutControl1.Controls.Add(teTotalCost);
            layoutControl1.Controls.Add(panelControl1);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(604, 337, 250, 350);
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(993, 525);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // gc_rateSummary
            // 
            gc_rateSummary.Location = new Point(2, 72);
            gc_rateSummary.MainView = gv_rateSummary;
            gc_rateSummary.MenuManager = rcRateSummery;
            gc_rateSummary.Name = "gc_rateSummary";
            gc_rateSummary.Size = new Size(989, 343);
            gc_rateSummary.TabIndex = 10;
            gc_rateSummary.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gv_rateSummary });
            // 
            // gv_rateSummary
            // 
            gv_rateSummary.Appearance.FocusedCell.BackColor = SystemColors.Highlight;
            gv_rateSummary.Appearance.FocusedCell.ForeColor = Color.White;
            gv_rateSummary.Appearance.FocusedCell.Options.UseBackColor = true;
            gv_rateSummary.Appearance.FocusedCell.Options.UseForeColor = true;
            gv_rateSummary.Appearance.FocusedRow.BackColor = SystemColors.Highlight;
            gv_rateSummary.Appearance.FocusedRow.ForeColor = Color.White;
            gv_rateSummary.Appearance.FocusedRow.Options.UseBackColor = true;
            gv_rateSummary.Appearance.FocusedRow.Options.UseForeColor = true;
            gv_rateSummary.Appearance.OddRow.BackColor = SystemColors.ControlLight;
            gv_rateSummary.Appearance.OddRow.Options.UseBackColor = true;
            gv_rateSummary.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gCol_SN, gCol_day, gCol_date, gCol_rateCode, gCol_roomRevenue, gCol_pkg, gCol_subTotal, gCol_serCharge, gCol_VAT, gCol_grandTotal });
            gv_rateSummary.GridControl = gc_rateSummary;
            gv_rateSummary.Name = "gv_rateSummary";
            gv_rateSummary.OptionsBehavior.Editable = false;
            gv_rateSummary.OptionsDetail.EnableMasterViewMode = false;
            gv_rateSummary.OptionsDetail.ShowDetailTabs = false;
            gv_rateSummary.OptionsDetail.SmartDetailExpand = false;
            gv_rateSummary.OptionsSelection.EnableAppearanceFocusedCell = false;
            gv_rateSummary.OptionsSelection.EnableAppearanceHideSelection = false;
            gv_rateSummary.OptionsView.EnableAppearanceOddRow = true;
            gv_rateSummary.OptionsView.ShowGroupPanel = false;
            gv_rateSummary.OptionsView.ShowIndicator = false;
            gv_rateSummary.CustomDrawCell += gv_rateSummary_CustomDrawCell;
            gv_rateSummary.RowStyle += gv_rateSummary_RowStyle;
            gv_rateSummary.DoubleClick += gv_rateSummary_DoubleClick;
            // 
            // gCol_SN
            // 
            gCol_SN.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_SN.AppearanceHeader.Options.UseFont = true;
            gCol_SN.Caption = "SN";
            gCol_SN.Name = "gCol_SN";
            gCol_SN.Visible = true;
            gCol_SN.VisibleIndex = 0;
            gCol_SN.Width = 41;
            // 
            // gCol_day
            // 
            gCol_day.AppearanceCell.Options.UseTextOptions = true;
            gCol_day.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            gCol_day.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_day.AppearanceHeader.Options.UseFont = true;
            gCol_day.Caption = "Day";
            gCol_day.FieldName = "day";
            gCol_day.Name = "gCol_day";
            gCol_day.Visible = true;
            gCol_day.VisibleIndex = 1;
            gCol_day.Width = 79;
            // 
            // gCol_date
            // 
            gCol_date.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_date.AppearanceHeader.Options.UseFont = true;
            gCol_date.Caption = "Date";
            gCol_date.FieldName = "date";
            gCol_date.Name = "gCol_date";
            gCol_date.Visible = true;
            gCol_date.VisibleIndex = 2;
            gCol_date.Width = 107;
            // 
            // gCol_rateCode
            // 
            gCol_rateCode.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_rateCode.AppearanceHeader.Options.UseFont = true;
            gCol_rateCode.Caption = "Rate Code";
            gCol_rateCode.FieldName = "rateCode";
            gCol_rateCode.Name = "gCol_rateCode";
            gCol_rateCode.Visible = true;
            gCol_rateCode.VisibleIndex = 3;
            gCol_rateCode.Width = 107;
            // 
            // gCol_roomRevenue
            // 
            gCol_roomRevenue.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_roomRevenue.AppearanceHeader.Options.UseFont = true;
            gCol_roomRevenue.Caption = "Room Revenue";
            gCol_roomRevenue.FieldName = "roomRevenue";
            gCol_roomRevenue.Name = "gCol_roomRevenue";
            gCol_roomRevenue.Visible = true;
            gCol_roomRevenue.VisibleIndex = 4;
            gCol_roomRevenue.Width = 107;
            // 
            // gCol_pkg
            // 
            gCol_pkg.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_pkg.AppearanceHeader.Options.UseFont = true;
            gCol_pkg.Caption = "Package";
            gCol_pkg.FieldName = "package";
            gCol_pkg.Name = "gCol_pkg";
            gCol_pkg.Visible = true;
            gCol_pkg.VisibleIndex = 5;
            gCol_pkg.Width = 115;
            // 
            // gCol_subTotal
            // 
            gCol_subTotal.AppearanceCell.Options.UseTextOptions = true;
            gCol_subTotal.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gCol_subTotal.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_subTotal.AppearanceHeader.Options.UseFont = true;
            gCol_subTotal.Caption = "Subtotal";
            gCol_subTotal.FieldName = "subTotal";
            gCol_subTotal.Name = "gCol_subTotal";
            gCol_subTotal.Visible = true;
            gCol_subTotal.VisibleIndex = 6;
            gCol_subTotal.Width = 105;
            // 
            // gCol_serCharge
            // 
            gCol_serCharge.AppearanceCell.Options.UseTextOptions = true;
            gCol_serCharge.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gCol_serCharge.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_serCharge.AppearanceHeader.Options.UseFont = true;
            gCol_serCharge.Caption = "Service Charge";
            gCol_serCharge.FieldName = "serCharge";
            gCol_serCharge.Name = "gCol_serCharge";
            gCol_serCharge.Visible = true;
            gCol_serCharge.VisibleIndex = 7;
            gCol_serCharge.Width = 105;
            // 
            // gCol_VAT
            // 
            gCol_VAT.AppearanceCell.Options.UseTextOptions = true;
            gCol_VAT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gCol_VAT.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_VAT.AppearanceHeader.Options.UseFont = true;
            gCol_VAT.Caption = "VAT";
            gCol_VAT.FieldName = "VAT";
            gCol_VAT.Name = "gCol_VAT";
            gCol_VAT.Visible = true;
            gCol_VAT.VisibleIndex = 8;
            gCol_VAT.Width = 105;
            // 
            // gCol_grandTotal
            // 
            gCol_grandTotal.AppearanceCell.Options.UseTextOptions = true;
            gCol_grandTotal.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gCol_grandTotal.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_grandTotal.AppearanceHeader.Options.UseFont = true;
            gCol_grandTotal.Caption = "Grand Total";
            gCol_grandTotal.FieldName = "grandTotal";
            gCol_grandTotal.Name = "gCol_grandTotal";
            gCol_grandTotal.Visible = true;
            gCol_grandTotal.VisibleIndex = 9;
            gCol_grandTotal.Width = 116;
            // 
            // rcRateSummery
            // 
            rcRateSummery.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcRateSummery.ExpandCollapseItem.Id = 0;
            rcRateSummery.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcRateSummery.ExpandCollapseItem, rcRateSummery.SearchEditItem, bbiRefresh, barButtonItem1, barButtonItem2, bbiPrintExport, bbiDetail, bbiPrint });
            rcRateSummery.Location = new Point(2, 2);
            rcRateSummery.MaxItemId = 8;
            rcRateSummery.Name = "rcRateSummery";
            rcRateSummery.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcRateSummery.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcRateSummery.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcRateSummery.Size = new Size(985, 66);
            rcRateSummery.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiRefresh
            // 
            bbiRefresh.Caption = "Refresh";
            bbiRefresh.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiRefresh.Id = 1;
            bbiRefresh.ImageOptions.Image = (Image)resources.GetObject("bbiRefresh.ImageOptions.Image");
            bbiRefresh.Name = "bbiRefresh";
            bbiRefresh.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // barButtonItem1
            // 
            barButtonItem1.Caption = "Dollar";
            barButtonItem1.Id = 3;
            barButtonItem1.Name = "barButtonItem1";
            // 
            // barButtonItem2
            // 
            barButtonItem2.Caption = "yen";
            barButtonItem2.Id = 4;
            barButtonItem2.Name = "barButtonItem2";
            // 
            // bbiPrintExport
            // 
            bbiPrintExport.Caption = "Close";
            bbiPrintExport.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiPrintExport.Id = 5;
            bbiPrintExport.ImageOptions.Image = (Image)resources.GetObject("bbiPrintExport.ImageOptions.Image");
            bbiPrintExport.ImageOptions.LargeImage = (Image)resources.GetObject("bbiPrintExport.ImageOptions.LargeImage");
            bbiPrintExport.Name = "bbiPrintExport";
            bbiPrintExport.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiPrintExport.ItemClick += bbiPrintExport_ItemClick;
            // 
            // bbiDetail
            // 
            bbiDetail.Caption = "Detail";
            bbiDetail.Id = 6;
            bbiDetail.ImageOptions.Image = (Image)resources.GetObject("bbiDetail.ImageOptions.Image");
            bbiDetail.ImageOptions.LargeImage = (Image)resources.GetObject("bbiDetail.ImageOptions.LargeImage");
            bbiDetail.Name = "bbiDetail";
            bbiDetail.ItemClick += bbiDetail_ItemClick;
            // 
            // bbiPrint
            // 
            bbiPrint.Caption = "Print";
            bbiPrint.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiPrint.Id = 7;
            bbiPrint.ImageOptions.Image = (Image)resources.GetObject("bbiPrint.ImageOptions.Image");
            bbiPrint.ImageOptions.LargeImage = (Image)resources.GetObject("bbiPrint.ImageOptions.LargeImage");
            bbiPrint.Name = "bbiPrint";
            bbiPrint.ItemClick += bbiPrint_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup3, ribbonPageGroup2, ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.ItemLinks.Add(bbiDetail);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            ribbonPageGroup3.Text = "Currency";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(bbiPrintExport);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            ribbonPageGroup2.Text = "Print or Export";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(bbiPrint);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // statusStrip1
            // 
            statusStrip1.AutoSize = false;
            statusStrip1.Dock = DockStyle.None;
            statusStrip1.Location = new Point(2, 503);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(989, 20);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 9;
            statusStrip1.Text = "statusStrip1";
            // 
            // teOutstandingCost
            // 
            teOutstandingCost.EditValue = "0.0";
            teOutstandingCost.Location = new Point(838, 477);
            teOutstandingCost.MenuManager = rcRateSummery;
            teOutstandingCost.Name = "teOutstandingCost";
            teOutstandingCost.Properties.AllowFocused = false;
            teOutstandingCost.Properties.AppearanceReadOnly.Options.UseTextOptions = true;
            teOutstandingCost.Properties.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            teOutstandingCost.Properties.ReadOnly = true;
            teOutstandingCost.Size = new Size(148, 20);
            teOutstandingCost.StyleController = layoutControl1;
            teOutstandingCost.TabIndex = 8;
            // 
            // teDeposit
            // 
            teDeposit.EditValue = "0.0";
            teDeposit.Location = new Point(838, 449);
            teDeposit.MenuManager = rcRateSummery;
            teDeposit.Name = "teDeposit";
            teDeposit.Properties.AllowFocused = false;
            teDeposit.Properties.AppearanceReadOnly.Options.UseTextOptions = true;
            teDeposit.Properties.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            teDeposit.Properties.ReadOnly = true;
            teDeposit.Size = new Size(148, 20);
            teDeposit.StyleController = layoutControl1;
            teDeposit.TabIndex = 7;
            // 
            // teTotalCost
            // 
            teTotalCost.EditValue = "0.0";
            teTotalCost.Location = new Point(838, 421);
            teTotalCost.MenuManager = rcRateSummery;
            teTotalCost.Name = "teTotalCost";
            teTotalCost.Properties.AllowFocused = false;
            teTotalCost.Properties.AppearanceReadOnly.Options.UseTextOptions = true;
            teTotalCost.Properties.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            teTotalCost.Properties.ReadOnly = true;
            teTotalCost.Size = new Size(148, 20);
            teTotalCost.StyleController = layoutControl1;
            teTotalCost.TabIndex = 6;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(rcRateSummery);
            panelControl1.Location = new Point(2, 2);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(989, 66);
            panelControl1.TabIndex = 5;
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciRibbonHolder, layoutControlItem2, layoutControlItem3, layoutControlItem4, emptySpaceItem2, layoutControlItem5, layoutControlItem6 });
            layoutControlGroup1.Name = "Root";
            layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup1.Size = new Size(993, 525);
            layoutControlGroup1.TextVisible = false;
            // 
            // lciRibbonHolder
            // 
            lciRibbonHolder.Control = panelControl1;
            lciRibbonHolder.CustomizationFormText = "layoutControlItem2";
            lciRibbonHolder.Location = new Point(0, 0);
            lciRibbonHolder.Name = "lciRibbonHolder";
            lciRibbonHolder.Size = new Size(993, 70);
            lciRibbonHolder.TextSize = new Size(0, 0);
            lciRibbonHolder.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem2.Control = teTotalCost;
            layoutControlItem2.CustomizationFormText = "Total Cost of Stay";
            layoutControlItem2.Location = new Point(708, 417);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(285, 28);
            layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 5, 2, 2);
            layoutControlItem2.Text = "Total Cost of Stay";
            layoutControlItem2.TextSize = new Size(111, 13);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem3.Control = teDeposit;
            layoutControlItem3.CustomizationFormText = "Deposit";
            layoutControlItem3.Location = new Point(708, 445);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(285, 28);
            layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 5, 2, 2);
            layoutControlItem3.Text = "Total Deducted";
            layoutControlItem3.TextSize = new Size(111, 13);
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem4.Control = teOutstandingCost;
            layoutControlItem4.CustomizationFormText = "Outstanding Stay Total";
            layoutControlItem4.Location = new Point(708, 473);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new Size(285, 28);
            layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 5, 2, 2);
            layoutControlItem4.Text = "Outstanding Stay Total";
            layoutControlItem4.TextSize = new Size(111, 13);
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.AllowHotTrack = false;
            emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
            emptySpaceItem2.Location = new Point(0, 417);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(708, 84);
            emptySpaceItem2.TextSize = new Size(0, 0);
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.Control = statusStrip1;
            layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            layoutControlItem5.Location = new Point(0, 501);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Size = new Size(993, 24);
            layoutControlItem5.TextSize = new Size(0, 0);
            layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.Control = gc_rateSummary;
            layoutControlItem6.CustomizationFormText = "layoutControlItem6";
            layoutControlItem6.Location = new Point(0, 70);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new Size(993, 347);
            layoutControlItem6.TextSize = new Size(0, 0);
            layoutControlItem6.TextVisible = false;
            // 
            // frmRateSummery
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(993, 525);
            Controls.Add(layoutControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmRateSummery.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmRateSummery";
            Text = "Rate Summery";
            Load += frmRateSummery_Load;
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gc_rateSummary).EndInit();
            ((System.ComponentModel.ISupportInitialize)gv_rateSummary).EndInit();
            ((System.ComponentModel.ISupportInitialize)rcRateSummery).EndInit();
            ((System.ComponentModel.ISupportInitialize)teOutstandingCost.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)teDeposit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)teTotalCost.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciRibbonHolder).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcRateSummery;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraLayout.LayoutControlItem lciRibbonHolder;
        private DevExpress.XtraBars.BarButtonItem bbiRefresh;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem bbiPrintExport;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraEditors.TextEdit teOutstandingCost;
        private DevExpress.XtraEditors.TextEdit teDeposit;
        private DevExpress.XtraEditors.TextEdit teTotalCost;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraBars.BarButtonItem bbiDetail;
        private StatusStrip statusStrip1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraGrid.GridControl gc_rateSummary;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_rateSummary;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_day;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_date;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_rateCode;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_roomRevenue;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_pkg;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_subTotal;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_serCharge;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_VAT;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_grandTotal;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_SN;
        private DevExpress.XtraBars.BarButtonItem bbiPrint;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
    }
}