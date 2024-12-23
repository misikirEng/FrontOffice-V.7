using CNET.ERP.Client.Common.UI; 
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using ERP.EventManagement.DTO;
using ProcessManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.EventManagement.Modals
{
    public partial class EventDetailForm : Form
    {

        private int _defType;
        private int _defArrangment;

        private bool _isEdit;
        public decimal Guaranteedpax { get; set; }
        public EventDetaildataDTO EventDetailDTO { get; set; }
        public int? EventHeaderId { get; set; }
        public string EventHeaderCode { get; set; }

        public int? SelectedHotelcode { get; set; }
        public DateTime CurrentTime { get; set; }

        private List<EventDisplayView> _eventDetailList;
        private List<RoomDetailDTO> _hallList = new List<RoomDetailDTO>();

        /***********************************    CONSTRUCTOR *******************************/
        public EventDetailForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        #region Helper Methods

        private void InitializeUI()
        {
            //Type
            lukType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Type"));
            lukType.Properties.DisplayMember = "Description";
            lukType.Properties.ValueMember = "Id";

            //Hall
            GridColumn col = sluHall.Properties.View.Columns.AddField("Space");
            col.Visible = false;
            col = sluHall.Properties.View.Columns.AddField("Description");
            col.Visible = true;
            sluHall.Properties.DisplayMember = "Description";
            sluHall.Properties.ValueMember = "Space";

            //Arrangment
            lukArrangement.Properties.Columns.Add(new LookUpColumnInfo("Description", "Arrangement"));
            lukArrangement.Properties.DisplayMember = "Description";
            lukArrangement.Properties.ValueMember = "Id";

        }

        private bool InitializeData()
        {
            try
            {
               
                if (EventHeaderId == null)
                {
                    XtraMessageBox.Show("Select Event Header First", "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                txtNoPeople.Value = Guaranteedpax;
                //Progress_Reporter.Show_Progress("Initializing Event Detail Editor", "Please Wait...");

                DateTime? currentDate = GetCurrentTime();
                if (currentDate == null)
                {
                    //Progress_Reporter.Close_Progress();
                    return false;
                }

                CurrentTime = currentDate.Value;

                _isEdit = EventDetailDTO == null ? false : true;


                //Populate Type
                var typeList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.EVENT_TYPE).ToList();
                lukType.Properties.DataSource = typeList;
                if (typeList != null)
                {
                    var def = typeList.FirstOrDefault(l => l.IsDefault);
                    if (def != null)
                    {
                        _defType = def.Id;
                    }
                }

                //Populate Arrangement
                var arrangList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.EVENT_ARRANGEMENT).ToList();
                lukArrangement.Properties.DataSource = arrangList;
                if (arrangList != null)
                {
                    var def = arrangList.FirstOrDefault(l => l.IsDefault);
                    if (def != null)
                    {
                        _defArrangment = def.Id;
                    }
                }

                //Populate Hall

                List<RoomTypeDTO> meetingRoomTypes = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value).Where(rt => rt.IsActive && rt.CanBeMeetingRoom.Value).ToList();
                if (meetingRoomTypes != null)
                {
                    foreach (var rt in meetingRoomTypes)
                    {
                        List<RoomDetailDTO> halls = UIProcessManager.GetRoomDetailByroomType(rt.Id);
                        if (halls != null && halls.Count > 0)
                        {
                            _hallList.AddRange(halls);
                        }
                    }
                }

                //Get Event Header's Start and End Date
                VoucherDTO regExt = UIProcessManager.GetVoucherById(EventHeaderId.Value);
                if (regExt == null)
                {
                    //Progress_Reporter.Close_Progress();
                    XtraMessageBox.Show("Fail to get Event Voucher Data !!", "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                DateTime startDate = regExt.StartDate.Value;
                DateTime endDate = regExt.EndDate.Value;

                deDate.Properties.MinValue = startDate;
                deDate.Properties.MaxValue = endDate;

                //Populate Existing Event Details in the Current Date Range
                _eventDetailList = UIProcessManager.GetEventDisplayView(startDate.Date, endDate.Date, SelectedHotelcode);

                //Populate Time combo
                PopulateTimeCombo();

                deDate.EditValue = startDate;
                comboStartTime.EditValue = "06:00";
                comboEndTime.EditValue = "07:00";

                if (_isEdit)
                {
                    var evDetaillist = UIProcessManager.GetEventDetailByIdlist(EventDetailDTO.Id);
                    if (evDetaillist == null || evDetaillist.Count == 0)
                    {
                        //Progress_Reporter.Close_Progress();
                        XtraMessageBox.Show("Unable to find the Event Detail", "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    EventDetailDTO evDetail = evDetaillist.FirstOrDefault();

                    lukType.EditValue = evDetail.Type;
                    txtDescription.EditValue = evDetail.Description;
                    sluHall.EditValue = evDetail.Space;
                    lukArrangement.EditValue = evDetail.SpaceArrangment;
                    deDate.EditValue = evDetail.StartTime;
                    txtNoPeople.Value = evDetail.NoOfPerson;
                    comboStartTime.EditValue = evDetail.StartTime.ToString("HH:mm");
                    comboEndTime.EditValue = evDetail.EndTime.ToString("HH:mm");
                    txtRemark.EditValue = evDetail.Remark;

                }
                else
                {
                    lukType.EditValue = _defType;
                    lukArrangement.EditValue = _defArrangment;


                }


                //Progress_Reporter.Close_Progress();
                return true;

            }
            catch (Exception ex)
            {
                //Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Error in initializing data. DETAIL:: " + ex.Message, "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;

            }
        }

        private DateTime? GetCurrentTime()
        {
            DateTime? date = UIProcessManager.GetServiceTime();
            if (date != null)
            {
                return date;

            }
            else
            {
                XtraMessageBox.Show("Error Geting Server datetime !!", "ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; ;
            }
        }
        private void Reset()
        {
            lukType.EditValue = _defType;
            txtDescription.EditValue = null;
            sluHall.EditValue = null;
            lukArrangement.EditValue = _defArrangment;
            comboStartTime.EditValue = "06:00";
            comboEndTime.EditValue = "07:00";
            txtNoPeople.Value = Guaranteedpax;
            txtRemark.EditValue = null;

        }

        private void PopulateTimeCombo()
        {
            List<string> values = new List<string>();
            var start = new DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day);
            var clockQuery = from offset in Enumerable.Range(0, 96) // 12: since we want to start the time to start from 06:00 and each interval is 30Min, 36: since we need 36 hours instad of 48 hours
                             select start.AddMinutes(15 * offset);

            foreach (var time in clockQuery)
            {
                values.Add(time.ToString("HH:mm"));

            }

            comboStartTime.Properties.Items.AddRange(values);
            comboEndTime.Properties.Items.AddRange(values);
        }


        private void PopulateHallsByDate(DateTime startTime, DateTime endTime)
        {
            beiStatusHall.EditValue = null;

            if (_eventDetailList == null || _eventDetailList.Count == 0)
            {
                sluHall.Properties.DataSource = _hallList;
            }
            if (_eventDetailList == null)
                _eventDetailList = new List<EventDisplayView>();

            var filteredEvDetails = _eventDetailList.Where(e => (DateTime.Compare(startTime, e.startTimeStamp) >= 0 && DateTime.Compare(startTime, e.endTimeStamp) <= 0) ||
             (DateTime.Compare(endTime, e.startTimeStamp) >= 0 && DateTime.Compare(endTime, e.endTimeStamp) <= 0)
            ).ToList();

            if (filteredEvDetails != null && filteredEvDetails.Count > 0)
            {
                List<string> existedHalls = filteredEvDetails.Select(ev => ev.HallDescription).ToList();
                if (_isEdit && existedHalls.Contains(EventDetailDTO.Hall))
                {
                    existedHalls.Remove(EventDetailDTO.Hall);
                }
                List<RoomDetailDTO> filteredHalls = _hallList.Where(h => !existedHalls.Contains(h.Description)).ToList();
                sluHall.Properties.DataSource = filteredHalls;

                StringBuilder sb = new StringBuilder();
                foreach (var hall in existedHalls)
                {
                    sb.Append(hall);
                    sb.Append(",");
                }

                beiStatusHall.EditValue = sb.ToString();

            }
            else
            {
                sluHall.Properties.DataSource = _hallList;
            }


        }



        #endregion

        #region Event Handlers

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                    lukType,
                    sluHall,
                    lukArrangement,
                    comboStartTime,
                    comboEndTime,
                    txtNoPeople,
                    txtDescription
                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    return;
                }


                //Progress_Reporter.Show_Progress("Saving Event Detail", "Please Wait...");

                EventDetailDTO isSaved = null;

                DateTime now = deDate.DateTime;
                DateTime startTime = Convert.ToDateTime(comboStartTime.EditValue.ToString());
                DateTime endTime = Convert.ToDateTime(comboEndTime.EditValue.ToString());

                EventDetailDTO currentEventDetail = new EventDetailDTO();
                currentEventDetail.Description = txtDescription.Text;
                currentEventDetail.Voucher = EventHeaderId.Value;
                currentEventDetail.Type = Convert.ToInt32(lukType.EditValue.ToString());
                currentEventDetail.Space = Convert.ToInt32(sluHall.EditValue.ToString());
                currentEventDetail.SpaceArrangment = Convert.ToInt32(lukArrangement.EditValue.ToString());
                currentEventDetail.NoOfPerson = (int)txtNoPeople.Value;
                currentEventDetail.StartTime = new DateTime(now.Year, now.Month, now.Day, startTime.Hour, startTime.Minute, startTime.Second);
                currentEventDetail.EndTime = new DateTime(now.Year, now.Month, now.Day, endTime.Hour, endTime.Minute, endTime.Second);
                currentEventDetail.Remark = txtRemark.Text;


                if (!_isEdit)
                {
                    isSaved = UIProcessManager.CreateEventDetail(currentEventDetail);
                    if (isSaved != null)
                    {
                        Reset();
                    }

                }
                else
                {
                    //Updating
                    currentEventDetail.Id = EventDetailDTO.Id;
                    isSaved = UIProcessManager.UpdateEventDetail(currentEventDetail);
                }
                if (isSaved != null)
                {
                    XtraMessageBox.Show("Event Detail is sucessfully saved !", "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    XtraMessageBox.Show("Event Detail is not saved !", "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }



                //Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                //Progress_Reporter.Close_Progress();
                XtraMessageBox.Show("Error in Saving Event Detail. DETAIL:: " + ex.Message, "Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }


        private void EventDetailForm_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }


        #endregion

        private void ceAllDay_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit view = sender as CheckEdit;
            if (view.Checked)
            {
                comboStartTime.EditValue = "06:00";
                comboEndTime.EditValue = "23:30";
                comboStartTime.Enabled = false;
                comboEndTime.Enabled = false;
            }
            else
            {
                comboStartTime.EditValue = "06:00";
                comboEndTime.EditValue = "07:00";
                comboStartTime.Enabled = true;
                comboEndTime.Enabled = true;
            }
        }

        private void deDate_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void comboStartTime_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit view = sender as ComboBoxEdit;
            string value = view.EditValue.ToString();

            DateTime now = deDate.DateTime;
            DateTime startTime = Convert.ToDateTime(value);
            if (comboEndTime.EditValue == null) return;
            DateTime endTime = Convert.ToDateTime(comboEndTime.EditValue.ToString());

            DateTime startTimeStamp = new DateTime(now.Year, now.Month, now.Day, startTime.Hour, startTime.Minute, startTime.Second);
            DateTime endTimeStamp = new DateTime(now.Year, now.Month, now.Day, endTime.Hour, endTime.Minute, endTime.Second);

            PopulateHallsByDate(startTimeStamp, endTimeStamp);
        }

        private void comboEndTime_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit view = sender as ComboBoxEdit;
            string value = view.EditValue.ToString();

            DateTime now = deDate.DateTime;
            DateTime startTime = Convert.ToDateTime(comboStartTime.EditValue.ToString());
            DateTime endTime = Convert.ToDateTime(value);

            DateTime startTimeStamp = new DateTime(now.Year, now.Month, now.Day, startTime.Hour, startTime.Minute, startTime.Second);
            DateTime endTimeStamp = new DateTime(now.Year, now.Month, now.Day, endTime.Hour, endTime.Minute, endTime.Second);

            PopulateHallsByDate(startTimeStamp, endTimeStamp);
        }
    }
}
