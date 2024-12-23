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
    public partial class BanquetPrint : DevExpress.XtraReports.UI.XtraReport
    {
        public BanquetPrint(List<BanquetDTO> PrintOutData)
        {
            InitializeComponent();

            xlCompanyName.Text = LocalBuffer.LocalBuffer.CompanyName; 
           // xlReservationdate.Text = EventDate.ToString("dddd, dd MMMM yyyy");
           // xlReservationtype.Text = EventType;
           // GroupField grpF = new GroupField("Department");
         //   GroupField grpF2 = new GroupField("Hall");
           // GroupHeader1.GroupFields.Add(grpF);
           // GroupHeader1.GroupFields.Add(grpF2);
            DataSource = PrintOutData;
            this.Name = "Banquet Order";


            #region logo 
            if (LocalBuffer.LocalBuffer.CompanyDefaultLogo != null)
            {
                try
                {
                    CompanyLogo.Image = LocalBuffer.LocalBuffer.CompanyDefaultLogo;
                    //this.xlCompanyName.Visible = false;
                    if (LocalBuffer.LocalBuffer.CompanyName != null)
                    {
                        xlCompanyName.Text = LocalBuffer.LocalBuffer.CompanyName;
                        xlCompanyName.Visible = true;
                    }

                }
                catch (Exception ex)
                {
                    if (LocalBuffer.LocalBuffer.CompanyDefaultLogo != null)
                    {
                        xlCompanyName.Text = LocalBuffer.LocalBuffer.CompanyName;
                        xlCompanyName.Visible = true;
                    }
                }
            }
            else
            {
                if (LocalBuffer.LocalBuffer.CompanyDefaultLogo != null)
                {
                    xlCompanyName.Text = LocalBuffer.LocalBuffer.CompanyName;
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
    public class BanquetDTO
    {
        public string RequirementCode { get; set; }
        public string PartyName { get; set; }
        public string CompanyName { get; set; }
        public string TIN { get; set; }
        public string Venue { get; set; }
        public string Function { get; set; }
        public string FunctionDate { get; set; }
        public string EventCode { get; set; }
        public string ExpectedPax { get; set; }
        public string GuarentedPax { get; set; }
        public string User { get; set; }
        public string ArticleName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }

    }
   
}
