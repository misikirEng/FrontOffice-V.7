using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using DevExpress.XtraScheduler;

using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.Validation;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmStrategyEditor : XtraForm//UILogicBase
    {
        private Boolean IsThisEdit;

        private List<WeekDayDTO> _cnetWeekDays = new List<WeekDayDTO>();
        private IList<Control> _invalidControls;


        private RateStrategyDTO _editedRateStrategy;
        private int _defCondition;
        private int _defRestrictionType;

        /** Proprties **/

        public DateTime CurrentTime { get; set; }
        public int RateHeaderCode { get; set; }
        //public string AdSyncout { get; set; }

        internal RateStrategyDTO EditedRateStrategy
        {
            get { return _editedRateStrategy; }
            set
            {
                _editedRateStrategy = value;
                IsThisEdit = true;


            }
        }

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


        /********* CONSTRUCTOR ***************/
        public frmStrategyEditor()
        {
            InitializeComponent();

            //  ApplyIcons();
            InitializeUI();
        }

        #region Helper Methods 

        public void InitializeUI()
        {
            Utility.AdjustForm(this);
            Utility.AdjustRibbon(lciRibbonHolder);
            Size = new Size(405, 520);
            Location = new Point(450, 150);

            //condition
            cacCondition.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacCondition.Properties.DisplayMember = "Description";
            cacCondition.Properties.ValueMember = "Id";

            //restriction
            cacRestrictionType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacRestrictionType.Properties.DisplayMember = "Description";
            cacRestrictionType.Properties.ValueMember = "Id";
        }

        public bool InitializeData()
        {
            try
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                {
                    return false;
                }
                CurrentTime = currentTime.Value;

                if (RateHeaderCode == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please Specify Rate Header! ", "ERROR");
                    return false;
                }

                // Progress_Reporter.Show_Progress("Initializing Data");

                List<SystemConstantDTO> condition = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.RATE_STRATEGY_CONDITION && l.IsActive).ToList();
                List<SystemConstantDTO> restrictionType = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.RATE_STRATEGY_RESTRICTION_TYPE && l.IsActive).ToList();


                cacCondition.Properties.DataSource = condition;
                if (cacCondition != null)
                {
                    var cond = condition.FirstOrDefault(c => c.IsDefault);
                    if (cond != null)
                    {
                        cacCondition.EditValue = (cond.Id);
                        _defCondition = cond.Id;
                    }
                }


                cacRestrictionType.Properties.DataSource = restrictionType;
                if (restrictionType != null)
                {
                    var rest = restrictionType.FirstOrDefault(c => c.IsDefault);
                    if (rest != null)
                    {
                        cacRestrictionType.EditValue = (rest.Id);
                        _defRestrictionType = rest.Id;
                    }
                }

                if (EditedRateStrategy != null)
                {
                    //   teDescription.Text = EditedRateStrategy.Description;
                    deFromDateRestriction.EditValue = EditedRateStrategy.StartDate;
                    deToDateRestriction.EditValue = EditedRateStrategy.EndDate;
                    deFromDateControl.EditValue = EditedRateStrategy.ControlBegin;
                    deToDateControl.EditValue = EditedRateStrategy.ControlEnd;
                    // ceIsActive.Checked = EditedRateStrategy.isactive;
                    cacRestrictionType.EditValue = EditedRateStrategy.RestrictionType;
                    cacCondition.EditValue = EditedRateStrategy.Condition;
                    teValue.EditValue = EditedRateStrategy.Value;
                    cbeValue.EditValue = EditedRateStrategy.IsPercent ? @"Percent (%)" : "Amount";
                    sePriority.Value = Convert.ToDecimal(EditedRateStrategy.Priority);
                    meRemark.Text = EditedRateStrategy.Remark;
                    if (EditedRateStrategy.Id != null)
                    {
                        // Paint WeekDaysCheckList control
                        _cnetWeekDays = UIProcessManager.GetWeekDaysByReferenceandPointer(EditedRateStrategy.Id, CNETConstantes.TABLE_RATE_STRATEGY);
                        wdceWeekdays.WeekDays = PmsHelper.PaintWeekDaysCheckList(_cnetWeekDays);
                    }
                }
                else
                {
                    // default date values
                    deFromDateControl.EditValue = CurrentTime;
                    deToDateControl.EditValue = CurrentTime;
                    deFromDateRestriction.EditValue = CurrentTime;
                    deToDateRestriction.EditValue = CurrentTime;
                }

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing data. Detail:: " + ex.Message, "ERROR");
                return false;
            }
        }

        private void ResetFields()
        {
            deFromDateControl.EditValue = CurrentTime;
            deToDateControl.EditValue = CurrentTime;
            deFromDateRestriction.EditValue = CurrentTime;
            deToDateRestriction.EditValue = CurrentTime;
            teDescription.Text = String.Empty;
            teValue.Text = String.Empty;
            meRemark.Text = String.Empty;
            cacCondition.EditValue = _defCondition;
            cacRestrictionType.EditValue = _defRestrictionType;
            ceIsActive.CheckState = CheckState.Checked;
            cbeValue.Reset();
            sePriority.Value = 1;
            wdceWeekdays.WeekDays = WeekDays.EveryDay;

        }

        private void Save()
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                    teDescription,
                    cacCondition,
                    cacRestrictionType,
                    cbeValue,
                    teValue
                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                    return;

                List<ValidationInfo> validationInfos = new List<ValidationInfo>
                {
                    new ValidationInfo(deToDateRestriction, CompareControlOperator.LessOrEqual,
                        conditionOperator: ConditionOperator.IsNotBlank)
                    {
                        Control = deFromDateRestriction,
                        IsValidated=true
                    },
                    new ValidationInfo(deFromDateRestriction, CompareControlOperator.GreaterOrEqual,
                        conditionOperator: ConditionOperator.IsNotBlank)
                    {
                        Control = deToDateRestriction,
                        IsValidated=true
                    },
                    new ValidationInfo(deToDateControl, CompareControlOperator.LessOrEqual,
                        conditionOperator: ConditionOperator.IsNotBlank)
                    {
                        Control = deFromDateControl,
                        IsValidated=true
                    },
                    new ValidationInfo(deFromDateControl, CompareControlOperator.GreaterOrEqual,
                        conditionOperator: ConditionOperator.IsNotBlank)
                    {
                        Control = deToDateControl,
                        IsValidated=true
                    }
                };

                invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (invalidControls.Count > 0)
                    return;

                // Progress_Reporter.Show_Progress("Saving Rate Strategy");


                RateStrategyDTO rateStrategy = new RateStrategyDTO
                {
                    // Description = teDescription.Text,
                    // Condition = Convert.ToInt32(cacCondition.EditValue.ToString()),
                    RestrictionType = Convert.ToInt32(cacRestrictionType.EditValue.ToString()),
                    IsPercent = (string)cbeValue.EditValue == @"Percent (%)",
                    Value = Convert.ToDecimal(teValue.EditValue),
                    StartDate = Convert.ToDateTime(deFromDateRestriction.EditValue),
                    EndDate = Convert.ToDateTime(deToDateRestriction.EditValue),
                    ControlBegin = Convert.ToDateTime(deFromDateControl.EditValue),
                    ControlEnd = Convert.ToDateTime(deToDateControl.EditValue),
                    Priority = Convert.ToInt32(sePriority.Value),
                    Remark = meRemark.Text,
                    // isActive = ceIsActive.Checked
                };

                RateCodeRateStrategyDTO rcrs = new RateCodeRateStrategyDTO
                {
                    RateCode = RateHeaderCode
                };

                // Check if it is on edit or create mode
                if (IsThisEdit)
                {
                    rateStrategy.Id = EditedRateStrategy.Id;
                    UIProcessManager.UpdateRateStrategy(rateStrategy);
                    // Delete all registered weekday per component and reference

                    List<WeekDayDTO> weekdaylist = UIProcessManager.GetWeekDaysByreference(rateStrategy.Id);
                    if (weekdaylist != null)
                        weekdaylist.ForEach(x => UIProcessManager.DeleteWeekDayById(x.Id));
                }
                else
                {

                    RateStrategyDTO savedrateStrate = UIProcessManager.CreateRateStrategy(rateStrategy);

                    rcrs.RateStrategy = savedrateStrate.Id;
                    UIProcessManager.CreateRateCodeRateStrategy(rcrs);

                }



                #region Save to WeekDays

                List<WeekDayDTO> cnetWeekDays;
                WeekDays weekDays = wdceWeekdays.WeekDays;

                PmsHelper.GenerateWeekDays(weekDays, CNETConstantes.TABLE_RATE_STRATEGY, rateStrategy.Id, out cnetWeekDays);

                // Save WeekDays information
                foreach (WeekDayDTO wd in cnetWeekDays)
                {
                    UIProcessManager.CreateWeekDay(wd);
                }



                #endregion

                DialogResult = DialogResult.OK;
                ////CNETInfoReporter.Hide();
                if (IsThisEdit)
                {
                    SystemMessage.ShowModalInfoMessage("Rate Strategy is updated!", "MESSAGE");
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Rate Strategy is saved!", "MESSAGE");
                }


                this.Close();

            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.No;
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving rate strategry. Detail:: " + ex.Message, "ERROR");
            }
        }

        public void validateArrivalAndDepartureDate()
        {
            //_invalidControls.Count=0;
            if (deToDateControl.IsModified || deFromDateControl.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deToDateControl, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deFromDateControl,
                    IsValidated=true
                },
                new ValidationInfo(deFromDateControl, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deToDateControl,
                    IsValidated=true
                }
            };
                _invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (_invalidControls.Count > 0)
                    SystemMessage.ShowModalInfoMessage("The start date can not be greate than the end date!!!", "ERROR");
                return;
            }
        }
        public void validateArrivalAndDepartureDateRestriction()
        {
            //_invalidControls.Count=0;
            if (deToDateRestriction.IsModified || deFromDateRestriction.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deToDateRestriction, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deFromDateRestriction,
                    IsValidated=true
                },
                new ValidationInfo(deFromDateRestriction, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deToDateRestriction,
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

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }
        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetFields();
        }

        private void frmStrategyEditor_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void cacCondition_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacRestrictionType_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void deFromDateRestriction_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deFromDateRestriction.EditValue;
            DateTime dtEnDateTime = deToDateRestriction.DateTime;
            if (dtSDateTime > dtEnDateTime) deToDateRestriction.DateTime = dtSDateTime.AddDays(1);
        }

        private void deToDateRestriction_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deFromDateRestriction.EditValue;
            DateTime dtEnDateTime = deToDateRestriction.DateTime;

            if (dtSDateTime > dtEnDateTime) validateArrivalAndDepartureDateRestriction();
        }

        private void deFromDateControl_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deFromDateControl.EditValue;
            DateTime dtEnDateTime = deToDateControl.DateTime;
            if (dtSDateTime > dtEnDateTime) deToDateControl.DateTime = dtSDateTime.AddDays(1);
        }

        private void deToDateControl_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deFromDateControl.EditValue;
            DateTime dtEnDateTime = deToDateControl.DateTime;

            if (dtSDateTime > dtEnDateTime) validateArrivalAndDepartureDate();
        }

        #endregion



    }
}