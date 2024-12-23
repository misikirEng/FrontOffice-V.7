using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET.FrontOffice_V._7;
using CNET.FrontOffice_V._7.Forms;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.TransactionSchema;
using DevExpress.CodeParser;
using CNET_V7_Domain.Misc.PmsDTO;
using System.Reflection.Emit;
using DevExpress.PivotGrid.OLAP.AdoWrappers;
using CNET_V7_Domain.Misc;

namespace CNET.FrontOffice_V._7.Group_Registration
{
    public partial class frmGroupRegistration : UILogicBase
    {
        private int _defPaymentMethod;

        private int _adRateAdjust;
        private int _adCheckin;
        private int _adSixPm;
        private int _adWaiting;
        private int _adGuaranteed;
        private int _selectedRoomType;

        private int _actDefCompany;
        private int _actDefPerson;

        private List<RateCodeHeaderDTO> _rateHeaderList;

        private List<RegistrationDocumentDTO> _regVMList = new List<RegistrationDocumentDTO>();

        private frmRateSearch _rateSearchForm;
        private frmRateSearch _RoomDefulatrateSearchForm;
        private frmRateSearch _CellrateSearchForm;
        private frmNewCompany _frmNewCompany;
        private frmNewGuest _frmNewGuest;

        private RateCodeHeaderDTO _selectedRateHeader;
        private decimal _currentRateAmount;
        private decimal _currentDefaultRateAmount;
        public SelectedRoom SelectedRoomHandler { get; set; }
        public SelectedRoomList SelectedRoomListHandler { get; set; }

        private int? _currentRoomCode;
        private int? _currentRoomType;

        private List<RoomTypeDTO> _allRoomTypes;

        private List<PackageHeaderDTO> _availalbePackages = null;
        private List<DailyRateCodeDTO> dailyRateCodeList = new List<DailyRateCodeDTO>();


        private List<AddedCompany> _addedCompanies = new List<AddedCompany>();
        private List<AddedGuest> _addedGuests = new List<AddedGuest>();
        public DateTime CheckOutDateTime { get; set; }

        /************************************ CONSTRUCTOR *******************/
        public frmGroupRegistration()
        {
            InitializeComponent();

            InitializeUI();
            ConfigurationDTO Checkoutconfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_CheckOutTime);
            if (Checkoutconfig != null)
            {
                CheckOutDateTime = Convert.ToDateTime(Checkoutconfig.CurrentValue);
            }
            gvGroupReg.OptionsBehavior.Editable = false;

        }

        #region Helper Methods

        private void InitializeUI()
        {
            //State
            repoLukStates.Columns.Add(new LookUpColumnInfo("Description", "State"));
            repoLukStates.DisplayMember = "Description";
            repoLukStates.ValueMember = "Id";

            //Existing Registrations 
            GridColumn colReg = sluRegistrations.Properties.View.Columns.AddField("Id");
            colReg.Visible = true;
            colReg = sluRegistrations.Properties.View.Columns.AddField("name");
            colReg.Visible = true;
            colReg = sluRegistrations.Properties.View.Columns.AddField("RoomTypeDescription");
            colReg.Visible = true;
            colReg.Caption = "Room Type";
            colReg = sluRegistrations.Properties.View.Columns.AddField("RoomNumber");
            colReg.Visible = true;
            sluRegistrations.Properties.DisplayMember = "name";
            sluRegistrations.Properties.ValueMember = "Id";


            //Group 
            GridColumn colGroup = sluGroup.Properties.View.Columns.AddField("Id");
            colGroup.Visible = false;
            colGroup = slueGuest.Properties.View.Columns.AddField("Code");
            colGroup.Visible = true;
            colGroup = sluGroup.Properties.View.Columns.AddField("FirstName");
            colGroup.Caption = "First Name";
            colGroup.Visible = true;
            colGroup = sluGroup.Properties.View.Columns.AddField("SecondName");
            colGroup.Caption = "Middle Name";
            colGroup.Visible = true;
            colGroup = sluGroup.Properties.View.Columns.AddField("Tin");
            colGroup.Visible = true;
            sluGroup.Properties.DisplayMember = "FirstName";
            sluGroup.Properties.ValueMember = "Id";

            //Guest 
            GridColumn column = slueGuest.Properties.View.Columns.AddField("Id");
            column.Visible = false;
            column = slueGuest.Properties.View.Columns.AddField("Code");
            column.Visible = true;
            column = slueGuest.Properties.View.Columns.AddField("FirstName");
            column.Caption = "First Name";
            column.Visible = true;
            column = slueGuest.Properties.View.Columns.AddField("SecondName");
            column.Caption = "Middle Name";
            column.Visible = true;
            column = slueGuest.Properties.View.Columns.AddField("Tin");
            column.Visible = true;
            slueGuest.Properties.DisplayMember = "FirstName";
            slueGuest.Properties.ValueMember = "Id";



            //Default Guest 
            GridColumn Defaultcolumn = sleDefaultGuest.Properties.View.Columns.AddField("Id");
            Defaultcolumn.Visible = false;
            Defaultcolumn = sleDefaultGuest.Properties.View.Columns.AddField("Code");
            Defaultcolumn.Visible = true;
            Defaultcolumn = sleDefaultGuest.Properties.View.Columns.AddField("FirstName");
            Defaultcolumn.Caption = "First Name";
            Defaultcolumn.Visible = true;
            Defaultcolumn = sleDefaultGuest.Properties.View.Columns.AddField("SecondName");
            Defaultcolumn.Caption = "Middle Name";
            Defaultcolumn.Visible = true;
            Defaultcolumn = sleDefaultGuest.Properties.View.Columns.AddField("Tin");
            Defaultcolumn.Visible = true;
            sleDefaultGuest.Properties.DisplayMember = "FirstName";
            sleDefaultGuest.Properties.ValueMember = "Id";


            GridColumn columnGuest = repositoryItemSearchLookUpEditGuest.View.Columns.AddField("Id");
            columnGuest.Visible = true;
            columnGuest = repositoryItemSearchLookUpEditGuest.View.Columns.AddField("FirstName");
            columnGuest.Caption = "First Name";
            columnGuest.Visible = true;
            columnGuest = repositoryItemSearchLookUpEditGuest.View.Columns.AddField("SecondName");
            columnGuest.Caption = "Middle Name";
            columnGuest.Visible = true;
            columnGuest = repositoryItemSearchLookUpEditGuest.View.Columns.AddField("BioId");
            columnGuest.Caption = "Id Number";
            columnGuest.Visible = true;
            repositoryItemSearchLookUpEditGuest.DisplayMember = "FirstName";
            repositoryItemSearchLookUpEditGuest.ValueMember = "Id";


            //Company
            GridColumn columnCompany = sluCustomer.Properties.View.Columns.AddField("Id");
            columnCompany.Visible = false;
            columnCompany = sluCustomer.Properties.View.Columns.AddField("Code");
            columnCompany.Visible = true;
            columnCompany = sluCustomer.Properties.View.Columns.AddField("FirstName");
            columnCompany.Caption = "First Name";
            columnCompany.Visible = true;
            columnCompany = sluCustomer.Properties.View.Columns.AddField("SecondName");
            columnCompany.Caption = "Middle Name";
            columnCompany.Visible = true;
            columnCompany = sluCustomer.Properties.View.Columns.AddField("Tin");
            columnCompany.Visible = true;
            sluCustomer.Properties.DisplayMember = "FirstName";
            sluCustomer.Properties.ValueMember = "Id";



            memoPurposeTravel.Properties.DisplayMember = "Description";
            memoPurposeTravel.Properties.ValueMember = "Id";



            //Payment Method
            lukPayment.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            lukPayment.Properties.DisplayMember = "Description";
            lukPayment.Properties.ValueMember = "Id";

            SelectedRoomHandler = SelectedRoomEventHandler;
            SelectedRoomListHandler = SelectedRoomListEventHandler;
        }

        private bool InitializeData()
        {
            try
            {

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }


                reiHotel.DisplayMember = "Name";
                reiHotel.ValueMember = "Id";
                reiHotel.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name }).ToList();

                if (LocalBuffer.LocalBuffer.HotelBranchBufferList != null && LocalBuffer.LocalBuffer.HotelBranchBufferList.Count > 0)
                {
                    beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                    reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;
                }

                teDate.EditValue = CurrentTime.Value;

                // Progress_Reporter.Show_Progress("Loading Data", "Please Wait...");

                //check workflow
                #region Check Workflow 

                // Progress_Reporter.Show_Progress("Getting Company Maintained workflow", "Please Wait...");
                /** Company **/
                ActivityDefinitionDTO workFlowCompany = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.MAINTAINED, CNETConstantes.CUSTOMER).FirstOrDefault();

                if (workFlowCompany != null)
                {

                    _actDefCompany = workFlowCompany.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of MAINTAINED for Customer (Organization).", "ERROR");
                    return false;
                }


                // Progress_Reporter.Show_Progress("Getting Guest Maintained workflow", "Please Wait...");
                /** Person **/
                ActivityDefinitionDTO workflowPerson = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.MAINTAINED, CNETConstantes.GUEST).FirstOrDefault();

                if (workflowPerson != null)
                {

                    _actDefPerson = workflowPerson.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of MAINTAINED for Person.", "ERROR");
                    return false;
                }

                ActivityDefinitionDTO workFlowRateAdjust = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_RATE_ADJUSTED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlowRateAdjust != null)
                {

                    _adRateAdjust = workFlowRateAdjust.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of RATE ADJUSTED for Registration Voucher ", "ERROR");
                    return false;
                }




                //check-in workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_CheckIN, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    _adCheckin = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of CHECK-IN for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(_adCheckin).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                    if (roleActivity != null)
                    {
                        frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            ////CNETInfoReporter.Hide();
                            return false;
                        }

                    }

                }

                //Six PM
                ActivityDefinitionDTO workFlowSixPM = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_6PM, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlowSixPM != null)
                {

                    _adSixPm = workFlowSixPM.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of SIX PM for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper2 = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper2 != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(_adSixPm).FirstOrDefault(r => r.Role == userRoleMapper2.Role && r.NeedsPassCode);
                    if (roleActivity != null)
                    {
                        frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            ////CNETInfoReporter.Hide();
                            return false;
                        }

                    }

                }

                //Waiting List
                ActivityDefinitionDTO workFlowWaitingList = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Waitinglist, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlowWaitingList != null)
                {

                    _adWaiting = workFlowWaitingList.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of WAITING LIST for Registration Voucher ", "ERROR");
                    return false;
                }

                //Guaranted
                ActivityDefinitionDTO workFlowGuaranteed = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_Guaranteed, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlowGuaranteed != null)
                {

                    _adGuaranteed = workFlowGuaranteed.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of GUARANTEED for Registration Voucher ", "ERROR");
                    return false;
                }

                #endregion

                //All Room Types
                _allRoomTypes = UIProcessManager.SelectAllRoomType();
                if (_allRoomTypes == null || _allRoomTypes.Count == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("No room type list is found", "ERROR");
                    return false;
                }

                //States
                var states = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(s =>
                 s.Id == CNETConstantes.SIX_PM_STATE ||
                 s.Id == CNETConstantes.OSD_WAITLIST_STATE ||
                 s.Id == CNETConstantes.GAURANTED_STATE ||
                 s.Id == CNETConstantes.CHECKED_IN_STATE
                 ).ToList();
                repoLukStates.DataSource = states;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;

                // Rate Headers
                _rateHeaderList = UIProcessManager.GetRateCodeHeaderByconsigneeunit(SelectedHotelcode.Value);
                if (_rateHeaderList == null || _rateHeaderList.Count == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define rate first.", "ERROR");
                    return false;
                }

                //Payment Methods
                List<SystemConstantDTO> paymentList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.PAYMENT_METHODS && l.IsActive).ToList();
                if (paymentList != null)
                {
                    paymentList = paymentList.Where(
                            r =>
                                r.Id == CNETConstantes.PAYMENTMETHODSCASH
                                //|| r.code == CNETConstantes.PAYMENTMETHODS_DIRECT_BILL
                                ).ToList();
                    lukPayment.Properties.DataSource = (paymentList.OrderByDescending(c => c.IsDefault).ToList());
                    var paymentDefault = paymentList.FirstOrDefault(c => c.IsDefault);
                    if (paymentDefault != null)
                    {
                        lukPayment.EditValue = (paymentDefault.Id);
                        _defPaymentMethod = paymentDefault.Id;
                    }
                }

                //Date Properties
                deArrivalDate.Properties.MinValue = CurrentTime.Value;
                deDepartureDate.Properties.MinValue = CurrentTime.Value;
                deChildArrival.Properties.MinValue = CurrentTime.Value;
                deChildDeparture.Properties.MinValue = CurrentTime.Value;
                deDefaultArrivalDate.Properties.MinValue = CurrentTime.Value;
                deDefaultDepartureDate.Properties.MinValue = CurrentTime.Value;


                //Exsisting Registration List
                _regVMList = UIProcessManager.GetRegistrationDTOData(null, null, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode.Value);
                sluRegistrations.Properties.DataSource = _regVMList;


                sluGroup.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                slueGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                sleDefaultGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                repositoryItemSearchLookUpEditGuest.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;

                //Company List
                sluCustomer.Properties.DataSource = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist;
                //Purpose of Travel
                List<LookupDTO> PurposeList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.TravelPurpose && l.IsActive).ToList();
                memoPurposeTravel.Properties.DataSource = PurposeList;
                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing group registration. DETAIL:: " + ex.Message, "ERROR");
                return false;
            }
        }


        private void AddRegistration()
        {


            if (sluGroup.EditValue == null || string.IsNullOrEmpty(sluGroup.EditValue.ToString()))
            {
                SystemMessage.ShowModalInfoMessage("Please select master guest first!", "ERROR");
                return;
            }



            List<GroupVM> addedDtoList = gvGroupReg.DataSource as List<GroupVM>;
            if (addedDtoList == null)
                addedDtoList = new List<GroupVM>();



            GroupVM dto = null;
            if (tcRegistration.SelectedTabPage == tpNewRegistrations)
            {
                //Check Existing Added Reg with the same room number
                var RoomAdded = addedDtoList.FirstOrDefault(r => r.RoomCode == _currentRoomCode);
                if (RoomAdded != null)
                {
                    SystemMessage.ShowModalInfoMessage(string.Format("Room Number {0} is already added", RoomAdded.Room), "ERROR");
                    btnRate.EditValue = null;
                    if (dailyRateCodeList != null)
                        dailyRateCodeList.Clear();

                    teRateAmount.EditValue = null;
                    return;
                }

                if (dailyRateCodeList == null || dailyRateCodeList.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("No rate is selected!", "ERROR");
                    return;
                }

                if (_selectedRateHeader == null)
                {
                    SystemMessage.ShowModalInfoMessage("No rate is selected!", "ERROR");
                    return;
                }

                if (_currentRoomType == null || _currentRoomCode == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please select room first", "ERROR");
                    return;
                }

                //New Registrations
                List<Control> controls = new List<Control>
                {
                    slueGuest,
                    btnRoom,
                    deArrivalDate,
                    deDepartureDate
                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    return;
                }


                dto = new GroupVM()
                {
                    Adult = Convert.ToInt16(spinAdult.Value),
                    Arrival = deChildArrival.DateTime,
                    Child = Convert.ToInt16(spinChild.Value),
                    Departure = deChildDeparture.DateTime,
                    Numberofnight = Convert.ToInt32((deChildDeparture.DateTime.Date - deChildArrival.DateTime.Date).TotalDays),
                    Guest = slueGuest.Text,
                    GuestCode = Convert.ToInt32(slueGuest.EditValue),
                    Rate = _selectedRateHeader.Description,
                    Room = btnRoom.Text,
                    RoomCode = _currentRoomCode.Value,
                    RateAmount = _currentRateAmount,
                };
                dto.TotalRateAmount = dto.Numberofnight == 0 ? dto.RateAmount : dto.Numberofnight * dto.RateAmount;

                var rType = _allRoomTypes.FirstOrDefault(rt => rt.Id == _currentRoomType);
                dto.RoomType = rType.Description;
                dto.RoomTypeCode = _currentRoomType.Value;

                dto.RateHeader = _selectedRateHeader;
                dto.DilyRateCodeList = new List<DailyRateCodeDTO>();
                dto.DilyRateCodeList.AddRange(dailyRateCodeList);

            }
            else if (tcRegistration.SelectedTabPage == tpExistingRegistraions)
            {



                if (sluRegistrations.EditValue == null || string.IsNullOrEmpty(sluRegistrations.EditValue.ToString()))
                {
                    SystemMessage.ShowModalInfoMessage("Please select a registration first!", "ERROR");
                    return;
                }

                RegistrationDocumentDTO reg = _regVMList.FirstOrDefault(r => r.code == sluRegistrations.EditValue.ToString());

                //Check Existing Added Reg with the same reg Number
                var regAdded = addedDtoList.FirstOrDefault(r => r.RegCode == reg.Id);
                if (regAdded != null)
                {
                    SystemMessage.ShowModalInfoMessage(string.Format("Reg. {0} is already added", regAdded.RegCode), "ERROR");
                    return;
                }


                dto = new GroupVM()
                {
                    IsExisting = true,
                    Adult = reg.adult,
                    Arrival = reg.arrivalDate,
                    Child = reg.child,
                    Departure = reg.departureDate,
                    Guest = reg.Guest,
                    GuestCode = reg.GuestId,
                    RateAmount = reg.rateAmount.Value,
                    Room = reg.RoomNumber,
                    RoomType = reg.RoomTypeDescription,
                    RegCode = reg.Id

                };

                var rHeader = _rateHeaderList.FirstOrDefault(r => r.Id == reg.rateCodeHeader);
                if (rHeader != null)
                {
                    dto.Rate = rHeader.Description;
                }
            }


            _currentRoomCode = null;
            _currentRoomType = null;
            _selectedRateHeader = null;
            btnRate.EditValue = null;
            if (dailyRateCodeList != null)
                dailyRateCodeList.Clear();

            teRateAmount.EditValue = null;
            btnRoom.EditValue = null;
            slueGuest.EditValue = null;
            sleDefaultGuest.EditValue = null;

            addedDtoList.Add(dto);
            gcGroupReg.DataSource = addedDtoList;
            gvGroupReg.RefreshData();
        }

        private void RemoveRegistration(GroupVM dto)
        {
            List<GroupVM> addedDtoList = gvGroupReg.DataSource as List<GroupVM>;
            if (addedDtoList != null && addedDtoList.Count > 0)
            {
                addedDtoList.Remove(dto);
                gvGroupReg.RefreshData();
            }
        }

        private void SelectedRoomEventHandler(RoomDetailDTO room)
        {
            if (room != null)
            {
                btnRoom.EditValue = room.Description;
                _currentRoomCode = room.Id;
                _currentRoomType = room.RoomType;
            }
        }

        private void ResetAll()
        {
            sluCustomer.EditValue = null;
            memoPurposeTravel.EditValue = null;
            sluGroup.EditValue = null;
            lukPayment.EditValue = _defPaymentMethod;
            slueGuest.EditValue = null;
            sleDefaultGuest.EditValue = null;

            if (dailyRateCodeList != null)
                dailyRateCodeList.Clear();
            _currentRateAmount = 0;
            _currentRoomCode = null;
            _currentRoomType = null;
            _selectedRateHeader = null;
            teDate.EditValue = null;
            btnRate.EditValue = null;
            btnRoom.EditValue = null;
            spinAdult.EditValue = 1;
            spinChild.EditValue = 0;

            beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;

            _addedGuests.Clear();
            _addedCompanies.Clear();
        }

        private ConsigneeDTO SaveGuest(AddedGuest guest, DateTime currentTime, int activityDef)
        {
            try
            {
                ConsigneeBuffer PersonBuffer = new ConsigneeBuffer();
                PersonBuffer.consignee = new ConsigneeDTO();
                PersonBuffer.consigneeUnits = new List<ConsigneeUnitDTO>();


                bool isSaved = false;
                List<ConsigneeDTO> personsList = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Where(p => p.GslType == CNETConstantes.GUEST || p.GslType == CNETConstantes.CONTACT).ToList();
                ConsigneeDTO personSaved = personsList.FirstOrDefault(p => p.FirstName.ToLower() == guest.FirstName.ToLower() && p.ThirdName.ToLower() == guest.LastName.ToLower() &&
                            p.SecondName.ToLower() == guest.MiddleName.ToLower());

                if (isSaved)
                {
                    SystemMessage.ShowModalInfoMessage(guest.FirstName + " " + guest.LastName + " " + guest.MiddleName + " already exists", "ERROR");
                    return personSaved;
                }
                else
                {
                    string personCode = UIProcessManager.IdGenerater("Consignee", CNETConstantes.GUEST, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (string.IsNullOrEmpty(personCode))
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to generate guest ID. ", "ERROR");
                        return null;
                    }

                    PreferenceDTO Preference = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(x => x.SystemConstant == CNETConstantes.GUEST);
                    PersonBuffer.consignee = new ConsigneeDTO()
                    {
                        Code = personCode,
                        Title = null,
                        IsPerson = true,
                        GslType = CNETConstantes.GUEST,
                        FirstName = guest.FirstName,
                        SecondName = guest.MiddleName,
                        ThirdName = guest.LastName,
                        IsActive = true,
                        StartDate = currentTime,
                        Nationality = guest.Nationality,
                        Gender = guest.Gender,
                        Preference = Preference.Id,
                    };
                    PersonBuffer.Identifications = new List<IdentificationDTO>()
                    {
                        new IdentificationDTO
                        {
                            Description = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == guest.IDType).Description,
                            IdNumber = guest.IDNumber,
                            Type = guest.IDType
                        }
                    };
                    PersonBuffer.Activity = ActivityLogManager.SetupActivity(currentTime, activityDef, CNETConstantes.CONSIGNEE);
                    PersonBuffer.consigneeUnits = new List<ConsigneeUnitDTO>(){
                        new ConsigneeUnitDTO()
                        {
                        Name = "Main Consignee",
                        Type = CNETConstantes.ORG_UNIT_TYPE_BRUNCH,
                        Phone1 = guest.Telephone
                        } };


                    ConsigneeBuffer isCreated = UIProcessManager.CreateConsigneeBuffer(PersonBuffer);
                    if (isCreated != null)
                    {
                        //Update ID 


                        //Save Identification


                        throw new NotImplementedException();
                        //Object State
                        //var objectStateDefinitions = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.Where(s => s.type == CNETConstantes.GUEST.ToString()).FirstOrDefault();
                        //if (objectStateDefinitions != null)
                        //{
                        //    ObjectState objstaObjectState = new ObjectState
                        //    {
                        //        code = String.Empty,
                        //        reference = personCode,
                        //        objectStateDefinition = objectStateDefinitions.code
                        //    };
                        //    UIProcessManager.CreateObjectState(new List<ObjectState>() { objstaObjectState });
                        //    LocalBuffer.LocalBuffer.LoadObjectStateTable();

                        //}



                        ConsigneeDTO SavedConsigneeview = UIProcessManager.GetConsigneeById(PersonBuffer.consignee.Id);

                        LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Add(SavedConsigneeview);
                        return SavedConsigneeview;
                    }
                    else
                    {
                        return null;
                    }
                }


            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in saving guest. DETAIL:: " + ex.Message, "ERROR");
                return null;
            }
        }

        private ConsigneeDTO SaveCompany(AddedCompany company, DateTime currentTime, int activityDef)
        {
            try
            {
                ConsigneeBuffer ConsigneeBufferDTO = new ConsigneeBuffer();
                ConsigneeBufferDTO.consignee = new ConsigneeDTO();
                ConsigneeBufferDTO.consigneeUnits = new List<ConsigneeUnitDTO>();

                ConsigneeDTO ExistConsignee = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(p => p.Tin == company.TIN);

                if (ExistConsignee != null)
                {
                    return ExistConsignee;
                }
                else
                {


                    string orgCode = UIProcessManager.IdGenerater("Consignee", CNETConstantes.CUSTOMER, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (string.IsNullOrEmpty(orgCode))
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to generate Organization ID. ", "ERROR");
                        return null;
                    }

                    PreferenceDTO Preference = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(x => x.SystemConstant == CNETConstantes.CUSTOMER);
                    ConsigneeBufferDTO.consignee = new ConsigneeDTO()
                    {
                        GslType = CNETConstantes.CUSTOMER,
                        IsPerson = false,
                        Code = orgCode,
                        FirstName = company.Name,
                        SecondName = company.Name,
                        IsActive = true,
                        Tin = company.TIN,
                        Preference = Preference.Id,
                    };
                    ConsigneeBufferDTO.consigneeUnits = new List<ConsigneeUnitDTO>()
                    {
                        new ConsigneeUnitDTO()
                        {
                            Name ="Head Office",
                            Type = CNETConstantes.ORG_UNIT_TYPE_BRUNCH,
                            Email = company.Email,
                            Phone1= company.PhoneNumber,
                        }
                    };
                    ConsigneeBufferDTO.Activity = ActivityLogManager.SetupActivity(currentTime, activityDef, CNETConstantes.CONSIGNEE);


                    ConsigneeBuffer isSaved = UIProcessManager.CreateConsigneeBuffer(ConsigneeBufferDTO);
                    if (isSaved != null)
                    {

                        //Update ID Setting 
                        //Load Organization 

                        ConsigneeDTO SavedConsigneeview = UIProcessManager.GetConsigneeById(isSaved.consignee.Id);

                        LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.Add(SavedConsigneeview);
                        return SavedConsigneeview;

                        //  return isSaved;

                    }
                    else
                    {
                        return null;
                    }

                }


            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in saving Company. DETAIL:: " + ex.Message, "ERROR");
                return null;
            }
        }

        private void OnSave()
        {
            try
            {
                DialogResult dr = MessageBox.Show("Do you want to save registrations?", "Group Registrations", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    return;
                }

                gvGroupReg.PostEditor();

                if (beiState.EditValue == null || string.IsNullOrEmpty(beiState.EditValue.ToString()))
                {
                    SystemMessage.ShowModalInfoMessage("Choose Registration State First!", "ERROR");
                    return;
                }

                if (sluGroup.EditValue == null || string.IsNullOrEmpty(sluGroup.EditValue.ToString()))
                {
                    SystemMessage.ShowModalInfoMessage("Choose Master Guest First!", "ERROR");
                    return;
                }


                List<GroupVM> addedList = gvGroupReg.DataSource as List<GroupVM>;
                if (addedList == null || addedList.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("Noting to save! please add registrations first.", "ERROR");
                    return;
                }


                GroupVM masterReg = addedList.FirstOrDefault(g => g.GuestCode == Convert.ToInt32(sluGroup.EditValue));
                if (masterReg == null)
                {
                    SystemMessage.ShowModalInfoMessage("select master registrations first.", "ERROR");
                    return;
                }


                List<GroupVM> NullGuestReg = addedList.Where(g => g.GuestCode == null).ToList();
                if (NullGuestReg != null && NullGuestReg.Count > 0)
                {
                    SystemMessage.ShowModalInfoMessage("Select Guest for all registrations first.", "ERROR");
                    return;
                }


                List<GroupVM> NullRateHeaderReg = addedList.Where(g => g.RateHeader == null).ToList();
                if (NullRateHeaderReg != null && NullRateHeaderReg.Count > 0)
                {
                    SystemMessage.ShowModalInfoMessage("Select Rate for all registrations first.", "ERROR");
                    return;
                }


                List<GroupVM> NullRateCodeListReg = addedList.Where(g => g.DilyRateCodeList == null || g.DilyRateCodeList.Count == 0).ToList();
                if (NullRateCodeListReg != null && NullRateCodeListReg.Count > 0)
                {
                    SystemMessage.ShowModalInfoMessage("Select Rate for all registrations first.", "ERROR");
                    return;
                }

                int totalTask = 14 * addedList.Count;

                // Progress_Reporter.Show_Progress("Preparing to save registrations", "Please Wait", 1, totalTask);



                //Get Current Date and Time
                // Progress_Reporter.Show_Progress("Getting Current Time", "Please Wait", 1, totalTask);

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return;
                }

                var device = LocalBuffer.LocalBuffer.CurrentDevice;
                var user = LocalBuffer.LocalBuffer.CurrentLoggedInUser;

                int? companyCode = sluCustomer.EditValue == null ? null : Convert.ToInt32(sluCustomer.EditValue);

                List<string> savedRooms = new List<string>();

                int count = 1;

                int? masterRegCode = null;
                List<int> childRegCode = new List<int>();

                //Looping over added
                foreach (var reg in addedList)
                {
                    if (reg.RegCode > 0)
                    {
                        savedRooms.Add(reg.Room);
                        childRegCode.Add(reg.RegCode);
                        continue;
                    }

                    count = count + 1;
                    string localvoCode = string.Empty;

                    int? guestCode = 0;


                    int state = Convert.ToInt32(beiState.EditValue);
                    string stateDesc = "";
                    var osd = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(o => o.Id == state);
                    if (osd != null)
                    {
                        stateDesc = osd.Description;
                    }


                    //Save Person
                    // Progress_Reporter.Show_Progress("Saving Person", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                    var newGuest = _addedGuests.FirstOrDefault(g => g.Id == reg.GuestCode);
                    if (newGuest != null)
                    {

                        var personSaved = SaveGuest(newGuest, CurrentTime.Value, _actDefPerson);
                        if (personSaved == null)
                        {
                            SystemMessage.ShowModalInfoMessage("Guest is not saved for room number: " + reg.Room, "ERROR");
                            ////CNETInfoReporter.Hide();
                            continue;
                        }
                        else
                        {
                            guestCode = personSaved.Id;
                            _addedGuests.Remove(newGuest);
                            ConsigneeDTO guest = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.FirstOrDefault(x => x.Id == newGuest.Id);
                            if (guest != null)
                            {
                                LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.FirstOrDefault(x => x.Id == newGuest.Id).Id = guestCode.Value;
                                guest.Id = guestCode.Value;
                                LocalBuffer.LocalBuffer.ConsigneeViewBufferList.Add(guest);
                            }
                        }

                    }
                    else
                    {
                        guestCode = reg.GuestCode;
                    }

                    //Save Company
                    // Progress_Reporter.Show_Progress("Saving Company", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                    if (sluCustomer.EditValue != null && !string.IsNullOrEmpty(sluCustomer.EditValue.ToString()))
                    {
                        int compCode = Convert.ToInt32(sluCustomer.EditValue);
                        // if (string.IsNullOrEmpty(companyCode))
                        // {

                        var newCompany = _addedCompanies.FirstOrDefault(g => g.Id == compCode);
                        if (newCompany != null)
                        {
                            var companySaved = SaveCompany(newCompany, CurrentTime.Value, _actDefCompany);
                            if (companySaved == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Company is not saved for room number: " + reg.Room, "ERROR");
                                ////CNETInfoReporter.Hide();
                                continue;
                            }
                            else
                            {
                                companyCode = companySaved.Id;
                                _addedCompanies.Remove(newCompany);
                                ConsigneeDTO Companynn = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == newCompany.Id);
                                if (Companynn != null)
                                {
                                    LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == newCompany.Id).Id = companyCode.Value;
                                    Companynn.Id = companyCode.Value;
                                    LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.Add(Companynn);
                                }
                            }

                        }
                        else
                        {
                            companyCode = compCode;
                        }
                        // }
                    }


                    // Progress_Reporter.Show_Progress("Generating Registration ID", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);


                    string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.REGISTRATION_VOUCHER, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (!string.IsNullOrEmpty(currentVoCode))
                    {
                        localvoCode = currentVoCode;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                        ////CNETInfoReporter.Hide();
                        break;

                    }

                    int currentAd = 0;
                    if (state == CNETConstantes.CHECKED_IN_STATE)
                        currentAd = _adCheckin;
                    else if (state == CNETConstantes.SIX_PM_STATE)
                        currentAd = _adSixPm;
                    else if (state == CNETConstantes.GAURANTED_STATE)
                        currentAd = _adGuaranteed;
                    else if (state == CNETConstantes.OSD_WAITLIST_STATE)
                        currentAd = _adWaiting;

                    DateTime arrivalDT = new DateTime(reg.Arrival.Year, reg.Arrival.Month, reg.Arrival.Day, timeArrival.Time.Hour, timeArrival.Time.Minute, timeArrival.Time.Second);
                    DateTime departureDT = new DateTime(reg.Departure.Year, reg.Departure.Month, reg.Departure.Day, timeDeparture.Time.Hour, timeDeparture.Time.Minute, timeDeparture.Time.Second);
                    DateTime departureDate = (departureDT).Date.AddHours(CheckOutDateTime.Hour).AddMinutes(CheckOutDateTime.Minute);

                    VoucherBuffer voucher = new VoucherBuffer
                    {
                        Voucher = new VoucherDTO()
                        {
                            Id = 0,
                            Code = localvoCode,
                            Type = CNETConstantes.RESERVATIONTYPECHECKIN, //beginning Voucher
                            Definition = CNETConstantes.REGISTRATION_VOUCHER, //Registration Voucher
                            Consignee1 = guestCode,
                            Consignee2 = companyCode,
                            IssuedDate = CurrentTime.Value,
                            Year = CurrentTime.Value.Year,
                            Month = CurrentTime.Value.Month,
                            Day = CurrentTime.Value.Day,
                            StartDate = arrivalDT,
                            EndDate = departureDate,
                            IsIssued = true,
                            IsVoid = false,
                            GrandTotal = 0,
                            Period = null,
                            LastState = state,
                            OriginConsigneeUnit = SelectedHotelcode,
                            HasEffect = false,
                            PaymentMethod = lukPayment.EditValue == null ? CNETConstantes.PAYMENTMETHODSCASH : Convert.ToInt32(lukPayment.EditValue),
                            Purpose = memoPurposeTravel.EditValue == null ? null : Convert.ToInt32(memoPurposeTravel.EditValue),
                        },
                        Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, currentAd, CNETConstantes.PMS_Pointer)
                    };



                    voucher.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                    voucher.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                    voucher.TransactionCurrencyBuffer = null;
                    // Progress_Reporter.Show_Progress("Creating Registration Voucher", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                    ResponseModel<VoucherBuffer> isSaved = UIProcessManager.CreateVoucherBuffer(voucher);
                    if (isSaved != null && isSaved.Success)
                    {

                        if (masterRegCode == null && reg.Room == masterReg.Room)
                            masterRegCode = isSaved.Data.Voucher.Id;
                        else
                            childRegCode.Add(isSaved.Data.Voucher.Id);

                        if (masterRegCode != null && isSaved.Data.Voucher.Id == masterRegCode)
                        {
                            isSaved.Data.Voucher.HasEffect = true;
                            UIProcessManager.UpdateVoucher(isSaved.Data.Voucher);
                        }


                        // Progress_Reporter.Show_Progress("Saving Activity", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);


                        // Progress_Reporter.Show_Progress("Updating Id", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                        //Update ID Setting

                        //Save Other Consignee 

                        // Registration Previledge
                        // Progress_Reporter.Show_Progress("Saving Registration Previledge", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                        RegistrationPrivllegeDTO previlage = new RegistrationPrivllegeDTO();
                        previlage.Voucher = isSaved.Data.Voucher.Id;
                        previlage.Nopost = false;
                        previlage.AuthorizeDirectBill = false;
                        previlage.PreStayCharging = false;
                        previlage.PostStayCharging = false;
                        previlage.AllowLatecheckout = false;
                        previlage.AuthorizeKeyReturn = false;
                        previlage.Remark = "";
                        RegistrationPrivllegeDTO isP = UIProcessManager.CreateRegistrationPrivllege(previlage);
                        if (isP == null) continue;









                        //Registration Detail
                        // Progress_Reporter.Show_Progress("Saving Registration Detail", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                        RegistrationDetailDTO regDetail = new RegistrationDetailDTO
                        {
                            Date = CurrentTime,
                            Adult = reg.Adult,
                            Child = reg.Child,
                            RoomCount = 1,
                            Room = reg.RoomCode,
                            RoomType = reg.RoomTypeCode,
                            ActualRtc = reg.RoomTypeCode,
                            IsFixedRate = false,
                            Market = null,
                            Source = null,
                            RateAmount = reg.RateAmount,
                            Adjustment = 0,
                            Remark = ""

                        };



                        List<GeneratedRegistrationDTO> Registrationlist = UIProcessManager.RegistrationDetailGenerator(voucher.Voucher.Id, voucher.Voucher.StartDate.Value, voucher.Voucher.EndDate.Value, regDetail, reg.DilyRateCodeList);

                        if (Registrationlist == null || Registrationlist.Count == 0)
                        {
                            SystemMessage.ShowModalInfoMessage("No registration detail generated. Please check the rate code given for Room Number: " + reg.Room, "ERROR");
                            ////CNETInfoReporter.Hide();
                            continue;
                        }

                        foreach (GeneratedRegistrationDTO GeneratedDetail in Registrationlist)
                        {

                            GeneratedDetail.registrationDetail.Adjustment = 0;
                            GeneratedDetail.registrationDetail.Voucher = isSaved.Data.Voucher.Id;

                            RegistrationDetailDTO newRegistration = UIProcessManager.CreateRegistrationDetail(GeneratedDetail.registrationDetail);

                            if (GeneratedDetail.packagesToPost != null)
                            {
                                foreach (PackagesToPostDTO packagetoPost in GeneratedDetail.packagesToPost)
                                {
                                    packagetoPost.RegistrationDetail = newRegistration.Id;

                                    UIProcessManager.CreatePackagesToPost(packagetoPost);
                                }

                            }

                        }

                        // Charge during check-in
                        try
                        {
                            if (voucher.Voucher.LastState == CNETConstantes.CHECKED_IN_STATE)
                            {

                                //Check Configuration
                                var chargeAtCheckin = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_ChargeAtCheckIn);
                                bool canCharge = false;
                                if (chargeAtCheckin != null)
                                {
                                    canCharge = Convert.ToBoolean(chargeAtCheckin.CurrentValue);
                                }

                                if (canCharge)
                                {
                                    int rateCodeHeader = 0;
                                    var currentDetail = Registrationlist.FirstOrDefault(x => x.registrationDetail.Date.Value.Date == CurrentTime.Value.Date).registrationDetail;
                                    if (currentDetail != null)
                                    {
                                        var rateDetail = UIProcessManager.SelectAllRateCodeDetail().FirstOrDefault(r => r.Id == currentDetail.RateCode);
                                        if (rateDetail != null)
                                        {
                                            var rHeader = UIProcessManager.GetRateCodeHeaderById(rateDetail.RateCodeHeader);
                                            if (rHeader != null)
                                            {
                                                rateCodeHeader = rHeader.Id;
                                            }
                                        }
                                    }

                                    if (rateCodeHeader > 0)
                                        CommonLogics.ChargeAtCheckin(voucher.Voucher.Id, CurrentTime.Value, rateCodeHeader, voucher.Voucher.Consignee1, SelectedHotelcode.Value, false);
                                }




                                if (LocalBuffer.LocalBuffer.EarlyCheckIn && LocalBuffer.LocalBuffer.EarlyCheckInArticle > 0 && CurrentTime.Value <= LocalBuffer.LocalBuffer.EarlyCheckInUntilTime)
                                {
                                    if (LocalBuffer.LocalBuffer.EarlyCheckInChargeMandatory)
                                        XtraMessageBox.Show("The Customer is Early Check-In." + Environment.NewLine + "There will be an Early Room Charge !!", "CNET-ERPV6", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    if (LocalBuffer.LocalBuffer.EarlyCheckInChargeMandatory || XtraMessageBox.Show("The Customer is Early Check-In." + Environment.NewLine + "Do you want to Early Room Charge ??", "CNET-ERPV6", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                                    {
                                        CommonLogics.ChargeAtEarlyCheckin(voucher.Voucher.Id, CurrentTime.Value, voucher.Voucher.Consignee1.Value, SelectedHotelcode.Value, LocalBuffer.LocalBuffer.EarlyCheckInArticle, true);
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        savedRooms.Add(reg.Room);

                    }
                    else
                    { 
                        SystemMessage.ShowModalInfoMessage("Fail Saving !! " + Environment.NewLine + isSaved.Message, "Error");
                    }



                }//end of outer loop


                //Save Transaction Reference
                if (masterRegCode > 0 && childRegCode.Count > 0)
                {
                    foreach (var reg in childRegCode)
                    {
                        TransactionReferenceDTO tranRef = new TransactionReferenceDTO()
                        {
                            ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                            Referenced = masterRegCode,
                            ReferencingVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                            Referring = reg,
                        };
                        UIProcessManager.CreateTransactionReference(tranRef);
                    }
                }

                //Refresh Added Registration Grid
                var savedRegList = addedList.Where(r => savedRooms.Contains(r.Room)).ToList();
                if (savedRegList != null && savedRegList.Count > 0)
                {


                    if (savedRegList.Count == addedList.Count)
                    {
                        SystemMessage.ShowModalInfoMessage("Registrations are Saved!", "MESSAGE");

                        foreach (var reg in savedRegList)
                        {
                            addedList.Remove(reg);
                        }

                        gvGroupReg.RefreshData();
                        //Reset
                        ResetAll();
                    }

                    else
                        SystemMessage.ShowModalInfoMessage("Some Registrations are not Saved!", "ERROR");
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Registrations are not Saved!", "ERROR");
                }

                tcRegistration.SelectedTabPage = tpNewRegistrations;
                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error has occured in saving registration. DETAIL:: " + ex.Message, "ERROR");
            }
        }

        private void DisplayRateAmount(RateCodeHeaderDTO rateCode, string rateAmount)
        {
            if (rateCode != null)
            {
                CurrencyDTO currency = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.Id == rateCode.CurrencyCode);
                if (currency != null)
                {
                    if (currency.Description.Trim().ToLower() == "birr")
                    {
                        teRateAmount.Text = string.Format("{0} {1}", rateAmount, "birr");
                    }
                    else
                    {
                        teRateAmount.Text = string.Format("{0}{1}", currency.Sign, rateAmount);
                    }
                }
                else
                {
                    teRateAmount.Text = rateAmount;
                }

            }
            else
                teRateAmount.Text = "0";

        }

        #endregion


        /************************************* EVENT HANDLERS ***************************************/

        #region Event Handlers

        private void frmGroupRegistration_Load(object sender, EventArgs e)
        {

            if (!InitializeData())
            {
                this.Close();
            }

            //System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(Application.StartupPath + "//FrontOffice.PMS.exe");
            //var setting = config.AppSettings.Settings["GroupRegistration"];

            //if (setting == null)
            //{
            //    SystemMessage.ShowModalInfoMessage("Please add GroupRegistration Setting in Frontoffice configuration file", "ERROR");
            //    this.Close();
            //}

            //string value = setting.Value;

            //if (!string.IsNullOrEmpty(value) && value.ToLower() == "sodere")
            //{
            //    frmSodereGroupReg sodereReg = new frmSodereGroupReg(this);
            //    sodereReg.Dock = DockStyle.Fill;
            //    pnlGroupReg.Controls.Clear();
            //    pnlGroupReg.Controls.Add(sodereReg);
            //}
            //else
            //{
            //    frmGeneralGroupReg general = new frmGeneralGroupReg(this);
            //    general.Dock = DockStyle.Fill;
            //    pnlGroupReg.Controls.Clear();
            //    pnlGroupReg.Controls.Add(general);
            //}

            //frmGeneralGroupReg general = new frmGeneralGroupReg(this);
            //general.Dock = DockStyle.Fill;
            //pnlGroupReg.Controls.Clear();
            //pnlGroupReg.Controls.Add(general);

            // state list

        }

        #endregion

        private void deArrivalDate_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtArrival = (DateTime)deArrivalDate.EditValue;
            DateTime dtDeparture = deDepartureDate.DateTime;
            if (dtArrival.Date >= dtDeparture.Date)
            {
                deDepartureDate.DateTime = dtArrival.AddDays(1);
                spinNoNight.EditValue = 1;

            }
            else
            {
                spinNoNight.EditValue = Convert.ToInt32((dtDeparture - dtArrival).TotalDays).ToString();
            }

            deDefaultArrivalDate.DateTime = dtArrival;
            deChildArrival.DateTime = dtArrival;
        }

        private void deDepartureDate_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtArrival = (DateTime)deArrivalDate.EditValue;
            if (deDepartureDate.EditValue == null) return;

            DateTime dtDeparture = (DateTime)deDepartureDate.EditValue;

            if (dtArrival.Date > dtDeparture.Date)
            {
                deDepartureDate.DateTime = dtArrival.AddDays(1);
                spinNoNight.EditValue = 1;
            }
            else
            {
                spinNoNight.EditValue = Convert.ToInt32((dtDeparture - dtArrival).TotalDays).ToString();

            }

            deDefaultDepartureDate.DateTime = deDepartureDate.DateTime;
            deChildDeparture.DateTime = deDepartureDate.DateTime;
        }

        private void gvGroupReg_DoubleClick(object sender, EventArgs e)
        {
            if (tcRegistration.SelectedTabPage == tpNewRegistrations)
            {
                GridView view = sender as GridView;
                GroupVM dto = view.GetRow(view.FocusedRowHandle) as GroupVM;
                if (dto == null) return;

                if (dto.IsExisting)
                {
                    RemoveRegistration(dto);
                    return;
                }

                //Populate Edit Fields
                slueGuest.EditValue = dto.GuestCode;
                btnRoom.Text = dto.Room;
                deChildArrival.DateTime = dto.Arrival;
                deChildDeparture.DateTime = dto.Departure;
                spinAdult.EditValue = dto.Adult;
                spinChild.EditValue = dto.Child;

                DisplayRateAmount(dto.RateHeader, dto.RateAmount.ToString());
                dailyRateCodeList = dto.DilyRateCodeList;
                _selectedRateHeader = dto.RateHeader;
                btnRate.Text = dto.Rate;

                RemoveRegistration(dto);


            }

        }


        private void deChildArrival_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtArrival = (DateTime)deChildArrival.EditValue;
            DateTime dtDeparture = deChildDeparture.DateTime;
            if (dtArrival.Date >= dtDeparture.Date)
            {
                deChildDeparture.DateTime = dtArrival.AddDays(1);

            }

            _selectedRateHeader = null;
            _availalbePackages = null;
            dailyRateCodeList = null;
            btnRate.EditValue = null;
            teRateAmount.EditValue = null;
            _currentRateAmount = 0;
        }

        private void deChildDeparture_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtArrival = (DateTime)deChildArrival.DateTime;
            if (deChildDeparture.EditValue == null) return;

            DateTime dtDeparture = (DateTime)deChildDeparture.EditValue;

            if (dtArrival.Date > dtDeparture.Date)
            {
                deChildDeparture.DateTime = dtArrival.AddDays(1);
            }


            _selectedRateHeader = null;
            dailyRateCodeList = null;
            btnRate.EditValue = null;
            teRateAmount.EditValue = null;
            _currentRateAmount = 0;
            _availalbePackages = null;
        }

        private void spinNoNight_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit view = sender as SpinEdit;
            if (view.EditValue == null) return;
            deDepartureDate.EditValue = deArrivalDate.DateTime.AddDays(Convert.ToInt32(view.EditValue.ToString()));
        }

        private void btnRate_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (_currentRoomType == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select Room first!", "ERROR");
                return;
            }

            Dictionary<String, String> args = new Dictionary<string, string>
            {
                {"fromDate", deChildArrival.Text},
                {"toDate", deChildDeparture.Text},
                {"adultCount", spinAdult.Value.ToString()},
                {"childCount", spinChild.Value.ToString()},
                {"roomCount", "1"},
                {"rateCode",""},
                {"roomType", _currentRoomType.ToString()}
            };


            _rateSearchForm = new frmRateSearch();
            _rateSearchForm.SelectedHotelcode = SelectedHotelcode.Value;
            _rateSearchForm.LoadData(this, args);
            _rateSearchForm.RateSelected += frmRateSearch_RateSelected;
            _rateSearchForm.ShowDialog();
        }

        private void frmRateSearch_RateSelected(object sender, RateSearchCellClickedEventArgs e)
        {
            _selectedRoomType = e.RoomType.Value;


            btnRate.Text = e.RowName;
            try
            {
                _currentRateAmount = Convert.ToDecimal(e.CellValue);
            }
            catch (Exception ex)
            {
                _currentRateAmount = 0;
            }

            if (_availalbePackages != null)
                _availalbePackages.Clear();

            if (dailyRateCodeList != null)
                dailyRateCodeList.Clear();


            if (_rateSearchForm.frmRateDetailInfo != null)
            {
                dailyRateCodeList = _rateSearchForm.frmRateDetailInfo.dailyRateCodeList;
            }


            if (e.RowCode != null)
            {
                RateCodeHeaderDTO rateCode = _rateHeaderList.FirstOrDefault(r => r.Id == e.RowCode);
                if (rateCode != null)
                {
                    _selectedRateHeader = rateCode;
                    DisplayRateAmount(_selectedRateHeader, e.CellValue);
                }
            }

        }



        private void btnRoom_EditValueChanged(object sender, EventArgs e)
        {
            RoomDetailDTO rd = UIProcessManager.SelectAllRoomDetail().FirstOrDefault(r => r.Description == btnRoom.Text);
            if (rd != null)
            {
                SelectedRoomHandler.Invoke(rd);

            }
        }

        private void btnRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>();
            args.Add("Guest Name", slueGuest.Text);
            args.Add("Arrival", deChildArrival.Text);
            args.Add("Departure", deChildDeparture.Text);

            string numberOfNight = Convert.ToInt32((deChildDeparture.DateTime - deChildArrival.DateTime).TotalDays).ToString();

            args.Add("Nights", numberOfNight);
            args.Add("RoomType", "");
            args.Add("RoomTypeDescription", "");

            if (Convert.ToInt32(beiState.EditValue) == CNETConstantes.CHECKED_IN_STATE)
            {
                args.Add("CHECK_HK", "YES");
            }
            else
            {
                args.Add("CHECK_HK", "NO");
            }


            //  Home.OpenForm(this, "ROOM SEARCH", args);
            frmRoomSearch frmRoom = new frmRoomSearch();
            frmRoom.SelectedHotelcode = SelectedHotelcode;
            frmRoom.LoadData(this, args);
            if (frmRoom.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }
        }

        private void beiState_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(beiState.EditValue.ToString()) == CNETConstantes.CHECKED_IN_STATE)
            {
                DateTime? date = UIProcessManager.GetServiceTime();
                if (date == null) return;
                deArrivalDate.DateTime = date.Value;
                deDefaultArrivalDate.DateTime = date.Value;
                deArrivalDate.Enabled = false;
                repositoryItemDateEditArrival.Enabled = false;
                gridColumnArrival.OptionsColumn.AllowEdit = false;
                deChildArrival.Enabled = false;
                timeArrival.Enabled = false;
                deDefaultArrivalDate.Enabled = false;

                rpgCheckIn.Visible = true;
                rpgSave.Visible = false;
            }
            else
            {
                deArrivalDate.Enabled = true;
                deChildArrival.Enabled = true;
                deDefaultArrivalDate.Enabled = true;
                gridColumnArrival.OptionsColumn.AllowEdit = true;
                repositoryItemDateEditArrival.Enabled = true;
                timeArrival.Enabled = true;

                rpgCheckIn.Visible = false;
                rpgSave.Visible = true;
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }


        private void btnAdd_ButtonClick(object sender, EventArgs e)
        {
            AddRegistration();
        }

        private void btnRemove_ButtonClick(object sender, EventArgs e)
        {
            GroupVM dto = gvGroupReg.GetRow(gvGroupReg.FocusedRowHandle) as GroupVM;
            if (dto == null) return;

            RemoveRegistration(dto);
        }

        private void gvGroupReg_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "Guest")
            {
                GroupVM dto = gvGroupReg.GetRow(e.RowHandle) as GroupVM;
                if (dto == null || sluGroup.EditValue == null) return;
                if (dto.GuestCode == Convert.ToInt32(sluGroup.EditValue))
                {
                    e.Appearance.ForeColor = ColorTranslator.FromHtml("Black");
                    e.Appearance.BackColor = ColorTranslator.FromHtml("LightGreen");
                }
            }
            if (e.Column.FieldName == "Room")
            {
                GroupVM dto = gvGroupReg.GetRow(e.RowHandle) as GroupVM;
                if (dto == null || sluGroup.EditValue == null) return;
                if (dto.IsExisting)
                {
                    e.Appearance.ForeColor = ColorTranslator.FromHtml("Black");
                    e.Appearance.BackColor = ColorTranslator.FromHtml("Yellow");
                }
            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetAll();
        }

        private void bbiCheckIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnSave();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnSave();
        }

        private void sluCustomer_AddNewValue(object sender, AddNewValueEventArgs e)
        {

            frmOrganization organization = new frmOrganization();
            organization.GslType = CNETConstantes.CUSTOMER;
            organization.Text = "Company";
            organization.LoadEventArg.Args = "Company";
            organization.LoadEventArg.Sender = null;
            organization.LoadData(this, null);
            if (organization.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (organization.SavedOrg != null)
                {
                    sluCustomer.Properties.DataSource = null;
                    sluCustomer.Properties.DataSource = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist;
                    e.Cancel = false;
                    e.NewValue = organization.SavedOrg.Id;
                }
            }



            //sluCustomer.EditValue = null;
            /*  _frmNewCompany = new frmNewCompany();
              if (_frmNewCompany.ShowDialog() == DialogResult.OK)
              {
                  if (_frmNewCompany.SavedCompany != null)
                  {
                      _addedCompanies.Add(_frmNewCompany.SavedCompany);

                      // VwConsigneeViewDTO dto = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(g => g.Id == _frmNewCompany.SavedCompany.Id);
                      VwConsigneeViewDTO dto = new VwConsigneeViewDTO();
                      dto.Code = _frmNewCompany.SavedCompany.Code;
                      dto.FirstName = _frmNewCompany.SavedCompany.Name;
                      dto.Tin = _frmNewCompany.SavedCompany.TIN;
                      if (!LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.Contains(dto))
                      {
                          LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.Add(dto);
                      };
                      sluCustomer.Properties.DataSource = null;

                      sluCustomer.Properties.DataSource = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist;

                      e.NewValue = dto.Id;
                      e.Cancel = false;

                  }
              }
            */
        }

        private void sluGuest_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            try
            {

                frmPerson frmperson = new frmPerson("Guest");
                frmperson.GSLType = CNETConstantes.GUEST;
                frmperson.Text = "Guest";
                frmperson.rpgScanFingerPrint.Visible = true;
                frmperson.LoadEventArg.Args = "Guest";
                frmperson.LoadData(this, this);
                frmperson.LoadEventArg.Sender = null;
                if (frmperson.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ///GetPerson();
                    if (frmperson.SavedPerson != null)
                    {

                        LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Add(frmperson.SavedPerson);

                        //for master guest
                        sluGroup.Properties.DataSource = null;
                        sluGroup.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                        sluGroup.Properties.View.RefreshData();


                        //for child guest
                        slueGuest.Properties.DataSource = null;
                        slueGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                        slueGuest.Properties.View.RefreshData();


                        sleDefaultGuest.Properties.DataSource = null;
                        sleDefaultGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                        sleDefaultGuest.Properties.View.RefreshData();

                        repositoryItemSearchLookUpEditGuest.DataSource = null;
                        repositoryItemSearchLookUpEditGuest.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                        repositoryItemSearchLookUpEditGuest.View.RefreshData();

                        e.Cancel = false;
                        e.NewValue = frmperson.SavedPerson.Id;

                        if ((SearchLookUpEdit)sender == sluGroup)
                            slueGuest.EditValue = frmperson.SavedPerson.Id;
                    }
                }


            }
            catch (Exception ex)
            {

            }

            /* SearchLookUpEdit viewsluGuest = sender as SearchLookUpEdit;

             //if (_frmNewGuest == null)
             _frmNewGuest = new frmNewGuest();


             _frmNewGuest.SavedGuest = null;

             if (_frmNewGuest.ShowDialog() == DialogResult.OK)
             {
                 if (_frmNewGuest.SavedGuest != null)
                 {



                     AddedGuest p = _frmNewGuest.SavedGuest;

                     VwConsigneeViewDTO dto = new VwConsigneeViewDTO();
                     dto.Code = _frmNewGuest.SavedGuest.Code;
                     dto.FirstName = _frmNewGuest.SavedGuest.FirstName;
                     dto.SecondName = _frmNewGuest.SavedGuest.MiddleName;
                     dto.ThirdName = _frmNewGuest.SavedGuest.LastName;

                     _addedGuests.Add(_frmNewGuest.SavedGuest);
                     LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Add(dto);

                     //for master guest
                     sluGroup.Properties.DataSource = null;
                     sluGroup.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                     sluGroup.Properties.View.RefreshData();


                     //for child guest
                     slueGuest.Properties.DataSource = null;
                     slueGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                     slueGuest.Properties.View.RefreshData();


                     sleDefaultGuest.Properties.DataSource = null;
                     sleDefaultGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                     sleDefaultGuest.Properties.View.RefreshData();

                     repositoryItemSearchLookUpEditGuest.DataSource = null;
                     repositoryItemSearchLookUpEditGuest.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                     repositoryItemSearchLookUpEditGuest.View.RefreshData();


                     if (viewsluGuest.Name == "sluGroup")
                     {
                         e.NewValue = dto.Id;
                         slueGuest.EditValue = dto.Id;

                     }
                     else
                     {
                         slueGuest.EditValue = dto.Id;
                         e.NewValue = dto.Id;
                     }


                     _frmNewGuest.SavedGuest = null;

                 }
             }*/
        }

        private void sleDefaultGuest_AddNewValue(object sender, AddNewValueEventArgs e)
        {



            frmPerson frmperson = new frmPerson("Guest");
            frmperson.GSLType = CNETConstantes.GUEST;
            frmperson.Text = "Guest";
            frmperson.rpgScanFingerPrint.Visible = true;
            frmperson.LoadEventArg.Args = "Guest";
            frmperson.LoadData(this, this);
            frmperson.LoadEventArg.Sender = null;
            if (frmperson.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (frmperson.SavedPerson != null)
                {

                    LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Add(frmperson.SavedPerson);

                    //for master guest
                    sluGroup.Properties.DataSource = null;
                    sluGroup.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    sluGroup.Properties.View.RefreshData();


                    //for child guest
                    slueGuest.Properties.DataSource = null;
                    slueGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    slueGuest.Properties.View.RefreshData();


                    sleDefaultGuest.Properties.DataSource = null;
                    sleDefaultGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    sleDefaultGuest.Properties.View.RefreshData();

                    repositoryItemSearchLookUpEditGuest.DataSource = null;
                    repositoryItemSearchLookUpEditGuest.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    repositoryItemSearchLookUpEditGuest.View.RefreshData();

                    e.Cancel = false;
                    e.NewValue = frmperson.SavedPerson.Id;
                }
            }


            /*
            SearchLookUpEdit viewsluGuest = sender as SearchLookUpEdit;

            //if (_frmNewGuest == null)
            _frmNewGuest = new frmNewGuest();


            _frmNewGuest.SavedGuest = null;

            if (_frmNewGuest.ShowDialog() == DialogResult.OK)
            {
                if (_frmNewGuest.SavedGuest != null)
                {
                    AddedGuest p = _frmNewGuest.SavedGuest;
                    VwConsigneeViewDTO gDto = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.FirstOrDefault(g => g.Code == p.Code);
                    if (gDto == null)
                    {
                        gDto = new VwConsigneeViewDTO();
                    }
                    gDto.Code = p.Code;
                    gDto.FirstName = p.FirstName;
                    gDto.SecondName = p.MiddleName;
                    gDto.ThirdName = p.LastName;
                    gDto.BioId = p.IDNumber;

                    _addedGuests.Add(_frmNewGuest.SavedGuest);
                    LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Add(gDto);

                    //for master guest
                    sluGroup.Properties.DataSource = null;
                    sluGroup.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    sluGroup.Properties.View.RefreshData();


                    //for child guest
                    slueGuest.Properties.DataSource = null;
                    slueGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    slueGuest.Properties.View.RefreshData();

                    sleDefaultGuest.Properties.DataSource = null;
                    sleDefaultGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    sleDefaultGuest.Properties.View.RefreshData();


                    repositoryItemSearchLookUpEditGuest.DataSource = null;
                    repositoryItemSearchLookUpEditGuest.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    repositoryItemSearchLookUpEditGuest.View.RefreshData();

                    e.NewValue = gDto.Id;
                    sleDefaultGuest.EditValue = gDto.Id;


                    _frmNewGuest.SavedGuest = null;

                }
            }

            */
        }

        private void sluCustomer_EditValueChanged1(object sender, EventArgs e)
        {
            //   string ddd = sluCustomer.EditValue.ToString();
        }

        private void ribbonControl1_Click(object sender, EventArgs e)
        {

        }

        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {
            SelectedHotelcode = beiHotel.EditValue == null ? null : Convert.ToInt32(beiHotel.EditValue);

            _rateHeaderList = UIProcessManager.GetRateCodeHeaderByconsigneeunit(SelectedHotelcode.Value);
            if (_rateHeaderList == null || _rateHeaderList.Count == 0)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Please define rate first.", "ERROR");
            }
        }
        public int? SelectedHotelcode { get; set; }


        private void tcRegistration_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tcRegistration.SelectedTabPage == tpMultipleRoomRegistraions)
            {
                gvGroupReg.OptionsBehavior.Editable = true;
                btnAdd.Enabled = false;
            }
            else
            {
                gvGroupReg.OptionsBehavior.Editable = false;
                btnAdd.Enabled = true;
            }

        }

        private void repositoryItemButtonEditRate_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            GroupVM ChangedRate = (GroupVM)gvGroupReg.GetFocusedRow();
            Dictionary<String, String> args = new Dictionary<string, string>
            {
                {"fromDate", ChangedRate.Arrival.ToString()},
                {"toDate", ChangedRate.Departure.ToString()},
                {"adultCount", ChangedRate.Adult.ToString()},
                {"childCount", ChangedRate.Child.ToString()},
                {"roomCount", "1"},
                {"rateCode",""},
                {"roomType", ChangedRate.RoomTypeCode.ToString()}
            };


            _CellrateSearchForm = new frmRateSearch();
            _CellrateSearchForm.SelectedHotelcode = SelectedHotelcode.Value;
            _CellrateSearchForm.LoadData(this, args);
            _CellrateSearchForm.RateSelected += GridCellRateSearch_RateSelected;
            _CellrateSearchForm.ShowDialog();

            if (_CellrateSearchForm.frmRateDetailInfo != null)
            {
                CelldailyRateCodeList = _CellrateSearchForm.frmRateDetailInfo.dailyRateCodeList;
            }
        }


        private decimal _CellcurrentDefaultRateAmount;
        private RateCodeHeaderDTO _CellselectedRateHeader;
        private List<PackageHeaderDTO> _CellavailalbePackages = null;
        private List<DailyRateCodeDTO> CelldailyRateCodeList = new List<DailyRateCodeDTO>();
        private void GridCellRateSearch_RateSelected(object sender, RateSearchCellClickedEventArgs e)
        {
            try
            {
                _CellcurrentDefaultRateAmount = Convert.ToDecimal(e.CellValue);
            }
            catch (Exception ex)
            {
                _CellcurrentDefaultRateAmount = 0;
            }

            if (_CellavailalbePackages != null)
                _CellavailalbePackages.Clear();

            if (CelldailyRateCodeList != null)
                CelldailyRateCodeList.Clear();


            if (_CellrateSearchForm.frmRateDetailInfo != null)
            {
                CelldailyRateCodeList = _CellrateSearchForm.frmRateDetailInfo.dailyRateCodeList;
            }


            if (e.RowCode > 0)
            {
                RateCodeHeaderDTO rateCode = _rateHeaderList.FirstOrDefault(r => r.Id == e.RowCode);
                if (rateCode != null)
                {
                    _CellselectedRateHeader = rateCode;
                    string ratevalue = GetDefaultRateAmount(_CellselectedRateHeader, e.CellValue);
                }
            }


            GroupVM ChangedRate = (GroupVM)gvGroupReg.GetFocusedRow();
            ChangedRate.Rate = _CellselectedRateHeader == null ? null : _CellselectedRateHeader.Description;
            ChangedRate.RateAmount = _CellcurrentDefaultRateAmount;
            ChangedRate.TotalRateAmount = ChangedRate.Numberofnight == 0 ? ChangedRate.RateAmount : ChangedRate.Numberofnight * ChangedRate.RateAmount;
            ChangedRate.RateHeader = _CellselectedRateHeader;
            ChangedRate.DilyRateCodeList = new List<DailyRateCodeDTO>();
            ChangedRate.DilyRateCodeList.AddRange(CelldailyRateCodeList);
            gvGroupReg.RefreshData();
            gvGroupReg.RefreshRow(gvGroupReg.FocusedRowHandle);
        }

        private string GetDefaultRateAmount(RateCodeHeaderDTO rateCode, string rateAmount)
        {
            string value = "";
            CurrencyDTO currency = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.Id == rateCode.CurrencyCode);
            if (currency != null)
            {
                if (currency.Description.Trim().ToLower() == "birr")
                {
                    value = string.Format("{0} {1}", rateAmount, "birr");
                }
                else
                {
                    value = string.Format("{0}{1}", currency.Sign, rateAmount);
                }
            }
            else
            {
                value = rateAmount;
            }
            return value;
        }

        private RateCodeHeaderDTO _DefaultselectedRateHeader;
        private List<PackageHeaderDTO> _DefaultavailalbePackages = null;
        private List<DailyRateCodeDTO> DefaultdailyRateCodeList = new List<DailyRateCodeDTO>();
        private void frmDefaultRateSearch_RateSelected(object sender, RateSearchCellClickedEventArgs e)
        {
            //_selectedRoomType = e.RoomType;


            btnDefRate.Text = e.RowName;
            try
            {
                _currentDefaultRateAmount = Convert.ToDecimal(e.CellValue);
            }
            catch (Exception ex)
            {
                _currentDefaultRateAmount = 0;
            }

            if (_DefaultavailalbePackages != null)
                _DefaultavailalbePackages.Clear();

            if (DefaultdailyRateCodeList != null)
                DefaultdailyRateCodeList.Clear();


            if (_RoomDefulatrateSearchForm.frmRateDetailInfo != null)
            {
                DefaultdailyRateCodeList = _RoomDefulatrateSearchForm.frmRateDetailInfo.dailyRateCodeList;
            }


            if (e.RowCode != null)
            {
                RateCodeHeaderDTO rateCode = _rateHeaderList.FirstOrDefault(r => r.Id == e.RowCode);
                if (rateCode != null)
                {
                    _DefaultselectedRateHeader = rateCode;
                    DisplayDefaultRateAmount(_DefaultselectedRateHeader, e.CellValue);
                }
            }

        }

        private void DisplayDefaultRateAmount(RateCodeHeaderDTO rateCode, string rateAmount)
        {
            CurrencyDTO currency = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.Id == rateCode.CurrencyCode);
            if (currency != null)
            {
                if (currency.Description.Trim().ToLower() == "birr")
                {
                    teDefRateAmount.Text = string.Format("{0} {1}", rateAmount, "birr");
                }
                else
                {
                    teDefRateAmount.Text = string.Format("{0}{1}", currency.Sign, rateAmount);
                }
            }
            else
            {
                teDefRateAmount.Text = rateAmount;
            }
        }

        private void btnSelectRooms_Click(object sender, EventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>();
            args.Add("Guest Name", "");
            args.Add("Arrival", deDefaultArrivalDate.Text);
            args.Add("Departure", deDefaultDepartureDate.Text);

            string numberOfNight = Convert.ToInt32((deDefaultDepartureDate.DateTime - deDefaultArrivalDate.DateTime).TotalDays).ToString();

            args.Add("Nights", numberOfNight);
            args.Add("RoomType", "");
            args.Add("RoomTypeDescription", "");

            if (Convert.ToInt32(beiState.EditValue) == CNETConstantes.CHECKED_IN_STATE)
            {
                args.Add("CHECK_HK", "YES");
            }
            else
            {
                args.Add("CHECK_HK", "NO");
            }

            List<int> FilterRoomType = null;
            if(DefaultdailyRateCodeList != null && DefaultdailyRateCodeList.Count > 0)
            {
                List<int?> RateDetaillist = DefaultdailyRateCodeList.Select(x=> x.RateCodeDetail).ToList();
                RateDetaillist = RateDetaillist.Where(x=> x != null).ToList();

                List<RateCodeDetailRoomTypeDTO> rateCodeDetailRoomTypes = UIProcessManager.SelectAllRateCodeDetailRoomType();

                rateCodeDetailRoomTypes = rateCodeDetailRoomTypes.Where(x=> RateDetaillist.Contains( x.RateCodeDetail) ).ToList();

                if (rateCodeDetailRoomTypes != null && rateCodeDetailRoomTypes.Count > 0)
                    FilterRoomType = rateCodeDetailRoomTypes.Select(x=> x.RoomType).ToList();
            }

            //  Home.OpenForm(this, "ROOM SEARCH", args);
            frmRoomSearch frmRoom = new frmRoomSearch();
            frmRoom._FilterroomTypeList = FilterRoomType;
            frmRoom.MultipleRoom = true;
            frmRoom.SelectedHotelcode = SelectedHotelcode;
            frmRoom.LoadData(this, args);
            if (frmRoom.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
            }



            //    List<RoomDetail> Roomdetaillist = new List<RoomDetail>();
            //    SelectedRoomListHandler.Invoke(Roomdetaillist);
        }
        private void SelectedRoomListEventHandler(List<RoomDetailDTO> room)
        {
            if (room != null && room.Count > 0)
            {
                // Create Registration list
                AddMultipleRegistration(room);

            }
            else
                XtraMessageBox.Show("Select Room Please", "CNET_ERPV6", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnDefRate_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            Dictionary<String, String> args = new Dictionary<string, string>
            {
                {"fromDate", deDefaultArrivalDate.Text},
                {"toDate", deDefaultDepartureDate.Text},
                {"adultCount", nudDefaultAdult.Value.ToString()},
                {"childCount", nudDefaultChild.Value.ToString()},
                {"roomCount", "1"},
                {"rateCode",""},
                {"roomType", ""}
            };


            _RoomDefulatrateSearchForm = new frmRateSearch();
            _RoomDefulatrateSearchForm.SelectedHotelcode = SelectedHotelcode.Value;
            _RoomDefulatrateSearchForm.LoadData(this, args);
            _RoomDefulatrateSearchForm.RateSelected += frmDefaultRateSearch_RateSelected;
            _RoomDefulatrateSearchForm.ShowDialog();

            if (_RoomDefulatrateSearchForm.frmRateDetailInfo != null)
            {
                DefaultdailyRateCodeList = _RoomDefulatrateSearchForm.frmRateDetailInfo.dailyRateCodeList;
            }


        }


        private void AddMultipleRegistration(List<RoomDetailDTO> roomList)
        {

            List<GroupVM> addedDtoList = gvGroupReg.DataSource as List<GroupVM>;
            if (addedDtoList == null)
                addedDtoList = new List<GroupVM>();



            foreach (RoomDetailDTO room in roomList)
            {
                var RoomAdded = addedDtoList.FirstOrDefault(r => r.RoomCode == room.Id);
                if (RoomAdded != null)
                {
                    SystemMessage.ShowModalInfoMessage(string.Format("Room Number {0} is already added", RoomAdded.Room), "ERROR");

                    return;
                }


                GroupVM dto = new GroupVM()
                {
                    Adult = Convert.ToInt16(nudDefaultAdult.Value),
                    Arrival = deDefaultArrivalDate.DateTime,
                    Child = Convert.ToInt16(nudDefaultChild.Value),
                    Departure = deDefaultDepartureDate.DateTime,
                    Guest = (sleDefaultGuest.EditValue != null && !string.IsNullOrEmpty(sleDefaultGuest.EditValue.ToString())) ? sleDefaultGuest.Text : null,
                    GuestCode = (sleDefaultGuest.EditValue != null && !string.IsNullOrEmpty(sleDefaultGuest.EditValue.ToString())) ? Convert.ToInt32(sleDefaultGuest.EditValue) : null,
                    Rate = _DefaultselectedRateHeader == null ? null : _DefaultselectedRateHeader.Description,
                    Numberofnight = Convert.ToInt32((deDefaultDepartureDate.DateTime.Date - deDefaultArrivalDate.DateTime.Date).TotalDays),
                    Room = room.Description,
                    RoomCode = room.Id,
                    RateAmount = _currentDefaultRateAmount,
                };
                dto.TotalRateAmount = dto.Numberofnight == 0 ? dto.RateAmount : dto.Numberofnight * dto.RateAmount;


                var rType = _allRoomTypes.FirstOrDefault(rt => rt.Id == room.RoomType);
                dto.RoomType = rType.Description;
                dto.RoomTypeCode = room.RoomType;

                dto.RateHeader = _DefaultselectedRateHeader == null ? null : _DefaultselectedRateHeader;
                dto.DilyRateCodeList = new List<DailyRateCodeDTO>();
                if (DefaultdailyRateCodeList != null && DefaultdailyRateCodeList.Count > 0)
                    dto.DilyRateCodeList.AddRange(DefaultdailyRateCodeList);

                addedDtoList.Add(dto);
            }

            _currentRoomCode = null;
            _currentRoomType = null;
            _selectedRateHeader = null;
            btnRate.EditValue = null;
            if (dailyRateCodeList != null)
                dailyRateCodeList.Clear();

            teRateAmount.EditValue = null;
            btnRoom.EditValue = null;
            slueGuest.EditValue = null;

            _CellselectedRateHeader = null;
            btnDefRate.EditValue = null;
            if (DefaultdailyRateCodeList != null)
                DefaultdailyRateCodeList.Clear();

            teDefRateAmount.EditValue = null;
            sleDefaultGuest.EditValue = null;

            nudDefaultAdult.Value = 1;
            nudDefaultChild.Value = 0;

            DateTime? date = UIProcessManager.GetServiceTime();
            if (date != null)
                deDefaultArrivalDate.DateTime = date.Value;

            gcGroupReg.DataSource = addedDtoList;
            gvGroupReg.RefreshData();
        }

        private void repositoryItemSpinEditAdult_EditValueChanged(object sender, EventArgs e)
        {
            gvGroupReg.PostEditor();
            GroupVM ChangedRate = (GroupVM)gvGroupReg.GetFocusedRow();
            ChangedRate.Numberofnight = Convert.ToInt32((ChangedRate.Departure.Date - ChangedRate.Arrival.Date).TotalDays);
            ChangedRate.Rate = null;
            ChangedRate.RateHeader = null;
            ChangedRate.RateAmount = 0;
            ChangedRate.TotalRateAmount = 0;
            ChangedRate.DilyRateCodeList = new List<DailyRateCodeDTO>();
            gvGroupReg.RefreshData();
            gvGroupReg.RefreshRow(gvGroupReg.FocusedRowHandle);
        }



        private void repositoryItemDateEditArrival_EditValueChanged(object sender, EventArgs e)
        {
            gvGroupReg.PostEditor();
            GroupVM ChangedRate = (GroupVM)gvGroupReg.GetFocusedRow();
            ChangedRate.Rate = null;

            if (ChangedRate.Arrival.Date >= ChangedRate.Departure.Date)
                ChangedRate.Departure = ChangedRate.Arrival.Date.AddDays(1);

            ChangedRate.Numberofnight = Convert.ToInt32((ChangedRate.Departure.Date - ChangedRate.Arrival.Date).TotalDays);

            ChangedRate.RateHeader = null;
            ChangedRate.RateAmount = 0;
            ChangedRate.TotalRateAmount = 0;
            ChangedRate.DilyRateCodeList = new List<DailyRateCodeDTO>();
            gvGroupReg.RefreshData();
            gvGroupReg.RefreshRow(gvGroupReg.FocusedRowHandle);
            DateTime dtArrival = (DateTime)deChildArrival.EditValue;
            DateTime dtDeparture = deChildDeparture.DateTime;

        }

        private void deDefaultArrivalDate_EditValueChanged(object sender, EventArgs e)
        {

            DateTime dtArrival = (DateTime)deDefaultArrivalDate.EditValue;
            DateTime dtDeparture = deDefaultDepartureDate.DateTime;
            if (dtArrival.Date >= dtDeparture.Date)
            {
                deDefaultDepartureDate.DateTime = dtArrival.AddDays(1);

            }

            _DefaultselectedRateHeader = null;
            _DefaultavailalbePackages = null;
            DefaultdailyRateCodeList = null;
            btnDefRate.EditValue = null;
            teDefRateAmount.EditValue = null;
            _currentDefaultRateAmount = 0;
        }

        private void deDefaultDepartureDate_EditValueChanged(object sender, EventArgs e)
        {

            DateTime dtArrival = (DateTime)deDefaultArrivalDate.DateTime;
            if (deDefaultArrivalDate.EditValue == null) return;

            DateTime dtDeparture = (DateTime)deDefaultArrivalDate.EditValue;

            if (dtArrival.Date > dtDeparture.Date)
            {
                deDefaultArrivalDate.DateTime = dtArrival.AddDays(1);
            }


            _DefaultselectedRateHeader = null;
            _DefaultavailalbePackages = null;
            DefaultdailyRateCodeList = null;
            btnDefRate.EditValue = null;
            teDefRateAmount.EditValue = null;
            _currentDefaultRateAmount = 0;
        }

        private void repositoryItemSearchLookUpEditGuest_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            frmPerson frmperson = new frmPerson("Guest");
            frmperson.GSLType = CNETConstantes.GUEST;
            frmperson.Text = "Guest";
            frmperson.rpgScanFingerPrint.Visible = true;
            frmperson.LoadEventArg.Args = "Guest";
            frmperson.LoadData(this, this);
            frmperson.LoadEventArg.Sender = null;
            if (frmperson.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ///GetPerson();
                if (frmperson.SavedPerson != null)
                {

                    LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Add(frmperson.SavedPerson);

                    //for master guest
                    sluGroup.Properties.DataSource = null;
                    sluGroup.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    sluGroup.Properties.View.RefreshData();


                    //for child guest
                    slueGuest.Properties.DataSource = null;
                    slueGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    slueGuest.Properties.View.RefreshData();


                    sleDefaultGuest.Properties.DataSource = null;
                    sleDefaultGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    sleDefaultGuest.Properties.View.RefreshData();

                    repositoryItemSearchLookUpEditGuest.DataSource = null;
                    repositoryItemSearchLookUpEditGuest.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                    repositoryItemSearchLookUpEditGuest.View.RefreshData();

                    e.Cancel = false;
                    e.NewValue = frmperson.SavedPerson.Id;

                    GroupVM ChangedRate = (GroupVM)gvGroupReg.GetFocusedRow();
                    ChangedRate.GuestCode = frmperson.SavedPerson.Id;
                    gvGroupReg.RefreshData();
                    gvGroupReg.RefreshRow(gvGroupReg.FocusedRowHandle);
                }
            }


            /*  SearchLookUpEdit viewsluGuest = sender as SearchLookUpEdit;

              //if (_frmNewGuest == null)
              _frmNewGuest = new frmNewGuest();


              _frmNewGuest.SavedGuest = null;

              if (_frmNewGuest.ShowDialog() == DialogResult.OK)
              {
                  if (_frmNewGuest.SavedGuest != null)
                  {



                      AddedGuest p = _frmNewGuest.SavedGuest;
                      VwConsigneeViewDTO gDto = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.FirstOrDefault(g => g.Id == p.Id);
                      if (gDto == null)
                      {
                          gDto = new VwConsigneeViewDTO();
                      }
                      gDto.Id = p.Id.Value;
                      gDto.FirstName = p.FirstName;
                      gDto.SecondName = p.MiddleName;
                      gDto.ThirdName = p.LastName;
                      gDto.BioId = p.IDNumber;

                      _addedGuests.Add(_frmNewGuest.SavedGuest);
                      LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist.Add(gDto);

                      //for master guest
                      sluGroup.Properties.DataSource = null;
                      sluGroup.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                      sluGroup.Properties.View.RefreshData();


                      //for child guest
                      slueGuest.Properties.DataSource = null;
                      slueGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                      slueGuest.Properties.View.RefreshData();


                      sleDefaultGuest.Properties.DataSource = null;
                      sleDefaultGuest.Properties.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                      sleDefaultGuest.Properties.View.RefreshData();

                      repositoryItemSearchLookUpEditGuest.DataSource = null;
                      repositoryItemSearchLookUpEditGuest.DataSource = LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist;
                      repositoryItemSearchLookUpEditGuest.View.RefreshData();

                      GroupVM ChangedRate = (GroupVM)gvGroupReg.GetFocusedRow();
                      ChangedRate.GuestCode = gDto.Id;
                      gvGroupReg.RefreshData();
                      gvGroupReg.RefreshRow(gvGroupReg.FocusedRowHandle);

                      _frmNewGuest.SavedGuest = null;

                  }
              }


          */

        }

        private void sluGroup_EditValueChanged(object sender, EventArgs e)
        {

            if (sluGroup.EditValue != null)
                slueGuest.EditValue = sluGroup.EditValue;
        }
    }
}
