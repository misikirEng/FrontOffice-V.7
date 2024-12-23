using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;
using DevExpress.XtraPrinting;  
using System.Windows.Forms;
using CNET.FrontOffice_V._7;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.ConsigneeSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class PackagePrintout : DevExpress.XtraReports.UI.XtraReport
    {
        public PackagePrintout()
        {
            InitializeComponent();
        }

        string DateTimeOfReport = "";
        string NameOfReport = "";
        public PackagePrintout(IPrintable control1, string ReportName, string Datetime, RegistrationListVMDTO CurrentRegistration)
            : this()
        {
            System.Drawing.Font mFont = new System.Drawing.Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);


            Size m = new Size();
            printableComponentContainer1.PrintableComponent = control1;
            VwConsigneeViewDTO rOrganization = LocalBuffer.LocalBuffer.CompanyConsigneeData;
            CompanyLbl.Text = rOrganization.FirstName;
            lblReportName.Text = ReportName;
            lblPrfeparedDate.Text = "Date :  " + Datetime;
            xtRegistrationno.Text = CurrentRegistration.Registration;
            xtRoomNo.Text = CurrentRegistration.Room;
            xtArrivalDate.Text = CurrentRegistration.Arrival.ToLongDateString();
            xtCompanyName.Text = CurrentRegistration.Company;
            xtDepartureDate.Text = CurrentRegistration.Departure.ToLongDateString();
            xtGuestName.Text = CurrentRegistration.Guest;
            xtNightpax.Text = CurrentRegistration.NumOfNight + " , " + (CurrentRegistration.adult + CurrentRegistration.child).ToString();

            if (string.IsNullOrEmpty(CurrentRegistration.Company))
            {
                xtCompanyName.Visible = false;
                xtCompanyNamelbl.Visible = false;
            }

        }
    }
}
