
using CNET.ERP.Client.Common.UI;
using ProcessManager;
using CNET.Progress.Reporter;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler;
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

namespace CNET.FrontOffice_V._7.Forms.Pinned_Tabs
{
    public partial class frmRegistrationDensityChart : UILogicBase
    {

        #region Declarations

        BindingList<RegistrationDocumentDTO> schedule = new BindingList<RegistrationDocumentDTO>();
        BindingList<RegistrationDocumentDTO> scheduleResources = new BindingList<RegistrationDocumentDTO>();

        #endregion

        #region Constractors

        public frmRegistrationDensityChart()
        {
            InitializeComponent();
            InitResourcesCustom();
            reiHotel.DisplayMember = "Name";
            reiHotel.ValueMember = "Id";
            reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

            if (LocalBuffer.LocalBuffer.HotelBranchBufferList != null && LocalBuffer.LocalBuffer.HotelBranchBufferList.Count > 0)
            {
                beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;
            }
        }

        private void GetAndSetDateTime()
        {
            DateTime? Today = UIProcessManager.GetServiceTime();// DateTime.Now;
            if (Today == null)
            {
                XtraMessageBox.Show("Server DateTime Error !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Today.Value.Month);
            string year = Today.Value.Year.ToString();
            beiMonth.EditValue = Month;
            beiYear.EditValue = year;
        }

        private void GetAllRooms()
        {
            scheduleResources.Clear();
            schedule.Clear();
            List<RoomDetailDTO> RoomDetailList = UIProcessManager.SelectAllRoomDetail();
            List<RoomTypeDTO> RoomTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value).Where(x => x.IspseudoRoomType == false).ToList();
            List<RegistrationDocumentDTO> FullRoomlist = (from roomd in RoomDetailList
                                                          join roomt in RoomTypeList
                                                              on roomd.RoomType equals roomt.Id
                                                          select new RegistrationDocumentDTO
                                                          {
                                                              code = roomd.Description + roomt.Description,
                                                              RoomNumber = "Room:- " + roomd.Description,
                                                              RoomTypeDescription = roomt.Description
                                                          }).ToList();

            if (FullRoomlist != null && FullRoomlist.Count > 0)
            {
                FullRoomlist = FullRoomlist.OrderBy(x => x.code).ToList().GroupBy(y => y.code).Select(s => s.First()).ToList();
                scheduleResources = new BindingList<RegistrationDocumentDTO>(FullRoomlist);
            }


            //foreach (RoomTypeDTO regRoom in RoomTypeList)
            //{
            //    RegistrationDocumentDTO rDto = new RegistrationDocumentDTO();
            //    rDto.Id = regRoom.Id;
            //    rDto.RoomNumber = regRoom.Description;
            //    rDto.RoomTypeDescription = null;
            //    if (!scheduleResources.Contains(rDto))
            //    {
            //        scheduleResources.Add(rDto);
            //    }
            //}
            schedulerStorage1.Resources.DataSource = scheduleResources;

        }

        #endregion

        #region Public Methods

        public void InitResourcesCustom()
        {
            ResourceMappingInfo mappings = this.schedulerStorage1.Resources.Mappings;
            mappings.Id = "code";
            mappings.Caption = "RoomNumber";
            mappings.ParentId = "RoomTypeDescription";

            AppointmentMappingInfo apMappings = this.schedulerStorage1.Appointments.Mappings;
            apMappings.ResourceId = "code";
            apMappings.Start = "arrivalDate";
            apMappings.End = "departureDate";
            apMappings.Subject = "Guest";
            apMappings.Label = "roomCount";


        }

        public void GetInHouseGuesthDataByMonth()
        {
            schedule.Clear();
            if (beiYear.EditValue != null && !string.IsNullOrEmpty(beiYear.EditValue.ToString()) && !string.IsNullOrEmpty(beiMonth.EditValue.ToString()) && beiMonth.EditValue != null)
            {
                DateTime FromDate = DateTime.Now.Date;
                DateTime ToDate = DateTime.Now.Date.AddMonths(1);

                try
                {
                    FromDate = Convert.ToDateTime("1/" + beiMonth.EditValue + "/" + beiYear.EditValue).Date;
                    ToDate = Convert.ToDateTime("1/" + beiMonth.EditValue + "/" + beiYear.EditValue).AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                catch
                {
                    XtraMessageBox.Show("Please Select Year and Month Properly !!", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GetAndSetDateTime();
                    return;
                }

                Progress_Reporter.Show_Progress("Getting Room Data...... " + Environment.NewLine +
                                          "From:- " + FromDate.ToString() + Environment.NewLine +
                                          "To Date:- " + ToDate.ToString(), "Please Wait...");

                List<RegistrationDocumentDTO> _regDocL = UIProcessManager.GetRegistrationDTOData(FromDate, ToDate, null, SelectedHotelcode.Value).OrderBy(p => p.RoomNumber).ToList();
                schedulerControl1.Start = FromDate;

                if (_regDocL != null && _regDocL.Count > 0)
                {

                    #region Guest Rooms Object
                    Progress_Reporter.Show_Progress("Popluating Guest Rooms...... ", "Please Wait...");
                    _regDocL.ForEach(x => x.code = (x.RoomNumber + x.RoomTypeDescription));
                    _regDocL.Where(y => y.foStatus == CNETConstantes.CHECKED_OUT_STATE).ToList().ForEach(x => x.roomCount = 2);
                    _regDocL.Where(y => y.foStatus == CNETConstantes.CHECKED_IN_STATE).ToList().ForEach(x => x.roomCount = 3);
                    _regDocL.Where(y => y.foStatus == CNETConstantes.SIX_PM_STATE).ToList().ForEach(x => x.roomCount = 4);
                    _regDocL.Where(y => y.foStatus == CNETConstantes.GAURANTED_STATE).ToList().ForEach(x => x.roomCount = 5);

                    _regDocL.ForEach(x => x.RoomNumber = ("Room:- " + x.RoomNumber));
                    schedule = new BindingList<RegistrationDocumentDTO>(_regDocL);
                    schedulerStorage1.Appointments.DataSource = schedule;
                    #endregion
                }
                else
                {
                    XtraMessageBox.Show("There is no Registration Data in this month and year !!", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                XtraMessageBox.Show("Please Select Year and Month first !!", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            schedulerStorage1.RefreshData();
            Progress_Reporter.Close_Progress();
        }

        #endregion

        #region Private Methods
        private void bbiShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetInHouseGuesthDataByMonth();
        }
        #endregion

        public int? SelectedHotelcode { get; set; }
        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {
            SelectedHotelcode = beiHotel.EditValue == null ? null : Convert.ToInt32(beiHotel.EditValue);
            GetAllRooms();
            GetAndSetDateTime();
        }

    }
}
