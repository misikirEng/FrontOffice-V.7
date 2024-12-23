using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace ERP.EventManagement
{
    partial class UIMaintainCustomer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(UIMaintainCustomer));
            layoutControl1 = new LayoutControl();
            groupBox2 = new GroupBox();
            layoutControl3 = new LayoutControl();
            txtEmail = new TextEdit();
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            btnOk = new DevExpress.XtraBars.BarButtonItem();
            btnClose = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            txtAddress = new TextEdit();
            txtPhone2 = new TextEdit();
            txtTelNo = new TextEdit();
            layoutControlGroup2 = new LayoutControlGroup();
            layoutControlItem7 = new LayoutControlItem();
            emptySpaceItem1 = new EmptySpaceItem();
            layoutControlItem10 = new LayoutControlItem();
            layoutControlItem11 = new LayoutControlItem();
            layoutControlItem12 = new LayoutControlItem();
            groupBox1 = new GroupBox();
            layoutControl2 = new LayoutControl();
            txtCode = new TextBox();
            lkCategory = new LookUpEdit();
            txtFirstName = new TextEdit();
            txtTIN = new TextEdit();
            Root = new LayoutControlGroup();
            layoutControlItem4 = new LayoutControlItem();
            layoutControlItem1 = new LayoutControlItem();
            layoutControlItem2 = new LayoutControlItem();
            layoutControlItem3 = new LayoutControlItem();
            statusStrip1 = new StatusStrip();
            layoutControlGroup1 = new LayoutControlGroup();
            layoutControlItem5 = new LayoutControlItem();
            layoutControlItem6 = new LayoutControlItem();
            layoutControlItem8 = new LayoutControlItem();
            layoutControlItem9 = new LayoutControlItem();
            errorProvider1 = new ErrorProvider(components);
            rpgHome = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            layoutControlGroup4 = new LayoutControlGroup();
            ((ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((ISupportInitialize)layoutControl3).BeginInit();
            layoutControl3.SuspendLayout();
            ((ISupportInitialize)txtEmail.Properties).BeginInit();
            ((ISupportInitialize)ribbon).BeginInit();
            ((ISupportInitialize)txtAddress.Properties).BeginInit();
            ((ISupportInitialize)txtPhone2.Properties).BeginInit();
            ((ISupportInitialize)txtTelNo.Properties).BeginInit();
            ((ISupportInitialize)layoutControlGroup2).BeginInit();
            ((ISupportInitialize)layoutControlItem7).BeginInit();
            ((ISupportInitialize)emptySpaceItem1).BeginInit();
            ((ISupportInitialize)layoutControlItem10).BeginInit();
            ((ISupportInitialize)layoutControlItem11).BeginInit();
            ((ISupportInitialize)layoutControlItem12).BeginInit();
            groupBox1.SuspendLayout();
            ((ISupportInitialize)layoutControl2).BeginInit();
            layoutControl2.SuspendLayout();
            ((ISupportInitialize)lkCategory.Properties).BeginInit();
            ((ISupportInitialize)txtFirstName.Properties).BeginInit();
            ((ISupportInitialize)txtTIN.Properties).BeginInit();
            ((ISupportInitialize)Root).BeginInit();
            ((ISupportInitialize)layoutControlItem4).BeginInit();
            ((ISupportInitialize)layoutControlItem1).BeginInit();
            ((ISupportInitialize)layoutControlItem2).BeginInit();
            ((ISupportInitialize)layoutControlItem3).BeginInit();
            ((ISupportInitialize)layoutControlGroup1).BeginInit();
            ((ISupportInitialize)layoutControlItem5).BeginInit();
            ((ISupportInitialize)layoutControlItem6).BeginInit();
            ((ISupportInitialize)layoutControlItem8).BeginInit();
            ((ISupportInitialize)layoutControlItem9).BeginInit();
            ((ISupportInitialize)errorProvider1).BeginInit();
            ((ISupportInitialize)layoutControlGroup4).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(groupBox2);
            layoutControl1.Controls.Add(groupBox1);
            layoutControl1.Controls.Add(statusStrip1);
            layoutControl1.Controls.Add(ribbon);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(779, 376);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(layoutControl3);
            groupBox2.Location = new Point(405, 103);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(370, 245);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Address";
            // 
            // layoutControl3
            // 
            layoutControl3.Controls.Add(txtEmail);
            layoutControl3.Controls.Add(txtAddress);
            layoutControl3.Controls.Add(txtPhone2);
            layoutControl3.Controls.Add(txtTelNo);
            layoutControl3.Dock = DockStyle.Fill;
            layoutControl3.Location = new Point(3, 17);
            layoutControl3.Name = "layoutControl3";
            layoutControl3.Root = layoutControlGroup2;
            layoutControl3.Size = new Size(364, 225);
            layoutControl3.TabIndex = 1;
            layoutControl3.Text = "layoutControl3";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(60, 93);
            txtEmail.MenuManager = ribbon;
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(295, 20);
            txtEmail.StyleController = layoutControl3;
            txtEmail.TabIndex = 6;
            // 
            // ribbon
            // 
            ribbon.Dock = DockStyle.None;
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, btnOk, btnClose });
            ribbon.Location = new Point(4, 4);
            ribbon.MaxItemId = 8;
            ribbon.Name = "ribbon";
            ribbon.OptionsPageCategories.ShowCaptions = false;
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            ribbon.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            ribbon.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
            ribbon.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            ribbon.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            ribbon.ShowToolbarCustomizeItem = false;
            ribbon.Size = new Size(771, 83);
            ribbon.Toolbar.ShowCustomizeItem = false;
            ribbon.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // btnOk
            // 
            btnOk.Caption = "Save\r\n";
            btnOk.Id = 1;
            btnOk.ImageOptions.Image = (Image)resources.GetObject("btnOk.ImageOptions.Image");
            btnOk.ImageOptions.LargeImage = (Image)resources.GetObject("btnOk.ImageOptions.LargeImage");
            btnOk.ImageOptions.LargeImageIndex = 8;
            btnOk.Name = "btnOk";
            btnOk.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnOk.ItemClick += btnOk_ItemClick;
            // 
            // btnClose
            // 
            btnClose.Caption = "Close\r\n[ESC]";
            btnClose.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            btnClose.Id = 6;
            btnClose.ImageOptions.Image = (Image)resources.GetObject("btnClose.ImageOptions.Image");
            btnClose.ImageOptions.LargeImage = (Image)resources.GetObject("btnClose.ImageOptions.LargeImage");
            btnClose.Name = "btnClose";
            btnClose.ItemClick += btnClose_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, ribbonPageGroup2 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.AllowTextClipping = false;
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(btnOk);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(btnClose);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(60, 65);
            txtAddress.MenuManager = ribbon;
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(295, 20);
            txtAddress.StyleController = layoutControl3;
            txtAddress.TabIndex = 5;
            // 
            // txtPhone2
            // 
            txtPhone2.Location = new Point(60, 37);
            txtPhone2.MenuManager = ribbon;
            txtPhone2.Name = "txtPhone2";
            txtPhone2.Size = new Size(295, 20);
            txtPhone2.StyleController = layoutControl3;
            txtPhone2.TabIndex = 4;
            // 
            // txtTelNo
            // 
            txtTelNo.Location = new Point(60, 9);
            txtTelNo.MenuManager = ribbon;
            txtTelNo.Name = "txtTelNo";
            txtTelNo.Size = new Size(295, 20);
            txtTelNo.StyleController = layoutControl3;
            txtTelNo.TabIndex = 0;
            // 
            // layoutControlGroup2
            // 
            layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup2.GroupBordersVisible = false;
            layoutControlGroup2.Items.AddRange(new BaseLayoutItem[] { layoutControlItem7, emptySpaceItem1, layoutControlItem10, layoutControlItem11, layoutControlItem12 });
            layoutControlGroup2.Name = "layoutControlGroup2";
            layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            layoutControlGroup2.Size = new Size(364, 225);
            layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            layoutControlItem7.Control = txtTelNo;
            layoutControlItem7.Location = new Point(0, 0);
            layoutControlItem7.Name = "layoutControlItem7";
            layoutControlItem7.Size = new Size(354, 28);
            layoutControlItem7.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem7.Text = "Phone 1";
            layoutControlItem7.TextSize = new Size(39, 13);
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new Point(0, 112);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(354, 103);
            emptySpaceItem1.TextSize = new Size(0, 0);
            // 
            // layoutControlItem10
            // 
            layoutControlItem10.Control = txtPhone2;
            layoutControlItem10.Location = new Point(0, 28);
            layoutControlItem10.Name = "layoutControlItem10";
            layoutControlItem10.Size = new Size(354, 28);
            layoutControlItem10.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem10.Text = "Phone 2";
            layoutControlItem10.TextSize = new Size(39, 13);
            // 
            // layoutControlItem11
            // 
            layoutControlItem11.Control = txtAddress;
            layoutControlItem11.Location = new Point(0, 56);
            layoutControlItem11.Name = "layoutControlItem11";
            layoutControlItem11.Size = new Size(354, 28);
            layoutControlItem11.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem11.Text = "Address";
            layoutControlItem11.TextSize = new Size(39, 13);
            // 
            // layoutControlItem12
            // 
            layoutControlItem12.Control = txtEmail;
            layoutControlItem12.Location = new Point(0, 84);
            layoutControlItem12.Name = "layoutControlItem12";
            layoutControlItem12.Size = new Size(354, 28);
            layoutControlItem12.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem12.Text = "Email";
            layoutControlItem12.TextSize = new Size(39, 13);
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(layoutControl2);
            groupBox1.Location = new Point(4, 103);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(397, 245);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Basic Info";
            // 
            // layoutControl2
            // 
            layoutControl2.Controls.Add(txtCode);
            layoutControl2.Controls.Add(lkCategory);
            layoutControl2.Controls.Add(txtFirstName);
            layoutControl2.Controls.Add(txtTIN);
            layoutControl2.Dock = DockStyle.Fill;
            layoutControl2.Location = new Point(3, 17);
            layoutControl2.Name = "layoutControl2";
            layoutControl2.Root = Root;
            layoutControl2.Size = new Size(391, 225);
            layoutControl2.TabIndex = 0;
            layoutControl2.Text = "layoutControl2";
            // 
            // txtCode
            // 
            txtCode.Enabled = false;
            txtCode.Location = new Point(75, 6);
            txtCode.Name = "txtCode";
            txtCode.ReadOnly = true;
            txtCode.Size = new Size(310, 20);
            txtCode.TabIndex = 8;
            // 
            // lkCategory
            // 
            lkCategory.Location = new Point(75, 62);
            lkCategory.MenuManager = ribbon;
            lkCategory.Name = "lkCategory";
            lkCategory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            lkCategory.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "Id", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "Description") });
            lkCategory.Properties.NullText = "";
            lkCategory.Size = new Size(310, 20);
            lkCategory.StyleController = layoutControl2;
            lkCategory.TabIndex = 4;
            // 
            // txtFirstName
            // 
            txtFirstName.Location = new Point(75, 34);
            txtFirstName.MenuManager = ribbon;
            txtFirstName.Name = "txtFirstName";
            txtFirstName.Size = new Size(310, 20);
            txtFirstName.StyleController = layoutControl2;
            txtFirstName.TabIndex = 0;
            // 
            // txtTIN
            // 
            txtTIN.Location = new Point(75, 90);
            txtTIN.MenuManager = ribbon;
            txtTIN.Name = "txtTIN";
            txtTIN.Properties.Mask.EditMask = "[0-9]{10}|[0-9]{10}(-[0-9]{1})|[0-9]{10}(-[0-9]{2})";
            txtTIN.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            txtTIN.Size = new Size(310, 20);
            txtTIN.StyleController = layoutControl2;
            txtTIN.TabIndex = 5;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new BaseLayoutItem[] { layoutControlItem4, layoutControlItem1, layoutControlItem2, layoutControlItem3 });
            Root.Name = "Root";
            Root.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            Root.Size = new Size(391, 225);
            Root.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.Control = txtCode;
            layoutControlItem4.Location = new Point(0, 0);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new Size(387, 28);
            layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem4.Text = "Code";
            layoutControlItem4.TextSize = new Size(57, 13);
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = txtFirstName;
            layoutControlItem1.Location = new Point(0, 28);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(387, 28);
            layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem1.Text = "Name";
            layoutControlItem1.TextSize = new Size(57, 13);
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = lkCategory;
            layoutControlItem2.Location = new Point(0, 56);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(387, 28);
            layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem2.Text = "Category";
            layoutControlItem2.TextSize = new Size(57, 13);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = txtTIN;
            layoutControlItem3.Location = new Point(0, 84);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(387, 137);
            layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlItem3.Text = "TIN Number";
            layoutControlItem3.TextSize = new Size(57, 13);
            // 
            // statusStrip1
            // 
            statusStrip1.AutoSize = false;
            statusStrip1.Dock = DockStyle.None;
            statusStrip1.Location = new Point(4, 352);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(771, 20);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new BaseLayoutItem[] { layoutControlItem5, layoutControlItem6, layoutControlItem8, layoutControlItem9 });
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            layoutControlGroup1.Size = new Size(779, 376);
            layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.Control = ribbon;
            layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            layoutControlItem5.Location = new Point(0, 0);
            layoutControlItem5.MaxSize = new Size(0, 99);
            layoutControlItem5.MinSize = new Size(200, 99);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Size = new Size(775, 99);
            layoutControlItem5.SizeConstraintsType = SizeConstraintsType.Custom;
            layoutControlItem5.TextSize = new Size(0, 0);
            layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.Control = statusStrip1;
            layoutControlItem6.CustomizationFormText = "layoutControlItem6";
            layoutControlItem6.Location = new Point(0, 348);
            layoutControlItem6.MaxSize = new Size(0, 24);
            layoutControlItem6.MinSize = new Size(200, 24);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new Size(775, 24);
            layoutControlItem6.SizeConstraintsType = SizeConstraintsType.Custom;
            layoutControlItem6.TextSize = new Size(0, 0);
            layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            layoutControlItem8.Control = groupBox1;
            layoutControlItem8.Location = new Point(0, 99);
            layoutControlItem8.Name = "layoutControlItem8";
            layoutControlItem8.Size = new Size(401, 249);
            layoutControlItem8.TextSize = new Size(0, 0);
            layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            layoutControlItem9.Control = groupBox2;
            layoutControlItem9.Location = new Point(401, 99);
            layoutControlItem9.Name = "layoutControlItem9";
            layoutControlItem9.Size = new Size(374, 249);
            layoutControlItem9.TextSize = new Size(0, 0);
            layoutControlItem9.TextVisible = false;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // rpgHome
            // 
            rpgHome.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            rpgHome.Name = "rpgHome";
            rpgHome.Text = "                                            ";
            // 
            // layoutControlGroup4
            // 
            layoutControlGroup4.CustomizationFormText = "layoutControlGroup2";
            layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup4.GroupBordersVisible = false;
            layoutControlGroup4.Location = new Point(0, 0);
            layoutControlGroup4.Name = "layoutControlGroup2";
            layoutControlGroup4.Size = new Size(470, 190);
            layoutControlGroup4.TextVisible = false;
            // 
            // UIMaintainCustomer
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(779, 376);
            ControlBox = false;
            Controls.Add(layoutControl1);
            IconOptions.Icon = (Icon)resources.GetObject("UIMaintainCustomer.IconOptions.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "UIMaintainCustomer";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Maintain Customer";
            ((ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            layoutControl1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ((ISupportInitialize)layoutControl3).EndInit();
            layoutControl3.ResumeLayout(false);
            ((ISupportInitialize)txtEmail.Properties).EndInit();
            ((ISupportInitialize)ribbon).EndInit();
            ((ISupportInitialize)txtAddress.Properties).EndInit();
            ((ISupportInitialize)txtPhone2.Properties).EndInit();
            ((ISupportInitialize)txtTelNo.Properties).EndInit();
            ((ISupportInitialize)layoutControlGroup2).EndInit();
            ((ISupportInitialize)layoutControlItem7).EndInit();
            ((ISupportInitialize)emptySpaceItem1).EndInit();
            ((ISupportInitialize)layoutControlItem10).EndInit();
            ((ISupportInitialize)layoutControlItem11).EndInit();
            ((ISupportInitialize)layoutControlItem12).EndInit();
            groupBox1.ResumeLayout(false);
            ((ISupportInitialize)layoutControl2).EndInit();
            layoutControl2.ResumeLayout(false);
            ((ISupportInitialize)lkCategory.Properties).EndInit();
            ((ISupportInitialize)txtFirstName.Properties).EndInit();
            ((ISupportInitialize)txtTIN.Properties).EndInit();
            ((ISupportInitialize)Root).EndInit();
            ((ISupportInitialize)layoutControlItem4).EndInit();
            ((ISupportInitialize)layoutControlItem1).EndInit();
            ((ISupportInitialize)layoutControlItem2).EndInit();
            ((ISupportInitialize)layoutControlItem3).EndInit();
            ((ISupportInitialize)layoutControlGroup1).EndInit();
            ((ISupportInitialize)layoutControlItem5).EndInit();
            ((ISupportInitialize)layoutControlItem6).EndInit();
            ((ISupportInitialize)layoutControlItem8).EndInit();
            ((ISupportInitialize)layoutControlItem9).EndInit();
            ((ISupportInitialize)errorProvider1).EndInit();
            ((ISupportInitialize)layoutControlGroup4).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private LayoutControl layoutControl1;
        private LayoutControlGroup layoutControlGroup1;
        private ErrorProvider errorProvider1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgHome;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private LayoutControlItem layoutControlItem5;
        public DevExpress.XtraBars.BarButtonItem btnOk;
        public DevExpress.XtraBars.BarButtonItem btnClose;
        private LayoutControlGroup layoutControlGroup4;
        private StatusStrip statusStrip1;
        private TextEdit txtTelNo;
        private TextEdit txtTIN;
        private TextEdit txtFirstName;
        private LayoutControlItem layoutControlItem6;
        private LookUpEdit lkCategory;
        private TextBox txtCode;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private GroupBox groupBox1;
        private LayoutControlItem layoutControlItem8;
        private GroupBox groupBox2;
        private LayoutControl layoutControl3;
        private TextEdit txtEmail;
        private TextEdit txtAddress;
        private TextEdit txtPhone2;
        private LayoutControlGroup layoutControlGroup2;
        private LayoutControlItem layoutControlItem7;
        private EmptySpaceItem emptySpaceItem1;
        private LayoutControlItem layoutControlItem10;
        private LayoutControlItem layoutControlItem11;
        private LayoutControlItem layoutControlItem12;
        private LayoutControl layoutControl2;
        private LayoutControlGroup Root;
        private LayoutControlItem layoutControlItem4;
        private LayoutControlItem layoutControlItem1;
        private LayoutControlItem layoutControlItem2;
        private LayoutControlItem layoutControlItem3;
        private LayoutControlItem layoutControlItem9;
    }
}