using DevExpress.XtraReports.UI;
using DocumentPrint;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace ERP.EventManagement.PrintOut
{
    public partial class EventPrint : DevExpress.XtraReports.UI.XtraReport
    {
        public EventPrint(EventPrintDTO PrintOutData)
        {
            InitializeComponent();
            xlDate.Text = DateTime.Now.ToShortDateString();
            objectDataSource1.DataSource = PrintOutData;


            #region logo 
            if (LocalBuffer.LocalBuffer.CompanyDefaultLogo != null)
            {
                try
                {
                    CompanyLogo.Image = LocalBuffer.LocalBuffer.CompanyDefaultLogo;
                    //this.xlCompanyName.Visible = false;
                    if (LocalBuffer.LocalBuffer.CompanyName != null)
                    {
                        //xlCompanyName.Text = DocumentPrintSetting.CompanyName;
                        //xlCompanyName.Visible = true;
                    }

                }
                catch (Exception ex)
                {
                    if (LocalBuffer.LocalBuffer.CompanyName != null)
                    {
                        //xlCompanyName.Text = DocumentPrintSetting.CompanyName;
                        //xlCompanyName.Visible = true;
                    }
                }
            }
            else
            {
                if (LocalBuffer.LocalBuffer.CompanyName  != null)
                {
                    //xlCompanyName.Text = DocumentPrintSetting.CompanyName;
                    //xlCompanyName.Visible = true;
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

    public class EventPrintDTO
    {
        public string EventCode { get; set; }
        public string Eventdate { get; set; }
        public string Eventtime { get; set; }
        public string Eventtype { get; set; }
        public string Venue { get; set; }
        public string pax { get; set; }
        public string PaxRate { get; set; }
        public string HallRate { get; set; }
        public string PartyName { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string BookedBy { get; set; }
        public string Billing { get; set; }
        public string SeatArrangement { get; set; }
        public string Menu { get; set; }
        public string DepartmentInstruction { get; set; }
        public string SpecialInstruction { get; set; }
        public string ConferenceFacilities { get; set; }
        public string PaymentMethod { get; set; }

    }
}
