namespace CNET.FrontOffice_V._7.Forms.Setting_and_Miscellaneous.Revenue_Management_Modals
{
    partial class frmNegotiatedRate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNegotiatedRate));
            panelControl1 = new DevExpress.XtraEditors.PanelControl();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            cacConsignee = new DevExpress.XtraEditors.SearchLookUpEdit();
            rcProfileAmendment = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiNew = new DevExpress.XtraBars.BarButtonItem();
            bbiOk = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            bbiReset = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            cacType = new DevExpress.XtraEditors.LookUpEdit();
            statusStrip1 = new StatusStrip();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cacConsignee.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rcProfileAmendment).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cacType.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(layoutControl1);
            panelControl1.Controls.Add(rcProfileAmendment);
            panelControl1.Dock = DockStyle.Fill;
            panelControl1.Location = new Point(0, 0);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(423, 220);
            panelControl1.TabIndex = 0;
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(cacConsignee);
            layoutControl1.Controls.Add(cacType);
            layoutControl1.Controls.Add(statusStrip1);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(2, 68);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(419, 150);
            layoutControl1.TabIndex = 2;
            layoutControl1.Text = "layoutControl1";
            // 
            // cacConsignee
            // 
            cacConsignee.Location = new Point(79, 70);
            cacConsignee.MenuManager = rcProfileAmendment;
            cacConsignee.Name = "cacConsignee";
            cacConsignee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cacConsignee.Properties.NullText = "";
            cacConsignee.Size = new Size(323, 20);
            cacConsignee.StyleController = layoutControl1;
            cacConsignee.TabIndex = 2;
            cacConsignee.KeyDown += cacConsignee_KeyDown;
            // 
            // rcProfileAmendment
            // 
            rcProfileAmendment.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcProfileAmendment.ExpandCollapseItem.Id = 0;
            rcProfileAmendment.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcProfileAmendment.ExpandCollapseItem, rcProfileAmendment.SearchEditItem, bbiNew, bbiOk, bbiClose, bbiReset });
            rcProfileAmendment.Location = new Point(2, 2);
            rcProfileAmendment.MaxItemId = 7;
            rcProfileAmendment.Name = "rcProfileAmendment";
            rcProfileAmendment.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcProfileAmendment.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcProfileAmendment.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            rcProfileAmendment.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
            rcProfileAmendment.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcProfileAmendment.Size = new Size(419, 66);
            rcProfileAmendment.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            rcProfileAmendment.TransparentEditorsMode = DevExpress.Utils.DefaultBoolean.False;
            // 
            // bbiNew
            // 
            bbiNew.Caption = "New";
            bbiNew.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiNew.Id = 1;
            bbiNew.ImageOptions.Image = (Image)resources.GetObject("bbiNew.ImageOptions.Image");
            bbiNew.Name = "bbiNew";
            bbiNew.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
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
            // bbiReset
            // 
            bbiReset.Caption = "New";
            bbiReset.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiReset.Id = 6;
            bbiReset.ImageOptions.Image = (Image)resources.GetObject("bbiReset.ImageOptions.Image");
            bbiReset.ImageOptions.LargeImage = (Image)resources.GetObject("bbiReset.ImageOptions.LargeImage");
            bbiReset.Name = "bbiReset";
            bbiReset.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiReset.ItemClick += bbiReset_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup2 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(bbiReset);
            ribbonPageGroup2.ItemLinks.Add(bbiOk);
            ribbonPageGroup2.ItemLinks.Add(bbiClose);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // cacType
            // 
            cacType.Location = new Point(79, 32);
            cacType.MenuManager = rcProfileAmendment;
            cacType.Name = "cacType";
            cacType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cacType.Properties.NullText = "";
            cacType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            cacType.Size = new Size(323, 20);
            cacType.StyleController = layoutControl1;
            cacType.TabIndex = 0;
            cacType.EditValueChanged += cacType_EditValueChanged;
            cacType.KeyDown += cacType_KeyDown;
            // 
            // statusStrip1
            // 
            statusStrip1.AutoSize = false;
            statusStrip1.Dock = DockStyle.None;
            statusStrip1.Location = new Point(12, 118);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(395, 20);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem3, layoutControlItem4, layoutControlItem5, emptySpaceItem3, emptySpaceItem1, emptySpaceItem2 });
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(419, 150);
            layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = statusStrip1;
            layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            layoutControlItem3.Location = new Point(0, 106);
            layoutControlItem3.MaxSize = new Size(0, 24);
            layoutControlItem3.MinSize = new Size(104, 24);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(399, 24);
            layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            layoutControlItem3.TextSize = new Size(0, 0);
            layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem4.Control = cacType;
            layoutControlItem4.CustomizationFormText = "Type";
            layoutControlItem4.Location = new Point(0, 18);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new Size(399, 28);
            layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 5, 2, 2);
            layoutControlItem4.Text = "Type";
            layoutControlItem4.TextSize = new Size(50, 13);
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem5.Control = cacConsignee;
            layoutControlItem5.CustomizationFormText = "Consignee";
            layoutControlItem5.Location = new Point(0, 56);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Size = new Size(399, 28);
            layoutControlItem5.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 5, 2, 2);
            layoutControlItem5.Text = "Consignee";
            layoutControlItem5.TextSize = new Size(50, 13);
            // 
            // emptySpaceItem3
            // 
            emptySpaceItem3.AllowHotTrack = false;
            emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
            emptySpaceItem3.Location = new Point(0, 46);
            emptySpaceItem3.Name = "emptySpaceItem3";
            emptySpaceItem3.Size = new Size(399, 10);
            emptySpaceItem3.TextSize = new Size(0, 0);
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new Point(0, 0);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(399, 18);
            emptySpaceItem1.TextSize = new Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.AllowHotTrack = false;
            emptySpaceItem2.Location = new Point(0, 84);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(399, 22);
            emptySpaceItem2.TextSize = new Size(0, 0);
            // 
            // frmNegotiatedRate
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(423, 220);
            Controls.Add(panelControl1);
            IconOptions.Icon = (Icon)resources.GetObject("frmNegotiatedRate.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmNegotiatedRate";
            Text = "Negotiated Rate";
            Load += frmNegotiatedRate_Load;
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)cacConsignee.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)rcProfileAmendment).EndInit();
            ((System.ComponentModel.ISupportInitialize)cacType.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcProfileAmendment;
        private DevExpress.XtraBars.BarButtonItem bbiNew;
        private DevExpress.XtraBars.BarButtonItem bbiOk;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraBars.BarButtonItem bbiReset;
        private StatusStrip statusStrip1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SearchLookUpEdit cacConsignee;
        private DevExpress.XtraEditors.LookUpEdit cacType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}