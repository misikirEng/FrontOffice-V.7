namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    partial class frmProfileAmendment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProfileAmendment));
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            leGuest = new DevExpress.XtraEditors.SearchLookUpEdit();
            searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            leContact = new DevExpress.XtraEditors.SearchLookUpEdit();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            leCompany = new DevExpress.XtraEditors.SearchLookUpEdit();
            gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            leAgent = new DevExpress.XtraEditors.SearchLookUpEdit();
            gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
            leGroup = new DevExpress.XtraEditors.SearchLookUpEdit();
            gridView4 = new DevExpress.XtraGrid.Views.Grid.GridView();
            leSource = new DevExpress.XtraEditors.SearchLookUpEdit();
            gridView5 = new DevExpress.XtraGrid.Views.Grid.GridView();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            rcProfileAmendment = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiOk = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            statusStrip1 = new StatusStrip();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)leGuest.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)searchLookUpEdit1View).BeginInit();
            ((System.ComponentModel.ISupportInitialize)leContact.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)leCompany.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)leAgent.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)leGroup.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)leSource.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rcProfileAmendment).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(layoutControl1);
            panelControl1.Controls.Add(rcProfileAmendment);
            panelControl1.Dock = DockStyle.Fill;
            panelControl1.Location = new Point(0, 0);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(461, 330);
            panelControl1.TabIndex = 0;
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(leGuest);
            layoutControl1.Controls.Add(leContact);
            layoutControl1.Controls.Add(leCompany);
            layoutControl1.Controls.Add(leAgent);
            layoutControl1.Controls.Add(leGroup);
            layoutControl1.Controls.Add(leSource);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(2, 68);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(457, 260);
            layoutControl1.TabIndex = 1;
            layoutControl1.Text = "layoutControl1";
            // 
            // leGuest
            // 
            leGuest.EditValue = "";
            leGuest.Location = new Point(77, 15);
            leGuest.Name = "leGuest";
            leGuest.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            leGuest.Properties.NullText = "";
            leGuest.Properties.PopupView = searchLookUpEdit1View;
            leGuest.Properties.ShowAddNewButton = true;
            leGuest.Size = new Size(360, 20);
            leGuest.StyleController = layoutControl1;
            leGuest.TabIndex = 7;
            leGuest.AddNewValue += leGuest_AddNewValue;
            // 
            // searchLookUpEdit1View
            // 
            searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            searchLookUpEdit1View.OptionsNavigation.UseTabKey = false;
            searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // leContact
            // 
            leContact.EditValue = "";
            leContact.Location = new Point(77, 45);
            leContact.Name = "leContact";
            leContact.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            leContact.Properties.NullText = "";
            leContact.Properties.PopupView = gridView1;
            leContact.Properties.ShowAddNewButton = true;
            leContact.Size = new Size(360, 20);
            leContact.StyleController = layoutControl1;
            leContact.TabIndex = 8;
            leContact.AddNewValue += leContact_AddNewValue;
            // 
            // gridView1
            // 
            gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView1.Name = "gridView1";
            gridView1.OptionsNavigation.UseTabKey = false;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // leCompany
            // 
            leCompany.EditValue = "";
            leCompany.Location = new Point(77, 75);
            leCompany.Name = "leCompany";
            leCompany.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            leCompany.Properties.NullText = "";
            leCompany.Properties.PopupView = gridView2;
            leCompany.Properties.ShowAddNewButton = true;
            leCompany.Size = new Size(360, 20);
            leCompany.StyleController = layoutControl1;
            leCompany.TabIndex = 10;
            leCompany.AddNewValue += leCompany_AddNewValue;
            // 
            // gridView2
            // 
            gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView2.Name = "gridView2";
            gridView2.OptionsNavigation.UseTabKey = false;
            gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // leAgent
            // 
            leAgent.EditValue = "";
            leAgent.Location = new Point(77, 105);
            leAgent.Name = "leAgent";
            leAgent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            leAgent.Properties.NullText = "";
            leAgent.Properties.PopupView = gridView3;
            leAgent.Properties.ShowAddNewButton = true;
            leAgent.Size = new Size(360, 20);
            leAgent.StyleController = layoutControl1;
            leAgent.TabIndex = 9;
            leAgent.AddNewValue += leAgent_AddNewValue;
            // 
            // gridView3
            // 
            gridView3.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView3.Name = "gridView3";
            gridView3.OptionsNavigation.UseTabKey = false;
            gridView3.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView3.OptionsView.ShowGroupPanel = false;
            // 
            // leGroup
            // 
            leGroup.EditValue = "";
            leGroup.Location = new Point(77, 135);
            leGroup.Name = "leGroup";
            leGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            leGroup.Properties.NullText = "";
            leGroup.Properties.PopupView = gridView4;
            leGroup.Properties.ShowAddNewButton = true;
            leGroup.Size = new Size(360, 20);
            leGroup.StyleController = layoutControl1;
            leGroup.TabIndex = 11;
            leGroup.AddNewValue += leGroup_AddNewValue;
            // 
            // gridView4
            // 
            gridView4.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView4.Name = "gridView4";
            gridView4.OptionsNavigation.UseTabKey = false;
            gridView4.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView4.OptionsView.ShowGroupPanel = false;
            // 
            // leSource
            // 
            leSource.EditValue = "";
            leSource.Location = new Point(77, 165);
            leSource.Name = "leSource";
            leSource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            leSource.Properties.NullText = "";
            leSource.Properties.PopupView = gridView5;
            leSource.Properties.ShowAddNewButton = true;
            leSource.Size = new Size(360, 20);
            leSource.StyleController = layoutControl1;
            leSource.TabIndex = 12;
            leSource.AddNewValue += leSource_AddNewValue;
            // 
            // gridView5
            // 
            gridView5.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView5.Name = "gridView5";
            gridView5.OptionsNavigation.UseTabKey = false;
            gridView5.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView5.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem4, layoutControlItem5, layoutControlItem7, layoutControlItem2, layoutControlItem1, layoutControlItem3 });
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(457, 260);
            layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem4.Control = leGuest;
            layoutControlItem4.CustomizationFormText = "Guest";
            layoutControlItem4.Location = new Point(0, 0);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 5, 5);
            layoutControlItem4.Size = new Size(437, 30);
            layoutControlItem4.Text = "Guest";
            layoutControlItem4.TextSize = new Size(45, 13);
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem5.Control = leContact;
            layoutControlItem5.CustomizationFormText = "Contact";
            layoutControlItem5.Location = new Point(0, 30);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 5, 5);
            layoutControlItem5.Size = new Size(437, 30);
            layoutControlItem5.Text = "Contact";
            layoutControlItem5.TextSize = new Size(45, 13);
            // 
            // layoutControlItem7
            // 
            layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem7.Control = leAgent;
            layoutControlItem7.CustomizationFormText = "Agent";
            layoutControlItem7.ImageOptions.Alignment = ContentAlignment.MiddleCenter;
            layoutControlItem7.Location = new Point(0, 90);
            layoutControlItem7.Name = "layoutControlItem7";
            layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 5, 5);
            layoutControlItem7.Size = new Size(437, 30);
            layoutControlItem7.Text = "Agent";
            layoutControlItem7.TextSize = new Size(45, 13);
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem2.Control = leCompany;
            layoutControlItem2.CustomizationFormText = "Company";
            layoutControlItem2.Location = new Point(0, 60);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 5, 5);
            layoutControlItem2.Size = new Size(437, 30);
            layoutControlItem2.Text = "Company";
            layoutControlItem2.TextSize = new Size(45, 13);
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem1.Control = leGroup;
            layoutControlItem1.CustomizationFormText = "Group";
            layoutControlItem1.Location = new Point(0, 120);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 5, 5);
            layoutControlItem1.Size = new Size(437, 30);
            layoutControlItem1.Text = "Group";
            layoutControlItem1.TextSize = new Size(45, 13);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem3.Control = leSource;
            layoutControlItem3.CustomizationFormText = "Source";
            layoutControlItem3.Location = new Point(0, 150);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 5, 5);
            layoutControlItem3.Size = new Size(437, 90);
            layoutControlItem3.Text = "Source";
            layoutControlItem3.TextSize = new Size(45, 13);
            // 
            // rcProfileAmendment
            // 
            rcProfileAmendment.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcProfileAmendment.ExpandCollapseItem.Id = 0;
            rcProfileAmendment.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcProfileAmendment.ExpandCollapseItem, rcProfileAmendment.SearchEditItem, bbiOk, bbiClose });
            rcProfileAmendment.Location = new Point(2, 2);
            rcProfileAmendment.MaxItemId = 6;
            rcProfileAmendment.Name = "rcProfileAmendment";
            rcProfileAmendment.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcProfileAmendment.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcProfileAmendment.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            rcProfileAmendment.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcProfileAmendment.Size = new Size(457, 66);
            rcProfileAmendment.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiOk
            // 
            bbiOk.Caption = "Save";
            bbiOk.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiOk.Id = 2;
            bbiOk.ImageOptions.Image = (Image)resources.GetObject("bbiOk.ImageOptions.Image");
            bbiOk.ImageOptions.LargeImage = (Image)resources.GetObject("bbiOk.ImageOptions.LargeImage");
            bbiOk.Name = "bbiOk";
            bbiOk.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiOk.ItemClick += bbiOk_ItemClick;
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
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup2 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup2.ItemLinks.Add(bbiOk);
            ribbonPageGroup2.ItemLinks.Add(bbiClose);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem6.CustomizationFormText = "Arrival Date";
            layoutControlItem6.Location = new Point(0, 148);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new Size(370, 28);
            layoutControlItem6.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 5, 2, 2);
            layoutControlItem6.Text = "Arrival Date";
            layoutControlItem6.TextSize = new Size(98, 13);
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(0, 308);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(461, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // frmProfileAmendment
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(461, 330);
            Controls.Add(statusStrip1);
            Controls.Add(panelControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmProfileAmendment.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmProfileAmendment";
            Text = "Profile Amendment";
            Load += frmProfileAmendment_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)leGuest.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)searchLookUpEdit1View).EndInit();
            ((System.ComponentModel.ISupportInitialize)leContact.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)leCompany.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)leAgent.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView3).EndInit();
            ((System.ComponentModel.ISupportInitialize)leGroup.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView4).EndInit();
            ((System.ComponentModel.ISupportInitialize)leSource.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)rcProfileAmendment).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcProfileAmendment;
        private DevExpress.XtraBars.BarButtonItem bbiOk;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private StatusStrip statusStrip1;
        private DevExpress.XtraEditors.SearchLookUpEdit leGuest;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private DevExpress.XtraEditors.SearchLookUpEdit leContact;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SearchLookUpEdit leCompany;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraEditors.SearchLookUpEdit leAgent;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
        private DevExpress.XtraEditors.SearchLookUpEdit leGroup;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView4;
        private DevExpress.XtraEditors.SearchLookUpEdit leSource;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView5;

    }
}