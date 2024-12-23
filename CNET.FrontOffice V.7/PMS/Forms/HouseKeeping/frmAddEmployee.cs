using CNET.ERP.Client.Common.UI;
using ProcessManager;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
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
    public partial class frmAddEmployee : DevExpress.XtraEditors.XtraForm
    {
        List<ConsigneeDTO> emps = new List<ConsigneeDTO>();
        public int? empCode = null;
        public frmAddEmployee()
        {
            InitializeComponent();
        }

        private void frmAddEmployee_Load(object sender, EventArgs e)
        {
            try
            {

               
                GridColumn col = empSearch.Properties.View.Columns.AddField("firstName");
                GridColumn col2 = empSearch.Properties.View.Columns.AddField("lastName");
                col.Visible = true;
                col2.Visible = true;

                empSearch.Properties.DataSource = BindEmployees();
                empSearch.Properties.ValueMember = "code";
                empSearch.Properties.DisplayMember = "firstName";
            }
            catch (Exception)
            {
                
               
            }
        }

        public List<ConsigneeDTO> BindEmployees()
        {
            List<ConsigneeDTO> ListPerson = new List<ConsigneeDTO>();
            List<ConsigneeDTO> FilterListPerson = new List<ConsigneeDTO>();
            List<string> Personcodelist = new List<string>();
            int code = 0;

            ConsigneeUnitDTO org = LocalBuffer.LocalBuffer.AllHotelBranchBufferList.Where(x => x.Description.ToLower() == CNETConstantes.HOUSE_KEEPING_ATTENDANT
                && x.Type == CNETConstantes.ORGUNITTYPE_DEPARTMENT).FirstOrDefault();

            if (org != null)
            {
                code = org.Id;
            }
            List<RelationDTO> ListRelation = UIProcessManager.SelectAllRelation().Where(x => x.ReferencedObject == code).ToList();

            foreach (RelationDTO re in ListRelation)
            {
                ConsigneeDTO person = UIProcessManager.GetConsigneeById(re.ReferringObject);
                 
                if (person != null)
                    ListPerson.Add(person);
            }

            //if (ListPerson != null)
            //{
            //    Personcodelist = ListPerson.Select(x => x.Id).ToList();


            //    List<ConsigneeUnitDTO> Branchlist = LoginPage.Authentication.OrganizationUnitBufferList.Where(x => Personcodelist.Contains(x.reference) && x.organizationUnitDefinition == SelectedHotelcode).ToList();
            //    if (Branchlist != null && Branchlist.Count > 0)
            //    {
            //        List<string> EmployeeInBranch = Branchlist.Select(x => x.reference).ToList();
            //        FilterListPerson = ListPerson.Where(x => EmployeeInBranch.Contains(x.code)).ToList();
            //    }
            //}
            return FilterListPerson;

        }
        private void CloseClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void OkClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (empSearch.EditValue == null)
                {
                    XtraMessageBox.Show("Please Select An Employee From The List!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    empCode =Convert.ToInt32( empSearch.EditValue);
                }
                this.Close();
            }
            catch (Exception) { }
        }

        public int SelectedHotelcode { get; set; }
    }
}
