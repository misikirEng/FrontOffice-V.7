using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET.FrontOffice_V._7.Validation;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.CommonSchema;
using DevExpress.Mvvm.Native;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmRegistrationDetail : UILogicBase
    {
        private RegistrationDetailDTO registarationDetail;
        private DateTime CurrentTime;
        private frmRateSearch RateSearchForm;
        private List<DailyRateCodeDTO> dailyRateCodeList = new List<DailyRateCodeDTO>();
        private IList<Control> _invalidControls;
        private VoucherDTO vo = null;

        private RoomDetailDTO selectRoomDetail = new RoomDetailDTO();
        private List<RateCodeDetailDTO> rateCodeLIst = null;

        private int? adRoomMove, adRateChange, adRegDetail = null;
        private string origionalRateDesc = "";
        private string origionalRoomType = "";
        private string origionalRoom = "";

        //Properties
        public int LastRegState { get; set; }
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        internal RegistrationDetailDTO RegistarationDetail
        {
            get { return registarationDetail; }
            set
            {
                registarationDetail = value;

            }

        }
        public int roomCode { get; set; }

        private List<RoomTypeDTO> _roomTypeList = null;



        /********************** CONSTRUCTOR ********************/
        public frmRegistrationDetail()
        {
            InitializeComponent();
            InitializeUI();
        }

        #region Helper Methods

        public void InitializeUI()
        {
            leRoomType.EditValueChanged += cacRoomType_EditValueChanged;
            this.StartPosition = FormStartPosition.CenterScreen;
            SelectedRoomHandler = SelectedRoomEventHandler;

            //Room types
            leRoomType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
            leRoomType.Properties.DisplayMember = "Description";
            leRoomType.Properties.ValueMember = "Id";

            //Actual RTC
            leRTC.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            leRTC.Properties.DisplayMember = "Description";
            leRTC.Properties.ValueMember = "Id";

            //Business Source
            leSource.Properties.Columns.Add(new LookUpColumnInfo("Description", "Business Source"));
            leSource.Properties.DisplayMember = "Description";
            leSource.Properties.ValueMember = "Id";

            //Market
            leMarket.Properties.Columns.Add(new LookUpColumnInfo("Description", "Market"));
            leMarket.Properties.DisplayMember = "Description";
            leMarket.Properties.ValueMember = "Id";
        }

        public bool InitializeData()
        {
            try
            {
                DateTime? currentDateTime = UIProcessManager.GetServiceTime();
                if (currentDateTime == null)
                {
                    return false;
                }
                else
                {
                    CurrentTime = currentDateTime.Value;
                }

                if (registarationDetail == null) return false;

                // Progress_Reporter.Show_Progress("Loading Data...");

                //check Registration detail amended
                ActivityDefinitionDTO workFlowRegDetail = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_REGISTRATION_DETAIL_AMENDED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlowRegDetail != null)
                {
                    adRegDetail = workFlowRegDetail.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of REGISTRATION DETAIL AMENDED for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adRegDetail.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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

                //check room move workflow
                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOM_MOVED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adRoomMove = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of ROOM MOVED for Registration Voucher ", "ERROR");
                    return false;
                }


                //check rate changed workflow
                ActivityDefinitionDTO workFlowRateChanged = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_RATE_CHANGED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();


                if (workFlowRateChanged != null)
                {

                    adRateChange = workFlowRateChanged.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of RATE CHANGED for Registration Voucher ", "ERROR");
                    return false;
                }


                //populate values
                if (registarationDetail.Date != null)
                {
                    deThroughDate.Properties.MinValue = registarationDetail.Date.Value;
                    deFromReservationDate.DateTime = registarationDetail.Date.Value;
                    // deThroughDate.DateTime = registarationDetail.Date.Value;
                }
                if (registarationDetail.Adult != null) seAdult.Text = registarationDetail.Adult.Value.ToString();
                if (registarationDetail.RoomCount != null)
                {
                    if (registarationDetail.RoomCount > 1)
                    {
                        beRoom.Enabled = false;
                    }
                    teNoOfRooms.Text = registarationDetail.RoomCount.ToString();

                }
                if (registarationDetail.Child != null) spChild.Text = registarationDetail.Child.Value.ToString();
                if (registarationDetail.Adjustment != null) teAdjustment.Text = Math.Round(registarationDetail.Adjustment.Value, 2).ToString();
                if (registarationDetail.RateAmount != null)
                    teAmount.Text = Math.Round(registarationDetail.RateAmount.Value, 2).ToString();

                if (registarationDetail.IsFixedRate != null)
                {
                    ceFixed.Checked = registarationDetail.IsFixedRate.Value;
                    if (registarationDetail.IsFixedRate.Value)
                    {
                        beRateCode.Enabled = false;
                    }
                    else
                    {
                        beRateCode.Enabled = true;
                    }
                }

                leMarket.EditValue = registarationDetail.Market;
                leSource.EditValue = registarationDetail.Source;

                vo = UIProcessManager.GetVoucherById(registarationDetail.Voucher);
                if (vo != null)
                {
                    if (vo.EndDate != null)
                    {
                        deThroughDate.Properties.MaxValue = vo.EndDate.Value;
                        deThroughDate.EditValue = vo.EndDate.Value;
                    }

                    if (vo.LastState == CNETConstantes.CHECKED_IN_STATE || registarationDetail.Room != null)
                    {
                        teNoOfRooms.Enabled = false;
                    }
                }



                _roomTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode).Where(r => r.IsActive && (r.ActivationDate != null && r.ActivationDate.Value.Date <= CurrentTime.Date)).ToList();
                leRoomType.Properties.DataSource = (_roomTypeList);
                leRTC.Properties.DataSource = (_roomTypeList);

                leRoomType.EditValue = registarationDetail.RoomType;
                leRTC.EditValue = registarationDetail.ActualRtc;
                origionalRoomType = _roomTypeList.FirstOrDefault(r => r.Id == registarationDetail.RoomType).Description;

                if (registarationDetail.Room != null)
                {
                    RoomDetailDTO rd = UIProcessManager.GetRoomDetailById(registarationDetail.Room.Value);
                    if (rd != null)
                    {
                        beRoom.Text = rd.Description;
                        origionalRoom = rd.Description;
                        selectRoomDetail = rd;
                    }
                }

                rateCodeLIst = UIProcessManager.SelectAllRateCodeDetail();
                RateCodeDetailDTO rateCode = rateCodeLIst.FirstOrDefault(r => r.Id == registarationDetail.RateCode);
                if (rateCode != null)
                {
                    beRateCode.Text = UIProcessManager.GetRateCodeHeaderById(rateCode.RateCodeHeader).Description;
                    origionalRateDesc = beRateCode.Text;
                }

                List<LookupDTO> busSoureceList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.BUSSINESS_SOURCE && l.IsActive).ToList();
                if (busSoureceList != null)
                {
                    leSource.Properties.DataSource = (busSoureceList.OrderByDescending(c => c.IsDefault).ToList());
                }

                List<LookupDTO> marketList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.MARKET && l.IsActive).ToList();
                if (marketList != null)
                {
                    leMarket.Properties.DataSource = (marketList.OrderByDescending(c => c.IsDefault).ToList());

                }



                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing data. Detail: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


        }

        private void AutoCompleteFreeRooms()
        {
            if (leRoomType.EditValue != null && leRoomType.EditValue != "")
            {
                AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                List<RoomDetailDTO> roomList = UIProcessManager.GetUnassignedRoomsByState(deFromReservationDate.DateTime,
                    deThroughDate.DateTime, CNETConstantes.CHECKED_OUT_STATE).Where(t => t.RoomType == Convert.ToInt32(leRoomType.EditValue)).ToList();

                if (roomList != null && roomList.Count > 0)
                {
                    string[] roomsListdesStrings = roomList.Select(r => r.Description).ToArray();
                    collection.AddRange(roomsListdesStrings);
                    beRoom.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    beRoom.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    beRoom.MaskBox.AutoCompleteCustomSource = collection;
                }
            }
        }
        public void validateArrivalAndDepartureDate()
        {
            if (deThroughDate.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deThroughDate, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deFromReservationDate,
                    IsValidated=true
                },
                 new ValidationInfo(deFromReservationDate, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deThroughDate,
                    IsValidated=true
                }
            };
                _invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (_invalidControls.Count > 0)
                    return;
            }
        }

        private void SelectedRoomEventHandler(RoomDetailDTO room)
        {
            if (room != null)
            {
                beRoom.EditValue = room.Description;
                roomCode = room.Id;
            }
        }

        public SelectedRoom SelectedRoomHandler { get; set; }

        #endregion


        #region Event Handlers

        private void cacRoomType_EditValueChanged(object sender, EventArgs e)
        {
            int? roomType = null;
            RoomTypeDTO rt = null;
            if (leRoomType.EditValue != null)
            {
                roomType = Convert.ToInt32(leRoomType.EditValue);
                rt = _roomTypeList.FirstOrDefault(r => r.Id == roomType);
                leRTC.EditValue = (roomType);
            }

            if (seAdult.EditValue != null && seAdult.EditValue.ToString() != @"")
            {
                if (rt != null)
                {
                    if (seAdult.Value > rt.MaxAdults)
                    {
                        bbiOK.Enabled = false;

                        SystemMessage.ShowModalInfoMessage("You can not assign more than  " + rt.MaxAdults + " adults to this room type.", "ERROR");
                    }
                    else
                    {
                        bbiOK.Enabled = true;
                    }
                }
            }
            if (spChild.EditValue != null && spChild.EditValue.ToString() != "")
            {
                if (rt != null)
                {
                    if (spChild.Value > rt.MaxChildren)
                    {
                        bbiOK.Enabled = false;
                        SystemMessage.ShowModalInfoMessage("You can not assign more than  " + rt.MaxChildren + " child/children to this room type.", "ERROR");
                    }
                    else if (seAdult.Value > rt.MaxAdults)
                    {
                        bbiOK.Enabled = false;
                    }
                    else
                    {
                        bbiOK.Enabled = true;
                    }
                }
            }
            beRoom.Text = "";
            AutoCompleteFreeRooms();
        }

        private void bbiOK_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>
            {
               leRoomType,
                leRTC,
                teNoOfRooms,
                deThroughDate
            };

                if (beRateCode.Enabled)
                {
                    controls.Add(beRateCode);
                }

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    SystemMessage.ShowModalInfoMessage("PLEASE FILL ALL REQUIRED FIELDS", "ERROR");
                    return;
                }
                // Progress_Reporter.Show_Progress("Saving Registraion Detail");
                bool isUpdated = true;
                RegistrationDetailDTO updateRegDetail = RegistarationDetail;
                if (updateRegDetail.Date != null &&
                    updateRegDetail.Date.Value.Date >= CurrentTime.Date)
                {

                    List<RegistrationDetailDTO> regDeatilList = UIProcessManager.GetRegistrationDetailByvoucher(updateRegDetail.Voucher).Where(r => r.Date >= updateRegDetail.Date && r.Date <= deThroughDate.DateTime).ToList();
                    if (regDeatilList != null)
                    {
                        foreach (RegistrationDetailDTO reg in regDeatilList)
                        {
                            decimal finalValue = 0;
                            reg.Adult = Convert.ToInt16(seAdult.Value);
                            reg.Child = Convert.ToInt16(spChild.Value);
                            if (!string.IsNullOrEmpty(teAdjustment.Text))
                            {
                                if (dailyRateCodeList.Count > 0)
                                {
                                    decimal adjust = Convert.ToDecimal(teAdjustment.Text);
                                    decimal dialyValue = Convert.ToDecimal(teAmount.Text);
                                    DailyRateCodeDTO dtCode = dailyRateCodeList.FirstOrDefault(r => reg.Date != null && r.StayDate.Value.Date == reg.Date.Value.Date);
                                    if (dtCode != null)
                                    {
                                        dialyValue = dtCode.UnitRoomRate;
                                        reg.RateCode = dtCode.RateCodeDetail;
                                    }
                                    finalValue = dialyValue;
                                }
                                else
                                {
                                    if (reg.RateAmount != null) finalValue = reg.RateAmount.Value;
                                }


                            }
                            reg.RateAmount = finalValue;
                            reg.RoomCount = !string.IsNullOrEmpty(teNoOfRooms.Text)
                                ? Convert.ToInt32(teNoOfRooms.Text)
                                : RegistarationDetail.RoomCount;
                            reg.RoomType = Convert.ToInt32(leRoomType.EditValue);
                            if (LastRegState > 0)
                            {
                                if (roomCode > 0 && (LastRegState != CNETConstantes.CHECKED_IN_STATE && LastRegState != CNETConstantes.OSD_PENDING_STATE))
                                {
                                    reg.Room = null;
                                }
                                else
                                {
                                    reg.Room = roomCode > 0 ? roomCode : reg.Room;
                                }
                            }
                            else
                            {
                                reg.Room = roomCode > 0 ? roomCode : reg.Room;
                            }
                            if (leSource.EditValue != null && !string.IsNullOrEmpty(leSource.EditValue.ToString()))
                                reg.Source = Convert.ToInt32(leSource.EditValue);
                            else
                                reg.Source = null;

                            if (leMarket.EditValue != null && !string.IsNullOrEmpty(leMarket.EditValue.ToString()))
                                reg.Market = Convert.ToInt32(leMarket.EditValue);
                            else
                                reg.Market = null;

                            reg.ActualRtc = Convert.ToInt32(leRTC.EditValue);
                            reg.IsFixedRate = ceFixed.Checked;
                            if (UIProcessManager.UpdateRegistrationDetail(reg) == null)
                            {
                                isUpdated = false;
                                break;
                            }
                            else
                            {
                                if (beRateCode.EditValue != null && origionalRateDesc != beRateCode.EditValue.ToString())
                                {
                                    List<PackagesToPostDTO> packagelist = UIProcessManager.GetPackagesToPostByRegistrationDetail(reg.Id);
                                    if (packagelist != null)
                                        packagelist.ForEach(x => UIProcessManager.DeletePackagesToPostById(x.Id));



                                    DailyRateCodeDTO dtCode = dailyRateCodeList.FirstOrDefault(r => reg.Date != null && r.StayDate.Value.Date == reg.Date.Value.Date);
                                    if (dtCode != null && dtCode.RateCodeDetail != null)
                                    {
                                        RateCodeDetailDTO rateDetail = UIProcessManager.GetRateCodeDetailById(dtCode.RateCodeDetail.Value);

                                        if (rateDetail != null)
                                        {
                                            List<RateCodePackageDTO> RatePackageList = UIProcessManager.GetRateCodePackageByRateCodeHeader(rateDetail.RateCodeHeader);//  await GetPackagesToPost(rateDetail, generatedRegistration.registrationDetail, dailyRateCode.StayDate.Value, arrivalDate, departureDate);
                                            if (RatePackageList != null && RatePackageList.Count > 0)
                                            {


                                                foreach (RateCodePackageDTO rateCodePackage in RatePackageList)
                                                {
                                                    List<PackageDetailDTO> detail = UIProcessManager.GetPackageDetailByHeader(rateCodePackage.PackageHeader);
                                                    if (detail != null && detail.Count > 0)
                                                    {
                                                        PackageDetailDTO pack = detail.FirstOrDefault(x => x.StartDate.Date <= reg.Date.Value.Date && x.EndingDate >= reg.Date.Value.Date);
                                                        PackagesToPostDTO packagesToPost = new PackagesToPostDTO()
                                                        {
                                                            RegistrationDetail = reg.Id,
                                                            PackageHeader = rateCodePackage.PackageHeader,
                                                            Amount = pack.Price.Value
                                                        };
                                                        UIProcessManager.CreatePackagesToPost(packagesToPost);
                                                    }
                                                }
                                            }
                                        }

                                    }

                                }
                            }

                        }
                    }


                    if (isUpdated)
                    {
                        //save Reg. Detail Amended activity
                        ActivityDTO activityRegDetail = ActivityLogManager.SetupActivity(CurrentTime, adRegDetail.Value, CNETConstantes.PMS_Pointer, "Registration Detail is Amended");
                        activityRegDetail.Reference = RegistarationDetail.Voucher;
                        UIProcessManager.CreateActivity(activityRegDetail);

                        //save Room moved Activity
                        if (beRoom.EditValue != null && origionalRoom != beRoom.EditValue.ToString())
                        {
                            ActivityDTO activityRoomMoved = ActivityLogManager.SetupActivity(CurrentTime, adRoomMove.Value, CNETConstantes.PMS_Pointer, string.Format("{0},{1}", origionalRoom, origionalRoomType));
                            activityRoomMoved.Reference = RegistarationDetail.Voucher;
                            UIProcessManager.CreateActivity(activityRoomMoved);
                        }
                        //save Rate Changed Activity
                        if (beRateCode.EditValue != null && origionalRateDesc != beRateCode.EditValue.ToString())
                        {
                            ActivityDTO activityRateChanged = ActivityLogManager.SetupActivity(CurrentTime, adRateChange.Value, CNETConstantes.PMS_Pointer, "Rate is chanaged from " + origionalRateDesc + " to " + beRateCode.Text);
                            activityRateChanged.Reference = RegistarationDetail.Voucher;
                            UIProcessManager.CreateActivity(activityRateChanged);

                        }

                        SystemMessage.ShowModalInfoMessage("Registration updated successfully!", "MESSAGE");
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        ////CNETInfoReporter.Hide();
                        this.Close();
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Registration updating not successful! Check date range for the selected rate and respective packages.", "ERROR");
                    }
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("You can not edit the passed registration!", "ERROR");
                }

                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in saving registration detail. Detail: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void deThroughDate_EditValueChanged(object sender, EventArgs e)
        {
            if (CurrentTime > deThroughDate.DateTime)
            {
                validateArrivalAndDepartureDate();
            }
            //beRateCode.Text = "";
        }

        private void beRateCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (!string.IsNullOrEmpty(deThroughDate.Text))
            {
                Dictionary<String, String> args = new Dictionary<string, string>
                {
                    {"fromDate", deFromReservationDate.Text},
                    {"toDate", deThroughDate.Text},
                    {"adultCount", seAdult.Text},
                    {"childCount", spChild.Text},
                    {"rateCode", beRateCode.Text},
                    {"roomType", leRTC.EditValue.ToString()}
                };
                this.SubForm = this;

                //frmRateSearch RateSearchForm = (frmRateSearch)Home.OpenForm(this, "RATE SEARCH", args);
                //this.RateSearchForm = RateSearchForm;
                //RateSearchForm.FormClosing += RateSearchForm_FormClosing;
                //RateSearchForm.RateSelected += frmRateSearch_RateSelected;
                frmRateSearch RateSearchForm = new frmRateSearch();
                RateSearchForm.SelectedHotelcode = SelectedHotelcode;
                RateSearchForm.LoadData(this, args);
                this.RateSearchForm = RateSearchForm;
                RateSearchForm.FormClosing += RateSearchForm_FormClosing;
                RateSearchForm.RateSelected += frmRateSearch_RateSelected;
                RateSearchForm.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please select througn date!", "ERROR");
            }

        }
        void frmRateSearch_RateSelected(object sender, RateSearchCellClickedEventArgs e)
        {
            if (leRoomType.EditValue == null || string.IsNullOrEmpty(leRoomType.EditValue.ToString()))
            {
                leRoomType.EditValue = e.RoomType;
            }
            teAmount.Text = e.CellValue;
            beRateCode.Text = e.RowName;

        }
        void RateSearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RateSearchForm.frmRateDetailInfo != null)
                dailyRateCodeList = RateSearchForm.frmRateDetailInfo.dailyRateCodeList;
        }
        private void beRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (!string.IsNullOrEmpty(deThroughDate.Text))
            {
                Dictionary<String, String> args = new Dictionary<string, string>();
                args.Add("Guest Name", "");
                args.Add("Arrival", deFromReservationDate.Text);
                args.Add("Departure", deThroughDate.Text);
                args.Add("Nights", "");
                args.Add("RoomType", leRoomType.EditValue.ToString());
                args.Add("RoomTypeDescription", leRoomType.Text);

                if (vo != null && vo.LastState == CNETConstantes.CHECKED_IN_STATE)
                {
                    args.Add("CHECK_HK", "YES");
                }
                else
                {
                    args.Add("CHECK_HK", "NO");
                }


                this.SubForm = this;
                //  Home.OpenForm(this, "ROOM SEARCH", args);
                frmRoomSearch frmRoom = new frmRoomSearch();
                frmRoom.SelectedHotelcode = SelectedHotelcode;
                frmRoom.LoadData(this, args);
                frmRoom.ShowDialog();
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please select througn date!", "ERROR");
            }
        }

        private void beRoom_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ErrorText = "This room does not exist. Please input again.";
        }

        private void beRoom_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(beRoom.Text))
            {


                List<RoomDetailDTO> roomList =
                    UIProcessManager.GetUnassignedRoomsByState(deFromReservationDate.DateTime,
                        deThroughDate.DateTime,
                        CNETConstantes.CHECKED_OUT_STATE).Where(r => r.IsActive != null && r.IsActive).ToList();
                roomList.Add(selectRoomDetail);
                RoomDetailDTO rd = new RoomDetailDTO();
                if (leRoomType.EditValue != null && !string.IsNullOrEmpty(leRoomType.EditValue.ToString()))
                {
                    rd = roomList.FirstOrDefault(
                            r => r.Description == beRoom.Text && r.RoomType == Convert.ToInt32(leRoomType.EditValue));
                }
                else
                {
                    rd = roomList.FirstOrDefault(r => r.Description == beRoom.Text);
                }
                if (rd != null)
                {
                    SelectedRoomHandler.Invoke(rd);
                }
                else
                {
                    e.Cancel = true;
                }
            }

        }

        private void seAdult_Validating(object sender, CancelEventArgs e)
        {
            int? roomType = null;
            RoomTypeDTO rt = new RoomTypeDTO();
            if (leRoomType.EditValue != null)
            {
                roomType = Convert.ToInt32(leRoomType.EditValue);
                rt = UIProcessManager.GetRoomTypeById(roomType.Value);
                if (seAdult.EditValue != null && seAdult.EditValue != "")
                {
                    if (rt != null)
                    {
                        if (seAdult.Value > rt.MaxAdults)
                        {
                            bbiOK.Enabled = false;
                            SystemMessage.ShowModalInfoMessage("You can not assign more than  " + rt.MaxAdults + " adults to this room type.", "ERROR");
                            e.Cancel = true;

                        }
                        else
                        {
                            bbiOK.Enabled = true;
                        }
                    }
                }

            }
        }

        private void spChild_Validating(object sender, CancelEventArgs e)
        {
            int? roomType = null;
            RoomTypeDTO rt = new RoomTypeDTO();
            if (leRoomType.EditValue != null)
            {
                roomType = Convert.ToInt32(leRoomType.EditValue);
                rt = UIProcessManager.GetRoomTypeById(roomType.Value);
                if (spChild.EditValue != null && spChild.EditValue != "")
                {
                    if (rt != null)
                    {
                        if (spChild.Value > rt.MaxChildren)
                        {
                            bbiOK.Enabled = false;
                            SystemMessage.ShowModalInfoMessage("You can not assign more than  " + rt.MaxChildren + " child/children to this room type.", "ERROR");
                            e.Cancel = true;


                        }
                        else
                        {
                            bbiOK.Enabled = true;
                        }
                    }
                }

            }
        }

        private void leRTC_EditValueChanged(object sender, EventArgs e)
        {
            beRateCode.Text = "";
        }

        private void spChild_EditValueChanged(object sender, EventArgs e)
        {
            beRateCode.Text = "";
        }

        private void seAdult_EditValueChanged(object sender, EventArgs e)
        {
            beRateCode.Text = "";
        }

        #endregion

        private void frmRegistrationDetail_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        public int SelectedHotelcode { get; set; }
    }
}
