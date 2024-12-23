using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using DevExpress.XtraEditors;
using System.IO; 
using System.Net.Mail;
using DevExpress.Utils.Controls; 
using DevExpress.XtraReports.UI; 
using DevExpress.Xpo.DB;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using System.Text;
using System.Diagnostics; 
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using DevExpress.XtraPrinting;

namespace DocumentPrint
{
    public partial class GridPrintDialog : DevExpress.XtraEditors.XtraForm
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

        public GridPrintDialog()
        {
            InitializeComponent();
        }
         

        public void PrintReport(DevExpress.XtraReports.UI.XtraReport report)
        { 
            documentViewer1.DocumentSource = null;
            report.CreateDocument();
            documentViewer1.DocumentSource = report;
        }



        private void bbiDirectPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET_V2016", MessageBoxButtons.OK,
                      MessageBoxIcon.Information);
            }
        }
    }
}