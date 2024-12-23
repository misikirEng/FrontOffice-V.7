using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Linq;

using CNET.ERP.Client.Common.UI;
using System.IO;
using DocumentPrint;

namespace ERP.EventManagement
{
    public partial class EventProforma : DevExpress.XtraReports.UI.XtraReport
    {
        public EventProforma(List<PrintOutDTO> PrintOutData,bool Confirmation, DateTime TodayDate, string CompanyName, string ConsigneeName,
            DateTime EventDate, string EventType, DateTime ReservationStarttime, DateTime ReservationEndtime)
        {
            InitializeComponent();

            xlCompanyName.Text = CompanyName;
            xlCompany.Text = CompanyName;
            xlReservationdate.Text = EventDate.ToString("dddd, dd MMMM yyyy");
            xlReservationtype.Text = EventType;
            xlConsigneeName.Text = ConsigneeName;
            GroupField grpF = new GroupField("Code");
            GroupHeader1.GroupFields.Add(grpF);
            DataSource = PrintOutData;

            List<string> articlecodeList = PrintOutData.Where(x => !string.IsNullOrEmpty(x.ArticleCode)).Select(x => x.ArticleCode).ToList();
            if (articlecodeList == null || articlecodeList.Count == 0)
            {

                xrTableCell15.Visible = false;
                xtPackagelist.Visible = false;
                xrTable4.Visible = false;
                xrTable5.Visible = false;
                xrTable2.Visible = false;
            }


            if (Confirmation)
            {
                this.Name = "Reservation Confirmation";
                xlEventPrintoutType.Text = "Reservation Confirmation";
                xlDocumentExplain.Text = "Reference to your reservation request; this is to kindly inform you that our Hotel will be glad to " +
               " confirm your reservation on the following basis:-";
            }
            else
            {
                this.Name = "Reservation proforma";
                xlEventPrintoutType.Text = "Reservation proforma";
                xlDocumentExplain.Text = "Reference to your reservation request; this is to kindly inform you that our Hotel will be glad to " +
               " confirm your reservation on the following basis:-";
            }

            #region logo 
            if (DocumentPrintSetting.CompanyAttachmentLogo != null)
            {
                try
                {
                    CompanyLogo.Image = DocumentPrintSetting.CompanyAttachmentLogo;
                    //this.xlCompanyName.Visible = false;
                    if (DocumentPrintSetting.CompanyName != null)
                    {
                        xlCompanyName.Text = DocumentPrintSetting.CompanyName;
                        xlCompanyName.Visible = true;
                    }

                }
                catch (Exception ex)
                {
                    if (DocumentPrintSetting.CompanyName != null)
                    {
                        xlCompanyName.Text = DocumentPrintSetting.CompanyName;
                        xlCompanyName.Visible = true;
                    }
                }
            }
            else
            {
                if (DocumentPrintSetting.CompanyName != null)
                {
                    xlCompanyName.Text = DocumentPrintSetting.CompanyName;
                    xlCompanyName.Visible = true;
                }
            }
            #endregion 
        }
     
        public void PrintOption()
        {
            RequestParameters = false;
            CreateDocument();
            ReportPrintTool rptTool = new ReportPrintTool(this);
            rptTool.AutoShowParametersPanel = false;
            rptTool.ShowPreviewDialog();
        }
    }

    public class PrintOutDTO
    {
        public string Code { get; set; }
        public string Voucher { get; set; }
        public int? TypeCode { get; set; }
        public string TypeDesc { get; set; }
        public string Description { get; set; }
        public int SpaceCode { get; set; }
        public string Hall { get; set; }
        public string ArrangementDesc { get; set; }
        public int ArrangementCode { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int NumOfPerson { get; set; }
        public int SN { get; set; }
        public string VoucherCode { get; set; }
        public string ArticleCode { get; set; }
        public string ArticleName { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
