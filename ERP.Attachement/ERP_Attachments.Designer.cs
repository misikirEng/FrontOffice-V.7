namespace ERP.Attachement
{
    partial class ERP_Attachments
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ERP_Attachments));
            bbiBrowse = new DevExpress.XtraBars.BarButtonItem();
            ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            bbiNew = new DevExpress.XtraBars.BarButtonItem();
            btnDeleteAtt = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            treeList1 = new DevExpress.XtraTreeList.TreeList();
            treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            icAttachmentCatagoryIcons = new DevExpress.Utils.ImageCollection(components);
            panel1 = new Panel();
            pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            spreadsheetControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
            richEditControl1 = new DevExpress.XtraRichEdit.RichEditControl();
            pdfViewer1 = new DevExpress.XtraPdfViewer.PdfViewer();
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)treeList1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)icAttachmentCatagoryIcons).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)axWindowsMediaPlayer1).BeginInit();
            SuspendLayout();
            // 
            // bbiBrowse
            // 
            bbiBrowse.Caption = "Browse";
            bbiBrowse.Id = 4;
            bbiBrowse.ImageOptions.Image = (Image)resources.GetObject("bbiBrowse.ImageOptions.Image");
            bbiBrowse.Name = "bbiBrowse";
            bbiBrowse.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // ribbonControl1
            // 
            ribbonControl1.BackColor = Color.White;
            ribbonControl1.EmptyAreaImageOptions.ImagePadding = new Padding(35);
            ribbonControl1.ExpandCollapseItem.Id = 0;
            ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl1.ExpandCollapseItem, ribbonControl1.SearchEditItem, bbiNew, btnDeleteAtt });
            ribbonControl1.Location = new Point(0, 0);
            ribbonControl1.Margin = new Padding(4, 3, 4, 3);
            ribbonControl1.MaxItemId = 4;
            ribbonControl1.Name = "ribbonControl1";
            ribbonControl1.OptionsMenuMinWidth = 385;
            ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.MacOffice;
            ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            ribbonControl1.ShowToolbarCustomizeItem = false;
            ribbonControl1.Size = new Size(1162, 83);
            ribbonControl1.Toolbar.ShowCustomizeItem = false;
            ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // bbiNew
            // 
            bbiNew.Caption = "New";
            bbiNew.Id = 1;
            bbiNew.ImageOptions.Image = (Image)resources.GetObject("bbiNew.ImageOptions.Image");
            bbiNew.Name = "bbiNew";
            bbiNew.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            bbiNew.ItemClick += bbiNew_ItemClick;
            // 
            // btnDeleteAtt
            // 
            btnDeleteAtt.Caption = "Delete";
            btnDeleteAtt.Id = 2;
            btnDeleteAtt.ImageOptions.Image = (Image)resources.GetObject("btnDeleteAtt.ImageOptions.Image");
            btnDeleteAtt.Name = "btnDeleteAtt";
            btnDeleteAtt.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnDeleteAtt.ItemClick += btnDeleteAtt_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(bbiNew);
            ribbonPageGroup1.ItemLinks.Add(btnDeleteAtt);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = DockStyle.Fill;
            splitContainerControl1.Location = new Point(0, 83);
            splitContainerControl1.Margin = new Padding(4, 3, 4, 3);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(treeList1);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(panel1);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new Size(1162, 436);
            splitContainerControl1.SplitterPosition = 255;
            splitContainerControl1.TabIndex = 1;
            splitContainerControl1.Text = "splitContainerControl1";
            // 
            // treeList1
            // 
            treeList1.AllowDrop = true;
            treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { treeListColumn1, treeListColumn2 });
            treeList1.Dock = DockStyle.Fill;
            treeList1.Location = new Point(0, 0);
            treeList1.Margin = new Padding(4, 3, 4, 3);
            treeList1.MinWidth = 23;
            treeList1.Name = "treeList1";
            treeList1.OptionsBehavior.AutoNodeHeight = false;
            treeList1.OptionsBehavior.Editable = false;
            treeList1.OptionsBehavior.PopulateServiceColumns = true;
            treeList1.OptionsLayout.AddNewColumns = false;
            treeList1.OptionsSelection.EnableAppearanceFocusedCell = false;
            treeList1.OptionsView.ShowColumns = false;
            treeList1.OptionsView.ShowFilterPanelMode = DevExpress.XtraTreeList.ShowFilterPanelMode.Never;
            treeList1.OptionsView.ShowHorzLines = false;
            treeList1.OptionsView.ShowIndicator = false;
            treeList1.OptionsView.ShowVertLines = false;
            treeList1.RowHeight = 23;
            treeList1.SelectImageList = icAttachmentCatagoryIcons;
            treeList1.Size = new Size(255, 436);
            treeList1.TabIndex = 0;
            treeList1.TreeLevelWidth = 21;
            treeList1.NodeCellStyle += treeList1_NodeCellStyle;
            treeList1.FocusedNodeChanged += treeList1_FocusedNodeChanged;
            treeList1.PopupMenuShowing += treeList1_PopupMenuShowing;
            // 
            // treeListColumn1
            // 
            treeListColumn1.Caption = "treeListColumn1";
            treeListColumn1.FieldName = "Description";
            treeListColumn1.MinWidth = 38;
            treeListColumn1.Name = "treeListColumn1";
            treeListColumn1.Visible = true;
            treeListColumn1.VisibleIndex = 0;
            treeListColumn1.Width = 350;
            // 
            // treeListColumn2
            // 
            treeListColumn2.Caption = "treeListColumn2";
            treeListColumn2.FieldName = "Id";
            treeListColumn2.MinWidth = 23;
            treeListColumn2.Name = "treeListColumn2";
            treeListColumn2.Width = 87;
            // 
            // icAttachmentCatagoryIcons
            // 
            icAttachmentCatagoryIcons.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("icAttachmentCatagoryIcons.ImageStream");
            icAttachmentCatagoryIcons.Images.SetKeyName(0, "Catalogue Icon.jpg");
            icAttachmentCatagoryIcons.Images.SetKeyName(1, "manual-icon.png");
            icAttachmentCatagoryIcons.Images.SetKeyName(2, "original-photosicon-icon-19.png");
            icAttachmentCatagoryIcons.Images.SetKeyName(3, "passport-icon.png");
            icAttachmentCatagoryIcons.Images.SetKeyName(4, "Personal Photo Icon.jpg");
            icAttachmentCatagoryIcons.Images.SetKeyName(5, "Reference Documents Icon.png");
            icAttachmentCatagoryIcons.Images.SetKeyName(6, "Audio-icon.png");
            icAttachmentCatagoryIcons.Images.SetKeyName(7, "document.png");
            icAttachmentCatagoryIcons.InsertGalleryImage("image_16x16.png", "images/content/image_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/content/image_16x16.png"), 8);
            icAttachmentCatagoryIcons.Images.SetKeyName(8, "image_16x16.png");
            icAttachmentCatagoryIcons.InsertGalleryImage("video_16x16.png", "images/miscellaneous/video_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/miscellaneous/video_16x16.png"), 9);
            icAttachmentCatagoryIcons.Images.SetKeyName(9, "video_16x16.png");
            icAttachmentCatagoryIcons.Images.SetKeyName(10, "License-manager-icon.png");
            icAttachmentCatagoryIcons.Images.SetKeyName(11, "Custom-Icon-Design-Pretty-Office-10-Diploma-Certificate.ico");
            icAttachmentCatagoryIcons.Images.SetKeyName(12, "Clean+Professional+Logo_blog.png");
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(pictureEdit1);
            panel1.Controls.Add(axWindowsMediaPlayer1);
            panel1.Controls.Add(spreadsheetControl1);
            panel1.Controls.Add(richEditControl1);
            panel1.Controls.Add(pdfViewer1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(897, 436);
            panel1.TabIndex = 0;
            // 
            // pictureEdit1
            // 
            pictureEdit1.Dock = DockStyle.Fill;
            pictureEdit1.Location = new Point(0, 0);
            pictureEdit1.Margin = new Padding(4, 3, 4, 3);
            pictureEdit1.MenuManager = ribbonControl1;
            pictureEdit1.Name = "pictureEdit1";
            pictureEdit1.Properties.ShowScrollBars = true;
            pictureEdit1.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
            pictureEdit1.Size = new Size(897, 436);
            pictureEdit1.TabIndex = 6;
            // 
            // axWindowsMediaPlayer1
            // 
            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            axWindowsMediaPlayer1.Enabled = true;
            axWindowsMediaPlayer1.Location = new Point(0, 0);
            axWindowsMediaPlayer1.Margin = new Padding(4, 3, 4, 3);
            axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            axWindowsMediaPlayer1.OcxState = (AxHost.State)resources.GetObject("axWindowsMediaPlayer1.OcxState");
            axWindowsMediaPlayer1.Size = new Size(897, 436);
            axWindowsMediaPlayer1.TabIndex = 5;
            // 
            // spreadsheetControl1
            // 
            spreadsheetControl1.Dock = DockStyle.Fill;
            spreadsheetControl1.Location = new Point(0, 0);
            spreadsheetControl1.Margin = new Padding(4, 3, 4, 3);
            spreadsheetControl1.MenuManager = ribbonControl1;
            spreadsheetControl1.Name = "spreadsheetControl1";
            spreadsheetControl1.ReadOnly = true;
            spreadsheetControl1.Size = new Size(897, 436);
            spreadsheetControl1.TabIndex = 4;
            spreadsheetControl1.Text = "spreadsheetControl1";
            spreadsheetControl1.MouseHover += spreadsheetControl1_MouseHover;
            // 
            // richEditControl1
            // 
            richEditControl1.Dock = DockStyle.Fill;
            richEditControl1.Location = new Point(0, 0);
            richEditControl1.Margin = new Padding(4, 3, 4, 3);
            richEditControl1.MenuManager = ribbonControl1;
            richEditControl1.Name = "richEditControl1";
            richEditControl1.ReadOnly = true;
            richEditControl1.Size = new Size(897, 436);
            richEditControl1.TabIndex = 1;
            richEditControl1.MouseHover += richEditControl1_MouseHover;
            // 
            // pdfViewer1
            // 
            pdfViewer1.Dock = DockStyle.Fill;
            pdfViewer1.Location = new Point(0, 0);
            pdfViewer1.Margin = new Padding(4, 3, 4, 3);
            pdfViewer1.MenuManager = ribbonControl1;
            pdfViewer1.Name = "pdfViewer1";
            pdfViewer1.Size = new Size(897, 436);
            pdfViewer1.TabIndex = 2;
            pdfViewer1.MouseHover += pdfViewer1_MouseHover;
            // 
            // ERP_Attachments
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainerControl1);
            Controls.Add(ribbonControl1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "ERP_Attachments";
            Size = new Size(1162, 519);
            ((System.ComponentModel.ISupportInitialize)ribbonControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)treeList1).EndInit();
            ((System.ComponentModel.ISupportInitialize)icAttachmentCatagoryIcons).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)axWindowsMediaPlayer1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.BarButtonItem bbiBrowse;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem bbiNew;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraSpreadsheet.SpreadsheetControl spreadsheetControl1;
        private DevExpress.XtraPdfViewer.PdfViewer pdfViewer1;
        private DevExpress.XtraRichEdit.RichEditControl richEditControl1;
        private System.Windows.Forms.Panel panel1;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.Utils.ImageCollection icAttachmentCatagoryIcons;
        private DevExpress.XtraTreeList.TreeList treeList1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraBars.BarButtonItem btnDeleteAtt;
    }
}
