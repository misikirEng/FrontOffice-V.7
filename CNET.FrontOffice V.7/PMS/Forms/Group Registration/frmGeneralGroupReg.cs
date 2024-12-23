 
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.APICommunication;
using CNET.FrontOffice_V._7.Forms;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Group_Registration
{
    public partial class frmGeneralGroupReg: UserControl
    {
        private UILogicBase _parentForm;

        private List<RoomTypeDTO> _roomTypeList;

        private frmRateSearch _rateSearchForm;

        private int _selectedRoomType;
        private RateCodeHeaderDTO _selectedRateHeader;
        private decimal _currentRateAmount;
        private decimal _minRateAdju = 0;

        private List<RateCodePackageDTO> _availalbePackages = null;


        private List<TempDailyRateCodeVM> _temRateCodeList = new List<TempDailyRateCodeVM>();
        private List<DailyRateCode> dailyRateCodeList = new List<DailyRateCode>();

        private List<AddedDTO> _addedDtoList = new List<AddedDTO>();
        private List<AddedCompany> _addedCompanies = new List<AddedCompany>();
        private List<AddedGuest> _addedGuests = new List<AddedGuest>();
         

        private frmNewCompany _frmNewCompany;
        private frmNewGuest _frmNewGuest;
        private frmNote _frmNote;

       
        private List<RateCodeHeaderDTO> _rateHeaderList;
        private List<RoomDetailDTO> _allRoomDetails;

        private int _actDefCompany;
        private int _actDefPerson;
        private int _actDefReg;
        private int _activityDefMsg;
        private int _adSeen;
        public DateTime CheckOutDateTime { get; set; }

        public frmGeneralGroupReg (UILogicBase parentForm)
        {
            if (parentForm == null) return;
            _parentForm = parentForm;
            InitializeComponent();

            InitializeUI();
            ConfigurationDTO Checkoutconfig = LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_CheckOutTime);
            if (Checkoutconfig != null)
            {
                CheckOutDateTime = Convert.ToDateTime(Checkoutconfig.CurrentValue);
            }

        }



        /*************************** Helper Methods ***************************/
        #region Helper Method
        private void InitializeUI()
        {
            //Customer 
            GridColumn columnCompany = sluCustomer.Properties.View.Columns.AddField("code");
            columnCompany.Visible = true;
            columnCompany = sluCustomer.Properties.View.Columns.AddField("name");
            columnCompany.Visible = true;
            columnCompany = sluCustomer.Properties.View.Columns.AddField("idNumber");
            columnCompany.Visible = true;
            sluCustomer.Properties.DisplayMember = "name";
            sluCustomer.Properties.ValueMember = "code";

            //Guest 
            GridColumn column = repoSearchLookupGuest.View.Columns.AddField("code");
            column.Visible = true;
            column = repoSearchLookupGuest.View.Columns.AddField("name");
            column.Visible = true;
            column = repoSearchLookupGuest.View.Columns.AddField("idNumber");
            column.Visible = true; 
            repoSearchLookupGuest.DisplayMember = "name";
            repoSearchLookupGuest.ValueMember = "code";

            //State
            repoLookupState.Columns.Add(new LookUpColumnInfo("description", "State"));
            repoLookupState.DisplayMember = "description";
            repoLookupState.ValueMember = "code";

            repoSearchLookupGuest.AddNewValue += RepoSearchLookupGuest_AddNewValue;
            repoSearchLookupGuest.EditValueChanged += RepoSearchLookupGuest_EditValueChanged;

            spinNoOfNight.Properties.MinValue = 0;
            spinNoOfNight.Properties.MaxValue = 1000;

            spinRoomAdd.Properties.MinValue = 0;
            spinRoomAdd.Properties.MaxValue = 1000;


            repoSpinMattress.MinValue = 0;
            repoSpinMattress.MaxValue = 1000;

        }



        private bool InitializeData()
        {
            try
            {
                int totalTask = 14;

                CNETInfoReporter.WaitForm("Getting Date and Time", "Please Wait...", 1, totalTask);

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    CNETInfoReporter.Hide();
                    return false;
                }


                CNETInfoReporter.WaitForm("Checking Rate Adjustment Security Authorization", "Please Wait...", 2, totalTask);
                EnableDisableSecuredComponents();


                CNETInfoReporter.WaitForm("Getting All Room Details", "Please Wait...", 3, totalTask);
                //Room Detail
                _allRoomDetails = UIProcessManager.SelectAllRoomDetail().GroupBy(r => r.Description).Select(r => r.First()).ToList();
                if (_allRoomDetails == null || _allRoomDetails.Count == 0)
                {
                    CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define rooms first.", "ERROR");
                    return false;
                }

                deArrival.Properties.MinValue = CurrentTime.Value;
                deDeparture.Properties.MinValue = CurrentTime.Value;

                deArrival.EditValue = CurrentTime.Value;
                teETA.Time = CurrentTime.Value;

                deDeparture.EditValue = CurrentTime.Value.AddDays(1);

                spinNoOfNight.EditValue = 1;

                //Check Workflow


                CNETInfoReporter.WaitForm("Checking Group Registration Workflow", "Please Wait...", 4, totalTask);
                /** Group Registration **/
                //Six PM
                vw_WorkFlowByReferenceView workflow = UIProcessManager.GetWorkFlowsByreference(CNETConstantes.REGISTRATION_VOUCHER.ToString(),
                   CNETConstantes.VOUCHER_COMPONENET).Where(w => w.description == CNETConstantes.LU_ACTIVITY_GROUP_REGISTERATION)
                   .FirstOrDefault();

                if (workflow != null)
                {

                    _actDefReg = workflow.code;
                }
                else
                {
                    CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of GROUP REGISTRATION for Registration Voucher ", "ERROR");
                    return false;
                }

                CNETInfoReporter.WaitForm("Checking activity previlage for group registration workflow", "Please Wait...", 5, totalTask);
                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByActivityDefination(_actDefReg).FirstOrDefault(r => r.role == userRoleMapper.Role && r.needsPassCode);
                    if (roleActivity != null)
                    {
                        FingerPrintRecogLib.UI.frmNeedPassword frmNeedPass = new FingerPrintRecogLib.UI.frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            CNETInfoReporter.Hide();
                            return false;
                        }

                    }

                }

                CNETInfoReporter.WaitForm("Getting Company Maintained workflow", "Please Wait...", 6, totalTask);
                /** Company **/
                vw_WorkFlowByReferenceView workFlowCompany = UIProcessManager.GetWorkFlowsByreference(CNETConstantes.CUSTOMER.ToString(),
                       CNETConstantes.GSLTYPELIST).Where(w => w.description == CNETConstantes.MAINTAINED)
                       .FirstOrDefault();

                if (workFlowCompany != null)
                {
                    _actDefCompany = workFlowCompany.code;
                }
                else
                {
                    CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of MAINTAINED for Customer (Organization).", "ERROR");
                    return false;
                }


                CNETInfoReporter.WaitForm("Getting Guest Maintained workflow", "Please Wait...", 7, totalTask);
                /** Person **/
                vw_WorkFlowByReferenceView workflowPerson = UIProcessManager.GetWorkFlowsByreference(CNETConstantes.GUEST.ToString(),
                       CNETConstantes.GSLTYPELIST).Where(w => w.description == CNETConstantes.MAINTAINED)
                       .FirstOrDefault();

                if (workflowPerson != null)
                {

                    _actDefPerson = workflowPerson.code;
                }
                else
                {
                    CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of MAINTAINED for Person.", "ERROR");
                    return false;
                }

                /** Get Message Made Workflow **/

                CNETInfoReporter.WaitForm("Getting Message Made workflow", "Please Wait...", 8, totalTask);

                vw_WorkFlowByReferenceView workflowMsgMade = UIProcessManager.GetWorkFlowsByreference(CNETConstantes.MESSAGE.ToString(),
                   CNETConstantes.VOUCHER_COMPONENET).Where(w => w.description == CNETConstantes.LU_ACTIVITY_DEFINATION_MESSAGEMADE)
                   .FirstOrDefault();

                if (workflowMsgMade != null)
                {

                    _activityDefMsg = workflowMsgMade.code;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Please define workflow of MESSAGE MADE for MESSAGE Voucher ", "ERROR");
                    CNETInfoReporter.Hide();
                    return false;
                }

                CNETInfoReporter.WaitForm("Getting Message Seen workflow", "Please Wait...", 9, totalTask);

                //check seen and deleted workflow
                vw_WorkFlowByReferenceView workFlowSeen = UIProcessManager.GetWorkFlowsByreference(CNETConstantes.MESSAGE.ToString(),
                   CNETConstantes.VOUCHER_COMPONENET).Where(w => w.description == CNETConstantes.LU_ACTIVITY_SEEN)
                   .FirstOrDefault();

                if (workFlowSeen != null)
                {

                    _adSeen = workFlowSeen.code;
                }
                else
                {
                    CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Please define workflow of SEEN for Message Voucher", "CNET ERPv6", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }


                // state list
                var states = LocalBuffer.SystemConstantDTOBufferList.Where(s =>
                      s.Id == CNETConstantes.SIX_PM_STATE ||
                      s.Id == CNETConstantes.OSD_WAITLIST_STATE ||
                      s.Id == CNETConstantes.GAURANTED_STATE ||
                      s.Id == CNETConstantes.CHECKED_IN_STATE
                      ).ToList();
                repoLookupState.DataSource = states;
                beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;


                CNETInfoReporter.WaitForm("Getting All Rate Headers", "Please Wait...", 10, totalTask);
                // Rate Headers
                _rateHeaderList = UIProcessManager.SelectAllRateCodeHeader();
                if (_rateHeaderList == null || _rateHeaderList.Count == 0)
                {
                    CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define rate first.", "ERROR");
                    return false;
                }



                CNETInfoReporter.WaitForm("Getting Minimum Rate Adjustment Amount Config", "Please Wait...", 11, totalTask);
                //Minimum Rate Adjustment Value
                var config = LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_MinRateAdjustment);
                if (config != null)
                {
                    _minRateAdju = Convert.ToDecimal(config.CurrentValue);
                }


                CNETInfoReporter.WaitForm("Getting Mattress Amount Config", "Please Wait...", 12, totalTask);
               



                CNETInfoReporter.WaitForm("Getting All Guest List", "Please Wait...", 13, totalTask);
                //Guest List
                repoSearchLookupGuest.DataSource = LocalBuffer.AllGuestVoucherConsignee;


                CNETInfoReporter.WaitForm("Getting All Company List", "Please Wait...", 14, totalTask);
                //Company List
                //var orgCompanyList = LocalBuffer.OrganizationBufferList.Where(o => o.type == CNETConstantes.CUSTOMERORG && o.isActive).ToList();
                //if (orgCompanyList != null)
                //{

                //    foreach (var con in orgCompanyList)
                //    {
                //        vw_VoucherConsignee dto = new vw_VoucherConsignee();
                //        dto.code = con.code;
                //        dto.name = con.tradeName;

                //        //Identification
                //        var ident = LocalBuffer.IdentificationBufferList.FirstOrDefault(i => i.reference == con.code);
                //        dto.idNumber = ident == null ? "" : ident.idNumber;
                //        if (!LocalBuffer.AllCompanyVoucherConsignee.Contains(dto))
                //        {
                //            LocalBuffer.AllCompanyVoucherConsignee.Add(dto);
                //        }
                //    }
                    sluCustomer.Properties.DataSource = LocalBuffer.AllCompanyVoucherConsignee;
               // }


                CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing group registraion. DETAIL:: " + ex.Message, "ERROR");
                return false;
            }
        }



        private void PopulateRoomTypes(DateTime currentTime)
        {
            gcRoomType.DataSource = null;
            gvRoomType.RefreshData();
            checkBxList.Items.Clear();

            //Room Type List
            _roomTypeList = UIProcessManager.SelectAllRoomType().Where(r => r.IsActive && (r.ActivationDate != null && r.ActivationDate.Value.Date <= currentTime.Date)).ToList();
            if (_roomTypeList != null)
            {
                //<Descripion, Day, Minimum Available Rooms>
                List<RoomTypeVM> _roomTypeDTOList = new List<RoomTypeVM>();
                foreach (var roomType in _roomTypeList)
                {
                    RoomTypeVM dto = new RoomTypeVM();
                    dto.Id = roomType.Id;
                    dto.Description = roomType.Description;
                    List<RoomDetailDTO> roomDetails = _allRoomDetails.Where(r => r.RoomType == roomType.Id).ToList();
                    if (roomDetails != null)
                    {
                        dto.Total = roomDetails.Count;
                        dto.RoomDetails = roomDetails;
                    }

                    List<string> avRooms = GetAvailableRooms(dto.Id, deArrival.DateTime, deDeparture.DateTime);
                    if (avRooms != null && avRooms.Count > 0)
                    {
                        dto.Balance = avRooms.Count;
                        dto.AvailableRooms = avRooms;
                    }
                    else
                    {
                        dto.Balance = 0;
                        dto.AvailableRooms = new List<string>();

                    }

                    dto.Occupied = dto.Total - dto.Balance;

                    _roomTypeDTOList.Add(dto);
                }

                gcRoomType.DataSource = _roomTypeDTOList;
                gvRoomType.RefreshData();

                if (_roomTypeDTOList.Count > 0)
                    gvRoomType.FocusedRowHandle = 0;

            }



        }

        private void RefreshRoomsList()
        {
            RoomTypeDTO dto = gvRoomType.GetFocusedRow() as RoomTypeDTO;
            if (dto == null) return;
            checkBxList.Items.Clear();
            if (dto.RoomDetails != null)
            {
                List<AddedDTO> addedDTOs = gvAddedRooms.DataSource as List<AddedDTO>;
                if (addedDTOs != null && addedDTOs.Count > 0)
                {
                    string[] addedRooms = addedDTOs.Select(r => r.RoomNumber).ToArray();
                    checkBxList.Items.AddRange(dto.RoomDetails.Where(r => dto.AvailableRooms.Contains(r.code) && !addedRooms.Contains(r.description)).Select(rd => new CheckedListBoxItem() { Value = rd.description, Description = rd.description }).ToArray());

                }
                else
                {
                    checkBxList.Items.AddRange(dto.RoomDetails.Where(r => dto.AvailableRooms.Contains(r.code)).Select(rd => new CheckedListBoxItem() { Value = rd.description, Description = rd.description }).ToArray());
                }
            }

            checkBxList.Refresh();
        }

        private void AddRegistrationRooms(bool isAll)
        {
            if (_selectedRateHeader == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select rate first.", "ERROR");
                return;
            }

            if (checkBxList.Items == null || checkBxList.Items.Count == 0) return;

            if (!isAll && spinRoomAdd.EditValue != null && !string.IsNullOrEmpty(spinRoomAdd.EditValue.ToString()))
            {
                int value = Convert.ToInt32(spinRoomAdd.EditValue.ToString());
                if (value < 0) return;
                if (checkBxList.Items.Count < value)
                {
                    SystemMessage.ShowModalInfoMessage("No enough rooms!", "ERROR");
                    return;
                }

                for (int i = 0; i < value; i++)
                {
                    checkBxList.SetItemChecked(i, true);
                }
            }

            spinRoomAdd.EditValue = "0";

            var checkedItems = checkBxList.CheckedItems;
            if (checkedItems != null && checkedItems.Count > 0)
            {


                foreach (var item in checkedItems)
                {

                    AddedDTO dto = new AddedDTO();
                    dto.RoomNumber = item.ToString();
                    dto.RoomType = _roomTypeList.FirstOrDefault(rt => rt.Id == _selectedRoomType).Description;
                    dto.RoomTypeCode = _selectedRoomType;

                    int defAdult = 1;
                    var roomDetail = _allRoomDetails.FirstOrDefault(r => r.Description == dto.RoomNumber);
                    if (roomDetail != null)
                    {
                        defAdult = roomDetail.MaxOccupnancy == null || roomDetail.MaxOccupnancy.Value == 0 ? 1 : roomDetail.MaxOccupnancy.Value;
                    }

                    dto.AdultNo = defAdult;
                    dto.DefAdultNo = defAdult;
                    dto.Mattress = 0;

                    DateTime arrival = new DateTime(deArrival.DateTime.Year, deArrival.DateTime.Month,
                        deArrival.DateTime.Day, teETA.Time.Hour, teETA.Time.Minute, teETA.Time.Second);
                    dto.ArrivalDate = arrival;
                    dto.DepartureDate = deDeparture.DateTime;
                    dto.NightNo = Convert.ToInt32(spinNoOfNight.Value);
                    if (sluCustomer.EditValue != null && !string.IsNullOrEmpty(sluCustomer.EditValue.ToString()))
                    {
                        dto.CompanyCode = sluCustomer.EditValue.ToString();
                    }


                    if (_availalbePackages != null && _availalbePackages.Count >= 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var pkg in _availalbePackages)
                        {
                            PackageHeaderDTO packageHeader = UIProcessManager.GetPackageHeaderById(pkg.PackageHeader);
                            sb.Append(packageHeader.Description);
                            sb.Append(",");
                        }

                        dto.Package = sb.ToString();
                    }

                    dto.RateDescription = _selectedRateHeader.Description;
                    dto.RateCode = _selectedRateHeader.Id;
                    dto.RateAmount = _currentRateAmount;
                    dto.DefRate = _currentRateAmount;
                    dto.Total = dto.NightNo <= 0 ? dto.RateAmount : dto.RateAmount * dto.NightNo;


                    dto.AddedRate = new AddedRate()
                    {
                        Code = _selectedRateHeader.Id,
                        Description = _selectedRateHeader.Description,
                    };

                    dto.AddedRate.DailyRateCodeList = new List<DailyRateCode>();
                    dto.AddedRate.Packages = new List<PackageHeaderDTO>();

                    if (_availalbePackages != null && _availalbePackages.Count > 0)
                    {
                        foreach (RateCodePackageDTO package in _availalbePackages)
                        {
                            PackageHeaderDTO packageHeader = UIProcessManager.GetPackageHeaderById(package.PackageHeader);
                            dto.AddedRate.Packages.Add(packageHeader);
                        }
                    }
                    if (dailyRateCodeList != null && dailyRateCodeList.Count > 0)
                        dto.AddedRate.DailyRateCodeList.AddRange(dailyRateCodeList);


                    PerformRateAdjustment(dto, teRateAdjust, radioGroupAdjust);

                    _addedDtoList.Add(dto);


                }



                gcAddedRooms.DataSource = _addedDtoList;
                gvAddedRooms.RefreshData();



                RefreshRoomsList();



            }
        }

        private void PerformRateAdjustment(AddedDTO dto, TextEdit view, RadioGroup radioGroupAdjust)
        {
            if (view.EditValue == null || string.IsNullOrEmpty(view.EditValue.ToString())) return;

            if (dto == null) return;

            if (radioGroupAdjust.SelectedIndex == 0)
            {
                //In Percent
                decimal value = Convert.ToDecimal(view.EditValue.ToString());
                var finalAmount = dto.DefRate - (dto.DefRate * value / 100);

                dto.RateAmount = finalAmount < _minRateAdju ? dto.DefRate : finalAmount;
                dto.Total = dto.NightNo == 0 ? dto.RateAmount : dto.RateAmount * dto.NightNo;
                dto.RateAdjustValue = finalAmount < _minRateAdju ? 0 : value;
                dto.IsPercent = true;

            }
            else
            {
                //In Amount
                decimal value = Convert.ToDecimal(view.EditValue.ToString());
                var finalAmount = dto.DefRate - value;

                dto.RateAmount = finalAmount < _minRateAdju ? dto.DefRate : finalAmount;
                dto.Total = dto.NightNo == 0 ? dto.RateAmount : dto.RateAmount * dto.NightNo;
                dto.RateAdjustValue = finalAmount < _minRateAdju ? 0 : value;
                dto.IsPercent = false;

            }

            gvAddedRooms.RefreshData();
        }

        private void PopulateFocusedRowAdjustment(AddedDTO dto)
        {
            bool isPercent = dto.IsPercent;
            decimal rateAdj = dto.RateAdjustValue;



            if (isPercent)
            {
                radioGroupAdjust.SelectedIndex = 0;
            }
            else
            {
                radioGroupAdjust.SelectedIndex = 1;
            }

            teRateAdjust.EditValue = rateAdj;
        }

        private List<string> GetAvailableRooms(int roomType, DateTime arrivalDate, DateTime departureDate)
        {

            List<RoomTypeDTO> pseudoRoomList =UIProcessManager.GetRoomTypeByispseudoRoomType(true);
            List<string> avRooms = new List<string>();
            List<int> pseudoRooms = new List<int>();
            if (pseudoRoomList != null)
                pseudoRooms = pseudoRoomList.Select(p => p.Id).ToList();

            List<int> stateList = new List<int> () { CNETConstantes.CHECKED_OUT_STATE, CNETConstantes.OSD_CANCEL_STATE };

            if (roomType >0)
            {
                var avRoomList = UIProcessManager.GetAvailabeRoomsByDateAndState(arrivalDate.Date, departureDate.Date,
                stateList).Where(r => r.RoomTypeCode == roomType).ToList();
                if (avRoomList != null && avRoomList.Count > 0)
                {
                    avRooms = avRoomList.Select(r => r.code).ToList();
                }

            }

            return avRooms;


        }

        private ConsigneeDTO SaveGuest(AddedGuest guest, DateTime currentTime, int activityDef)
        {
            try
            {
                ConsigneeDTO person = null;

                bool isSaved = false;
                List<VwConsigneeViewDTO> personsList = LocalBuffer.ConsgineeViewBufferList.Where(p => p.Type == CNETConstantes.GUEST || p.Type == CNETConstantes.CONTACT).ToList();
                VwConsigneeViewDTO personSaved = personsList.FirstOrDefault(p => p.FirstName.ToLower() == guest.FirstName.ToLower() && p.ThirdName.ToLower() == guest.LastName.ToLower() &&
                            p.SecondName.ToLower() == guest.MiddleName.ToLower());
                //List<Identification> idLists = new List<Identification>();
                //if (personSaved != null)
                //{
                //    idLists = LocalBuffer.IdentificationBufferList.Where(i => i.reference == personSaved.code).ToList();
                //    if ((idLists.Select(i => i.idNumber).ToList()).Contains(guest.IDNumber))
                //    {
                //        isSaved = true;
                //    }
                //}
                if (isSaved)
                {
                    SystemMessage.ShowModalInfoMessage(guest.FirstName + " " + guest.LastName + " " + guest.MiddleName + " already exists", "ERROR");
                    return person = new ConsigneeDTO() { Id = personSaved.Id, FirstName = personSaved.FirstName, ThirdName = personSaved.ThirdName, SecondName = personSaved.SecondName };

                }
                else
                {
                    string personCode = "";

                    var id = UIProcessManager.IdGenerater("Person", LocalBuffer.DeviceObject,
                            CNETConstantes.GUEST.ToString(),
                            CNETConstantes.PERSON, 1);
                    if (id != null)
                    {
                        personCode = id.GeneratedNewId;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to generate guest ID. ", "ERROR");
                        return null;
                    }


                    person = new ConsigneeDTO();
                    person.Code = personCode;
                    person.Title =null;
                    person.GslType = CNETConstantes.GUEST;
                    person.FirstName = guest.FirstName;
                    person.SecondName = guest.MiddleName;
                    person.ThirdName = guest.LastName;
                    person.IsActive = true;
                    person.StartDate = currentTime;
                    person.Nationality = guest.Nationality;
                    person.Gender = guest.Gender; 
                    person.Preference = "PREFE000000823";

                    ConsigneeDTO isCreated = UIProcessManager.CreateConsignee(person);
                    if (isCreated != null)
                    {
                        // Save Activity
                        var _activity = ActivityLogManager.SetupActivity(person.Id, currentTime);
                        int? activityCode = ActivityLogManager.CommitActivity(_activity, activityDef, person.GslType, null, CNETConstantes.PERSON);

                        //refresh Login Buffer List
                       // LocalBuffer.loadvw_AllPersonView();

                        //Update ID


                        //Save Identification
                        var ident = new IdentificationDTO
                        { 
                            Consignee = isCreated.Id,
                            Description = LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == guest.IDType).Description,
                            IdNumber = guest.IDNumber,
                            Type = guest.IDType
                        };

                        var isIdentSave = UIProcessManager.CreateIdentification(ident);
                        LocalBuffer.loadIdentification();

                        //Object State

                        throw new NotImplementedException();
                        var objectStateDefinitions = LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(s => s.Type == "");// CNETConstantes.GUEST.ToString());
                        if (objectStateDefinitions != null)
                        {
                            ObjectStateDTO objstaObjectState = new ObjectStateDTO
                            { 
                                Reference = isCreated.Id,
                                ObjectStateDefinition = objectStateDefinitions.Id
                            };
                            //UIProcessManager.CreateObjectState(new List<ObjectStateDTO>() { objstaObjectState });
                            UIProcessManager.CreateObjectState(objstaObjectState);
                            LocalBuffer.LoadObjectStateTable();

                        }
                         
                        //Address
                        if (!string.IsNullOrEmpty(guest.Telephone))
                        {
                            Address address = new Address();
                            address.code = "";
                            address.value = guest.Telephone;
                            address.reference = personCode;
                            address.preference = CNETConstantes.TELE_PHONE;

                            bool isAddressSaved = UIProcessManager.CreateAddress(new List<Address>() { address });
                        }
                        //ConsigneeUnit
                        ConsigneeUnitDTO consigneeUnit = new ConsigneeUnitDTO()
                        {
                            Name = "Main Consignee",
                            Type = CNETConstantes.ORG_UNIT_TYPE_BRUNCH,
                            Phone1 = guest.Telephone
                        };


                        return person;
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

        private ConsigneeBufferDTO SaveCompany(AddedCompany company, DateTime currentTime, int activityDef)
        {
            try
            {
                ConsigneeBufferDTO ConsigneeBufferDTO = new ConsigneeBufferDTO();
                ConsigneeBufferDTO.consignee = new ConsigneeDTO();
                ConsigneeBufferDTO.consigneeUnits = new List<ConsigneeUnitDTO>();   
                ConsigneeDTO org = null;

                ConsigneeDTO ExistConsignee = LocalBuffer.ConsigneeBufferList.FirstOrDefault(i => i.Tin == company.TIN);
                if (ExistConsignee != null)
                {
                    ConsigneeBufferDTO.consignee = ExistConsignee;
                    ConsigneeBufferDTO.consigneeUnits = UIProcessManager.GetConsigneeUnitByconsignee(ExistConsignee.Id);
                    return ConsigneeBufferDTO;
                }
                else
                {
                    string orgCode = "";

                    var id = UIProcessManager.IdGenerater("Person", LocalBuffer.DeviceObject,
                            CNETConstantes.CUSTOMERORG.ToString(),
                            CNETConstantes.ORGANIZATION, 1);
                    if (id != null)
                    {
                        orgCode = id.GeneratedNewId;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to generate Organization ID. ", "ERROR");
                        return null;
                    }

                    ConsigneeBufferDTO.consignee = new ConsigneeDTO()
                    {
                        GslType = CNETConstantes.CUSTOMER,
                        Code = orgCode,
                        FirstName = company.Name,
                        SecondName = company.Name,
                        IsActive = true,
                        Tin = company.TIN,
                        Preference = "PREFE000000804",
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
                    ConsigneeBufferDTO.Activity = ActivityLogManager.SetupActivity(currentTime, activityDef, CNETConstantes.ORGANIZATION);

                    ConsigneeBufferDTO isSaved = UIProcessManager.CreateConsigneeBuffer(ConsigneeBufferDTO);
                    if (isSaved != null)
                    { 
                        //Update ID Setting 

                        //Load Organization
                      //  LocalBuffer.loadOrganization();
 
                        //Save Organization Unit
                        // LocalBuffer.OrganizationUnitDefinitionBufferList.FirstOrDefault(oud => oud.code == "");
                        
                      
                        return isSaved;

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


        private void Reset()
        {
            beiState.EditValue = CNETConstantes.CHECKED_IN_STATE;
            sluCustomer.EditValue = null;
            btnEditRate.EditValue = null;
            teRate.Text = "";
            _selectedRateHeader = null;
            _availalbePackages = null;
            dailyRateCodeList = null;
            _currentRateAmount = 0;
            lciRateCode.AppearanceItemCaption.ForeColor = ColorTranslator.FromHtml("black");




            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null)
            {
                this._parentForm.Close();
            }

            deArrival.Properties.MinValue = CurrentTime.Value;
            deDeparture.Properties.MinValue = CurrentTime.Value;

            deArrival.EditValue = CurrentTime.Value;
            teETA.Time = CurrentTime.Value;

            deDeparture.EditValue = CurrentTime.Value.AddDays(1);
            spinNoOfNight.EditValue = 1;

            gcAddedRooms.DataSource = null;
            gvAddedRooms.RefreshData();

            _addedGuests.Clear();
            _addedCompanies.Clear();

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



                if (beiState.EditValue == null || string.IsNullOrEmpty(beiState.EditValue.ToString()))
                {
                    SystemMessage.ShowModalInfoMessage("Choose Registration State First!", "ERROR");
                    return;
                }


                List<AddedDTO> addedList = gvAddedRooms.DataSource as List<AddedDTO>;
                if (addedList == null || addedList.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("Noting to save! please add registrations first.", "ERROR");
                    return;
                }

                int totalTask = 14 * addedList.Count;

                CNETInfoReporter.WaitForm("Preparing to save registrations", "Please Wait", 1, totalTask);

                var nonGuestLists = addedList.Where(r =>  r.Guest >0 ).ToList();
                if (nonGuestLists != null && nonGuestLists.Count > 0)
                {
                    CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("There is a registration with No-Guest. Please assign a guest first.", "ERROR");
                    return;
                }

                //Get Current Date and Time
                CNETInfoReporter.WaitForm("Getting Current Time", "Please Wait", 1, totalTask);

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    CNETInfoReporter.Hide();
                    return;
                }

                var device = LocalBuffer.CurrentDevice;
                var user = LocalBuffer.CurrentLoggedInUser;

                List<string> savedRooms = new List<string>();

                int count = 1;

                //Looping over added
                foreach (var reg in addedList)
                {
                    count = count + 1;
                    string localvoCode = string.Empty;
                    int voId = 0;

                    int? guestCode = null;
                    int? companyCode = null;

                    int state = Convert.ToInt32(beiState.EditValue.ToString());
                    string stateDesc = "";
                    var osd = LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(o => o.Id == state);
                    if (osd != null)
                    {
                        stateDesc = osd.Description;
                    }


                    //Save Person
                    CNETInfoReporter.WaitForm("Saving Person", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                    var newGuest = _addedGuests.FirstOrDefault(g => g.Id == reg.Guest);
                    if (newGuest != null)
                    {

                        var personSaved = SaveGuest(newGuest, CurrentTime.Value, _actDefPerson);
                        if (personSaved == null)
                        {
                            SystemMessage.ShowModalInfoMessage("Guest is not saved for room number: " + reg.RoomNumber, "ERROR");
                            CNETInfoReporter.Hide();
                            continue;
                        }
                        else
                        {
                            guestCode = personSaved.Id;

                            _addedGuests.Remove(newGuest);
                        }

                    }
                    else
                    {
                        guestCode = reg.Guest;
                    }

                    //Save Company
                    CNETInfoReporter.WaitForm("Saving Company", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                    if (sluCustomer.EditValue != null && !string.IsNullOrEmpty(sluCustomer.EditValue.ToString()))
                    {
                        int compCode =Convert.ToInt32( sluCustomer.EditValue.ToString());
                        var newCompany = _addedCompanies.FirstOrDefault(g => g.Id == compCode);
                        if (newCompany != null)
                        {
                            ConsigneeBufferDTO companySaved = SaveCompany(newCompany, CurrentTime.Value, _actDefCompany);
                            if (companySaved == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Company is not saved for room number: " + reg.RoomNumber, "ERROR");
                                CNETInfoReporter.Hide();
                                continue;
                            }
                            else
                            {
                                companyCode = companySaved.consignee.Id;
                                _addedCompanies.Remove(newCompany);
                            }

                        }
                        else
                        {
                            companyCode = compCode;
                        }
                    }

                    CNETInfoReporter.WaitForm("Generating Registration ID", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                    Generated_ID id = null;
                    id = UIProcessManager.IdGenerater("Voucher", device,
                        CNETConstantes.REGISTRATION_VOUCHER.ToString(),
                        CNETConstantes.VOUCHER_COMPONENET, 1);
                    if (id != null)
                    {
                        localvoCode = id.GeneratedNewId;

                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                        CNETInfoReporter.Hide();
                        break;

                    }

                    DateTime departureDate = (reg.DepartureDate).Date.AddHours(CheckOutDateTime.Hour).AddMinutes(CheckOutDateTime.Minute);
                    VoucherDTO voucher = new VoucherDTO
                    { 
                        Id=0,
                        Code = localvoCode,
                        Type = CNETConstantes.TRANSACTIONTYPEBEGINGVOUCH, //beginning Voucher
                        Definition = CNETConstantes.REGISTRATION_VOUCHER, //Registration Voucher
                        Consignee1 = guestCode,
                        Consignee2 = companyCode, 
                        IssuedDate = CurrentTime.Value,
                        Year = CurrentTime.Value.Year,
                        Month = CurrentTime.Value.Month,
                        Date = CurrentTime.Value.Day,
                        StartDate = reg.ArrivalDate,
                        EndDate = departureDate,
                        IsIssued = true,
                        IsVoid = false,
                        GrandTotal = 0,
                        Period = null,
                        LastState = state
                    };

                    CNETInfoReporter.WaitForm("Creating Registration Voucher", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                    VoucherDTO isSaved = UIProcessManager.CreateVoucher(voucher);
                    if (isSaved != null)
                    {
                        voId = isSaved.Id;
                        CNETInfoReporter.WaitForm("Saving Activity", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                        //Activity
                        ActivityDTO activity = ActivityLogManager.SetupActivity(voucher.Id, CurrentTime.Value);
                        int? activityResult = ActivityLogManager.CommitActivity(activity, _actDefReg, CNETConstantes.REGISTRATION_VOUCHER, String.Format("State = {0}  Room = {1}  Arrival = {2}  Departure =  {3}", stateDesc, reg.RoomNumber, reg.ArrivalDate.ToShortDateString(), reg.DepartureDate.ToShortDateString()), CNETConstantes.PMS_Pointer);
                        if (activityResult != null)
                        {
                            voucher.LastActivity = activityResult;
                            UIProcessManager.UpdateVoucher(voucher);
                        }

                        CNETInfoReporter.WaitForm("Updating Id", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
         

                         


                        // Registration Previledge
                        CNETInfoReporter.WaitForm("Saving Registration Previledge", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                        RegistrationPrivllegeDTO previlage = new RegistrationPrivllegeDTO(); 
                        previlage.Voucher = voucher.Id;
                        previlage.Nopost = false;
                        previlage.AuthorizeDirectBill = false;
                        previlage.PreStayCharging = false;
                        //previlage.postStayCharging = false;
                        //previlage.AllowLatecheckout = false;
                        previlage.AuthorizeKeyReturn = false;
                        previlage.Remark = "";
                        bool isP = UIProcessManager.CreateRegistrationPrevilage(previlage);
                        if (!isP) continue;


                        //Registration Extension
                        //CNETInfoReporter.WaitForm("Saving Registratio Header", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                        //RegistrationExtension registrationHeader = new RegistrationExtension
                        //{
                        //    code = String.Empty, // only on algorithm invokation
                        //    arrivalDate = reg.ArrivalDate,
                        //    departureDate = departureDate,
                        //    voucher = voCode,
                            
                        //    origin = "",
                        //    resType = "",
                        //    Specials = "",
                        //    exchangeRate = null,
                        //    remark = memoPurposeTravel.EditValue == null ? "" : memoPurposeTravel.EditValue.ToString(),
                        //    paymentType = CNETConstantes.PAYMENTMETHODSCASH
                        //};
                       // string headerCode = UIProcessManager.CreateRegistrationExtension(registrationHeader);

                        //if (string.IsNullOrEmpty(headerCode))
                        //{
                        //    SystemMessage.ShowModalInfoMessage("Registration Header is not saved for room number: " + reg.RoomNumber, "ERROR");
                        //    CNETInfoReporter.Hide();
                        //    continue;

                        //}

                        CNETInfoReporter.WaitForm("Saving Rate Adjustment", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                        if (reg.RateAdjustValue > 0)
                        {
                            //create rate adjustment
                            RateAdjustmentDTO rateAdjustment = new RateAdjustmentDTO();
                            rateAdjustment.code = "";
                            rateAdjustment.registrationNo = voCode;
                            rateAdjustment.startDate = reg.ArrivalDate;
                            rateAdjustment.endDate = reg.DepartureDate;
                            rateAdjustment.amount = reg.RateAdjustValue;
                            rateAdjustment.value = reg.RateAmount;
                            rateAdjustment.isPercent = reg.IsPercent;
                            rateAdjustment.reason = "";
                            var isRateAdusted = UIProcessManager.CreateRateAdjustment(rateAdjustment);

                        }


                        //Registration Detail
                        CNETInfoReporter.WaitForm("Saving Registration Detail", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                        RegistrationDetailDTO regDetail = new RegistrationDetailDTO
                        { 
                            Date = CurrentTime,
                            Adult = reg.AdultNo,
                            Child = 0,
                            RoomCount = 1,
                            RoomType = reg.RoomTypeCode,
                            ActualRtc = reg.RoomTypeCode,
                            IsFixedRate = false,
                            Market = null,
                            Source = null,
                            RateAmount = reg.RateAmount,
                            adjustment = reg.RateAdjustValue,
                            Remark = reg.Mattress.ToString(),
                        };

                        var room = _allRoomDetails.FirstOrDefault(r => r.Description == reg.RoomNumber);
                        regDetail.Room = room.Id;


                        foreach (var drc in reg.AddedRate.DailyRateCodeList)
                        {
                            drc.UnitRoomRate = reg.DefRate;

                        }

                        List<RegistrationDetailDTO> _registrationDetails = new List<RegistrationDetailDTO>();
                        List<PackagesToPostDTO> _packagesToPosts = new List<PackagesToPostDTO>();
                        UIProcessManager.RegistrationDetailGenerator(registrationHeader, regDetail, reg.AddedRate.DailyRateCodeList, out _registrationDetails, out _packagesToPosts);

                        if (_registrationDetails == null || _registrationDetails.Count == 0)
                        {
                            SystemMessage.ShowModalInfoMessage("No registration detail generated. Please check the rate code given for Room Number: " + reg.RoomNumber, "ERROR");
                            CNETInfoReporter.Hide();
                            continue;
                        }

                        foreach (RegistrationDetailDTO registrationDetail in _registrationDetails)
                        {

                            registrationDetail.adjustment = reg.RateAdjustValue;
                            registrationDetail.Voucher = voId;

                            string oldRegistrationCode = registrationDetail.code;
                            string newRegistrationCode = UIProcessManager.CreateRegistrationDetail(registrationDetail);

                            // the next packagetopost comes from grid
                            //_packagesToPosts = new List<PackagesToPost>();
                            List<PackagesToPostDTO> perDetailPackagesToPosts =
                                _packagesToPosts.Where(p => p.RegistrationDetail == oldRegistrationCode).Distinct().ToList();

                            if (!string.IsNullOrEmpty(newRegistrationCode))
                            {
                                foreach (PackagesToPostDTO packagetoPost in perDetailPackagesToPosts)
                                { 
                                    packagetoPost.RegistrationDetail = newRegistrationCode;
                                    UIProcessManager.CreatePackagesToPost(packagetoPost);
                                }
                            }
                        }


                        //Save Log Message
                        CNETInfoReporter.WaitForm("Saving Log Message", string.Format("{0} of {1}", count - 1, addedList.Count), count, totalTask);
                        if (!string.IsNullOrEmpty(reg.LogMessage))
                        {
                            try
                            {

                                string vCode = "";
                                var msgID = UIProcessManager.IdGenerater("Voucher", device,
                                    CNETConstantes.MESSAGE.ToString(),
                                    CNETConstantes.VOUCHER_COMPONENET, 1);
                                if (msgID != null)
                                {
                                    vCode = msgID.GeneratedNewId;
                                }
                                else
                                {
                                }

                                if (!string.IsNullOrEmpty(vCode))
                                {
                                    VoucherDTO msgVoucher = new VoucherDTO();
                                    msgVoucher.Code = vCode;
                                    msgVoucher.Consignee1 = voucher.Consignee1;
                                    msgVoucher.Definition = CNETConstantes.MESSAGE;
                                    msgVoucher.Type = CNETConstantes.INTERNAL_MESSAGE; 
                                    msgVoucher.IssuedDate = CurrentTime.Value;
                                    msgVoucher.IsIssued = true;
                                    msgVoucher.Year = CurrentTime.Value.Year;
                                    msgVoucher.Month = CurrentTime.Value.Month;
                                    msgVoucher.Date = CurrentTime.Value.Day;
                                    msgVoucher.IsVoid = false;
                                    msgVoucher.Period = null;
                                    msgVoucher.Remark = "Message Voucher";
                                    msgVoucher.Note = reg.LogMessage;
                                    msgVoucher.LastState = CNETConstantes.OSD_UNREAD_STATE;

                                    isSaved = UIProcessManager.CreateVoucher(msgVoucher);
                                    if (isSaved != null)
                                    {
                                        //Save Activity
                                        ActivityDTO activityMsg = ActivityLogManager.SetupActivity(msgVoucher.Id, CurrentTime.Value);
                                        int? activityFlag = ActivityLogManager.CommitActivity(activityMsg, _activityDefMsg, CNETConstantes.MESSAGE, "", CNETConstantes.PMS_Pointer);
                                        if (activityFlag == null)
                                        {
                                            XtraMessageBox.Show("WARINING: activity log is not saved." + voucher.Code, "CNET_v2016", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                                        }

                                        //save activity Seen
                                        ActivityDTO activitySeen = ActivityLogManager.SetupActivity(msgVoucher.Id, CurrentTime.Value);
                                        int? activityFlagSeen = ActivityLogManager.CommitActivity(activitySeen, _adSeen, CNETConstantes.MESSAGE, "", CNETConstantes.PMS_Pointer);


                                        //Update ID setting


                                        //save transaction reference
                                        TransactionReferenceDTO tranRef = new TransactionReferenceDTO()
                                        {
                                            Referenced = voucher.Id,
                                            ReferencedVoucherDefn =voucher.Definition,
                                            Referring = msgVoucher.Id,
                                            ReferencingVoucherDefn = msgVoucher.Definition,
                                            Value = 0,

                                        };
                                        var isTranSaved = UIProcessManager.CreateTransactionReference(tranRef);

                                        //Refresh Log Buffer
                                        localBuffer.LoadLogBuffer();
                                    }
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        savedRooms.Add(reg.RoomNumber);

                    }




                }//end of outer loop


                //Refresh Added Registration Grid

                var savedRegList = addedList.Where(r => savedRooms.Contains(r.RoomNumber)).ToList();
                if (savedRegList != null && savedRegList.Count > 0)
                {
                    foreach (var reg in savedRegList)
                    {
                        addedList.Remove(reg);
                    }

                    gvAddedRooms.RefreshData();

                    SystemMessage.ShowModalInfoMessage("Registrations are Saved!", "MESSAGE");
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Registrations are not Saved!", "ERROR");
                }


                //Reset
                Reset();

                CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error has occured in saving registration. DETAIL:: " + ex.Message, "ERROR");
            }
        }

        #endregion

        #region Security Method

        private void EnableDisableSecuredComponents()
        {
            var allSecuredfunctions = GetAllSecuredFunctions();
            if (allSecuredfunctions == null || allSecuredfunctions.Count == 0) return;
            List<String> approvedFunctionalities = allSecuredfunctions.Where(x => x.category == "Amendment").Select(x => x.visuaCompDesc).ToList();
            if (approvedFunctionalities == null) return;
            if (!IsFunctionExists(approvedFunctionalities, "Rate Adjustment"))
            {
                teRateAdjust.Enabled = false;
                radioGroupAdjust.Enabled = false;
                toggleAdj.Enabled = false;
            }
        }

        private static bool IsFunctionExists(List<string> approvedFunctionalities, string selectedName)
        {
            foreach (String str in approvedFunctionalities)
            {
                if (str.ToLower().Trim().Equals(selectedName.ToLower().Trim()))
                {
                    return true;
                }
            }
            return false;
        }

        private List<viewFunctWithAccessM> GetAllSecuredFunctions()
        {
            List<viewFunctWithAccessM> retVal = new List<viewFunctWithAccessM>();

            try
            {

                String SubSystemComponent = CNETConstantes.SECURITYRegistrationDocument;
                string currentRole = "";
                var role = LocalBuffer.UserRoleMapperBufferList.Where(x => x.User == LocalBuffer.CurrentLoggedInUser.Id).FirstOrDefault();
                if (role != null)
                    currentRole = role.Role;

                retVal.AddRange(UIProcessManager.GetFuncwithAccessMatView(currentRole, "Amendment", SubSystemComponent).Where(x => x.access == true).ToList());

            }
            catch (Exception ex)
            {

            }
            return retVal;
        }

        #endregion


        /*************************** EVENT HANDLERS ***************************/
        #region Event Handler

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _parentForm.Close();
        }

        private void RepoSearchLookupGuest_EditValueChanged(object sender, EventArgs e)
        {
            gvAddedRooms.FocusedColumn = gvAddedRooms.Columns[0];
        }

        private void gvRoomType_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            btnEditRate.EditValue = null;
            teRate.Text = "";
            _selectedRateHeader = null;
            _availalbePackages = null;
            dailyRateCodeList = null;
            _currentRateAmount = 0;
            lciRateCode.AppearanceItemCaption.ForeColor = ColorTranslator.FromHtml("black");

            RefreshRoomsList();

        }

        private void btnCustomer_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (_frmNewCompany == null)
                _frmNewCompany = new frmNewCompany();
            if (_frmNewCompany.ShowDialog() == DialogResult.OK)
            {
                if (_frmNewCompany.SavedCompany != null)
                {
                    _addedCompanies.Add(_frmNewCompany.SavedCompany);

                    VwConsigneeViewDTO dto = new VwConsigneeViewDTO();
                    dto.Id = _frmNewCompany.SavedCompany.Id;
                    dto.FirstName = _frmNewCompany.SavedCompany.Name;
                    dto.Tin = _frmNewCompany.SavedCompany.TIN;
                    if (!LocalBuffer.AllCompanyVoucherConsignee.Contains(dto))
                    {
                        LocalBuffer.AllCompanyVoucherConsignee.Add(dto);
                    }

                    sluCustomer.EditValue = dto.Id;



                }
            }
        }

        private void RepoSearchLookupGuest_AddNewValue(object sender, AddNewValueEventArgs e)
        {
            AddedDTO dto = gvAddedRooms.GetFocusedRow() as AddedDTO;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select added registration first", "ERROR");
                return;
            }

            if (_frmNewGuest == null)
                _frmNewGuest = new frmNewGuest();
            if (_frmNewGuest.ShowDialog() == DialogResult.OK)
            {
                if (_frmNewGuest.SavedGuest != null)
                {
                    _addedGuests.Add(_frmNewGuest.SavedGuest);

                    AddedGuest p = _frmNewGuest.SavedGuest;

                    VwConsigneeViewDTO gDto = new VwConsigneeViewDTO();
                    gDto.Id = p.Id;
                    gDto.FirstName = p.FirstName + " " + p.LastName + " " + p.MiddleName;
                    gDto.SecondName = p.FirstName + " " + p.LastName + " " + p.MiddleName;
                    gDto.ThirdName = p.FirstName + " " + p.LastName + " " + p.MiddleName;
                    gDto.idNumber = p.IDNumber; 
                    gDto.IdentficationType = p.IDType;
                    


                    LocalBuffer.AllGuestVoucherConsignee.Add(gDto);

                    _frmNewGuest.SavedGuest = null;

                    dto.Guest = gDto.Id;
                    gvAddedRooms.RefreshData();
                    gvAddedRooms.FocusedColumn = gvAddedRooms.Columns[0];

                }
            }
        }

        

        private void btnEditRate_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            RoomTypeDTO dto = gvRoomType.GetFocusedRow() as RoomTypeDTO;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select Room Type first!", "ERROR");
                return;
            }

            Dictionary<String, String> args = new Dictionary<string, string>
            {
                {"fromDate", deArrival.Text},
                {"toDate", deDeparture.Text},
                {"adultCount", "1"},
                {"childCount", "0"},
                {"roomCount", "1"},
                {"rateCode",""},
                {"roomType", dto.Id.ToString()}
            };


            _rateSearchForm = new frmRateSearch(); 
            _rateSearchForm.LoadData(this._parentForm, args);
            _rateSearchForm.RateSelected += frmRateSearch_RateSelected;
            _rateSearchForm.ShowDialog();
        }



        void frmRateSearch_RateSelected(object sender, RateSearchCellClickedEventArgs e)
        {
            _selectedRoomType = e.RoomType;


            btnEditRate.Text = e.RowName;
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


            if (e.RowCode >0)// e.RowName != "")
            {
                RateCodeHeaderDTO rateCode = _rateHeaderList.FirstOrDefault(r => r.Id == e.RowCode);
                if (rateCode != null)
                {
                    _selectedRateHeader = rateCode;
                    CurrencyDTO currency = LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.Id == rateCode.CurrencyCode);
                    if (currency != null)
                    {
                        if (currency.Description.Trim().ToLower() == "birr")
                        {
                            teRate.Text = string.Format("{0} {1}", e.CellValue, "birr");
                        }
                        else
                        {
                            teRate.Text = string.Format("{0}{1}", currency.Sign, e.CellValue);
                        }
                    }
                    else
                    {
                        teRate.Text = e.CellValue;
                    }

                    _availalbePackages = UIProcessManager.GetRateCodePackagesByrateCodeHeader(rateCode.Id);
                    if (_availalbePackages != null && _availalbePackages.Count > 0)
                    {
                        lciRateCode.AppearanceItemCaption.ForeColor = Color.Blue;
                    }
                    else
                    {
                        lciRateCode.AppearanceItemCaption.ForeColor = Color.Black;
                    }

                }
            }
            if (_rateSearchForm != null)
            {
                if (_rateSearchForm.AverageRateAmt != _rateSearchForm.FirstNightAmount)
                {
                    teRate.Properties.Appearance.BackColor = Color.Orange;
                }
                else
                {
                    teRate.Properties.Appearance.BackColor = Color.White;
                }
                //teTotal.Text = RateSearchForm.TotalAmount.ToString();
            }
        }

        //Add / Remove Buttons
        private void btnAddSingle_Click(object sender, EventArgs e)
        {

            AddRegistrationRooms(false);

        }


        private void frmGroupRegistration_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                _parentForm.Close();
            }
        }

        private void beiState_EditValueChanged(object sender, EventArgs e)
        {
            var view = sender as BarEditItem;
            if (view == null || view.EditValue == null) return;

            if (Convert.ToInt32( view.EditValue) == CNETConstantes.CHECKED_IN_STATE)
            {
                deArrival.Enabled = false;
                rpgCheckIn.Visible = true;
                rpgSave.Visible = false;

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    this._parentForm.Close();
                }

                deArrival.EditValue = CurrentTime.Value;
            }
            else
            {
                deArrival.Enabled = true;
                rpgSave.Visible = true;
                rpgCheckIn.Visible = false;
            }
        }

        private void gvAddedRooms_RowStyle(object sender, RowStyleEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view == null) return;
            //AddedDTO dto = view.GetRow(e.RowHandle) as AddedDTO;
            //if(dto != null)
            //    dto.SN = e.RowHandle + 1;

        }


        private void spinNoOfNight_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit view = sender as SpinEdit;
            if (view.EditValue == null) return;
            deDeparture.EditValue = deArrival.DateTime.AddDays(Convert.ToInt32(view.EditValue.ToString()));
        }

        private void deArrival_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtArrival = (DateTime)deArrival.EditValue;
            DateTime dtDeparture = deDeparture.DateTime;
            if (dtArrival.Date >= dtDeparture.Date)
            {
                deDeparture.DateTime = dtArrival.AddDays(1);
                spinNoOfNight.EditValue = 1;

            }
            else
            {
                spinNoOfNight.EditValue = Convert.ToInt32((dtDeparture - dtArrival).TotalDays).ToString();
            }

            _selectedRateHeader = null;
            _availalbePackages = null;
            dailyRateCodeList = null;
            btnEditRate.EditValue = null;
            teRate.EditValue = null;
            _currentRateAmount = 0;
            lciRateCode.AppearanceItemCaption.ForeColor = ColorTranslator.FromHtml("black");

            CNETInfoReporter.WaitForm("Populating Rooms", "Please Wait...");
            PopulateRoomTypes(dtArrival);
            CNETInfoReporter.Hide();

        }

        private void deDeparture_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtArrival = (DateTime)deArrival.EditValue;
            if (deDeparture.EditValue == null) return;

            DateTime dtDeparture = (DateTime)deDeparture.EditValue;

            if (dtArrival.Date > dtDeparture.Date)
            {
                deDeparture.DateTime = dtArrival.AddDays(1);
                spinNoOfNight.EditValue = 1;
            }
            else
            {
                spinNoOfNight.EditValue = Convert.ToInt32((dtDeparture - dtArrival).TotalDays).ToString();

            }


            _selectedRateHeader = null;
            dailyRateCodeList = null;
            btnEditRate.EditValue = null;
            teRate.EditValue = null;
            _currentRateAmount = 0;
            _availalbePackages = null;
            lciRateCode.AppearanceItemCaption.ForeColor = ColorTranslator.FromHtml("black");


            CNETInfoReporter.WaitForm("Populating Rooms", "Please Wait...");
            PopulateRoomTypes(dtArrival);
            CNETInfoReporter.Hide();
        }


        private void btnAddAll_Click(object sender, EventArgs e)
        {
            checkBxList.CheckAll();
            AddRegistrationRooms(true);
        }

        private void btnRemoveSingle_Click(object sender, EventArgs e)
        {
            List<AddedDTO> addedDTOs = gvAddedRooms.DataSource as List<AddedDTO>;
            if (addedDTOs == null || addedDTOs.Count == 0) return;

            AddedDTO dto = gvAddedRooms.GetFocusedRow() as AddedDTO;
            if (dto == null) return;
            addedDTOs.Remove(dto);

            gcAddedRooms.DataSource = addedDTOs;
            gvAddedRooms.RefreshData();

            gvAddedRooms.FocusedRowHandle = 0;

            AddedDTO newDto = gvAddedRooms.GetFocusedRow() as AddedDTO;
            if (newDto != null)
            {
                PopulateFocusedRowAdjustment(newDto);
            }

            else
            {
                teRateAdjust.EditValue = 0;
            }

            RefreshRoomsList();

        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {

            _addedDtoList.Clear();
            gcAddedRooms.DataSource = null;
            gvAddedRooms.RefreshData();

            teRateAdjust.EditValue = 0;

            RefreshRoomsList();
        }

        private void gvAddedRooms_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            AddedDTO dto = view.GetRow(e.RowHandle) as AddedDTO;
            if (dto == null) return;

            if (e.Column.Caption == "Guest" && dto.Guest == null)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("red");
                e.Appearance.ForeColor = ColorTranslator.FromHtml("white");
            }

            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();

                dto.SN = (e.RowHandle + 1);
            }

            if ((e.Column.Caption == "Rate Amt" || e.Column.Caption == "Total") && dto.RateAdjustValue != 0)
            {
                e.Appearance.BackColor = ColorTranslator.FromHtml("NavajoWhite");
                e.Appearance.ForeColor = ColorTranslator.FromHtml("black");
            }
        }

        private void teRateAdjust_EditValueChanged(object sender, EventArgs e)
        {
            if (!toggleAdj.IsOn)
            {
                return;
            }
            TextEdit view = sender as TextEdit;
            AddedDTO dto = gvAddedRooms.GetFocusedRow() as AddedDTO;
            PerformRateAdjustment(dto, view, radioGroupAdjust);

        }

        private void radioGroupAdjust_SelectedIndexChanged(object sender, EventArgs e)
        {
            teRateAdjust.EditValue = 0;
        }

        private void gvAddedRooms_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            AddedDTO dto = view.GetRow(e.FocusedRowHandle) as AddedDTO;
            if (dto == null) return;

            PopulateFocusedRowAdjustment(dto);
        }



        private void bbiCheckIn_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnSave();
        }

        private void bbiSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            OnSave();
        }

        private void btnNote_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            AddedDTO dto = gvAddedRooms.GetFocusedRow() as AddedDTO;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select added registration first.", "ERROR");
                return;
            }

            if (_frmNote == null)
                _frmNote = new frmNote();

            if (!string.IsNullOrEmpty(dto.LogMessage))
                _frmNote.NoteContent = dto.LogMessage;
            else
                _frmNote.NoteContent = "";

            if (_frmNote.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(_frmNote.NoteContent))
                {
                    dto.LogMessage = _frmNote.NoteContent;
                }
            }


        }

        private void btnEditProfile_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            AddedDTO dto = gvAddedRooms.GetFocusedRow() as AddedDTO;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select added registration first.", "ERROR");
                return;
            }

            if (_frmNewGuest == null)
                _frmNewGuest = new frmNewGuest();

            bool isExist = false;
            AddedGuest existed = null;
            if (dto.Guest >0)
            {
                isExist = true;
                existed = _addedGuests.FirstOrDefault(g => g.Id == dto.Guest);
            }

            _frmNewGuest.SavedGuest = existed;

            if (_frmNewGuest.ShowDialog() == DialogResult.OK)
            {
                if (_frmNewGuest.SavedGuest != null)
                {



                    AddedGuest p = _frmNewGuest.SavedGuest;
                    VwConsigneeViewDTO gDto = LocalBuffer.AllGuestVoucherConsignee.FirstOrDefault(g => g.Id == p.Id);
                    if (gDto == null)
                    {
                        gDto = new VwConsigneeViewDTO();
                    }
                    gDto.Id = p.Id; 
                    gDto.FirstName = p.FirstName;
                    gDto.SecondName = p.MiddleName ;
                    gDto.ThirdName = p.LastName ;
                    gDto.idNumber = p.IDNumber; 
                    gDto.IdentficationType = p.IDType;
                     

                    if (!isExist)
                    {
                        _addedGuests.Add(_frmNewGuest.SavedGuest);
                        LocalBuffer.AllGuestVoucherConsignee.Add(gDto);
                    }



                    dto.Guest = gDto.Id;
                    repoSearchLookupGuest.DataSource = null;
                    repoSearchLookupGuest.DataSource = LocalBuffer.AllGuestVoucherConsignee;
                    repoSearchLookupGuest.View.RefreshData();

                    gcAddedRooms.RefreshDataSource();
                    gvAddedRooms.RefreshData();

                    string display = repoSearchLookupGuest.GetDisplayTextByKeyValue(dto.Guest);
                    gvAddedRooms.FocusedColumn = gvAddedRooms.Columns[0];

                    _frmNewGuest.SavedGuest = null;

                }
            }
        }

        private void bbiNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            Reset();
        }

        private void bbiEditGuest_ItemClick(object sender, ItemClickEventArgs e)
        {
            AddedDTO dto = gvAddedRooms.GetFocusedRow() as AddedDTO;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select added registration first.", "ERROR");
                return;
            }

            if (_frmNewGuest == null)
                _frmNewGuest = new frmNewGuest();

            bool isExist = false;
            AddedGuest existed = null;
            if (dto.Guest > 0)
            {
                isExist = true;
                existed = _addedGuests.FirstOrDefault(g => g.Id == dto.Guest);
            }

            _frmNewGuest.SavedGuest = existed;

            if (_frmNewGuest.ShowDialog() == DialogResult.OK)
            {
                if (_frmNewGuest.SavedGuest != null)
                {



                    AddedGuest p = _frmNewGuest.SavedGuest;
                    vw_VoucherConsignee gDto = LocalBuffer.AllGuestVoucherConsignee.FirstOrDefault(g => g.code == p.Code);
                    if (gDto == null)
                    {
                        gDto = new vw_VoucherConsignee();
                    }
                    gDto.code = p.Code; 
                    gDto.name = p.FirstName + " " + p.LastName + " " + p.MiddleName; 
                    gDto.idNumber = p.IDNumber;
                    gDto.IdentficationType = p.IDType;
                    

                    if (!isExist)
                    {
                        _addedGuests.Add(_frmNewGuest.SavedGuest);
                        LocalBuffer.AllGuestVoucherConsignee.Add(gDto);
                    }



                    dto.Guest = gDto.code;
                    repoSearchLookupGuest.DataSource = null;
                    repoSearchLookupGuest.DataSource = LocalBuffer.AllGuestVoucherConsignee;
                    repoSearchLookupGuest.View.RefreshData();

                    gcAddedRooms.RefreshDataSource();
                    gvAddedRooms.RefreshData();

                    string display = repoSearchLookupGuest.GetDisplayTextByKeyValue(dto.Guest);
                    gvAddedRooms.FocusedColumn = gvAddedRooms.Columns[0];

                    _frmNewGuest.SavedGuest = null;

                }
            }
        }

        private void bbiLog_ItemClick(object sender, ItemClickEventArgs e)
        {
            AddedDTO dto = gvAddedRooms.GetFocusedRow() as AddedDTO;
            if (dto == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select added registration first.", "ERROR");
                return;
            }

            if (_frmNote == null)
                _frmNote = new frmNote();

            if (!string.IsNullOrEmpty(dto.LogMessage))
                _frmNote.NoteContent = dto.LogMessage;
            else
                _frmNote.NoteContent = "";

            if (_frmNote.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(_frmNote.NoteContent))
                {
                    dto.LogMessage = _frmNote.NoteContent;
                }
            }
        }

        #endregion


        /************************** INNER CLASSES *****************************/
        #region Inner Classes
        private class RoomTypeVM
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public int Total { get; set; }
            public int Occupied { get; set; }
            public int Balance { get; set; }



            public List<RoomDetailDTO> RoomDetails { get; set; }
            public List<string> AvailableRooms { get; set; }

        }

        private class AddedDTO
        {
            public int SN { get; set; }

            public int RoomTypeCode { get; set; }
            public string RoomType { get; set; }
            public string RoomNumber { get; set; }

            public int DefAdultNo { get; set; }
            public int AdultNo { get; set; }
            public int Mattress { get; set; }
            public decimal DefRate { get; set; }
            public decimal RateAmount { get; set; }

            public decimal Total { get; set; }

            public string RateDescription { get; set; }
            public string Package { get; set; }
            public int Guest { get; set; }

            public decimal RateAdjustValue { get; set; }
            public bool IsPercent { get; set; }

            public DateTime ArrivalDate { get; set; }
            public DateTime DepartureDate { get; set; }
            public int NightNo { get; set; }

            public string CompanyCode { get; set; }

            public int RateCode { get; set; }

            public string LogMessage { get; set; }

            public AddedRate AddedRate { get; set; }

        }

        private class AddedRate
        {
            public int Code { get; set; }
            public string Description { get; set; }

            public decimal Amount { get; set; }

            public List<DailyRateCode> DailyRateCodeList { get; set; }

            public List<PackageHeaderDTO> Packages { get; set; }
        }


        #endregion


    }

}
