using System;
using System.Drawing;
using DevExpress.Office.Internal;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;
using DocumentPrint.DTO;

namespace DocumentPrint.Template
{
    public partial class RecieptReport
    {
        public RecieptReport(HeaderDTO header, bool waterMarkcustom )
        {
            InitializeComponent(header);
             

            if (waterMarkcustom)
            {
                subreport1.LocationF = new PointF(0, 150);
                subreport2.LocationF = new PointF(0, 350);
            }
            else
            {
                subreport1.LocationF = new PointF(0, 0);
                subreport2.LocationF = new PointF(0, 200);

            }
        }
    }
}
