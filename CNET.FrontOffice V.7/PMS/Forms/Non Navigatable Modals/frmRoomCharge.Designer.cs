namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    partial class frmRoomCharge
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRoomCharge));
            gvRoomTaxPostingDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            gcRoomTaxPosting = new DevExpress.XtraGrid.GridControl();
            gvRoomTaxPostingMaster = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn141 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn142 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn143 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn144 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn145 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn146 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn147 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn148 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn149 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn150 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn151 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn152 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn153 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn154 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn155 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn156 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn157 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn158 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn159 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn160 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            rcCheckOut = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiPost = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            bsiOptions = new DevExpress.XtraBars.BarSubItem();
            bbiToday = new DevExpress.XtraBars.BarButtonItem();
            bbiEntireStay = new DevExpress.XtraBars.BarButtonItem();
            bbiNights = new DevExpress.XtraBars.BarButtonItem();
            bbiReturnCard = new DevExpress.XtraBars.BarButtonItem();
            beiFrom = new DevExpress.XtraBars.BarEditItem();
            rideFrom = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            beiTo = new DevExpress.XtraBars.BarEditItem();
            rideTo = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            beiNights = new DevExpress.XtraBars.BarEditItem();
            riteNights = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            bbiShow = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup5 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgNoNights = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)gvRoomTaxPostingDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gcRoomTaxPosting).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvRoomTaxPostingMaster).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rcCheckOut).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rideFrom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rideFrom.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rideTo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rideTo.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)riteNights).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            SuspendLayout();
            // 
            // gvRoomTaxPostingDetail
            // 
            gvRoomTaxPostingDetail.Appearance.EvenRow.BackColor = Color.FromArgb(192, 255, 255);
            gvRoomTaxPostingDetail.Appearance.EvenRow.Options.UseBackColor = true;
            gvRoomTaxPostingDetail.Appearance.OddRow.BackColor = Color.White;
            gvRoomTaxPostingDetail.Appearance.OddRow.Options.UseBackColor = true;
            gvRoomTaxPostingDetail.GridControl = gcRoomTaxPosting;
            gvRoomTaxPostingDetail.Name = "gvRoomTaxPostingDetail";
            gvRoomTaxPostingDetail.OptionsView.EnableAppearanceEvenRow = true;
            gvRoomTaxPostingDetail.OptionsView.EnableAppearanceOddRow = true;
            gvRoomTaxPostingDetail.OptionsView.ShowGroupPanel = false;
            gvRoomTaxPostingDetail.OptionsView.ShowIndicator = false;
            // 
            // gcRoomTaxPosting
            // 
            gcRoomTaxPosting.Dock = DockStyle.Fill;
            gridLevelNode1.LevelTemplate = gvRoomTaxPostingDetail;
            gridLevelNode1.RelationName = "FK_LineItem_Voucher";
            gcRoomTaxPosting.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
            gcRoomTaxPosting.Location = new Point(0, 83);
            gcRoomTaxPosting.MainView = gvRoomTaxPostingMaster;
            gcRoomTaxPosting.Name = "gcRoomTaxPosting";
            gcRoomTaxPosting.Size = new Size(913, 410);
            gcRoomTaxPosting.TabIndex = 14;
            gcRoomTaxPosting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvRoomTaxPostingMaster, gvRoomTaxPostingDetail });
            // 
            // gvRoomTaxPostingMaster
            // 
            gvRoomTaxPostingMaster.Appearance.EvenRow.BackColor = SystemColors.GradientInactiveCaption;
            gvRoomTaxPostingMaster.Appearance.EvenRow.Options.UseBackColor = true;
            gvRoomTaxPostingMaster.Appearance.FocusedCell.BackColor = SystemColors.Highlight;
            gvRoomTaxPostingMaster.Appearance.FocusedCell.ForeColor = Color.White;
            gvRoomTaxPostingMaster.Appearance.FocusedCell.Options.UseBackColor = true;
            gvRoomTaxPostingMaster.Appearance.FocusedCell.Options.UseForeColor = true;
            gvRoomTaxPostingMaster.Appearance.FocusedRow.BackColor = SystemColors.Highlight;
            gvRoomTaxPostingMaster.Appearance.FocusedRow.ForeColor = Color.White;
            gvRoomTaxPostingMaster.Appearance.FocusedRow.Options.UseBackColor = true;
            gvRoomTaxPostingMaster.Appearance.FocusedRow.Options.UseForeColor = true;
            gvRoomTaxPostingMaster.Appearance.OddRow.BackColor = Color.White;
            gvRoomTaxPostingMaster.Appearance.OddRow.Options.UseBackColor = true;
            gvRoomTaxPostingMaster.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn141, gridColumn142, gridColumn143, gridColumn144, gridColumn145, gridColumn146, gridColumn147, gridColumn148, gridColumn149, gridColumn150, gridColumn151, gridColumn152, gridColumn153, gridColumn154, gridColumn155, gridColumn156, gridColumn157, gridColumn158, gridColumn159, gridColumn160, gridColumn1 });
            gvRoomTaxPostingMaster.GridControl = gcRoomTaxPosting;
            gvRoomTaxPostingMaster.Name = "gvRoomTaxPostingMaster";
            gvRoomTaxPostingMaster.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvRoomTaxPostingMaster.OptionsSelection.EnableAppearanceHideSelection = false;
            gvRoomTaxPostingMaster.OptionsView.EnableAppearanceEvenRow = true;
            gvRoomTaxPostingMaster.OptionsView.EnableAppearanceOddRow = true;
            gvRoomTaxPostingMaster.OptionsView.ShowGroupPanel = false;
            gvRoomTaxPostingMaster.OptionsView.ShowIndicator = false;
            // 
            // gridColumn141
            // 
            gridColumn141.FieldName = "Id";
            gridColumn141.Name = "gridColumn141";
            // 
            // gridColumn142
            // 
            gridColumn142.FieldName = "Id";
            gridColumn142.Name = "gridColumn142";
            // 
            // gridColumn143
            // 
            gridColumn143.FieldName = "type";
            gridColumn143.Name = "gridColumn143";
            // 
            // gridColumn144
            // 
            gridColumn144.FieldName = "voucherDefinition";
            gridColumn144.Name = "gridColumn144";
            // 
            // gridColumn145
            // 
            gridColumn145.FieldName = "component";
            gridColumn145.Name = "gridColumn145";
            // 
            // gridColumn146
            // 
            gridColumn146.FieldName = "consignee";
            gridColumn146.Name = "gridColumn146";
            // 
            // gridColumn147
            // 
            gridColumn147.FieldName = "IssuedDate";
            gridColumn147.Name = "gridColumn147";
            // 
            // gridColumn148
            // 
            gridColumn148.FieldName = "IsIssued";
            gridColumn148.Name = "gridColumn148";
            // 
            // gridColumn149
            // 
            gridColumn149.FieldName = "year";
            gridColumn149.Name = "gridColumn149";
            // 
            // gridColumn150
            // 
            gridColumn150.FieldName = "month";
            gridColumn150.Name = "gridColumn150";
            // 
            // gridColumn151
            // 
            gridColumn151.FieldName = "date";
            gridColumn151.Name = "gridColumn151";
            // 
            // gridColumn152
            // 
            gridColumn152.FieldName = "IsVoid";
            gridColumn152.Name = "gridColumn152";
            // 
            // gridColumn153
            // 
            gridColumn153.FieldName = "grandTotal";
            gridColumn153.Name = "gridColumn153";
            // 
            // gridColumn154
            // 
            gridColumn154.FieldName = "period";
            gridColumn154.Name = "gridColumn154";
            // 
            // gridColumn155
            // 
            gridColumn155.FieldName = "remark";
            gridColumn155.Name = "gridColumn155";
            // 
            // gridColumn156
            // 
            gridColumn156.FieldName = "LastObjectState";
            gridColumn156.Name = "gridColumn156";
            // 
            // gridColumn157
            // 
            gridColumn157.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gridColumn157.AppearanceHeader.Options.UseFont = true;
            gridColumn157.Caption = "Registration No";
            gridColumn157.FieldName = "registrationNo";
            gridColumn157.Name = "gridColumn157";
            gridColumn157.Visible = true;
            gridColumn157.VisibleIndex = 0;
            // 
            // gridColumn158
            // 
            gridColumn158.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gridColumn158.AppearanceHeader.Options.UseFont = true;
            gridColumn158.Caption = "Room No";
            gridColumn158.FieldName = "roomNo";
            gridColumn158.Name = "gridColumn158";
            gridColumn158.Visible = true;
            gridColumn158.VisibleIndex = 2;
            // 
            // gridColumn159
            // 
            gridColumn159.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gridColumn159.AppearanceHeader.Options.UseFont = true;
            gridColumn159.Caption = "Consignee";
            gridColumn159.FieldName = "consignee";
            gridColumn159.Name = "gridColumn159";
            gridColumn159.Visible = true;
            gridColumn159.VisibleIndex = 3;
            // 
            // gridColumn160
            // 
            gridColumn160.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gridColumn160.AppearanceHeader.Options.UseFont = true;
            gridColumn160.Caption = "Amount";
            gridColumn160.FieldName = "amount";
            gridColumn160.Name = "gridColumn160";
            gridColumn160.Visible = true;
            gridColumn160.VisibleIndex = 4;
            // 
            // gridColumn1
            // 
            gridColumn1.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            gridColumn1.AppearanceHeader.Options.UseFont = true;
            gridColumn1.Caption = "Date";
            gridColumn1.FieldName = "date";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 1;
            // 
            // rcCheckOut
            // 
            rcCheckOut.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcCheckOut.ExpandCollapseItem.Id = 0;
            rcCheckOut.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcCheckOut.ExpandCollapseItem, rcCheckOut.SearchEditItem, bbiPost, bbiClose, bsiOptions, bbiReturnCard, bbiToday, bbiEntireStay, bbiNights, beiFrom, beiTo, beiNights, bbiShow });
            rcCheckOut.Location = new Point(0, 0);
            rcCheckOut.MaxItemId = 19;
            rcCheckOut.Name = "rcCheckOut";
            rcCheckOut.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcCheckOut.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { rideFrom, rideTo, riteNights });
            rcCheckOut.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
            rcCheckOut.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            rcCheckOut.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcCheckOut.Size = new Size(913, 83);
            rcCheckOut.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiPost
            // 
            bbiPost.Caption = "Post";
            bbiPost.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiPost.Id = 2;
            bbiPost.ImageOptions.Image = (Image)resources.GetObject("bbiPost.ImageOptions.Image");
            bbiPost.ImageOptions.LargeImage = (Image)resources.GetObject("bbiPost.ImageOptions.LargeImage");
            bbiPost.Name = "bbiPost";
            bbiPost.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiPost.ItemClick += bbiPost_ItemClick;
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
            // bsiOptions
            // 
            bsiOptions.Caption = "Options";
            bsiOptions.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bsiOptions.Id = 5;
            bsiOptions.ImageOptions.Image = (Image)resources.GetObject("bsiOptions.ImageOptions.Image");
            bsiOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(bbiToday), new DevExpress.XtraBars.LinkPersistInfo(bbiEntireStay), new DevExpress.XtraBars.LinkPersistInfo(bbiNights) });
            bsiOptions.Name = "bsiOptions";
            bsiOptions.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bbiToday
            // 
            bbiToday.Caption = "Today";
            bbiToday.Id = 9;
            bbiToday.Name = "bbiToday";
            bbiToday.ItemClick += bbiToday_ItemClick;
            // 
            // bbiEntireStay
            // 
            bbiEntireStay.Caption = "Entire Stay";
            bbiEntireStay.Id = 10;
            bbiEntireStay.Name = "bbiEntireStay";
            bbiEntireStay.ItemClick += bbiEntireStay_ItemClick;
            // 
            // bbiNights
            // 
            bbiNights.Caption = "Nights";
            bbiNights.Id = 11;
            bbiNights.Name = "bbiNights";
            bbiNights.ItemClick += bbiNights_ItemClick;
            // 
            // bbiReturnCard
            // 
            bbiReturnCard.Caption = "View";
            bbiReturnCard.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiReturnCard.Id = 6;
            bbiReturnCard.ImageOptions.Image = (Image)resources.GetObject("bbiReturnCard.ImageOptions.Image");
            bbiReturnCard.ImageOptions.LargeImage = (Image)resources.GetObject("bbiReturnCard.ImageOptions.LargeImage");
            bbiReturnCard.Name = "bbiReturnCard";
            // 
            // beiFrom
            // 
            beiFrom.Caption = "From";
            beiFrom.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            beiFrom.Edit = rideFrom;
            beiFrom.EditWidth = 100;
            beiFrom.Id = 12;
            beiFrom.Name = "beiFrom";
            // 
            // rideFrom
            // 
            rideFrom.AutoHeight = false;
            rideFrom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            rideFrom.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            rideFrom.Name = "rideFrom";
            rideFrom.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // beiTo
            // 
            beiTo.Caption = "    To";
            beiTo.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            beiTo.Edit = rideTo;
            beiTo.EditWidth = 100;
            beiTo.Id = 13;
            beiTo.Name = "beiTo";
            // 
            // rideTo
            // 
            rideTo.AutoHeight = false;
            rideTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            rideTo.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            rideTo.Name = "rideTo";
            rideTo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // beiNights
            // 
            beiNights.Caption = "No. of Nights";
            beiNights.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            beiNights.Edit = riteNights;
            beiNights.EditWidth = 100;
            beiNights.Id = 15;
            beiNights.Name = "beiNights";
            // 
            // riteNights
            // 
            riteNights.AutoHeight = false;
            riteNights.Mask.EditMask = "\\d{0,3}";
            riteNights.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            riteNights.Name = "riteNights";
            // 
            // bbiShow
            // 
            bbiShow.Caption = "Show";
            bbiShow.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiShow.Id = 17;
            bbiShow.ImageOptions.Image = (Image)resources.GetObject("bbiShow.ImageOptions.Image");
            bbiShow.ImageOptions.LargeImage = (Image)resources.GetObject("bbiShow.ImageOptions.LargeImage");
            bbiShow.Name = "bbiShow";
            bbiShow.ItemClick += bbiShow_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup2, ribbonPageGroup4, ribbonPageGroup1, ribbonPageGroup5, ribbonPageGroup3, rpgNoNights });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup2.ItemLinks.Add(bbiPost);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // ribbonPageGroup4
            // 
            ribbonPageGroup4.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup4.ItemLinks.Add(bbiShow);
            ribbonPageGroup4.Name = "ribbonPageGroup4";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(bsiOptions);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // ribbonPageGroup5
            // 
            ribbonPageGroup5.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup5.ItemLinks.Add(bbiClose);
            ribbonPageGroup5.Name = "ribbonPageGroup5";
            // 
            // ribbonPageGroup3
            // 
            ribbonPageGroup3.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup3.ItemLinks.Add(beiFrom);
            ribbonPageGroup3.ItemLinks.Add(beiTo);
            ribbonPageGroup3.Name = "ribbonPageGroup3";
            // 
            // rpgNoNights
            // 
            rpgNoNights.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgNoNights.ItemLinks.Add(beiNights);
            rpgNoNights.Name = "rpgNoNights";
            rpgNoNights.Visible = false;
            // 
            // layoutControl1
            // 
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 83);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(913, 410);
            layoutControl1.TabIndex = 1;
            layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(913, 410);
            layoutControlGroup1.TextVisible = false;
            // 
            // frmRoomCharge
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(913, 493);
            Controls.Add(gcRoomTaxPosting);
            Controls.Add(layoutControl1);
            Controls.Add(rcCheckOut);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmRoomCharge.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmRoomCharge";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Room Charge";
            Load += frmRoomCharge_Load;
            ((System.ComponentModel.ISupportInitialize)gvRoomTaxPostingDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)gcRoomTaxPosting).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvRoomTaxPostingMaster).EndInit();
            ((System.ComponentModel.ISupportInitialize)rcCheckOut).EndInit();
            ((System.ComponentModel.ISupportInitialize)rideFrom.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)rideFrom).EndInit();
            ((System.ComponentModel.ISupportInitialize)rideTo.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)rideTo).EndInit();
            ((System.ComponentModel.ISupportInitialize)riteNights).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl rcCheckOut;
        private DevExpress.XtraBars.BarButtonItem bbiPost;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.BarSubItem bsiOptions;
        private DevExpress.XtraBars.BarButtonItem bbiReturnCard;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup5;
        private DevExpress.XtraBars.BarButtonItem bbiToday;
        private DevExpress.XtraBars.BarButtonItem bbiEntireStay;
        private DevExpress.XtraBars.BarButtonItem bbiNights;
        private DevExpress.XtraBars.BarEditItem beiFrom;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit rideFrom;
        private DevExpress.XtraBars.BarEditItem beiTo;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit rideTo;
        private DevExpress.XtraBars.BarEditItem beiNights;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit riteNights;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgNoNights;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gcRoomTaxPosting;
        private DevExpress.XtraGrid.Views.Grid.GridView gvRoomTaxPostingDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gvRoomTaxPostingMaster;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn141;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn142;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn143;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn144;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn145;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn146;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn147;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn148;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn149;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn150;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn151;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn152;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn153;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn154;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn155;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn156;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn157;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn158;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn159;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn160;
        private DevExpress.XtraBars.BarButtonItem bbiShow;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
    }
}