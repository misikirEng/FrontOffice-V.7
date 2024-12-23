using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.IO;

using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using CNET.ERP.Client.Common.UI;
namespace CNET.FrontOffice_V._7.HouseKeeping
{
    public partial class TaskAssignmentReport : DevExpress.XtraReports.UI.XtraReport
    {
        private string attendantName;
        private string date;
        private int rooms;
        private decimal credit;
        public TaskAssignmentReport()
        {
            InitializeComponent();
        }
        //List<Address> companyAddress;
        //List<Attachment> rAttachment = new List<Attachment>();
        Stream CompanyLogoPic;
        byte[] logoPic;
        public TaskAssignmentReport(IPrintable control1, string attendantName, string date, int rooms, decimal credit)
            : this()

        {
            this.attendantName = attendantName;
            this.date = date;
            this.rooms = rooms;
            this.credit = credit;
            printableComponentContainer1.PrintableComponent = control1;
            if (LocalBuffer.LocalBuffer.CompanyName != null)
            {
                this.xrLabel8.Text = LocalBuffer.LocalBuffer.CompanyName;
                lblCompanyName.Text = LocalBuffer.LocalBuffer.CompanyName;
            }

            xrLabel8.Text = LocalBuffer.LocalBuffer.CompanyName;
            xrLabel8.Visible = true;

            if (LocalBuffer.LocalBuffer.CompanyConsigneeData != null)
            {
                this.lblEmail.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.Email;
                this.lblTelephone.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.Phone1;
                this.lblTelephone.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.Phone2;
                this.lblWeb.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.Website;
                this.lblPOBox.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.PoBox;
                this.lblCity.Text = "Addis Ababa";
            }

            xrAttendantName.Text = attendantName;
            xrTaskDate.Text = date;
            xrTtlRms.Text = rooms.ToString();
            xrTtlcrdt.Text = credit.ToString();
            xrPreparedBy.Text = LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName;
        }

    }
}
