using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting; 
using System.Windows.Forms;

namespace DocumentPrint
{
    public partial class GridandChart : DevExpress.XtraReports.UI.XtraReport
    {
        public GridandChart()
        {
            InitializeComponent();
        }
        public GridandChart(IPrintable control1, IPrintable control2, string ReportName, DateTime Datetime) : this() 
        {
            System.Drawing.Font mFont = new System.Drawing.Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);
            float mVerticalPositoion = 25;
            float mHoriZontalPosition = 20;
            Size m = new Size();
            if (DocumentPrintSetting.CompanyName != null)
            {
                m = TextRenderer.MeasureText(DocumentPrintSetting.CompanyName, mFont);
                mHoriZontalPosition = ((GridPrint.DefaultPageSize.Height - m.Width)) / 2;
                this.CompanyLbl.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                this.CompanyLbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                this.CompanyLbl.Text = DocumentPrintSetting.CompanyName;
                this.CompanyLbl.Visible = true;
                GroupHeader1.Band.Controls.Add(CompanyLbl);
                mVerticalPositoion += this.CompanyLbl.Font.Height + 5;
            }
            if (!string.IsNullOrEmpty(ReportName)) 
            {
                m = TextRenderer.MeasureText(ReportName, mFont);
                mHoriZontalPosition = ((GridPrint.DefaultPageSize.Height) - m.Width) / 2;
                this.lblReportName.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                this.lblReportName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                this.lblReportName.Text = ReportName;
                this.lblReportName.Visible = true;
                GroupHeader1.Band.Controls.Add(lblReportName);
                mVerticalPositoion += this.CompanyLbl.Font.Height + 5;
            }
                mFont = new System.Drawing.Font("Tahoma", 12, FontStyle.Bold, GraphicsUnit.Pixel);
                DateTime iDate = Convert.ToDateTime(Datetime);
                m = TextRenderer.MeasureText("Date: " + iDate.ToShortDateString(), mFont);
                mHoriZontalPosition = (GridPrint.DefaultPageSize.Height) - m.Width;
                this.lblPrfeparedDate.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                this.lblPrfeparedDate.AutoWidth = true;
                this.lblPrfeparedDate.Text = "Date :  " + iDate.ToShortDateString();
                this.lblPrfeparedDate.Visible = true;
                GroupHeader1.Band.Controls.Add(lblPrfeparedDate);
                mVerticalPositoion += this.CompanyLbl.Font.Height + 5;
            
            printableComponentContainer1.PrintableComponent = control1;
            printableComponentContainer2.PrintableComponent = control2;
        }
    }
}
