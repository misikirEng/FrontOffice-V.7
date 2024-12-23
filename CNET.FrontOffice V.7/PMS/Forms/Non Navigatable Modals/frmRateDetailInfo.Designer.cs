using CNET.ERP.ResourceProvider;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    partial class frmRateDetailInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRateDetailInfo));
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiPrintPreview = new DevExpress.XtraBars.BarButtonItem();
            bbiRefresh = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgRefresh = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgPrintPreview = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgClose = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            statusStrip1 = new StatusStrip();
            gc_rateDetailInfo = new DevExpress.XtraGrid.GridControl();
            gv_rateDetailInfo = new DevExpress.XtraGrid.Views.Grid.GridView();
            gCol_Date = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_weekDay = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_description = new DevExpress.XtraGrid.Columns.GridColumn();
            gCol_price = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gc_rateDetailInfo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gv_rateDetailInfo).BeginInit();
            SuspendLayout();
            // 
            // ribbonControl1
            // 
            ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, bbiPrintPreview, bbiRefresh, bbiClose, ribbonControl1.SearchEditItem });
            ribbonControl1.Location = new Point(0, 0);
            ribbonControl1.MaxItemId = 4;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            ribbonControl1.Size = new Size(541, 66);
            ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiPrintPreview
            // 
            bbiPrintPreview.Caption = "Print Preview";
            bbiPrintPreview.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiPrintPreview.Id = 1;
            bbiPrintPreview.ImageOptions.Image = (Image)resources.GetObject("bbiPrintPreview.ImageOptions.Image");
            bbiPrintPreview.Name = "bbiPrintPreview";
            bbiPrintPreview.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bbiRefresh
            // 
            bbiRefresh.Caption = "New";
            bbiRefresh.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiRefresh.Id = 2;
            bbiRefresh.ImageOptions.Image = (Image)resources.GetObject("bbiRefresh.ImageOptions.Image");
            bbiRefresh.ImageOptions.LargeImage = (Image)resources.GetObject("bbiRefresh.ImageOptions.LargeImage");
            bbiRefresh.Name = "bbiRefresh";
            bbiRefresh.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
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
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgRefresh, rpgPrintPreview, rpgClose });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // rpgRefresh
            // 
            rpgRefresh.AllowTextClipping = false;
            rpgRefresh.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgRefresh.ItemLinks.Add(bbiRefresh);
            rpgRefresh.Name = "rpgRefresh";
            rpgRefresh.Text = "Refresh";
            // 
            // rpgPrintPreview
            // 
            rpgPrintPreview.AllowTextClipping = false;
            rpgPrintPreview.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgPrintPreview.ItemLinks.Add(bbiPrintPreview);
            rpgPrintPreview.Name = "rpgPrintPreview";
            rpgPrintPreview.Text = "Print Preview";
            // 
            // rpgClose
            // 
            rpgClose.AllowTextClipping = false;
            rpgClose.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgClose.ItemLinks.Add(bbiClose);
            rpgClose.Name = "rpgClose";
            rpgClose.Text = "Close";
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(0, 507);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(541, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // gc_rateDetailInfo
            // 
            gc_rateDetailInfo.Dock = DockStyle.Fill;
            gc_rateDetailInfo.Location = new Point(0, 66);
            gc_rateDetailInfo.MainView = gv_rateDetailInfo;
            gc_rateDetailInfo.MenuManager = ribbonControl1;
            gc_rateDetailInfo.Name = "gc_rateDetailInfo";
            gc_rateDetailInfo.Size = new Size(541, 441);
            gc_rateDetailInfo.TabIndex = 5;
            gc_rateDetailInfo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gv_rateDetailInfo });
            // 
            // gv_rateDetailInfo
            // 
            gv_rateDetailInfo.Appearance.FocusedCell.BackColor = SystemColors.Highlight;
            gv_rateDetailInfo.Appearance.FocusedCell.ForeColor = Color.White;
            gv_rateDetailInfo.Appearance.FocusedCell.Options.UseBackColor = true;
            gv_rateDetailInfo.Appearance.FocusedCell.Options.UseForeColor = true;
            gv_rateDetailInfo.Appearance.FocusedRow.BackColor = SystemColors.Highlight;
            gv_rateDetailInfo.Appearance.FocusedRow.ForeColor = Color.White;
            gv_rateDetailInfo.Appearance.FocusedRow.Options.UseBackColor = true;
            gv_rateDetailInfo.Appearance.FocusedRow.Options.UseForeColor = true;
            gv_rateDetailInfo.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gCol_Date, gCol_weekDay, gCol_description, gCol_price });
            gv_rateDetailInfo.GridControl = gc_rateDetailInfo;
            gv_rateDetailInfo.Name = "gv_rateDetailInfo";
            gv_rateDetailInfo.OptionsBehavior.Editable = false;
            gv_rateDetailInfo.OptionsSelection.EnableAppearanceFocusedCell = false;
            gv_rateDetailInfo.OptionsSelection.EnableAppearanceHideSelection = false;
            gv_rateDetailInfo.OptionsView.ShowGroupPanel = false;
            gv_rateDetailInfo.OptionsView.ShowIndicator = false;
            // 
            // gCol_Date
            // 
            gCol_Date.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_Date.AppearanceHeader.Options.UseFont = true;
            gCol_Date.Caption = "Date";
            gCol_Date.FieldName = "StayDate";
            gCol_Date.Name = "gCol_Date";
            gCol_Date.Visible = true;
            gCol_Date.VisibleIndex = 0;
            // 
            // gCol_weekDay
            // 
            gCol_weekDay.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_weekDay.AppearanceHeader.Options.UseFont = true;
            gCol_weekDay.Caption = "Week Day";
            gCol_weekDay.FieldName = "DayWeek";
            gCol_weekDay.Name = "gCol_weekDay";
            gCol_weekDay.Visible = true;
            gCol_weekDay.VisibleIndex = 1;
            // 
            // gCol_description
            // 
            gCol_description.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_description.AppearanceHeader.Options.UseFont = true;
            gCol_description.Caption = "Rate Detail";
            gCol_description.FieldName = "Description";
            gCol_description.Name = "gCol_description";
            gCol_description.Visible = true;
            gCol_description.VisibleIndex = 2;
            // 
            // gCol_price
            // 
            gCol_price.AppearanceCell.Options.UseTextOptions = true;
            gCol_price.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gCol_price.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gCol_price.AppearanceHeader.Options.UseFont = true;
            gCol_price.Caption = "Price";
            gCol_price.FieldName = "UnitRoomRate";
            gCol_price.Name = "gCol_price";
            gCol_price.Visible = true;
            gCol_price.VisibleIndex = 3;
            // 
            // frmRateDetailInfo
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(541, 529);
            Controls.Add(gc_rateDetailInfo);
            Controls.Add(statusStrip1);
            Controls.Add(ribbonControl1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            IconOptions.Icon = (Icon)resources.GetObject("frmRateDetailInfo.IconOptions.Icon");
            Name = "frmRateDetailInfo";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Rate Info";
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gc_rateDetailInfo).EndInit();
            ((System.ComponentModel.ISupportInitialize)gv_rateDetailInfo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem bbiPrintPreview;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgRefresh;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgPrintPreview;
        public DevExpress.XtraBars.BarButtonItem bbiRefresh;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgClose;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private DevExpress.XtraGrid.GridControl gc_rateDetailInfo;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_rateDetailInfo;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_Date;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_weekDay;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_description;
        private DevExpress.XtraGrid.Columns.GridColumn gCol_price;
    }
}