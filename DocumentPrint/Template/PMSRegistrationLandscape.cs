using CNET_V7_Domain;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.CodeParser;
using DevExpress.XtraReports.UI;
using DocumentPrint.DTO;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace DocumentPrint.Grid
{
    public partial class PMSRegistrationLandscape : DevExpress.XtraReports.UI.XtraReport
    {
        public PMSRegistrationLandscape(RegistrationPrintOutDTO registrationPrintOut, Bitmap? Signature)
        {

            InitializeComponent();

            bool Exist = FTPInterface.FTPAttachment.InitalizePMSFTPAttachment(LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin);
            if (Exist && !string.IsNullOrEmpty(registrationPrintOut.GuestDefaultImageUrl))
            {
                picGuestImage.Image = FTPInterface.FTPAttachment.GetImageFromFTP(registrationPrintOut.GuestDefaultImageUrl);
               
            }

            if (DocumentPrintSetting.CompanyName != null)
            {
                this.lblCompanyName.Text = DocumentPrintSetting.CompanyName;
            }

            lblBranch.Text = DocumentPrintSetting.CompanyBranchName;
            lblCompanyTinNo.Text = DocumentPrintSetting.CompanyConsigneeDTO.Tin;

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
            this.lblEmail.Text = DocumentPrintSetting.CompanyEmail;
            this.lblTelephone.Text = DocumentPrintSetting.CompanyTel;
            this.lblWeb.Text = DocumentPrintSetting.CompanyWeb;
            this.lblPOBox.Text = DocumentPrintSetting.CompanyPOBox;


            if (string.IsNullOrEmpty(registrationPrintOut.CompanyName))
                lblCompany.Visible = false;

            if (string.IsNullOrEmpty(registrationPrintOut.AgentName))
                lblAgent.Visible = false;

            if (string.IsNullOrEmpty(registrationPrintOut.GroupName))
                lblGroup.Visible = false;

            if (string.IsNullOrEmpty(registrationPrintOut.SourceName))
                lblSource.Visible = false;

            if (registrationPrintOut.PaymentType == 1754)
                lblPaymentDirectBill.Text = "[X] Direct Bill";

            if (registrationPrintOut.PaymentType == 1748)
                lblPaymentCash.Text = "[X] Cash";

            if (Signature != null)
                SetSignature(Signature);

            objectDataSource1.DataSource = registrationPrintOut;
        }

        public void SetSignature(Bitmap signature)
        { 
            xpSignature.Visible = true;
            xpSignature.Image = signature;  
        }
    }
}
