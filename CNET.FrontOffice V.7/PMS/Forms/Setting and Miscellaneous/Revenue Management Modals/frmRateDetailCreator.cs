using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraScheduler;





namespace CNET.FrontOffice_V._7.Forms.Setting_and_Miscellaneous.Revenue_Management_Modals
{
    public partial class frmRateDetailCreator : XtraForm//UILogicBase
    {

        private RateCodeDetailDTO _rateCodeDetail;

        private List<Control> MyControls = new List<Control>();
        private IList<Control> _invalidControls;
        private IList<Control> invalidMYControls;

        private List<WeekDayDTO> _cnetWeekDays = new List<WeekDayDTO>();
        private List<WeekDayDTO> orgWeekDays = new List<WeekDayDTO>();
        private List<RoomTypeDTO> roomTypes = null;
        //public string AdSyncCode { get; set; }

        private RateCodeHeaderDTO _rateCodeHeader;
        private bool IsThisEdit;

        /** Properties **/
        public DateTime CurrentTime { get; set; }

        public RateCodeHeaderDTO SelectedRateCodeHeader
        {
            get
            {
                return _rateCodeHeader;
            }
            set
            {
                _rateCodeHeader = value;

            }
        }

        internal RateCodeDetailDTO EditedRateCodeDetail
        {
            get { return _rateCodeDetail; }
            set
            {
                _rateCodeDetail = value;

            }
        }

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

        /************************** CONSTRUCTOR *****************************/
        public frmRateDetailCreator()
        {
            InitializeComponent();

            InitializeUi();
        }

        #region Helper Methods 

        public void InitializeUi()
        {
            Utility.AdjustForm(this);
            Utility.AdjustRibbon(layoutControlItem1);
            Size = new Size(1000, 450);
            Location = new Point(450, 150);

            MyControls = new List<Control> { teDescription, seMin, seMax };

            teDescription.KeyDown += textEditorCreator_KeyDown;
            teAdult1.KeyDown += textEditorCreator_KeyDown;
            teDefaultChild.KeyDown += textEditorCreator_KeyDown;
        }

        public bool InitializeData()
        {
            try
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return false;
                CurrentTime = currentTime.Value;

                // Progress_Reporter.Show_Progress("Loading Data");
                if (EditedRateCodeDetail != null)
                {
                    IsThisEdit = true;
                    populateRateDetail(EditedRateCodeDetail);
                }
                else
                {
                    if (SelectedRateCodeHeader == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to find rate code header", "ERROR");
                        ////CNETInfoReporter.Hide();
                        return false;
                    }

                    //default description value
                    teDescription.Text = SelectedRateCodeHeader.Description;

                    //default start and end dates
                    deStartDateRateDetail.EditValue = SelectedRateCodeHeader.StartDate;
                    deEndDateRateDetail.EditValue = SelectedRateCodeHeader.EndDate;

                    //default rooms types
                    populateRoomTypes();
                }


                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing data. Detail:: " + ex.Message, "ERROR");
                return false;
            }

        }
        public int RateHotelCode { get; set; }
        private void populateRoomTypes()
        {

            roomTypes = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedRateCodeHeader.Consigneeunit);
            List<RateCodeDetailRoomTypeDTO> rateCodeDetailRTList = null;
            List<RateCodeRoomTypeDTO> rateCodeRTList = null;

            if (EditedRateCodeDetail != null)
            {
                rateCodeDetailRTList = UIProcessManager.GetRateCodeDetailRoomTypeByrateCodeDetail(EditedRateCodeDetail.Id);
                if (rateCodeDetailRTList == null && rateCodeDetailRTList.Count == 0)
                {
                    //get rate header's room types
                    rateCodeRTList = UIProcessManager.GetRateCodeRoomTypeByrateCode(SelectedRateCodeHeader.Id);
                }
            }
            else
            {
                //populate rate header's room types
                rateCodeRTList = UIProcessManager.GetRateCodeRoomTypeByrateCode(SelectedRateCodeHeader.Id);

            }

            foreach (var rt in roomTypes)
            {
                CheckedListBoxItem item = new CheckedListBoxItem
                {
                    CheckState = CheckState.Unchecked,
                    Value = rt.Id,
                    Description = rt.Description,
                    Enabled = true
                };

                if (rateCodeDetailRTList != null)
                {
                    foreach (var rc in rateCodeDetailRTList)
                    {

                        if (rt.Id == rc.RoomType)
                        {
                            item.CheckState = CheckState.Checked;
                        }
                    }
                }
                else if (rateCodeRTList != null)
                {
                    foreach (var rc in rateCodeRTList)
                    {

                        if (rt.Id == rc.RoomType)
                        {
                            item.CheckState = CheckState.Checked;
                        }
                    }
                }

                clbcRoomFeatureRateDetail.Items.Add(item);

            }//end of foreach
        }

        private void populateRateDetail(RateCodeDetailDTO rateCodeDetail)
        {
            SelectedRateCodeHeader = UIProcessManager.GetRateCodeHeaderById(rateCodeDetail.RateCodeHeader);
            if (SelectedRateCodeHeader != null)
            {
                deStartDateRateDetail.EditValue = SelectedRateCodeHeader.StartDate;
                deStartDateRateDetail.DateTime = (DateTime)SelectedRateCodeHeader.StartDate;
                deEndDateRateDetail.EditValue = SelectedRateCodeHeader.EndDate;
                deEndDateRateDetail.DateTime = (DateTime)SelectedRateCodeHeader.EndDate;

                if (SelectedRateCodeHeader.StartDate != null)
                {
                    deStartDateRateDetail.Properties.MinValue = SelectedRateCodeHeader.StartDate;
                    deEndDateRateDetail.Properties.MinValue = SelectedRateCodeHeader.StartDate;
                }
                if (SelectedRateCodeHeader.EndDate != null)
                {
                    deStartDateRateDetail.Properties.MaxValue = SelectedRateCodeHeader.EndDate;
                    // deEndDateRateDetail.Properties.MaxValue = SelectedRateCodeHeader.EndDate;
                }


                teDescription.Text = rateCodeDetail.Description;
                populateRoomTypes();


            }
            deStartDateRateDetail.EditValue = rateCodeDetail.StartDate;
            deEndDateRateDetail.EditValue = rateCodeDetail.EndDate;

            seMin.Value = rateCodeDetail.StayMin;
            seMax.Value = rateCodeDetail.StayMax;


            // Paint WeekDaysCheckList control
            _cnetWeekDays = UIProcessManager.GetWeekDaysByReferenceandPointer(rateCodeDetail.Id, CNETConstantes.TABLE_RATE_CODE_DETAIL);
            if (_cnetWeekDays != null)
            {
                orgWeekDays.AddRange(_cnetWeekDays);
                wdceWeekdays.WeekDays = PmsHelper.PaintWeekDaysCheckList(_cnetWeekDays);
            }


            List<RateCodeDetailGuestCountDTO> rateCodeDetailGuestCounts = UIProcessManager.GetRateCodeDetailGuestCountsByRateCodeDetail(rateCodeDetail.Id);

            if (rateCodeDetailGuestCounts != null)
            {
                foreach (var guestCount in rateCodeDetailGuestCounts)
                {
                    if (guestCount.GuestCount == 1)
                    {
                        teAdult1.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teAdult1.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }
                    if (guestCount.GuestCount == 2)
                    {
                        teAdult2.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teAdult2.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }

                    if (guestCount.GuestCount == 3)
                    {
                        teAdult3.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teAdult3.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }

                    if (guestCount.GuestCount == 4)
                    {
                        teAdult4.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teAdult4.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }

                    if (guestCount.GuestCount == 5)
                    {
                        teAdult5.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teAdult5.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }

                    if (guestCount.GuestCount == -900)
                    {
                        teAdultExtra.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teAdultExtra.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }

                    if (guestCount.GuestCount == -600)
                    {
                        teChild0_3.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teChild0_3.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }

                    if (guestCount.GuestCount == -700)
                    {
                        teChild4_12.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teChild4_12.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }

                    if (guestCount.GuestCount == -800)
                    {
                        teChild13_18.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teChild13_18.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }

                    if (guestCount.GuestCount == -950)
                    {
                        teDefaultChild.Text = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                        teDefaultChild.Tag = Math.Round(guestCount.Amount, 2).ToString(CultureInfo.CurrentCulture);
                    }

                }
            }

        }

        public void validateArrivalAndDepartureDate()
        {
            //_invalidControls.Count=0;
            if (deEndDateRateDetail.IsModified || deStartDateRateDetail.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deEndDateRateDetail, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deStartDateRateDetail,
                    IsValidated=true
                },
                new ValidationInfo(deStartDateRateDetail, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deEndDateRateDetail,
                    IsValidated=true
                }
            };
                _invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (_invalidControls.Count > 0)
                    SystemMessage.ShowModalInfoMessage("The start date can not be greate than the end date!!!", "ERROR");
                return;
            }
        }

        public void MaintainAvailabilityCalender(List<DateTime> dateList, List<int> daysOfWeek, int rateCode)
        {
            AvailabilityCalendarDTO aCalender = new AvailabilityCalendarDTO();
            bool isASaved = false;
            foreach (DateTime date in dateList)
            {
                if (daysOfWeek.Contains(Convert.ToInt32(date.DayOfWeek)))
                {
                    aCalender = new AvailabilityCalendarDTO
                    {
                        Pointer = CNETConstantes.TABLE_RATE_CODE_DETAIL,
                        Reference = rateCode,
                        Date = date,
                        Year = date.Year,
                        Month = date.Month,
                        Day = date.Day,
                        AvailabilityStatus = 1,
                        Locked = false
                    };
                    AvailabilityCalendarDTO av = UIProcessManager.CreateAvailabilityCalendar(aCalender);
                    isASaved = true;
                }

            }

            if (isASaved)
            {

                SystemMessage.ShowModalInfoMessage("Availability Calendar Saved");
            }
        }

        public List<int> GetDaysOfWeek(List<WeekDayDTO> cnetWeekDays)
        {
            List<int> weekL = new List<int>();

            foreach (WeekDayDTO wk in cnetWeekDays)
            {
                int weeks = 0;
                switch (wk.Day)
                {

                    case 1://Sunday
                        {
                            weeks = 0;
                            break;
                        }
                    case 2://Monday
                        {
                            weeks = 1;
                            break;
                        }
                    case 4://Tuesday
                        {
                            weeks = 2;
                            break;
                        }
                    case 8://Wednesday
                        {
                            weeks = 3;
                            break;
                        }
                    case 16://Thursday
                        {
                            weeks = 4;
                            break;
                        }
                    case 32://Friday
                        {
                            weeks = 5;

                            break;
                        }
                    case 64://Saturday
                        {
                            weeks = 6;
                            break;
                        }

                }

                weekL.Add(weeks);
            }
            return weekL;
        }

        private void ResetFields()
        {
            if (SelectedRateCodeHeader != null)
            {
                deStartDateRateDetail.EditValue = SelectedRateCodeHeader.StartDate;
                deEndDateRateDetail.EditValue = SelectedRateCodeHeader.EndDate;
            }
            else
            {
                deStartDateRateDetail.EditValue = CurrentTime.ToShortDateString();
                deEndDateRateDetail.EditValue = CurrentTime.ToShortDateString();
            }
            teDescription.Text = String.Empty;
            teAdult1.Text = String.Empty;
            teAdult2.Text = String.Empty;
            teAdult3.Text = String.Empty;
            teAdult4.Text = String.Empty;
            teAdult5.Text = String.Empty;
            teDefaultChild.Text = String.Empty;
            teAdultExtra.Text = String.Empty;
            teChild0_3.Text = String.Empty;
            teChild4_12.Text = String.Empty;
            teChild13_18.Text = String.Empty;
            teAdult1.Tag = String.Empty;
            teAdult2.Tag = String.Empty;
            teAdult3.Tag = String.Empty;
            teAdult4.Tag = String.Empty;
            teAdult5.Tag = String.Empty;
            teDefaultChild.Tag = String.Empty;
            teAdultExtra.Tag = String.Empty;
            teChild0_3.Tag = String.Empty;
            teChild4_12.Tag = String.Empty;
            teChild13_18.Tag = String.Empty;
            seMin.Value = 1;
            seMax.Value = 1;

            clbcRoomFeatureRateDetail.Items.Clear();

            if (roomTypes == null || roomTypes.Count == 0)
            {
                roomTypes = UIProcessManager.SelectAllRoomType();
            }
            if (roomTypes != null)
            {
                foreach (var roomType in roomTypes)
                {
                    clbcRoomFeatureRateDetail.Items.Add(new
                        CheckedListBoxItem
                    {
                        CheckState = CheckState.Unchecked,
                        Value = roomType.Id,
                        Description = roomType.Description,
                        Enabled = true
                    });
                }
            }


            wdceWeekdays.WeekDays = WeekDays.EveryDay;
        }

        #endregion

        #region Event Handlers

        private void bbiSave_ItemClick(object sender, ItemClickEventArgs e)
        {

            try
            {
                bool isAvailabile = true;
                List<int> rateCodeDetailRoomTypes = (from CheckedListBoxItem item in clbcRoomFeatureRateDetail.Items
                                                     where item.CheckState == CheckState.Checked
                                                     select (int)item.Value).ToList();

                if (rateCodeDetailRoomTypes == null || rateCodeDetailRoomTypes.Count == 0)
                {
                    lcgRoomType.OptionsToolTip.ToolTip = @"At least one room type is required.";
                    lcgRoomType.AppearanceGroup.BorderColor = Color.Red;
                    lcgRoomType.AppearanceGroup.ForeColor = Color.Red;

                    return;
                }
                else
                {
                    lcgRoomType.OptionsToolTip.ToolTip = String.Empty;
                    lcgRoomType.AppearanceGroup.BorderColor = Color.Black;
                    lcgRoomType.AppearanceGroup.ForeColor = Color.Black;
                }

                #region Validate Controls

                // controls depend on another control, e.g. (start and end date ranges)
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deEndDateRateDetail, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deStartDateRateDetail,
                    IsValidated=true
                },
                new ValidationInfo(deStartDateRateDetail, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deEndDateRateDetail,
                    IsValidated=true
                },
                new ValidationInfo(seMax, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = seMin,
                    IsValidated=true
                },
                new ValidationInfo(seMin, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = seMax,
                    IsValidated=true
                }
            };

                _invalidControls = CustomValidationRule.Validate2(validationInfos);
                if (_invalidControls.Count > 0)
                {
                    return;
                }


                CustomValidationRule.RemoveInvalidatedControls(validationInfos);

                if (string.IsNullOrWhiteSpace(teAdult1.Text))
                {
                    MyControls.Add(teAdult1);
                }

                if (string.IsNullOrWhiteSpace(teDefaultChild.Text))
                {
                    MyControls.Add(teDefaultChild);
                }

                invalidMYControls = CustomValidationRule.Validate(MyControls);
                if (invalidMYControls.Count > 0)
                    return;
                CustomValidationRule.RemoveInvalidatedControls(MyControls);

                #endregion

                // Progress_Reporter.Show_Progress("Saving Rate Detail", "Please Wait...");
                int rateCodeDetailCode = 0;

                if (EditedRateCodeDetail != null)
                    rateCodeDetailCode = EditedRateCodeDetail.Id;



                List<string> result = new List<string>();
                List<string> unAvailabileRoomTypes = new List<string>();

                WeekDays weekDays = wdceWeekdays.WeekDays;
                List<WeekDayDTO> cnetWeekDays;

                List<int?> ModifiedWeekDay = new List<int?>();
                PmsHelper.GenerateWeekDays(weekDays, CNETConstantes.TABLE_RATE_CODE_DETAIL, rateCodeDetailCode, out cnetWeekDays);

                List<int?> wDays = new List<int?>();
                List<int?> mDays = new List<int?>();
                foreach (WeekDayDTO w in cnetWeekDays)
                {
                    wDays.Add(w.Day);

                }
                foreach (WeekDayDTO w in orgWeekDays)
                {
                    mDays.Add(w.Day);

                }
                if (IsThisEdit)
                {
                    ModifiedWeekDay = wDays.Except(mDays).ToList();//cnetWeekDays.Where(a => orgWeekDays.Contains(r => r.day != a.day)).Cast<WeekDay>.ToList();
                }
                else
                {
                    ModifiedWeekDay.AddRange(wDays);
                }


                foreach (int roomType in rateCodeDetailRoomTypes)
                {
                    result = null;// UIProcessManager.CheckRoomAvailablityForRate(ModifiedWeekDay, deStartDateRateDetail.DateTime, deEndDateRateDetail.DateTime, roomType, SelectedRateCodeHeader.Id).Distinct().ToList();
                    if (result != null && result.Count > 0)
                    {
                        RoomTypeDTO rt = null;
                        if (roomTypes != null)
                        {
                            rt = roomTypes.FirstOrDefault(r => r.Id == roomType);
                        }
                        if (rt == null)
                        {
                            rt = UIProcessManager.GetRoomTypeById(roomType);
                        }
                        if (rt != null)
                            unAvailabileRoomTypes.Add(rt.Description);
                        isAvailabile = false;
                    }
                }

                if (!isAvailabile)
                {
                    string concatRoomTypes = "";
                    string concatWeekDays = "";
                    List<string> UnavaWeeekDays = PmsHelper.GetWeekDaysName(result);
                    foreach (string str in UnavaWeeekDays)
                    {
                        concatWeekDays += str + ",";
                    }
                    foreach (string st in unAvailabileRoomTypes)
                    {
                        concatRoomTypes += " " + st;
                    }

                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Room Type(s)" + concatRoomTypes + " is (are) already assigned for this rate on " + concatWeekDays.TrimEnd(',') + ". Please revise your inputs", "ERROR");
                    return;
                }



                RateCodeDetailDTO rcd = new RateCodeDetailDTO
                {
                    Id = rateCodeDetailCode,
                    RateCodeHeader = SelectedRateCodeHeader.Id,
                    StartDate = Convert.ToDateTime(deStartDateRateDetail.EditValue),
                    EndDate = Convert.ToDateTime(deEndDateRateDetail.EditValue),
                    Description = teDescription.Text,
                    StayMax = Convert.ToInt32(seMax.Value),
                    StayMin = Convert.ToInt32(seMin.Value)
                };

                //int rangeCode = 0;
                //RangeDTO RangeExist = RangeBufferList.FirstOrDefault(x => x.Description == "RateDetail" && x.Min == seMin.Value && x.Max == seMax.Value);
                //if (RangeExist != null)
                //    rangeCode = RangeExist.Id;
                //else
                //{
                //    // get of stayDuration value,
                //    RangeDTO Nrewrange = UIProcessManager.CreateRange(new RangeDTO
                //    {
                //        Description = "RateDetail",
                //        Min = seMin.Value,
                //        Max = seMax.Value
                //    });

                //    rangeCode = Nrewrange.Id;
                //    RangeBufferList.Add(Nrewrange);
                //}

                //if (rangeCode == 0)
                //{
                //    ////CNETInfoReporter.Hide();
                //    SystemMessage.ShowModalInfoMessage("Unable to save Range!", "ERROR");
                //    return;
                //}


                //rcd.StayDuration = rangeCode;
                if (IsThisEdit)
                {



                    rateCodeDetailCode = EditedRateCodeDetail.Id;
                    rcd.Id = EditedRateCodeDetail.Id;
                    RateCodeDetailDTO updateResult = UIProcessManager.UpdateRateCodeDetail(rcd);
                    if (updateResult != null)
                    {
                        List<WeekDayDTO> weekdaylist = UIProcessManager.GetWeekDaysByReferenceandPointer(rcd.Id, CNETConstantes.TABLE_RATE_CODE_DETAIL);
                        if (weekdaylist != null)
                            weekdaylist.ForEach(x => UIProcessManager.DeleteWeekDayById(x.Id));

                        List<AvailabilityCalendarDTO> AvailabilityCalendarlist = UIProcessManager.GetAvailabilityCalendarBypointerandreference(CNETConstantes.TABLE_RATE_CODE_DETAIL, rcd.Id);
                        if (AvailabilityCalendarlist != null)
                            AvailabilityCalendarlist.ForEach(x => UIProcessManager.DeleteAvailabilityCalendarById(x.Id));

                        List<RateCodeDetailRoomTypeDTO> RateCodeDetailRoomTypelist = UIProcessManager.GetRateCodeDetailRoomTypeByrateCodeDetail(rcd.Id);
                        if (RateCodeDetailRoomTypelist != null)
                            RateCodeDetailRoomTypelist.ForEach(x => UIProcessManager.DeleteRateCodeDetailRoomTypeById(x.Id));

                        List<RateCodeDetailGuestCountDTO> RateCodeDetailGuestCountlist = UIProcessManager.GetRateCodeDetailGuestCountByrateCodeDetail(rcd.Id);
                        if (RateCodeDetailGuestCountlist != null)
                            RateCodeDetailGuestCountlist.ForEach(x => UIProcessManager.DeleteRateCodeDetailGuestCountById(x.Id));



                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Updating is not successfull!", "ERROR");
                        return;
                    }
                }
                else
                {
                    RateCodeDetailDTO SavedCodeDetail = UIProcessManager.CreateRateCodeDetail(rcd);
                    rateCodeDetailCode = SavedCodeDetail.Id;
                }

                if (rateCodeDetailCode == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Saving is not successfull!", "ERROR");
                    return;
                }

                List<RateCodeDetailGuestCountDTO> rateCodeDetailGuestCount = new List<RateCodeDetailGuestCountDTO>();

                // Save Adult and Child Amounts

                #region RateCodeDetailGuestCount


                if (!string.IsNullOrWhiteSpace(teAdult1.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO();

                    rcdgc.RateCodeDetail = rateCodeDetailCode;
                    rcdgc.IsAdult = true;
                    rcdgc.Amount = Convert.ToDecimal(teAdult1.EditValue);
                    rcdgc.GuestCount = 1;
                    rcdgc.Previousamount = (teAdult1.Tag != null && !string.IsNullOrEmpty(teAdult1.Tag.ToString())) ? Convert.ToDecimal(teAdult1.Tag) : 0;

                    rateCodeDetailGuestCount.Add(rcdgc);
                }

                if (!string.IsNullOrWhiteSpace(teAdult2.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO
                    {
                        RateCodeDetail = rateCodeDetailCode,
                        IsAdult = true,
                        Amount = Convert.ToDecimal(teAdult2.EditValue),
                        GuestCount = 2,
                        Previousamount = (teAdult2.Tag != null && !string.IsNullOrEmpty(teAdult2.Tag.ToString())) ? Convert.ToDecimal(teAdult2.Tag) : 0
                    };

                    rateCodeDetailGuestCount.Add(rcdgc);
                }

                if (!string.IsNullOrWhiteSpace(teAdult3.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO
                    {
                        RateCodeDetail = rateCodeDetailCode,
                        IsAdult = true,
                        Amount = Convert.ToDecimal(teAdult3.EditValue),
                        GuestCount = 3,
                        Previousamount = (teAdult3.Tag != null && !string.IsNullOrEmpty(teAdult3.Tag.ToString())) ? Convert.ToDecimal(teAdult3.Tag) : 0
                    };

                    rateCodeDetailGuestCount.Add(rcdgc);
                }

                if (!string.IsNullOrWhiteSpace(teAdult4.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO
                    {
                        RateCodeDetail = rateCodeDetailCode,
                        IsAdult = true,
                        Amount = Convert.ToDecimal(teAdult4.EditValue),
                        GuestCount = 4,
                        Previousamount = (teAdult4.Tag != null && !string.IsNullOrEmpty(teAdult4.Tag.ToString())) ? Convert.ToDecimal(teAdult4.Tag) : 0
                    };

                    rateCodeDetailGuestCount.Add(rcdgc);
                }

                if (!string.IsNullOrWhiteSpace(teAdult5.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO
                    {
                        RateCodeDetail = rateCodeDetailCode,
                        IsAdult = true,
                        Amount = Convert.ToDecimal(teAdult5.EditValue),
                        GuestCount = 5,
                        Previousamount = (teAdult5.Tag != null && !string.IsNullOrEmpty(teAdult5.Tag.ToString())) ? Convert.ToDecimal(teAdult5.Tag) : 0
                    };

                    rateCodeDetailGuestCount.Add(rcdgc);
                }

                if (!string.IsNullOrWhiteSpace(teAdultExtra.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO
                    {
                        RateCodeDetail = rateCodeDetailCode,
                        IsAdult = false,
                        Amount = Convert.ToDecimal(teAdultExtra.EditValue),
                        GuestCount = -900,
                        Previousamount = (teAdultExtra.Tag != null && !string.IsNullOrEmpty(teAdultExtra.Tag.ToString())) ? Convert.ToDecimal(teAdultExtra.Tag) : 0
                    };

                    rateCodeDetailGuestCount.Add(rcdgc);
                }

                if (!string.IsNullOrWhiteSpace(teChild0_3.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO
                    {
                        RateCodeDetail = rateCodeDetailCode,
                        IsAdult = false,
                        Amount = Convert.ToDecimal(teChild0_3.EditValue),
                        GuestCount = -600,
                        Previousamount = (teChild0_3.Tag != null && !string.IsNullOrEmpty(teChild0_3.Tag.ToString())) ? Convert.ToDecimal(teChild0_3.Tag) : 0
                    };

                    rateCodeDetailGuestCount.Add(rcdgc);
                }

                if (!string.IsNullOrWhiteSpace(teChild4_12.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO
                    {
                        RateCodeDetail = rateCodeDetailCode,
                        IsAdult = false,
                        Amount = Convert.ToDecimal(teChild4_12.EditValue),
                        GuestCount = -700,
                        Previousamount = (teChild4_12.Tag != null && !string.IsNullOrEmpty(teChild4_12.Tag.ToString())) ? Convert.ToDecimal(teChild4_12.Tag) : 0
                    };

                    rateCodeDetailGuestCount.Add(rcdgc);
                }

                if (!string.IsNullOrWhiteSpace(teChild13_18.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO
                    {
                        RateCodeDetail = rateCodeDetailCode,
                        IsAdult = false,
                        Amount = Convert.ToDecimal(teChild13_18.EditValue),
                        GuestCount = -800,
                        Previousamount = (teChild13_18.Tag != null && !string.IsNullOrEmpty(teChild13_18.Tag.ToString())) ? Convert.ToDecimal(teChild13_18.Tag) : 0
                    };
                    rateCodeDetailGuestCount.Add(rcdgc);
                }

                if (!string.IsNullOrWhiteSpace(teDefaultChild.Text))
                {
                    RateCodeDetailGuestCountDTO rcdgc = new RateCodeDetailGuestCountDTO
                    {
                        RateCodeDetail = rateCodeDetailCode,
                        IsAdult = false,
                        Amount = Convert.ToDecimal(teDefaultChild.EditValue),
                        GuestCount = -950,
                        Previousamount = (teDefaultChild.Tag != null && !string.IsNullOrEmpty(teDefaultChild.Tag.ToString())) ? Convert.ToDecimal(teDefaultChild.Tag) : 0
                    };
                    rateCodeDetailGuestCount.Add(rcdgc);
                }



                foreach (RateCodeDetailGuestCountDTO element in rateCodeDetailGuestCount)
                {
                    UIProcessManager.CreateRateCodeDetailGuestCount(element);
                }

                #endregion

                //Saving Rate Code Detail Room Types
                #region RateCodeDetailRoomType


                foreach (int roomType in rateCodeDetailRoomTypes)
                {

                    RateCodeDetailRoomTypeDTO rateCodeDetailRoomType = new RateCodeDetailRoomTypeDTO
                    {
                        RoomType = roomType,
                        RateCodeDetail = rateCodeDetailCode
                    };

                    UIProcessManager.CreateRateCodeDetailRoomType(rateCodeDetailRoomType);
                }


                #endregion

                #region Save to WeekDays

                PmsHelper.GenerateWeekDays(weekDays, CNETConstantes.TABLE_RATE_CODE_DETAIL, rateCodeDetailCode, out cnetWeekDays);

                // Save WeekDays information
                if (cnetWeekDays != null)
                {
                    foreach (WeekDayDTO wd in cnetWeekDays)
                    {
                        UIProcessManager.CreateWeekDay(wd);
                    }

                }


                #endregion

                #region Save to AvailabilityCalendar

                List<DateTime> dateList = new List<DateTime>();
                DateTime startDate = deStartDateRateDetail.DateTime;
                DateTime endDate = deEndDateRateDetail.DateTime;
                int DayInterval = 1;
                dateList.Add(startDate);

                while (startDate.AddDays(DayInterval) <= endDate)
                {
                    startDate = startDate.AddDays(DayInterval);
                    dateList.Add(startDate);
                }
                List<int> daysOfweek = GetDaysOfWeek(cnetWeekDays);

                MaintainAvailabilityCalender(dateList, daysOfweek, rateCodeDetailCode);


                #endregion

                DialogResult = DialogResult.OK;
                XtraMessageBox.Show("Rate Detail Successfully Saved!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ////CNETInfoReporter.Hide();


                this.Close();

            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.No;
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving rate detail. DETAIL:: " + ex.Message, "ERROR");

            }

        }

        private void textEditorCreator_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                CustomValidationRule.RemoveInvalidatedControls(MyControls);
            }
        }

        private void bbiReset_ItemClick(object sender, ItemClickEventArgs e)
        {
            ResetFields();
        }


        private void bbiCancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void deStartDateRateDetail_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deStartDateRateDetail.EditValue;
            DateTime dtEnDateTime = deEndDateRateDetail.DateTime;
            if (dtSDateTime > dtEnDateTime) deEndDateRateDetail.DateTime = dtSDateTime.AddDays(1);
        }

        private void deEndDateRateDetail_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deStartDateRateDetail.EditValue;
            DateTime dtEnDateTime = deEndDateRateDetail.DateTime;

            if (dtSDateTime > dtEnDateTime) validateArrivalAndDepartureDate();
        }

        private void teDescription_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void teAdult1_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void teDefaultChild_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void frmRateDetailCreator_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                DialogResult = DialogResult.Abort;
                this.Close();
            }
        }
        #endregion

    }
}