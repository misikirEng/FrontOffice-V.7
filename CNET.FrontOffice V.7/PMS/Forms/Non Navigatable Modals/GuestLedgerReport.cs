using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

using CNET.ERP.Client.Common.UI;
using System.Linq;
using System.IO;
namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class GuestLedgerReport : DevExpress.XtraReports.UI.XtraReport
    {
        public GuestLedgerReport()
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
        List<Address> companyAddress;
        List<Attachment> rAttachment = new List<Attachment>();
        Stream CompanyLogoPic;
        byte[] logoPic;
        public GuestLedgerReport(IPrintable control1, IPrintable control2, IPrintable control3)
            : this()
        {
            printableComponentContainer1.PrintableComponent = control1;
            printableComponentContainer1.CanGrow = false;
            printableComponentContainer2.PrintableComponent = control2;
            printableComponentContainer3.PrintableComponent = control3;
            Organization rOrganization = UIProcessManager.GetOrganizationByGSL(CNETConstantes.COMPANY).FirstOrDefault();
            if (rOrganization != null)
            {
                this.lblCompanyName.Text = rOrganization.tradeName;
                rAttachment = UIProcessManager.GetAttachmentByReference(rOrganization.code);
            }
            List<Identification> rIdentification = new List<Identification>();
            rIdentification = UIProcessManager.GetIdentificationByReference(rOrganization.code).ToList();
            foreach (Identification objIdentication in rIdentification)
            {
                switch (objIdentication.type)
                {
                    case CNETConstantes.ORGANIZATION_IDENTIFICATION_TYPETIN:
                        lblCompanyTinNo.Text = objIdentication.idNumber;
                        break;
                    case CNETConstantes.ORGANIZATION_IDENTIFICATION_TYPEVAT:
                        lblCompanyVatno.Text = objIdentication.idNumber;
                        break;
                }
            }

            if (rAttachment != null)
            {
                foreach (var ab in rAttachment)
                {
                    if (ab.description.ToLower().Contains("logo"))
                    {
                        if (ab.file != null)
                        {

                            CompanyLogoPic = new MemoryStream(ab.file);
                            this.xrPictureBox1.Image = Image.FromStream(CompanyLogoPic);
                            this.xrLabel8.Visible = false;
                        }
                        else
                        {
                            FileStream file = null;
                            try
                            {
                                if (File.Exists(ab.url))
                                {
                                    file = new FileStream(ab.url, FileMode.Open, FileAccess.Read);
                                    if (file != null)
                                    {
                                        xrPictureBox1.Image = Image.FromFile(ab.url);
                                        this.xrLabel8.Visible = false;
                                    }
                                }
                                else
                                {

                                    xrLabel8.Text = rOrganization.brandName;
                                    xrLabel8.Visible = true;
                                }

                            }
                            catch (Exception ex)
                            {

                            }


                        }

                    }
                }
            }
            else
            {
                xrLabel8.Text = rOrganization.brandName;
                xrLabel8.Visible = true;
            }


            string deviceName = System.Windows.Forms.SystemInformation.ComputerName;
            string devicecode = "";
            this.lblCity.Text = "";
            string articlecode = UIProcessManager.getArticleCode(deviceName);
            if (!string.IsNullOrEmpty(articlecode))
            {
                devicecode = UIProcessManager.getDeviceByArticle(articlecode);
            }
            var cmpnyOUD = UIProcessManager.GetOrganizationUnit(devicecode).FirstOrDefault();
            if (!string.IsNullOrEmpty(cmpnyOUD.organizationUnitDefinition))
            {
                companyAddress = UIProcessManager.GetAddress(cmpnyOUD.organizationUnitDefinition);
            }
            if (companyAddress.Count != 0)
            {
                foreach (Address objAddress in companyAddress)
                {
                    switch (objAddress.preference)
                    {
                        case CNETConstantes.Email:
                            this.lblEmail.Text = objAddress.value;
                            break;
                        case CNETConstantes.Mobilephone:
                            this.lblTelephone.Text = objAddress.value;
                            break;
                        case CNETConstantes.TELE_PHONE:

                            this.lblTelephone.Text = objAddress.value;
                            break;
                        case CNETConstantes.Fax:

                            this.lblFax.Text = objAddress.value;
                            break;
                        //case CNETConstantes.Website:

                        //    this.lblWeb.Text = objAddress.value;
                        //    break;
                        case CNETConstantes.POBox:

                            this.lblPOBox.Text = objAddress.value;
                            break;
                        case CNETConstantes.CityProvince:
                            this.lblCity.Text += objAddress.value;
                            break;
                        case CNETConstantes.COUNTRY_PREFERENCE_CODE:
                            this.lblCity.Text += objAddress.value;
                            break;

                    }
                }

            }

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
        public void ExtraBillsHistoryParameters(string TotalOtherBill)
        {
            decimal totlOterbil = 0;
            if (!string.IsNullOrEmpty(TotalOtherBill)) totlOterbil = Convert.ToDecimal(TotalOtherBill);

            this.lblTotalOtherBill.Text = string.Format("{0:N}", totlOterbil);
        }
        public void PaymentHistoryParameters(string TotalPaid, string TotalCredit, string Refund, string RemainingBalance)
        {
            decimal TotPaid = Convert.ToDecimal(TotalPaid);
            decimal TotCrdt = Convert.ToDecimal(TotalCredit);
            decimal Rfnd = Convert.ToDecimal(Refund);
            this.lblTotalPaid.Text = string.Format("{0:N}", TotPaid);
            this.lblTotalCredit.Text = string.Format("{0:N}", TotCrdt);
            this.lblRefund.Text = string.Format("{0:N}", Rfnd) + " ";
            this.lblRemainingBalance.Text = RemainingBalance;

        }


    }
}
