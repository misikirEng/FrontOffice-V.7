using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.CommonSchema;

namespace CNET.FrontOffice_V._7.Forms.Setting_and_Miscellaneous.Revenue_Management_Modals
{
    public partial class frmRateCategory : XtraForm
    {



        private IList<Control> _invalidControls;
        private Boolean editMode = false;
        private int _defType;

        /** Properties **/
        public DateTime CurrentTime { get; set; }

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

        RateCategoryDTO editedRateCategory;

        internal RateCategoryDTO EditedRateCategory
        {
            get { return editedRateCategory; }
            set
            {
                editedRateCategory = value;
                editMode = true;

            }
        }

        /************* CONSTRUCTOR *********************/
        public frmRateCategory()
        {
            InitializeComponent();
            InitializeUI();

        }

        #region Helper Methods

        public void InitializeUI()
        {
            Utility.AdjustForm(this);
            Utility.AdjustRibbon(lciRibbonHolder);
            Location = new Point(450, 150);

            //parent category
            cacParentTextCategory.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacParentTextCategory.Properties.DisplayMember = "Description";
            cacParentTextCategory.Properties.ValueMember = "Id";

            //category Type
            cacTypeTextCategory.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacTypeTextCategory.Properties.DisplayMember = "Description";
            cacTypeTextCategory.Properties.ValueMember = "Id";
        }

        private bool InitializeData()
        {
            try
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return false;
                CurrentTime = currentTime.Value;


                // Progress_Reporter.Show_Progress("Initializing Data");



                //parent category list
                List<RateCategoryDTO> rateCatList = UIProcessManager.SelectAllRateCategory();
                cacParentTextCategory.Properties.DataSource = (rateCatList);

                //optional rate category description list
                List<LookupDTO> rateCategoryDescList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.RATE_CATEGORY_DESCRIPTION).ToList();
                teDescriptionTextCategory.Properties.Items.AddRange(rateCategoryDescList.Select(r => r.Description).ToList());

                //category type list
                List<LookupDTO> catList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.RATE_CATEGORY).ToList();
                cacTypeTextCategory.Properties.DataSource = catList;
                if (catList != null)
                {
                    var type = catList.FirstOrDefault(c => c.IsDefault);
                    if (type != null)
                    {
                        cacTypeTextCategory.EditValue = (type.Id);
                        _defType = type.Id;
                    }
                }

                if (EditedRateCategory != null)
                {
                    RateCategoryDTO orginalCat = UIProcessManager.GetRateCategoryById(EditedRateCategory.Id);
                    teCodeTextCategory.Text = EditedRateCategory.Id.ToString();
                    teDescriptionTextCategory.Text = EditedRateCategory.Description;
                    deStartDateTextCategory.DateTime = (DateTime)EditedRateCategory.StartDate;
                    deEndDateTextCategory.DateTime = (DateTime)EditedRateCategory.EndDate;
                    if (orginalCat != null)
                    {
                        cacParentTextCategory.EditValue = orginalCat.ParentId;
                        cacTypeTextCategory.EditValue = orginalCat.Type;
                    }

                    meRemarkTextCategory.Text = EditedRateCategory.Remark;
                    memoRatePolicy.Text = editedRateCategory.RatePolicy;
                }
                else
                {
                    deStartDateTextCategory.DateTime = CurrentTime;
                    deEndDateTextCategory.DateTime = CurrentTime;
                }

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing data. DETAIL:: " + ex.Message, "ERROR");
                return false;
            }
        }

        public void validateArrivalAndDepartureDate()
        {
            if (deEndDateTextCategory.IsModified || deStartDateTextCategory.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deEndDateTextCategory, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deStartDateTextCategory,
                    IsValidated=true
                },
                new ValidationInfo(deStartDateTextCategory, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deEndDateTextCategory,
                    IsValidated=true
                }
            };
                _invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (_invalidControls.Count > 0)
                    SystemMessage.ShowModalInfoMessage("The start date can not be greate than the end date!!!", "ERROR");
                return;
            }
        }

        #endregion 


        #region Event Handlers

        private void bbiReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            editMode = false;
            cacParentTextCategory.EditValue = "";
            cacTypeTextCategory.EditValue = "";
            teDescriptionTextCategory.Text = "";
            cacTypeTextCategory.EditValue = _defType;

            meRemarkTextCategory.Text = "";
            memoRatePolicy.Text = "";
            deStartDateTextCategory.DateTime = CurrentTime;
            deEndDateTextCategory.DateTime = CurrentTime;

        }


        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>();

                controls.Add(teDescriptionTextCategory);
                //  controls.Add(teCodeTextCategory);

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                    return;

                // Progress_Reporter.Show_Progress("Saving Rate Category");
                RateCategoryDTO rateCat = new RateCategoryDTO();
                rateCat.Description = teDescriptionTextCategory.Text;
                rateCat.StartDate = deStartDateTextCategory.DateTime;
                rateCat.EndDate = deEndDateTextCategory.DateTime;
                rateCat.ParentId = (cacParentTextCategory.EditValue != null && !string.IsNullOrEmpty(cacParentTextCategory.EditValue.ToString())) ? Convert.ToInt32(cacParentTextCategory.EditValue.ToString()) : null;
                rateCat.Type = (cacTypeTextCategory.EditValue != null && !string.IsNullOrEmpty(cacTypeTextCategory.EditValue.ToString())) ? Convert.ToInt32(cacTypeTextCategory.EditValue.ToString()) : null;
                rateCat.RatePolicy = memoRatePolicy.Text;
                rateCat.Remark = meRemarkTextCategory.Text;

                if (editMode)
                {
                    rateCat.Id = editedRateCategory.Id;
                    if (UIProcessManager.UpdateRateCategory(rateCat) != null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Rate Category Updated Successfully!", "MESSAGE");
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();

                    }
                }

                else
                {
                    //RateCategoryDTO savedCat = null;

                    //savedCat = UIProcessManager.GetRateCategoryById(Convert.ToInt32(teCodeTextCategory.Text));
                    //if (savedCat == null)
                    //{
                    if (UIProcessManager.CreateRateCategory(rateCat) != null)
                    {
                        ////CNETInfoReporter.Hide();
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        SystemMessage.ShowModalInfoMessage("Rate Category Created Successfully!", "MESSAGE");
                        this.Close();
                    }
                    //}

                }



            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving rate category. Detail:: " + ex.Message, "ERROR");

            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void deStartDateTextCategory_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deStartDateTextCategory.EditValue;
            DateTime dtEnDateTime = deEndDateTextCategory.DateTime;

            if (dtSDateTime > dtEnDateTime) validateArrivalAndDepartureDate();
        }

        private void deEndDateTextCategory_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deStartDateTextCategory.EditValue;
            DateTime dtEnDateTime = deEndDateTextCategory.DateTime;

            if (dtSDateTime > dtEnDateTime) validateArrivalAndDepartureDate();
        }

        private void cacParentTextCategory_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacTypeTextCategory_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void frmRateCategory_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion



    }
}
