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
    public partial class BanquetPrintOut : DevExpress.XtraReports.UI.XtraReport
    {
        public BanquetPrintOut(List<BanquetPrintOutDTO> PrintOutData, bool Confirmation, DateTime TodayDate, string CompanyName, string ConsigneeName,
            DateTime EventDate, string EventType, DateTime ReservationStarttime, DateTime ReservationEndtime)
        {
            InitializeComponent();

            xlCompanyName.Text = CompanyName; 
            xlReservationdate.Text = EventDate.ToString("dddd, dd MMMM yyyy");
            xlReservationtype.Text = EventType;
            GroupField grpF = new GroupField("Department");
         //   GroupField grpF2 = new GroupField("Hall");
            GroupHeader1.GroupFields.Add(grpF);
           // GroupHeader1.GroupFields.Add(grpF2);
            DataSource = PrintOutData;
            this.Name = "Banquet Order";


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
    public class BanquetPrintOutDTO
    {
        public string Voucher { get; set; }
        public int? TypeCode { get; set; }
        public string TypeDesc { get; set; }
        public string Description { get; set; }
        public int SpaceCode { get; set; }
        public string Hall { get; set; }
        public string ArrangementDesc { get; set; }
        public int ArrangementCode { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int NumOfPerson { get; set; }
        public int SN { get; set; }
        public string VoucherCode { get; set; }
        public string ArticleCode { get; set; }
        public string ArticleName { get; set; }
        public decimal Qty { get; set; }
        public string Department { get; set; } 
        public int LineItemCode { get; set; } 
        public string LineItemNote { get; set; }
        public string VoucherNote { get; set; }
        
    }
   
}
