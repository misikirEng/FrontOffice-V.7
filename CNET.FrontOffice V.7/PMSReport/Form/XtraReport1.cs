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
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.CommonSchema;

namespace PMSReport
{
    public partial class XtraReport1 : XtraReport
    {
        public XtraReport1()
        {
            InitializeComponent();
        }
      

        public XtraReport1(IPrintable control1, IPrintable control2, string ReportName, string Datetime, string Branch, bool printByPortrait = false, DateTime? dateEnd = null)
            : this()
        {
            UserDTO currentUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;
            DateTime? serverDate = UIProcessManager.GetServiceTime();
            string mTextToPrint = String.Format("Prepared By : {0} On {1}", currentUser.UserName, serverDate);

            System.Drawing.Font mFont = new System.Drawing.Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);
            //float mVerticalPositoion = 25;
            //float mHoriZontalPosition = 20;
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

            //lblBranch.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter; 
            if (dateEnd != null)
            {
                this.lblPrfeparedDate.Text = String.Format("From  {0}  to  {1}.", Datetime, dateEnd.Value.Date.ToShortDateString());
            }
            else
            {
                this.lblPrfeparedDate.Text = "At the Day of " + Datetime + ".";
            }
            this.LblPrintDate.Text = mTextToPrint;
            if (printByPortrait)
            {
               
                    //m = TextRenderer.MeasureText(LocalBuffer.LocalBuffer.CompanyName, mFont);
                    //mHoriZontalPosition = ((XtraReport1.DefaultPageSize.Width - 75) - m.Width) / 2;
                    //this.CompanyLbl.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                    this.CompanyLbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                    this.CompanyLbl.Text = LocalBuffer.LocalBuffer.CompanyName;
                    this.CompanyLbl.Visible = true;
                    GroupHeader1.Band.Controls.Add(CompanyLbl);
                    //mVerticalPositoion += this.CompanyLbl.Font.Height + 5;
                

     

               // m = TextRenderer.MeasureText(Branch, mFont);
                //mHoriZontalPosition = ((XtraReport1.DefaultPageSize.Width - 75) - m.Width) / 2;
                //this.lblBranch.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                this.lblBranch.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                this.lblBranch.Text = Branch;
                this.lblBranch.Visible = true;
               // GroupHeader1.Band.Controls.Add(lblBranch);
                //mVerticalPositoion += this.lblBranch.Font.Height + 5;

                m = TextRenderer.MeasureText(ReportName, mFont);
                //mHoriZontalPosition = ((XtraReport1.DefaultPageSize.Width - 75) - m.Width) / 2;
                //this.lblReportName.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                this.lblReportName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                this.lblReportName.Text = ReportName;
                this.lblReportName.Visible = true;
               // GroupHeader1.Band.Controls.Add(lblReportName);
                //mVerticalPositoion += this.CompanyLbl.Font.Height + 5;


                mFont = new System.Drawing.Font("Tahoma", 12, FontStyle.Bold, GraphicsUnit.Pixel);

                DateTime iDate = Convert.ToDateTime(Datetime);
                //m = TextRenderer.MeasureText(this.lblPrfeparedDate.Text, mFont);
                //mHoriZontalPosition = (XtraReport1.DefaultPageSize.Width - 100) - m.Width;
                //this.lblPrfeparedDate.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                //this.lblPrfeparedDate.AutoWidth = true;
                //this.lblPrfeparedDate.Text = "At the Day of " + iDate.ToShortDateString();
                this.lblPrfeparedDate.Visible = true;
              //  GroupHeader1.Band.Controls.Add(lblPrfeparedDate);
                //mVerticalPositoion += this.CompanyLbl.Font.Height + 5;

                //mVerticalPositoion += mFont.Height * 2;

               // m = TextRenderer.MeasureText(mTextToPrint, mFont);
                //mHoriZontalPosition = (XtraReport1.DefaultPageSize.Width - 100) - m.Width;
                this.LblPrintDate.Text = mTextToPrint;
                if (control2 != null)
                {
                    //mVerticalPositoion = printableComponentContainer2.LocationF.Y + printableComponentContainer2.HeightF + 20;
                }
                //this.LblPrintDate.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                this.LblPrintDate.AutoWidth = true;

                this.LblPrintDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
                this.lnFooterLine.SizeF = new SizeF(765, 13);
            }
            else
            {
                    //m = TextRenderer.MeasureText(LocalBuffer.LocalBuffer.CompanyName, mFont);
                    //mHoriZontalPosition = ((XtraReport1.DefaultPageSize.Height - 14) - m.Width) / 2;
                    //this.CompanyLbl.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                    this.CompanyLbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                    this.CompanyLbl.Text = LocalBuffer.LocalBuffer.CompanyName;
                    this.CompanyLbl.Visible = true;
                    ///GroupHeader1.Band.Controls.Add(CompanyLbl);
                    //mVerticalPositoion += this.CompanyLbl.Font.Height + 5;

                
                if (ReportName == "Police Report")
                {
                    xrLabel1.Visible = true;
                    xrLabel2.Visible = true;
                    xrLine1.Visible = true;
                    xrLine2.Visible = true;
                    xrLabelNotice.Visible = true;
                    xrLineNotice.Visible = true;
                    xrLblNB.Visible = true;
                    xrLblPOBOX.Visible = true;
                    xrLblTEL.Visible = true;
                    xrLblEMAIL.Visible = true;
                    xrLblWEB.Visible = true;
                    xrlblCITY.Visible = true;

                    populateCompanyAddress();
                }
                m = TextRenderer.MeasureText(Branch, mFont);
                //mHoriZontalPosition = ((XtraReport1.DefaultPageSize.Height - 14) - m.Width) / 2;
                //this.lblBranch.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                this.lblBranch.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                this.lblBranch.Text = Branch;
                this.lblBranch.Visible = true;
               // GroupHeader1.Band.Controls.Add(lblBranch);
                //mVerticalPositoion += this.lblBranch.Font.Height + 5;

                m = TextRenderer.MeasureText(ReportName, mFont);
                //mHoriZontalPosition = ((XtraReport1.DefaultPageSize.Height - 14) - m.Width) / 2;
                //this.lblReportName.LocationF = new PointF(mHoriZontalPosition, mVerticalPositoion);
                this.lblReportName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                this.lblReportName.Text = ReportName;
                this.lblReportName.Visible = true;
              //  GroupHeader1.Band.Controls.Add(lblReportName);
                //mVerticalPositoion += this.CompanyLbl.Font.Height + 5;

            }

        }



        private void populateCompanyAddress()
        {
            string deviceName = System.Windows.Forms.SystemInformation.ComputerName;
            string devicecode = ""; 
            string companyCity = "";
            string companyCountry = "";


            if (LocalBuffer.LocalBuffer.CompanyConsigneeData != null)
            {
                this.xrLblEMAIL.Text = string.Format("Email: {0}", LocalBuffer.LocalBuffer.CompanyConsigneeData.Email);
                this.xrLblTEL.Text = string.Format("Telephone: {0}", LocalBuffer.LocalBuffer.CompanyConsigneeData.Phone1);
                this.xrLblTEL.Text = string.Format("Telephone: {0}", LocalBuffer.LocalBuffer.CompanyConsigneeData.Phone2);
                this.xrLblWEB.Text = string.Format("Web: {0}", LocalBuffer.LocalBuffer.CompanyConsigneeData.Website);
                this.xrLblPOBOX.Text = string.Format("P.O.Box: {0}", LocalBuffer.LocalBuffer.CompanyConsigneeData.PoBox);

                if (LocalBuffer.LocalBuffer.CompanyConsigneeData.City != null)
                {
                    SubCountryDTO subCountry = UIProcessManager.GetSubCountryById(LocalBuffer.LocalBuffer.CompanyConsigneeData.City.Value);
                    if (subCountry != null)
                        companyCity = subCountry.Name;
                }
                if (LocalBuffer.LocalBuffer.CompanyConsigneeData.City != null)
                {
                    CountryDTO Country = UIProcessManager.GetCountryById(LocalBuffer.LocalBuffer.CompanyConsigneeData.City.Value);
                    if (Country != null)
                        companyCountry = Country.Name;

                }
                 

                string mtextToPrint = string.Empty;
                bool hasCity = false;
                if (!string.IsNullOrEmpty(companyCity))
                {
                    mtextToPrint = companyCity;
                    hasCity = true;
                }
                if (!string.IsNullOrEmpty(companyCountry))
                {
                    if (hasCity)
                        mtextToPrint += " ,";
                    mtextToPrint += companyCountry;
                }
                this.xrlblCITY.Text = string.Format("{0}", mtextToPrint);

            }
            
        }
    }
}
