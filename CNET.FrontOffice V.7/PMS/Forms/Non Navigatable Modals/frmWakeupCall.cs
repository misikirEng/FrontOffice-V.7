
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmWakeupCall : UILogicBase
    {

        private bool _isHeaderUpdate = false;
        private bool _isDetailUpdate = false;
        private ScheduleHeaderDTO _currentScheduleHeader = null;
        private ScheduleDetailDTO _currentScheduleDetail = null;



        public frmWakeupCall()
        {
            InitializeComponent();
            InitializeUI();
        }

        #region Properties

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


        public DateTime CurrentTime { get; set; }

        private RegistrationListVMDTO _regExt;
        public RegistrationListVMDTO RegExt
        {
            get
            {
                return _regExt;
            }
            set
            {
                if (value == null) return;
                _regExt = value;

                teRoom.Text = value.Room;
                teGuest.Text = value.Guest;

                deFrom.DateTime = value.Arrival;
                deTo.EditValue = null;


                deFrom.Properties.MaxValue = value.Departure;

                deTo.Properties.MaxValue = value.Departure;

                teNote.Text = "Wake-up Schedule";

                teRegistrationNumber.Text = value.Registration;

                spin_NumOfDays.Properties.MaxValue = Convert.ToInt32((value.Departure - value.Arrival).TotalDays);


            }
        }

        #endregion

        #region Methods


        private void InitializeUI()
        {
            spin_NumOfDays.EditValue = 0;
        }

        private bool InitializeData()
        {
            try
            {
                if (RegExt == null) return false;
                // Progress_Reporter.Show_Progress("Initializing Data", "Please Wait...");
                //Current Time
                DateTime? date = UIProcessManager.GetServiceTime();
                if (date != null)
                {
                    CurrentTime = date.Value;

                    deFrom.Properties.MinValue = CurrentTime;
                    deTo.Properties.MinValue = CurrentTime;
                    timeEdit.Text = CurrentTime.ToShortTimeString();
                }
                else
                {
                    XtraMessageBox.Show("Can't Get Service Date Time !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ////CNETInfoReporter.Hide();
                    return false;
                }


                //populate grid
                PopulateGrid();

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing wake-up call data", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ////CNETInfoReporter.Hide();
                return false;
            }


        }

        private void PopulateGrid()
        {
            try
            {
                gc_schedules.DataSource = null;
                List<VwScheduleDetailViewDTO> schDetailList = UIProcessManager.GetScheduleDetailByvoucherandType(RegExt.Id, CNETConstantes.WAKEUP_SCHEDULE_TYPE);

                if (schDetailList == null || schDetailList.Count == 0) return;

                VwScheduleDetailViewDTO sDetail = schDetailList.LastOrDefault();
                if (sDetail == null) return;
                WakeupVM wakeupVM = new WakeupVM()
                {
                    FromDate = sDetail.StartDate,
                    ToDate = sDetail.EndDate,
                    regCode = sDetail.Reference,
                    Note = sDetail.HeaderDesc,
                    headerType = sDetail.HeaderType,
                    scheduleCode = sDetail.ScheduleCode,
                    headerCode = sDetail.HeaderCode,
                    detailCode = sDetail.DetailCode

                };

                List<WakeupDetailVM> wDetailList = new List<WakeupDetailVM>();
                WakeupDetailVM detail = new WakeupDetailVM()
                {
                    dayMonth = sDetail.DayMonth,
                    dayOfMonth = (int)sDetail.DayOfMonth,
                    startTime = sDetail.StartTime,
                    endTime = sDetail.EndTime,
                    detailCode = sDetail.DetailCode,

                };

                wDetailList.Add(detail);

                try
                {
                    wakeupVM.Time = DateTime.ParseExact(sDetail.StartTime, "HH:mm", CultureInfo.InvariantCulture);
                }
                catch (Exception ex) { }

                wakeupVM.WakeUpDetails = wDetailList;
                gc_schedules.DataSource = new List<WakeupVM>() { wakeupVM };
                for (int i = 0; i < gv_schedules.RowCount; i++)
                {
                    gv_schedules.ExpandMasterRow(i);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in populating wake-up data. DETAIL::" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private bool SaveScheduleHeader(ScheduleHeaderDTO existedSchHeader)
        {
            try
            {
                if (existedSchHeader != null)
                {
                    //it is updating
                    if (UIProcessManager.UpdateScheduleHeader(existedSchHeader) != null)
                    {
                        //delete existing details
                        List<ScheduleDetailDTO> details = UIProcessManager.SelectAllScheduleDetail().Where(s => s.Seheduleheader == existedSchHeader.Id).ToList();
                        if (details == null) return false;
                        foreach (var d in details)
                        {
                            UIProcessManager.DeleteScheduleDetailById(d.Id);
                        }

                        //generate Schedule Details
                        DateTime date = existedSchHeader.StartDate;
                        while (date <= existedSchHeader.EndDate)
                        {

                            ScheduleDetailDTO sDetail = new ScheduleDetailDTO();
                            sDetail.Seheduleheader = existedSchHeader.Id;
                            sDetail.DayMonth = date.DayOfWeek.ToString();
                            sDetail.StartTime = timeEdit.Time.TimeOfDay;
                            sDetail.EndTime = timeEdit.Time.AddMinutes(15).TimeOfDay;
                            sDetail.DayOfMonth = (byte)date.Day;

                            UIProcessManager.CreateScheduleDetail(sDetail);

                            //update counter
                            date = date.AddDays(1);
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
                else
                {
                    ScheduleHeaderDTO sHeader = new ScheduleHeaderDTO()
                    {
                        Description = teNote.EditValue == null ? "" : teNote.EditValue.ToString(),
                        Cateogry = CNETConstantes.WAKEUP_SCHEDULE_Category,
                        Type = CNETConstantes.WAKEUP_SCHEDULE_TYPE,
                        StartDate = deFrom.DateTime.Date,
                        EndDate = deTo.DateTime.Date,
                        IsActive = true

                    };

                    ScheduleHeaderDTO Savedheader = UIProcessManager.CreateScheduleHeader(sHeader);
                    if (Savedheader == null) return false;

                    //Save Schedule
                    ScheduleDTO schedule = new ScheduleDTO()
                    {
                        Description = "Wake-up Call",
                        Pointer = CNETConstantes.PMS_Pointer,
                        Reference = RegExt.Id,
                        ScheduledHeader = Savedheader.Id
                    };

                    if (UIProcessManager.CreateSchedule(schedule) == null)
                    {
                        return false;
                    }

                    //generate Schedule Details
                    DateTime date = sHeader.StartDate;
                    while (date <= sHeader.EndDate)
                    {

                        ScheduleDetailDTO sDetail = new ScheduleDetailDTO();
                        sDetail.Seheduleheader = Savedheader.Id;
                        sDetail.DayMonth = date.DayOfWeek.ToString();
                        sDetail.StartTime = timeEdit.Time.TimeOfDay;
                        sDetail.EndTime = timeEdit.Time.AddMinutes(15).TimeOfDay;
                        sDetail.DayOfMonth = (byte)date.Day;

                        UIProcessManager.CreateScheduleDetail(sDetail);

                        //update counter
                        date = date.AddDays(1);
                    }

                    return true;


                }//end of else

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in saving schedule. DETAIL::" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void ResetForm()
        {
            _isHeaderUpdate = false;
            _isDetailUpdate = false;
            spin_NumOfDays.EditValue = 0;
            deFrom.DateTime = RegExt.Arrival;
            deTo.EditValue = null;
            timeEdit.EditValue = CurrentTime.Date.TimeOfDay;
            teNote.Text = "Wake-up call";

        }


        #endregion

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            deTo.EditValue = DateTime.Now;
            this.Close();
        }

        private void frmWakeupCall_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }

        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Progress_Reporter.Show_Progress("Saving", "Please Wait...");

            List<Control> controls = new List<Control>
                {
                    deTo
                };


            if (_isHeaderUpdate)
            {


                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    ////CNETInfoReporter.Hide();
                    return;

                }

                if (_currentScheduleHeader != null)
                {
                    bool result = SaveScheduleHeader(_currentScheduleHeader);
                    if (result)
                    {
                        _isHeaderUpdate = false;
                        _currentScheduleHeader = null;
                        XtraMessageBox.Show("Updated!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetForm();
                        //refresh grid
                        PopulateGrid();
                    }
                    else
                    {
                        XtraMessageBox.Show("Not Updated!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


            }
            else if (_isDetailUpdate)
            {
                _isDetailUpdate = false;
            }
            else
            {
                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    ////CNETInfoReporter.Hide();
                    return;

                }

                bool result = SaveScheduleHeader(null);
                if (result)
                {
                    XtraMessageBox.Show("Saved!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetForm();

                    //refresh grid
                    PopulateGrid();
                }
                else
                {
                    XtraMessageBox.Show("Not Saved!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


            ////CNETInfoReporter.Hide();

            //Load Buffer List 
        }

        private void spin_NumOfDays_EditValueChanged(object sender, EventArgs e)
        {
            if (spin_NumOfDays.EditValue != null)
            {
                deTo.DateTime = deFrom.DateTime.AddDays(Convert.ToInt32(spin_NumOfDays.EditValue.ToString()));
            }

        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetForm();
        }

        private void gv_schedules_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gv_scheduleDetail_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void bbiEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            WakeupVM selectedVM = (WakeupVM)gv_schedules.GetFocusedRow();
            if (selectedVM == null)
            {
                SystemMessage.ShowModalInfoMessage("Please Select Wake-Up Call Header!", "ERROR");
                return;
            }

            _isHeaderUpdate = true;
            deFrom.DateTime = selectedVM.FromDate;
            deTo.DateTime = selectedVM.ToDate;
            spin_NumOfDays.EditValue = Convert.ToInt32((deTo.DateTime - deFrom.DateTime).TotalDays);
            teNote.Text = selectedVM.Note;
            timeEdit.EditValue = selectedVM.Time;
            _currentScheduleHeader = new ScheduleHeaderDTO()
            {
                Id = selectedVM.headerCode.Value,
                Description = selectedVM.Note,
                StartDate = selectedVM.FromDate,
                EndDate = selectedVM.ToDate,
                Cateogry = selectedVM.headerType,
                Type = selectedVM.headerType

            };

        }

        private void bbiRemoveAlarm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                WakeupVM selectedVM = (WakeupVM)gv_schedules.GetFocusedRow();
                if (selectedVM == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please Select Wake-Up Call Header!", "ERROR");
                    return;
                }

                bool flag = UIProcessManager.DeleteScheduleDetailById(selectedVM.detailCode);
                if (flag)
                {
                    flag = UIProcessManager.DeleteScheduleHeaderById(selectedVM.headerCode.Value);
                }

                if (flag)
                {
                    SystemMessage.ShowModalInfoMessage("Wake-Up call is removed!", "MESSAGE");
                    //refresh grid
                    PopulateGrid();
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Wake-Up call is not removed!", "ERROR");
                }

            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in removing wake-up call. Detail:: " + ex.Message, "ERROR");

            }


        }//end of method


    }
}
