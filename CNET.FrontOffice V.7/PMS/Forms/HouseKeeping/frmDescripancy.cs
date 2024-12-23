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
using CNET.FrontOffice_V._7;
using DevExpress.XtraGrid.Columns;

using DevExpress.Utils.Win;
using DevExpress.XtraLayout;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraGrid;
using DevExpress.XtraReports.UI;
using ProcessManager;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraRichEdit.Import.Doc;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.Progress.Reporter;

namespace CNET.FrontOffice_V._7.HouseKeeping
{
    public partial class frmDescripancy : UILogicBase
    {
        public string _WindowFlag = "";
        List<BarCheckItem> checkBoxGroup1;

        //List<vw_discrepancy> list = new List<vw_discrepancy>();
        static DateTime currentDate { get; set; }// = UIProcessManager.GetServiceTime().Value;

        static string time { get; set; }  //= currentDate.Date.ToString("yyyy-MM-dd");
        DateTime t { get; set; }  //= DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        List<DiscrepancyVM> holdValuee = new List<DiscrepancyVM>();
        List<DiscrepancyVM> currentDateDiscrepancy = new List<DiscrepancyVM>();
        List<CheckEdit> checkEditNames = new List<CheckEdit>();
        List<CheckBox> checkboxnames = new List<CheckBox>();
        GridColumn col = new GridColumn();
        List<BarCheckItem> barcheckboxnames = new List<BarCheckItem>();
        private List<DiscrepancyVM> currcurrentDateDiscrepancy;
        List<RoomTypeDTO> pseuodoRooms = new List<RoomTypeDTO>();
        DateTime CurrentTime { get; set; }
        const string HK_ACTIVITY_DISC = "HK_ACTIVITY_DISC";


        public frmDescripancy()
        {
            InitializeComponent();
            //SecurityCheck("Descripancy");
            try
            {
                DateTime? date = UIProcessManager.GetServiceTime();
                if (date != null)
                {
                    CurrentTime = date.Value;
                }
                else
                {
                    XtraMessageBox.Show("Server Date Time Error", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                currentDate = CurrentTime;

                time = currentDate.Date.ToString("yyyy-MM-dd");
                t = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            checkBoxGroup1 = new List<BarCheckItem>() { sleep1, skip1, person1, dueout1, slPer1, skPer1 };

            CustomizeNavigator();
        }

        private void filter()
        {
            List<DiscrepancyVM> members = new List<DiscrepancyVM>();
            foreach (DiscrepancyVM ddto in holdValuee)
            {
                members.Add(ddto);
            }
            List<DiscrepancyVM> result = HandleDiscrepancyChecks(checkBoxGroup1, 1, members);
            if (result != null)
            {
                members = result;
            }

            gridControl1.DataSource = members;
        }

        private List<DiscrepancyVM> HandleDiscrepancyChecks(List<BarCheckItem> checkBoxs, int type, List<DiscrepancyVM> mng)
        {
            List<DiscrepancyVM> member = new List<DiscrepancyVM>();
            bool check = false;
            String checker = "";

            foreach (BarCheckItem cb in checkBoxs)
            {
                if (cb.Checked)
                {
                    check = true;
                    break;
                }
            }


            if (check)
            {
                foreach (BarCheckItem cb in checkBoxs)
                {
                    if (cb.Checked)
                    {
                        switch (type)
                        {
                            case 1:
                                if (dueout1.Checked)
                                {
                                    member.AddRange(mng.Where(x => x.resstatus == cb.AccessibleName).ToList());
                                }
                                member.AddRange(mng.Where(x => x.discrepancy == Convert.ToInt32(cb.AccessibleName)).ToList());

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

        private void SecurityCheck(string parent)
        {
            //List<String> allowedTabs = MasterPageForm.AllowedFunctionalities(parent);
            //int closeedCounter = 0;
            //if (!allowedTabs.Contains("New"))
            //{
            //    bbiSave.Enabled = false;
            //    closeedCounter++;
            //}
            //if (!allowedTabs.Contains("Print"))
            //{
            //    bbiPrint.Enabled = false;
            //    closeedCounter++;
            //}
            //if (closeedCounter == 2)
            //{
            //    this.Enabled = false;
            //}
        }

        private void ShowActivity()
        {

            List<VwActivityDetailViewDTO> hkActivity = new List<VwActivityDetailViewDTO>();
            hkActivity = UIProcessManager.GetActivityDetailView(null, currentDate, null).Where(b => b.Remark == HK_ACTIVITY_DISC).OrderByDescending(b => b.TimeStamp).ToList();
            grdDiscActivity.DataSource = hkActivity;

        }

        private void PopulateGrid(string dateTime)
        {
            holdValuee.Clear();
            List<DiscrepancyDTO> l = new List<DiscrepancyDTO>();
            l = UIProcessManager.SelectAllDiscrepancy().Where(b => Convert.ToDateTime(b.Date).ToShortDateString() == dateTime).ToList();
            List<VwRoomManagmentViewDTO> RoomManagementViewList = UIProcessManager.GetAllRoomManagment(SelectedHotelcode);
            foreach (DiscrepancyDTO d in l)
            {
                DiscrepancyVM ddto = new DiscrepancyVM();
                int roomcode = d.RoomDetail.Value;
                VwRoomManagmentViewDTO rr = RoomManagementViewList.FirstOrDefault(r => r.roomDetailCode == roomcode);
                if (RoomDetailcodeList.Contains(rr.roomDetailCode))
                {
                    ddto.roomno = rr.roomNo;
                    ddto.roomtype = rr.RoomType;
                    ddto.roomstatus = rr.rmstatus;
                    LookupDTO look = LocalBuffer.LocalBuffer.LookUpBufferList.FirstOrDefault(x => x.Id == d.HkValue);
                    ddto.hkstatus = look.Description;
                    RegistrationStatusDTO foandresStatus = UIProcessManager.GetRegistrationStatus(roomcode, t);
                    string fstatus = foandresStatus.FOStatus;
                    if (fstatus == null & foandresStatus.registrationStatus == null) 
                    { 
                        ddto.fostatus = "";
                        ddto.resstatus = ""; 
                    }
                    else
                    {

                        if (fstatus.Equals("1"))
                        { 
                            ddto.fostatus = "Ocuppied";
                        }
                        else if (fstatus.Equals("0"))
                        {
                            ddto.fostatus = "Vacant";
                        }
                        ddto.resstatus = foandresStatus.registrationStatus;
                    }
                    ddto.hkperson = d.FoValue;
                    ddto.discrepancy = d.DiscrepancyType;
                    ddto.roomdetail = d.RoomDetail;
                    ddto.date = d.Date.ToString();
                    DateTime dt = Convert.ToDateTime(d.Date);
                    string g = (dt).Date.ToString("yyyy-MM-dd");
                    DateTime tt = DateTime.ParseExact(g, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    string fp = "";
                    List<RegistrationDetailDTO> regFop = new List<RegistrationDetailDTO>();
                    regFop = UIProcessManager.GetRegistrationDetailByDateRoom(roomcode, t).ToList();
                    if (regFop.Count == 0)
                    {
                        ddto.foperson = "0";
                    }
                    else
                    {
                        int totalFoPerson = 0;
                        foreach (RegistrationDetailDTO regD in regFop)
                        {
                            totalFoPerson += Convert.ToInt32(regD.Adult);
                            ddto.foperson = totalFoPerson.ToString();
                        }
                    }

                    holdValuee.Add(ddto);
                }
            }
            string k = t.ToString();
            //currcurrentDateDiscrepancy = holdValuee.Where(b => b.date == t.ToString()).ToList();//select records of current date only for display
            gridControl1.DataSource = null;
            gridControl1.DataSource = holdValuee;
        }

        private void CloselClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
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
            roomtypecombo.DataSource = rmType;
        }

        private void BindRoomNums()
        {

            List<VwRoomManagmentViewDTO> rmNum = new List<VwRoomManagmentViewDTO>();
            List<VwRoomManagmentViewDTO> fakeRooms = UIProcessManager.GetAllRoomManagment(SelectedHotelcode);
            if (pseuodoRooms.Count > 0)
            {
                foreach (RoomTypeDTO rt in pseuodoRooms)
                {
                    fakeRooms.RemoveAll(b => b.RoomTypeId == rt.Id);
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

            roomcombo.DataSource = rmNum;
        }

        private void BindFloor()
        {
            List<VwRoomManagmentViewDTO> Rooms = UIProcessManager.GetAllRoomManagment(SelectedHotelcode);

            Rooms = Rooms.Where(x => RoomDetailcodeList.Contains(x.roomDetailCode)).ToList();
            List<string> floornumberlist = Rooms.Select(b => b.Floor).Distinct().ToList();
            floorcombo.DisplayMember = "Floor";
            floorcombo.ValueMember = "Floor";
            floorcombo.DataSource = floornumberlist;
        }

        private void NewDiscrepancyDialog(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            frmDiscrpancyDialog discre = new frmDiscrpancyDialog();
            discre.SelectedHotelcode = SelectedHotelcode;
            discre.RoomDetailcodeList = RoomDetailcodeList;
            discre.Tag = "New";
            discre.ShowDialog();
            discre.StartPosition = FormStartPosition.CenterScreen;

        }
        public int? GetActivityCode(int lookup)
        {
            ActivityDefinitionDTO listAD = LocalBuffer.LocalBuffer.ActivityDefinitionBufferList.FirstOrDefault(b => b.Description == lookup);
            int? adCode;
            if (listAD != null)
            {
                return listAD.Id;
            }
            else
            {
                return null;
            }
        }
        private Color StatusColor(int lookup, List<VwRoomManagmentViewDTO> RoomManagementList)
        {
            int? objectState = GetActivityCode(lookup);
            VwRoomManagmentViewDTO mgmt = RoomManagementList.FirstOrDefault(r => r.roomStatusCode == objectState);
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
                List<VwRoomManagmentViewDTO> vwRoomManagmentViews = UIProcessManager.GetAllRoomManagment(SelectedHotelcode);
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
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
                        Color color = StatusColor(CNETConstantes.OOS, vwRoomManagmentViews);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;
                    }
                    else if (colorCode == CNETConstantes.status_clean)
                    {
                        Color color = StatusColor(CNETConstantes.CLEAN, vwRoomManagmentViews);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;// Color.Yellow;
                    }
                    else if (colorCode == CNETConstantes.status_dirty)
                    {
                        Color color = StatusColor(CNETConstantes.Dirty, vwRoomManagmentViews);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;//Color.Red;
                    }
                    else if (colorCode == CNETConstantes.status_inspected)
                    {
                        Color color = StatusColor(CNETConstantes.INSPECTED, vwRoomManagmentViews);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;// Color.Blue;
                    }
                    else if (colorCode == CNETConstantes.status_ooo)
                    {
                        Color color = StatusColor(CNETConstantes.OOO, vwRoomManagmentViews);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;// Color.Orange;
                    }
                    else if (colorCode == CNETConstantes.status_pickup)
                    {
                        Color color = StatusColor(CNETConstantes.PICKUP, vwRoomManagmentViews);
                        gridView1.Columns["roomstatus"].AppearanceCell.BackColor = color;// Color.Lime;
                    }
                }
            }
            catch (Exception col)
            {

            }
        }

        private void frmDescripancy_Load(object sender, EventArgs e)
        {
            pickdate.EditValue = CurrentTime.Date;// DateTime.Now.Date;
            pseuodoRooms = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
            GridColumn col = roomtypecombo.View.Columns.AddField("Description");
            col.Visible = true;

            roomtypecombo.DisplayMember = "Description";
            roomtypecombo.ValueMember = "Description";

            GridColumn col1 = roomcombo.View.Columns.AddField("roomNo");
            col1.Visible = true;

            roomcombo.DisplayMember = "roomNo";
            roomcombo.ValueMember = "roomDetailCode";

            reiHotel.DisplayMember = "Name";
            reiHotel.ValueMember = "Id";
            reiHotel.DataSource = (LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name })).ToList();

            if (LocalBuffer.LocalBuffer.HotelBranchBufferList != null && LocalBuffer.LocalBuffer.HotelBranchBufferList.Count > 0)
            {
                beiHotel.EditValue = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
                reiHotel.ReadOnly = !LocalBuffer.LocalBuffer.UserHasHotelBranchAccess;
            }


            bbiSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(SaveClikedOnDialog);
        }

        private void SaveClikedOnDialog(object sender, EventArgs e)
        {
            gridControl1.DataSource = null;
            holdValuee.Clear();
            PopulateGrid(DateTime.Now.ToShortDateString());

        }
        /// <summary>
        /// button click event that will refersh discrepancy controls (checkboxes,lookupedits...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RefreshClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //refresh check boxes
            sleep1.Checked = false;
            skip1.Checked = false;
            dueout1.Checked = false;
            person1.Checked = false;



            pickdate.EditValue = null;
            pickdate.EditValue = CurrentTime.Date;// DateTime.Now.Date;
            roomnosrch.EditValue = null;
            floorSrch.EditValue = null;
            roomtypesrch.EditValue = null;
            gridControl1.DataSource = null;
            holdValuee.Clear();
            PopulateGrid(DateTime.Now.ToShortDateString());

        }

        private void RoomType_EditValueChanged(object sender, EventArgs e)
        {
            string selectedText = "";
            if (roomtypesrch.EditValue == null)
            {
                selectedText = "";
                gridControl1.DataSource = holdValuee;
                return;
            }
            else
            {
                selectedText = roomtypesrch.EditValue.ToString();
            }

            List<DiscrepancyVM> holdSelectedValue = new List<DiscrepancyVM>();
            List<DiscrepancyVM> filterHoldSelectedValue = new List<DiscrepancyVM>();
            List<DiscrepancyVM> vacantlist = holdValuee as List<DiscrepancyVM>;

            string t = "";

            if (roomnosrch.EditValue != null)
            {
                t = roomnosrch.Edit.GetDisplayText(roomnosrch.EditValue);
            }
            if (t != "")
            {
                holdSelectedValue = vacantlist.Where(l => l.roomtype == selectedText & l.roomno.Contains(t)).ToList();
            }
            else
            {
                holdSelectedValue = vacantlist.Where(l => l.roomtype == selectedText).ToList();
            }


            foreach (CheckBox ch in checkboxnames)
            {
                //Text = ch.CheckState.ToString();
                if (ch.Checked)
                {
                    List<DiscrepancyVM> rm = vacantlist.Where(x => x.discrepancy == Convert.ToInt32(ch.AccessibleName) & x.roomtype == selectedText).ToList();
                    filterHoldSelectedValue.AddRange(rm);

                }
            }
            if (selectedText == "") { gridControl1.DataSource = holdValuee; }
            else if (filterHoldSelectedValue.Count == 0) { gridControl1.DataSource = holdSelectedValue; }
            else if (filterHoldSelectedValue.Count != 0) { gridControl1.DataSource = filterHoldSelectedValue; }
        }

        private void RoomNum_EditValueChanged(object sender, EventArgs e)
        {
            string selectedText = "";
            if (roomnosrch.EditValue == null)
            {
                selectedText = "";
                gridControl1.DataSource = currcurrentDateDiscrepancy;
                return;
            }
            else
            {
                selectedText = roomnosrch.Edit.GetDisplayText(roomnosrch.EditValue);

            }

            List<DiscrepancyVM> holdSelectedValue = new List<DiscrepancyVM>();
            List<DiscrepancyVM> filterHoldSelectedValue = new List<DiscrepancyVM>();
            List<DiscrepancyVM> vacantlist = holdValuee as List<DiscrepancyVM>;

            string t = "";

            if (roomtypesrch.EditValue != null)
            {
                t = roomtypesrch.Edit.GetDisplayText(roomtypesrch.EditValue);
            }
            if (t != "")
            {
                holdSelectedValue = vacantlist.Where(l => l.roomno == selectedText & l.roomtype == t).ToList();
            }
            else
            {
                holdSelectedValue = vacantlist.Where(l => l.roomno == selectedText).ToList();
            }


            foreach (CheckBox ch in checkboxnames)
            {

                if (ch.Checked)
                {
                    List<DiscrepancyVM> rm = vacantlist.Where(x => x.discrepancy == Convert.ToInt32(ch.AccessibleName) & x.roomno == selectedText).ToList();
                    filterHoldSelectedValue.AddRange(rm);

                }
            }
            if (selectedText == "") { gridControl1.DataSource = currcurrentDateDiscrepancy; }
            else if (filterHoldSelectedValue.Count == 0) { gridControl1.DataSource = holdSelectedValue; }
            else if (filterHoldSelectedValue.Count != 0) { gridControl1.DataSource = filterHoldSelectedValue; }
        }

        /// <summary>
        /// Event That will be fired when one of discrepancy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private void clearButton_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = null;
            gridControl1.DataSource = holdValuee;

        }

        private void UpdateOnDoubleClick(object sender, RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
                Object id = gridView1.GetFocusedRow();
                var drr = id as DiscrepancyVM;
                frmDiscrpancyDialog discre = new frmDiscrpancyDialog();
                discre.SelectedHotelcode = SelectedHotelcode;
                discre.RoomDetailcodeList = RoomDetailcodeList;
                if (drr.date != t.ToString())
                {
                    XtraMessageBox.Show("Can Only Update Discrepancy For Current Date", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                discre.rpRoomNoCombo.EditValue = drr.roomdetail;
                discre.hkStatusCombo.EditValue = drr.hkstatus;
                discre.rmStatus.EditValue = drr.roomstatus;
                discre.numberCombo.Value = Convert.ToInt32(drr.hkperson);
                discre.discrepancyTextBox.EditValue = drr.discrepancy;
                discre.foPerson.Text = drr.foperson;

                discre.StartPosition = FormStartPosition.CenterScreen;
                discre.Tag = "Update";
                discre.ShowDialog();
            }

        }

        private void DatePicked(object sender, EventArgs e)
        {
            if (((DevExpress.XtraBars.BarEditItem)(sender)).EditValue == null)
            {
                gridControl1.DataSource = null;
                gridControl1.DataSource = currcurrentDateDiscrepancy;
                return;
            }
            DateTime dateTime = (DateTime)((DevExpress.XtraBars.BarEditItem)(sender)).EditValue;

            string j = dateTime.ToString();
            if (dateTime == null) { return; }
            string time = dateTime.Date.ToString("yyyy-MM-dd");
            DateTime t = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            List<DiscrepancyDTO> bydate = new List<DiscrepancyDTO>();
            if (SelectedHotelcode != null)
                PopulateGrid(t.ToShortDateString());


        }

        private void Floor_ValueChanged(object sender, EventArgs e)
        {
            Progress_Reporter.Show_Progress("fetching data ...", "Please Wait...");
            if (floorSrch.EditValue == null)
            {
                roomcombo.View.Columns.RemoveAt(0);
                roomcombo.DataSource = null;
                BindRoomNums();
            }
            else
            {
                List<VwRoomManagmentViewDTO> rmNum = UIProcessManager.GetAllRoomManagment(SelectedHotelcode).Where(r => r.Floor == floorSrch.EditValue.ToString()).ToList();
                roomcombo.DataSource = null;
                roomcombo.View.Columns.RemoveAt(0);
                GridColumn col = roomcombo.View.Columns.AddField("roomNo");
                col.Visible = true;

                roomcombo.DisplayMember = "roomNo";
                roomcombo.ValueMember = "roomDetailCode";
                roomcombo.DataSource = rmNum;


                List<DiscrepancyVM> byfloor = new List<DiscrepancyVM>();
                foreach (VwRoomManagmentViewDTO r in rmNum)
                {
                    DiscrepancyVM d = new DiscrepancyVM();
                    if (pickdate.EditValue != null)
                    {
                        d = holdValuee.Where(b => b.roomdetail == r.roomDetailCode && b.date == pickdate.EditValue.ToString()).FirstOrDefault();
                    }
                    else if (pickdate.EditValue == null)
                    {
                        d = holdValuee.Where(b => b.roomdetail == r.roomDetailCode && b.date == DateTime.Now.ToShortDateString()).FirstOrDefault();
                    }

                    if (d != null)
                    {
                        byfloor.Add(d);
                    }
                }

                gridControl1.DataSource = byfloor;
            }
            Progress_Reporter.Close_Progress();
        }

        private void sleep1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            filter();
        }

        private void skip1_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            filter();
        }

        private void person1_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            filter();
        }

        private void dueout1_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            filter();
        }

        private void PrintClick(object sender, ItemClickEventArgs e)
        {
            HouseKeepingReport report = new HouseKeepingReport();
            report = new HouseKeepingReport(gridControl1, false, "", "");
            report.Landscape = true;

            ReportPrintTool pt = new ReportPrintTool(report);
            pt.ShowPreview();
        }

        private void headerLink_CreateDetailArea(object sender, CreateAreaEventArgs e)
        {
            TextBrick brick = new TextBrick(BorderSide.None, 0, Color.White, Color.Gray, Color.Blue);
            brick.Text = "Discrepancy For " + CurrentTime.Date.ToShortDateString();//DateTime.Now.ToShortDateString();
            brick.Rect = new RectangleF(0, 0, 400, 20);
            e.Graph.DrawBrick(brick);
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
        List<int> RoomDetailcodeList { get; set; }
        List<RoomTypeDTO> RoomTypeList { get; set; }
        List<int> RoomTypeListcode = new List<int>();
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

            BindFloor();
            BindRoomNums();
            BindRoomTypes();
            PopulateGrid(DateTime.Now.ToShortDateString());
            ShowActivity();

        }

        public int? SelectedHotelcode { get; set; }
    }
}