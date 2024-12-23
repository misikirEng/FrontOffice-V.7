
namespace CNET.FrontOffice_V._7.Forms
{
    partial class frmRateSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRateSearch));
            repositoryItemTimeEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            rcRateSearch = new DevExpress.XtraBars.Ribbon.RibbonControl();
            beiRate = new DevExpress.XtraBars.BarEditItem();
            repositoryItemRadioGroup1 = new DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup();
            beiRateCode = new DevExpress.XtraBars.BarEditItem();
            repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            beiStatus = new DevExpress.XtraBars.BarEditItem();
            repositoryItemRadioGroup2 = new DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup();
            bbiSearch = new DevExpress.XtraBars.BarButtonItem();
            beiStartDate = new DevExpress.XtraBars.BarEditItem();
            rideStartDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            beiEndDate = new DevExpress.XtraBars.BarEditItem();
            rideEndDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            bbiRateInfo = new DevExpress.XtraBars.BarButtonItem();
            bbiSelect = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgSelect = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgDate = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgRateInfo = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgClose = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            gc_rateSearch = new DevExpress.XtraGrid.GridControl();
            gv_rateSearch = new DevExpress.XtraGrid.Views.Grid.GridView();
            statusStrip1 = new StatusStrip();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)repositoryItemTimeEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)rcRateSearch).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemRadioGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemTextEdit1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemRadioGroup2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rideStartDate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rideStartDate.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rideEndDate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rideEndDate.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gc_rateSearch).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gv_rateSearch).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            SuspendLayout();
            // 
            // repositoryItemTimeEdit1
            // 
            repositoryItemTimeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            repositoryItemTimeEdit1.Name = "repositoryItemTimeEdit1";
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(rcRateSearch);
            layoutControl1.Controls.Add(gc_rateSearch);
            layoutControl1.Controls.Add(statusStrip1);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(1157, 577);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // rcRateSearch
            // 
            rcRateSearch.Dock = DockStyle.None;
            rcRateSearch.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcRateSearch.ExpandCollapseItem.Id = 0;
            rcRateSearch.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcRateSearch.ExpandCollapseItem, rcRateSearch.SearchEditItem, beiRate, beiRateCode, beiStatus, bbiSearch, beiStartDate, beiEndDate, bbiRateInfo, bbiSelect, bbiClose });
            rcRateSearch.Location = new Point(2, 2);
            rcRateSearch.MaxItemId = 11;
            rcRateSearch.Name = "rcRateSearch";
            rcRateSearch.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcRateSearch.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repositoryItemRadioGroup1, repositoryItemTextEdit1, repositoryItemRadioGroup2, rideStartDate, rideEndDate });
            rcRateSearch.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcRateSearch.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            rcRateSearch.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcRateSearch.Size = new Size(1153, 66);
            rcRateSearch.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // beiRate
            // 
            beiRate.Caption = "Rate";
            beiRate.CaptionAlignment = DevExpress.Utils.HorzAlignment.Center;
            beiRate.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            beiRate.Edit = repositoryItemRadioGroup1;
            beiRate.EditHeight = 45;
            beiRate.EditWidth = 150;
            beiRate.Id = 1;
            beiRate.ImageOptions.Image = (Image)resources.GetObject("beiRate.ImageOptions.Image");
            beiRate.Name = "beiRate";
            beiRate.EditValueChanged += beiRate_EditValueChanged;
            // 
            // repositoryItemRadioGroup1
            // 
            repositoryItemRadioGroup1.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] { new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Average Rate"), new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Total Rate") });
            repositoryItemRadioGroup1.Name = "repositoryItemRadioGroup1";
            // 
            // beiRateCode
            // 
            beiRateCode.Caption = "Rate Code";
            beiRateCode.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            beiRateCode.Edit = repositoryItemTextEdit1;
            beiRateCode.EditHeight = 20;
            beiRateCode.EditWidth = 120;
            beiRateCode.Id = 2;
            beiRateCode.ImageOptions.Image = (Image)resources.GetObject("beiRateCode.ImageOptions.Image");
            beiRateCode.Name = "beiRateCode";
            // 
            // repositoryItemTextEdit1
            // 
            repositoryItemTextEdit1.AutoHeight = false;
            repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // beiStatus
            // 
            beiStatus.Caption = "Status";
            beiStatus.CaptionAlignment = DevExpress.Utils.HorzAlignment.Center;
            beiStatus.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            beiStatus.Edit = repositoryItemRadioGroup2;
            beiStatus.EditHeight = 80;
            beiStatus.EditWidth = 150;
            beiStatus.Id = 3;
            beiStatus.ImageOptions.Image = (Image)resources.GetObject("beiStatus.ImageOptions.Image");
            beiStatus.Name = "beiStatus";
            beiStatus.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // repositoryItemRadioGroup2
            // 
            repositoryItemRadioGroup2.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] { new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Closed"), new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Negotiated"), new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Day Use") });
            repositoryItemRadioGroup2.Name = "repositoryItemRadioGroup2";
            // 
            // bbiSearch
            // 
            bbiSearch.Caption = "Search";
            bbiSearch.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSearch.Id = 4;
            bbiSearch.ImageOptions.Image = (Image)resources.GetObject("bbiSearch.ImageOptions.Image");
            bbiSearch.Name = "bbiSearch";
            bbiSearch.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiSearch.ItemClick += bbiSearch_ItemClick;
            // 
            // beiStartDate
            // 
            beiStartDate.Caption = "Start Date";
            beiStartDate.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            beiStartDate.Edit = rideStartDate;
            beiStartDate.EditWidth = 100;
            beiStartDate.Id = 6;
            beiStartDate.Name = "beiStartDate";
            beiStartDate.EditValueChanged += beiStartDate_EditValueChanged;
            // 
            // rideStartDate
            // 
            rideStartDate.AutoHeight = false;
            rideStartDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            rideStartDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            rideStartDate.Name = "rideStartDate";
            rideStartDate.ReadOnly = true;
            rideStartDate.Validating += rideStartDate_Validating;
            // 
            // beiEndDate
            // 
            beiEndDate.Caption = "End Date  ";
            beiEndDate.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            beiEndDate.Edit = rideEndDate;
            beiEndDate.EditWidth = 100;
            beiEndDate.Id = 7;
            beiEndDate.Name = "beiEndDate";
            beiEndDate.EditValueChanged += beiEndDate_EditValueChanged;
            // 
            // rideEndDate
            // 
            rideEndDate.AutoHeight = false;
            rideEndDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            rideEndDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            rideEndDate.Name = "rideEndDate";
            rideEndDate.ReadOnly = true;
            rideEndDate.Validating += rideEndDate_Validating;
            // 
            // bbiRateInfo
            // 
            bbiRateInfo.Caption = "Rate Info";
            bbiRateInfo.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiRateInfo.Id = 8;
            bbiRateInfo.ImageOptions.Image = (Image)resources.GetObject("bbiRateInfo.ImageOptions.Image");
            bbiRateInfo.ImageOptions.LargeImage = (Image)resources.GetObject("bbiRateInfo.ImageOptions.LargeImage");
            bbiRateInfo.Name = "bbiRateInfo";
            bbiRateInfo.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiRateInfo.ItemClick += bbiRateInfo_ItemClick;
            // 
            // bbiSelect
            // 
            bbiSelect.Caption = "Select";
            bbiSelect.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiSelect.Id = 9;
            bbiSelect.ImageOptions.Image = (Image)resources.GetObject("bbiSelect.ImageOptions.Image");
            bbiSelect.Name = "bbiSelect";
            bbiSelect.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiSelect.ItemClick += bbiSelect_ItemClick;
            // 
            // bbiClose
            // 
            bbiClose.Caption = "Close";
            bbiClose.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiClose.Id = 10;
            bbiClose.ImageOptions.Image = (Image)resources.GetObject("bbiClose.ImageOptions.Image");
            bbiClose.ImageOptions.LargeImage = (Image)resources.GetObject("bbiClose.ImageOptions.LargeImage");
            bbiClose.Name = "bbiClose";
            bbiClose.ItemClick += bbiClose_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgSelect, rpgDate, ribbonPageGroup1, ribbonPageGroup2, ribbonPageGroup3, rpgRateInfo, rpgClose });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // rpgSelect
            // 
            rpgSelect.AllowTextClipping = false;
            rpgSelect.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgSelect.ItemLinks.Add(bbiSelect);
            rpgSelect.Name = "rpgSelect";
            rpgSelect.Text = "Select";
            // 
            // rpgDate
            // 
            rpgDate.AllowTextClipping = false;
            rpgDate.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgDate.ItemLinks.Add(beiStartDate);
            rpgDate.ItemLinks.Add(beiEndDate);
            rpgDate.Name = "rpgDate";
            rpgDate.Text = "Date";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.AllowTextClipping = false;
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(beiRate);
            ribbonPageGroup1.ItemLinks.Add(beiRateCode);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Rate Search Key";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.AllowTextClipping = false;
            ribbonPageGroup2.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup2.ItemLinks.Add(beiStatus);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            ribbonPageGroup2.Text = "Status";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.AllowTextClipping = false;
            ribbonPageGroup3.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup3.ItemLinks.Add(bbiSearch);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            ribbonPageGroup3.Text = "Search";
            // 
            // rpgRateInfo
            // 
            rpgRateInfo.AllowTextClipping = false;
            rpgRateInfo.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgRateInfo.ItemLinks.Add(bbiRateInfo);
            rpgRateInfo.Name = "rpgRateInfo";
            rpgRateInfo.Text = "Rate Info";
            // 
            // rpgClose
            // 
            rpgClose.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgClose.ItemLinks.Add(bbiClose);
            rpgClose.Name = "rpgClose";
            // 
            // gc_rateSearch
            // 
            gc_rateSearch.Location = new Point(2, 72);
            gc_rateSearch.MainView = gv_rateSearch;
            gc_rateSearch.MenuManager = rcRateSearch;
            gc_rateSearch.Name = "gc_rateSearch";
            gc_rateSearch.Size = new Size(1153, 450);
            gc_rateSearch.TabIndex = 7;
            gc_rateSearch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gv_rateSearch });
            // 
            // gv_rateSearch
            // 
            gv_rateSearch.Appearance.FocusedCell.BackColor = SystemColors.HotTrack;
            gv_rateSearch.Appearance.FocusedCell.ForeColor = Color.White;
            gv_rateSearch.Appearance.FocusedCell.Options.UseBackColor = true;
            gv_rateSearch.Appearance.FocusedCell.Options.UseForeColor = true;
            gv_rateSearch.Appearance.FocusedRow.BackColor = SystemColors.GradientActiveCaption;
            gv_rateSearch.Appearance.FocusedRow.ForeColor = Color.FromArgb(64, 64, 64);
            gv_rateSearch.Appearance.FocusedRow.Options.UseBackColor = true;
            gv_rateSearch.Appearance.FocusedRow.Options.UseForeColor = true;
            gv_rateSearch.Appearance.HeaderPanel.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gv_rateSearch.Appearance.HeaderPanel.Options.UseFont = true;
            gv_rateSearch.Appearance.OddRow.BackColor = SystemColors.ControlLight;
            gv_rateSearch.Appearance.OddRow.Options.UseBackColor = true;
            gv_rateSearch.Appearance.Row.Options.UseFont = true;
            gv_rateSearch.GridControl = gc_rateSearch;
            gv_rateSearch.Name = "gv_rateSearch";
            gv_rateSearch.OptionsBehavior.Editable = false;
            gv_rateSearch.OptionsSelection.EnableAppearanceHideSelection = false;
            gv_rateSearch.OptionsView.EnableAppearanceOddRow = true;
            gv_rateSearch.OptionsView.ShowGroupPanel = false;
            gv_rateSearch.OptionsView.ShowIndicator = false;
            gv_rateSearch.RowHeight = 20;
            // 
            // statusStrip1
            // 
            statusStrip1.AutoSize = false;
            statusStrip1.Dock = DockStyle.None;
            statusStrip1.Location = new Point(2, 536);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1153, 39);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem2, emptySpaceItem1, layoutControlItem3 });
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            layoutControlGroup1.Size = new Size(1157, 577);
            layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = statusStrip1;
            layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            layoutControlItem1.Location = new Point(0, 534);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(1157, 43);
            layoutControlItem1.TextSize = new Size(0, 0);
            layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = gc_rateSearch;
            layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            layoutControlItem2.Location = new Point(0, 70);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(1157, 454);
            layoutControlItem2.TextSize = new Size(0, 0);
            layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            emptySpaceItem1.Location = new Point(0, 524);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(1157, 10);
            emptySpaceItem1.TextSize = new Size(0, 0);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = rcRateSearch;
            layoutControlItem3.Location = new Point(0, 0);
            layoutControlItem3.MaxSize = new Size(0, 70);
            layoutControlItem3.MinSize = new Size(209, 70);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(1157, 70);
            layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem3.TextSize = new Size(0, 0);
            layoutControlItem3.TextVisible = false;
            // 
            // frmRateSearch
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1157, 577);
            Controls.Add(layoutControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmRateSearch.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmRateSearch";
            Text = "Rate Search";
            Load += frmRateSearch_Load;
            ((System.ComponentModel.ISupportInitialize)repositoryItemTimeEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            layoutControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)rcRateSearch).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemRadioGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemTextEdit1).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemRadioGroup2).EndInit();
            ((System.ComponentModel.ISupportInitialize)rideStartDate.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)rideStartDate).EndInit();
            ((System.ComponentModel.ISupportInitialize)rideEndDate.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)rideEndDate).EndInit();
            ((System.ComponentModel.ISupportInitialize)gc_rateSearch).EndInit();
            ((System.ComponentModel.ISupportInitialize)gv_rateSearch).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcRateSearch;
        private DevExpress.XtraBars.BarEditItem beiRate;
        private DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup repositoryItemRadioGroup1;
        private DevExpress.XtraBars.BarEditItem beiRateCode;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraBars.BarEditItem beiStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup repositoryItemRadioGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem bbiSearch;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.BarEditItem beiStartDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit rideStartDate;
        private DevExpress.XtraBars.BarEditItem beiEndDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit rideEndDate;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit repositoryItemTimeEdit1;
        private DevExpress.XtraBars.BarButtonItem bbiRateInfo;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgRateInfo;
        private DevExpress.XtraBars.BarButtonItem bbiSelect;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgSelect;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgClose;
        private DevExpress.XtraGrid.GridControl gc_rateSearch;
        private DevExpress.XtraGrid.Views.Grid.GridView gv_rateSearch;
        private StatusStrip statusStrip1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
    }
}