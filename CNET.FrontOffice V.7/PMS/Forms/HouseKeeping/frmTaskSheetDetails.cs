
using CNET.ERP.Client.Common.UI;
using ProcessManager;
using CNET.Progress.Reporter;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
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
    public partial class frmTaskSheetDetails : DevExpress.XtraEditors.XtraForm
    {
        static DateTime currentDate { get; set; }// = UIProcessManager.GetServiceTime().Value;
        static string time { get; set; }// = currentDate.Date.ToString("yyyy-MM-dd");
        DateTime t { get; set; }// = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        List<String> taskcodes = new List<String>();
        List<ConsigneeDTO> employees = new List<ConsigneeDTO>();

        List<string> accessibleNames = new List<string>();

        DateTime CurrentTime { get; set; }

        public frmTaskSheetDetails()
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
                    XtraMessageBox.Show("Error Getting Server Date Time !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                currentDate = date.Value;
                time = date.Value.Date.ToString("yyyy-MM-dd");
                t = DateTime.ParseExact(time, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmTaskSheetDetails_FormClosing);
            clean1.CheckedChanged += new ItemClickEventHandler(HoldcheckState);
            dirty1.CheckedChanged += new ItemClickEventHandler(HoldcheckState);
            pkup1.CheckedChanged += new ItemClickEventHandler(HoldcheckState);
            insp1.CheckedChanged += new ItemClickEventHandler(HoldcheckState);
            oo1.CheckedChanged += new ItemClickEventHandler(HoldcheckState);
            oos1.CheckedChanged += new ItemClickEventHandler(HoldcheckState);

        }

        private void frmTaskSheetDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmTaskAssignment frm = new frmTaskAssignment();
            DateTime time = (DateTime)barEditItem1.EditValue;
            frm.PopulateGrid(time.Date);
        }
        public Boolean isclosed = false;

        private void ClosedClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
            frmTaskAssignment frm = new frmTaskAssignment();
            DateTime time = (DateTime)barEditItem1.EditValue;
            frm.PopulateGrid(time.Date);

        }

        public List<ConsigneeDTO> BindEmployees()
        {

            List<ConsigneeDTO> ListPerson = new List<ConsigneeDTO>();
            List<ConsigneeDTO> FilterListPerson = new List<ConsigneeDTO>();
            List<string> Personcodelist = new List<string>();
            int? code = null;

            ConsigneeUnitDTO org = LocalBuffer.LocalBuffer.AllHotelBranchBufferList.FirstOrDefault(x => x.Name != null && x.Name.ToLower() == CNETConstantes.HOUSE_KEEPING_DEPARTMENT.ToString().ToLower()
                && x.Type == CNETConstantes.ORGUNITTYPE_DEPARTMENT);

            if (org != null)
            {
                code = org.Id;
            }
            List<RelationDTO> ListRelation = UIProcessManager.SelectAllRelation().Where(x => x.ReferencedObject == code).ToList();

            foreach (RelationDTO re in ListRelation)
            {
                // ConsigneeDTO person = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x=> x.Id == re.ReferringObject);
                ConsigneeDTO person = UIProcessManager.GetConsigneeById(re.ReferringObject);
                if (person != null)
                    ListPerson.Add(person);
            }

            //if (ListPerson != null)
            //{
            //    Personcodelist = ListPerson.Select(x => x.code).ToList();


            //    List<ConsigneeUnitDTO> Branchlist = LoginPage.Authentication.OrganizationUnitBufferList.Where(x => Personcodelist.Contains(x.reference) && x.organizationUnitDefinition == SelectedHotelcode).ToList();
            //    if (Branchlist != null && Branchlist.Count > 0)
            //    {
            //        List<string> EmployeeInBranch = Branchlist.Select(x => x.reference).ToList();
            //        FilterListPerson = ListPerson.Where(x => EmployeeInBranch.Contains(x.code)).ToList();
            //    }
            //}
            return ListPerson;

        }
        private void GenerateTaskCliked(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Progress_Reporter.Show_Progress("Generating Task Sheets ...", "Please Wait...");
            if (employees == null || employees.Count == 0)
            {
                XtraMessageBox.Show("Please Select Attendants!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<HkLoadDistributionDTO> hk = new List<HkLoadDistributionDTO>();
            List<HkLoadDistributionDTO> filtered = new List<HkLoadDistributionDTO>();
            List<int> fulllist = new List<int>();

            string taskcode = "";
            string fostatus = "";
            foreach (string taskname in taskcodes)
            {
                taskcode += taskname + "|";
            }
            foreach (string checkname in accessibleNames)
            {
                int? hkstatus = MapADs(checkname);
                if (hkstatus != null)
                    fulllist.Add(hkstatus.Value);
            }
            if (fulllist.Count == 0)
            {
                XtraMessageBox.Show("Please Select House Keeping Status!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (employees.Count == 0)
            {
                XtraMessageBox.Show("Please Select Employees!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!chkOcc.Checked && !chkVac.Checked)
            {
                XtraMessageBox.Show("Please Select Fo Status First!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            hk = UIProcessManager.AssignTask(t, employees, fulllist, 0, 0, SelectedHotelcode);
            if (hk != null && hk.Count == 0)
            {
                XtraMessageBox.Show("Check if \"isHousekeeping\" is Set to True on Room Type Maintainance Window!", "There is No Room To Assign!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (hk != null)
            {
                if (chkOcc.Checked && !chkVac.Checked)
                {
                    filtered = hk.Where(b => b.fostatus == "Occupied").ToList();
                }
                else if (chkVac.Checked && !chkOcc.Checked)
                {
                    filtered = hk.Where(b => b.fostatus == "Vacant").ToList();
                }
                else if (chkVac.Checked && chkOcc.Checked)
                {
                    filtered = hk;
                }
            }

            List<HkassignmentDTO> current = UIProcessManager.SelectAllHKAssignment().Where(b => b.Date == t && b.Consigneeunit == SelectedHotelcode).ToList();
            if (current != null | current.Count != 0)
            {
                foreach (HkassignmentDTO todaysAssignment in current)
                {
                    UIProcessManager.DeleteHKAssignmentById(todaysAssignment.Id);
                }
            }
            foreach (HkLoadDistributionDTO load in filtered)
            {
                int lo = load.room;
                if (load.employee == null)
                {
                    continue;
                }

                SaveTask(load.room, load.employee.Id, t, "", load.credit);

            }
            int count = hk.Count();
            XtraMessageBox.Show("Task Assigned. Open Task Sheet Window For Detail!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            logActiviy();
            ClosedClick(null, null);
            Progress_Reporter.Close_Progress();
        }

        private void logActiviy()
        {
            try
            {
                ActivityDTO act = new ActivityDTO();
                act.ActivityDefinition = GetActivityCode(CNETConstantes.HK_MAINTAINED).Value;
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
                act.Remark = "HK TASK SHEET GENERATED";
                UIProcessManager.CreateActivity(act);
            }
            catch (Exception e) { }
        }

        private void frmTaskSheetDetails_Load(object sender, EventArgs e)
        {

            List<ConsigneeDTO> ListPerson = BindEmployees();
            checkedComboBoxEdit1.Properties.DataSource = ListPerson;
            checkedComboBoxEdit1.Properties.DisplayMember = "FirstName";
            checkedComboBoxEdit1.Properties.ValueMember = "Id";
            barEditItem1.EditValue = currentDate.Date;
            barEditItem1.CanOpenEdit = false;
        }

        private void TaskCode_EditValueChanged(object sender, EventArgs e)
        {

            //object val = checkedComboBoxEdit2.EditValue;
            //if (val == null || val.Equals(string.Empty)) return;
            //string[] checkedItems = val.ToString().Split(checkedComboBoxEdit2.Properties.SeparatorChar);
            //List<string> trimed = new List<string>();
            //foreach (string item in checkedItems) {
            //    string j = item.Trim();
            //    trimed.Add(j);
            //}
            //taskcodes.Clear();
            //foreach (string item in trimed) {
            //    taskcodes.Add(item);
            //}
        }

        private void Attendant_EditValueChanged(object sender, EventArgs e)
        {
            object val = checkedComboBoxEdit1.EditValue;
            if (val == null || val.Equals(string.Empty)) return;
            string[] checkedItems = val.ToString().Split(checkedComboBoxEdit1.Properties.SeparatorChar);
            List<int> trimed = new List<int>();
            foreach (string item in checkedItems)
            {
                int j = Convert.ToInt32(item.Trim());
                trimed.Add(j);
            }
            employees.Clear();
            foreach (int item in trimed)
            {
                ConsigneeDTO per = UIProcessManager.GetConsigneeById(item);
                employees.Add(per);
            }

            // Text = checkedItems.Length.ToString();
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
        private int? MapADs(string val)
        {
            int? returnval = 0;

            switch (val)
            {
                case "Clean":
                    returnval = GetActivityCode(CNETConstantes.CLEAN);
                    break;
                case "Dirty":
                    returnval = GetActivityCode(CNETConstantes.Dirty);
                    break;
                case "Out Of Order":
                    returnval = GetActivityCode(CNETConstantes.OOO);
                    break;
                case "Out Of Service":
                    returnval = GetActivityCode(CNETConstantes.OOS);
                    break;
                case "Inspected":
                    returnval = GetActivityCode(CNETConstantes.INSPECTED);
                    break;
                default:
                    returnval = GetActivityCode(CNETConstantes.PICKUP);
                    break;
            }
            return returnval;
        }
        private void SaveTask(int roomcode, int empcode, DateTime date, string task, decimal credit)
        {
            HkassignmentDTO tsk = new HkassignmentDTO();
            tsk.Employee = empcode;
            tsk.Credit = credit;
            tsk.RoomDetail = roomcode;
            tsk.Date = date.Date;
            tsk.Task = 0;
            tsk.Consigneeunit = SelectedHotelcode;
            UIProcessManager.CreateHKAssignment(tsk);
        }

        private void HoldcheckState(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            BarCheckItem ch = sender as BarCheckItem;
            if (ch.Checked)
            {
                accessibleNames.Add(ch.AccessibleName);
            }
            else if (!ch.Checked)
            {
                accessibleNames.Remove(ch.AccessibleName);
            }
        }

        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            return;
        }

        public int SelectedHotelcode { get; set; }
    }
}
