using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.CommonSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmRoomMove : UILogicBase
    {

        private RegistrationListVMDTO regExtension;
        public SelectedRoom SelectedRoomHandler { get; set; }

        private RegistrationDetailDTO _currentRegDetail = null;

        private List<DailyRateCodeDTO> dailyRateCodeList = new List<DailyRateCodeDTO>();


        int? roomCode = null;

        private RateCodeHeaderDTO selectedHeader;
        private frmRateSearch RateSearchForm;

        private RateCodeDetailDTO _oldRateDetail;
        private RateCodeHeaderDTO _oldRateHeader;
        private int? _oldRTC;
        private int? _oldRoomCode;

        private int? adCode = null;
        private int? _adRateChange = null;

        //properties
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

        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                if (value == null) return;

                regExtension = value;

            }
        }

        public DateTime? CurrentTime { get; set; }


        //**************** CONSTRUCTOR ******************//
        public frmRoomMove()
        {
            InitializeComponent();
            InitializeUI();

            SelectedRoomHandler = SelectedRoomEventHandler;


        }



        #region helper Methods

        public void InitializeUI()
        {
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;


            leRoomType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
            leRoomType.Properties.DisplayMember = "Description";
            leRoomType.Properties.ValueMember = "Id";


            lukRTC.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
            lukRTC.Properties.DisplayMember = "Description";
            lukRTC.Properties.ValueMember = "Id";

            leReason.Properties.Columns.Add(new LookUpColumnInfo("Description", "Reason"));
            leReason.Properties.DisplayMember = "Description";
            leReason.Properties.ValueMember = "Id";
        }


        public bool InitializeData()
        {

            if (RegExtension == null) return false;

            try
            {
                // Progress_Reporter.Show_Progress("Loading data...");

                CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                //check Room Move workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOM_MOVED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of ROOM MOVED for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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


                //check Rate Changed workflow
                ActivityDefinitionDTO workflowRateChange = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_RATE_CHANGED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();


                if (workflowRateChange != null)
                {

                    _adRateChange = workflowRateChange.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of RATE CHANGED for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Rate Changed Activity Previlage
                var userRoleMapperRate = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapperRate != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(_adRateChange.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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

                deFrom.Properties.MaxValue = RegExtension.Departure;
                deTo.Properties.MaxValue = RegExtension.Departure;
                deFrom.Properties.MinValue = RegExtension.Arrival;
                deTo.Properties.MinValue = RegExtension.Arrival;

                teCurrentRoom.Text = RegExtension.Room;
                teCurrentRoomType.Text = RegExtension.RoomTypeDescription;
                teRegistration.Text = RegExtension.Registration;
                teGuest.Text = RegExtension.Guest;

                List<RoomTypeDTO> roomType = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode).Where(r => r.IsActive && (r.ActivationDate != null && r.ActivationDate.Value.Date <= CurrentTime.Value.Date)).ToList();
                leRoomType.Properties.DataSource = (roomType);
                lukRTC.Properties.DataSource = (roomType);

                RoomTypeDTO rtcRoomtype = roomType.FirstOrDefault(r => r.Id == RegExtension.RoomType);

                teRTC.Text = rtcRoomtype == null ? "" : rtcRoomtype.Description;

                leRoomType.EditValue = RegExtension.RoomType;
                beRoom.Text = RegExtension.Room;
                roomCode = RegExtension.RoomCode;
                lukRTC.EditValue = RegExtension.RTC;



                if (CurrentTime.Value.Date <= RegExtension.Arrival)
                {
                    deFrom.DateTime = RegExtension.Arrival;
                    deFrom.Properties.MinValue = RegExtension.Arrival;
                    deTo.Properties.MinValue = RegExtension.Arrival;

                }
                else
                {
                    deFrom.DateTime = CurrentTime.Value.Date;
                    deFrom.Properties.MinValue = CurrentTime.Value;
                    deTo.Properties.MinValue = CurrentTime.Value;
                }
                if (CurrentTime.Value.Date <= RegExtension.Departure)
                {
                    deTo.DateTime = RegExtension.Departure;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("You can't change the room for overdueout guests.", "ERROR");
                    return false;
                }


                List<LookupDTO> reasonsToMove = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.Room_Move_Reason).ToList();
                leReason.Properties.DataSource = (reasonsToMove);

                GetCurrentRegDetail(true);


                ////CNETInfoReporter.Hide();

                return true;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing room move. DETAIL: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ////CNETInfoReporter.Hide();
                return false;
            }
        }

        private void GetCurrentRegDetail(bool isFirst = false)
        {
            beRateCode.Text = "";
            teNewAmount.Text = "";

            DateTime regDetailDate = deFrom.DateTime.Date;
            if (RegExtension.Arrival.Date == RegExtension.Departure.Date)
            {
                regDetailDate = RegExtension.Arrival.Date;
            }
            else if (deFrom.DateTime.Date == RegExtension.Departure.Date)
            {
                regDetailDate = deFrom.DateTime.AddDays(-1).Date;
            }
            else
            {
                regDetailDate = deFrom.DateTime.Date;
            }

            // load registraion detail 
            _currentRegDetail = UIProcessManager.GetRegistrationDetailByvoucher(RegExtension.Id).FirstOrDefault(r => r.Date.Value.Date == regDetailDate);
            if (_currentRegDetail != null)
            {
                _oldRTC = _currentRegDetail.ActualRtc;
                _oldRoomCode = _currentRegDetail.Room;
                var rateCodeLIst = UIProcessManager.SelectAllRateCodeDetail();
                RateCodeDetailDTO rateCode = rateCodeLIst.FirstOrDefault(r => r.Id == _currentRegDetail.RateCode);
                if (rateCode != null)
                {
                    var rateHeader = UIProcessManager.GetRateCodeHeaderById(rateCode.RateCodeHeader);
                    _oldRateHeader = rateHeader;
                    _oldRateDetail = rateCode;

                    teCurrentRate.Text = rateHeader.Description;
                    teOldAmount.Text = String.Format("{0:N2}", _currentRegDetail.RateAmount);

                    if (isFirst)
                    {
                        beRateCode.Text = rateHeader.Description;
                        teNewAmount.Text = String.Format("{0:N2}", _currentRegDetail.RateAmount);
                    }
                }

                if (_currentRegDetail.IsFixedRate.Value)
                {
                    beRateCode.Enabled = false;
                }
                else
                {
                    beRateCode.Enabled = true;
                }

            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Unable to get registration detail.", "ERROR");
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        public void validateArrivalAndDepartureDate()
        {
            IList<Control> _invalidControls;
            if (deFrom.IsModified || deTo.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deTo, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deFrom,
                    IsValidated=true
                },
                new ValidationInfo(deFrom, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deTo,
                    IsValidated=true
                }
            };
                _invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (_invalidControls.Count > 0)
                    return;
            }
        }

        #endregion

        #region Event Handlers

        private void SelectedRoomEventHandler(RoomDetailDTO room)
        {
            if (room != null)
            {
                beRoom.EditValue = room.Description;
                roomCode = room.Id;


                if (deFrom.DateTime.Date == Convert.ToDateTime("1/1/0001 12:00:00 AM") || (deTo.DateTime.Date == Convert.ToDateTime("1/1/0001 12:00:00 AM")))
                    return;

                bool flag = CommonLogics.IsRoomOccupied(RegExtension.Id, beRoom.EditValue == null ? null : Convert.ToInt32(beRoom.Tag), leRoomType.EditValue == null ? 0 : Convert.ToInt32(leRoomType.EditValue), deFrom.DateTime.Date, deTo.DateTime.Date, CurrentTime.Value, SelectedHotelcode);
                if (!flag)
                    bbiOk.Enabled = true;
                else
                    bbiOk.Enabled = false;
            }
        }

        private void bbiOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<Control> controls = new List<Control>
            {
                leReason,
                leRoomType,
                lukRTC,
                beRoom,
                deFrom,
                deTo
            };

            if (beRateCode.Enabled)
            {
                controls.Add(beRateCode);
            }

            IList<Control> invalidControls = CustomValidationRule.Validate(controls);

            if (invalidControls.Count > 0)
            {
                SystemMessage.ShowModalInfoMessage("Please fill all required fields!", "ERROR");
                return;
            }

            if (roomCode == null)
            {
                SystemMessage.ShowModalInfoMessage("Please select room number first!", "ERROR");
                return;
            }
            try
            {
                // Progress_Reporter.Show_Progress("Performing Room Move", "Please Wait...");

                _currentRegDetail.Room = roomCode;
                _currentRegDetail.RoomType = Convert.ToInt32(leRoomType.EditValue);
                _currentRegDetail.ActualRtc = Convert.ToInt32(lukRTC.EditValue);


                if (_oldRTC != _currentRegDetail.ActualRtc && !_currentRegDetail.IsFixedRate.Value)
                {
                    _currentRegDetail.Adjustment = null;
                    _currentRegDetail.Remark = "UPDATE";
                }
                else
                {
                    _currentRegDetail.Remark = null;
                }

                bool isMoved = false;

                List<RelationDTO> relations = UIProcessManager.GetRelationalStateByvoucher(RegExtension.Id);
                if (relations != null && relations.Count > 0)
                {
                    DialogResult dr = MessageBox.Show(@"This registration has room sharing, Do you want to break the share?", "Room Move", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dr == DialogResult.Yes)
                    {
                        isMoved = UIProcessManager.RoomMove(RegExtension.Id, deFrom.DateTime, deTo.DateTime, _currentRegDetail, dailyRateCodeList);
                        if (isMoved)
                        {
                            foreach (var rel in relations)
                            {
                                rel.RelationLevel = 0;
                                UIProcessManager.UpdateRelation(rel);
                            }
                        }

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    isMoved = UIProcessManager.RoomMove(RegExtension.Id, deFrom.DateTime.Date, deTo.DateTime.Date, _currentRegDetail, dailyRateCodeList);
                }

                if (isMoved)
                {
                    string r = teCurrentRoom.Text;
                    string ty = teRTC.Text;
                    

                   ActivityDTO activity = ActivityLogManager.SetupActivity(CurrentTime.Value, adCode.Value, CNETConstantes.PMS_Pointer, "" + RegExtension.Room + "," + RegExtension.RoomTypeDescription);
                    activity.Reference = RegExtension.Id;
                    ActivityDTO savedact = UIProcessManager.CreateActivity(activity);

                    if (savedact == null)
                    {
                        XtraMessageBox.Show("WARNING: activity log for this room move is not saved", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (teCurrentRate.Text != beRateCode.EditValue.ToString())
                    {
                        //Save Activity of Rate Changed
                        ActivityDTO activityRateChanged = ActivityLogManager.SetupActivity(CurrentTime.Value, _adRateChange.Value, CNETConstantes.PMS_Pointer, string.Format("{0} to {1}", teCurrentRate.Text, beRateCode.EditValue.ToString()));
                        activity.Reference = RegExtension.Id;
                        UIProcessManager.CreateActivity(activity);
                        //  ActivityLogManager.CommitActivity(activityRateChanged, _adRateChange, CNETConstantes.REGISTRATION_VOUCHER, string.Format("{0} to {1}", teCurrentRate.Text, beRateCode.EditValue.ToString()), CNETConstantes.PMS);

                    }


                    //Update Room to Dirty State
                    RoomDetailDTO lastRoomDetail = UIProcessManager.GetRoomDetailById(_oldRoomCode.Value);

                    if (lastRoomDetail != null)
                    {
                        List<RoomTypeDTO> pseudoRoomList = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
                        List<int> pseudoRooms = new List<int>();
                        if (pseudoRoomList != null)
                            pseudoRooms = pseudoRoomList.Select(p => p.Id).ToList();
                        if (pseudoRooms != null && !pseudoRooms.Contains(lastRoomDetail.RoomType))
                        {
                            //Get Activity Definition For Dirty
                            ActivityDefinitionDTO adDirty = UIProcessManager.GetActivityDefinitionBycomponetanddescription(CNETConstantes.PMS_Pointer, CNETConstantes.Dirty).FirstOrDefault();
                            if (adDirty != null)
                            {
                                lastRoomDetail.LastState = adDirty.Id;

                            }

                            UIProcessManager.UpdateRoomDetail(lastRoomDetail);
                            logActiviy(lastRoomDetail);
                        }

                    }
                    SystemMessage.ShowModalInfoMessage("Room Moved successfully!", "MESSAGE");
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Room Moving not successful!", "ERROR");
                }

                ////CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Exception has occured in saving Room Move. DETAIL:: " + ex.Message, "ERROR");

            }
        }
        private void logActiviy(RoomDetailDTO rmd)
        {
            try
            {
                ActivityDTO act = new ActivityDTO();
                act.ActivityDefinition = rmd.LastState.Value;
                act.TimeStamp = CurrentTime.Value.ToLocalTime();
                act.Year = CurrentTime.Value.Year;
                act.Month = CurrentTime.Value.Month;
                act.Day = CurrentTime.Value.Day;
                act.Reference = rmd.Id;
                act.Platform = "1";
                act.User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                act.Device = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                act.ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                act.Remark = "Room Moved";
                UIProcessManager.CreateActivity(act);
            }
            catch (Exception e) { }
        }
        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            this.Close();
        }

        private void deFrom_EditValueChanged(object sender, EventArgs e)
        {

            if (deFrom.DateTime > deTo.DateTime)
            {
                validateArrivalAndDepartureDate();
            }

            GetCurrentRegDetail();

            if (deFrom.DateTime.Date == Convert.ToDateTime("1/1/0001 12:00:00 AM") || (deTo.DateTime.Date == Convert.ToDateTime("1/1/0001 12:00:00 AM")))
                return;

            bool flag = CommonLogics.IsRoomOccupied(RegExtension.Id, beRoom.EditValue == null ? null : Convert.ToInt32(beRoom.Tag), leRoomType.EditValue == null ? 0 : Convert.ToInt32(leRoomType.EditValue), deFrom.DateTime.Date, deTo.DateTime.Date, CurrentTime.Value, SelectedHotelcode);

            if (flag)
            {
                XtraMessageBox.Show("There is another reservation for room number = " + beRoom.Text + " within the selected days", "CNET ERP v2016", MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                bbiOk.Enabled = false;
            }
            else
            {
                bbiOk.Enabled = true;

            }
        }

        private void deTo_EditValueChanged(object sender, EventArgs e)
        {
            if (deFrom.DateTime > deTo.DateTime)
            {
                deTo.DateTime = deFrom.DateTime;
                validateArrivalAndDepartureDate();
            }
            else
            {
                GetCurrentRegDetail();
            }

            if (deFrom.DateTime.Date == Convert.ToDateTime("1/1/0001 12:00:00 AM") || (deTo.DateTime.Date == Convert.ToDateTime("1/1/0001 12:00:00 AM")))
                return;

            bool flag = CommonLogics.IsRoomOccupied(RegExtension.Id, beRoom.EditValue == null ? null : Convert.ToInt32(beRoom.Tag), leRoomType.EditValue == null ? 0 : Convert.ToInt32(leRoomType.EditValue), deFrom.DateTime.Date, deTo.DateTime.Date, CurrentTime.Value, SelectedHotelcode);

            if (flag)
            {
                XtraMessageBox.Show("There is another reservation for room number = " + beRoom.Text + " within the selected days", "CNET ERP v2016", MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                bbiOk.Enabled = false;
            }
            else
            {
                bbiOk.Enabled = true;

            }
        }

        private void leRoomType_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit view = sender as LookUpEdit;

            beRoom.Text = "";
            lukRTC.EditValue = view.EditValue;
        }

        private void beRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>();
            args.Add("Guest Name", RegExtension.Guest);
            args.Add("Arrival", deFrom.Text);
            args.Add("Departure", deTo.Text);
            args.Add("Nights", "");
            args.Add("RoomType", leRoomType.EditValue.ToString());
            args.Add("RoomTypeDescription", leRoomType.Text);

            if (RegExtension.lastState == CNETConstantes.CHECKED_IN_STATE)
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
            frmRoom.ShowDialog();
        }

        private void frmRoomMove_load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }


        }


        #endregion

        private void beRateCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>
            {
                {"fromDate", deFrom.DateTime.ToShortDateString()},
                {"toDate", deTo.DateTime.ToShortDateString()},
                {"adultCount", _currentRegDetail.Adult.ToString()},
                {"childCount", _currentRegDetail.Child.ToString()},
                {"rateCode", beRateCode.Text},
                {"roomType", lukRTC.EditValue.ToString()}
            };
            this.SubForm = this;

            frmRateSearch RateSearchForm = new frmRateSearch();
            RateSearchForm.SelectedHotelcode = SelectedHotelcode;
            RateSearchForm.LoadData(this, args);
            this.RateSearchForm = RateSearchForm;
            RateSearchForm.FormClosing += RateSearchForm_FormClosing;
            RateSearchForm.RateSelected += frmRateSearch_RateSelected;
            RateSearchForm.ShowDialog();
        }

        private void frmRateSearch_RateSelected(object sender, RateSearchCellClickedEventArgs e)
        {
            if (leRoomType.EditValue == null || string.IsNullOrEmpty(leRoomType.EditValue.ToString()))
            {
                leRoomType.EditValue = e.RoomType;
            }

            if (!string.IsNullOrEmpty(e.CellValue))
            {
                decimal amount = Convert.ToDecimal(e.CellValue);
                teNewAmount.Text = String.Format("{0:N2}", amount);

            }
            beRateCode.Text = e.RowName;

        }
        void RateSearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RateSearchForm.frmRateDetailInfo != null)
                dailyRateCodeList = RateSearchForm.frmRateDetailInfo.dailyRateCodeList;
        }


        private void lukRTC_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit view = sender as LookUpEdit;

            if (_currentRegDetail != null && !_currentRegDetail.IsFixedRate.Value)
            {
                beRateCode.Text = "";
                teNewAmount.Text = "";
            }

            if (view != null && view.EditValue != null && _currentRegDetail != null && Convert.ToInt32(view.EditValue) == _currentRegDetail.ActualRtc)
            {
                beRateCode.Text = teCurrentRate.Text;
                teNewAmount.Text = string.Format("{0:N2}", _currentRegDetail.RateAmount);
            }

        }

        public int SelectedHotelcode { get; set; }
    }
}
