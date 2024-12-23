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

using CNET.ERP.Client.Common.UI;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using CNET.FrontOffice_V._7;

using DevExpress.Utils.Win;
using DevExpress.XtraLayout;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraReports.UI;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.ViewSchema;

namespace CNET.FrontOffice_V._7.HouseKeeping
{
    public partial class frmTurndwnMgmt : UILogicBase
    {
        static DateTime currentDate { get; set; }// = UIProcessManager.GetServiceTime().Value;

        static string time { get; set; }//= currentDate.Date.ToString("yyyy-MM-dd");
        DateTime t { get; set; }//= DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        List<CheckEdit> checkEditNames = new List<CheckEdit>();
        List<VwRegistrationWithRoomStateViewDTO> resultHolder = new List<VwRegistrationWithRoomStateViewDTO>();
        List<TurndownVM> holdvalue = new List<TurndownVM>();

        List<BarCheckItem> checkboxnames = new List<BarCheckItem>();
        List<BarCheckItem> checkBoxGroup1;
        List<BarCheckItem> checkBoxGroup2;

        DateTime CurrentTime { get; set; }

        public frmTurndwnMgmt()
        {
            InitializeComponent();
            //SecurityCheck("Turn Down Managment");

            try
            {
                DateTime? date = UIProcessManager.GetServiceTime();
                if (date != null)
                {
                    CurrentTime = date.Value;
                }
                else
                {
                    XtraMessageBox.Show("Error Getting Server Date Time !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                currentDate = date.Value;

                time = currentDate.Date.ToString("yyyy-MM-dd");
                t = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            checkboxnames.Add(arvd); checkboxnames.Add(arvls); checkboxnames.Add(stovr);
            checkboxnames.Add(required); checkboxnames.Add(notrequired);

            checkBoxGroup1 = new List<BarCheckItem>() { required, notrequired };
            checkBoxGroup2 = new List<BarCheckItem>() { arvd, arvls, stovr };
            CustomizeNavigator();

            repositoryItemComboBox1.EditValueChanged += repositoryItemComboBox1_EditValueChanged;
        }
        private void SecurityCheck(string parent)
        {
            List<String> allowedTabs = MasterPageForm.AllowedFunctionalities(parent);
            int closeedCounter = 0;
            if (!allowedTabs.Contains("Save"))
            {
                bbiSave.Enabled = false;
                closeedCounter++;
            }
            if (!allowedTabs.Contains("Print"))
            {
                bbiPrint.Enabled = false;
                closeedCounter++;
            }
            if (closeedCounter == 2)
            {
                this.Enabled = false;
            }
        }
        List<int> changedRows = new List<int>();
        private void repositoryItemComboBox1_EditValueChanged(object sender, EventArgs e)
        {
            int k = gridView1.FocusedRowHandle;
            changedRows.Add(k);
            gridView1.PostEditor();
        }


        private void close(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        List<RoomTypeDTO> pseuodoRooms = new List<RoomTypeDTO>();
        private void BindRoomNums()
        {
            List<VwRoomManagmentViewDTO> rmNum = new List<VwRoomManagmentViewDTO>();
            List<VwRoomManagmentViewDTO> fakeRooms = UIProcessManager.GetAllRoomManagment(SelectedHotelcode).OrderBy(b => b.roomNo).ToList();
            if (pseuodoRooms.Count > 0)
            {
                foreach (RoomTypeDTO rt in pseuodoRooms)
                {
                    fakeRooms.RemoveAll(b => b.RoomType == rt.Description);
                }

                if (fakeRooms != null)
                    fakeRooms = fakeRooms.Where(x => RoomDetailcodeList.Contains(x.roomDetailCode)).ToList();

                rmNum.AddRange(fakeRooms);
            }
            else
            {
                if (fakeRooms != null)
                    fakeRooms = fakeRooms.Where(x => RoomDetailcodeList.Contains(x.roomDetailCode)).ToList();
                rmNum.AddRange(fakeRooms);
            }

            frmRoom.DataSource = rmNum;
            toRoom.DataSource = rmNum;
        }

        private void BindRoomTypes()
        {
            List<RoomTypeDTO> rmType = new List<RoomTypeDTO>();
            if (pseuodoRooms.Count > 0)
            {
                foreach (RoomTypeDTO rt in pseuodoRooms)
                {
                    RoomTypeList.RemoveAll(b => b.Id == rt.Id);
                }
                rmType.AddRange(RoomTypeList);
            }
            else
            {
                rmType.AddRange(RoomTypeList);
            }
            roomType.DataSource = rmType;
        }

        int firstTime = 0;
        private void frmTurndwnMgmt_Load(object sender, EventArgs e)
        {
            firstTime = 1;
            pseuodoRooms = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
            List<LookupDTO> list = LocalBuffer.LocalBuffer.LookUpBufferList.Where(x => x.Type == CNETConstantes.TurnDown).ToList();
            List<string> comboList = new List<string> { CNETConstantes.TD_Required, CNETConstantes.TD_NotRequired };

            repositoryItemComboBox1.Items.AddRange(comboList);

            reiHotel.DisplayMember = "Name";
            reiHotel.ValueMember = "Id";
            reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

            beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
            reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;

            roomType.Popup += new EventHandler(Combos_Popup);
            frmRoom.Popup += new EventHandler(Combos_Popup);
            toRoom.Popup += new EventHandler(Combos_Popup);

            turnDownDate.EditValue = DateTime.Now;

            GridColumn col = frmRoom.View.Columns.AddField("roomNo");
            col.Visible = true;

            frmRoom.DisplayMember = "roomNo";
            frmRoom.ValueMember = "roomDetailCode";
            //frmRoom.DataSource = rmNum;


            GridColumn col1 = toRoom.View.Columns.AddField("roomNo");
            col1.Visible = true;
            toRoom.DisplayMember = "roomNo";
            toRoom.ValueMember = "roomDetailCode";
            //toRoom.DataSource = rmNum;


            GridColumn col2 = roomType.View.Columns.AddField("description");
            col2.Visible = true;

            roomType.DisplayMember = "description";
            roomType.ValueMember = "description";

        }
        public int? GetActivityCode(int lookup)
        {
            ActivityDefinitionDTO listAD = LocalBuffer.LocalBuffer.ActivityDefinitionBufferList.FirstOrDefault(b => b.Description == lookup);

            if (listAD != null)
            {
                return listAD.Id;
            }
            else
            {
                return null;
            }
        }

        private Color StatusColor(int lookup, List<VwRoomManagmentViewDTO> mgmtList)
        {
            int? objectState = GetActivityCode(lookup);
            VwRoomManagmentViewDTO mgmt = mgmtList.FirstOrDefault(r => r.roomStatusCode == objectState);
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

        private void SetColorCode(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    List<VwRoomManagmentViewDTO> mgmtList = UIProcessManager.GetAllRoomManagment(SelectedHotelcode);
                    string colorCode = View.GetRowCellDisplayText(e.RowHandle, View.Columns["roomstatus"]);

                    if (colorCode == "")
                    {
                        if (e.RowHandle % 2 == 0)
                            gridView1.Columns["roomstatus"].AppearanceCell.BackColor = Color.White;
                        else
                            gridView1.Columns["roomstatus"].AppearanceCell.BackColor = Color.AliceBlue;
                    }
                    else if (colorCode == CNETConstantes.status_oos)
                    {

                        Color color = StatusColor(CNETConstantes.OOS, mgmtList);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;

                    }
                    else if (colorCode == CNETConstantes.status_clean)
                    {
                        Color color = StatusColor(CNETConstantes.CLEAN, mgmtList);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;

                    }
                    else if (colorCode == CNETConstantes.status_dirty)
                    {
                        Color color = StatusColor(CNETConstantes.Dirty, mgmtList);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;

                    }
                    else if (colorCode == CNETConstantes.status_inspected)
                    {
                        Color color = StatusColor(CNETConstantes.INSPECTED, mgmtList);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;

                    }
                    else if (colorCode == CNETConstantes.status_ooo)
                    {
                        Color color = StatusColor(CNETConstantes.OOO, mgmtList);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;

                    }
                    else if (colorCode == CNETConstantes.status_pickup)
                    {
                        Color color = StatusColor(CNETConstantes.PICKUP, mgmtList);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;

                    }
                }
            }
            catch (Exception col)
            {

            }
        }

        private void PopulateGrid()
        {
            int? ooo = GetActivityCode(CNETConstantes.OOO);
            int? oos = GetActivityCode(CNETConstantes.OOS);

            resultHolder.Clear();
            List<RoomTypeDTO> suRooms = UIProcessManager.GetRoomTypeByispseudoRoomType(true);

            //VwRegistrationDocumentViewDTO

            var unfillteredRooms = UIProcessManager.GetRegistrationWithRoomStatus(t, CNETConstantes.CHECKED_IN_STATE, SelectedHotelcode.Value)
                .Where(b => b.HKState != ooo || b.HKState != oos && b.arrivalDate == DateTime.Now.Date)
                .GroupBy(b => b.room)
                .Select(b => b.FirstOrDefault()).ToList();

            var unfillteredRooms2 = UIProcessManager.GetRegistrationWithRoomStatus(t, CNETConstantes.SIX_PM_STATE, SelectedHotelcode.Value)
             .Where(b => b.HKState != ooo || b.HKState != oos && b.arrivalDate == DateTime.Now.Date)
             .GroupBy(b => b.room)
             .Select(b => b.FirstOrDefault()).ToList();

            var unfillteredRooms3 = UIProcessManager.GetRegistrationWithRoomStatus(t, CNETConstantes.GAURANTED_STATE, SelectedHotelcode.Value)
             .Where(b => b.HKState != ooo || b.HKState != oos && b.arrivalDate == DateTime.Now.Date)
             .GroupBy(b => b.room)
             .Select(b => b.FirstOrDefault()).ToList();

            var unfillteredRooms4 = UIProcessManager.GetRegistrationWithRoomStatus(t, CNETConstantes.OSD_WAITLIST_STATE, SelectedHotelcode.Value)
           .Where(b => b.HKState != ooo || b.HKState != oos && b.arrivalDate == DateTime.Now.Date)
           .GroupBy(b => b.room)
           .Select(b => b.FirstOrDefault()).ToList();

            unfillteredRooms.AddRange(unfillteredRooms2);
            unfillteredRooms.AddRange(unfillteredRooms3);
            unfillteredRooms.AddRange(unfillteredRooms4);

            if (suRooms.Count > 0)
            {
                foreach (RoomTypeDTO rt in suRooms)
                {
                    unfillteredRooms.RemoveAll(b => b.Id == rt.Id);
                }
                resultHolder.AddRange(unfillteredRooms);
            }
            else
            {
                resultHolder.AddRange(unfillteredRooms);
            }
            resultHolder = resultHolder.Where(x => RoomDetailcodeList.Contains(x.RoomDetailcode.Value)).ToList();
            foreach (VwRegistrationWithRoomStateViewDTO result in resultHolder)
            {
                TurndownVM trndn = new TurndownVM();
                trndn.roomcode = result.room;
                TurndownDTO up = UIProcessManager.SelectAllTurndown().Where(b => b.RoomDetail == trndn.roomcode & b.TimeStamp.Date == currentDate.Date).FirstOrDefault();
                if (up != null)
                {
                    bool val = Convert.ToBoolean(up.Value);
                    if (val == false) { trndn.turndn = CNETConstantes.TD_Required; }
                    else if (val == true) { trndn.turndn = CNETConstantes.TD_NotRequired; }
                    trndn.code = up.Id;
                }
                else
                {
                    trndn.turndn = CNETConstantes.TD_Required;
                }
                trndn.roomno = result.RoomNumber;
                trndn.roomtype = result.RoomTypeDescription;

                HkassignmentDTO h = UIProcessManager.SelectAllHKAssignment().Where(b => b.RoomDetail == trndn.roomcode & b.Date == currentDate.Date).FirstOrDefault();
                if (h == null)
                {
                    trndn.empname = "";
                }
                else
                {
                    string name = "";
                    ConsigneeDTO p = UIProcessManager.GetConsigneeById(h.Employee.Value);
                    name += p.FirstName + " ";
                    name += p.ThirdName;
                    trndn.empname = name;
                }

                RegistrationStatusDTO resstatus = UIProcessManager.GetRegistrationStatus(result.room.Value, t);
                //trndn.resstatus = (rsstatus.registrationStatus != null |resstatus.registrationStatus != null) ? resstatus.registrationStatus : "Not Reserved";
                trndn.resstatus = resstatus.registrationStatus;
                trndn.roomcode = result.room;

                VwRoomManagmentViewDTO roomstatus = UIProcessManager.GetAllRoomManagment(SelectedHotelcode).Where(r => r.roomDetailCode == result.room).ToList().FirstOrDefault();
                if (roomstatus == null)
                {
                    trndn.roomstatus = "";
                }
                else
                {
                    trndn.roomstatus = roomstatus.rmstatus;
                }
                holdvalue.Add(trndn);
            }
            gridControl1.DataSource = holdvalue;
        }

        private void ToRoom_ValueChanged(object sender, EventArgs e)
        {
            string frmrm = "";
            string trm = "";
            if (fromRoom.EditValue == null)
            {
                frmrm = "";
            }
            else if (fromRoom.EditValue != null)
            {
                frmrm = fromRoom.Edit.GetDisplayText(fromRoom.EditValue);
            }
            if (troom.EditValue == null)
            {
                trm = "";
            }
            else if (troom.EditValue != null)
            {
                trm = troom.Edit.GetDisplayText(troom.EditValue);
            }
            List<TurndownVM> range = new List<TurndownVM>();
            range = holdvalue.Where(b => b.roomno.CompareTo(frmrm) >= 0 && b.roomno.CompareTo(trm) <= 0).ToList();
            gridControl1.DataSource = range;

        }

        private void TurnDownSave(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int rows = changedRows.Count();
            if (firstTime > 1 && changedRows.Count() == 0)
            {
                XtraMessageBox.Show("Change TurnDown Status First", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < rows; i++)
            {
                int j = repositoryItemComboBox1.Items.IndexOf(gridView1.GetRowCellDisplayText(changedRows[i], gridView1.Columns[5]));
                if (j == -1) { continue; }
                int roomCode = Convert.ToInt32(gridView1.GetRowCellDisplayText(changedRows[i], gridView1.Columns[7]));
                TurndownDTO trn = new TurndownDTO();
                trn.RoomDetail = roomCode;
                trn.TimeStamp = currentDate;
                if (j == 0)
                {
                    trn.Value = 0;
                }
                else
                {
                    trn.Value = 1;
                }
                TurndownDTO up = UIProcessManager.SelectAllTurndown().Where(b => b.RoomDetail == roomCode & b.TimeStamp.Date == currentDate.Date).FirstOrDefault();
                if (up != null)
                {
                    up.Value = trn.Value;
                    UIProcessManager.UpdateTurndown(up);
                }
                else
                {
                    UIProcessManager.CreateTurndown(trn);
                }


            }
            XtraMessageBox.Show("Turndown Status Successfully Saved!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            firstTime += 1;
            logActiviy();

        }
        private void logActiviy()
        {
            try
            {
                ActivityDTO act = new ActivityDTO();
                act.ActivityDefinition = GetActivityCode(CNETConstantes.CLEAN).Value;
                act.TimeStamp = CurrentTime.ToLocalTime();
                act.Year = CurrentTime.Year;
                act.Month = CurrentTime.Month;
                act.Day = CurrentTime.Day;
                act.Reference = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                act.User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                act.Pointer = CNETConstantes.HouseKeeping_Mgt;
                act.Device = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                act.Platform = "1";
                act.ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                act.Remark = "TURN DOWN SAVED";
                UIProcessManager.CreateActivity(act);
            }
            catch (Exception ex) { }
        }
        private void RoomTypeValueChanged(object sender, EventArgs e)
        {
            string selectedText = "";
            if (rmType.EditValue == null)
            {
                selectedText = "";
                gridControl1.DataSource = holdvalue;
                return;
            }
            else
            {
                selectedText = rmType.Edit.GetDisplayText(rmType.EditValue);
            }

            List<TurndownVM> holdSelectedValue = new List<TurndownVM>();
            List<TurndownVM> filterHoldSelectedValue = new List<TurndownVM>();
            List<TurndownVM> vacantlist = holdvalue as List<TurndownVM>;


            holdSelectedValue = vacantlist.Where(r => r.roomtype == selectedText).ToList();
            foreach (BarCheckItem ch in checkboxnames)
            {

                if (ch.Checked)
                {
                    List<TurndownVM> rm = vacantlist.Where(x => x.resstatus == ch.AccessibleName & x.roomtype == selectedText).ToList();
                    filterHoldSelectedValue.AddRange(rm);

                }
            }
            if (selectedText == "") { gridControl1.DataSource = holdvalue; }
            else if (filterHoldSelectedValue.Count == 0) { gridControl1.DataSource = holdSelectedValue; }
            else if (filterHoldSelectedValue.Count != 0) { gridControl1.DataSource = filterHoldSelectedValue; }
        }

        private void Combos_Popup(object sender, EventArgs e)
        {
            IPopupControl popupControl = sender as IPopupControl;
            //popupControl.PopupWindow.Size = new System.Drawing.Size(200,300);

            LayoutControl layoutControl = popupControl.PopupWindow.Controls[2].Controls[0] as LayoutControl;
            SimpleButton clearButton = ((LayoutControlItem)layoutControl.Items.FindByName("lciClear")).Control as SimpleButton;
            if (clearButton != null)
            {

                clearButton.Text = "All";
                clearButton.Click += new EventHandler(clearButton_Click);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = null;
            gridControl1.DataSource = holdvalue;

        }

        private void Refresh(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (BarCheckItem ch in checkboxnames)
            {
                ch.Checked = false;
            }
            fromRoom.EditValue = null;
            troom.EditValue = null;
            rmType.EditValue = null;
            gridControl1.DataSource = null;
            holdvalue.Clear();
            PopulateGrid();

        }

        private void required_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void notrequired_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void arvls_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void arvd_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void stovr_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            checkBoxers();
        }

        private void checkBoxers()
        {

            List<TurndownVM> members = new List<TurndownVM>();
            foreach (TurndownVM hold in holdvalue)
            {
                members.Add(hold);
            }
            if (members.Count > 0)
            {
                List<TurndownVM> result = handleCheckBox1(checkBoxGroup1, 1, members);
                if (result != null)
                {
                    members = result;
                }

                List<TurndownVM> result2 = handleCheckBox1(checkBoxGroup2, 2, members);
                if (result2 != null)
                {
                    members = result2;
                }
                gridControl1.DataSource = members;
            }
        }

        private List<TurndownVM> handleCheckBox1(List<BarCheckItem> checkBox, int type, List<TurndownVM> mng)
        {
            List<TurndownVM> member = new List<TurndownVM>();
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
                                member.AddRange(mng.Where(x => x.turndn == cb.AccessibleName).ToList());
                                break;
                            case 2:
                                member.AddRange(mng.Where(x => x.resstatus == cb.AccessibleName).ToList());
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

        private void bbiPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            //ShowPrintPreview(gridControl1);
            HouseKeepingReport report = new HouseKeepingReport();
            report = new HouseKeepingReport(gridControl1, false, "", "", "Turn Down Report");

            ReportPrintTool pt = new ReportPrintTool(report);
            pt.ShowPreview();
        }

        private void ShowPrintPreview(GridControl grid)
        {

            if (!grid.IsPrintingAvailable)
            {
                MessageBox.Show("Printing library not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            grid.ShowPrintPreview();
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

            BindRoomNums();
            BindRoomTypes();

            fromRoom.EditValue = null;
            troom.EditValue = null;
            rmType.EditValue = null;
            gridControl1.DataSource = null;
            holdvalue.Clear();
            PopulateGrid();
        }

        public int? SelectedHotelcode { get; set; }
    }

}