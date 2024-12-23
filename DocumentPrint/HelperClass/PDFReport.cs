using System.Drawing;
using DevExpress.XtraReports.UI;
using System.Drawing.Printing;

namespace DocumentPrint
{
    public partial class PDFReport : XtraReport
    {
        public PDFReport(PrintDocument document)
        {
            InitializeComponent();
            PrintController spc = document.PrintController;
            PreviewPrintController pc = new PreviewPrintController();
            document.PrintController = pc;
            document.Print();
            document.PrintController = spc;
            Margins = new Margins(0, 0, 0, 0);
            Bands.Clear();
            DetailBand b = new DetailBand();
            Bands.Add(b);
            PreviewPageInfo[] pages = pc.GetPreviewPageInfo();
            int top = 0;
            for (int i = 0; i < pages.Length; i++)
            {
                XRPictureBox pic = new XRPictureBox();
                pic.Image = pages[i].Image;
                pic.Sizing = DevExpress.XtraPrinting.ImageSizeMode.AutoSize;
                pic.Location = new Point(0, top);
                b.Controls.Add(pic);
                //XtraReport rpt = new XtraReport();
                //rpt.Landscape = true;
                top = pic.Bottom;
                XRPageBreak pgbrek = new XRPageBreak();
                pgbrek.Location = new Point(0, top);
                b.Controls.Add(pgbrek);
                top = pgbrek.Bottom;
            }

        }

    }
}
