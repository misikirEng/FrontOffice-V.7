namespace CNET.ERP.Client.UI_Logic.PMS.Forms
{
    partial class frmReservationSearch
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
            CNETDataEditor.Logic logic1 = new CNETDataEditor.Logic();
            ImplementationDefault.CNETDataEditor.Property.CNETDataEditorProperty cnetDataEditorProperty1 = new ImplementationDefault.CNETDataEditor.Property.CNETDataEditorProperty();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReservationSearch));
            this.gcolRegistration = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRoom = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRoomType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolArrival = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolDeparture = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolRooms = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolTax = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolGroup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cdeRegistrationSearch = new CNETDataEditor.CNETDataEditor();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.rcRegistrationSearch = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.bbiSearch = new DevExpress.XtraBars.BarButtonItem();
            this.rpMain = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciRibbonHolder = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemTextEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemTextEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.repositoryItemTextEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rcRegistrationSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRibbonHolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit5)).BeginInit();
            this.SuspendLayout();
            // 
            // gcolRegistration
            // 
            this.gcolRegistration.Caption = "Registartion";
            this.gcolRegistration.FieldName = "registration";
            this.gcolRegistration.Name = "gcolRegistration";
            this.gcolRegistration.Visible = true;
            this.gcolRegistration.VisibleIndex = 0;
            // 
            // gcolName
            // 
            this.gcolName.Caption = "Name";
            this.gcolName.FieldName = "name";
            this.gcolName.Name = "gcolName";
            this.gcolName.Visible = true;
            this.gcolName.VisibleIndex = 1;
            // 
            // gcolRoom
            // 
            this.gcolRoom.Caption = "Room";
            this.gcolRoom.FieldName = "room";
            this.gcolRoom.Name = "gcolRoom";
            this.gcolRoom.Visible = true;
            this.gcolRoom.VisibleIndex = 2;
            // 
            // gcolRoomType
            // 
            this.gcolRoomType.Caption = "Room Type";
            this.gcolRoomType.FieldName = "roomType";
            this.gcolRoomType.Name = "gcolRoomType";
            this.gcolRoomType.Visible = true;
            this.gcolRoomType.VisibleIndex = 3;
            // 
            // gcolArrival
            // 
            this.gcolArrival.Caption = "Arrival";
            this.gcolArrival.FieldName = "arrival";
            this.gcolArrival.Name = "gcolArrival";
            this.gcolArrival.Visible = true;
            this.gcolArrival.VisibleIndex = 4;
            // 
            // gcolDeparture
            // 
            this.gcolDeparture.Caption = "Departure";
            this.gcolDeparture.FieldName = "departure";
            this.gcolDeparture.Name = "gcolDeparture";
            this.gcolDeparture.Visible = true;
            this.gcolDeparture.VisibleIndex = 5;
            // 
            // gcolRooms
            // 
            this.gcolRooms.Caption = "Rooms";
            this.gcolRooms.FieldName = "rooms";
            this.gcolRooms.Name = "gcolRooms";
            this.gcolRooms.Visible = true;
            this.gcolRooms.VisibleIndex = 6;
            // 
            // gcolTax
            // 
            this.gcolTax.Caption = "Tax";
            this.gcolTax.FieldName = "tax";
            this.gcolTax.Name = "gcolTax";
            this.gcolTax.Visible = true;
            this.gcolTax.VisibleIndex = 7;
            // 
            // gcolStatus
            // 
            this.gcolStatus.Caption = "Status";
            this.gcolStatus.FieldName = "status";
            this.gcolStatus.Name = "gcolStatus";
            this.gcolStatus.Visible = true;
            this.gcolStatus.VisibleIndex = 8;
            // 
            // gcolGroup
            // 
            this.gcolGroup.Caption = "Group";
            this.gcolGroup.FieldName = "group";
            this.gcolGroup.Name = "gcolGroup";
            this.gcolGroup.Visible = true;
            this.gcolGroup.VisibleIndex = 9;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.statusStrip1);
            this.layoutControl1.Controls.Add(this.cdeRegistrationSearch);
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(871, 497);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Location = new System.Drawing.Point(2, 475);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(867, 20);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // cdeRegistrationSearch
            // 
            this.cdeRegistrationSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cdeRegistrationSearch.Location = new System.Drawing.Point(2, 101);
            logic1.NewRowKeyText = "New";
            this.cdeRegistrationSearch.Logic = logic1;
            this.cdeRegistrationSearch.MenuDockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.cdeRegistrationSearch.Name = "cdeRegistrationSearch";
            cnetDataEditorProperty1.AllowAddingNewRecord = true;
            cnetDataEditorProperty1.AllowDeleteingRecord = true;
            cnetDataEditorProperty1.AllowEditingRecord = true;
            cnetDataEditorProperty1.AlwaysShowNewRow = true;
            cnetDataEditorProperty1.ApplyCnetFieldFormat = false;
            cnetDataEditorProperty1.ApplyCnetObjectState = false;
            cnetDataEditorProperty1.AutoRemoveBlankRows = true;
            cnetDataEditorProperty1.CNETComponent = "";
            cnetDataEditorProperty1.CNETFieldType = "";
            cnetDataEditorProperty1.CnetIconColumnNames = new string[0];
            cnetDataEditorProperty1.CNETObjectType = "";
            cnetDataEditorProperty1.Column = null;
            cnetDataEditorProperty1.ColumnFields = new string[0];
            cnetDataEditorProperty1.Columns = new DevExpress.XtraGrid.Columns.GridColumn[] {
        this.gcolRegistration,
        this.gcolName,
        this.gcolRoom,
        this.gcolRoomType,
        this.gcolArrival,
        this.gcolDeparture,
        this.gcolRooms,
        this.gcolTax,
        this.gcolStatus,
        this.gcolGroup};
            cnetDataEditorProperty1.DisplayType = CNETDataEditor.Enums.DisplayType.Grid;
            cnetDataEditorProperty1.IconSize = CNETIconAndImagePovider.Enum.PictureSize.Dimension_16X16;
            cnetDataEditorProperty1.KeyFieldColumn = "Id";
            cnetDataEditorProperty1.KeyFieldName = "Id";
            cnetDataEditorProperty1.NotEditablecolumnFields = new string[0];
            cnetDataEditorProperty1.ParentFieldName = "parent";
            cnetDataEditorProperty1.RowColorEven = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            cnetDataEditorProperty1.RowColorOdd = System.Drawing.Color.WhiteSmoke;
            cnetDataEditorProperty1.ShowColumnHeader = true;
            cnetDataEditorProperty1.ShowContextMenu = true;
            cnetDataEditorProperty1.ShowSN = true;
            cnetDataEditorProperty1.UseAsSelector = false;
            cnetDataEditorProperty1.UseCNETIconLibrary = false;
            cnetDataEditorProperty1.UseDataProcessor = true;
            cnetDataEditorProperty1.UseZebraColorStyle = true;
            this.cdeRegistrationSearch.Properties = cnetDataEditorProperty1;
            this.cdeRegistrationSearch.Size = new System.Drawing.Size(867, 370);
            this.cdeRegistrationSearch.TabIndex = 5;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.rcRegistrationSearch);
            this.panelControl1.Location = new System.Drawing.Point(2, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(867, 95);
            this.panelControl1.TabIndex = 4;
            // 
            // rcRegistrationSearch
            // 
            this.rcRegistrationSearch.ExpandCollapseItem.Id = 0;
            this.rcRegistrationSearch.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.rcRegistrationSearch.ExpandCollapseItem,
            this.bbiSearch});
            this.rcRegistrationSearch.Location = new System.Drawing.Point(2, 2);
            this.rcRegistrationSearch.MaxItemId = 2;
            this.rcRegistrationSearch.Name = "rcRegistrationSearch";
            this.rcRegistrationSearch.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.rpMain});
            this.rcRegistrationSearch.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            this.rcRegistrationSearch.Size = new System.Drawing.Size(863, 96);
            this.rcRegistrationSearch.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiSearch
            // 
            this.bbiSearch.Caption = "Search";
            this.bbiSearch.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            this.bbiSearch.Glyph = ((System.Drawing.Image)(resources.GetObject("bbiSearch.Glyph")));
            this.bbiSearch.Id = 1;
            this.bbiSearch.Name = "bbiSearch";
            this.bbiSearch.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // rpMain
            // 
            this.rpMain.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.rpMain.Name = "rpMain";
            this.rpMain.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.bbiSearch);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = " ";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciRibbonHolder,
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(871, 497);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciRibbonHolder
            // 
            this.lciRibbonHolder.Control = this.panelControl1;
            this.lciRibbonHolder.CustomizationFormText = "lciRibbonHolder";
            this.lciRibbonHolder.Location = new System.Drawing.Point(0, 0);
            this.lciRibbonHolder.Name = "lciRibbonHolder";
            this.lciRibbonHolder.Size = new System.Drawing.Size(871, 99);
            this.lciRibbonHolder.Text = "lciRibbonHolder";
            this.lciRibbonHolder.TextSize = new System.Drawing.Size(0, 0);
            this.lciRibbonHolder.TextToControlDistance = 0;
            this.lciRibbonHolder.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cdeRegistrationSearch;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 99);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(871, 374);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.statusStrip1;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 473);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(871, 24);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // repositoryItemTextEdit2
            // 
            this.repositoryItemTextEdit2.AutoHeight = false;
            this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
            // 
            // repositoryItemTextEdit3
            // 
            this.repositoryItemTextEdit3.AutoHeight = false;
            this.repositoryItemTextEdit3.Name = "repositoryItemTextEdit3";
            // 
            // repositoryItemTextEdit4
            // 
            this.repositoryItemTextEdit4.AutoHeight = false;
            this.repositoryItemTextEdit4.Name = "repositoryItemTextEdit4";
            // 
            // repositoryItemTextEdit5
            // 
            this.repositoryItemTextEdit5.AutoHeight = false;
            this.repositoryItemTextEdit5.Name = "repositoryItemTextEdit5";
            // 
            // frmReservationSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 497);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmReservationSearch";
            this.Text = "Registration Search";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rcRegistrationSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRibbonHolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciRibbonHolder;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit3;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit4;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit5;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcRegistrationSearch;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpMain;
        private DevExpress.XtraBars.BarButtonItem bbiSearch;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private CNETDataEditor.CNETDataEditor cdeRegistrationSearch;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRegistration;
        private DevExpress.XtraGrid.Columns.GridColumn gcolName;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRoom;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRoomType;
        private DevExpress.XtraGrid.Columns.GridColumn gcolArrival;
        private DevExpress.XtraGrid.Columns.GridColumn gcolDeparture;
        private DevExpress.XtraGrid.Columns.GridColumn gcolRooms;
        private DevExpress.XtraGrid.Columns.GridColumn gcolTax;
        private DevExpress.XtraGrid.Columns.GridColumn gcolStatus;
        private DevExpress.XtraGrid.Columns.GridColumn gcolGroup;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;

    }
}