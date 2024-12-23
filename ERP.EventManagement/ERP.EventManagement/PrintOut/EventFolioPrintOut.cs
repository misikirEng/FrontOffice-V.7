using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Linq;

using CNET.ERP.Client.Common.UI;
using System.IO;
using DevExpress.XtraPrinting;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Domain.PmsSchema;
using DocumentPrint;
using CNET_V7_Domain.Misc.PmsDTO;

namespace ERP.EventManagement
{
    public partial class EventFolioPrintOut : DevExpress.XtraReports.UI.XtraReport
    {
        public EventFolioPrintOut(EventFolioDTO EventFolioData, IPrintable control1, IPrintable control2, string CompanyName, List<EventRequirementView> EventRequirementdata, List<EventCRVData> CRVdata)
        {
            InitializeComponent();

            xlCompanyName.Text = CompanyName;

            txtEventOwner.Text = EventFolioData.EventOwner;
            txtEventOrganizer.Text = EventFolioData.EventOrganizer;
            txtEventSource.Text = EventFolioData.EventSource;
            txtEventContact.Text = EventFolioData.EventContact;
            txtEventCode.Text = EventFolioData.EventCode;
            txtEventIssuedDate.Text = EventFolioData.EventDate;
            txtEventStartDateTime.Text = EventFolioData.EventStartDate.Value.ToString("dddd, dd MMMM yyyy") ;
            txtEventEndDateTime.Text = EventFolioData.EventEndDate.Value.ToString("dddd, dd MMMM yyyy");



            txtSubtotal.Text = EventFolioData.EvRSubtotal.ToString("N2");
            txtDiscount.Text = EventFolioData.EvRDiscount == null? "0.00": EventFolioData.EvRDiscount.Value.ToString("N2");
            txtServiceCharge.Text = EventFolioData.EvRServiceCharge == null ? "0.00" :  EventFolioData.EvRServiceCharge.Value.ToString("N2");
            txtVat.Text = EventFolioData.EvRVAT == null ? "0.00" : EventFolioData.EvRVAT.Value.ToString("N2");
            txtGrandTotal.Text = EventFolioData.EvRGrandTotal == null ? "0.00" :  EventFolioData.EvRGrandTotal.Value.ToString("N2");



            txtEventReqTotal.Text = EventFolioData.EvRGrandTotal == null ? "0.00" : EventFolioData.EvRGrandTotal.Value.ToString("N2");
            //txtEventReqTotal.Text = EventFolioData.EvRGrandTotal.ToString("N2");
            txtCRVTotal.Text = EventFolioData.CRVGrandTotal.ToString("N2");
            txtRemainingBalance.Text = EventFolioData.DiffGrandTotal.ToString("N2");


            if (EventRequirementdata != null && EventRequirementdata.Count > 0)
                printableComponentContainer1.PrintableComponent = control1;
            else
                printableComponentContainer1.Visible = false;

            if (CRVdata != null && CRVdata.Count > 0)
                printableComponentContainer2.PrintableComponent = control2;
            else
                printableComponentContainer2.Visible = false;


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
     
}
