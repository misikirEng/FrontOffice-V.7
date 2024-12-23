using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.Utils.About;
using DevExpress.XtraReports.UI;
using ProcessManager;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace DocumentPrint.Template
{
    public partial class PMSConformation : DevExpress.XtraReports.UI.XtraReport
    {
        public PMSConformation(RegistrationPrintOutDTO registrationPrintOut, string UserName, int Laststate)
        {
            InitializeComponent();

            #region parameters

            lblCompany.Text = DocumentPrintSetting.CompanyName;
            lblTIN.Text = DocumentPrintSetting.CompanyTIN;
            lblVAT.Text = DocumentPrintSetting.CompanyVATNumber;
            lblTell.Text = DocumentPrintSetting.CompanyTel;
            lblWeb.Text = DocumentPrintSetting.CompanyWeb;
            lblFax.Text = DocumentPrintSetting.CompanyFax;
            lblPobox.Text = DocumentPrintSetting.CompanyPOBox;
            lblEmail.Text = DocumentPrintSetting.CompanyEmail;
            lblReservationNo.Text = DocumentPrintSetting.CompanyContact;

            #endregion

            if (Laststate == CNETConstantes.GAURANTED_STATE)
                xrGuaranteed.Text = "Guaranteed";
            else
                xrGuaranteed.Text = "Non - Guaranteed";


            lblUserName.Text = UserName;
            lblUserName2.Text = UserName;
            lblCompanyName2.Text = DocumentPrintSetting.CompanyName;
            lblHotelBranch.Text = DocumentPrintSetting.CompanyBranchName;
            lblDatetime.Text = DateTime.Now.ToString("dd-MMM-yyyy");

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
            //                this.lblCompanyName.Visible = false;
            //            }
            //        }
            //        else
            //        {
            //            lblCompanyName.Text = DocumentPrintSetting.CompanyName;
            //            lblCompanyName.Visible = true;
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
            //        lblCompanyName.Text = DocumentPrintSetting.CompanyName;
            //        lblCompanyName.Visible = true;
            //    }
            //}

            if (DocumentPrintSetting.CompanyAttachmentLogo != null)
            { 
                try
                {
                    xrPictureBox1.Image = DocumentPrintSetting.CompanyAttachmentLogo;
                    this.lblCompanyName.Visible = false;
                }
                catch (Exception ex)
                {
                    if (DocumentPrintSetting.CompanyName != null)
                    {
                        lblCompanyName.Text = DocumentPrintSetting.CompanyName;
                        lblCompanyName.Visible = true;
                    }
                }
            }
            else
            {
                if (DocumentPrintSetting.CompanyName != null)
                {
                    lblCompanyName.Text = DocumentPrintSetting.CompanyName;
                    lblCompanyName.Visible = true;
                }
            }

            lblMessage1.Text = "Thank you very much for your interest in the " + DocumentPrintSetting.CompanyName + ". We are pleased to confirm your reservation as follows: ";
            lblMessage2.Text = "Our team looks forward to welcoming you at  " + DocumentPrintSetting.CompanyName + " and trust you will have an enjoyable stay with us. Should you require any further information, Please don't hesitate to contact our reservation team.";

            if (registrationPrintOut.RatehasServiceCharge && registrationPrintOut.RatehasVAT)
                lblRateInfo.Text = "NB.The Above Rate is Subjected to Service Charge and VAT";
            else if (!registrationPrintOut.RatehasServiceCharge && registrationPrintOut.RatehasVAT)
                lblRateInfo.Text = "NB.The Above Rate is Subjected to VAT";
            else if (registrationPrintOut.RatehasServiceCharge && !registrationPrintOut.RatehasVAT)
                lblRateInfo.Text = "NB.The Above Rate is Subjected to Service Charge";
            else if (registrationPrintOut.RatehasServiceCharge && !registrationPrintOut.RatehasVAT)
                lblRateInfo.Text = string.Empty;

            //companyName.Value = DocumentPrintSetting.CompanyName;
            objectDataSource1.DataSource = registrationPrintOut;
        }
    }
}
