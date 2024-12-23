using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;
using DevExpress.XtraPrinting;
using System.Windows.Forms;

namespace DocumentPrint
{
    public partial class GridPrint : DevExpress.XtraReports.UI.XtraReport
    {
        public GridPrint()
        {
            InitializeComponent();
        }
        
        public GridPrint(IPrintable control1, string ReportName, string Datetime, string Peroid = "", string Account = "") : this()
        {
            System.Drawing.Font mFont = new System.Drawing.Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);
        
            Size m = new Size();
            printableComponentContainer1.PrintableComponent = control1;


            if (DocumentPrintSetting.CompanyName != null)
            {
                this.CompanyLbl.Text = DocumentPrintSetting.CompanyName;
                this.CompanyLbl.Visible = true;
            }
            this.lblReportName.Text = ReportName;
            DateTime iDate = Convert.ToDateTime(Datetime);
            this.lblPrfeparedDate.Text = "Date :  " + iDate.ToShortDateString();

            if (!string.IsNullOrEmpty(Peroid))
            {
                this.lblPeroid.AutoWidth = true;
                this.lblPeroid.Text = Peroid;
                this.lblPeroid.Visible = true;
            }

            if (!string.IsNullOrEmpty(Account))
            {
                this.lblAccount.AutoWidth = true;
                this.lblAccount.Text = Account;
                this.lblAccount.Visible = true;
            }

        }
    }
}
