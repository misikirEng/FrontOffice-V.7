using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Domain.CommonSchema;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmRateAdjustmnet : UILogicBase
    {
        private IList<Control> _invalidControls;
        private bool isEdit = false;
        private RateAdjustmentDTO rateAdj = new RateAdjustmentDTO();
        private List<RegistrationDetailDTO> regDetailList = null;
        private int adCode;

        private RegistrationDetailDTO _defRegDetail = null;

        private decimal minRateAdju = 0;

        /** Properties **/
        private RegistrationListVMDTO registrationExt;
        internal RegistrationListVMDTO RegistrationExt
        {
            get { return registrationExt; }
            set
            {
                registrationExt = value;

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

        /*****************************  CONSTRUCTOR  ****************************/
        public frmRateAdjustmnet()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeUI();

        }

        #region Helper Methods

        private void InitializeUI()
        {
            //Reason
            cacReason.Properties.Columns.Add(new LookUpColumnInfo("Description", "Reason"));
            cacReason.Properties.DisplayMember = "Description";
            cacReason.Properties.ValueMember = "Id";
        }

        private bool InitializeData()
        {
            try
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return false;

                if (RegistrationExt == null) return false;


                // Progress_Reporter.Show_Progress("Loading Data...");

                //check workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_RATE_ADJUSTED, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workFlow != null)
                {

                    adCode = workFlow.Id;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please define workflow of RATE ADJUSTED for Registration Voucher ", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adCode).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                    if (roleActivity != null)
                    {
                        frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                        frmNeedPass.ShowDialog();
                        if (!frmNeedPass.IsAutenticated)
                        {
                            ////CNETInfoReporter.Hide();
                            return false;
                        }

                    }

                }

                regDetailList = UIProcessManager.GetRegistrationDetailByvoucher(RegistrationExt.Id);
                if (regDetailList == null && regDetailList.Count == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Unable to get any registration detail!", "ERROR");
                    return false;
                }

                rgFactor.EditValue = "value";
                teRegistrationNo.Text = RegistrationExt.Registration;
                rateAdj = UIProcessManager.GetRateAdjustmentByvoucher(RegistrationExt.Id);
                if (rateAdj != null)
                {
                    rgFactor.EditValue = rateAdj.IsPercent ? "percent" : "value";
                    deThroughDate.DateTime = rateAdj.EndDate;
                    teFactor.Text = Math.Round(rateAdj.Amount, 2).ToString();
                    teValue.Text = Math.Round(rateAdj.Value, 2).ToString();


                    if (RegistrationExt.Arrival.Date > currentTime.Value.Date)
                    {
                        deFromReservationDate.Properties.MinValue = RegistrationExt.Arrival;
                        deThroughDate.Properties.MinValue = RegistrationExt.Arrival;
                        deFromReservationDate.DateTime = rateAdj.StartDate;
                    }
                    else
                    {
                        deFromReservationDate.Properties.MinValue = currentTime.Value;
                        deThroughDate.Properties.MinValue = currentTime.Value;
                        deFromReservationDate.DateTime = currentTime.Value;
                    }
                    deFromReservationDate.Properties.MaxValue = RegistrationExt.Departure;
                    deThroughDate.Properties.MaxValue = RegistrationExt.Departure;



                    isEdit = true;
                }
                else
                {
                    deThroughDate.DateTime = RegistrationExt.Departure;

                    if (RegistrationExt.Arrival.Date > currentTime.Value.Date)
                    {
                        deFromReservationDate.Properties.MinValue = RegistrationExt.Arrival;
                        deThroughDate.Properties.MinValue = RegistrationExt.Arrival;
                        deFromReservationDate.DateTime = RegistrationExt.Arrival;
                    }
                    else
                    {
                        deFromReservationDate.Properties.MinValue = currentTime.Value;
                        deThroughDate.Properties.MinValue = currentTime.Value;
                        deFromReservationDate.DateTime = currentTime.Value;
                    }
                    deFromReservationDate.Properties.MaxValue = RegistrationExt.Departure;
                    deThroughDate.Properties.MaxValue = RegistrationExt.Departure;

                }


                var config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_MinRateAdjustment);
                if (config != null)
                {
                    minRateAdju = Convert.ToDecimal(config.CurrentValue);
                }

                List<LookupDTO> reasonList = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.RATE_ADJUSTMENT_REASON).ToList();
                if (reasonList != null)
                {
                    cacReason.Properties.DataSource = (reasonList.OrderByDescending(c => c.IsDefault).ToList());
                    LookupDTO canLookup = reasonList.FirstOrDefault(c => c.IsDefault);
                    if (canLookup != null)
                        cacReason.EditValue = (canLookup.Id);
                }

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing data. Detail: " + ex.Message, "ERROR");
                return false;
            }
        }

        public void validateArrivalAndDepartureDate()
        {
            if (deThroughDate.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deThroughDate, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deFromReservationDate,
                    IsValidated=true
                },
                 new ValidationInfo(deFromReservationDate, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deThroughDate,
                    IsValidated=true
                }
            };
                _invalidControls = CustomValidationRule.Validate2(validationInfos);

                if (_invalidControls.Count > 0)
                    return;
            }
        }


        private void PopulateDataByStartDate(DateTime startDate)
        {
            var filtered = regDetailList.Where(r => r.Date >= startDate.Date && r.Date <= RegistrationExt.Departure.Date).ToList();

            if (filtered == null || filtered.Count == 0)
            {
                SystemMessage.ShowModalInfoMessage("No Registration Detail within the selected date!", "ERROR");
                bbiOK.Enabled = false;
                return;
            }
            else
            {
                bbiOK.Enabled = true;
            }

            //set current start time value
            if (rateAdj != null)
            {
                if (rateAdj.StartDate.Date == startDate.Date)
                {
                    teFactor.Text = Math.Round(rateAdj.Amount, 2).ToString();
                    teValue.Text = Math.Round(rateAdj.Value, 2).ToString();
                }
                else
                {
                    teFactor.Text = "";
                    teValue.Text = "";
                }
            }

            _defRegDetail = filtered.FirstOrDefault();
            teAmount.Text = _defRegDetail.RateAmount.ToString();

            //get today's rate code header
            //RateCodeDetail rateCode = UIProcessManager.SelectRateCodeDetail(regDetail.rateCodeDetail);
            RateCodeDetailDTO rateCode = UIProcessManager.GetRateCodeDetailById(_defRegDetail.RateCode.Value);
            if (rateCode != null)
            {
                var rateHeader = UIProcessManager.GetRateCodeHeaderById(rateCode.RateCodeHeader);
                teRateCode.Text = rateHeader == null ? "" : rateHeader.Description;
            }
        }

        #endregion

        #region Event Handlers
        private void bbiOK_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                    teValue,
                    deFromReservationDate,
                    deThroughDate,
                    cacReason
                };

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(teValue.Text))
                {
                    SystemMessage.ShowModalInfoMessage("You did not enter the adjustment value.", "ERROR");
                    return;
                }

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                {
                    return;
                }

                // Progress_Reporter.Show_Progress("Saving Rate Adjustment...");

                List<RegistrationDetailDTO> filtered = regDetailList.Where(r => r.Date >= deFromReservationDate.DateTime.Date && r.Date <= deThroughDate.DateTime.Date).ToList();
                List<DateTime> failedDates = new List<DateTime>();
                //bool canBeAdjusted = true;
                //foreach (var rd in filtered)
                //{
                //    var adjusted = Convert.ToDecimal(teValue.Text);
                //    var newRateMat = rd.rateAmount - adjusted;
                //    if (newRateMat < 0)
                //    {
                //        canBeAdjusted = false;
                //        break;
                //    }

                //}

                //if (!canBeAdjusted)
                //{
                //    SystemMessage.ShowModalInfoMessage("The adjusted value can't be greater than rate value!", "ERROR");
                //    return;
                //}

                bool isPercent = false;
                if (rgFactor.EditValue == "percent")
                {
                    isPercent = true;
                }

                foreach (var rd in filtered)
                {
                    if (isPercent)
                    {
                        rd.Adjustment = Convert.ToDecimal(teValue.Text);

                    }
                    else
                    {
                        rd.Adjustment = Convert.ToDecimal(teValue.Text);


                    }

                    rd.RateAmount = (Convert.ToDecimal(teAmount.Text));
                    if (UIProcessManager.UpdateRegistrationDetail(rd) == null)
                    {
                        failedDates.Add(rd.Date.Value);
                    }
                }

                if (failedDates.Count == filtered.Count)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Rate Adjustment is not succssfull!", "ERROR");
                    return;
                }

                if (failedDates.Count > 0)
                {
                    ////CNETInfoReporter.Hide();
                    StringBuilder sb = new StringBuilder();
                    foreach (var d in failedDates)
                    {
                        sb.Append(d.ToShortDateString());
                        sb.Append(", ");
                    }
                    XtraMessageBox.Show("WARNING: Rate Adjustment is failed for " + sb.ToString(), "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

                string remark = "";
                if (rgFactor.EditValue == "value")
                {
                    if (!string.IsNullOrEmpty(teValue.Text))
                    {
                        remark = "Amount: " + teValue.Text;
                    }
                }
                else if (rgFactor.EditValue == "percent")
                {
                    if (!string.IsNullOrEmpty(teFactor.Text))
                    {
                        remark = "Percent: " + teFactor.Text;
                    }
                }
                ActivityDTO activity = ActivityLogManager.SetupActivity(currentTime.Value, adCode, CNETConstantes.PMS_Pointer, remark);
                activity.Reference = RegistrationExt.Id;
                UIProcessManager.CreateActivity(activity);


                RateAdjustmentDTO rateAdjustment = new RateAdjustmentDTO();

                rateAdjustment.Voucher = RegistrationExt.Id;
                rateAdjustment.StartDate = deFromReservationDate.DateTime;
                rateAdjustment.EndDate = deThroughDate.DateTime;
                if (rgFactor.EditValue == "value")
                {
                    rateAdjustment.IsPercent = false;
                }
                else
                {
                    rateAdjustment.IsPercent = true;
                }
                if (!string.IsNullOrEmpty(teFactor.Text))
                {
                    rateAdjustment.Amount = Convert.ToDecimal(teFactor.Text);
                }
                else
                {
                    rateAdjustment.Amount = 0;
                }
                if (!string.IsNullOrEmpty(teValue.Text))
                {
                    rateAdjustment.Value = Convert.ToDecimal(teValue.Text);
                }
                else
                {
                    rateAdjustment.Value = 0;
                }

                rateAdjustment.Reason = cacReason.EditValue != null ? Convert.ToInt32(cacReason.EditValue) : null;
                rateAdjustment.Remark = "";

                bool isSaved = true;
                if (isEdit)
                {
                    rateAdjustment.Id = rateAdj.Id;
                    if (UIProcessManager.UpdateRateAdjustment(rateAdjustment) == null)
                    {
                        isSaved = false;
                    }
                }
                else
                {
                    if (UIProcessManager.CreateRateAdjustment(rateAdjustment) != null)
                    {
                        isSaved = false;
                    }
                }

                DialogResult = DialogResult.OK;
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Rate adjusted successfully.", "MESSAGE");
                this.Close();
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving rate adjustment. DETAIL: " + ex.Message, "ERROR");
            }
        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {

            if (rgFactor.EditValue == "value")
            {
                if (!string.IsNullOrEmpty(teFactor.Text) && _defRegDetail != null)
                {
                    decimal rateAmount = _defRegDetail.RateAmount.Value;
                    decimal factor = !string.IsNullOrEmpty(teFactor.Text) ? Convert.ToDecimal(teFactor.Text) : 0;
                    decimal adjustedAmt = rateAmount - factor;
                    teValue.Text = (Math.Round(factor, 2)).ToString();
                    teAmount.Text = Math.Round(adjustedAmt, 2).ToString();

                    if (adjustedAmt <= minRateAdju)
                    {
                        bbiOK.Enabled = false;
                    }
                    else
                    {
                        bbiOK.Enabled = true;
                    }

                }
            }
            else if (rgFactor.EditValue == "percent")
            {
                if (!string.IsNullOrEmpty(teFactor.Text) && _defRegDetail != null)
                {
                    decimal rateAmount = _defRegDetail.RateAmount.Value;
                    decimal factor = !string.IsNullOrEmpty(teFactor.Text) ? (Convert.ToDecimal(teFactor.Text) * rateAmount) / 100 : 0;
                    decimal adjustedAmt = rateAmount - factor;
                    teValue.Text = (Math.Round(factor, 2)).ToString();
                    teAmount.Text = Math.Round(adjustedAmt, 2).ToString();

                    if (adjustedAmt <= minRateAdju)
                    {
                        bbiOK.Enabled = false;
                    }
                    else
                    {
                        bbiOK.Enabled = true;
                    }
                }
            }
        }

        private void deFromReservationDate_EditValueChanged(object sender, EventArgs e)
        {
            if (deFromReservationDate.DateTime.Date > deThroughDate.DateTime.Date)
            {
                deThroughDate.DateTime = deFromReservationDate.DateTime.AddDays(1);
            }

            PopulateDataByStartDate(deFromReservationDate.DateTime);
        }

        private void deThroughDate_EditValueChanged(object sender, EventArgs e)
        {
            if (deFromReservationDate.DateTime.Date > deThroughDate.DateTime.Date)
            {
                validateArrivalAndDepartureDate();
            }
        }

        private void rgFactor_SelectedIndexChanged(object sender, EventArgs e)
        {
            teFactor.Text = "";
            teValue.Text = "";
            teAmount.Text = _defRegDetail == null ? "" : Math.Round(_defRegDetail.RateAmount.Value, 2).ToString();
        }

        private void frmRateAdjustmnet_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion


    }
}