namespace DocumentPrint.Forms
{
    partial class PDFViewer
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PDFViewer));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
            DocViewer = new DevExpress.XtraPdfViewer.PdfViewer();
            barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            barManager1 = new DevExpress.XtraBars.BarManager(components);
            pdfCommandBar1 = new DevExpress.XtraPdfViewer.Bars.PdfCommandBar();
            pdfFileOpenBarItem1 = new DevExpress.XtraPdfViewer.Bars.PdfFileOpenBarItem();
            pdfFilePrintBarItem1 = new DevExpress.XtraPdfViewer.Bars.PdfFilePrintBarItem();
            bsiEmail = new DevExpress.XtraBars.BarButtonItem();
            pdfPreviousPageBarItem1 = new DevExpress.XtraPdfViewer.Bars.PdfPreviousPageBarItem();
            pdfNextPageBarItem1 = new DevExpress.XtraPdfViewer.Bars.PdfNextPageBarItem();
            pdfFindTextBarItem1 = new DevExpress.XtraPdfViewer.Bars.PdfFindTextBarItem();
            pdfZoomOutBarItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoomOutBarItem();
            pdfZoomInBarItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoomInBarItem();
            pdfExactZoomListBarSubItem1 = new DevExpress.XtraPdfViewer.Bars.PdfExactZoomListBarSubItem();
            pdfZoom10CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom10CheckItem();
            pdfZoom25CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom25CheckItem();
            pdfZoom50CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom50CheckItem();
            pdfZoom75CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom75CheckItem();
            pdfZoom100CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom100CheckItem();
            pdfZoom125CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom125CheckItem();
            pdfZoom150CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom150CheckItem();
            pdfZoom200CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom200CheckItem();
            pdfZoom400CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom400CheckItem();
            pdfZoom500CheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfZoom500CheckItem();
            pdfSetActualSizeZoomModeCheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfSetActualSizeZoomModeCheckItem();
            pdfSetPageLevelZoomModeCheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfSetPageLevelZoomModeCheckItem();
            pdfSetFitWidthZoomModeCheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfSetFitWidthZoomModeCheckItem();
            pdfSetFitVisibleZoomModeCheckItem1 = new DevExpress.XtraPdfViewer.Bars.PdfSetFitVisibleZoomModeCheckItem();
            bbiClose = new DevExpress.XtraBars.BarButtonItem();
            barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            pdfBarController1 = new DevExpress.XtraPdfViewer.Bars.PdfBarController(components);
            DocViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)barManager1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pdfBarController1).BeginInit();
            SuspendLayout();
            // 
            // DocViewer
            // 
            DocViewer.Controls.Add(barDockControlLeft);
            DocViewer.Controls.Add(barDockControlRight);
            DocViewer.Controls.Add(barDockControlBottom);
            DocViewer.Controls.Add(barDockControlTop);
            DocViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            DocViewer.Location = new System.Drawing.Point(0, 0);
            DocViewer.MenuManager = barManager1;
            DocViewer.Name = "DocViewer";
            DocViewer.Size = new System.Drawing.Size(1001, 520);
            DocViewer.TabIndex = 0;
            DocViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.PageLevel;
            // 
            // barDockControlLeft
            // 
            barDockControlLeft.CausesValidation = false;
            barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            barDockControlLeft.Location = new System.Drawing.Point(0, 24);
            barDockControlLeft.Manager = barManager1;
            barDockControlLeft.Size = new System.Drawing.Size(0, 496);
            // 
            // barManager1
            // 
            barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] { pdfCommandBar1 });
            barManager1.DockControls.Add(barDockControlTop);
            barManager1.DockControls.Add(barDockControlBottom);
            barManager1.DockControls.Add(barDockControlLeft);
            barManager1.DockControls.Add(barDockControlRight);
            barManager1.Form = DocViewer;
            barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { pdfFileOpenBarItem1, pdfFilePrintBarItem1, pdfPreviousPageBarItem1, pdfNextPageBarItem1, pdfFindTextBarItem1, pdfZoomOutBarItem1, pdfZoomInBarItem1, pdfExactZoomListBarSubItem1, pdfZoom10CheckItem1, pdfZoom25CheckItem1, pdfZoom50CheckItem1, pdfZoom75CheckItem1, pdfZoom100CheckItem1, pdfZoom125CheckItem1, pdfZoom150CheckItem1, pdfZoom200CheckItem1, pdfZoom400CheckItem1, pdfZoom500CheckItem1, pdfSetActualSizeZoomModeCheckItem1, pdfSetPageLevelZoomModeCheckItem1, pdfSetFitWidthZoomModeCheckItem1, pdfSetFitVisibleZoomModeCheckItem1, bsiEmail, bbiClose });
            barManager1.MaxItemId = 26;
            // 
            // pdfCommandBar1
            // 
            pdfCommandBar1.Control = DocViewer;
            pdfCommandBar1.DockCol = 0;
            pdfCommandBar1.DockRow = 0;
            pdfCommandBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            pdfCommandBar1.FloatLocation = new System.Drawing.Point(-1221, 136);
            pdfCommandBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(pdfFileOpenBarItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfFilePrintBarItem1), new DevExpress.XtraBars.LinkPersistInfo(bsiEmail), new DevExpress.XtraBars.LinkPersistInfo(pdfPreviousPageBarItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfNextPageBarItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfFindTextBarItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoomOutBarItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoomInBarItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfExactZoomListBarSubItem1), new DevExpress.XtraBars.LinkPersistInfo(bbiClose) });
            pdfCommandBar1.OptionsBar.AllowDelete = true;
            pdfCommandBar1.OptionsBar.AllowQuickCustomization = false;
            pdfCommandBar1.OptionsBar.AllowRename = true;
            pdfCommandBar1.OptionsBar.DisableCustomization = true;
            pdfCommandBar1.OptionsBar.DrawDragBorder = false;
            // 
            // pdfFileOpenBarItem1
            // 
            pdfFileOpenBarItem1.Id = 2;
            pdfFileOpenBarItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O);
            pdfFileOpenBarItem1.Name = "pdfFileOpenBarItem1";
            // 
            // pdfFilePrintBarItem1
            // 
            pdfFilePrintBarItem1.Id = 3;
            pdfFilePrintBarItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P);
            pdfFilePrintBarItem1.Name = "pdfFilePrintBarItem1";
            // 
            // bsiEmail
            // 
            bsiEmail.Caption = "Email";
            bsiEmail.Id = 24;
            bsiEmail.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bsiEmail.ImageOptions.Image");
            bsiEmail.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bsiEmail.ImageOptions.LargeImage");
            bsiEmail.Name = "bsiEmail";
            bsiEmail.ItemClick += bsiEmail_ItemClick;
            // 
            // pdfPreviousPageBarItem1
            // 
            pdfPreviousPageBarItem1.Id = 4;
            pdfPreviousPageBarItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.PageUp);
            pdfPreviousPageBarItem1.Name = "pdfPreviousPageBarItem1";
            // 
            // pdfNextPageBarItem1
            // 
            pdfNextPageBarItem1.Id = 5;
            pdfNextPageBarItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.PageDown);
            pdfNextPageBarItem1.Name = "pdfNextPageBarItem1";
            // 
            // pdfFindTextBarItem1
            // 
            pdfFindTextBarItem1.Id = 6;
            pdfFindTextBarItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F);
            pdfFindTextBarItem1.Name = "pdfFindTextBarItem1";
            // 
            // pdfZoomOutBarItem1
            // 
            pdfZoomOutBarItem1.Id = 7;
            pdfZoomOutBarItem1.Name = "pdfZoomOutBarItem1";
            toolTipTitleItem1.Text = "Zoom Out (Ctrl + Minus)";
            toolTipItem1.Text = "Zoom out to see more of the page at a reduced size.";
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            pdfZoomOutBarItem1.SuperTip = superToolTip1;
            // 
            // pdfZoomInBarItem1
            // 
            pdfZoomInBarItem1.Id = 8;
            pdfZoomInBarItem1.Name = "pdfZoomInBarItem1";
            toolTipTitleItem2.Text = "Zoom In (Ctrl + Plus)";
            toolTipItem2.Text = "Zoom in to get a close-up view of the PDF document.";
            superToolTip2.Items.Add(toolTipTitleItem2);
            superToolTip2.Items.Add(toolTipItem2);
            pdfZoomInBarItem1.SuperTip = superToolTip2;
            // 
            // pdfExactZoomListBarSubItem1
            // 
            pdfExactZoomListBarSubItem1.Id = 9;
            pdfExactZoomListBarSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(pdfZoom10CheckItem1, true), new DevExpress.XtraBars.LinkPersistInfo(pdfZoom25CheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoom50CheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoom75CheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoom100CheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoom125CheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoom150CheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoom200CheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoom400CheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfZoom500CheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfSetActualSizeZoomModeCheckItem1, true), new DevExpress.XtraBars.LinkPersistInfo(pdfSetPageLevelZoomModeCheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfSetFitWidthZoomModeCheckItem1), new DevExpress.XtraBars.LinkPersistInfo(pdfSetFitVisibleZoomModeCheckItem1) });
            pdfExactZoomListBarSubItem1.Name = "pdfExactZoomListBarSubItem1";
            pdfExactZoomListBarSubItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionInMenu;
            // 
            // pdfZoom10CheckItem1
            // 
            pdfZoom10CheckItem1.Id = 10;
            pdfZoom10CheckItem1.Name = "pdfZoom10CheckItem1";
            // 
            // pdfZoom25CheckItem1
            // 
            pdfZoom25CheckItem1.Id = 11;
            pdfZoom25CheckItem1.Name = "pdfZoom25CheckItem1";
            // 
            // pdfZoom50CheckItem1
            // 
            pdfZoom50CheckItem1.Id = 12;
            pdfZoom50CheckItem1.Name = "pdfZoom50CheckItem1";
            // 
            // pdfZoom75CheckItem1
            // 
            pdfZoom75CheckItem1.Id = 13;
            pdfZoom75CheckItem1.Name = "pdfZoom75CheckItem1";
            // 
            // pdfZoom100CheckItem1
            // 
            pdfZoom100CheckItem1.Id = 14;
            pdfZoom100CheckItem1.Name = "pdfZoom100CheckItem1";
            // 
            // pdfZoom125CheckItem1
            // 
            pdfZoom125CheckItem1.Id = 15;
            pdfZoom125CheckItem1.Name = "pdfZoom125CheckItem1";
            // 
            // pdfZoom150CheckItem1
            // 
            pdfZoom150CheckItem1.Id = 16;
            pdfZoom150CheckItem1.Name = "pdfZoom150CheckItem1";
            // 
            // pdfZoom200CheckItem1
            // 
            pdfZoom200CheckItem1.Id = 17;
            pdfZoom200CheckItem1.Name = "pdfZoom200CheckItem1";
            // 
            // pdfZoom400CheckItem1
            // 
            pdfZoom400CheckItem1.Id = 18;
            pdfZoom400CheckItem1.Name = "pdfZoom400CheckItem1";
            // 
            // pdfZoom500CheckItem1
            // 
            pdfZoom500CheckItem1.Id = 19;
            pdfZoom500CheckItem1.Name = "pdfZoom500CheckItem1";
            // 
            // pdfSetActualSizeZoomModeCheckItem1
            // 
            pdfSetActualSizeZoomModeCheckItem1.Id = 20;
            pdfSetActualSizeZoomModeCheckItem1.Name = "pdfSetActualSizeZoomModeCheckItem1";
            // 
            // pdfSetPageLevelZoomModeCheckItem1
            // 
            pdfSetPageLevelZoomModeCheckItem1.Id = 21;
            pdfSetPageLevelZoomModeCheckItem1.Name = "pdfSetPageLevelZoomModeCheckItem1";
            // 
            // pdfSetFitWidthZoomModeCheckItem1
            // 
            pdfSetFitWidthZoomModeCheckItem1.Id = 22;
            pdfSetFitWidthZoomModeCheckItem1.Name = "pdfSetFitWidthZoomModeCheckItem1";
            // 
            // pdfSetFitVisibleZoomModeCheckItem1
            // 
            pdfSetFitVisibleZoomModeCheckItem1.Id = 23;
            pdfSetFitVisibleZoomModeCheckItem1.Name = "pdfSetFitVisibleZoomModeCheckItem1";
            // 
            // bbiClose
            // 
            bbiClose.Caption = "Close";
            bbiClose.Id = 25;
            bbiClose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bbiClose.ImageOptions.Image");
            bbiClose.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("bbiClose.ImageOptions.LargeImage");
            bbiClose.Name = "bbiClose";
            bbiClose.ItemClick += bbiClose_ItemClick;
            // 
            // barDockControlTop
            // 
            barDockControlTop.CausesValidation = false;
            barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            barDockControlTop.Location = new System.Drawing.Point(0, 0);
            barDockControlTop.Manager = barManager1;
            barDockControlTop.Size = new System.Drawing.Size(1001, 24);
            // 
            // barDockControlBottom
            // 
            barDockControlBottom.CausesValidation = false;
            barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            barDockControlBottom.Location = new System.Drawing.Point(0, 520);
            barDockControlBottom.Manager = barManager1;
            barDockControlBottom.Size = new System.Drawing.Size(1001, 0);
            // 
            // barDockControlRight
            // 
            barDockControlRight.CausesValidation = false;
            barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            barDockControlRight.Location = new System.Drawing.Point(1001, 24);
            barDockControlRight.Manager = barManager1;
            barDockControlRight.Size = new System.Drawing.Size(0, 496);
            // 
            // pdfBarController1
            // 
            pdfBarController1.BarItems.Add(pdfFileOpenBarItem1);
            pdfBarController1.BarItems.Add(pdfFilePrintBarItem1);
            pdfBarController1.BarItems.Add(pdfPreviousPageBarItem1);
            pdfBarController1.BarItems.Add(pdfNextPageBarItem1);
            pdfBarController1.BarItems.Add(pdfFindTextBarItem1);
            pdfBarController1.BarItems.Add(pdfZoomOutBarItem1);
            pdfBarController1.BarItems.Add(pdfZoomInBarItem1);
            pdfBarController1.BarItems.Add(pdfExactZoomListBarSubItem1);
            pdfBarController1.BarItems.Add(pdfZoom10CheckItem1);
            pdfBarController1.BarItems.Add(pdfZoom25CheckItem1);
            pdfBarController1.BarItems.Add(pdfZoom50CheckItem1);
            pdfBarController1.BarItems.Add(pdfZoom75CheckItem1);
            pdfBarController1.BarItems.Add(pdfZoom100CheckItem1);
            pdfBarController1.BarItems.Add(pdfZoom125CheckItem1);
            pdfBarController1.BarItems.Add(pdfZoom150CheckItem1);
            pdfBarController1.BarItems.Add(pdfZoom200CheckItem1);
            pdfBarController1.BarItems.Add(pdfZoom400CheckItem1);
            pdfBarController1.BarItems.Add(pdfZoom500CheckItem1);
            pdfBarController1.BarItems.Add(pdfSetActualSizeZoomModeCheckItem1);
            pdfBarController1.BarItems.Add(pdfSetPageLevelZoomModeCheckItem1);
            pdfBarController1.BarItems.Add(pdfSetFitWidthZoomModeCheckItem1);
            pdfBarController1.BarItems.Add(pdfSetFitVisibleZoomModeCheckItem1);
            pdfBarController1.Control = DocViewer;
            // 
            // PDFViewer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1001, 520);
            Controls.Add(DocViewer);
            IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("PDFViewer.IconOptions.Icon");
            Name = "PDFViewer";
            Text = "PDF Viewer";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            DocViewer.ResumeLayout(false);
            DocViewer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)barManager1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pdfBarController1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraPdfViewer.Bars.PdfCommandBar pdfCommandBar1;
        private DevExpress.XtraPdfViewer.Bars.PdfFileOpenBarItem pdfFileOpenBarItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfFilePrintBarItem pdfFilePrintBarItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfPreviousPageBarItem pdfPreviousPageBarItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfNextPageBarItem pdfNextPageBarItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfFindTextBarItem pdfFindTextBarItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoomOutBarItem pdfZoomOutBarItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoomInBarItem pdfZoomInBarItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfExactZoomListBarSubItem pdfExactZoomListBarSubItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom10CheckItem pdfZoom10CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom25CheckItem pdfZoom25CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom50CheckItem pdfZoom50CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom75CheckItem pdfZoom75CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom100CheckItem pdfZoom100CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom125CheckItem pdfZoom125CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom150CheckItem pdfZoom150CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom200CheckItem pdfZoom200CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom400CheckItem pdfZoom400CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfZoom500CheckItem pdfZoom500CheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfSetActualSizeZoomModeCheckItem pdfSetActualSizeZoomModeCheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfSetPageLevelZoomModeCheckItem pdfSetPageLevelZoomModeCheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfSetFitWidthZoomModeCheckItem pdfSetFitWidthZoomModeCheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfSetFitVisibleZoomModeCheckItem pdfSetFitVisibleZoomModeCheckItem1;
        private DevExpress.XtraPdfViewer.Bars.PdfBarController pdfBarController1;
        private DevExpress.XtraBars.BarButtonItem bsiEmail;
        public DevExpress.XtraPdfViewer.PdfViewer DocViewer;
        private DevExpress.XtraBars.BarButtonItem bbiClose;
    }
}