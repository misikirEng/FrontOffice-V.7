using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using System.Linq;
using System.IO;
namespace DocumentPrint
{
    public partial class GuestLedgerRoomCharge : DevExpress.XtraReports.UI.XtraReport
    {
        public GuestLedgerRoomCharge()
        {
            InitializeComponent();


        }

        string CustomerName, RegistrationNumber, Plan, FSNO, ArrivalDate, DepartureDate, TINNo;
       
        string CompanyCity = "";
        string CompanyCountry = "";
        Stream CompanyLogoPic;
        byte[] logoPic;


        public GuestLedgerRoomCharge(IPrintable control1): this()
        {
            
            printableComponentContainer1.PrintableComponent = control1;
            printableComponentContainer1.CanGrow = false;

            if (DocumentPrintSetting.CompanyName != null)
            {
                this.lblCompanyName.Text = DocumentPrintSetting.CompanyName;
            }

            lblCompanyTinNo.Text = DocumentPrintSetting.CompanyConsigneeDTO.Tin;
            lblCompanyVatno.Text = DocumentPrintSetting.CompanyVATNumber;


            //if (DocumentPrintSetting.CompanyAttachmentLocation != null)
            //{
            //    FileStream file = null;
            //    try
            //    {
            //        if (File.Exists(DocumentPrintSetting.CompanyAttachmentLocation))
            //        {
            //            file = new FileStream(DocumentPrintSetting.CompanyAttachmentLocation, FileMode.Open, FileAccess.Read);
            //            if (file != null)
            //            {
            //                xrPictureBox1.Image = Image.FromFile(DocumentPrintSetting.CompanyAttachmentLocation);
            //                this.xrLabel8.Visible = false;
            //            }
            //        }
            //        else
            //        {

            //            xrLabel8.Text = DocumentPrintSetting.CompanyName;
            //            xrLabel8.Visible = true;
            //        }

            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            //else
            //{
            //    if (DocumentPrintSetting.CompanyName != null)
            //    {
            //        xrLabel8.Text = DocumentPrintSetting.CompanyName;
            //        xrLabel8.Visible = true;
            //    }
            //}

            if (DocumentPrintSetting.CompanyAttachmentLogo != null)
            { 
                try
                {
                    xrPictureBox1.Image = DocumentPrintSetting.CompanyAttachmentLogo;
                    this.xrLabel8.Visible = false;

                }
                catch (Exception ex)
                {
                    if (DocumentPrintSetting.CompanyName != null)
                    {
                        xrLabel8.Text = DocumentPrintSetting.CompanyName;
                        xrLabel8.Visible = true;
                    }
                }
            }
            else
            {
                if (DocumentPrintSetting.CompanyName != null)
                {
                    xrLabel8.Text = DocumentPrintSetting.CompanyName;
                    xrLabel8.Visible = true;
                }
            }

            this.lblPreparedBy.Text = LocalBuffer.LocalBuffer.CurrentLoggedInUserEmployeeName;
            this.lblEmail.Text = DocumentPrintSetting.CompanyConsigneeUnitDTO.Email;
            this.lblTelephone.Text = DocumentPrintSetting.CompanyConsigneeUnitDTO.Phone1;
            this.lblWeb.Text = DocumentPrintSetting.CompanyConsigneeUnitDTO.Website;
            this.lblPOBox.Text = DocumentPrintSetting.CompanyConsigneeUnitDTO.PoBox;
            //CompanyCity = objAddress.value;
            //CompanyCountry = objAddress.value;


            //string mtextToPrint = string.Empty;
            //bool hasCity = false;
            //if (!string.IsNullOrEmpty(CompanyCity))
            //{
            //    mtextToPrint = CompanyCity;
            //    hasCity = true;
            //}

            //if (!string.IsNullOrEmpty(CompanyCountry))
            //{
            //    if (hasCity)
            //        mtextToPrint += " ,";
            //    mtextToPrint += CompanyCountry;
            //}
            //this.lblCity.Text = mtextToPrint;
        }

        public void LedgerParameters(string Header, string CustomerName,string CompanyName, string CompanyTIN, string RegistrationNumber, string Plan, string FSNo, string TINNo, string ArrivalDate, string DepartureDate, int? ConsigneeUnit,string user)
        { 
            if (!string.IsNullOrEmpty(Header)) 
            {
                lblDocumentHeader.Text = Header;
            }

            this.lblTinno.Text = "  " + CompanyTIN;

            this.lblCustomerName.Text = "  " + CustomerName;
            //this.lblCustomerName.Text ="  "+ CustomerName;
            this.lblRegistrationNo.Text = "  " + RegistrationNumber;
            //this.lblPlan.Text = "  "+Plan;
            this.lblTinno.Text = "  "+TINNo;
            this.lblFSNo.Text = "  " + FSNo;
            this.lblDepartureDate.Text = "  " + DepartureDate;
            this.lblArrivalDate.Text = "  " + ArrivalDate;

            this.lblUserName.Text = user;
            this.lblGuestName.Text = CustomerName;
            this.lblCompany.Text = "  " + CompanyName;
            this.lblBranch.Text = DocumentPrintSetting.CompanyConsigneeUnitDTO.Name;
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
