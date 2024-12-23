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
    public partial class HouseKeepingReport : DevExpress.XtraReports.UI.XtraReport
    {
        private string attendantName;
        public HouseKeepingReport()
        {
            InitializeComponent();
        } 

        Stream CompanyLogoPic;
        byte[] logoPic;
        string ttlRms;
        string ttlCredit;
        string reportName;
        bool isTask = false;
        public HouseKeepingReport(IPrintable control1,bool isTask,string ttlRms="",string ttlCredit = "",string reportName = "") : this() 
        {

            this.isTask = isTask;
            this.ttlRms = ttlRms;
            this.ttlCredit = ttlCredit;
            this.reportName = reportName;
            printableComponentContainer1.PrintableComponent = control1;

            //Organization rOrganization = UIProcessManager.GetOrganizationByGSL(CNETConstantes.COMPANY).FirstOrDefault();
            //if (rOrganization != null)
            //{
                this.xrLabel8.Text = LocalBuffer.LocalBuffer.CompanyName;
                lblCompanyName.Text = LocalBuffer.LocalBuffer.CompanyName;
                //rAttachment = UIProcessManager.GetAttachmentByReference(rOrganization.code);
            //}
        
            //if (rAttachment != null)
            //{
            //    foreach (var ab in rAttachment)
            //    {
            //        if (ab.description.ToLower().Contains("logo"))
            //        {
            //            if (ab.file != null)
            //            {

            //                CompanyLogoPic = new MemoryStream(ab.file);
            //                this.xrPictureBox1.Image = Image.FromStream(CompanyLogoPic);
            //                this.xrLabel8.Visible = false;
            //            }
            //            else
            //            {
            //                FileStream file = null;
            //                try
            //                {
            //                    if (File.Exists(ab.url))
            //                    {
            //                        file = new FileStream(ab.url, FileMode.Open, FileAccess.Read);
            //                        if (file != null)
            //                        {
            //                            xrPictureBox1.Image = Image.FromFile(ab.url);
            //                            this.xrLabel8.Visible = false;
            //                        }
            //                    }
            //                    else
            //                    {

            //                        xrLabel8.Text = rOrganization.brandName;
            //                        xrLabel8.Visible = true;
            //                    }

            //                }
            //                catch (Exception ex)
            //                {

            //                }


            //            }

            //        }
            //    }
            //}
            //else
            //{
                xrLabel8.Text = LocalBuffer.LocalBuffer.CompanyName;
                xrLabel8.Visible = true;
           // }

            //string deviceName = System.Windows.Forms.SystemInformation.ComputerName;
            //string devicecode = "";
            //this.lblCity.Text = "";
            //string articlecode = UIProcessManager.getArticleCode(deviceName);
            //if (!string.IsNullOrEmpty(articlecode))
            //{
            //    devicecode = UIProcessManager.getDeviceByArticle(articlecode);
            //}
            //var cmpnyOUD = UIProcessManager.GetOrganizationUnit(devicecode).FirstOrDefault();
            //try
            //{
            //    if (!string.IsNullOrEmpty(cmpnyOUD.organizationUnitDefinition))
            //    {
            //        companyAddress = UIProcessManager.GetAddress(cmpnyOUD.organizationUnitDefinition);
            //    }
            //    if (companyAddress.Count != 0)
            //    {
            //        foreach (Address objAddress in companyAddress)
            //        {
            //            switch (objAddress.preference)
            //            {
            //                case CNETConstantes.Email:
            //                    this.lblEmail.Text = objAddress.value;
            //                    break;
            //                case CNETConstantes.Mobilephone:
            //                    this.lblTelephone.Text = objAddress.value;
            //                    break;
            //                case CNETConstantes.TELE_PHONE:

            //                    this.lblTelephone.Text = objAddress.value;
            //                    break;
            //                case CNETConstantes.Fax:

            //                    this.lblFax.Text = objAddress.value;
            //                    break;
            //                case CNETConstantes.Website:

            //                    this.lblWeb.Text = objAddress.value;
            //                    break;
            //                case CNETConstantes.POBox:

            //                    this.lblPOBox.Text = objAddress.value;
            //                    break;
            //                case CNETConstantes.CityProvince:
            //                    this.lblCity.Text += objAddress.value;
            //                    break;
            //                case CNETConstantes.COUNTRY_PREFERENCE_CODE:
            //                    this.lblCity.Text += objAddress.value;
            //                    break;

            //            }
            //        }

            //    }
            //}
            //catch (Exception exx) { }

            //int assignedBy = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
            //vw_VoucherUserNameDetailView a = UIProcessManager.GetUserDetailViewByUserCode(assignedBy);

            //string firstName = "";
            //string lastname = "";
            //if (a != null) {
            //    if (a.firstName != null)
            //        firstName = a.firstName;
            //    if (a.lastName != null)
            //        lastname = a.lastName;
            //}

            xrPreparedBy.Text = LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName;// firstName + " " + lastname;
            if (isTask)
            {
                ttlRmsNum.Text = ttlRms.ToString();
                ttlCrdtNum.Text = ttlCredit.ToString();
            }
            else if (!isTask) {
                ttlRmsNum.Visible = false;
                ttlCrdtNum.Visible = false;
                ttlRooms.Visible = false;
                xrTtlCrdt.Visible = false;
            }
            xrLabel9.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
            xrReportName.Text = reportName;
        }

    }
}
