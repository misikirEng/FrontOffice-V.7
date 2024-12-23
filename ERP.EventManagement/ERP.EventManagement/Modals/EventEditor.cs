using CNET.ERP.Client.Common.UI;
using CNET.POS.Common.Models;
using CNET_V7_Domain;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.ThirdParty;
using DevExpress.Diagram.Core.Shapes;
using DevExpress.Mvvm.POCO;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Pkcs;
using DevExpress.Utils.Animation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.Design.ParameterEditor;
using ERP.EventManagement;
using ERP.EventManagement.DTO;
using ERP.EventManagement.PrintOut;
using ProcessManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace ERP.EventManagement.Modals
{
    public partial class EventEditor : Form
    {

        private int? _adPrepared;
        private int? _adEdited;

        private int? _defEventCateg;
        private int? _defEventType;
        private int? _defBookingType;
        private int? _defObjState;

        private string _currentEventHeaderCode { get; set; }
        private int _currentEventHeaderId { get; set; }

        List<EventConsgineeDTO> consigneeDTOList { get; set; }

        private List<EventDetaildataDTO> _eventDetailDtoList = new List<EventDetaildataDTO>();

        public int? SelectedHotelcode { get; set; }

        public EventHeaderDTO EventDTO { get; set; }

        public DateTime CurrentTime { get; set; }

        VoucherBuffer EventVoucherBuffer { get; set; }

        VoucherFinalDTO VoucherFinal = new VoucherFinalDTO();
        List<LineItemBuffer> LineItemList { get; set; }

        List<LineItemDetails> lineItemDetails = new List<LineItemDetails>();


        public ArticleDTO? PaxArticle { get; set; }

        public ArticleDTO? HallArticle { get; set; }

        public bool EventHeaderAccess { get; set; }
        public bool EventDetailAccess { get; set; }
        public bool EventRequirementAccess { get; set; }


        public EventEditor()
        {
            InitializeComponent();

            InitializeUI();


        }

        #region Helper Methods

        private void InitializeUI()
        {
            //Event Owner
            GridColumn columnGuest = sluEventOwner.Properties.View.Columns.AddField("code");
            columnGuest.Visible = true;
            columnGuest = sluEventOwner.Properties.View.Columns.AddField("id");
            columnGuest.Visible = false;
            columnGuest = sluEventOwner.Properties.View.Columns.AddField("name");
            columnGuest.Visible = true;
            columnGuest = sluEventOwner.Properties.View.Columns.AddField("idNumber");
            columnGuest.Visible = true;
            sluEventOwner.Properties.DisplayMember = "name";
            sluEventOwner.Properties.ValueMember = "id";

            //company Owner
            GridColumn columncompany = sleCompany.Properties.View.Columns.AddField("code");
            columncompany.Visible = true;
            columncompany = sleCompany.Properties.View.Columns.AddField("id");
            columncompany.Visible = false;
            columncompany = sleCompany.Properties.View.Columns.AddField("name");
            columncompany.Visible = true;
            columncompany = sleCompany.Properties.View.Columns.AddField("idNumber");
            columncompany.Visible = true;
            sleCompany.Properties.DisplayMember = "name";
            sleCompany.Properties.ValueMember = "id";

            //Event Organizer
            GridColumn columnCompany = sluOrganizer.Properties.View.Columns.AddField("code");
            columnCompany.Visible = true;
            columnCompany = sluOrganizer.Properties.View.Columns.AddField("id");
            columnCompany.Visible = false;
            columnCompany = sluOrganizer.Properties.View.Columns.AddField("name");
            columnCompany.Visible = true;
            columnCompany = sluOrganizer.Properties.View.Columns.AddField("idNumber");
            columnCompany.Visible = true;
            sluOrganizer.Properties.DisplayMember = "name";
            sluOrganizer.Properties.ValueMember = "id";

            //Contact
            GridColumn columnContact = sluContact.Properties.View.Columns.AddField("code");
            columnContact.Visible = true;
            columnContact = sluContact.Properties.View.Columns.AddField("id");
            columnContact.Visible = false;
            columnContact = sluContact.Properties.View.Columns.AddField("name");
            columnContact.Visible = true;
            columnContact = sluContact.Properties.View.Columns.AddField("idNumber");
            columnContact.Visible = true;
            sluContact.Properties.DisplayMember = "name";
            sluContact.Properties.ValueMember = "id";

            //Source
            GridColumn columnSource = sluSource.Properties.View.Columns.AddField("code");
            columnSource.Visible = true;
            columnSource = sluSource.Properties.View.Columns.AddField("id");
            columnSource.Visible = false;
            columnSource = sluSource.Properties.View.Columns.AddField("name");
            columnSource.Visible = true;
            columnSource = sluSource.Properties.View.Columns.AddField("idNumber");
            columnSource.Visible = true;
            sluSource.Properties.DisplayMember = "name";
            sluSource.Properties.ValueMember = "id";

            //category
            lukEventCateg.Properties.Columns.Add(new LookUpColumnInfo("Description", "Event Category"));
            lukEventCateg.Properties.DisplayMember = "Description";
            lukEventCateg.Properties.ValueMember = "Id";

            //Booking Type
            lukBookingtype.Properties.Columns.Add(new LookUpColumnInfo("Description", "Booking Type"));
            lukBookingtype.Properties.DisplayMember = "Description";
            lukBookingtype.Properties.ValueMember = "Id";

            //Object State
            lukObjectState.Properties.Columns.Add(new LookUpColumnInfo("Description", "Object State"));
            lukObjectState.Properties.DisplayMember = "Description";
            lukObjectState.Properties.ValueMember = "Id";
        }


        private bool InitializeData()
        {
            try
            {

                List<String> approvedFunctionalities = LocalBuffer.LocalBuffer.AllAccessMatrixFunctionList.Where(x => x.VisuaCompDesc == "Event Management").Select(x => x.Description).ToList();

                if (approvedFunctionalities.Contains("Event Header"))
                    EventHeaderAccess = true;

                if (approvedFunctionalities.Contains("Event Detail"))
                    EventDetailAccess = true;

                if (approvedFunctionalities.Contains("Event Requirement"))
                    EventRequirementAccess = true;

                //Progress_Reporter.Show_Progress("loading data", "Please Wait...");

                DateTime? currentDate = GetCurrentTime();
                if (currentDate == null)
                {
                    //Progress_Reporter.Close_Progress();
                    return false;
                }

                CurrentTime = currentDate.Value;

                bool isEdit = EventDTO == null ? false : true;

                //Get Current Event Number
                string currentCode = string.Empty;
                if (isEdit)
                {
                    currentCode = EventDTO.Code;
                    _currentEventHeaderId = EventDTO.Id;
                    _currentEventHeaderCode = currentCode;
                }
                else
                {
                    //Generate ID
                    string id = UIProcessManager.IdGenerater("Voucher", CNETConstantes.EVENT_VOUCHER, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (id != null)
                    {
                        currentCode = id;
                    }
                    else
                    {
                        //Progress_Reporter.Close_Progress();
                        XtraMessageBox.Show("There is a problem on id setting! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                }
                cmbBillingType.EditValue = "Cash";

                teEventNo.Text = currentCode;

                GetRoundSetting();

                EventVoucherBuffer = new VoucherBuffer();
                EventVoucherBuffer.Voucher = new VoucherDTO
                {
                    Code = teEventNo.Text,
                    Type = CNETConstantes.VOUCHER_COMPONENET,
                    Definition = CNETConstantes.EVENT_VOUCHER,
                    IssuedDate = currentDate.Value,
                    IsIssued = true,
                    Year = currentDate.Value.Year,
                    Month = currentDate.Value.Month,
                    Day = currentDate.Value.Day,
                    GrandTotal = 0,
                    Period = null,
                    LastActivity = null,
                    Remark = null
                };

                EventVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.EVENT_VOUCHER);

                ConfigurationDTO PaxArticleConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == "Rate/Pax Article");

                if (PaxArticleConfig != null && !string.IsNullOrEmpty(PaxArticleConfig.CurrentValue))
                {
                    int? paxartid = ParseNullableInt(PaxArticleConfig.CurrentValue);
                    if (paxartid != null)
                        PaxArticle = UIProcessManager.GetArticleById(paxartid.Value);
                    else
                        PaxArticle = null;
                }
                ConfigurationDTO HallChargeArticleConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == "Hall Charge Article");

                if (HallChargeArticleConfig != null && !string.IsNullOrEmpty(HallChargeArticleConfig.CurrentValue))
                {
                    int? Hallartid = ParseNullableInt(HallChargeArticleConfig.CurrentValue);
                    if (Hallartid != null)
                        HallArticle = UIProcessManager.GetArticleById(Hallartid.Value);
                    else
                        HallArticle = null;
                }

                List<int> paymentlist = new List<int>() { 1748, 1757, 1758 };
                List<SystemConstantDTO> paymentList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.PAYMENT_METHODS && paymentlist.Contains(l.Id)).ToList();
                lkPaymentMethod.Properties.DataSource = (paymentList.OrderByDescending(c => c.IsDefault).ToList());
                lkPaymentMethod.Properties.DisplayMember = "Description";
                lkPaymentMethod.Properties.ValueMember = "Id";
                SystemConstantDTO cashpayment = paymentList.FirstOrDefault(x => x.Description.ToLower() == "cash");
                if (cashpayment != null)
                {
                    lkPaymentMethod.EditValue = cashpayment.Id;
                }
                else
                {
                    lkPaymentMethod.EditValue = paymentList.FirstOrDefault().Id;
                }

                //check PREPARED workflow
                if (!isEdit)
                {



                    ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.EVENT_VOUCHER).FirstOrDefault();
                    if (workFlow != null)
                    {

                        _adPrepared = workFlow.Id;
                    }
                    else
                    {
                        //Progress_Reporter.Close_Progress();
                        XtraMessageBox.Show("Please define workflow of PREPARED for Event Voucher ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return false;
                    }

                }
                else
                {
                    ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Edit, CNETConstantes.EVENT_VOUCHER).FirstOrDefault();


                    if (workFlow != null)
                    {

                        _adEdited = workFlow.Id;
                    }
                    else
                    {
                        //Progress_Reporter.Close_Progress();
                        XtraMessageBox.Show("Please define workflow of UPDATED for Event Voucher ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return false;
                    }
                }

                int? currentWorkflow = !isEdit ? _adPrepared : _adEdited;

                /*  
                  //Check Activity Previlage
                  var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                  if (userRoleMapper != null)
                  {
                      var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(currentWorkflow.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                      if (roleActivity != null)
                      {
                          frmNeedPassword frmNeedPass = new frmNeedPassword(true);
                          frmNeedPass.ShowDialog();
                          if (!frmNeedPass.IsAutenticated)
                          {
                              //////Progress_Reporter.Close_Progress();
                              return false;
                          }
                      }
                  }
                */

                //Populate Consignees 
                if (LocalBuffer.LocalBuffer.ConsigneeViewBufferList != null && LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Count > 0)
                {
                    consigneeDTOList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Select(c => new EventConsgineeDTO()
                    {
                        id = c.Id,
                        code = c.Code,
                        gslType = c.GslType,
                        IdentficationType = "TIN Number",
                        idNumber = c.Tin,
                        name = c.FirstName + " " + c.SecondName + " " + c.ThirdName
                    }).ToList();

                    var ownersList = consigneeDTOList.Where(c => (c.gslType == CNETConstantes.GUEST || c.gslType == CNETConstantes.CUSTOMER)).ToList();
                    sluEventOwner.Properties.DataSource = ownersList;

                    var CustomerList = consigneeDTOList.Where(c => c.gslType == CNETConstantes.CUSTOMER).ToList();
                    sleCompany.Properties.DataSource = CustomerList;

                    var organizerList = consigneeDTOList.Where(c => c.gslType == CNETConstantes.AGENT).ToList();
                    sluOrganizer.Properties.DataSource = organizerList;

                    var sourceList = consigneeDTOList.Where(c => c.gslType == CNETConstantes.BUSINESSsOURCE).ToList();
                    sluSource.Properties.DataSource = sourceList;

                    var contactList = consigneeDTOList.Where(c => c.gslType == CNETConstantes.CONTACT).ToList();
                    sluContact.Properties.DataSource = contactList;

                }

                //Populate Event Category
                List<LookupDTO> eventCategList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.EVENT_CATEGORY).ToList();
                if (eventCategList != null && eventCategList.Count > 0)
                {
                    var defaultValue = eventCategList.FirstOrDefault(c => c.IsDefault);
                    _defEventCateg = defaultValue == null ? null : defaultValue.Id;
                    lukEventCateg.Properties.DataSource = eventCategList;

                }

                //Populate Booking Type
                List<SystemConstantDTO> bookingTypes = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.RESERVATION_TYPE).ToList();
                if (bookingTypes != null && bookingTypes.Count > 0)
                {
                    var defaultValue = bookingTypes.FirstOrDefault(c => c.IsDefault);
                    _defBookingType = defaultValue == null ? null : defaultValue.Id;
                    lukBookingtype.Properties.DataSource = bookingTypes;

                }

                //Populate Object State
                var osdList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(s => s.Category == CNETConstantes.EVENT_STATE.ToString()).ToList();

                lukObjectState.Properties.DataSource = osdList.Where(x => x.Id != CNETConstantes.OSD_EVENTCHECKEDOUT);
                lukObjectState.EditValue = CNETConstantes.OSD_EVENTConfirmed;


                //Setup Remaining Fields
                deStartDate.Properties.MinValue = CurrentTime;
                deEndDate.Properties.MinValue = CurrentTime;

                teDate.Text = CurrentTime.ToShortDateString();
                List<EventDepartSelection> eventDeparts = new List<EventDepartSelection>();

                var EventDepartment = LocalBuffer.LocalBuffer.LookUpBufferList.Where(x => x.Type == CNETConstantes.EVENT_Department).ToList();

                if (EventDepartment != null && EventDepartment.Count > 0)
                {
                    eventDeparts = EventDepartment.Select(x => new EventDepartSelection()
                    {
                        selected = false,
                        id = x.Id,
                        Description = x.Description,
                        Value = x.Value
                    }).ToList();

                    gcDepartmentInst.DataSource = eventDeparts;
                }


                //Populate Fields
                if (isEdit)
                {

                    var response = UIProcessManager.GetVoucherBufferById(EventDTO.Id);

                    if (!response.Success)
                    {
                        XtraMessageBox.Show("Fail to get Event Voucher !!" + Environment.NewLine + response.Message);
                        return false;
                    }

                    EventVoucherBuffer = response.Data;

                    if (EventDTO.OsdCode == CNETConstantes.OSD_EVENTCHECKEDOUT)
                    {
                        lukObjectState.Properties.DataSource = osdList;
                    }

                    cmbBillingType.EditValue = string.IsNullOrEmpty(EventVoucherBuffer.Voucher.Extension6) ? "Cash" : EventVoucherBuffer.Voucher.Extension6;

                    lkPaymentMethod.EditValue = EventVoucherBuffer.Voucher.PaymentMethod;

                    sluEventOwner.EditValue = EventDTO.Consignee1;

                    //Company
                    sleCompany.EditValue = EventDTO.Consignee2;

                    //Organizer
                    sluOrganizer.EditValue = EventDTO.Consignee3;

                    //Source
                    sluSource.EditValue = EventDTO.Consignee4;

                    //Contact
                    sluContact.EditValue = EventVoucherBuffer.Voucher.Consignee5;


                    decimal hallcharge = 0;
                    decimal ratepax = 0;
                    decimal ExpectedGuest = 1;
                    decimal GuaranteedGuest = 1;

                    decimal.TryParse(EventVoucherBuffer.Voucher.Extension2, out ExpectedGuest);
                    decimal.TryParse(EventVoucherBuffer.Voucher.Extension3, out GuaranteedGuest);




                    txtHallCharge.EditValue = EventVoucherBuffer.Voucher.Extension5;
                    txtRatePax.EditValue = EventVoucherBuffer.Voucher.Extension4;
                    nudExpectedGuest.Value = ExpectedGuest <= 0 ? 1 : ExpectedGuest;
                    nudGuaranteedGuest.Value = GuaranteedGuest <= 0 ? 1 : GuaranteedGuest;
                    memoInstructionorFlow.EditValue = EventVoucherBuffer.Voucher.Remark;
                    memoConferenceFacility.EditValue = EventVoucherBuffer.Voucher.DefaultImageUrl;

                    //Extension2 = nudExpectedGuest.Value.ToString(),
                    //Extension3 = nudGuaranteedGuest.Value.ToString(),
                    //Extension4 = txtRatePax.EditValue == null ? null : txtRatePax.EditValue.ToString(),
                    //Extension5 = txtHallCharge.EditValue == null ? null : txtHallCharge.EditValue.ToString(),


                    lukEventCateg.EditValue = EventDTO.EventCategCode;
                    txtDescription.EditValue = EventDTO.Description;
                    teDate.EditValue = EventDTO.Date;
                    deStartDate.EditValue = EventDTO.StartDate;
                    deEndDate.EditValue = EventDTO.EndDate;
                    lukBookingtype.EditValue = EventDTO.BookingTypeCode;
                    lukObjectState.EditValue = EventDTO.OsdCode;

                    if (EventDTO.OsdCode == CNETConstantes.OSD_EVENTCHECKEDOUT)
                    {
                        EventCheckedout = true;
                        DisableControl();
                    }

                    if (EventVoucherBuffer.VoucherLookupLists != null && EventVoucherBuffer.VoucherLookupLists.Count > 0 && eventDeparts != null && eventDeparts.Count > 0)
                    {

                        foreach (var dep in eventDeparts)
                        {
                            var data = EventVoucherBuffer.VoucherLookupLists.FirstOrDefault(x => x.SelectedLookup == dep.id);
                            if (data != null)
                            {
                                dep.selected = true;
                                dep.Value = data.Remark;
                            }
                        }
                        gcDepartmentInst.DataSource = eventDeparts;
                        gcDepartmentInst.RefreshDataSource();
                    }
                }
                else
                {
                    deStartDate.EditValue = CurrentTime;
                    deEndDate.EditValue = CurrentTime.AddDays(1);
                    lukEventCateg.EditValue = _defEventCateg;
                    lukBookingtype.EditValue = _defBookingType;
                }


                //Populate Event Detail;
                PopulateEventDetail();

                if (!EventDetailAccess)
                    tpEventDetail.PageEnabled = false;


                if (!EventRequirementAccess)
                    tpEventRequirement.PageEnabled = false;

                if (!EventHeaderAccess)
                    DisableControl();

                //Progress_Reporter.Close_Progress();
                return true;
            }
            catch (Exception ex)
            {
                //Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Error has occured in populating data. DETAIL:: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public int QuantityRoundDigit = 2;
        public int UnitPriceRoundDigit = 2;
        public int TotalAmountRoundDigit = 2;
        public void GetRoundSetting()
        {
            var configurationList = LocalBuffer.LocalBuffer.ConfigurationBufferList.Where(x => x.Reference == CNETConstantes.EVENT_REQUIREMENT_VOUCHER.ToString());
            foreach (ConfigurationDTO config in configurationList)
            {
                switch (config.Attribute.ToLower())
                {
                    case "round digit quantity":
                        QuantityRoundDigit = Convert.ToInt32(config.CurrentValue);
                        break;
                    case "round digit unit price":
                        UnitPriceRoundDigit = Convert.ToInt32(config.CurrentValue);
                        break;
                    case "round digit total":
                        TotalAmountRoundDigit = Convert.ToInt32(config.CurrentValue);
                        break;
                }
            }
        }
        bool EventCheckedout = false;
        public void DisableControl()
        {
            bbiSave.Enabled = false;
            lukObjectState.Enabled = false;
            lukEventCateg.Enabled = false;
            txtDescription.Enabled = false;
            //teDate.Enabled = false;
            deStartDate.Enabled = false;
            deEndDate.Enabled = false;
            lukBookingtype.Enabled = false;
            sluOrganizer.Enabled = false;
            sluSource.Enabled = false;
            sluContact.Enabled = false;
            bbiDelete.Enabled = false;
            sluEventOwner.Enabled = false;
            sleCompany.Enabled = false;
            bbiEdit.Enabled = false;
            memoInstructionorFlow.Enabled = false;
            nudExpectedGuest.Enabled = false;
            nudGuaranteedGuest.Enabled = false;
            txtHallCharge.Enabled = false;
            txtRatePax.Enabled = false;
            //btnCashReceipt.Enabled = false;
            memoConferenceFacility.Enabled = false;
            gcDepartmentInst.Enabled = false;
            if (!EventHeaderAccess)
                bbiNew.Enabled = false;
        }
        public void EnableControl()
        {
            bbiSave.Enabled = true;
            lukObjectState.Enabled = true;
            lukEventCateg.Enabled = true;
            txtDescription.Enabled = true;
            sleCompany.Enabled = true;
            //teDate.Enabled = true;
            deStartDate.Enabled = true;
            deEndDate.Enabled = true;
            lukBookingtype.Enabled = true;
            sluOrganizer.Enabled = true;
            sluSource.Enabled = true;
            sluContact.Enabled = true;
            bbiDelete.Enabled = true;
            sluEventOwner.Enabled = true;
            bbiEdit.Enabled = true;
            memoInstructionorFlow.Enabled = true;
            nudExpectedGuest.Enabled = true;
            nudGuaranteedGuest.Enabled = true;
            txtHallCharge.Enabled = true;
            txtRatePax.Enabled = true;
            memoConferenceFacility.Enabled = true;
            gcDepartmentInst.Enabled = true;
            //btnCashReceipt.Enabled = true;

            if (!EventHeaderAccess)
                bbiNew.Enabled = true;
        }
        private DateTime? GetCurrentTime()
        {
            DateTime? date = UIProcessManager.GetServiceTime();
            if (date != null)
            {
                return date.Value;

            }
            else
            {
                XtraMessageBox.Show("Server Datetime Error !!", "ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; ;
            }
        }

        private void PopulateEventDetail()
        {
            //Progress_Reporter.Show_Progress("Populating Event Detail", "Please Wait");
            _eventDetailDtoList.Clear();
            gcEventDetailList.DataSource = null;
            gcEventDetail.DataSource = null;
            gvEventDetail.RefreshData();
            gvEventDetailList.RefreshData();

            List<EventDisplayView> eventDetailList = UIProcessManager.GetEventDisplayView(_currentEventHeaderId);
            if (eventDetailList == null || eventDetailList.Count == 0)
            {
                //Progress_Reporter.Close_Progress();
                return;
            }
            int count = 0;
            foreach (var ev in eventDetailList)
            {
                count++;
                EventDetaildataDTO dto = new EventDetaildataDTO()
                {
                    SN = count,
                    ArrangementDesc = ev.SpaceArrangementDes,
                    Arrangementid = ev.spaceArrangment,
                    Id = ev.EventDetailId,
                    Description = ev.EventDetailDescription,
                    EndTime = ev.endTimeStamp == null ? CurrentTime : ev.endTimeStamp,
                    Hall = ev.HallDescription,
                    SpaceCode = ev.space,
                    NumOfPerson = ev.noOfPerson == null ? 0 : ev.noOfPerson,
                    StartTime = ev.startTimeStamp == null ? CurrentTime : ev.startTimeStamp,
                    TypeDesc = ev.EventDetailTypeDescription,
                    TypeCode = ev.EventDetailType

                };

                _eventDetailDtoList.Add(dto);

                gcEventDetail.DataSource = _eventDetailDtoList;
                gvEventDetail.RefreshData();

                gcEventDetailList.DataSource = _eventDetailDtoList;
                gvEventDetailList.RefreshData();

                //Progress_Reporter.Close_Progress();
            }
        }

        private void PopulateEventRequirements(int eventDetailid, string eventDetailCode)
        {
            gcEventReq.DataSource = null;
            gvEventReq.RefreshData();

            //get eventDetailData by EventHeader.
            List<EventRequirementView> eventRequirementList = UIProcessManager.GetEventRequirementView(eventDetailid).ToList();
            List<EventReqDTO> eventReqDtoList = new List<EventReqDTO>();
            if (eventRequirementList == null || eventRequirementList.Count == 0) return;
            int count = 0;
            foreach (var evReq in eventRequirementList)
            {
                count++;
                EventReqDTO dto = new EventReqDTO()
                {
                    EventDetailCode = eventDetailCode,
                    ArticleCode = evReq.Articlecode,
                    Articleid = evReq.Articleid,
                    ArticleName = evReq.ArtName,
                    Qty = evReq.Quantity,
                    SN = count,
                    TaxAmount = evReq.TaxAmount == null ? 0 : evReq.TaxAmount,
                    TotalAmount = evReq.TotalAmount == null ? 0 : evReq.TotalAmount,
                    UnitAmount = evReq.UnitAmt == null ? 0 : evReq.UnitAmt,
                    VoucherCode = evReq.Vouchercode,
                    VoucherId = evReq.Voucherid,
                    LineItemNote = evReq.LineItemNote,
                    VoucherNote = evReq.VoucherNote
                };

                eventReqDtoList.Add(dto);
            }

            gcEventReq.DataSource = eventReqDtoList;
            gvEventReq.RefreshData();

            //Grouping
            gvEventReq.BeginSort();
            try
            {
                gvEventReq.Columns["VoucherCode"].GroupIndex = 0;
            }
            finally
            {
                gvEventReq.EndSort();
            }
            gvEventReq.ExpandAllGroups();
        }

        private List<EventReqDTO> GetEventRequirements(int eventDetailid, string eventDetailCode)
        {

            //get eventDetailData by EventHeader.
            List<EventRequirementView> eventRequirementList = UIProcessManager.GetEventRequirementView(eventDetailid).ToList();
            List<EventReqDTO> eventReqDtoList = new List<EventReqDTO>();
            if (eventRequirementList == null || eventRequirementList.Count == 0)
                return eventReqDtoList;
            int count = 0;
            foreach (var evReq in eventRequirementList)
            {
                count++;
                EventReqDTO dto = new EventReqDTO()
                {
                    EventDetailid = eventDetailid,
                    EventDetailCode = eventDetailCode,
                    Articleid = evReq.Articleid,
                    ArticleCode = evReq.Articlecode,
                    ArticleName = evReq.ArtName,
                    Qty = evReq.Quantity,
                    SN = count,
                    AdditionalCharge = evReq.AdditionalCharge,
                    Discount = evReq.Discount,
                    TaxAmount = evReq.TaxAmount == null ? 0 : evReq.TaxAmount,
                    TotalAmount = evReq.TotalAmount == null ? 0 : evReq.TotalAmount,
                    UnitAmount = evReq.UnitAmt == null ? 0 : evReq.UnitAmt,
                    VoucherCode = evReq.Vouchercode,
                    VoucherId = evReq.Voucherid,
                    LineItemNote = evReq.LineItemNote,
                    VoucherNote = evReq.VoucherNote,
                    LineItemCode = evReq.Lineitemid
                };

                eventReqDtoList.Add(dto);
            }
            return eventReqDtoList;
        }

        private void Reset()
        {
            _currentEventHeaderCode = string.Empty;
            _eventDetailDtoList.Clear();
            gcEventDetail.DataSource = null;
            gvEventDetail.RefreshData();
            gcEventReq.DataSource = null;
            gcEventDetailList.DataSource = null;
            sleCompany.EditValue = null;
            sluEventOwner.EditValue = null;
            sluOrganizer.EditValue = null;
            sluSource.EditValue = null;
            sluContact.EditValue = null;

            lukEventCateg.EditValue = _defEventCateg;
            lukBookingtype.EditValue = _defBookingType;
            lukObjectState.EditValue = _defObjState;

            nudExpectedGuest.Value = 1;
            nudGuaranteedGuest.Value = 1;
            txtHallCharge.EditValue = null;
            txtRatePax.EditValue = null;
            txtDescription.EditValue = null;
            memoInstructionorFlow.EditValue = null;

            // Get Current Time
            DateTime? currentDate = GetCurrentTime();
            if (currentDate == null)
            {
                this.Close();
            }

            CurrentTime = currentDate.Value;


            //Generate ID
            string id = UIProcessManager.IdGenerater("Voucher", CNETConstantes.EVENT_VOUCHER, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(id))
            {
                teEventNo.Text = id;
            }
            else
            {
                XtraMessageBox.Show("There is a problem on id setting! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
         
        private void SaveEventHeader()
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                    sluEventOwner,
                    lukEventCateg,
                    txtDescription,
                    deStartDate,
                    deEndDate,
                    lukBookingtype,
                    lukObjectState,
                    lkPaymentMethod,
                    cmbBillingType
                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    return;
                }
                if (lukObjectState.EditValue == null)
                {

                    XtraMessageBox.Show("Please Select State First !! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }

                //Progress_Reporter.Show_Progress("Saving Event Header", "Please Wait....");

                bool isEdit = EventDTO == null ? false : true;

                DateTime? currentDate = GetCurrentTime();
                if (currentDate == null)
                {
                    //Progress_Reporter.Close_Progress();
                    return;
                }

                CurrentTime = currentDate.Value;

                string voCode = "";

                //Generate Id
                string id = UIProcessManager.IdGenerater("Voucher", CNETConstantes.EVENT_VOUCHER, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(id))
                    voCode = id;
                else
                {
                    XtraMessageBox.Show("There is a problem on id setting! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Progress_Reporter.Close_Progress();
                    return;

                }

                EventVoucherBuffer.Voucher = new VoucherDTO
                {
                    Code = voCode,
                    OriginConsigneeUnit = SelectedHotelcode,
                    Type = Convert.ToInt32(lukBookingtype.EditValue.ToString()),
                    Extension1 = lukEventCateg.EditValue.ToString(), // Event Category
                    Extension2 = nudExpectedGuest.Value.ToString(),
                    Extension3 = nudGuaranteedGuest.Value.ToString(),
                    Extension4 = txtRatePax.EditValue == null ? null : txtRatePax.EditValue.ToString(),
                    Extension5 = txtHallCharge.EditValue == null ? null : txtHallCharge.EditValue.ToString(),
                    Extension6 = cmbBillingType.EditValue == null ? "Cash" : cmbBillingType.EditValue.ToString(),
                    Definition = CNETConstantes.EVENT_VOUCHER,
                    Consignee1 = Convert.ToInt32(sluEventOwner.EditValue.ToString()),
                    IssuedDate = CurrentTime,
                    Year = CurrentTime.Year,
                    Month = CurrentTime.Month,
                    Day = CurrentTime.Day,
                    IsIssued = true,
                    IsVoid = false,
                    Discount = VoucherFinal.voucher.Discount,
                    AddCharge = VoucherFinal.voucher.AddCharge,
                    SubTotal = VoucherFinal.voucher.SubTotal,
                    GrandTotal = VoucherFinal.voucher.GrandTotal,
                    Period = null,
                    LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id,
                    LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id,
                    StartDate = deStartDate.DateTime,
                    EndDate = deEndDate.DateTime,
                    PaymentMethod = lkPaymentMethod.EditValue == null ? CNETConstantes.PAYMENTMETHODSCASH : Convert.ToInt32(lkPaymentMethod.EditValue.ToString()),
                    Note = txtDescription.EditValue.ToString(),
                    LastState = Convert.ToInt32(lukObjectState.EditValue.ToString()),
                    Remark = memoInstructionorFlow.Text,
                    DefaultImageUrl = memoConferenceFacility.Text
                };

                if (sleCompany.EditValue != null && !string.IsNullOrEmpty(sleCompany.EditValue.ToString()))
                    EventVoucherBuffer.Voucher.Consignee2 = Convert.ToInt32(sleCompany.EditValue.ToString());

                if (sluOrganizer.EditValue != null && !string.IsNullOrEmpty(sluOrganizer.EditValue.ToString()))
                    EventVoucherBuffer.Voucher.Consignee3 = Convert.ToInt32(sluOrganizer.EditValue.ToString());

                if (sluSource.EditValue != null && !string.IsNullOrEmpty(sluSource.EditValue.ToString()))
                    EventVoucherBuffer.Voucher.Consignee4 = Convert.ToInt32(sluSource.EditValue.ToString());

                if (sluContact.EditValue != null && !string.IsNullOrEmpty(sluContact.EditValue.ToString()))
                    EventVoucherBuffer.Voucher.Consignee5 = Convert.ToInt32(sluContact.EditValue.ToString());


                EventVoucherBuffer.VoucherLookupLists = new List<VoucherLookupListDTO>();

                EventVoucherBuffer.LineItemsBuffer = lineItemDetails.Select(x => new LineItemBuffer { LineItem = x.lineItems, LineItemValueFactors = x.lineItemValueFactor }).ToList();
                //EventVoucherBuffer.TaxTransactions = VoucherFinal.taxTransactions;
                //EventVoucherBuffer.Voucher.SubTotal =   VoucherFinal.voucher.SubTotal;
                //EventVoucherBuffer.Voucher.GrandTotal =   VoucherFinal.voucher.GrandTotal;


                gvDepartmentInst.PostEditor();
                List<EventDepartSelection> ListdeventDepartment = (List<EventDepartSelection>)gcDepartmentInst.DataSource;
                if (ListdeventDepartment != null && ListdeventDepartment.Count > 0)
                {
                    ListdeventDepartment = ListdeventDepartment.Where(x => x.selected).ToList();
                    if (ListdeventDepartment != null && ListdeventDepartment.Count > 0)
                    {
                        EventVoucherBuffer.VoucherLookupLists = ListdeventDepartment.Select(x => new VoucherLookupListDTO()
                        {
                            SelectedLookup = x.id,
                            Remark = x.Value
                        }).ToList();
                    }
                }

                if (!isEdit)
                {
                    EventVoucherBuffer.Activity = SetupActivity(currentDate.Value, _adPrepared.Value, CNETConstantes.VOUCHER_COMPONENET, String.Format("Owner = {0}  Category = {1}  StartDate = {2}  EndDate =  {3}", sluEventOwner.Text, lukEventCateg.Text, deStartDate.DateTime.ToShortDateString(), deEndDate.DateTime.ToShortDateString()));



                    var isSaved = UIProcessManager.CreateVoucherBuffer(EventVoucherBuffer);


                    if (isSaved != null && isSaved.Success)
                    {
                        _currentEventHeaderId = isSaved.Data.Voucher.Id;
                        _currentEventHeaderCode = isSaved.Data.Voucher.Code;
                        id = UIProcessManager.IdGenerater("Voucher", CNETConstantes.EVENT_VOUCHER, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);
                        XtraMessageBox.Show("Event Header is Successfully Saved!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        XtraMessageBox.Show("Event Header is not saved!" + Environment.NewLine + isSaved.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                }
                else
                {
                    //UPDATING EVENT HEADER
                    EventVoucherBuffer.Voucher.Id = EventDTO.Id;
                    EventVoucherBuffer.Voucher.Code = EventDTO.Code;
                    EventVoucherBuffer.Activity = SetupActivity(currentDate.Value, _adEdited.Value, CNETConstantes.VOUCHER_COMPONENET, String.Format("Owner = {0}  Category = {1}  StartDate = {2}  EndDate =  {3}", sluEventOwner.Text, lukEventCateg.Text, deStartDate.DateTime.ToShortDateString(), deEndDate.DateTime.ToShortDateString()));


                    var isSaved = UIProcessManager.UpdateVoucherBuffer(EventVoucherBuffer);


                    if (isSaved != null && isSaved.Success)
                    {

                        XtraMessageBox.Show("Event Header Update Successfully Saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        XtraMessageBox.Show("Event Header is not Updated!" + Environment.NewLine + isSaved.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }


                //Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                //Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Error has occured in saving event header. DETAIL:: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        public static ActivityDTO SetupActivity(DateTime serverTimeStamp, int activityDefCode, int compCode, string remark = "", int? userCode = null)
        {
            //bool isExist = IsExistActDefinitionCode(activityDefCode);

            //if (!isExist) return null;

            UserDTO currentUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;
            DeviceDTO device = LocalBuffer.LocalBuffer.CurrentDevice;

            ActivityDTO activity = new ActivityDTO()
            {
                ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit,
                TimeStamp = serverTimeStamp,
                Year = serverTimeStamp.Year,
                ActivityDefinition = activityDefCode,
                Month = serverTimeStamp.Month,
                Day = serverTimeStamp.Day,
                Device = device.Id,
                Pointer = compCode,
                Platform = "1",
                User = userCode == null ? currentUser.Id : userCode.Value,
                Remark = remark
            };

            return activity;
        }



        #endregion


        #region Event Handlers

        #endregion

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tcEventEditor.SelectedTabPage == tpEventHeader)
            {
                SaveEventHeader();

            }
            else if (tcEventEditor.SelectedTabPage == tpEventDetail)
            {

            }
            else if (tcEventEditor.SelectedTabPage == tpEventRequirement)
            {

            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void EventEditor_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tcEventEditor.SelectedTabPage == tpEventHeader)
            {
                EventCheckedout = false;
                EnableControl();
                Reset();

            }
            else if (tcEventEditor.SelectedTabPage == tpEventDetail && !EventCheckedout)
            {

                EventDetailForm frmEventDetail = new EventDetailForm();
                frmEventDetail.SelectedHotelcode = SelectedHotelcode;
                frmEventDetail.EventHeaderId = _currentEventHeaderId;
                frmEventDetail.EventHeaderCode = _currentEventHeaderCode;
                frmEventDetail.Guaranteedpax = nudGuaranteedGuest.Value;
                if (frmEventDetail.ShowDialog() == DialogResult.OK)
                {
                    PopulateEventDetail();
                }
            }
            else if (!EventCheckedout)
            {

                EventDetaildataDTO dto = gvEventDetailList.GetFocusedRow() as EventDetaildataDTO;
                if (dto == null)
                {
                    XtraMessageBox.Show("Please Select Event Detail First!", "Event Req.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int? customerid = null; ParseNullableInt(sluEventOwner.EditValue.ToString());
                if (sluEventOwner.EditValue != null)
                    customerid = ParseNullableInt(sluEventOwner.EditValue.ToString());

                EventRequirement eventRequirement = new EventRequirement(_currentEventHeaderId, dto.Id);
                eventRequirement.Customerid = customerid;
                if (eventRequirement.PreparedActDef == null)
                {
                    XtraMessageBox.Show("Please Maintain Prepared Workflow first !", "Event Req.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
                eventRequirement.ShowDialog();
                PopulateEventRequirements(dto.Id, dto.Code);

                //Voucher vo = UIProcessManager.SelectVoucher(_currentEventHeaderCode);
                //if(vo == null)
                //{
                //    XtraMessageBox.Show("Unable to get Event Voucher", "Event Req.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                //Voucher_UI voucherUI = new Voucher_UI(CNETConstantes.EVENT_REQUIREMENT_VOUCHER, dto.Code );
                //voucherUI.IsVoucherLineitem = true;


                //var voBuffer = IntializeVoucherBuffer(CNETConstantes.EVENT_REQUIREMENT_VOUCHER);
                //voBuffer.voucher.consignee = vo.consignee;

                //if (voucherUI.IsVoucherCreated(voBuffer))
                //{

                //    lv = voucherUI.GetCreatedVoucher();
                //    lv.WindowState = FormWindowState.Normal;
                //    lv.StartPosition = FormStartPosition.CenterScreen;
                //    lv.TopMost = false;
                //    lv.ShowDialog(this);

                //    PopulateEventRequirements(dto.Code);
                //}
            }
        }
        public int? ParseNullableInt(string input)
        {
            if (int.TryParse(input, out int result))
            {
                return result; // Parsing successful
            }
            return null; // Parsing failed, return null
        }
        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tcEventEditor.SelectedTabPage == tpEventDetail)
            {
                PopulateEventDetail();
            }
            else
            {
                EventDetaildataDTO dto = gvEventDetailList.GetFocusedRow() as EventDetaildataDTO;
                if (dto != null)
                {
                    PopulateEventRequirements(dto.Id, dto.Code);
                }
            }

        }

        private void tcEventEditor_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            //TabControl tc = sender as TabControl;
            //if (tc == null) return;

            if (e.Page == tpEventDetail)
            {
                rpgRefresh.Visible = true;
                rpgDelete.Visible = true;
                rpgEdit.Visible = true;
            }
            else if (e.Page == tpEventRequirement)
            {
                rpgRefresh.Visible = true;
                rpgEdit.Visible = true;
                rpgDelete.Visible = false;
            }
            else
            {
                rpgRefresh.Visible = false;
                rpgDelete.Visible = false;
                rpgEdit.Visible = false;
            }

        }

        private void gvEventDetailList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            EventDetaildataDTO dto = view.GetRow(e.FocusedRowHandle) as EventDetaildataDTO;

            gcEventReq.DataSource = null;
            gvEventReq.RefreshData();

            if (dto == null) return;
            PopulateEventRequirements(dto.Id, dto.Code);
        }

        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tcEventEditor.SelectedTabPage == tpEventDetail)
            {
                EventDetaildataDTO dto = gvEventDetail.GetFocusedRow() as EventDetaildataDTO;
                if (dto == null)
                {
                    XtraMessageBox.Show("Please Select Event Detail First!", "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult dr = XtraMessageBox.Show("Do you want to delete event detail?", "Event Detail", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dr == DialogResult.No)
                {
                    return;
                }


                bool flag = UIProcessManager.DeleteVoucherById(dto.Id);
                if (flag)
                {
                    XtraMessageBox.Show("Event Detail is successfully deleted", "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PopulateEventDetail();
                }
                else
                {
                    XtraMessageBox.Show("Event Detail is not deleted!", "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }


            }
            else if (tcEventEditor.SelectedTabPage == tpEventRequirement)
            {

            }
            else
            {

            }
        }

        private void bbiEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (tcEventEditor.SelectedTabPage == tpEventDetail)
                {
                    EventDetaildataDTO dto = gvEventDetail.GetFocusedRow() as EventDetaildataDTO;
                    if (dto == null)
                    {
                        XtraMessageBox.Show("Please Select Event Detail!", "Event Managment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    EventDetailForm frmEventDetail = new EventDetailForm();
                    frmEventDetail.SelectedHotelcode = SelectedHotelcode;
                    frmEventDetail.EventHeaderId = _currentEventHeaderId;
                    frmEventDetail.EventHeaderCode = _currentEventHeaderCode;
                    frmEventDetail.Guaranteedpax = nudGuaranteedGuest.Value;
                    frmEventDetail.EventDetailDTO = dto;
                    if (frmEventDetail.ShowDialog() == DialogResult.OK)
                    {
                        PopulateEventDetail();
                    }
                }
                else if (tcEventEditor.SelectedTabPage == tpEventRequirement)
                {
                    EventDetaildataDTO EventDetailDTO = gvEventDetailList.GetFocusedRow() as EventDetaildataDTO;
                    if (EventDetailDTO == null)
                    {
                        XtraMessageBox.Show("Please Select Event Detail First!", "Event Req.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //check workflow
                    var workflowEventReqVoucher = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Edit, CNETConstantes.EVENT_REQUIREMENT_VOUCHER);

                    if (workflowEventReqVoucher == null)
                    {
                        //Progress_Reporter.Close_Progress();
                        XtraMessageBox.Show("Please Define Workflow of EDIT for Event Req. Voucher", "Event Req.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    EventReqDTO dto = gvEventReq.GetFocusedRow() as EventReqDTO;
                    if (dto != null && dto.VoucherId != null)
                    {
                        EventRequirement eventRequirement = new EventRequirement(_currentEventHeaderId, EventDetailDTO.Id, dto.VoucherId.Value);
                        if (eventRequirement.EditActDef == null)
                        {
                            XtraMessageBox.Show("Please Maintain Edit Workflow first !", "Event Req.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;

                        }
                        eventRequirement.ShowDialog();
                        PopulateEventRequirements(EventDetailDTO.Id, EventDetailDTO.Code);

                        //NewVoucherBuffer voBuff = frmVoucherMain.PrepareVoucherBufferObjForExternal(dto.VoucherCode, CNETConstantes.EVENT_REQUIREMENT_VOUCHER);
                        //Voucher_UI voucherUI = new Voucher_UI(CNETConstantes.EVENT_REQUIREMENT_VOUCHER);
                        //voucherUI.IsVoucherLineitem = true;
                        //if (voucherUI.IsVoucherCreated(voBuff))
                        //{
                        //    lv = voucherUI.GetCreatedVoucher();
                        //    lv.PopulateCurrentVoucherBuffer(dto.VoucherCode, workflowEventReqVoucher.code);
                        //    lv.WindowState = FormWindowState.Normal;
                        //    lv.StartPosition = FormStartPosition.CenterScreen;
                        //    lv.TopMost = false;
                        //    lv.ShowDialog(this);
                        //    PopulateEventRequirements(EventDetailDTO.Code);
                        //}
                    }
                    else
                    {
                        XtraMessageBox.Show("Please Select Event Requirement First!", "Event Req.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in Editing. DETAIL:: " + ex.Message, "Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void gvEventDetail_DoubleClick(object sender, EventArgs e)
        {
            if (!EventCheckedout)
            {
                GridView view = sender as GridView;
                EventDetaildataDTO dto = view.GetFocusedRow() as EventDetaildataDTO;
                if (dto == null)
                {
                    XtraMessageBox.Show("Please Select Event Detail!", "Event Managment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                EventDetailForm frmEventDetail = new EventDetailForm();
                frmEventDetail.SelectedHotelcode = SelectedHotelcode;
                frmEventDetail.EventHeaderId = _currentEventHeaderId;
                frmEventDetail.Guaranteedpax = nudGuaranteedGuest.Value;
                frmEventDetail.EventHeaderCode = _currentEventHeaderCode;
                frmEventDetail.EventDetailDTO = dto;
                if (frmEventDetail.ShowDialog() == DialogResult.OK)
                {
                    PopulateEventDetail();
                }

            }
        }

        private void btnConfirmation_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            List<EventDetaildataDTO> EventDetailList = (List<EventDetaildataDTO>)gcEventDetail.DataSource;

            if (EventDetailList != null)
            {
                List<EventReqDTO> EventReqList = new List<EventReqDTO>();
                decimal SubTotal = 0;
                decimal Taxamount = 0;
                decimal GrandTotal = 0;


                foreach (EventDetaildataDTO detail in EventDetailList)
                {
                    List<EventReqDTO> EventReq = GetEventRequirements(detail.Id, detail.Code);
                    if (EventReq != null && EventReq.Count > 0)
                    {
                        EventReqList.AddRange(EventReq);
                        List<int> vouid = EventReq.Select(x => x.VoucherId.Value).Distinct().ToList();

                        foreach (int vid in vouid)
                        {
                            var VoucherData = UIProcessManager.GetVoucherById(vid);
                            var dd = EventReq.Where(x => x.VoucherId == vid);
                            if (VoucherData != null)
                            {
                                SubTotal += VoucherData.SubTotal;
                                Taxamount += dd.Sum(x => x.TaxAmount.Value);
                                GrandTotal += VoucherData.GrandTotal;
                            }

                        }
                    }
                }
                // if (EventReqList != null && EventReqList.Count > 0)
                // {
                List<PrintOutDTO> EventData = (from eventdetail in EventDetailList
                                               join eventreq in EventReqList
                                    on eventdetail.Id equals eventreq.EventDetailid
                                        into a
                                               from b in a.DefaultIfEmpty()
                                               select new PrintOutDTO
                                               {
                                                   Code = eventdetail.Code,
                                                   Voucher = eventdetail.Voucher,
                                                   TypeCode = eventdetail.TypeCode,
                                                   TypeDesc = eventdetail.TypeDesc,
                                                   Description = eventdetail.Description,
                                                   SpaceCode = eventdetail.SpaceCode,
                                                   Hall = eventdetail.Hall,
                                                   ArrangementDesc = eventdetail.ArrangementDesc,
                                                   ArrangementCode = eventdetail.Arrangementid,
                                                   StartTime = eventdetail.StartTime,
                                                   EndTime = eventdetail.EndTime,
                                                   NumOfPerson = eventdetail.NumOfPerson,
                                                   SN = b == null ? 0 : b.SN,
                                                   VoucherCode = b == null ? "" : b.VoucherCode,
                                                   ArticleCode = b == null ? "" : b.ArticleCode,
                                                   ArticleName = b == null ? "" : b.ArticleName,
                                                   Qty = b == null ? 0 : b.Qty,
                                                   UnitAmount = b == null ? 0 : Math.Round(b.UnitAmount, 2),
                                                   TotalAmount = b == null ? 0 : Math.Round(b.TotalAmount, 2),
                                               }).ToList();

                EventData.ForEach(x => x.SubTotal = SubTotal);
                EventData.ForEach(x => x.TaxAmount = Taxamount);
                EventData.ForEach(x => x.GrandTotal = GrandTotal);
                EventProforma confirm = new EventProforma(EventData, true, DateTime.Now, LocalBuffer.LocalBuffer.CompanyName, sluEventOwner.Text,
                       Convert.ToDateTime(teDate.EditValue.ToString()), lukEventCateg.Text, DateTime.Now, DateTime.Now);


                confirm.PrintOption();
                //}
                //else
                //{
                //    XtraMessageBox.Show("There is no Event Requirement !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
            }
            else
            {
                XtraMessageBox.Show("There is no Event Detail !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        List<ArticleDTO> ArticleList { get; set; }
        private void btnProforma_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<EventDetaildataDTO> EventDetailList = (List<EventDetaildataDTO>)gcEventDetail.DataSource;

            if (EventDetailList != null)
            {
                List<EventReqDTO> EventReqList = new List<EventReqDTO>();
                decimal SubTotal = 0;
                decimal Taxamount = 0;
                decimal GrandTotal = 0;
                //EventFolioDTO data = UIProcessManager.GetEventFolioData(detail.Id);

                //if (data != null)
                //{
                //    SubTotal = data.EvRSubtotal;
                //    Taxamount = data.EvRVAT.HasValue ? data.EvRVAT.Value : 0;
                //    GrandTotal = data.EvRGrandTotal.HasValue ? data.EvRGrandTotal.Value : 0; ;
                //}

                foreach (EventDetaildataDTO detail in EventDetailList)
                {
                    List<EventReqDTO> EventReq = GetEventRequirements(detail.Id, detail.Code);
                    if (EventReq != null && EventReq.Count > 0)
                    {
                        EventReqList.AddRange(EventReq);
                        List<int> vouid = EventReq.Select(x => x.VoucherId.Value).Distinct().ToList();

                        foreach (int vid in vouid)
                        {
                            var VoucherData = UIProcessManager.GetVoucherById(vid);
                            var dd = EventReq.Where(x => x.VoucherId == vid);
                            if (VoucherData != null)
                            {
                                SubTotal += VoucherData.SubTotal;
                                Taxamount += dd.Sum(x => x.TaxAmount.Value);
                                GrandTotal += VoucherData.GrandTotal;
                            }

                        }
                    }
                }


                if (EventReqList != null && EventReqList.Count > 0)
                {
                    List<PrintOutDTO> EventData = (from eventdetail in EventDetailList
                                                   join eventreq in EventReqList
                                        on eventdetail.Id equals eventreq.EventDetailid
                                            into a
                                                   from b in a.DefaultIfEmpty()
                                                   select new PrintOutDTO
                                                   {
                                                       Code = eventdetail.Code,
                                                       Voucher = eventdetail.Voucher,
                                                       TypeCode = eventdetail.TypeCode,
                                                       TypeDesc = eventdetail.TypeDesc,
                                                       Description = eventdetail.Description,
                                                       SpaceCode = eventdetail.SpaceCode,
                                                       Hall = eventdetail.Hall,
                                                       ArrangementDesc = eventdetail.ArrangementDesc,
                                                       ArrangementCode = eventdetail.Arrangementid,
                                                       StartTime = eventdetail.StartTime,
                                                       EndTime = eventdetail.EndTime,
                                                       NumOfPerson = eventdetail.NumOfPerson,
                                                       SN = b == null ? 0 : b.SN,
                                                       VoucherCode = b == null ? "" : b.VoucherCode,
                                                       ArticleCode = b == null ? "" : b.ArticleCode,
                                                       ArticleName = b == null ? "" : b.ArticleName,
                                                       Qty = b == null ? 0 : b.Qty,
                                                       UnitAmount = b == null ? 0 : Math.Round(b.UnitAmount, 2),
                                                       TotalAmount = b == null ? 0 : Math.Round(b.TotalAmount, 2),
                                                   }).ToList();

                    EventData.ForEach(x => x.SubTotal = SubTotal);
                    EventData.ForEach(x => x.TaxAmount = Taxamount);
                    EventData.ForEach(x => x.GrandTotal = GrandTotal);

                    EventProforma confirm = new EventProforma(EventData, false, DateTime.Now, LocalBuffer.LocalBuffer.CompanyName, sluEventOwner.Text,
                           Convert.ToDateTime(teDate.EditValue.ToString()), lukEventCateg.Text, DateTime.Now, DateTime.Now);


                    confirm.PrintOption();
                }
                else
                {
                    XtraMessageBox.Show("There is no Event Requirement !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("There is no Event Detail !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnCashReceipt_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            Double GrandTotal = 0;
            VoucherDTO EventVoucher = UIProcessManager.GetVoucherByCode(teEventNo.Text);
            if (EventVoucher != null)
            {
                List<EventConsgineeDTO> ownersList = consigneeDTOList.Where(c => (c.gslType == CNETConstantes.GUEST || c.gslType == CNETConstantes.CUSTOMER)).ToList();
                frmCashReceipt CashReceipt = new frmCashReceipt(ownersList, EventVoucher, GrandTotal);
                CashReceipt.ShowDialog();
            }
            else
            {

                XtraMessageBox.Show("First Save The Event First ! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnBanquetOrder_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<EventDetaildataDTO> ALLEventDetailList = (List<EventDetaildataDTO>)gcEventDetail.DataSource;
            List<EventDetaildataDTO> EventDetailList = new List<EventDetaildataDTO>();
            if (ALLEventDetailList != null)
            {
                List<EventReqDTO> EventReqList = new List<EventReqDTO>();

                foreach (EventDetaildataDTO detail in ALLEventDetailList)
                {
                    List<EventReqDTO> EventReq = GetEventRequirements(detail.Id, detail.Code);
                    if (EventReq != null && EventReq.Count > 0)
                    {
                        EventReqList.AddRange(EventReq);
                        EventDetailList.Add(detail);
                    }
                }

                if (EventReqList != null && EventReqList.Count > 0)
                {
                    string VoucherNote = "";
                    List<EventReqDTO> DistinctEventReqList = EventReqList.GroupBy(x => new { x.VoucherCode })
                             .Select(x => x.FirstOrDefault()).ToList();
                    foreach (EventReqDTO DistinctEventReq in DistinctEventReqList)
                    {
                        if (DistinctEventReq.VoucherNote != null)
                        {
                            VoucherNote += DistinctEventReq.VoucherNote + Environment.NewLine + Environment.NewLine; ;
                        }
                    }
                    List<BanquetPrintOutDTO> EventData = (from eventdetail in EventDetailList
                                                          join eventreq in EventReqList
                                                   on eventdetail.Id equals eventreq.EventDetailid
                                                   into a
                                                          from b in a.DefaultIfEmpty()
                                                          join article in EventMgtForm.AllArticleList
                                                       on b.Articleid equals article.Id
                                                       into c
                                                          from d in c.DefaultIfEmpty()
                                                          join Ordermap in EventMgtForm.AllOrderStationMap
                                                         on d.ChildPreferenceId equals Ordermap.Preference
                                                                   into f
                                                          from g in f.DefaultIfEmpty()
                                                          select new BanquetPrintOutDTO
                                                          {
                                                              Voucher = eventdetail.Voucher,
                                                              TypeCode = eventdetail.TypeCode,
                                                              TypeDesc = eventdetail.TypeDesc,
                                                              Description = eventdetail.Description,
                                                              SpaceCode = eventdetail.SpaceCode,
                                                              Hall = eventdetail.Hall,
                                                              ArrangementDesc = eventdetail.ArrangementDesc,
                                                              ArrangementCode = eventdetail.Arrangementid,
                                                              StartTime = eventdetail.StartTime.ToString("HH:mm:ss"),
                                                              EndTime = eventdetail.EndTime.ToString("HH:mm:ss"),
                                                              NumOfPerson = eventdetail.NumOfPerson,
                                                              SN = b == null ? 0 : b.SN,
                                                              VoucherCode = b == null ? "" : b.VoucherCode,
                                                              ArticleCode = b == null ? "" : b.ArticleCode,
                                                              ArticleName = b == null ? "" : b.ArticleName,
                                                              Qty = b == null ? 0 : Math.Round(b.Qty, 2),
                                                              Department = g == null ? "Department" : g.Name,
                                                              LineItemNote = b == null ? "" : string.IsNullOrEmpty(b.LineItemNote) ? "" : "- " + b.LineItemNote,
                                                              LineItemCode = b == null ? 0 : b.LineItemCode

                                                          }).ToList();

                    EventData = EventData.OrderBy(x => x.StartTime).ToList();
                    EventData = EventData.GroupBy(x => x.LineItemCode).Select(x => x.FirstOrDefault()).ToList();
                    EventData.ForEach(x => x.VoucherNote = VoucherNote);

                    BanquetPrintOut Banquet = new BanquetPrintOut(EventData, true, DateTime.Now, LocalBuffer.LocalBuffer.CompanyName, sluEventOwner.Text, Convert.ToDateTime(teDate.EditValue.ToString()), lukEventCateg.Text, DateTime.Now, DateTime.Now);


                    Banquet.PrintOption();
                }
                else
                {
                    XtraMessageBox.Show("There is no Event Requirement !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("There is no Event Detail !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void sluAddNewPerson_AddNewValue(object sender, AddNewValueEventArgs e)
        {

        }



        private void sluEventOwner_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Caption == "Add")
            {
                if (((DevExpress.XtraEditors.SearchLookUpEdit)sender).Tag == "Owner")
                {
                    if (XtraMessageBox.Show("Is Owner Organization or Guest ?" + Environment.NewLine + "If Organization Press OK", "CNET_ERPV6", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
                    {
                        UIMaintainCustomer MiniOrganization = new UIMaintainCustomer(CNETConstantes.CUSTOMER);
                        MiniOrganization.ShowDialog();
                    }
                    else
                    {
                        UIMaintainCustomer MiniOrganization = new UIMaintainCustomer(CNETConstantes.GUEST);
                        MiniOrganization.ShowDialog();
                    }
                    if (UIMaintainCustomer.CreatedCustomer != null)
                    {
                        LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(UIMaintainCustomer.CreatedCustomer);

                        var newcus = new EventConsgineeDTO()
                        {
                            id = UIMaintainCustomer.CreatedCustomer.Id,
                            code = UIMaintainCustomer.CreatedCustomer.Code,
                            gslType = UIMaintainCustomer.CreatedCustomer.GslType,
                            IdentficationType = "TIN Number",
                            idNumber = UIMaintainCustomer.CreatedCustomer.Tin,
                            name = UIMaintainCustomer.CreatedCustomer.FirstName + " " + UIMaintainCustomer.CreatedCustomer.SecondName + " " + UIMaintainCustomer.CreatedCustomer.ThirdName
                        };
                        consigneeDTOList.Add(newcus);

                        var ownersList = consigneeDTOList.Where(c => (c.gslType == CNETConstantes.GUEST || c.gslType == CNETConstantes.CUSTOMER)).ToList();
                        sluEventOwner.Properties.DataSource = ownersList;
                        sluEventOwner.EditValue = UIMaintainCustomer.CreatedCustomer.Id;
                    }

                }
                else if (((DevExpress.XtraEditors.SearchLookUpEdit)sender).Tag == "Company")
                {
                    UIMaintainCustomer MiniPerson = new UIMaintainCustomer(CNETConstantes.CUSTOMER);
                    MiniPerson.ShowDialog();

                    if (UIMaintainCustomer.CreatedCustomer != null)
                    {
                        LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(UIMaintainCustomer.CreatedCustomer);

                        var newcus = new EventConsgineeDTO()
                        {
                            id = UIMaintainCustomer.CreatedCustomer.Id,
                            code = UIMaintainCustomer.CreatedCustomer.Code,
                            gslType = UIMaintainCustomer.CreatedCustomer.GslType,
                            IdentficationType = "TIN Number",
                            idNumber = UIMaintainCustomer.CreatedCustomer.Tin,
                            name = UIMaintainCustomer.CreatedCustomer.FirstName + " " + UIMaintainCustomer.CreatedCustomer.SecondName + " " + UIMaintainCustomer.CreatedCustomer.ThirdName
                        };
                        consigneeDTOList.Add(newcus);

                        var companyList = consigneeDTOList.Where(c => c.gslType == CNETConstantes.CUSTOMER).ToList();
                        sleCompany.Properties.DataSource = companyList;
                        sleCompany.EditValue = UIMaintainCustomer.CreatedCustomer.Id;

                        var ownersList = consigneeDTOList.Where(c => (c.gslType == CNETConstantes.GUEST || c.gslType == CNETConstantes.CUSTOMER)).ToList();
                        sluEventOwner.Properties.DataSource = ownersList; 
                    }

                }
                else if (((DevExpress.XtraEditors.SearchLookUpEdit)sender).Tag == "Agent")
                {
                    UIMaintainCustomer MiniPerson = new UIMaintainCustomer(CNETConstantes.AGENT);
                    MiniPerson.ShowDialog();

                    if (UIMaintainCustomer.CreatedCustomer != null)
                    {
                        LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(UIMaintainCustomer.CreatedCustomer);

                        var newcus = new EventConsgineeDTO()
                        {
                            id = UIMaintainCustomer.CreatedCustomer.Id,
                            code = UIMaintainCustomer.CreatedCustomer.Code,
                            gslType = UIMaintainCustomer.CreatedCustomer.GslType,
                            IdentficationType = "TIN Number",
                            idNumber = UIMaintainCustomer.CreatedCustomer.Tin,
                            name = UIMaintainCustomer.CreatedCustomer.FirstName + " " + UIMaintainCustomer.CreatedCustomer.SecondName + " " + UIMaintainCustomer.CreatedCustomer.ThirdName
                        };
                        consigneeDTOList.Add(newcus);

                        var ownersList = consigneeDTOList.Where(c => c.gslType == CNETConstantes.AGENT).ToList();
                        sluOrganizer.Properties.DataSource = ownersList;
                        sluOrganizer.EditValue = UIMaintainCustomer.CreatedCustomer.Id;
                    }

                }
                else if (((DevExpress.XtraEditors.SearchLookUpEdit)sender).Tag == "Source")
                {
                    UIMaintainCustomer MiniPerson = new UIMaintainCustomer(CNETConstantes.BUSINESSsOURCE);
                    MiniPerson.ShowDialog();

                    if (UIMaintainCustomer.CreatedCustomer != null)
                    {
                        LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(UIMaintainCustomer.CreatedCustomer);

                        var newcus = new EventConsgineeDTO()
                        {
                            id = UIMaintainCustomer.CreatedCustomer.Id,
                            code = UIMaintainCustomer.CreatedCustomer.Code,
                            gslType = UIMaintainCustomer.CreatedCustomer.GslType,
                            IdentficationType = "TIN Number",
                            idNumber = UIMaintainCustomer.CreatedCustomer.Tin,
                            name = UIMaintainCustomer.CreatedCustomer.FirstName + " " + UIMaintainCustomer.CreatedCustomer.SecondName + " " + UIMaintainCustomer.CreatedCustomer.ThirdName
                        };
                        consigneeDTOList.Add(newcus);

                        var ownersList = consigneeDTOList.Where(c => c.gslType == CNETConstantes.BUSINESSsOURCE).ToList();
                        sluSource.Properties.DataSource = ownersList;
                        sluSource.EditValue = UIMaintainCustomer.CreatedCustomer.Id;
                    }
                }
                else if (((DevExpress.XtraEditors.SearchLookUpEdit)sender).Tag == "Contact")
                {
                    UIMaintainCustomer MiniPerson = new UIMaintainCustomer(CNETConstantes.CONTACT);
                    MiniPerson.ShowDialog();
                    if (UIMaintainCustomer.CreatedCustomer != null)
                    {
                        LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(UIMaintainCustomer.CreatedCustomer);

                        var newcus = new EventConsgineeDTO()
                        {
                            id = UIMaintainCustomer.CreatedCustomer.Id,
                            code = UIMaintainCustomer.CreatedCustomer.Code,
                            gslType = UIMaintainCustomer.CreatedCustomer.GslType,
                            IdentficationType = "TIN Number",
                            idNumber = UIMaintainCustomer.CreatedCustomer.Tin,
                            name = UIMaintainCustomer.CreatedCustomer.FirstName + " " + UIMaintainCustomer.CreatedCustomer.SecondName + " " + UIMaintainCustomer.CreatedCustomer.ThirdName
                        };
                        consigneeDTOList.Add(newcus);

                        var ownersList = consigneeDTOList.Where(c => c.gslType == CNETConstantes.CONTACT).ToList();
                        sluContact.Properties.DataSource = ownersList;
                        sluContact.EditValue = UIMaintainCustomer.CreatedCustomer.Id;
                    }
                }

            }
        }

        private void btnPrintAllConformation_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<EventDetaildataDTO> EventDetailList = (List<EventDetaildataDTO>)gcEventDetail.DataSource;

            if (EventDetailList != null)
            {

                List<EventReqDTO> EventReqList = new List<EventReqDTO>();
                decimal SubTotal = 0;
                decimal Taxamount = 0;
                decimal GrandTotal = 0;


                foreach (EventDetaildataDTO detail in EventDetailList)
                {
                    List<EventReqDTO> EventReq = GetEventRequirements(detail.Id, detail.Code);
                    if (EventReq != null && EventReq.Count > 0)
                    {
                        EventReqList.AddRange(EventReq);
                        List<int> vouid = EventReq.Select(x => x.VoucherId.Value).Distinct().ToList();

                        foreach (int vid in vouid)
                        {
                            var VoucherData = UIProcessManager.GetVoucherById(vid);
                            var dd = EventReq.Where(x => x.VoucherId == vid);
                            if (VoucherData != null)
                            {
                                SubTotal += VoucherData.SubTotal;
                                Taxamount += dd.Sum(x => x.TaxAmount.Value);
                                GrandTotal += VoucherData.GrandTotal;
                            }

                        }
                    }
                }
                // if (EventReqList != null && EventReqList.Count > 0)
                // {
                List<PrintOutDTO> EventData = (from eventdetail in EventDetailList
                                               join eventreq in EventReqList
                                        on eventdetail.Id equals eventreq.EventDetailid
                                        into a
                                               from b in a.DefaultIfEmpty()
                                               select new PrintOutDTO
                                               {
                                                   Code = eventdetail.Code,
                                                   Voucher = eventdetail.Voucher,
                                                   TypeCode = eventdetail.TypeCode,
                                                   TypeDesc = eventdetail.TypeDesc,
                                                   Description = eventdetail.Description,
                                                   SpaceCode = eventdetail.SpaceCode,
                                                   Hall = eventdetail.Hall,
                                                   ArrangementDesc = eventdetail.ArrangementDesc,
                                                   ArrangementCode = eventdetail.Arrangementid,
                                                   StartTime = eventdetail.StartTime,
                                                   EndTime = eventdetail.EndTime,
                                                   NumOfPerson = eventdetail.NumOfPerson,
                                                   SN = b == null ? 0 : b.SN,
                                                   VoucherCode = b == null ? "" : b.VoucherCode,
                                                   ArticleCode = b == null ? "" : b.ArticleCode,
                                                   ArticleName = b == null ? "" : b.ArticleName,
                                                   Qty = b == null ? 0 : b.Qty,
                                                   UnitAmount = b == null ? 0 : Math.Round(b.UnitAmount, 2),
                                                   TotalAmount = b == null ? 0 : Math.Round(b.TotalAmount, 2),
                                               }).ToList();

                EventData.ForEach(x => x.SubTotal = SubTotal);
                EventData.ForEach(x => x.TaxAmount = Taxamount);
                EventData.ForEach(x => x.GrandTotal = GrandTotal);






                EventProforma confirm = new EventProforma(EventData, true, DateTime.Now, LocalBuffer.LocalBuffer.CompanyName, sluEventOwner.Text,
                       Convert.ToDateTime(teDate.EditValue.ToString()), lukEventCateg.Text, DateTime.Now, DateTime.Now);


                confirm.PrintOption();
                //}
                //else
                //{
                //    XtraMessageBox.Show("There is no Event Requirement !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}

            }
            else
            {
                XtraMessageBox.Show("There is no Event Detail !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnPrintSelectedConformation_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EventDetaildataDTO SelectedEventDetail = (EventDetaildataDTO)gvEventDetail.GetFocusedRow();

            if (SelectedEventDetail != null)
            {
                List<EventDetaildataDTO> EventDetailList = new List<EventDetaildataDTO> { SelectedEventDetail };
                List<EventReqDTO> EventReqList = new List<EventReqDTO>();
                decimal ServiceCharge = 0;
                decimal Discount = 0;
                decimal Taxamount = 0;
                decimal SubTotal = 0;
                decimal GrandTotal = 0;
                ConsigneeDTO Company = null;


                foreach (EventDetaildataDTO detail in EventDetailList)
                {
                    List<EventReqDTO> EventReq = GetEventRequirements(detail.Id, detail.Code);
                    if (EventReq != null && EventReq.Count > 0)
                    {
                        EventReqList.AddRange(EventReq);
                        List<int> vouid = EventReq.Select(x => x.VoucherId.Value).Distinct().ToList();

                        foreach (int vid in vouid)
                        {
                            var VoucherData = UIProcessManager.GetVoucherById(vid);
                            var dd = EventReq.Where(x => x.VoucherId == vid);
                            if (VoucherData != null)
                            {
                                ServiceCharge += VoucherData.AddCharge;
                                Discount += VoucherData.Discount;
                                SubTotal += VoucherData.SubTotal;
                                Taxamount += dd.Sum(x => x.TaxAmount.Value);
                                GrandTotal += VoucherData.GrandTotal;
                            }

                        }
                    }
                }
                if (EventReqList != null && EventReqList.Count > 0)
                {
                    List<BanquetDTO> EventData = (from eventdetail in EventDetailList
                                                  join eventreq in EventReqList
                                           on eventdetail.Id equals eventreq.EventDetailid
                                           into a
                                                  from b in a.DefaultIfEmpty()
                                                  select new BanquetDTO
                                                  {
                                                      RequirementCode = b == null ? "" : b.VoucherCode,
                                                      ArticleName = b == null ? "" : b.ArticleName,
                                                      Quantity = b == null ? 0 : b.Qty,
                                                      Venue = eventdetail.Hall,
                                                      Price = b == null ? 0 : Math.Round(b.UnitAmount, 2),
                                                      TotalPrice = b == null ? 0 : Math.Round(b.TotalAmount, 2),
                                                  }).ToList();

                    string Halldata = "";

                    VoucherDTO vodata = UIProcessManager.GetVoucherById(_currentEventHeaderId);
                    UserDTO user = LocalBuffer.LocalBuffer.UserBufferList.FirstOrDefault(x => x.Id == vodata.LastUser);

                    if (vodata.Consignee2 != null)
                        Company = UIProcessManager.GetConsigneeById(vodata.Consignee2.Value);

                    EventHeaderView eventheader = UIProcessManager.GetEventHeaderViewById(_currentEventHeaderId);
                    if (eventheader != null && EventData != null && EventData.Count > 0)
                    {
                        EventData.ForEach(x => x.PartyName = eventheader.consigneeName);
                        if (Company != null)
                        {
                            EventData.ForEach(x => x.CompanyName = Company.FirstName);
                            EventData.ForEach(x => x.TIN = Company.Tin);
                        }
                        EventData.ForEach(x => x.Function = eventheader.bookingTypeDesc);
                        EventData.ForEach(x => x.EventCode = eventheader.Code);
                        EventData.ForEach(x => x.ExpectedPax = vodata.Extension4);
                        EventData.ForEach(x => x.GuarentedPax = vodata.Extension5);
                        EventData.ForEach(x => x.User = user == null ? "Unknown User" : user.UserName);

                    }


                    EventData.ForEach(x => x.ServiceCharge = ServiceCharge);
                    EventData.ForEach(x => x.Discount = Discount);
                    EventData.ForEach(x => x.Subtotal = SubTotal);
                    EventData.ForEach(x => x.TaxAmount = Taxamount);
                    EventData.ForEach(x => x.GrandTotal = GrandTotal);



                    BanquetPrint confirm = new BanquetPrint(EventData);
                    confirm.PrintOption();
                }
                else
                {
                    XtraMessageBox.Show("There is no Event Requirement !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("Please Select Event Detail !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        private void txtCalculate_EditValueChanged(object sender, EventArgs e)
        {
            ValueChangedCalculation();
        }
        private void nudGuaranteedGuest_ValueChanged(object sender, EventArgs e)
        {
            ValueChangedCalculation();

            if (nudExpectedGuest.Value < nudGuaranteedGuest.Value)
                nudExpectedGuest.Value = nudGuaranteedGuest.Value;
        }

        public void CalculateEventVoucher(decimal pax, decimal paxrate, decimal HallRate)
        {

            if (pax > 0 && (paxrate > 0 || HallRate > 0))
            {

                lineItemDetails = new List<LineItemDetails>();
                VoucherFinal = new VoucherFinalDTO();
                if (paxrate > 0 && PaxArticle == null)
                {
                    XtraMessageBox.Show("There is no Rate/Pax Article !!");
                    return;
                }
                else if (paxrate > 0 && PaxArticle != null) 
                {
                    LineItemDTO line = new LineItemDTO()
                    {
                        Article = PaxArticle.Id,
                        UnitAmount = paxrate,
                        Uom = PaxArticle.Uom,
                        Quantity = pax
                    };
                    LineItemCalculatorDTO ll = new LineItemCalculatorDTO()
                    {
                        voucherDefintion = CNETConstants.GOODS_RESERVATION_VOUCHER,
                        consignee = null,
                        prferentialValueFactorDefn = null,
                        currentLineItem = line,
                        discountValueFactorCode = null,
                        additionalChargeValueFactorCode = null,
                        additionalChargeParam = null,
                        discountParam = null,
                        priceExtracted = true,
                        isPercentDiscount = false,
                        isPercentAdditionalCharge = false,
                        scIncluded = false,
                        discountChecked = true,
                        additionalChargeChecked = true,
                        flexibleTaxChecked = true,
                        consigneeUnit = null

                    };

                    var processorDiscount = UIProcessManager.Get_Value_Factor_Definition_By_Reference_And_Type(PaxArticle.Id, CNETConstantes.Discount);

                    var processorServiceCharge = UIProcessManager.Get_Value_Factor_Definition_By_Reference_And_Type(PaxArticle.Id, CNETConstantes.ADDTIONAL_CHARGE);

                    //int? DiscountId = null;
                    //decimal DiscountValue = 0;

                    //if (processorDiscount != null && processorDiscount.Count > 0)
                    //{
                    //    DiscountId = processorDiscount.FirstOrDefault().Id;
                    //    DiscountValue = processorDiscount.FirstOrDefault().Value;
                    //}

                    //int? ServicechargeId = null;
                    //decimal ServicechargeValue = 0;
                    //if (processorServiceCharge != null && processorServiceCharge.Count > 0)
                    //{
                    //    ServicechargeId = processorServiceCharge.FirstOrDefault().Id;
                    //    ServicechargeValue = processorServiceCharge.FirstOrDefault().Value;
                    //}


                    LineItemDTO lineItem = new LineItemDTO();
                    lineItem.Quantity = pax;
                    lineItem.Article = PaxArticle.Id;
                    lineItem.UnitAmount = paxrate;
                    lineItem.Tax = PaxArticle.DefaultTax;
                    lineItem.ObjectState = null;

                    LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(EventVoucherBuffer, lineItem, null, null,null,  null, null, true, false, false, false);
                    LineItemDetails liDetails = new LineItemDetails()
                    {
                        articleName = PaxArticle == null ? "Unknown Article" : PaxArticle.Name,
                        lineItems = liDetail.lineItem,
                        lineItemValueFactor = liDetail.lineItemValueFactor
                    };

                    lineItemDetails.Add(liDetails);

                }

                if (HallRate > 0 && HallArticle == null)
                {
                    XtraMessageBox.Show("There is no Hall Charge Article !!");
                    return;
                }
                else if (HallRate > 0 && HallArticle != null)
                {

                    LineItemDTO line = new LineItemDTO()
                    {
                        Article = HallArticle.Id,
                        UnitAmount = HallRate,
                        Uom = HallArticle.Uom,
                        Quantity = 1
                    };
                    LineItemCalculatorDTO ll = new LineItemCalculatorDTO()
                    {
                        voucherDefintion = CNETConstants.GOODS_RESERVATION_VOUCHER,
                        consignee = null,
                        prferentialValueFactorDefn = null,
                        currentLineItem = line,
                        discountValueFactorCode = null,
                        additionalChargeValueFactorCode = null,
                        additionalChargeParam = null,
                        discountParam = null,
                        priceExtracted = true,
                        isPercentDiscount = false,
                        isPercentAdditionalCharge = false,
                        scIncluded = false,
                        discountChecked = true,
                        additionalChargeChecked = true,
                        flexibleTaxChecked = true,
                        consigneeUnit = null

                    };
                   

                    LineItemDTO lineItem = new LineItemDTO();
                    lineItem.Quantity = 1;
                    lineItem.Article = HallArticle.Id;
                    lineItem.UnitAmount = HallRate;
                    lineItem.Tax = HallArticle.DefaultTax;
                    lineItem.ObjectState = null;

                    LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(EventVoucherBuffer, lineItem, null, null, null, null, null, true, false, false, false);
                    LineItemDetails liDetails = new LineItemDetails()
                    {
                        articleName = HallArticle == null ? "Unknown Article" : HallArticle.Name,
                        lineItems = liDetail.lineItem,
                        lineItemValueFactor = liDetail.lineItemValueFactor
                    };

                    lineItemDetails.Add(liDetails);
                }
                if (lineItemDetails != null && lineItemDetails.Count > 0)
                    VoucherFinal = new VoucherFinalCalculator().VoucherCalculation(EventVoucherBuffer.Voucher, lineItemDetails);
            }
            DisplayTotalCharge();
        }

        public void DisplayTotalCharge()
        {
            string ChargeText = "";
            if (VoucherFinal != null)
            {

                decimal hallcharge = 0;
                decimal ratepax = 0;
                decimal GuaranteedGuest = nudGuaranteedGuest.Value;

                string HallCharge = txtHallCharge.EditValue == null ? "0" : txtHallCharge.EditValue.ToString();
                string RatePax = txtRatePax.EditValue == null ? "0" : txtRatePax.EditValue.ToString();

                decimal.TryParse(HallCharge, out hallcharge);
                decimal.TryParse(RatePax, out ratepax);

                if (GuaranteedGuest > 0 && (ratepax > 0 || hallcharge > 0))
                {

                    if (ratepax > 0 && hallcharge > 0)
                    {
                        hallcharge = Math.Round(hallcharge, UnitPriceRoundDigit);
                            ratepax = Math.Round(ratepax, UnitPriceRoundDigit);

                        ChargeText = string.Format("({0} pax * {1}) + ({2} Hall Charge) = {3} Total Charge", GuaranteedGuest, ratepax.ToString(), hallcharge.ToString(), VoucherFinal.voucher.GrandTotal.ToString("N3"));
                    }
                    else if (ratepax > 0 && hallcharge == 0)
                    {
                        ratepax = Math.Round(ratepax, UnitPriceRoundDigit);
                        ChargeText = string.Format("({0} pax * {1}) = {2} Total Charge", GuaranteedGuest, ratepax.ToString(), VoucherFinal.voucher.GrandTotal.ToString("N3"));
                    }
                    else if (ratepax == 0 && hallcharge > 0)
                    {
                        hallcharge = Math.Round(hallcharge, UnitPriceRoundDigit);

                        ChargeText = string.Format("({0} Hall Charge) = {1} Total Charge", hallcharge.ToString(), VoucherFinal.voucher.GrandTotal.ToString("N3"));
                    }
                    else
                        ChargeText = "";
                }
                else
                    ChargeText = "";
            }
            txtTotalCharge.Text = ChargeText;
        }


        public void ValueChangedCalculation()
        {

            decimal hallcharge = 0;
            decimal ratepax = 0;
            decimal GuaranteedGuest = nudGuaranteedGuest.Value;

            string HallCharge = txtHallCharge.EditValue == null ? "0" : txtHallCharge.EditValue.ToString();
            string RatePax = txtRatePax.EditValue == null ? "0" : txtRatePax.EditValue.ToString();

            decimal.TryParse(HallCharge, out hallcharge);
            decimal.TryParse(RatePax, out ratepax);

            CalculateEventVoucher(GuaranteedGuest, ratepax, hallcharge);
        }

        private void btnEventPrintOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EventHeaderView eventheader = UIProcessManager.GetEventHeaderViewById(_currentEventHeaderId);
            if (eventheader != null)
            {
                VoucherDTO vodata = UIProcessManager.GetVoucherById(_currentEventHeaderId);
                string Halldata = "";
                string time = "";
                string SeatArrangemet = "";
                string Menus = "";
                string PaymentMethod = "";
                VwConsigneeViewDTO contactperson = null;
                ConsigneeDTO Company = null;


                UserDTO user = LocalBuffer.LocalBuffer.UserBufferList.FirstOrDefault(x => x.Id == vodata.LastUser);

                if (vodata.Consignee5 != null)
                    contactperson = UIProcessManager.GetConsigneeViewById(vodata.Consignee5.Value);



                if (vodata.Consignee2 != null)
                    Company = UIProcessManager.GetConsigneeById(vodata.Consignee2.Value);

                if (vodata.PaymentMethod != null)
                {
                    SystemConstantDTO payment = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == vodata.PaymentMethod.Value);
                    if (payment != null)
                        PaymentMethod = payment.Description;
                }


                List<EventDisplayView> Eventdetail = UIProcessManager.GetEventDisplayView(_currentEventHeaderId);
                if (Eventdetail != null && Eventdetail.Count > 0)
                {
                    List<string> halllist = Eventdetail.Select(x => x.HallDescription).Distinct().ToList();
                    if (Eventdetail != null && Eventdetail.Count > 0)
                    {
                        Halldata = string.Join(" , ", halllist);
                    }

                    time = Eventdetail.FirstOrDefault().startTimeStamp.ToString("HH:mm") + " To " + Eventdetail.FirstOrDefault().endTimeStamp.ToString("HH:mm");

                    SeatArrangemet = Eventdetail.FirstOrDefault().SpaceArrangementDes;

                    foreach (EventDisplayView detail in Eventdetail)
                    {
                        List<EventRequirementView> requirement = UIProcessManager.GetEventRequirementView(detail.EventDetailId);
                        if (requirement != null && requirement.Count > 0)
                        {
                            List<string> ArticleList = requirement.Select(x => x.ArtName).Distinct().ToList();
                            if (ArticleList != null && ArticleList.Count > 0)
                            {
                                Menus = string.Join("<br>", ArticleList);
                            }
                        }
                    }
                }


                string DepartmentInstructions = "";
                List<VoucherLookupListDTO> lookupListDTOs = UIProcessManager.GetVoucherLookupListByVoucher(_currentEventHeaderId);
                if (lookupListDTOs != null && lookupListDTOs.Count > 0)
                {
                    foreach (VoucherLookupListDTO dTO in lookupListDTOs)
                    {
                        LookupDTO lookup = LocalBuffer.LocalBuffer.LookUpBufferList.FirstOrDefault(x => x.Id == dTO.SelectedLookup);
                        if (lookup != null)
                        {
                            DepartmentInstructions += @"<style>
                                                            .medium-font { font-size: 14px; }
                                                            .small-font { font-size: 12px; }
                                                        </style>";
                            DepartmentInstructions += @"<div>";
                            DepartmentInstructions += "<b  class=\"medium-font\">" + lookup.Description + "</b><br>";
                            DepartmentInstructions += "<span class=\"small-font\">" + dTO.Remark + "</span>" + "<br>";
                            DepartmentInstructions += "<span class=\"small-font\">" + "Signature: __________________________" + "</span>" + "<br>"; 
                            DepartmentInstructions += @"</div>";
                        }
                    }
                }

                EventPrintDTO printdto = new EventPrintDTO()
                {
                    EventCode = eventheader.Code,
                    Eventdate = eventheader.startDate?.ToString("dd/MM/yy") + " To " + eventheader.endDate?.ToString("dd/MM/yy"),
                    Eventtime = time,
                    Eventtype = eventheader.eventCategDesc,
                    Venue = Halldata,
                    pax = vodata.Extension3,
                    PaxRate = "Rate/Pax : " + vodata.Extension4,
                    HallRate = "Hall Charge : " + vodata.Extension5,
                    PartyName = eventheader.consigneeName,
                    CompanyName = Company == null ? "" : Company.FirstName,
                    ContactPerson = contactperson == null ? "" : contactperson.FullName,
                    ContactAddress = contactperson == null ? "" : contactperson.AddressLine1,
                    ContactPhone = contactperson == null ? "" : contactperson.Phone1,
                    BookedBy = user == null ? "Unknown User" : user.UserName,
                    Billing = "",
                    SeatArrangement = SeatArrangemet,
                    Menu = Menus,
                    DepartmentInstruction = DepartmentInstructions,
                    SpecialInstruction = eventheader.remark,
                    ConferenceFacilities = vodata.DefaultImageUrl,
                    PaymentMethod = PaymentMethod
                };

                EventPrint confirm = new EventPrint(printdto);
                confirm.PrintOption();
            }
        }

        private void cmbBillingType_EditValueChanged(object sender, EventArgs e)
        {
            if(cmbBillingType.EditValue != null && cmbBillingType.EditValue.ToString().ToLower() == "cash")
            {
                lkPaymentMethod.EditValue = CNETConstantes.PAYMENTMETHODSCASH;
                lkPaymentMethod.Enabled = false;
            }
            else
            {
                lkPaymentMethod.EditValue = 1757;
                lkPaymentMethod.Enabled = true;
            }
        }
    }
    public class EventDepartSelection
    {
        public bool selected { get; set; }
        public int id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }

}
