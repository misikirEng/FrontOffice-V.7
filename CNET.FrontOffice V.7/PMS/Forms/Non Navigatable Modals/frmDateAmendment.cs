
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
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.Pdf.Native.BouncyCastle.Ocsp;
using CNET_V7_Domain.Domain.TransactionSchema;
using DevExpress.Mvvm.Native;
using CNET_V7_Domain.Misc;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmDateAmendment : UILogicBase
    {
        #region Fields

        private frmRateSearch RateSearchForm;

        private RegistrationDetailDTO regDetail;

        private List<RegistrationDetailDTO> _registrationDetails;

        private List<PackagesToPostDTO> _packagesToPosts;

        private IList<Control> _invalidControls;

        private List<DailyRateCodeDTO> dailyRateCodeList = new List<DailyRateCodeDTO>();

        private List<RateCodeDetailDTO> _rateCodeList = null;
        private List<RateCodeHeaderDTO> _rateHeaderList = null;

        private int? adCode = null;


        //** Properties **//

        public DateTime CurrentTime { get; set; }

        public bool IsFromNightAudit { get; set; }

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

        public SelectedRoom SelectedRoomHandler { get; set; }

        private RegistrationListVMDTO registrationExt;
        internal RegistrationListVMDTO RegistrationExt
        {
            get { return registrationExt; }
            set
            {
                registrationExt = value;

            }
        }

        #endregion


        //******** CONSTRUCTOR ********************
        public frmDateAmendment()
        {
            InitializeComponent();

            InitializeUI();

        }


        //** Methods **//
        #region Methods

        public void InitializeUI()
        {

            //Room type
            leRoomType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Room Types"));
            leRoomType.Properties.DisplayMember = "Description";
            leRoomType.Properties.ValueMember = "Id";

            //RTC
            leRTC.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            leRTC.Properties.DisplayMember = "Description";
            leRTC.Properties.ValueMember = "Id";


            SelectedRoomHandler = SelectedRoomEventHandler;

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public bool InitializeData()
        {
            try
            {
                //DateTime? currentTime = UIProcessManager.GetServiceTime(IsFromNightAudit);
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return false;
                CurrentTime = currentTime.Value;

                deDepartureDate.Properties.MinValue = CurrentTime.Date;
                deArrivalDate.Properties.MinValue = CurrentTime.Date;

                if (RegistrationExt == null) return false;

                // Progress_Reporter.Show_Progress("Please wait...");

                deArrivalDate.DateTime = RegistrationExt.Arrival;
                deDepartureDate.DateTime = RegistrationExt.Departure;
                beRoom.Text = RegistrationExt.Room;


                //check workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_DATE_AMENDED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {
                    adCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of DATE AMENDED for Registration Voucher ", "ERROR");
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



                List<RoomTypeDTO> roomType = UIProcessManager.SelectAllRoomType();
                leRoomType.Properties.DataSource = (roomType);
                leRTC.Properties.DataSource = (roomType);

                regDetail = UIProcessManager.GetLastRegistration(RegistrationExt.Id);
                if (regDetail != null)
                {
                    if (regDetail.Adult != null) teAdult.Text = regDetail.Adult.Value.ToString();
                    if (regDetail.Child != null) teChild.Text = regDetail.Child.Value.ToString();
                    if (regDetail.RateAmount != null) teAmount.Text = regDetail.RateAmount.Value.ToString();

                    leRoomType.EditValue = regDetail.RoomType;

                    leRTC.EditValue = regDetail.ActualRtc;

                    if (regDetail.IsFixedRate != null) ceFixed.Checked = regDetail.IsFixedRate.Value;


                    _rateCodeList = UIProcessManager.SelectAllRateCodeDetail();
                    _rateHeaderList = UIProcessManager.GetRateCodeHeaderByconsigneeunit(SelectedHotelcode);

                    if (_rateCodeList != null && _rateHeaderList != null)
                    {
                        RateCodeDetailDTO rateCode = _rateCodeList.FirstOrDefault(r => r.Id == regDetail.RateCode);
                        if (rateCode != null)
                        {
                            RateCodeHeaderDTO rateHeader = _rateHeaderList.FirstOrDefault(r => r.Id == rateCode.RateCodeHeader);
                            if (rateHeader != null)
                            {
                                beRateCode.Text = rateHeader.Description;
                                beRateCode.Tag = rateHeader.Id;
                            }
                        }
                    }
                }

                //if last state is check-in, disable arrival date
                if (registrationExt.lastState == CNETConstantes.CHECKED_IN_STATE)
                {
                    deArrivalDate.Enabled = false;
                }
                else
                {
                    deArrivalDate.Enabled = true;
                }

                // deArrivalDate.DateTime = registrationExt.Arrival;

                beRoom.Enabled = false;
                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing date amendment. DETAIL: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ////CNETInfoReporter.Hide();
                return false;
            }
        }


        private void SelectedRoomEventHandler(RoomDetailDTO room)
        {
            if (room != null)
                beRoom.EditValue = room.Description;
        }


        public int validateArrivalAndDepartureDate()
        {
            int count = 0;
            if (deArrivalDate.IsModified || deDepartureDate.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deDepartureDate, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deArrivalDate,
                    IsValidated=true
                },
                new ValidationInfo(deArrivalDate, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deDepartureDate,
                    IsValidated=true
                }
            };
                _invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (_invalidControls.Count > 0)
                {
                    count = _invalidControls.Count;
                }

                return count;
            }
            else
            {
                return 0;
            }
        }





        #endregion

        //** Event Handlers **//
        #region Event Handlers

        private void bbiOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Progress_Reporter.Show_Progress("Saving. Please Wait...");
                if (regDetail == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get last registration detail.", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }


                RegistrationInfoDTO registration = new RegistrationInfoDTO();
                registration.RoomType = Convert.ToInt32(leRoomType.EditValue);
                registration.RTC = Convert.ToInt32(leRTC.EditValue);

                if (_rateHeaderList != null && beRateCode.Tag != null && !string.IsNullOrEmpty(beRateCode.Tag.ToString()))
                {
                    var firstOrDefault = _rateHeaderList.FirstOrDefault(r => r.Id == Convert.ToInt32(beRateCode.Tag));
                    if (firstOrDefault != null)
                        registration.RateCode = firstOrDefault.Id;
                }
                registration.AdultCount = Convert.ToInt32(teAdult.Text);
                registration.ChildCount = Convert.ToInt32(teChild.Text);
                registration.ArrivalDate = deArrivalDate.DateTime;
                registration.DepartureDate = deDepartureDate.DateTime;

                if (regDetail.RoomCount != null) registration.RoomCount = regDetail.RoomCount.Value;

                //existed registration detail
                List<RegistrationDetailDTO> existedRegDetails = UIProcessManager.GetRegistrationDetailByvoucher(RegistrationExt.Id);
                if (existedRegDetails == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get registration detail.", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }

                //RateCodeDetailDTO rateCodeDetai = UIProcessManager.GetRateCodeDetailById(regDetail.RateCode.Value);

                //if (rateCodeDetai.RateCodeHeader == registration.RateCode)
                //{
                //update Rate Adjustment
                RateAdjustmentDTO rateAdju = UIProcessManager.GetRateAdjustmentByvoucher(RegistrationExt.Id);
                if (rateAdju != null)
                {
                    regDetail.Adjustment = rateAdju.Amount;
                    rateAdju.StartDate = deArrivalDate.DateTime;
                    rateAdju.EndDate = deDepartureDate.DateTime;
                    RateAdjustmentDTO isRateAdjusUpdated = UIProcessManager.UpdateRateAdjustment(rateAdju);
                }
                //}
                //else
                //    regDetail.Adjustment =0;


                List<GeneratedRegistrationDTO> Registrationlist = UIProcessManager.AmendRegistrationDate(RegistrationExt.lastState.Value, regDetail, registration, deArrivalDate.DateTime.Date, deDepartureDate.DateTime.Date, SelectedHotelcode);

                if (Registrationlist == null)
                {
                    SystemMessage.ShowModalInfoMessage("Date amendment is not successful. Either the rate is not available or registration detail can't be updated!!!", "ERROR");

                    //SystemMessage.ShowModalInfoMessage("No registration detail generated. Please check the rate code given for Room Number: " + registrationExt.Room, "ERROR");
                    //CNETInfoReporter.Hide();
                }
                else
                {
                    foreach (GeneratedRegistrationDTO GeneratedDetail in Registrationlist)
                    {
                        if (RegistrationExt.lastState == CNETConstantes.CHECKED_IN_STATE)
                        {
                            var reg = existedRegDetails.FirstOrDefault(r => r.Date.Value.Date == GeneratedDetail.registrationDetail.Date);
                            if (reg != null) continue;
                        }

                        GeneratedDetail.registrationDetail.Adjustment = 0;
                        GeneratedDetail.registrationDetail.Voucher = RegistrationExt.Id;

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


                    var response = UIProcessManager.GetVoucherBufferById(RegistrationExt.Id);


                    VoucherBuffer voucherBuffer = response.Data;
                    voucherBuffer.Voucher.StartDate = deArrivalDate.DateTime;
                    voucherBuffer.Voucher.EndDate = deDepartureDate.DateTime;

                    if (voucherBuffer.TransactionReferencesBuffer != null && voucherBuffer.TransactionReferencesBuffer.Count > 0)
                        voucherBuffer.TransactionReferencesBuffer.ForEach(x => x.ReferencedActivity = null);


                    voucherBuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, adCode.Value, CNETConstantes.PMS_Pointer, "Date is amended. Arrival: " + deArrivalDate.DateTime + ", Departure: " + deDepartureDate.DateTime);


                    voucherBuffer.TransactionCurrencyBuffer = null;
                    ResponseModel<VoucherBuffer> updated = UIProcessManager.UpdateVoucherBuffer(voucherBuffer);
                    if (updated != null && updated.Success)
                    {
                        SystemMessage.ShowModalInfoMessage("Date amendment successful!!!", "MESSAGE");
                        DialogResult = System.Windows.Forms.DialogResult.OK;

                    }
                    else
                        SystemMessage.ShowModalInfoMessage("fail to amendment!!!" + Environment.NewLine + updated.Message, "MESSAGE");

                }


                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in date amendment. DETAIL:: " + ex.Message, "ERROR");
                ////CNETInfoReporter.Hide();
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            this.Close();
        }

        private void deDepartureDate_EditValueChanged(object sender, EventArgs e)
        {
            int count = 0;
            if (deArrivalDate.DateTime.Date > deDepartureDate.DateTime.Date)
            {
                count = validateArrivalAndDepartureDate();
            }


            bool flag = CommonLogics.IsRoomOccupied(RegistrationExt.Id, RegistrationExt.RoomCode, RegistrationExt.RoomType.Value, deArrivalDate.DateTime.Date, deDepartureDate.DateTime.Date, CurrentTime, SelectedHotelcode);

            if (flag)
            {
                XtraMessageBox.Show("There is another reservation for room number = " + RegistrationExt.Room + " within the selected days", "CNET ERP v2016", MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                bbiOk.Enabled = false;
                return;
            }
            else
            {
                bbiOk.Enabled = true;

            }

            if (count > 0)
                bbiOk.Enabled = false;
            else
                bbiOk.Enabled = true;

            if (deDepartureDate.DateTime.Date < RegistrationExt.Departure.Date)
            {
                teAdult.Enabled = false;
                teChild.Enabled = false;
                teAmount.Enabled = false;
                leRoomType.Enabled = false;
                leRTC.Enabled = false;
                beRoom.Enabled = false;
                beRateCode.Enabled = false;
                ceFixed.Enabled = false;
            }
            else
            {
                teAdult.Enabled = true;
                teChild.Enabled = true;
                teAmount.Enabled = true;
                leRoomType.Enabled = true;
                leRTC.Enabled = true;
                beRoom.Enabled = true;
                beRateCode.Enabled = true;
                ceFixed.Enabled = true;


            }
        }

        private void deArrivalDate_EditValueChanged(object sender, EventArgs e)
        {
            if (deArrivalDate.DateTime.Date >= deDepartureDate.DateTime.Date)
            {
                DateTime dtArrival = (DateTime)deArrivalDate.EditValue;
                deDepartureDate.DateTime = dtArrival.AddDays(1);
                validateArrivalAndDepartureDate();
            }

            bool flag = CommonLogics.IsRoomOccupied(RegistrationExt.Id, RegistrationExt.RoomCode, RegistrationExt.RoomType.Value, deArrivalDate.DateTime.Date, deDepartureDate.DateTime.Date, CurrentTime, SelectedHotelcode);

            if (flag)
            {
                XtraMessageBox.Show("There is another reservation for room number = " + RegistrationExt.Room + " within the selected days", "CNET ERP v2016", MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                bbiOk.Enabled = false;
            }
            else
            {
                bbiOk.Enabled = true;

            }
        }


        private void beRateCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>
            {
                {"fromDate", deArrivalDate.Text},
                {"toDate", deDepartureDate.Text},
                {"adultCount", teAdult.Text},
                {"childCount", teChild.Text},
                {"rateCode", beRateCode.Text},
                {"roomType", leRoomType.EditValue.ToString()}
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
        void frmRateSearch_RateSelected(object sender, RateSearchCellClickedEventArgs e)
        {
            leRoomType.EditValue = e.RoomType;
            teAmount.Text = e.CellValue;
            beRateCode.Text = e.RowName;
            beRateCode.Tag = e.RowCode;
        }
        void RateSearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RateSearchForm.frmRateDetailInfo != null)
                dailyRateCodeList = RateSearchForm.frmRateDetailInfo.dailyRateCodeList;
        }

        private void frmDateAmendment_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }


        #endregion


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                if (_registrationDetails != null)
                {
                    _registrationDetails.Clear();
                    _registrationDetails = null;
                }

                if (_packagesToPosts != null)
                {
                    _packagesToPosts.Clear();
                    _packagesToPosts = null;
                }

                if (_invalidControls != null)
                {
                    _invalidControls.Clear();
                    _invalidControls = null;
                }

                if (dailyRateCodeList != null)
                {
                    dailyRateCodeList.Clear();
                    dailyRateCodeList = null;
                }

                if (_rateCodeList != null)
                {
                    _rateCodeList.Clear();
                    _rateCodeList = null;
                }

                if (_rateHeaderList != null)
                {
                    _rateHeaderList.Clear();
                    _rateHeaderList = null;
                }

                if (RateSearchForm != null)
                {
                    RateSearchForm = null;
                }

                regDetail = null;
                RegistrationExt = null;
            }
            base.Dispose(disposing);
        }


        public int SelectedHotelcode { get; set; }
    }
}
