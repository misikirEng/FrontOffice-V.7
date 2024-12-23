using DevExpress.XtraEditors;

namespace CNET.ERP.Client.UI_Logic.PMS.Forms.Non_Navigatable_Modals
{
    partial class frmRegistrationDateAmendement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRegistrationDateAmendement));
            logic1 = new SearchLookUpEdit();
            cnetAdvancedComboProperty1 = new SearchLookUpEdit();
            panelControl1 = new PanelControl();
            statusStrip1 = new StatusStrip();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            deDepartureDate = new DateEdit();
            rcProfileAmendment = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiNew = new DevExpress.XtraBars.BarButtonItem();
            bbiOk = new DevExpress.XtraBars.BarButtonItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            bbiDetail = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            deArrivalDate = new DateEdit();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)logic1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cnetAdvancedComboProperty1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).BeginInit();
            panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)deDepartureDate.Properties.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)deDepartureDate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rcProfileAmendment).BeginInit();
            ((System.ComponentModel.ISupportInitialize)deArrivalDate.Properties.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)deArrivalDate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            SuspendLayout();
            // 
            // logic1
            // 
            logic1.Location = new Point(0, 0);
            logic1.Name = "logic1";
            logic1.Size = new Size(100, 20);
            logic1.TabIndex = 0;
            // 
            // cnetAdvancedComboProperty1
            // 
            cnetAdvancedComboProperty1.Location = new Point(0, 0);
            cnetAdvancedComboProperty1.Name = "cnetAdvancedComboProperty1";
            cnetAdvancedComboProperty1.Size = new Size(100, 20);
            cnetAdvancedComboProperty1.TabIndex = 0;
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(statusStrip1);
            panelControl1.Controls.Add(layoutControl1);
            panelControl1.Controls.Add(rcProfileAmendment);
            panelControl1.Dock = DockStyle.Fill;
            panelControl1.Location = new Point(0, 0);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(409, 264);
            panelControl1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(2, 240);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(405, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(deDepartureDate);
            layoutControl1.Controls.Add(deArrivalDate);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(2, 68);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new Size(405, 194);
            layoutControl1.TabIndex = 1;
            layoutControl1.Text = "layoutControl1";
            // 
            // deDepartureDate
            // 
            deDepartureDate.EditValue = null;
            deDepartureDate.Location = new Point(104, 51);
            deDepartureDate.MenuManager = rcProfileAmendment;
            deDepartureDate.Name = "deDepartureDate";
            deDepartureDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            deDepartureDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            deDepartureDate.Size = new Size(284, 20);
            deDepartureDate.StyleController = layoutControl1;
            deDepartureDate.TabIndex = 6;
            // 
            // rcProfileAmendment
            // 
            rcProfileAmendment.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.False;
            rcProfileAmendment.ExpandCollapseItem.Id = 0;
            rcProfileAmendment.Items.AddRange(new DevExpress.XtraBars.BarItem[] { rcProfileAmendment.ExpandCollapseItem, rcProfileAmendment.SearchEditItem, bbiNew, bbiOk, bbiClose, bbiDetail });
            rcProfileAmendment.Location = new Point(2, 2);
            rcProfileAmendment.MaxItemId = 7;
            rcProfileAmendment.Name = "rcProfileAmendment";
            rcProfileAmendment.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            rcProfileAmendment.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            rcProfileAmendment.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            rcProfileAmendment.Size = new Size(405, 66);
            rcProfileAmendment.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
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
            bbiOk.Caption = "OK";
            bbiOk.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiOk.Id = 2;
            bbiOk.ImageOptions.Image = (Image)resources.GetObject("bbiOk.ImageOptions.Image");
            bbiOk.Name = "bbiOk";
            bbiOk.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bbiClose
            // 
            bbiClose.Caption = "Cancel";
            bbiClose.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiClose.Id = 3;
            bbiClose.ImageOptions.Image = (Image)resources.GetObject("bbiClose.ImageOptions.Image");
            bbiClose.Name = "bbiClose";
            bbiClose.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // bbiDetail
            // 
            bbiDetail.Caption = "Detail";
            bbiDetail.CategoryGuid = new Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            bbiDetail.Id = 6;
            bbiDetail.ImageOptions.Image = (Image)resources.GetObject("bbiDetail.ImageOptions.Image");
            bbiDetail.Name = "bbiDetail";
            bbiDetail.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup2 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.ItemLinks.Add(bbiOk);
            ribbonPageGroup2.ItemLinks.Add(bbiClose);
            ribbonPageGroup2.ItemLinks.Add(bbiDetail);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            // 
            // deArrivalDate
            // 
            deArrivalDate.EditValue = null;
            deArrivalDate.Location = new Point(104, 17);
            deArrivalDate.MenuManager = rcProfileAmendment;
            deArrivalDate.Name = "deArrivalDate";
            deArrivalDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            deArrivalDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            deArrivalDate.Size = new Size(284, 20);
            deArrivalDate.StyleController = layoutControl1;
            deArrivalDate.TabIndex = 5;
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { emptySpaceItem3, layoutControlItem2, layoutControlItem3, layoutControlItem1 });
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new Size(405, 194);
            layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            emptySpaceItem3.AllowHotTrack = false;
            emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
            emptySpaceItem3.Location = new Point(0, 108);
            emptySpaceItem3.Name = "emptySpaceItem3";
            emptySpaceItem3.Size = new Size(385, 66);
            emptySpaceItem3.TextSize = new Size(0, 0);
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem2.Control = deArrivalDate;
            layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            layoutControlItem2.Location = new Point(0, 0);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(385, 34);
            layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            layoutControlItem2.Text = "Arrival Date";
            layoutControlItem2.TextSize = new Size(75, 13);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem3.Control = deDepartureDate;
            layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            layoutControlItem3.Location = new Point(0, 34);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(385, 34);
            layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            layoutControlItem3.Text = "Departure Date";
            layoutControlItem3.TextSize = new Size(75, 13);
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            layoutControlItem1.ImageOptions.Alignment = ContentAlignment.MiddleCenter;
            layoutControlItem1.Location = new Point(0, 68);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(385, 40);
            layoutControlItem1.Text = "Rate Code";
            layoutControlItem1.TextSize = new Size(75, 13);
            // 
            // frmRegistrationDateAmendement
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(409, 264);
            Controls.Add(panelControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("frmRegistrationDateAmendement.IconOptions.Icon");
            IconOptions.ShowIcon = false;
            Name = "frmRegistrationDateAmendement";
            Text = "Registration Amendement";
            ((System.ComponentModel.ISupportInitialize)logic1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cnetAdvancedComboProperty1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelControl1).EndInit();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)deDepartureDate.Properties.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)deDepartureDate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)rcProfileAmendment).EndInit();
            ((System.ComponentModel.ISupportInitialize)deArrivalDate.Properties.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)deArrivalDate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PanelControl panelControl1;
        private DevExpress.XtraBars.Ribbon.RibbonControl rcProfileAmendment;
        private DevExpress.XtraBars.BarButtonItem bbiNew;
        private DevExpress.XtraBars.BarButtonItem bbiOk;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DateEdit deDepartureDate;
        private DateEdit deArrivalDate;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private SearchLookUpEdit cacRateCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem bbiDetail;
        private StatusStrip statusStrip1;
        private SearchLookUpEdit logic1;
        private SearchLookUpEdit cnetAdvancedComboProperty1;
    }
}