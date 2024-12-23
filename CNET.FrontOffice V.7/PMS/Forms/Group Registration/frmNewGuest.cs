using CNET.ERP.Client.Common.UI;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Group_Registration
{
    public partial class frmNewGuest : XtraForm
    {

        public AddedGuest SavedGuest { get; set; }

        private int? _defGender = null;

        public frmNewGuest()
        {
            InitializeComponent();

            //Gender List
            lukGender.Properties.Columns.Add(new LookUpColumnInfo("Description", "Sex"));
            lukGender.Properties.DisplayMember = "Description";
            lukGender.Properties.ValueMember = "Id";

            //Nationlity
            lukNationality.Properties.Columns.Add(new LookUpColumnInfo("name", "Country"));
            lukNationality.Properties.Columns.Add(new LookUpColumnInfo("nationality", "Nationality"));
            lukNationality.Properties.DisplayMember = "nationality";
            lukNationality.Properties.ValueMember = "Id";
        }

        #region Helper Methods

        private bool InitializeData()
        {
            try
            {


                //lukIdType.Properties.Columns.Add(new LookUpColumnInfo("Description", "ID Type"));
                //lukIdType.Properties.DisplayMember = "Description";
                //lukIdType.Properties.ValueMember = "Id";

                ////ID type list
                //var idTypeList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.PersonalIdentificationType && l.IsActive).ToList();
                //if (idTypeList != null)
                //{
                //    lukIdType.Properties.DataSource = (idTypeList.OrderByDescending(c => c.IsDefault).ToList());
                //    var defualtID = idTypeList.FirstOrDefault(c => c.IsDefault);
                //    if (defualtID != null)
                //    {
                //        lukIdType.EditValue = (defualtID.Id);
                //    }
                //}

                //Gender
                var genderList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.GENDER && l.IsActive).ToList();
                if (genderList != null)
                {
                    lukGender.Properties.DataSource = genderList;
                    var defaultSex = genderList.FirstOrDefault(c => c.IsDefault);
                    if (defaultSex != null)
                    {
                        lukGender.EditValue = (defaultSex.Id);
                        _defGender = defaultSex.Id;
                    }
                }

                //Nationality
                lukNationality.Properties.DataSource = LocalBuffer.LocalBuffer.CountryBufferList;

                if (SavedGuest != null)
                {
                    teName.Text = string.Format("{0} {1} {2}", SavedGuest.FirstName, SavedGuest.LastName, SavedGuest.MiddleName);
                    teIdNumber.Text = SavedGuest.IDNumber;
                    teTelephone.Text = SavedGuest.Telephone;
                    //lukIdType.EditValue = SavedGuest.IDType;
                    lukGender.EditValue = SavedGuest.Gender;
                    lukNationality.EditValue = SavedGuest.Nationality;
                }


                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        #endregion

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            teName.EditValue = null;
            lukGender.EditValue = null;
            lukNationality.EditValue = null;
            teIdNumber.EditValue = null;
            teTelephone.EditValue = null;
            this.Close();
        }

        private void frmNewGuest_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                DialogResult = DialogResult.Abort;
                this.Hide();
            }
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                    teName,
                    lukNationality,
                    teIdNumber,
                    lukGender
                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    return;
                }


                string code = SavedGuest == null ? Guid.NewGuid().ToString() : SavedGuest.Code;
                if (SavedGuest == null)
                    SavedGuest = new AddedGuest();

                string[] names = teName.Text.Split(' ');
                if (names == null || names.Length < 1)
                {
                    SystemMessage.ShowModalInfoMessage("Please Enter Full Name", "ERROR");
                    return;
                }

                SavedGuest.Code = code;
                SavedGuest.FirstName = names[0];
                SavedGuest.LastName = names.Length > 1 ? names[1] : "";
                SavedGuest.MiddleName = names.Length > 2 ? names[2] : "";
                //SavedGuest.IDType = Convert.ToInt32(lukIdType.EditValue);
                SavedGuest.IDNumber = teIdNumber.Text;
                SavedGuest.Telephone = teTelephone.Text;
                SavedGuest.Gender = Convert.ToInt32(lukGender.EditValue);
                SavedGuest.Nationality = Convert.ToInt32(lukNationality.EditValue);


                DialogResult = DialogResult.OK;

                teName.EditValue = null;
                lukGender.EditValue = _defGender;
                lukNationality.EditValue = null;
                teIdNumber.EditValue = null;
                teTelephone.EditValue = null;

                this.Hide();
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in saving new guest. DETAIL:: " + ex.Message, "ERROR");
            }
        }
    }

    public class AddedGuest
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int IDType { get; set; }
        public string IDNumber { get; set; }
        public string Telephone { get; set; }
        public int Gender { get; set; }
        public int Nationality { get; set; }
    }
}
