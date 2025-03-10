using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.HouseKeeping
{
    public partial class frmDiscrpancyDialog : XtraForm
    {

        static DateTime? currentDate = UIProcessManager.GetServiceTime();
        public int? SelectedHotelcode { get; set; }
        static string time = currentDate.Value.Date.ToString("yyyy-MM-dd");
        DateTime t = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        // List<spGetCheckedInRooms_Result> lii = new List<spGetCheckedInRooms_Result>();
        List<VwRoomManagmentViewDTO> rooms = new List<VwRoomManagmentViewDTO>();
        List<string> comboListHk = new List<string>();
        List<string> comboList = new List<string>();
        DateTime CurrentTime { get; set; }
        public List<int> RoomDetailcodeList { get; set; }
        public frmDiscrpancyDialog()
        {
            InitializeComponent();

            try
            {
                DateTime? date = UIProcessManager.GetServiceTime();
                if (date != null)
                {
                    CurrentTime = date.Value;
                }
                else
                {
                    XtraMessageBox.Show("Server DateTime Error!!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<SystemConstantDTO> list = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(x => x.Category == "Housekeeping Activities").ToList();//.Where(cd => cd.component == CNETConstantes.HouseKeeping_Mgt).ToList();

            for (int i = 0; i < 6; i++)
            {
                string state = list[i].Description;
                comboList.Add(state);
            }


            bindHKStatus();
            rmStatus.Properties.DataSource = comboList;
            barEditItem1.EditValue = CurrentTime.Date;
            barEditItem1.CanOpenEdit = false;

            // lii = PMSUIProcessManager.CheckedInRooms().ToList().Where(x => x.foStatus == CNETConstantes.CHECKED_IN_STATE & x.Date == t).ToList();

            //rooms = PMSUIProcessManager.getRoomMgmtDetails();


        }
        private void frmDiscrpancyDialog_Load(object sender, EventArgs e)
        {
            List<VwRoomManagmentViewDTO> orig = new List<VwRoomManagmentViewDTO>();
            orig = UIProcessManager.GetAllRoomManagment(SelectedHotelcode);
            //rooms = new List<vw_roomManagment>();
            List<RoomTypeDTO> li = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
            if (li.Count > 0)
            {
                foreach (RoomTypeDTO r in li)
                {
                    List<VwRoomManagmentViewDTO> temp = new List<VwRoomManagmentViewDTO>();
                    orig.RemoveAll(b => b.RoomTypeId == r.Id);
                }
                rooms.AddRange(orig);
            }
            else if (li.Count == 0)
            {
                rooms.AddRange(orig);
            }
            GridColumn col = rpRoomNoCombo.Properties.View.Columns.AddField("roomNo");
            GridColumn col2 = rpRoomNoCombo.Properties.View.Columns.AddField("RoomType");
            col.Visible = true;
            col2.Visible = true;

            if (rooms != null)
                rooms = rooms.Where(x => RoomDetailcodeList.Contains(x.roomDetailCode)).ToList();

            rpRoomNoCombo.Properties.DataSource = rooms;
            rpRoomNoCombo.Properties.ValueMember = "roomDetailCode";
            rpRoomNoCombo.Properties.DisplayMember = "roomNo";
            if (this.Tag.Equals("Update"))
            {
                rmN = rpRoomNoCombo.EditValue;
                hks = hkStatusCombo.EditValue;
                rms = rmStatus.EditValue;
                j = numberCombo.Value;
                dtb = discrepancyTextBox.Text;
                fop = foPerson.Text;
            }
            else if (this.Tag.Equals("New"))
            {
                string lam = this.Tag.ToString();
            }


        }

        private void CloseDiscrpancyDialog(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bindHKStatus()
        {
            List<LookupDTO> list = LocalBuffer.LocalBuffer.LookUpBufferList.Where(x => x.Type == "Front Office Status").Take(2).ToList();

            for (int i = 0; i < 2; i++)
            {
                string fostatus = list[i].Description;
                comboListHk.Add(fostatus);
            }


            hkStatusCombo.Properties.DataSource = list;
            hkStatusCombo.Properties.DisplayMember = "Description";
            hkStatusCombo.Properties.ValueMember = "Id";
        }


        private void RoomNo_ValueChanged(object sender, EventArgs e)
        {
            string time = CurrentTime.Date.ToString("yyyy-MM-dd");
            DateTime t = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            int roomDetailCode = 0;
            if (rpRoomNoCombo.EditValue == null)
            {
                roomDetailCode = 0;
            }
            else
            {
                roomDetailCode = Convert.ToInt32(rpRoomNoCombo.EditValue);
            }

            VwRoomManagmentViewDTO li = UIProcessManager.GetAllRoomManagment(SelectedHotelcode).Where(l => l.roomDetailCode == roomDetailCode).ToList().First();
            roomType.Text = li.RoomType;

            RegistrationStatusDTO registrationStatus = UIProcessManager.GetRegistrationStatus(roomDetailCode, t);

            string fostatus = registrationStatus.FOStatus;
            string resStatus = registrationStatus.registrationStatus;
            if (fostatus == null) { foStatus.Text = "Vacant"; resStatus = "Not Reserved"; }
            else if (fostatus.Equals("0")) { foStatus.Text = "Vacant"; }
            else { foStatus.Text = "Occupied"; }

            resStatusEdit.Text = resStatus;
            List<RegistrationDetailDTO> regFop = new List<RegistrationDetailDTO>();
            regFop = UIProcessManager.GetRegistrationDetailByDateRoom(roomDetailCode, t).ToList();
            if (regFop.Count == 0)
            {
                foPerson.Text = "0";
            }
            else
            {
                int totalFoPerson = 0;
                foreach (RegistrationDetailDTO regD in regFop)
                {
                    totalFoPerson += Convert.ToInt32(regD.Adult);
                    foPerson.Text = totalFoPerson.ToString();
                }
            }

        }

        private void HKStatus_ValueChanged(object sender, EventArgs e)
        {
            string hkperson = "";
            string hkvalue = hkStatusCombo.Text;
            string fostatus = foStatus.Text;

            if (numberCombo.Value.ToString() != "")
            {
                hkperson = numberCombo.Value.ToString();
            }
            else
            {
                hkperson = "";
            }


            string foperson = foPerson.Text;

            checkDiscrepancy(hkvalue, fostatus, hkperson, foperson);

        }

        private void Hkperson_ValueChanged(object sender, EventArgs e)
        {

            string hkvalue = "";
            if (hkStatusCombo.Text == "")
            {
                hkvalue = "";
            }
            else
            {
                hkvalue = hkStatusCombo.Text;

            }

            string fostatus = foStatus.Text;

            string hkperson = numberCombo.Value.ToString();
            string foperson = foPerson.Text;

            checkDiscrepancy(hkvalue, fostatus, hkperson, foperson);

        }

        private void checkDiscrepancy(string hkvalue, string fovalue, string hkperson, string foperson)
        {

            hkvalue = "";

            if (hkStatusCombo.Text == "")
            {
                hkvalue = "";
            }
            else
            {
                hkvalue = hkStatusCombo.Text.ToString();
            }

            fovalue = foStatus.Text;

            if (numberCombo.Value.ToString() != null)
            {
                hkperson = numberCombo.Value.ToString();
            }
            foperson = foPerson.Text;
            /*
              public const int Discrepancy_None = 244;
        public const int Discrepancy_Person = 245;
        public const int Discrepancy_Skip_Person = 246;
        public const int Discrepancy_Skips = 247;
        public const int Discrepancy_Sleep_Person = 248;
        public const int Discrepancy_Sleeps = 249; */
            if (hkperson.Equals(foperson) && hkvalue.Equals(fovalue))
            {
                discrepancyTextBox.Text = "None";
                DiscrepancytypeId = CNETConstantes.Discrepancy_None;
            }
            else if (!hkperson.Equals(foperson) && (hkvalue.Equals("Vacant") && fovalue.Equals("Occupied")))
            {
                discrepancyTextBox.Text = "Skip/Person";
                DiscrepancytypeId = CNETConstantes.Discrepancy_Skip_Person;
            }
            else if (!hkperson.Equals(foperson) && (hkvalue.Equals("Occupied") && fovalue.Equals("Vacant")))
            {
                discrepancyTextBox.Text = "Sleep/Person";
                DiscrepancytypeId = CNETConstantes.Discrepancy_Sleep_Person;
            }
            else if ((fovalue.Equals("Vacant") & hkvalue.Equals("Occupied")) & hkperson.Equals(foperson))
            {
                discrepancyTextBox.Text = "Sleep";
                DiscrepancytypeId = CNETConstantes.Discrepancy_Sleeps;
            }
            else if ((fovalue.Equals("Occupied") & hkvalue.Equals("Vacant")) & hkperson.Equals(foperson))
            {
                discrepancyTextBox.Text = "Skip";
                DiscrepancytypeId = CNETConstantes.Discrepancy_Skips;
            }
            else if (fovalue.Equals(hkvalue) & !hkperson.Equals(foperson))
            {
                discrepancyTextBox.Text = "Person";
                DiscrepancytypeId = CNETConstantes.Discrepancy_Person;
            }
            else if (hkvalue.Equals("") & hkperson.Equals(foperson))
            {
                discrepancyTextBox.Text = "None";
                DiscrepancytypeId = CNETConstantes.Discrepancy_None;
            }
            else if (hkvalue.Equals("") & !hkperson.Equals(foperson))
            {
                discrepancyTextBox.Text = "Person";
                DiscrepancytypeId = CNETConstantes.Discrepancy_Person;
            }
            else
            {
                discrepancyTextBox.Text = "None";
                DiscrepancytypeId = CNETConstantes.Discrepancy_None;
            }
        }
        int DiscrepancytypeId = CNETConstantes.Discrepancy_None;

        private void SaveDiscrepancy(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                DateTime? date = UIProcessManager.GetServiceTime();
                if (date != null)
                {
                    CurrentTime = date.Value;
                }
                else
                {
                    XtraMessageBox.Show("Server Date Time Error !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                frmDescripancy frm = new frmDescripancy();

                int roomStatusIndex = rmStatus.ItemIndex;

                string rmStatusText = rmStatus.Text;

                string descripancy = discrepancyTextBox.Text;
                string rmark = remarkTextBox.Text;
                int roomDetailCode = Convert.ToInt32(rpRoomNoCombo.EditValue);
                int hkperson = Convert.ToInt32(numberCombo.Value);
                int hkvalue = Convert.ToInt32(hkStatusCombo.EditValue);

                LookupDTO discrepancyCode = LocalBuffer.LocalBuffer.LookUpBufferList.FirstOrDefault(x => x.Id == DiscrepancytypeId);

                //LookupDTO hkValueCode = LocalBuffer.LocalBuffer.LookUpBufferList.Where(x => x.Type.Contains("Front Office Staus")).FirstOrDefault();

                DiscrepancyDTO disc = new DiscrepancyDTO();



                disc.Description = discrepancyCode== null ?"None": discrepancyCode.Description;
                disc.DiscrepancyType = discrepancyCode == null ? CNETConstantes.Discrepancy_None : DiscrepancytypeId;

                disc.Date = t;
                disc.Remark = rmark;
                disc.RoomDetail = roomDetailCode;
                disc.HkValue = hkvalue;
                disc.FoValue = hkperson;
                disc.ConsigneeUnit = SelectedHotelcode;

                RoomDetailDTO rd = UIProcessManager.GetRoomDetailById(roomDetailCode);
                DiscrepancyDTO ar = UIProcessManager.SelectAllDiscrepancy().Where(x => x.RoomDetail == roomDetailCode).ToList().FirstOrDefault();
                if (ar == null)
                {
                    DiscrepancyDTO isSaved = UIProcessManager.CreateDiscrepancy(disc);
                    if (isSaved != null)
                    {
                        XtraMessageBox.Show("Discrepancy Saved Successfully!", "Discrepancy Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frm.gridControl1.Refresh();
                        frm.gridControl1.RefreshDataSource();
                    }
                    //if (rd != null)
                    //{
                    //    logActiviy(rd, true);
                    //}
                }
                else
                {
                    disc.Id = ar.Id;
                    DiscrepancyDTO isUpdated = UIProcessManager.UpdateDiscrepancy(disc);
                    if (isUpdated != null)
                    {
                        XtraMessageBox.Show("Discrepancy Updated Successfully!", "Discrepancy Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frm.gridControl1.Refresh();
                        frm.gridControl1.RefreshDataSource();
                    }
                    //if (rd != null)
                    //{
                    //    logActiviy(rd, true);
                    //}
                }
                if (roomStatusIndex < 0)
                {
                    return;
                }
                ChangeRoomStatus(roomStatusIndex);

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Discrepancy Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CancelClicked(null, null);
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
        private void ChangeRoomStatus(int itemIndex)
        {

            int rodCode = 0;
            if (rpRoomNoCombo.Text == "")
            {
                return;
            }
            else
            {
                rodCode = Convert.ToInt32(rpRoomNoCombo.EditValue);
            }

            RoomDetailDTO rmd = UIProcessManager.GetRoomDetailById(rodCode);
            switch (itemIndex)
            {
                case 0:

                    //rmd.lastState = CNETConstantes.CLEAN;
                    int? clean = GetActivityCode(CNETConstantes.CLEAN);
                    rmd.LastState = clean;
                    break;
                case 2:
                    //rmd.lastState = CNETConstantes.Dirty;
                    int? dirty = GetActivityCode(CNETConstantes.Dirty);
                    rmd.LastState = dirty;
                    break;
                case 3:
                    int? inspected = GetActivityCode(CNETConstantes.INSPECTED);
                    rmd.LastState = inspected;

                    //rmd.lastState = CNETConstantes.OOO;
                    break;
                case 5:
                    int? oos = GetActivityCode(CNETConstantes.OOS);
                    rmd.LastState = oos;
                    //rmd.lastState = CNETConstantes.OOS;
                    break;
                case 4:
                    int? ooo = GetActivityCode(CNETConstantes.OOO);
                    rmd.LastState = ooo;

                    break;
                case 1:
                    int? pickup = GetActivityCode(CNETConstantes.PICKUP);
                    rmd.LastState = pickup;
                    //rmd.lastState = CNETConstantes.PICKUP;
                    break;
            }
            /* Update the room detail here and Log Activity*/
            UIProcessManager.UpdateRoomDetail(rmd);
            logActiviy(rmd, false);
        }
        int dCounter = 0;
        private void logActiviy(RoomDetailDTO rmd, bool isdisc)
        {
            try
            {
                ActivityDTO act = new ActivityDTO();
                if (isdisc)
                {
                    int? discAdded = GetActivityCode(CNETConstantes.HK_MAINTAINED);
                    //act.activitiyDefinition = CNETConstantes.DISCREPANCY_ADDED;
                    act.ActivityDefinition = discAdded.Value;
                    act.Remark = "House Keeping Discrepancy";
                }
                else
                {
                    act.ActivityDefinition = rmd.LastState.Value;
                    act.Remark = "House Keeping Discrepancy";
                }

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


                UIProcessManager.CreateActivity(act);
            }
            catch (Exception ex)
            {

                if (dCounter == 0)
                {
                    XtraMessageBox.Show("Discrepancy Activity Not Saved!", "Activity Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dCounter++;
                }
            }
        }

        private void clear()
        {
            numberCombo.Value = 0;
            remarkTextBox.Text = "";

            discrepancyTextBox.Text = "";
            // if (rpRoomNoCombo.EditValue != "") {
            roomType.Text = "";
            foStatus.Text = "";
            resStatusEdit.Text = "";
            foPerson.Text = "";
            //rpRoomNoCombo.Text = "";
            //rpRoomNoCombo.EditValue = "";
            // }
            //if (hkStatusCombo.EditValue != null) {
            hkStatusCombo.Properties.DataSource = null;
            hkStatusCombo.EditValue = "";
            hkStatusCombo.Properties.DataSource = comboListHk;
            // }
            // if (rmStatus != null) {
            rmStatus.Properties.DataSource = null;
            rmStatus.EditValue = "";
            rmStatus.Properties.DataSource = comboList;
            // }


        }

        private void CancelClicked(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (rmN != null & hks != null & rms != null & dtb != null & fop != null)
            {
                try
                {
                    clear();
                    rpRoomNoCombo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                    rpRoomNoCombo.EditValue = null;
                    rmStatus.EditValue = null;


                    foPerson.Text = "";
                    if (hkStatusCombo.EditValue != null && hkStatusCombo.EditValue != "")
                    {
                        hkStatusCombo.EditValue = null;
                    }

                }
                catch (Exception ex) { }
            }
            else if (rpRoomNoCombo.EditValue != "" | hkStatusCombo.EditValue != null | rmStatus.EditValue != null)
            {
                clear();
            }
            numberCombo.ResetText();
            discrepancyTextBox.Text = "";
        }

        public object rmN;
        public object hks;
        public object rms;
        public decimal j;
        public string dtb;
        public string fop;
        private const string HK_ACTIVITY = "HK_ACTIVITY";
        private const string HK_ACTIVITY_DISC = "HK_ACTIVITY_DISC";
    }
}
