using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

using CNET.ERP.Client.Common.UI;
using System.Linq;
using System.IO;
using CNET.FrontOffice_V._7.APICommunication;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.SettingSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class GuestLedgerRoomCharge : DevExpress.XtraReports.UI.XtraReport
    {
        public GuestLedgerRoomCharge()
        {
            InitializeComponent();


        }
        string CustomerName, RegistrationNumber, Plan, FSNO, ArrivalDate, DepartureDate, TINNo;
        string CompanyName = "";
        string CompanyTel = "";
        string CompanyFax = "";
        string CompanyWeb = "";
        string CompanyEmail = "";
        string CompanyPOBox = "";
        string CompanyAddress = "";
        string CompanyCity = "";
        string CompanyCountry = "";
        string CompanyMobile = "";
       // List<Address> companyAddress;
       // List<Attachment> rAttachment = new List<Attachment>();
        Stream CompanyLogoPic;
        byte[] logoPic;
        public GuestLedgerRoomCharge(IPrintable control1)
            : this()
        {
            printableComponentContainer1.PrintableComponent = control1;
            printableComponentContainer1.CanGrow = false;
            VwConsigneeViewDTO rOrganization = LocalBuffer.CompanyConsigneeData;
            if (rOrganization != null)
            {
                this.lblCompanyName.Text = rOrganization.FirstName;
                lblCompanyTinNo.Text = rOrganization.Tin;
                //  rAttachment = UIProcessManager.GetAttachmentByReference(rOrganization.code);
            }
            List<IdentificationDTO> rIdentification = new List<IdentificationDTO>();
            rIdentification = UIProcessManager.GetIdentificationByconsigneeandtype(rOrganization.Id, CNETConstantes.ORGANIZATION_IDENTIFICATION_TYPEVAT).ToList();
            if (rIdentification != null && rIdentification.Count > 0)
                lblCompanyVatno.Text = rIdentification.FirstOrDefault().IdNumber;

            if (rOrganization.ConsigneeImageUrl != null)
            {
                FileStream file = null;
                try
                {
                    if (File.Exists(rOrganization.ConsigneeImageUrl))
                    {
                        file = new FileStream(rOrganization.ConsigneeImageUrl, FileMode.Open, FileAccess.Read);
                        if (file != null)
                        {
                            xrPictureBox1.Image = Image.FromFile(rOrganization.ConsigneeImageUrl);
                            this.xrLabel8.Visible = false;
                        }
                    }
                    else
                    {

                        xrLabel8.Text = rOrganization.SecondName;
                        xrLabel8.Visible = true;
                    }

                }
                catch (Exception ex)
                {

                }

            }
            else
            {
                xrLabel8.Text = rOrganization.SecondName;
                xrLabel8.Visible = true;
            }

            this.lblEmail.Text = rOrganization.Email;
            this.lblTelephone.Text = rOrganization.Phone1;
            this.lblTelephone.Text = rOrganization.Phone2;
           // this.lblFax.Text = rOrganization.Fax;
            this.lblWeb.Text = rOrganization.Website;
            this.lblPOBox.Text = rOrganization.PoBox;
            this.lblCity.Text += rOrganization.City == null ? "" : LocalBuffer.SubCountryBufferlist.FirstOrDefault(x=> x.Id == rOrganization.City).Name ; 



        }

        public void LedgerParameters(string CustomerName,string CompanyName, string RegistrationNumber, string Plan, string FSNo, string TINNo, string ArrivalDate, string DepartureDate)
        {
            if (!string.IsNullOrEmpty(Plan))
            {
                List<OtherConsignee> OtherConsigneeList = UIProcessManager.GetOtherConsigneesListByVoucher(RegistrationNumber);
                if (OtherConsigneeList != null)
                {
                    foreach (OtherConsignee OC in OtherConsigneeList)
                    {
                        switch (OC.requiredGSL)
                        {
                            case CNETConstantes.REQ_GSL_COMPANY:
                                List<Identification> Identt = LocalBuffer.IdentificationBufferList.Where(i => i.reference == OC.consignee).ToList();
                                if (Identt != null)
                                {
                                    foreach (Identification Id in Identt)
                                    {
                                        switch (Id.type)
                                        {
                                            case CNETConstantes.ORGANIZATION_IDENTIFICATION_TYPETIN:
                                                this.lblTinno.Text = "  " + Id.idNumber;
                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                
            }
            this.lblCustomerName.Text = "  " + CustomerName;
            //this.lblCustomerName.Text ="  "+ CustomerName;
            this.lblRegistrationNo.Text = "  " + RegistrationNumber;
            //this.lblPlan.Text = "  "+Plan;
            //7 this.lblTinno.Text = "  "+TINNo;
            this.lblFSNo.Text = "  " + FSNo;
            this.lblDepartureDate.Text = "  " + DepartureDate;
            this.lblArrivalDate.Text = "  " + ArrivalDate;
            User currentUser = LocalBuffer.GetAuthorizedUser();
            this.lblUserName.Text = currentUser.userName;
            this.lblGuestName.Text = CustomerName;
            this.lblCompany.Text = "  " + CompanyName;

        }

        public void RoomChargeParameters(string SubTotal, string Discount, string ServiceCharge, string Vat, string Grandtotal)
        {

            decimal serChrg = 0;
            decimal vat = 0;
            decimal dscnt = 0;
            decimal GrndTotl = 0;
            if (!string.IsNullOrEmpty(ServiceCharge)) serChrg = Convert.ToDecimal(ServiceCharge);
            if (!string.IsNullOrEmpty(Vat)) vat = Convert.ToDecimal(Vat);
            if (!string.IsNullOrEmpty(Discount)) dscnt = Convert.ToDecimal(Discount);
            if (!string.IsNullOrEmpty(Grandtotal)) GrndTotl = Convert.ToDecimal(Grandtotal);
            this.lblSubTotal.Text = SubTotal;
            this.lblDiscount.Text = string.Format("{0:N}", dscnt);
            this.lblServiceCharge.Text = string.Format("{0:N}", serChrg); ;
            this.lblVat.Text = string.Format("{0:N}", vat);
            this.lblGrandTotal.Text = string.Format("{0:N}", GrndTotl);

        }
    }
}
