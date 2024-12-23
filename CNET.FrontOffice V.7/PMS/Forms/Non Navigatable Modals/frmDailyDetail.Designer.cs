
using DevExpress.XtraEditors;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    partial class frmDailyDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDailyDetail));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            gc_dailyDetail = new DevExpress.XtraGrid.GridControl();
            gv_dailyDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            gCol_SN = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_day = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_date = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_rate = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_currency = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_rateAmt = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_fixed = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_roomType = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_room = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_actualRTC = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_adult = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_child = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_disAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_percent = new DevExpress.XtraGrid.Columns.GridColumn();
            rcDailyDetail = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiNew = new DevExpress.XtraBars.BarButtonItem();
            bbiSave = new DevExpress.XtraBars.BarButtonItem();
            bbiDelete = new DevExpress.XtraBars.BarButtonItem();
            bsiOptions = new DevExpress.XtraBars.BarSubItem();
            barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            bbiRateInfo = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgRateInfo = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonStatusBar1 = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            panelControl1 = new PanelControl();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            lciRibbonHolder = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl2).BeginInit();
            layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gc_dailyDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gv_dailyDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rcDailyDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciRibbonHolder).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(layoutControl2);
            layoutControl1.Controls.Add(ribbonStatusBar1);
            layoutControl1.Controls.Add(panelControl1);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(851, 510);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl2
            // 
            layoutControl2.Controls.Add(gc_dailyDetail);
            layoutControl2.Location = new Point(2, 68);
            layoutControl2.Name = "layoutControl2";
            layoutControl2.Root = Root;
            layoutControl2.Size = new Size(847, 416);
            layoutControl2.TabIndex = 5;
            layoutControl2.Text = "layoutControl2";
            // 
            // gc_dailyDetail
            // 
            gc_dailyDetail.Location = new Point(12, 12);
            gc_dailyDetail.MainView = gv_dailyDetail;
            gc_dailyDetail.MenuManager = rcDailyDetail;
            gc_dailyDetail.Name = "gc_dailyDetail";
            gc_dailyDetail.Size = new Size(823, 392);
            gc_dailyDetail.TabIndex = 4;
            gc_dailyDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gv_dailyDetail });
            // 
            // gv_dailyDetail
            // 
            gv_dailyDetail.Appearance.FocusedCell.BackColor = SystemColors.Highlight;
            gv_dailyDetail.Appearance.FocusedCell.Options.UseBackColor = true;
            gv_dailyDetail.Appearance.FocusedRow.BackColor = SystemColors.Highlight;
            gv_dailyDetail.Appearance.FocusedRow.Options.UseBackColor = true;
            gv_dailyDetail.Appearance.OddRow.BackColor = SystemColors.ControlLight;
            gv_dailyDetail.Appearance.OddRow.Options.UseBackColor = true;
            gv_dailyDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gCol_SN, gCol_day, gCol_date, gCol_rate, gCol_currency, gCol_rateAmt, gCol_fixed, gCol_roomType, gCol_room, gCol_actualRTC, gCol_adult, gCol_child, gCol_disAmount, gCol_percent });
            gv_dailyDetail.GridControl = gc_dailyDetail;
            gv_dailyDetail.Name = "gv_dailyDetail";
            gv_dailyDetail.OptionsView.EnableAppearanceOddRow = true;
            gv_dailyDetail.OptionsView.ShowGroupPanel = false;
            gv_dailyDetail.OptionsView.ShowIndicator = false;
            // 
            // gCol_SN
            // 
            gCol_SN.Caption = "SN";
            gCol_SN.Name = "gCol_SN";
            gCol_SN.Visible = true;
            gCol_SN.VisibleIndex = 0;
            gCol_SN.Width = 31;
            // 
            // gCol_day
            // 
            gCol_day.Caption = "Day";
            gCol_day.FieldName = "day";
            gCol_day.Name = "gCol_day";
            gCol_day.Visible = true;
            gCol_day.VisibleIndex = 1;
            gCol_day.Width = 46;
            // 
            // gCol_date
            // 
            gCol_date.Caption = "Date";
            gCol_date.FieldName = "date";
            gCol_date.Name = "gCol_date";
            gCol_date.Visible = true;
            gCol_date.VisibleIndex = 2;
            gCol_date.Width = 60;
            // 
            // gCol_rate
            // 
            gCol_rate.Caption = "Rate";
            gCol_rate.FieldName = "RateCode";
            gCol_rate.Name = "gCol_rate";
            gCol_rate.Visible = true;
            gCol_rate.VisibleIndex = 3;
            gCol_rate.Width = 77;
            // 
            // gCol_currency
            // 
            gCol_currency.Caption = "Currency";
            gCol_currency.Name = "gCol_currency";
            gCol_currency.Visible = true;
            gCol_currency.VisibleIndex = 4;
            gCol_currency.Width = 57;
            // 
            // gCol_rateAmt
            // 
            gCol_rateAmt.Caption = "Rate Amount";
            gCol_rateAmt.FieldName = "RateAmount";
            gCol_rateAmt.Name = "gCol_rateAmt";
            gCol_rateAmt.Visible = true;
            gCol_rateAmt.VisibleIndex = 5;
            gCol_rateAmt.Width = 72;
            // 
            // gCol_fixed
            // 
            gCol_fixed.Caption = "Is Fixed";
            gCol_fixed.FieldName = "IsFixedRate";
            gCol_fixed.Name = "gCol_fixed";
            gCol_fixed.Visible = true;
            gCol_fixed.VisibleIndex = 6;
            gCol_fixed.Width = 50;
            // 
            // gCol_roomType
            // 
            gCol_roomType.Caption = "Room Type";
            gCol_roomType.FieldName = "RoomType";
            gCol_roomType.Name = "gCol_roomType";
            gCol_roomType.Visible = true;
            gCol_roomType.VisibleIndex = 7;
            gCol_roomType.Width = 94;
            // 
            // gCol_room
            // 
            gCol_room.Caption = "Room";
            gCol_room.FieldName = "Room";
            gCol_room.Name = "gCol_room";
            gCol_room.Visible = true;
            gCol_room.VisibleIndex = 8;
            gCol_room.Width = 59;
            // 
            // gCol_actualRTC
            // 
            gCol_actualRTC.Caption = "Actual RTC";
            gCol_actualRTC.FieldName = "ActualRTC";
            gCol_actualRTC.Name = "gCol_actualRTC";
            gCol_actualRTC.Visible = true;
            gCol_actualRTC.VisibleIndex = 9;
            gCol_actualRTC.Width = 73;
            // 
            // gCol_adult
            // 
            gCol_adult.Caption = "Adult";
            gCol_adult.FieldName = "Adult";
            gCol_adult.Name = "gCol_adult";
            gCol_adult.Visible = true;
            gCol_adult.VisibleIndex = 10;
            gCol_adult.Width = 34;
            // 
            // gCol_child
            // 
            gCol_child.Caption = "Child";
            gCol_child.FieldName = "Child";
            gCol_child.Name = "gCol_child";
            gCol_child.Visible = true;
            gCol_child.VisibleIndex = 11;
            gCol_child.Width = 33;
            // 
            // gCol_disAmount
            // 
            gCol_disAmount.Caption = "Dis. Amount";
            gCol_disAmount.FieldName = "disAmt";
            gCol_disAmount.Name = "gCol_disAmount";
            gCol_disAmount.Visible = true;
            gCol_disAmount.VisibleIndex = 12;
            gCol_disAmount.Width = 68;
            // 
            // gCol_percent
            // 
            gCol_percent.Caption = "%";
            gCol_percent.FieldName = "percent";
            gCol_percent.Name = "gCol_percent";
            gCol_percent.Visible = true;
            gCol_percent.VisibleIndex = 13;
            gCol_percent.Width = 67;
            // 
            // rcDailyDetail
            // 
            rcDailyDetail.ApplicationButtonImageOptions.Image = (Image)resources.GetObject("rcDailyDetail.ApplicationButtonImageOptions.Image");
            rcDailyDetail.ExpandCollapseItem.Id = 0;
            rcDailyDetail.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcDailyDetail.ExpandCollapseItem, rcDailyDetail.SearchEditItem, bbiNew, bbiSave, bbiDelete, bsiOptions, barButtonItem1, bbiRateInfo });
            rcDailyDetail.Location = new Point(2, 2);
            rcDailyDetail.MaxItemId = 8;
            rcDailyDetail.Name = "rcDailyDetail";
            rcDailyDetail.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcDailyDetail.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcDailyDetail.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcDailyDetail.Size = new Size(843, 83);
            rcDailyDetail.StatusBar = ribbonStatusBar1;
            rcDailyDetail.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiNew
            // 
            bbiNew.Caption = "New";
            bbiNew.Id = 1;
            bbiNew.ImageOptions.Image = (Image)resources.GetObject("bbiNew.ImageOptions.Image");
            bbiNew.Name = "bbiNew";
            bbiNew.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            bbiNew.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bbiSave
            // 
            bbiSave.Caption = "Save";
            bbiSave.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSave.Id = 2;
            bbiSave.ImageOptions.Image = (Image)resources.GetObject("bbiSave.ImageOptions.Image");
            bbiSave.Name = "bbiSave";
            bbiSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            bbiSave.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bbiDelete
            // 
            bbiDelete.Caption = "Delete";
            bbiDelete.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiDelete.Id = 3;
            bbiDelete.ImageOptions.Image = (Image)resources.GetObject("bbiDelete.ImageOptions.Image");
            bbiDelete.Name = "bbiDelete";
            bbiDelete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            bbiDelete.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bsiOptions
            // 
            bsiOptions.Caption = "Options";
            bsiOptions.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bsiOptions.Id = 5;
            bsiOptions.ImageOptions.Image = (Image)resources.GetObject("bsiOptions.ImageOptions.Image");
            bsiOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(barButtonItem1) });
            bsiOptions.Name = "bsiOptions";
            bsiOptions.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // barButtonItem1
            // 
            barButtonItem1.Caption = "Option 1";
            barButtonItem1.Id = 6;
            barButtonItem1.Name = "barButtonItem1";
            // 
            // bbiRateInfo
            // 
            bbiRateInfo.Caption = "Rate Info";
            bbiRateInfo.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiRateInfo.Id = 7;
            bbiRateInfo.Name = "bbiRateInfo";
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, ribbonPageGroup2, ribbonPageGroup3, rpgRateInfo });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(bbiNew);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = " New";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(bbiSave);
            ribbonPageGroup2.ItemLinks.Add(bbiDelete);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            ribbonPageGroup2.Text = " Save and Delete";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.ItemLinks.Add(bsiOptions);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            ribbonPageGroup3.Text = "Options";
            // 
            // rpgRateInfo
            // 
            rpgRateInfo.Name = "rpgRateInfo";
            rpgRateInfo.Text = "Rate Info";
            // 
            // ribbonStatusBar1
            // 
            ribbonStatusBar1.Dock = DockStyle.None;
            ribbonStatusBar1.Location = new Point(2, 488);
            ribbonStatusBar1.Name = "ribbonStatusBar1";
            ribbonStatusBar1.Ribbon = rcDailyDetail;
            ribbonStatusBar1.Size = new Size(847, 27);
            // 
            // Root
            // 
            Root.CustomizationFormText = "Root";
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem3 });
            Root.Name = "Root";
            Root.Size = new Size(847, 416);
            Root.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = gc_dailyDetail;
            layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            layoutControlItem3.Location = new Point(0, 0);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(827, 396);
            layoutControlItem3.TextSize = new Size(0, 0);
            layoutControlItem3.TextVisible = false;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(rcDailyDetail);
            panelControl1.Location = new Point(2, 2);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(847, 62);
            panelControl1.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciRibbonHolder, layoutControlItem2, layoutControlItem1 });
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup1.Size = new Size(851, 510);
            layoutControlGroup1.TextVisible = false;
            // 
            // lciRibbonHolder
            // 
            lciRibbonHolder.Control = panelControl1;
            lciRibbonHolder.CustomizationFormText = "lciRibbonHolder";
            lciRibbonHolder.Location = new Point(0, 0);
            lciRibbonHolder.Name = "lciRibbonHolder";
            lciRibbonHolder.Size = new Size(851, 66);
            lciRibbonHolder.TextSize = new Size(0, 0);
            lciRibbonHolder.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = ribbonStatusBar1;
            layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            layoutControlItem2.Location = new Point(0, 486);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(851, 24);
            layoutControlItem2.TextSize = new Size(0, 0);
            layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = layoutControl2;
            layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            layoutControlItem1.Location = new Point(0, 66);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(851, 420);
            layoutControlItem1.TextSize = new Size(0, 0);
            layoutControlItem1.TextVisible = false;
            // 
            // frmDailyDetail
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(851, 510);
            Controls.Add(layoutControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmDailyDetail.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmDailyDetail";
            Text = "Daily Detail";
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)layoutControl2).EndInit();
            layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gc_dailyDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)gv_dailyDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)rcDailyDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciRibbonHolder).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private PanelControl panelControl1;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcDailyDetail;
        private DevExpress.XtraBars.BarButtonItem bbiNew;
        private DevExpress.XtraBars.BarButtonItem bbiSave;
        private DevExpress.XtraBars.BarButtonItem bbiDelete;
        private DevExpress.XtraBars.BarSubItem bsiOptions;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraLayout.LayoutControlItem lciRibbonHolder;
        private DevExpress.XtraBars.BarButtonItem bbiRateInfo;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgRateInfo;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraGrid.GridControl gc_dailyDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_dailyDetail;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_SN;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_day;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_date;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_rate;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_currency;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_rateAmt;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_fixed;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_roomType;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_room;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_actualRTC;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_adult;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_child;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_disAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_percent;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}