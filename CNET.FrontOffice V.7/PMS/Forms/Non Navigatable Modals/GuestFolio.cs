using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

using CNET.ERP.Client.Common.UI;
using System.Linq;
using System.IO;
using CNET_V7_Domain.Domain.ViewSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.CommonSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class GuestFolio : DevExpress.XtraReports.UI.XtraReport
    {
        public GuestFolio()
        {
            InitializeComponent();
        } 
        Stream CompanyLogoPic;
        byte[] logoPic;
        public GuestFolio(IPrintable control1) : this()
        {
            printableComponentContainer1.PrintableComponent = control1;
            if (LocalBuffer.LocalBuffer.CompanyConsigneeData != null)
            {
                this.lblCompanyName.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.FirstName;
                lblCompanyTinNo.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin;
            }

            List<IdentificationDTO> rIdentification = new List<IdentificationDTO>();


            rIdentification = UIProcessManager.GetIdentificationByconsigneeandtype(LocalBuffer.LocalBuffer.CompanyConsigneeData.Id, CNETConstantes.ORGANIZATION_IDENTIFICATION_TYPEVAT).ToList();
            if (rIdentification != null && rIdentification.Count > 0)
                lblCompanyVatno.Text = rIdentification.FirstOrDefault().IdNumber;



            if (LocalBuffer.LocalBuffer.CompanyConsigneeData.ConsigneeImageUrl != null)
            {
                FileStream file = null;
                try
                {
                    if (File.Exists(LocalBuffer.LocalBuffer.CompanyConsigneeData.ConsigneeImageUrl))
                    {
                        file = new FileStream(LocalBuffer.LocalBuffer.CompanyConsigneeData.ConsigneeImageUrl, FileMode.Open, FileAccess.Read);
                        if (file != null)
                        {
                            xrPictureBox1.Image = Image.FromFile(LocalBuffer.LocalBuffer.CompanyConsigneeData.ConsigneeImageUrl);
                            this.xrLabel8.Visible = false;
                        }
                    }
                    else
                    {

                        xrLabel8.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.SecondName;
                        xrLabel8.Visible = true;
                    }

                }
                catch (Exception ex)
                {

                }

            }
            else
            {
                xrLabel8.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.SecondName;
                xrLabel8.Visible = true;
            }



            this.lblEmail.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.Email;
            this.lblTelephone.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.Phone1;
            this.lblTelephone.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.Phone2;
            // this.lblFax.Text = rOrganization.Fax;
            this.lblWeb.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.Website;
            this.lblPOBox.Text = LocalBuffer.LocalBuffer.CompanyConsigneeData.PoBox;
            if (LocalBuffer.LocalBuffer.CompanyConsigneeData.City != null)
            {
                SubCountryDTO subCountry = UIProcessManager.GetSubCountryById(LocalBuffer.LocalBuffer.CompanyConsigneeData.City.Value);
                this.lblCity.Text += subCountry.Name;
            }
        }

        public void FolioParametres(string CustomerName, string RegistrationNumber, string Plan, string FSNo, string TINNo, string ArrivalDate, string DepartureDate)
        {
            this.lblCustomerName.Text = "  " + CustomerName;
            this.lblRegistrationNo.Text = "  " + RegistrationNumber;
            this.lblPlan.Text = "  " + Plan;
            this.lblTinno.Text = "  " + TINNo;
            this.lblFSNo.Text = "  " + FSNo;
            this.lblDepartureDate.Text = "  " + DepartureDate;
            this.lblArrivalDate.Text = "  " + ArrivalDate; 
            this.lblUserName.Text = LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName;
            this.lblGuestName.Text = CustomerName;
        }

    }
}
