using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using DevExpress.XtraEditors;
using System.IO;
using CNET_V7_Domain;
using DocumentPrint.DTO;
using System.Net.Mail;
using DevExpress.Utils.Controls;
using CNET_V7_Domain.Domain.SettingSchema;
using DocumentPrint.Grid;
using DevExpress.XtraReports.UI;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.TransactionSchema;
using ProcessManager;
using DevExpress.Xpo.DB;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using System.Text;
using System.Diagnostics;
using CNET_V7_Domain.Domain.CommonSchema;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using DevExpress.XtraPrinting;

namespace DocumentPrint
{
    public partial class PrintDialog : DevExpress.XtraEditors.XtraForm
    {
        private string _documentname;
        private int? _ConsigneeId;
        private int? _BranchId;

        public int? BranchId
        {
            get { return _BranchId; }
            set { _BranchId = value; }
        }

        public int? ConsigneeId
        {
            get { return _ConsigneeId; }
            set { _ConsigneeId = value; }
        }

        public string DocumentName
        {
            get { return _documentname; }
            set { _documentname = value; }
        }

        private static Bitmap _signature;
        public static Bitmap Signature
        {
            get { return _signature; }
            set { _signature = value; }
        }

        public PrintDialog()
        {
            InitializeComponent();
        }

        bool IsPortrait { get; set; }
        RegistrationPrintOutDTO registrationPrintOut { get; set; }
        VoucherPrintModel VoucherPrint { get; set; }
        HeaderDTO NonLineItemprintmodel { get; set; }

        public void PrintReport(DevExpress.XtraReports.UI.XtraReport report, VoucherPrintModel VoucherPrintmodel = null, bool ontab = false, RegistrationPrintOutDTO registrationPrintOutdata = null, HeaderDTO headerdata = null, bool portrait = true)
        {
            documentViewer1.ForeColor = Color.Black;
            documentViewer1.DocumentSource = null;
            report.CreateDocument();
            documentViewer1.DocumentSource = report;


            IsPortrait = portrait;

            if (ontab && IsPortrait)
                AddSignaturePanel(new Point(452, 1050), new Size(333, 219), "Signature panel");
            else if (ontab && !IsPortrait)
            {
                AddSignaturePanel(new Point(980, 550), new Size(300, 200), "Signature panel");
                //this.VerticalScroll.Enabled = false;    
            }
       
            registrationPrintOut = registrationPrintOutdata;

            NonLineItemprintmodel = headerdata;
            if (NonLineItemprintmodel != null)
            {
                MaxNoOfPrinting = NonLineItemprintmodel.MaxNoOfPrinting;
                mPrintCount = NonLineItemprintmodel.PrintCount;
                voucherId = NonLineItemprintmodel.VoucherId;
                NoOfCopies = NonLineItemprintmodel.NoOfCopies;
                //DistrbutionPrinterList = NonLineItemprintmodel.DistrbutionPrinterList;
                //CopyDescription = NonLineItemprintmodel.CopyDescription;
                PrintCopyDistribution = NonLineItemprintmodel.PrintCopyDistribution;
                defaultPrinter = NonLineItemprintmodel.defaultPrinter;

            }

            VoucherPrint = VoucherPrintmodel;

            if(VoucherPrint != null)
            {
                MaxNoOfPrinting = VoucherPrint.MaxNoOfPrinting;
                mPrintCount = VoucherPrint.PrintCount;
                voucherId = VoucherPrint.VoucherId;
                NoOfCopies = VoucherPrint.NoOfCopies;
                DistrbutionPrinterList = VoucherPrint.DistrbutionPrinterList;
                CopyDescription = VoucherPrint.CopyDescription;
                PrintCopyDistribution = VoucherPrint.PrintCopyDistribution;
                defaultPrinter = VoucherPrint.defaultPrinter;
                
            }
            else
            {
                MaxNoOfPrinting = 5;
                mPrintCount = 0;
                voucherId = 1;
                NoOfCopies = 1;
                DistrbutionPrinterList = new List<string>() ;
                CopyDescription = new List<string>();
                PrintCopyDistribution = false;
                defaultPrinter = "";

            }

        }


        string defaultPrinter { get; set; }
        bool PrintCopyDistribution { get; set; }
        
        public List<string> CopyDescription { get; set; }
        public List<string> DistrbutionPrinterList { get; set; }
        short NoOfCopies = 0;
        int voucherId = 0;
        uint MaxNoOfPrinting = 1;
        int mPrintCount { get; set; }
        bool mIsVoucherPrinted { get; set; }
        ScribbleWinTab.TransparentSignature signatureCtrl = null;
        ScribbleWinTab.TransparentControlLandscape TransparentControlLandscape = null;
        private void AddSignaturePanel(Point p, Size size, string ControlName)
        {
            barDockControlTop.Visible = false;
            Control[] c = documentViewer1.Controls.Find(ControlName, false);
            if (c != null && c.Length > 0)
            {
                Control scribblec = c.FirstOrDefault();
                documentViewer1.Controls.Remove(scribblec);
            }
            signatureCtrl = new ScribbleWinTab.TransparentSignature(size);
            signatureCtrl.Location = p;
            signatureCtrl.PenEvent += new ScribbleWinTab.TransparentSignature.PenEventHandler(TriggerPenEvent);
            documentViewer1.Controls.Add(signatureCtrl);
            signatureCtrl.Name = ControlName;
            signatureCtrl.Parent = documentViewer1;
            signatureCtrl.BringToFront();
            documentViewer1.Zoom = 0.9f;

            this.Invalidate();
        }
        private void TriggerPenEvent(object sender, ScribbleWinTab.PenEventArgs e)
        {

            if (printinprogress)
                return;
            try
            {
                switch (e.Eventtype)
                {
                    case ScribbleWinTab.PenEventArgs.EventType.Cancel:
                        signatureCtrl.CloseCurrentContext();
                        RemoveSignaturePanel(signatureCtrl.Name);
                        AddSignaturePanel(signatureCtrl.Location, signatureCtrl.Size, signatureCtrl.Name);
                        break;
                    case ScribbleWinTab.PenEventArgs.EventType.Ok:
                        Signature = e.Signature;
                        PrintSignature(Signature);
                        break;
                }
            }
            catch {; }
        }
        private void AddSignaturePanelLandscape(Point p, Size size, string ControlName)
        {
            //barDockControlTop.Visible = false;
            //Control[] c = documentViewer1.Controls.Find(ControlName, false);
            //if (c != null && c.Length > 0)
            //{
            //    Control scribblec = c.FirstOrDefault();
            //    documentViewer1.Controls.Remove(scribblec);
            //}
            //TransparentControlLandscape = new ScribbleWinTab.TransparentControlLandscape();
            //TransparentControlLandscape.Size = size;
            //TransparentControlLandscape.Location = p;
            //TransparentControlLandscape.PenEvent += new ScribbleWinTab.TransparentControlLandscape.PenEventHandler(TriggerPenEventLandscscape);
            //documentViewer1.Controls.Add(TransparentControlLandscape);
            //TransparentControlLandscape.Name = ControlName;
            //TransparentControlLandscape.Parent = documentViewer1;
            //TransparentControlLandscape.BringToFront();
            //documentViewer1.Zoom = 0.85f;

            //this.Invalidate();
        }
        //private void TriggerPenEventLandscscape(object sender, ScribbleWinTab.PenEventArgs e)
        //{
        //    try
        //    {
        //        switch (e.Eventtype)
        //        switch (e.Eventtype)
        //        {
        //            case ScribbleWinTab.PenEventArgs.EventType.Cancel:
        //                TransparentControlLandscape.CloseCurrentContext();
        //                RemoveSignaturePanel(TransparentControlLandscape.Name);
        //                AddSignaturePanelLandscape(TransparentControlLandscape.Location, TransparentControlLandscape.Size, TransparentControlLandscape.Name);
        //                break;
        //            case ScribbleWinTab.PenEventArgs.EventType.Ok:
        //                Signature = e.Signature;
        //                PrintSignature(Signature);
        //                break;
        //        }
        //    }
        //    catch {; }
        //}
        private void RemoveSignaturePanel(string ControlName)
        {
            Control[] c = documentViewer1.Controls.Find(ControlName, false);
            if (c != null && c.Length > 0)
            {
                Control scribblec = c.FirstOrDefault();

                // signatureCtrl.PenEvent -= new ScribbleWinTab.TransparentSignature.PenEventHandler(TriggerPenEvent);
                documentViewer1.Controls.Remove(scribblec);
            }
        }
        private string GetLocalExportPath(string basePath, string SelectedHotelcode)
        {
            DateTime CurrentTime = DateTime.Now;
            StringBuilder sb = new StringBuilder();

            sb.Append(basePath);
            sb.Append("\\");
            sb.Append(SelectedHotelcode);
            sb.Append("\\");
            sb.Append(CurrentTime.ToString("dd-MM-yyy"));

            string path = sb.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
        bool printinprogress { get; set; }
        private void PrintSignature(Bitmap signature)
        {
            printinprogress = true;
            try
            {
                Form lf = Application.OpenForms.OfType<Form>().Where((t) => t.Text.ToLower() == "home").LastOrDefault();
                DialogResult result = XtraMessageBox.Show(lf, "Signature captured successfully.Do you want to save the file?", "CNET_ERPV2016", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    string baseDir = GetLocalExportPath(Environment.CurrentDirectory + @"//Transaction", BranchId != null ? BranchId.Value.ToString() : "0");

                    string localsavepath = baseDir + @"\" + registrationPrintOut.RegistrationNo + ".pdf";

                    if (IsPortrait)
                    {
                        PMSRegistration ConformationDetail = new PMSRegistration(registrationPrintOut, signature);
                        ConformationDetail.ExportToPdf(localsavepath);
                    }
                    else
                    {
                        PMSRegistrationLandscape ConformationDetail = new PMSRegistrationLandscape(registrationPrintOut, signature);
                        ConformationDetail.ExportToPdf(localsavepath);
                    }


                    if (File.Exists(localsavepath))
                    {
                        bool Exist = FTPInterface.FTPAttachment.InitalizeFTPAttachment(LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
                        if (Exist)
                        {
                            FTPInterface.FTPAttachment.ORGUnitDefcode = BranchId != null ? BranchId.Value.ToString() : "0";
                            DocumentName = FTPInterface.FTPAttachment.SendTransactionAttachement(CNETConstantes.REGISTRATION_VOUCHER, localsavepath, registrationPrintOut.RegistrationId);
                        }
                        else
                        {
                            XtraMessageBox.Show("No FTP Attachement location Found !!", "CNET_ERPV2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        //if (!string.IsNullOrEmpty(DocumentName))
                        //{
                        //    VoucherDTO Registrationvoucher = UIProcessManager.GetVoucherById(registrationPrintOut.RegistrationId);
                        //    if (Registrationvoucher != null)
                        //    {
                        //        Registrationvoucher.DefaultImageUrl = DocumentName;
                        //        Registrationvoucher.LastModified = DateTime.Now;
                        //        UIProcessManager.UpdateVoucher(Registrationvoucher);
                        //    }
                        //}
                    }

                    Signature = null;
                    if (signatureCtrl != null || TransparentControlLandscape != null)
                    {
                        if (signatureCtrl != null)
                        {
                            signatureCtrl.CloseCurrentContext();
                            RemoveSignaturePanel(signatureCtrl.Name);
                        }
                        else
                        {
                            TransparentControlLandscape.CloseCurrentContext();
                            RemoveSignaturePanel(TransparentControlLandscape.Name);
                        }
                    }
                    this.Close();
                }
                else if (result == DialogResult.No)
                {
                    if (signatureCtrl != null || TransparentControlLandscape != null)
                    {
                        //if (signatureCtrl != null)
                        //{
                        signatureCtrl.CloseCurrentContext();
                        RemoveSignaturePanel(signatureCtrl.Name);
                        AddSignaturePanel(signatureCtrl.Location, signatureCtrl.Size, signatureCtrl.Name);
                        //}
                        //else
                        //{
                        //    TransparentControlLandscape.CloseCurrentContext();
                        //    RemoveSignaturePanel(TransparentControlLandscape.Name);
                        //    AddSignaturePanel(signatureCtrl.Location, signatureCtrl.Size, signatureCtrl.Name);
                        //}

                    }
                }


                //if (!string.IsNullOrEmpty(DocumentPrintSetting.DefaultAttachmentPath))
                //{
                //    if (Directory.Exists(DocumentPrintSetting.DefaultAttachmentPath))
                //    {
                //        Form lf = Application.OpenForms.OfType<Form>().Where((t) => t.Text.ToLower() == "home").LastOrDefault();
                //        DialogResult result = XtraMessageBox.Show(lf, "Signature captured successfully.Do you want to save the file?", "CNET_ERPV2016", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                //        if (result == DialogResult.Yes)
                //        {
                //            #region  Save pdf and update voucher
                //            using (MemoryStream ms = new MemoryStream())
                //            {
                //                DocumentName = DocumentPrintSetting.DefaultAttachmentPath + @"\" + registrationPrintOut.RegistrationNo + ".pdf";
                //                FileStream fs = new FileStream(DocumentName, FileMode.Create, FileAccess.ReadWrite);
                //                ms.WriteTo(fs);
                //                fs.Close();
                //            }
                //            VoucherDTO Registrationvoucher = UIProcessManager.GetVoucherById(registrationPrintOut.RegistrationId);
                //            if (Registrationvoucher != null)
                //            {
                //                Registrationvoucher.DefaultImageUrl = DocumentName;
                //                Registrationvoucher.LastModified = DateTime.Now;
                //                UIProcessManager.UpdateVoucher(Registrationvoucher);
                //            }
                //            #endregion
                //            Signature = null;
                //            if (signatureCtrl != null || TransparentControlLandscape != null)
                //            {
                //                if (signatureCtrl != null)
                //                {
                //                    signatureCtrl.CloseCurrentContext();
                //                    RemoveSignaturePanel(signatureCtrl.Name);
                //                }
                //                else
                //                {
                //                    TransparentControlLandscape.CloseCurrentContext();
                //                    RemoveSignaturePanel(TransparentControlLandscape.Name);
                //                }
                //            }
                //           this.Close();
                //        }
                //        else if (result == DialogResult.No)
                //        {
                //            if (signatureCtrl != null || TransparentControlLandscape != null)
                //            {
                //                //if (signatureCtrl != null)
                //                //{
                //                    signatureCtrl.CloseCurrentContext();
                //                    RemoveSignaturePanel(signatureCtrl.Name);
                //                    AddSignaturePanel(signatureCtrl.Location, signatureCtrl.Size, signatureCtrl.Name);
                //                //}
                //                //else
                //                //{
                //                //    TransparentControlLandscape.CloseCurrentContext();
                //                //    RemoveSignaturePanel(TransparentControlLandscape.Name);
                //                //    AddSignaturePanel(signatureCtrl.Location, signatureCtrl.Size, signatureCtrl.Name);
                //                //}
                //            }
                //        }
                //    }
                //    else
                //    {
                //        //CNETInfoReporter.Hide();
                //        XtraMessageBox.Show("Default Attachment path Inaccessible or Doesn't Exist!", "CNET_ERPV2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    }
                //}
                //else
                //{
                //    //CNETInfoReporter.Hide();
                //    XtraMessageBox.Show("Default Attachment path not Saved!", "CNET_ERPV2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}


            }
            catch (Exception ex)
            {
                //CNETInfoReporter.Hide();
                XtraMessageBox.Show(ex.Message, "CNET_ERPV2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            printinprogress = false;
        }

        private void bbiDirectPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowVoucherDialog();
        }
        public void ShowVoucherDialog()
        {
            DevExpress.XtraPrinting.PrintTool Documentprint = new DevExpress.XtraPrinting.PrintTool(documentViewer1.PrintingSystem);
            if (PrintDocumentVoucher.DocumentBrowser)
            {  
                if (PrintCopyDistribution)
                {
                    if (DistrbutionPrinterList != null)
                    {
                        if (mPrintCount > DistrbutionPrinterList.Count - 1)
                        {
                            if (mPrintCount >= MaxNoOfPrinting)
                            {
                                mIsVoucherPrinted = false;
                                return;
                            }
                            mIsVoucherPrinted = true;
                            PrintDocument(Documentprint, PrintAction.PrintToPrinter);
                            mIsVoucherPrinted = false;
                        }
                        else
                        {
                            int j = 0;
                            for (int i = mPrintCount; i <= DistrbutionPrinterList.Count - 1; i++)
                            {

                                if (mPrintCount > CopyDescription.Count)
                                {
                                    mIsVoucherPrinted = false;
                                    return;
                                }

                                if (mPrintCount >= MaxNoOfPrinting)
                                {
                                    mIsVoucherPrinted = false;
                                    return;
                                }
                                //this is added so that multiple activities per distibution count is saved
                                if (j == 0)
                                {
                                    mIsVoucherPrinted = true;
                                }
                                else
                                {
                                    mIsVoucherPrinted = false;
                                }

                                DocumentprintDrawer.PrinterSettings.Copies = 1;
                                PrintDocument(Documentprint, PrintAction.PrintToPrinter, DistrbutionPrinterList[mPrintCount]);
                                //mPrintCount += 1;
                                j++;
                            }
                        }
                    }
                }
                else
                {
                    mIsVoucherPrinted = true;
                    if ( NoOfCopies < 1)
                    {
                        DocumentprintDrawer.PrinterSettings.Copies = 1;
                    }
                    else
                    {
                        DocumentprintDrawer.PrinterSettings.Copies = NoOfCopies;
                    }
                    PrintDocument(Documentprint);
                }


            }
            else
            {
                mPrintCount = 0;
                if (NoOfCopies < 1)
                {
                    NoOfCopies = 1;
                }
                if (PrintCopyDistribution)
                {
                    if (DistrbutionPrinterList != null)
                    {
                        if (DistrbutionPrinterList.Count == 0)
                        {

                            if (!string.IsNullOrEmpty(defaultPrinter))
                            {
                                if (CopyDescription.Count > 0)
                                {
                                    for (int i = 0; i <= CopyDescription.Count - 1; i++)
                                    {
                                        if (mPrintCount >= MaxNoOfPrinting)
                                        {
                                            return;
                                        }
                                        DocumentprintDrawer.PrinterSettings.Copies = 1;
                                        // DefaultPrinter = defaultPrinter;
                                        PrintDocument(Documentprint, PrintAction.PrintToPrinter, defaultPrinter);
                                        mPrintCount += 1;
                                    }
                                }
                                else
                                {
                                    DocumentprintDrawer.PrinterSettings.Copies = NoOfCopies;
                                    PrintDocument(Documentprint);
                                }
                            }
                        }
                        else
                        {

                            mIsVoucherPrinted = true;
                            if (mIsVoucherPrinted)
                            {
                                for (int i = 0; i <= DistrbutionPrinterList.Count - 1; i++)
                                {

                                    if (mPrintCount > CopyDescription.Count)
                                    {
                                        mIsVoucherPrinted = false;
                                        return;
                                    }

                                    if (mPrintCount >= MaxNoOfPrinting)
                                    {
                                        mIsVoucherPrinted = false;
                                        return;
                                    }
                                    DocumentprintDrawer.PrinterSettings.Copies = 1;
                                    PrintDocument(Documentprint, PrintAction.PrintToPrinter, DistrbutionPrinterList[mPrintCount]);
                                    mPrintCount += 1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    mIsVoucherPrinted = true;
                    DocumentprintDrawer.PrinterSettings.Copies = NoOfCopies;
                    PrintDocument(Documentprint);
                }
            }
            mIsVoucherPrinted = false;

        }
        private void SetParameterAndTemplate()
        {

            //documentViewer1.DocumentSource = null;
            //if (!PrintDocumentVoucher.IsFirstTime)
            //{ 
            //    this.DocumentprintDrawer = new PrintDocument();
            //    
            //    PrintDocumentVoucher.IsFirstTime = false;
            //}

            this.DocumentprintDrawer.Dispose();
            this.DocumentprintDrawer = new PrintDocument();
            this.DocumentprintDrawer.EndPrint += DocumentprintDrawer_EndPrint;
            this.documentViewer1.DocumentSource = this.DocumentprintDrawer;
            DocumentprintDrawer.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
        }

        private void DocumentprintDrawer_EndPrint(object sender, PrintEventArgs e)
        {
            try
            {
                //if (PrintDocumentVoucher.VoucherInformation.VoucherDefinition == CNETConstantes.REGISTRATION_VOUCHER && PrintDocumentVoucher.IsForced && ScribbleWinTab.TabInfo.IsTabAvailable() && Screen.AllScreens.Length > 1 && PrintDocumentVoucher.IsOnTab)
                //    if (PrintDocumentVoucher.VoucherOrientation.ToLower() == "portrait")
                //        AddSignaturePanel(new Point(407, 900), new Size(336, 250), "Signature panel");
                //    else if (PrintDocumentVoucher.VoucherOrientation.ToLower() == "landscape")
                //        AddSignaturePanelLandscape(new Point(756, 502), new Size(425, 170), "Signature panel");


                try
                {
                    if (mPrintCount >= MaxNoOfPrinting)
                    {
                        XtraMessageBox.Show("Number of prints allowed for the current document exceeds maximum no of prints allowed.", "CNET_V2016", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        bbiDirectPrint.Enabled = false;
                        bbiOptionalPrint.Enabled = false;
                        bbiSaveDocument.Enabled = false;
                        bsiExport.Enabled = false;

                    }
                    else
                    {
                        bbiDirectPrint.Enabled = true;
                        bbiOptionalPrint.Enabled = true;
                        bbiSaveDocument.Enabled = true;
                        bsiExport.Enabled = true;
                    }
                    if (mIsVoucherPrinted)
                    {
                        if (DocumentPrintSetting.PrintActivityDefinitionDTO != null)
                        {
                            DateTime? serverTimeStamp = UIProcessManager.GetServiceTime();

                            ActivityDTO activity = new ActivityDTO()
                            {
                                ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit,
                                TimeStamp = serverTimeStamp.Value,
                                Year = serverTimeStamp.Value.Year,
                                ActivityDefinition = DocumentPrintSetting.PrintActivityDefinitionDTO.Id,
                                Month = serverTimeStamp.Value.Month,
                                Day = serverTimeStamp.Value.Day,
                                Device = LocalBuffer.LocalBuffer.CurrentDevice.Id,
                                Pointer = CNETConstantes.VOUCHER_COMPONENET,
                                Platform = "1",
                                User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id
                            };
                            activity.Reference = voucherId;

                            if (UIProcessManager.CreateActivity(activity) != null)
                            {
                                mPrintCount++;
                                if (mPrintCount >= MaxNoOfPrinting)
                                {
                                    mIsVoucherPrinted = false;
                                    XtraMessageBox.Show("Number of prints allowed for the current document exceeds maximum no of prints allowed.", "CNET_V2016", MessageBoxButtons.OK,
                                                       MessageBoxIcon.Information);
                                    bbiDirectPrint.Enabled = false;
                                    bbiOptionalPrint.Enabled = false;
                                    bbiSaveDocument.Enabled = false;
                                    bsiExport.Enabled = false;
                                    return;
                                }
                            }
                        }

                    }
                }
                catch {; }
                
            }
            catch (Exception)
            {

            }
        }


        private void PrintDocument(PrintTool pt)
        {

            try
            {
                //if (!mPrintDocObj.PrinterSettings.IsValid)
                //{
                for (int i = 0; i <= System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count; i++)
                {
                    pt.PrinterSettings.PrinterName = System.Drawing.Printing.PrinterSettings.InstalledPrinters[i];
                    // mPrintDocObj.PrinterSettings.PrinterName = System.Drawing.Printing.PrinterSettings.InstalledPrinters[i];
                    //if (mPrintDocObj.PrinterSettings.IsDefaultPrinter)
                    //{
                    //    break;
                    //}
                    if (pt.PrinterSettings.IsDefaultPrinter)
                    {
                        break;
                    }
                }
                //}
                //
                //pt
                pt.Print();
            }
            catch (Exception ex)
            {
                ;
            }
        }
        private void PrintDocument(PrintTool pt, PrintAction Action, string mPrinter = "")
        {
            //if (PrintDocumentVoucher.PrintWithoutPreview && !PrintDocumentVoucher.DocumentBrowser)
            //{
            //    SetParameterAndTemplate();
            //}
            //mPrintDocObj.DefaultPageSettings.PaperSize = mPaperSize;
            if (string.IsNullOrEmpty(mPrinter))
            {
                 //mPrintDocObj.PrinterSettings.PrinterName = "HP LaserJet M101-M106";
                //if (!mPrintDocObj.PrinterSettings.IsValid)
                //{
                    for (int i = 0; i <= System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count - 1; i++)
                    {
                    pt.PrinterSettings.PrinterName = System.Drawing.Printing.PrinterSettings.InstalledPrinters[i];
                        if (pt.PrinterSettings.IsDefaultPrinter)
                        {
                            break;
                        }
                    }
                //}

            }
            else
            {
                pt.PrinterSettings.PrinterName = mPrinter;
                //if (!mPrintDocObj.PrinterSettings.IsValid)
                //{
                    for (int i = 0; i <= System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count - 1; i++)
                    {
                    pt.PrinterSettings.PrinterName = System.Drawing.Printing.PrinterSettings.InstalledPrinters[i];
                        if (pt.PrinterSettings.IsDefaultPrinter)
                        {
                            break;
                        }
                    }
                //}
            }
            try
            {
                if (Action == PrintAction.PrintToPrinter)
                {
                    //mPrintActionPreview = false;
                    pt.Print();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void bbiOptionalPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            { 

                DevExpress.XtraPrinting.PrintTool pt = new DevExpress.XtraPrinting.PrintTool(documentViewer1.PrintingSystem);
                
                if (mPrintCount >= MaxNoOfPrinting)
                {
                    mIsVoucherPrinted = false;
                    XtraMessageBox.Show("Number of prints allowed for the current document exceeds maximum no of prints allowed.", "CNET_V2016", MessageBoxButtons.OK,
                                       MessageBoxIcon.Information);
                    bbiDirectPrint.Enabled = false;
                    bbiOptionalPrint.Enabled = false;
                    bbiSaveDocument.Enabled = false;
                    bsiExport.Enabled = false;
                    return;
                }
                 
               
                bool? result = pt.PrintDialog();
                if (result.HasValue && result.Value)
                {
                    try
                    {
                        DateTime? serverTimeStamp = UIProcessManager.GetServiceTime();

                        if (DocumentPrintSetting.PrintActivityDefinitionDTO != null)
                        {


                            ActivityDTO activity = new ActivityDTO()
                            {
                                ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit,
                                TimeStamp = serverTimeStamp.Value,
                                Year = serverTimeStamp.Value.Year,
                                ActivityDefinition = DocumentPrintSetting.PrintActivityDefinitionDTO.Id,
                                Month = serverTimeStamp.Value.Month,
                                Day = serverTimeStamp.Value.Day,
                                Device = LocalBuffer.LocalBuffer.CurrentDevice.Id,
                                Pointer = CNETConstantes.VOUCHER_COMPONENET,
                                Platform = "1",
                                User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id
                            };
                            activity.Reference = voucherId;

                            if (UIProcessManager.CreateActivity(activity) != null)
                            {
                                if (mPrintCount >= MaxNoOfPrinting)
                                {
                                    mIsVoucherPrinted = false;
                                    XtraMessageBox.Show("Number of prints allowed for the current document exceeds maximum no of prints allowed.", "CNET_V2016", MessageBoxButtons.OK,
                                                       MessageBoxIcon.Information);
                                    bbiDirectPrint.Enabled = false;
                                    bbiOptionalPrint.Enabled = false;
                                    bbiSaveDocument.Enabled = false;
                                    bsiExport.Enabled = false;
                                    return;
                                }
                                mPrintCount++;
                            }
                        }
                        
                    }
                    catch {; }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET_V2016", MessageBoxButtons.OK,
                      MessageBoxIcon.Information);
            }
        }
    }
}