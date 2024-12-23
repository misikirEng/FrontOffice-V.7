using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraGrid.Views.Grid; 
using ProcessManager;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Misc.PmsDTO;

namespace ERP.EventManagement.Modals
{
    public partial class EventFolio : DevExpress.XtraEditors.XtraForm
    {
        public int EventId { get; set; }
        public string EventCode { get; set; }
        public int EventObjectState { get; set; }

        public EventFolio()
        {
            InitializeComponent();
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PopulateEventFolioData();
        }

        private void EventFolio_Load(object sender, EventArgs e)
        {
            PopulateEventFolioData();

        }
        EventFolioDTO EventFolioData { get; set; }
        private void PopulateEventFolioData()
        {
            //Progress_Reporter.Show_Progress("Getting Event Folio data", "Please Wait...");
            EventFolioData = UIProcessManager.GetEventFolioData(EventId);
            if (EventFolioData != null)
            {
                txtEventOwner.EditValue = EventFolioData.EventOwner;
                txtEventOrganizer.EditValue = EventFolioData.EventOrganizer;
                txtEventSource.EditValue = EventFolioData.EventSource;
                txtEventContact.EditValue = EventFolioData.EventContact;
                txtEventCode.EditValue = EventFolioData.EventCode;
                txtEventIssuedDate.EditValue = EventFolioData.EventDate;
                txtEventStartDateTime.EditValue = EventFolioData.EventStartDate;
                txtEventEndDateTime.EditValue = EventFolioData.EventEndDate;



                txtSubtotal.EditValue = EventFolioData.EvRSubtotal.ToString("N2");
                txtDiscount.EditValue = EventFolioData.EvRDiscount.Value.ToString("N2");
                txtServiceCharge.EditValue = EventFolioData.EvRServiceCharge.Value.ToString("N2");
                txtVat.EditValue = EventFolioData.EvRVAT.Value.ToString("N2");
                txtGrandTotal.EditValue = EventFolioData.EvRGrandTotal.Value.ToString("N2");



                txtEventReqTotal.EditValue = EventFolioData.EvRGrandTotal.Value.ToString("N2");
                txtCRVTotal.EditValue = EventFolioData.CRVGrandTotal.ToString("N2");
                txtRemainingBalance.EditValue = EventFolioData.DiffGrandTotal.ToString("N2");


                gcCRVPayment.DataSource = EventFolioData.crvdatalist;
                gcEventRequirement.DataSource = EventFolioData.EventRequirementList;
                gvEventRequirement.ExpandAllGroups();
            }


            if (EventObjectState == CNETConstantes.OSD_EVENTCHECKEDOUT)
            {
                btnPrintBill.Enabled = false;
            }
            //Progress_Reporter.Close_Progress();
        }

        private void gvCRVPayment_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.Caption == "SN")
                e.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void btnFolioPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Progress_Reporter.Show_Progress("Showing Event Folio Printout", "Please Wait....");


            List<EventRequirementView> EventRequirementListdata = (List<EventRequirementView>)gcEventRequirement.DataSource;
            List<EventCRVData> CRVListdata = (List<EventCRVData>)gcCRVPayment.DataSource;

            if ((EventRequirementListdata == null || EventRequirementListdata.Count == 0) && (CRVListdata == null || CRVListdata.Count == 0))
            {
                XtraMessageBox.Show("There are no Event Requirement and Event CRV", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            EventFolioPrintOut EventFolioPrintOut = new EventFolioPrintOut(EventFolioData, gcEventRequirement, gcCRVPayment,LocalBuffer.LocalBuffer.CompanyName,
                EventRequirementListdata, CRVListdata);

            //Progress_Reporter.Close_Progress();
            EventFolioPrintOut.PrintOption();

        }

        private void btnPrintBill_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (EventObjectState == CNETConstantes.OSD_EVENTConducted)
            {
                if (EventFolioData.EventRequirementList != null && EventFolioData.EventRequirementList.Count > 0)
                {
                    frmFrontOfficePOS PrintBill = new frmFrontOfficePOS();
                    PrintBill.EventOwnerName = EventFolioData.EventOwner;
                    PrintBill.EventId = EventFolioData.EventId;
                    PrintBill.EventCode = EventFolioData.EventCode;
                    PrintBill.EventRequirementList = EventFolioData.EventRequirementList;
                    PrintBill.EventLineItemList = EventFolioData.EventLineItemList;
                    PrintBill.ShowDialog();
                    this.Close();
                }
                else
                    XtraMessageBox.Show("There are no Event Requirement to Print !", "CENTERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                XtraMessageBox.Show("The Event Status must be Conducted to Check Out !", "CENTERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}