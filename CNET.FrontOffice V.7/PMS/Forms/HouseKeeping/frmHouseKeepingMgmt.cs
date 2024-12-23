#define all_checked
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid;
using CNET.FrontOffice_V._7;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars;
using DevExpress.Utils.Win;
using DevExpress.XtraLayout;
using System.Timers;
using DevExpress.XtraReports.UI;
using CNET_V7_Domain.Misc.PmsView;
using ProcessManager;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.CommonSchema;
using System.Runtime.Intrinsics.Arm;

namespace CNET.FrontOffice_V._7.HouseKeeping
{

    public partial class frmHouseKeepingMgmt : UILogicBase

    {
        private string roomNo;
        private string roomType;
        private string roomStatus;
        private string foStatus;
        private string resStatus;
        private string floor;
        private int code;
        private string SN;


        List<BindRoomMgmtVM> holdValue = new List<BindRoomMgmtVM>();
        List<VwRoomManagmentViewDTO> rmDetail = null;
        static DateTime? currentDate = UIProcessManager.GetServiceTime();
        List<CheckBox> checkBoxNames = new List<CheckBox>();
        List<CheckEdit> checkEditNames = new List<CheckEdit>();
        TextEdit roomNum;
        LookUpEdit roomtp;
        const string HK_ACTIVITY = "HK_ACTIVITY";
        List<DevExpress.XtraEditors.CheckEdit> c = new List<DevExpress.XtraEditors.CheckEdit>();

        List<BarCheckItem> checkboxenames = new List<BarCheckItem>();

        static string time = currentDate.Value.Date.ToString("yyyy-MM-dd");
        DateTime t = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        List<BarCheckItem> checkBoxGroup1;
        List<BarCheckItem> checkBoxGroup2;
        List<BarCheckItem> checkBoxGroup3;
        List<BarCheckItem> checkBoxGroup4;
        List<BarCheckItem> allchecks = new List<BarCheckItem>();
        private String rmType = "";
        private String rmNum = "";
        private String floorSrch = "";
        private int numOfCleans;
        private int numOfDirties;
        private int numOfOOOs;
        private int numOfOOSs;
        private int numOfPickups;
        private int numOfInsps;
        BarCheckItem od = new BarCheckItem();
        BarCheckItem ev = new BarCheckItem();
        List<string> oddEvenList = new List<string>();
        List<RoomTypeDTO> pseuodoRooms = new List<RoomTypeDTO>();
        DateTime CurrentTime { get; set; }

        public frmHouseKeepingMgmt()
        {

            InitializeComponent();
            //SecurityCheck("Room Managment");
            try
            {
                DateTime? date = UIProcessManager.GetServiceTime();
                if (date != null)
                {
                    CurrentTime = date.Value;
                }
                else
                {
                    XtraMessageBox.Show("Error Server Datetime !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            od.AccessibleName = "Odd";
            ev.AccessibleName = "Even";
            checkBoxGroup1 = new List<BarCheckItem>() { cln1, drt1, oos, insp1, pkcp, ooo };
            checkBoxGroup2 = new List<BarCheckItem>() { od, ev };
            checkBoxGroup3 = new List<BarCheckItem>() { vac, occ };
            checkBoxGroup4 = new List<BarCheckItem>() { notres, ckout, dout, stovr, arrvd, arvls };
            allchecks.AddRange(checkBoxGroup1);
            allchecks.AddRange(checkBoxGroup2);
            allchecks.AddRange(checkBoxGroup3);
            allchecks.AddRange(checkBoxGroup4);

            rmTypeCombo.EditValueChanged += new EventHandler(RoomType_Combo_ValueChanged);
            CustomizeNavigator();
            CheckandCreateHouseKeppingActivity();
        }

        private void SecurityCheck(string parent)
        {
            List<String> allowedTabs = MasterPageForm.AllowedFunctionalities(parent);
            int closeedCounter = 0;
            if (!allowedTabs.Contains("Print"))
            {
                bbiPrint.Enabled = false;
                closeedCounter++;
            }
            if (!allowedTabs.Contains("Save"))
            {
                bbiSave.Enabled = false;
                closeedCounter++;
            }
            if (closeedCounter == 2)
            {
                this.Enabled = false;
            }
        }

        private void RoomMgmt_OnLoad(object sender, EventArgs e)
        {
            try
            {
                ShowActivity();
                DefaultRoomStatus();
                pseuodoRooms = UIProcessManager.GetRoomTypeByispseudoRoomType(true);

                reiHotel.DisplayMember = "Name";
                reiHotel.ValueMember = "Id";
                reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

                if (LocalBuffer.LocalBuffer.HotelBranchBufferList != null && LocalBuffer.LocalBuffer.HotelBranchBufferList.Count > 0)
                {
                    beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                    //reiHotel.ReadOnly = !MasterPageForm.UserHasHotelBranchAccess;

                }
                StatusNumber();


                List<string> comboList = new List<string>();
                //comboList = UIProcessManager.GetObjectStateDfn("1001").Select(b => b.description).ToList();


                List<SystemConstantDTO> list = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(x => x.Category == "Housekeeping Activities").ToList();

                for (int i = 0; i < 6; i++)
                {
                    string state = list[i].Description;
                    comboList.Add(state);
                }

                repositoryItemComboBox3.Items.AddRange(comboList);
                repositoryItemComboBox3.SelectedIndexChanged += new EventHandler(RoomStatus_SelectedIndexChanged);
                romn.Popup += new EventHandler(romn_Popup);
                Toroomn.Popup += new EventHandler(Toroomn_Popup);
                floorCom.Popup += new EventHandler(floor_popup);

                oddEvenList = new List<string>() { "Odd", "Even", "All" };
                odevcombo.DataSource = oddEvenList;
                barEditItem4.EditValue = oddEvenList[2];
            }
            catch (Exception ex)
            {

            }
        }

        private void ShowActivity()
        {
            try
            {
                List<VwActivityDetailViewDTO> hkActivity = new List<VwActivityDetailViewDTO>();
                hkActivity = UIProcessManager.GetActivityDetailView(null, currentDate, null).Where(b => b.Remark == HK_ACTIVITY).OrderByDescending(b => b.TimeStamp).ToList();
                grdActivity.DataSource = hkActivity;
            }
            catch (Exception ex)
            {

            }

        }
        private void floor_popup(object sender, EventArgs e)
        {
            //IPopupControl popupControl = sender as IPopupControl;
            //LayoutControl layoutControl = popupControl.PopupWindow.Controls[2].Controls[0] as LayoutControl;
            //SimpleButton clearButton = ((LayoutControlItem)layoutControl.Items.FindByName("lciClear")).Control as SimpleButton;
            //if (clearButton != null)
            //{
            //    //clearButton.Text = "All";
            //    clearButton.Click += new EventHandler(clearFloorClick);
            //}
        }

        private void Toroomn_Popup(object sender, EventArgs e)
        {
            //IPopupControl popupControl = sender as IPopupControl;
            //LayoutControl layoutControl = popupControl.PopupWindow.Controls[2].Controls[0] as LayoutControl;
            //SimpleButton clearButton = ((LayoutControlItem)layoutControl.Items.FindByName("lciClear")).Control as SimpleButton;
            //if (clearButton != null)
            //{
            //    clearButton.Click += new EventHandler(ClearToRoomNumClick);
            //}
        }
        private void romn_Popup(object sender, EventArgs e)
        {
            //IPopupControl popupControl = sender as IPopupControl;
            //LayoutControl layoutControl = popupControl.PopupWindow.Controls[2]as LayoutControl;
            //SimpleButton clearButton = ((LayoutControlItem)layoutControl.Items.FindByName("lciClear")).Control as SimpleButton;
            //if (clearButton != null)
            //{
            //    //clearButton.Text = "All";
            //    clearButton.Click += new EventHandler(ClearRoomNumClick);
            //}
        }


        private void rmTypeCombo1_Popup(object sender, EventArgs e)
        {
            //IPopupControl popupControl = sender as IPopupControl;
            //LayoutControl layoutControl = popupControl.PopupWindow.Controls[2].Controls[0] as LayoutControl;
            //SimpleButton clearButton = ((LayoutControlItem)layoutControl.Items.FindByName("lciClear")).Control as SimpleButton;
            //if (clearButton != null)
            //{
            //    //clearButton.Text = "All";
            //    clearButton.Click += new EventHandler(clearButton_Click);
            //}
        }


        private void clearButton_Click(object sender, EventArgs e)
        {
            rmType = "";
            checkBoxers();
        }

        private void clearFloorClick(object sender, EventArgs e)
        {
            rmNum = "";
            floorSrch = "";
            romnSearchGrid.EditValue = null;
            checkBoxers();
        }

        private void ClearRoomNumClick(object sender, EventArgs e)
        {
            rmNum = "";
            checkBoxers();
        }

        private void ClearToRoomNumClick(object sender, EventArgs e)
        {
            checkBoxers();
        }
        private void rmNo_KeyDown(object sender, KeyEventArgs e)
        {
            TextEdit tx = sender as TextEdit;
            string text = tx.Text;
            roomNum = tx;
            if (e.KeyCode == Keys.Enter)
            {
                List<BindRoomMgmtVM> holdSelectedValue = new List<BindRoomMgmtVM>();
                List<BindRoomMgmtVM> filterHoldSelectedValue = new List<BindRoomMgmtVM>();
                List<BindRoomMgmtVM> vacantlist = holdValue as List<BindRoomMgmtVM>;

                string t = "";// selectRoomTypeCombo.Text;
                if (roomtp != null)
                {
                    t = (string)roomtp.EditValue;
                }
                if (t != "")
                {
                    holdSelectedValue = vacantlist.Where(l => l.roomNo.Contains(text) & l.roomType == t).ToList();
                }
                else
                {
                    holdSelectedValue = vacantlist.Where(l => l.roomNo.Contains(text)).ToList();
                }

                int count = 0;
                foreach (CheckBox ch in checkBoxNames)
                {
                    if (ch.Checked)
                    {
                        List<BindRoomMgmtVM> rm = vacantlist.Where(x => x.roomStatus == ch.Text & x.roomNo.Contains(text)).ToList();
                        filterHoldSelectedValue.AddRange(rm);
                        if (count >= 6) { break; }
                        count++;
                    }
                }
                if (filterHoldSelectedValue.Count == 0) { gridControl1.DataSource = holdSelectedValue; }
                else { gridControl1.DataSource = filterHoldSelectedValue; }
            }
        }



        private void CloseForm(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();

        }

        List<int> changedRows = new List<int>();

        private void RoomStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            int k = gridView1.FocusedRowHandle;
            changedRows.Add(k);
            gridView1.PostEditor();
        }

        private void PopulateInitialGrid()
        {
            holdValue = new List<BindRoomMgmtVM>();
            List<VwRoomManagmentViewDTO> orig = new List<VwRoomManagmentViewDTO>();
            orig = UIProcessManager.GetAllRoomManagment(SelectedHotelcode);
            rmDetail = new List<VwRoomManagmentViewDTO>();
            List<RoomTypeDTO> roomTypeDTOs = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value);
            List<RoomTypeDTO> li = roomTypeDTOs.Where(x => x.IspseudoRoomType == true || x.CanBeMeetingRoom == true || x.IsHouseKeeping == false).ToList();

            if (li.Count > 0 && li != null)
            {
                foreach (RoomTypeDTO r in li)
                {
                    orig.RemoveAll(b => b.RoomTypeId == r.Id);
                }
                rmDetail.AddRange(orig);
            }
            else
            {
                rmDetail.AddRange(orig);
            }



            if (rmDetail != null)
                rmDetail = rmDetail.Where(x => RoomDetailcodeList.Contains(x.roomDetailCode)).ToList();

            int i = 1;
            List<RegistrationStatusDTO> foStatusList = new List<RegistrationStatusDTO>();

            if (rmDetail != null && rmDetail.Count > 0)
                foStatusList = UIProcessManager.GetRegistrationStatusList(rmDetail.Select(r => r.roomDetailCode).ToList(), t);
            foreach (VwRoomManagmentViewDTO rd in rmDetail)
            {
                SN = i.ToString();
                roomNo = rd.roomNo.ToString();
                roomType = rd.RoomType.ToString();
                roomStatus = rd.rmstatus.ToString();
                code = rd.roomDetailCode;
                //Status foandresStatus = PMSUIProcessManager.GetRegistrationStatus(rd.roomDetailCode, t);
                RegistrationStatusDTO foandresStatus = null;
                if (foStatusList != null && foStatusList.Count > 0)
                {
                    foandresStatus = foStatusList.FirstOrDefault(f => f.RoomNumber == code);
                }
                string fs = "";
                if (foandresStatus == null) { foStatus = "Vacant"; resStatus = "Not Reserved"; }
                else
                {
                    fs = foandresStatus.FOStatus;
                    resStatus = foandresStatus.registrationStatus;
                }
                if ((fs == null || fs == "") && (resStatus == null || resStatus == "")) { foStatus = "Vacant"; resStatus = "Not Reserved"; }
                else if (fs.Equals("0")) { foStatus = "Vacant"; }
                else if (fs.Equals("1")) { foStatus = "Occupied"; }
                floor = rd.Floor.ToString();

                holdValue.Add(new BindRoomMgmtVM(true, SN, roomNo, roomType, roomStatus, foStatus, resStatus, floor, code));
                i++;
            }
            gridControl1.DataSource = holdValue;
            ////CNETInfoReporter.Hide();
        }

        private Color StatusColor(int lookup)
        {
            int? objectState = GetActivityCode(lookup);
            VwRoomManagmentViewDTO mgmt = UIProcessManager.GetAllRoomManagment(SelectedHotelcode).Where(r => r.roomStatusCode == objectState).First();
            if (mgmt == null)
            {
                return Color.Transparent;
            }
            else
            {

                if (mgmt.color.Contains(','))
                {
                    string[] colorValues = mgmt.color.Split(',');
                    int r = Convert.ToInt32(colorValues[0]);
                    int g = Convert.ToInt32(colorValues[1]);
                    int b = Convert.ToInt32(colorValues[2]);
                    Color color = Color.FromArgb(r, g, b);
                    return color;
                }
                else
                {
                    Color color = Color.FromName(mgmt.color);

                    return color;
                }

            }
        }

        private void SetColorCode(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    string colorCode = View.GetRowCellDisplayText(e.RowHandle, View.Columns["roomStatus"]);
                    if (colorCode == "")
                    {
                        if (e.RowHandle % 2 == 0)
                            gridView1.Columns["roomStatus"].AppearanceCell.BackColor = Color.White;
                        else
                            gridView1.Columns["roomStatus"].AppearanceCell.BackColor = Color.AliceBlue;
                    }
                    else if (colorCode == CNETConstantes.status_oos)
                    {
                        Color color = StatusColor(CNETConstantes.OOS);
                        gridView1.Columns["roomStatus"].AppearanceCell.BackColor = color;
                    }
                    else if (colorCode == CNETConstantes.status_clean)
                    {
                        Color color = StatusColor(CNETConstantes.CLEAN);
                        gridView1.Columns["roomStatus"].AppearanceCell.BackColor = color;
                    }
                    else if (colorCode == CNETConstantes.status_dirty)
                    {
                        Color color = StatusColor(CNETConstantes.Dirty);
                        gridView1.Columns["roomStatus"].AppearanceCell.BackColor = color;
                    }
                    else if (colorCode == CNETConstantes.status_inspected)
                    {
                        Color color = StatusColor(CNETConstantes.INSPECTED);
                        gridView1.Columns["roomStatus"].AppearanceCell.BackColor = color;
                    }
                    else if (colorCode == CNETConstantes.status_ooo)
                    {
                        Color color = StatusColor(CNETConstantes.OOO);
                        gridView1.Columns["roomStatus"].AppearanceCell.BackColor = color;
                    }
                    else if (colorCode == CNETConstantes.status_pickup)
                    {
                        Color color = StatusColor(CNETConstantes.PICKUP);
                        gridView1.Columns["roomStatus"].AppearanceCell.BackColor = color;
                    }
                }
            }
            catch (Exception col)
            {

            }
        }

        private void bindRoomTypes()
        {
            List<RoomTypeDTO> rmType = new List<RoomTypeDTO>();

            if (pseuodoRooms != null && pseuodoRooms.Count > 0)
            {
                foreach (RoomTypeDTO rt in pseuodoRooms)
                {
                    RoomTypeList.RemoveAll(b => b.Description == rt.Description);
                }
                rmType.AddRange(RoomTypeList);
            }
            else
            {
                rmType.AddRange(RoomTypeList);
            }
            GridColumn col = rmTypeCombo1.View.Columns.AddField("Description");
            col.Visible = true;

            rmTypeCombo1.DisplayMember = "Description";
            rmTypeCombo1.ValueMember = "Description";
            rmTypeCombo1.DataSource = rmType;

        }
        List<VwRoomManagmentViewDTO> roomNums { get; set; }

        private void bindRoomNumbers()
        {
            romn.View.Columns.Clear();
            roomNums = new List<VwRoomManagmentViewDTO>();
            List<int> RoomTypecodelist = new List<int>();

            List<VwRoomManagmentViewDTO> fakeRooms = UIProcessManager.GetAllRoomManagment(SelectedHotelcode).ToList();

            if (fakeRooms != null)
                RoomTypecodelist = RoomTypeList.Select(x => x.Id).ToList();

            if (pseuodoRooms.Count > 0)
            {
                foreach (RoomTypeDTO rt in pseuodoRooms)
                {
                    fakeRooms.RemoveAll(b => b.RoomTypeId == rt.Id);
                }
                if (fakeRooms != null)
                    fakeRooms = fakeRooms.Where(x => RoomTypecodelist.Contains(x.RoomTypeId)).ToList();
                if (fakeRooms != null)
                    roomNums.AddRange(fakeRooms);
            }
            else
            {
                if (fakeRooms != null)
                    fakeRooms = fakeRooms.Where(x => RoomTypecodelist.Contains(x.RoomTypeId)).ToList();
                roomNums.AddRange(fakeRooms);
            }
            GridColumn col1 = romn.View.Columns.AddField("roomNo");
            GridColumn col2 = romn.View.Columns.AddField("RoomType");
            col1.Visible = true;
            col2.Visible = true;

            romn.DataSource = roomNums;
            romn.ValueMember = "roomNo";
            romn.DisplayMember = "roomNo";

            Fromroomn.DataSource = roomNums;
            Fromroomn.ValueMember = "roomDetailCode";
            Fromroomn.DisplayMember = "roomNo";

            Toroomn.DataSource = roomNums;
            Toroomn.ValueMember = "roomDetailCode";
            Toroomn.DisplayMember = "roomNo";
        }

        private List<BindRoomMgmtVM> SelectOddEven(string param, List<BindRoomMgmtVM> holdSelectedValue)
        {
            //List<VwRoomManagmentViewDTO> dataSource = new List<VwRoomManagmentViewDTO>();
            List<BindRoomMgmtVM> retList = new List<BindRoomMgmtVM>();

            if (param.Equals("Odd"))
            {
                foreach (BindRoomMgmtVM br in holdSelectedValue)
                {
                    try
                    {
                        string temprmNo = br.roomNo;
                        string intrmn = string.Join(string.Empty, Regex.Matches(temprmNo, @"\d+")
                            .OfType<Match>().Select(m => m.Value));

                        int isodd = Int32.Parse(intrmn);
                        if (isodd % 2 != 0)
                        {
                            retList.Add(br);
                        }
                    }
                    catch (Exception ex) { }
                }

            }
            if (param.Equals("Even"))
            {
                foreach (BindRoomMgmtVM br in holdSelectedValue)
                {
                    try
                    {
                        string temprmNo = br.roomNo;
                        string intrmn = string.Join(string.Empty, Regex.Matches(temprmNo, @"\d+")
                            .OfType<Match>().Select(m => m.Value));

                        int isodd = Int32.Parse(intrmn);
                        if (isodd % 2 == 0)
                        {
                            retList.Add(br);

                        }
                    }
                    catch (Exception ex) { }
                }
            }

            return retList;
        }
        List<ActivityDefinitionDTO> activityDefinitionsList = UIProcessManager.SelectAllActivityDefinition();
        public int? GetActivityCode(int lookup)
        {
            //List<ActivityDefinitionDTO> listAD = UIProcessManager.GetActivityDefinitionByDescription(lookup);
            ActivityDefinitionDTO listAD = activityDefinitionsList.FirstOrDefault(x => x.Description == lookup);
            int adCode;
            if (listAD != null)
            {
                adCode = listAD.Id;
                return adCode;
            }
            else
            {
                return null;
            }
        }
        int? cleanActivity { get; set; }
        int? dirtyActivity { get; set; }
        int? pickupActivity { get; set; }
        int? inspectedActivity { get; set; }
        int? oooActivity { get; set; }
        int? oosActivity { get; set; }


        public void CheckandCreateHouseKeppingActivity()
        {

            cleanActivity = GetActivityCode(CNETConstantes.CLEAN);

            if (cleanActivity == null)
                cleanActivity = CreateActivityDef(CNETConstantes.CLEAN, CNETConstantes.OS_CLEAN);


            dirtyActivity = GetActivityCode(CNETConstantes.Dirty);


            if (dirtyActivity == null)
                dirtyActivity = CreateActivityDef(CNETConstantes.Dirty, CNETConstantes.OS_Dirty);

            pickupActivity = GetActivityCode(CNETConstantes.PICKUP);

            if (pickupActivity == null)
                pickupActivity = CreateActivityDef(CNETConstantes.PICKUP, CNETConstantes.OS_PICKUP);

            inspectedActivity = GetActivityCode(CNETConstantes.INSPECTED);

            if (inspectedActivity == null)
                inspectedActivity = CreateActivityDef(CNETConstantes.INSPECTED, CNETConstantes.OS_INSPECTED);

            oooActivity = GetActivityCode(CNETConstantes.OOO);

            if (oooActivity == null)
                oooActivity = CreateActivityDef(CNETConstantes.OOO, CNETConstantes.OS_OOO);

            oosActivity = GetActivityCode(CNETConstantes.OOS);

            if (oosActivity == null)
                oosActivity = CreateActivityDef(CNETConstantes.OOS, CNETConstantes.OS_OOS);


        }

        private int? CreateActivityDef(int descripton, int state)
        {
            int? newcode = null;
            ActivityDefinitionDTO NewActivityDefinitionDTO = new ActivityDefinitionDTO()
            {
                Reference = CNETConstantes.REGISTRATION_VOUCHER,
                Description = descripton,
                State = state,
                Index = 0,
                NeedsPassCode = false,
                IssuingEffect = false,
                IsManual = false,
                RequiredTime = 0,
                MaxRepeat = 1,
                Sequence = false,
                IsPrint = false
            };
            ActivityDefinitionDTO SaveActivityDefinition = UIProcessManager.CreateActivityDefinition(NewActivityDefinitionDTO);
            return newcode;
        }

        private void Save_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime? date = UIProcessManager.GetServiceTime();
            if (date != null)
            {
                CurrentTime = date.Value;
            }
            else
            {
                XtraMessageBox.Show("Server Date Time Error ", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int rows = changedRows.Count;
            if (rows == 0)
            {
                XtraMessageBox.Show("Change Room Status First.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            for (int i = 0; i < rows; i++)
            {

                int j = repositoryItemComboBox3.Items.IndexOf(gridView1.GetRowCellDisplayText(changedRows[i], gridView1.Columns[4]));

                int rodCode = Convert.ToInt32(gridView1.GetRowCellDisplayText(changedRows[i], gridView1.Columns[8]));
                RoomDetailDTO rmd = UIProcessManager.GetRoomDetailById(rodCode);
                switch (j)
                {
                    case 1:

                        //rmd.lastState = CNETConstantes.CLEAN;
                        // int? clean = GetActivityCode(CNETConstantes.CLEAN);
                        rmd.LastState = cleanActivity;
                        break;
                    case 2:
                        //rmd.lastState = CNETConstantes.Dirty;
                        //int? dirty = GetActivityCode(CNETConstantes.Dirty);
                        rmd.LastState = dirtyActivity;
                        break;
                    case 5:
                        //int? pickup = GetActivityCode(CNETConstantes.PICKUP);
                        rmd.LastState = pickupActivity;
                        //rmd.lastState = CNETConstantes.PICKUP;
                        break;
                    case 0:
                        //int? inspected = GetActivityCode(CNETConstantes.INSPECTED);
                        rmd.LastState = inspectedActivity;
                        //rmd.lastState = CNETConstantes.OOO;
                        break;
                    case 3:
                        //int? ooo = GetActivityCode(CNETConstantes.OOO);
                        rmd.LastState = oooActivity;
                        //rmd.lastState = CNETConstantes.OOS;
                        break;
                    case 4:
                        //int? oos = GetActivityCode(CNETConstantes.OOS);
                        rmd.LastState = oosActivity;
                        //rmd.lastState = CNETConstantes.INSPECTED;
                        break;
                }
                UIProcessManager.UpdateRoomDetail(rmd);
                logActiviy(rmd, "House Keeping");
            }
            XtraMessageBox.Show("Room Status Saved!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            changedRows.Clear();
            RefreshOnSave();
            btnCheckAll.Checked = true;
        }

        private void logActiviy(RoomDetailDTO rmd, string remark)
        {
            try
            {
                ActivityDTO act = new ActivityDTO();
                act.ActivityDefinition = rmd.LastState.Value;
                act.TimeStamp = CurrentTime.ToLocalTime();
                act.Year = CurrentTime.Year;
                act.Month = CurrentTime.Month;
                act.Day = CurrentTime.Day;
                act.Reference = rmd.Id;
                act.User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                act.Pointer = CNETConstantes.HouseKeeping_Mgt;
                act.Platform = "1";
                //string deviceName = System.Windows.Forms.SystemInformation.ComputerName;
                //string articlecode = UIProcessManager.getArticleCode(deviceName);
                //string device = UIProcessManager.getDeviceByArticle(articlecode);
                act.Device = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                act.ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                act.Remark = "House Keeping";
                UIProcessManager.CreateActivity(act);
            }
            catch (Exception e) { }
        }

        private void Refresh_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Progress_Reporter.Show_Progress("Refreshing Data ...", "Please Wait");
            foreach (BarCheckItem ch in allchecks)
            {
                if (ch.Checked)
                {
                    ch.Checked = false;
                }
            }
            if (barEditItem4.EditValue != null)
            {
                barEditItem4.EditValue = oddEvenList[2];
            }
            if (romnSearchGrid.EditValue != null)
            {
                romnSearchGrid.EditValue = null;
            }
            gridControl1.DataSource = null;
            holdValue.Clear();
            changedRows.Clear();

            PopulateInitialGrid();
            ShowActivity();
            bindRoomNumbers();
            ////CNETInfoReporter.Hide();
        }

        private void RoomType_Combo_ValueChanged(object sender, EventArgs e)
        {
            FromromnSearchGrid.EditValue = null;
            ToromnSearchGrid.EditValue = null;
            string selectedText = "";
            if (rmTypSrchGrd.EditValue == null)
            {
                selectedText = "";
                return;
            }
            else
            {
                selectedText = rmTypSrchGrd.EditValue.ToString();
            }
            rmType = selectedText;
            checkBoxers();
        }

        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            string selectedText = "";
            if (romnSearchGrid.EditValue == null)
            {
                rmNum = "";
                selectedText = "";
                gridControl1.DataSource = holdValue;
                return;
            }
            else
            {
                FromromnSearchGrid.EditValue = null;
                ToromnSearchGrid.EditValue = null;
                selectedText = romnSearchGrid.EditValue.ToString();
            }
            rmNum = selectedText;
            checkBoxers();
        }

        private void RefreshOnSave()
        {
            foreach (BarCheckItem ch in allchecks)
            {
                if (ch.Checked)
                {
                    ch.Checked = false;
                }
            }
            if (barEditItem4.EditValue != null)
            {
                barEditItem4.EditValue = oddEvenList[2];
            }
            if (romnSearchGrid.EditValue != null)
            {
                romnSearchGrid.EditValue = null;
            }
            FromromnSearchGrid.EditValue = null;
            ToromnSearchGrid.EditValue = null;

            gridControl1.DataSource = null;
            holdValue.Clear();
            PopulateInitialGrid();
            StatusNumber();
        }

        private void StatusNumber()
        {
            count();
            cln1.Caption = "Clean";
            drt1.Caption = "Dirty";
            pkcp.Caption = "Pickup";
            insp1.Caption = "Inspected";
            oos.Caption = "Out Of Service";
            ooo.Caption = "Out Of Order";

            cln1.Caption += "(" + numOfCleans + ")";
            drt1.Caption += "(" + numOfDirties + ")";
            ooo.Caption += "(" + numOfOOOs + ")";
            oos.Caption += "(" + numOfOOSs + ")";
            pkcp.Caption += "(" + numOfPickups + ")";
            insp1.Caption += "(" + numOfInsps + ")";
        }

        private void checkBoxers()
        {

            List<BindRoomMgmtVM> members = new List<BindRoomMgmtVM>();
            foreach (BindRoomMgmtVM hold in holdValue)
            {
                members.Add(hold);
            }
            if (members.Count > 0)
            {
                List<BindRoomMgmtVM> result = handleCheckBox1(checkBoxGroup1, 1, members);
                if (result != null)
                {
                    members = result;
                }

                List<BindRoomMgmtVM> result2 = handleCheckBox1(checkBoxGroup2, 2, members);
                if (result2 != null)
                {
                    members = result2;
                }

                List<BindRoomMgmtVM> result3 = handleCheckBox1(checkBoxGroup3, 3, members);
                if (result3 != null)
                {
                    members = result3;
                }

                List<BindRoomMgmtVM> result4 = handleCheckBox1(checkBoxGroup4, 4, members);
                if (result4 != null)
                {
                    members = result4;
                }

                List<BindRoomMgmtVM> result5 = members.Where(l => l.roomType == rmType).ToList();
                if (result5.Count > 0)
                {
                    members = result5;
                }
                List<BindRoomMgmtVM> result6 = members.Where(l => l.roomNo == rmNum).ToList();
                if (result6.Count > 0)
                {
                    members = result6;
                }

                List<BindRoomMgmtVM> result7 = members.Where(l => l.floor == floorSrch).ToList();
                if (result7.Count > 0)
                {
                    members = result7;
                }

                List<BindRoomMgmtVM> result8 = new List<BindRoomMgmtVM>();
                if (FromromnSearchGrid.EditValue != null && ToromnSearchGrid.EditValue != null)
                {
                    VwRoomManagmentViewDTO Fromroom = roomNums.FirstOrDefault(x => x.roomDetailCode == Convert.ToInt32(FromromnSearchGrid.EditValue));
                    VwRoomManagmentViewDTO Toroom = roomNums.FirstOrDefault(x => x.roomDetailCode == Convert.ToInt32(ToromnSearchGrid.EditValue));

                    int startindex = roomNums.IndexOf(Fromroom);
                    int endindex = roomNums.IndexOf(Toroom);

                    if (endindex >= startindex)
                    {
                        List<VwRoomManagmentViewDTO> Filteredroom = roomNums.Where(x => roomNums.IndexOf(x) >= startindex && roomNums.IndexOf(x) <= endindex).ToList();
                        List<string> roomlist = Filteredroom.Select(x => x.roomNo).ToList(); ;
                        result8 = members.Where(l => roomlist.Contains(l.roomNo)).ToList();
                        if (result8.Count > 0)
                        {
                            members = result8;
                        }
                    }
                    else
                    {

                        XtraMessageBox.Show("Please Select Proper Range !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }

                gridControl1.DataSource = members;
            }
        }

        private List<BindRoomMgmtVM> handleCheckBox1(List<BarCheckItem> checkBox, int type, List<BindRoomMgmtVM> mng)
        {
            List<BindRoomMgmtVM> member = new List<BindRoomMgmtVM>();
            bool check = false;
            String checker = "";


            foreach (BarCheckItem cb in checkBox)
            {
                if (cb.Checked)
                {
                    check = true;
                    break;
                }
            }
            if (check)
            {
                foreach (BarCheckItem cb in checkBox)
                {
                    if (cb.Checked)
                    {
                        switch (type)
                        {
                            case 1:
                                member.AddRange(mng.Where(x => x.roomStatus == cb.AccessibleName).ToList());
                                break;
                            case 2:
                                member.AddRange(SelectOddEven(cb.AccessibleName, mng));
                                break;
                            case 3:
                                member.AddRange(mng.Where(x => x.foStatus == cb.AccessibleName).ToList());
                                break;
                            case 4:
                                member.AddRange(mng.Where(x => x.resStatus == cb.AccessibleName).ToList());
                                break;

                        }

                    }
                }
            }
            else
            {
                member = null;
            }
            return member;
        }

        private void cln1_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void oos_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void insp1_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void pkcp_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void ooo_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void oddr_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void evenr_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void vac_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void occ_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void arvls_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void arrvd_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void stovr_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void dout_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void ckout_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void notres_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void drt1_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void DefaultRoomStatus()
        {
            List<RoomDetailDTO> rooms = UIProcessManager.SelectAllRoomDetail().Where(b => b.LastState == null).ToList();
            // int? clean = GetActivityCode(CNETConstantes.CLEAN);
            foreach (RoomDetailDTO rd in rooms)
            {
                rd.LastState = cleanActivity;
                UIProcessManager.UpdateRoomDetail(rd);
                logActiviy(rd, "House keeping Default Room Status");
            }
            int count = rooms.Count;
        }

        private void count()
        {
            numOfCleans = holdValue.Where(b => b.roomStatus == "Clean").Count();
            numOfDirties = holdValue.Where(b => b.roomStatus == "Dirty").Count();
            numOfOOOs = holdValue.Where(b => b.roomStatus == "Out Of Order").Count();
            numOfOOSs = holdValue.Where(b => b.roomStatus == "Out Of Service").Count();
            numOfPickups = holdValue.Where(b => b.roomStatus == "Pickup").Count();
            numOfInsps = holdValue.Where(b => b.roomStatus == "Inspected").Count();
        }

        private void PrintClick(object sender, ItemClickEventArgs e)
        {

            HouseKeepingReport report = new HouseKeepingReport();
            report = new HouseKeepingReport(gridControl1, false, "", "", "Room Managment Report");
            report.Landscape = true;
            ReportPrintTool pt = new ReportPrintTool(report);
            pt.ShowPreview();
        }

        private void CustomizeNavigator()
        {
            ControlNavigator na = gridControl1.EmbeddedNavigator;
            na.Buttons.Remove.Visible = false;
            na.Buttons.EndEdit.Visible = false;
            na.Buttons.Edit.Visible = false;
            na.Buttons.CancelEdit.Visible = false;
            na.Buttons.Append.Visible = false;
        }

        private void barEditItem4_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (barEditItem4.EditValue != null)
                {
                    string text = barEditItem4.EditValue.ToString();
                    if (text.Equals("Odd"))
                    {
                        od.Checked = true;
                        ev.Checked = false;
                        checkBoxers();
                    }
                    else if (text.Equals("Even"))
                    {
                        ev.Checked = true;
                        od.Checked = false;
                        checkBoxers();
                    }
                    else if (text.Equals("All"))
                    {
                        ev.Checked = false;
                        od.Checked = false;
                        checkBoxers();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void barFloor_EditValueChanged(object sender, EventArgs e)
        {
            FromromnSearchGrid.EditValue = null;
            ToromnSearchGrid.EditValue = null;
            string selectedText = "";
            if (barFloor.EditValue == null)
            {
                selectedText = "";
                gridControl1.DataSource = holdValue;
                bindRoomNumbers();
                return;
            }
            else
            {
                rmNum = "";
                selectedText = barFloor.EditValue.ToString();
                romn.View.Columns.Clear();

                GridColumn col1 = romn.View.Columns.AddField("roomNo");
                GridColumn col2 = romn.View.Columns.AddField("roomType");
                col1.Visible = true;
                col2.Visible = true;

                romn.DataSource = null;
                romn.DataSource = holdValue.Where(b => b.floor == selectedText).ToList();
                romn.ValueMember = "roomNo";
                romn.DisplayMember = "roomNo";

                Fromroomn.DataSource = holdValue.Where(b => b.floor == selectedText).ToList();
                Fromroomn.ValueMember = "roomDetailCode";
                Fromroomn.DisplayMember = "roomNo";

                Toroomn.DataSource = holdValue.Where(b => b.floor == selectedText).ToList();
                Toroomn.ValueMember = "roomDetailCode";
                Toroomn.DisplayMember = "roomNo";


            }
            floorSrch = selectedText;
            checkBoxers();
        }

        private void BindFloor()
        {
            List<string> l = UIProcessManager.GetAllRoomManagment(SelectedHotelcode).Where(x => RoomDetailcodeList.Contains(x.roomDetailCode)).Select(b => b.Floor).Distinct().ToList();
            floorCom.DisplayMember = "Floor";
            floorCom.ValueMember = "Floor";
            floorCom.DataSource = l;
        }

        private void frmHouseKeepingMgmt_FormClosed(object sender, FormClosedEventArgs e)
        {
            holdValue = new List<BindRoomMgmtVM>();
            gridControl1.DataSource = holdValue;
            this.Dispose();

        }

        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.Caption == "SN")
            {
                BindRoomMgmtVM dto = view.GetRow(e.RowHandle) as BindRoomMgmtVM;
                if (dto == null) return;
                string sn = (e.RowHandle + 1).ToString();
                dto.SN = sn;
                e.DisplayText = sn;
            }
        }

        private void btnChangeStatusTo_ItemClick(object sender, ItemClickEventArgs e)
        {
            gridView1.PostEditor();
            string Tag = e.Item.Tag.ToString();
            changedRows = new List<int>();
            List<BindRoomMgmtVM> data = (List<BindRoomMgmtVM>)gridControl1.DataSource;
            if (data != null)
            {
                List<BindRoomMgmtVM> checkdata = data.Where(x => x.Check).ToList();
                checkdata.ForEach(x => x.roomStatus = Tag);
                checkdata.ForEach(x => changedRows.Add(data.IndexOf(x)));
            }

            gridControl1.Refresh();
            gridControl1.RefreshDataSource();


        }

        private void btnCheckAll_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            gridView1.PostEditor();
            List<BindRoomMgmtVM> data = (List<BindRoomMgmtVM>)gridControl1.DataSource;
            if (data != null)
            {
                if (btnCheckAll.Checked)
                {
                    data.ForEach(x => x.Check = true);
                }
                else
                {
                    data.ForEach(x => x.Check = false);
                }
            }

            gridControl1.Refresh();
            gridControl1.RefreshDataSource();

        }

        private void ToromnSearchGrid_EditValueChanged(object sender, EventArgs e)
        {
            //romnSearchGrid.EditValue = null;
            if (ToromnSearchGrid.EditValue == null)
            {
                gridControl1.DataSource = holdValue;
                return;
            }
            else
            {
                rmNum = "";
                romnSearchGrid.EditValue = null;
            }
            checkBoxers();
        }

        private void FromromnSearchGrid_EditValueChanged(object sender, EventArgs e)
        {
            if (FromromnSearchGrid.EditValue == null)
            {
                gridControl1.DataSource = holdValue;
                return;
            }
            else
            {
                rmNum = "";
                romnSearchGrid.EditValue = null;
            }
            checkBoxers();
        }

        List<RoomDetailDTO> RoomDetailList;
        List<RoomTypeDTO> RoomTypeList { get; set; }
        List<int> RoomTypeListcode { get; set; }
        List<int> RoomDetailcodeList { get; set; }
        private void beiHotel_EditValueChanged(object sender, EventArgs e)
        {
            SelectedHotelcode = beiHotel.EditValue == null ? null : Convert.ToInt32(beiHotel.EditValue);


            RoomTypeList = UIProcessManager.GetRoomTypeByConsigneeUnit(SelectedHotelcode.Value);
            RoomDetailList = UIProcessManager.SelectAllRoomDetail();
            if (RoomTypeList != null)
            {
                RoomTypeListcode = RoomTypeList.Select(x => x.Id).ToList();
                RoomDetailList = RoomDetailList.Where(y => RoomTypeListcode.Contains(y.RoomType)).ToList();
                if (RoomDetailList != null)
                    RoomDetailcodeList = RoomDetailList.Select(x => x.Id).ToList();
            }


            bindRoomTypes();
            bindRoomNumbers();
            BindFloor();
            PopulateInitialGrid();
            StatusNumber();
        }
        public int? SelectedHotelcode { get; set; }
    }
}