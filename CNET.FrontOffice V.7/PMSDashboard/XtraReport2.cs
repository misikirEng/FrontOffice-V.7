using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;
using DevExpress.XtraPrinting;
using  CNET.ERP.Client.Common.UI;
using System.Windows.Forms;
using CNET.FrontOffice_V._7;
using CNET_V7_Domain.Domain.SecuritySchema;
using ProcessManager;

namespace PMS_Dashboard
{
    public partial class XtraReport2 : XtraReport
    {
        private DevExpress.XtraGrid.GridControl gridControl;
        private string p1;
        private string p2;

        public XtraReport2()
        {
            InitializeComponent();
        }
        public XtraReport2(IPrintable control1, IPrintable control2, IPrintable control3, IPrintable control4, IPrintable control5, string ReportName, string Datetime)
            : this()
        {
            UserDTO currentUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;
            DateTime? serverDate = UIProcessManager.GetServiceTime();
            string mTextToPrint = String.Format("Prepared By : {0} On {1}", currentUser.UserName, serverDate.Value);

            System.Drawing.Font mFont = new System.Drawing.Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);
            float mVerticalPositoion = 25;
            float mHoriZontalPosition = 20;
            Size m = new Size();
            DevExpress.XtraGrid.GridControl ab = new DevExpress.XtraGrid.GridControl();
            printableComponentContainer1.PrintableComponent = control1;
            if (control2 != null)
            {
                printableComponentContainer2.Visible = true;
                printableComponentContainer2.PrintableComponent = control2;
            }
            else
            {
                printableComponentContainer2.Visible = false;
            }
           
            if (control5 != null)
            {
                printableComponentContainer5.Visible = true;
                printableComponentContainer5.PrintableComponent = control5;
            }
            else
            {
                printableComponentContainer5.Visible = false;
            }
             
            if (control3 != null)
            {
                printableComponentContainer3.Visible = true;
                printableComponentContainer3.PrintableComponent = control3;
                
            }
            else
            {
                printableComponentContainer3.Visible = false;
            }
            if (control4 != null)
            {
                printableComponentContainer4.Visible = true;
                printableComponentContainer4.PrintableComponent = control4;
            }
            else
            {
                printableComponentContainer4.Visible = false;
            }

             
            this.lblPrfeparedDate.Text = "At the Day of " + Datetime + ".";
             

            if (LocalBuffer.LocalBuffer.CompanyName != null)
            {
                m = TextRenderer.MeasureText(LocalBuffer.LocalBuffer.CompanyName, mFont);
                mHoriZontalPosition = ((XtraReport2.DefaultPageSize.Height - 14) - m.Width) / 2;
                this.CompanyLbl.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                this.CompanyLbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                this.CompanyLbl.Text = LocalBuffer.LocalBuffer.CompanyName;
                this.CompanyLbl.Visible = true;
                GroupHeader1.Band.Controls.Add(CompanyLbl);
                mVerticalPositoion += this.CompanyLbl.Font.Height + 5;

            }
             
            m = TextRenderer.MeasureText(ReportName, mFont);
            mHoriZontalPosition = ((XtraReport2.DefaultPageSize.Height - 14) - m.Width) / 2;
            this.lblReportName.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
            this.lblReportName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblReportName.Text = ReportName;
            this.lblReportName.Visible = true;
            GroupHeader1.Band.Controls.Add(lblReportName);
            mVerticalPositoion += this.CompanyLbl.Font.Height + 5;



        }

    

        private void populateCompanyAddress()
        {
            //string deviceName = System.Windows.Forms.SystemInformation.ComputerName;
            //string devicecode = "";
            //string articlecode = LoginPage.Authentication.DeviceObject.article;
            //string companyCity = "";
            //string companyCountry = "";
            
            //if (!string.IsNullOrEmpty(articlecode))
            //{
            //    devicecode = LoginPage.Authentication.DeviceBufferList.Where(x => x.article == articlecode).FirstOrDefault().code;
            //    OrganizationUnit cmpnyOUD = LoginPage.Authentication.OrganizationUnitBufferList.Where(x => x.reference == devicecode).FirstOrDefault();
            //    if (!string.IsNullOrEmpty(cmpnyOUD.organizationUnitDefinition))
            //    {
            //        var companyAddress = LoginPage.Authentication.AddressBufferList.Where(x => x.reference == cmpnyOUD.organizationUnitDefinition).ToList();

            //        if (companyAddress != null && companyAddress.Count != 0)
            //        {
            //            foreach (Address objAddress in companyAddress)
            //            {
            //                switch (objAddress.preference)
            //                {
            //                    case CNETConstantes.Email:
            //                        this.xrLblEMAIL.Text = string.Format("Email: {0}", objAddress.value);
            //                        break;
            //                    case CNETConstantes.Mobilephone:
            //                        this.xrLblTEL.Text = string.Format("Telephone: {0}", objAddress.value);
            //                        break;
            //                    case CNETConstantes.OfficePhone:

            //                        this.xrLblTEL.Text = string.Format("Telephone: {0}", objAddress.value);
            //                        break;
            //                    case CNETConstantes.Fax:

            //                        //this.lblFax.Text = objAddress.value;
            //                        break;
            //                    case CNETConstantes.Website:
            //                        this.xrLblWEB.Text = string.Format("Web: {0}", objAddress.value);
            //                        break;
            //                    case CNETConstantes.POBox:

            //                        this.xrLblPOBOX.Text = string.Format("P.O.Box: {0}", objAddress.value);
            //                        break;
            //                    case CNETConstantes.CityProvince:
            //                        companyCity = objAddress.value;
            //                        break;
            //                    case CNETConstantes.COUNTRY_PREFERENCE_CODE:
            //                        companyCountry = objAddress.value;
            //                        break;

            //                }
            //            }
            //            string mtextToPrint = string.Empty;
            //            bool hasCity = false;
            //            if (!string.IsNullOrEmpty(companyCity))
            //            {
            //                mtextToPrint = companyCity;
            //                hasCity = true;
            //            }
            //            if (!string.IsNullOrEmpty(companyCountry))
            //            {
            //                if (hasCity)
            //                    mtextToPrint += " ,";
            //                mtextToPrint += companyCountry;
            //            }
            //            this.xrlblCITY.Text = string.Format("{0}", mtextToPrint);
            //        }//end of if
            //    }
            //}
        }
    }
}
