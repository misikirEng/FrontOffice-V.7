using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.UI;
using CNET.FrontOffice_V._7;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using System.IO.Packaging;
using CNET.FrontOffice_V._7.Validation;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmPackageDetail : XtraForm
    {

        private List<WeekDayDTO> _cnetWeekDays = new List<WeekDayDTO>();

        private bool _isThisEdit;
        public DateTime CurrentTime { get; set; }
        private IList<Control> _invalidControls;


        /** properties **/
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

        private PackageDetailDTO _editedPackageDetail;
        internal PackageDetailDTO EditedPackageDetail
        {
            get { return _editedPackageDetail; }
            set
            {
                _editedPackageDetail = value;
                _isThisEdit = true;
            }
        }

        private PackageHeaderView _pkgHeader;
        public PackageHeaderView PackageHeader
        {
            get
            {
                return _pkgHeader;
            }
            set
            {
                _pkgHeader = value;
            }
        }

        /******************************* CONSTRUCTOR ****************************************/
        public frmPackageDetail()
        {
            InitializeComponent();
            InitializeUi();
        }

        #region Helper Methods

        public void InitializeUi()
        {
            Utility.AdjustRibbon(lciRibbonHolder);
            Utility.AdjustForm(this);
            Size = new Size(500, 410);
            deEndDatePostingDetail.Properties.MinValue = CurrentTime;
            //Location = new Point(450, 150);
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

                if (PackageHeader == null) return false;

                // Progress_Reporter.Show_Progress("Initializing data");

                if (EditedPackageDetail != null)
                {
                    EditedPackageDetail = UIProcessManager.GetPackageDetailById(_editedPackageDetail.Id);
                    if (EditedPackageDetail == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Unable to get package detail!", "ERROR");
                        return false;
                    }
                    if (EditedPackageDetail.Id > 0)
                    {
                        teCodePostingDetail.Text = EditedPackageDetail.Id.ToString();
                        deStartDatePostingDetail.EditValue = EditedPackageDetail.StartDate;
                        deEndDatePostingDetail.EditValue = EditedPackageDetail.EndingDate;
                        meRamark.Text = EditedPackageDetail.Remark;
                        if (EditedPackageDetail.Price != null)
                            tePricePostingDetail.Text = Math.Round((decimal)EditedPackageDetail.Price, 2).ToString();
                        if (EditedPackageDetail.Allowance != null)
                            teAllowancePostingDetail.Text = Math.Round((decimal)EditedPackageDetail.Allowance, 2).ToString();
                        meRamark.Text = EditedPackageDetail.Remark;


                        CheckAndDisableWeekDay(EditedPackageDetail.PackageHeader);
                        // Paint WeekDaysCheckList control
                        _cnetWeekDays = UIProcessManager.GetWeekDaysByReferenceandPointer(EditedPackageDetail.Id, CNETConstantes.TABLE_PACKAGE_DETAIL);
                        if (_cnetWeekDays != null)
                        {
                            wdceWeekdaysPostingDetail.WeekDays = PmsHelper.PaintWeekDaysCheckList(_cnetWeekDays);
                        }

                    }
                    else if (EditedPackageDetail.PackageHeader != null && EditedPackageDetail.PackageHeader > 0)
                    {
                        CheckAndDisableWeekDay(EditedPackageDetail.PackageHeader);
                    }
                }
                else
                {
                    teCodePostingDetail.Text = "";
                    //if (string.IsNullOrWhiteSpace(teCodePostingDetail.Text))
                    //{
                    //    ////CNETInfoReporter.Hide();
                    //    SystemMessage.ShowModalInfoMessage("Unable to generate packge detail code!", "ERROR");
                    //    return false;
                    //}
                    deStartDatePostingDetail.EditValue = CurrentTime;
                    deEndDatePostingDetail.EditValue = CurrentTime.AddDays(1);
                }


                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in intitializing data. Detail: " + ex.Message, "ERROR");
                return false;
            }
        }
        public void Save()
        {
            try
            {

                List<Control> controls = new List<Control>
                {
                     tePricePostingDetail
                };
                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                    return;

                // Progress_Reporter.Show_Progress("Saving Package Detail");

                PackageDetailDTO pDetail = new PackageDetailDTO
                {
                    PackageHeader = PackageHeader.Id,
                    StartDate = deStartDatePostingDetail.DateTime,
                    EndingDate = deEndDatePostingDetail.DateTime,
                    Remark = meRamark.Text
                };

                if (!string.IsNullOrEmpty(tePricePostingDetail.Text))
                {
                    pDetail.Price = Convert.ToDecimal(tePricePostingDetail.Text);
                }
                if (!string.IsNullOrEmpty(teAllowancePostingDetail.Text))
                {
                    pDetail.Allowance = Convert.ToDecimal(teAllowancePostingDetail.Text);
                }

                if (_isThisEdit)
                {
                    pDetail.Id = _editedPackageDetail.Id;
                    if (UIProcessManager.UpdatePackageDetail(pDetail) != null)
                    {
                        List<WeekDayDTO> weekdaylist = UIProcessManager.GetWeekDaysByReferenceandPointer(pDetail.Id, CNETConstantes.TABLE_PACKAGE_DETAIL);
                        if (weekdaylist != null)
                            weekdaylist.ForEach(x => UIProcessManager.DeleteWeekDayById(x.Id));
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Unable to update package detail!", "ERROR");
                        return;
                    }
                }
                else
                {
                    pDetail = UIProcessManager.CreatePackageDetail(pDetail);
                    if (pDetail == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to save package detail!", "ERROR");
                        return;
                    }
                }

                List<WeekDayDTO> cnetWeekDays;
                WeekDays weekDays = wdceWeekdaysPostingDetail.WeekDays;

                PmsHelper.GenerateWeekDays(weekDays, CNETConstantes.TABLE_PACKAGE_DETAIL, pDetail.Id, out cnetWeekDays);
                // Save WeekDays information
                foreach (WeekDayDTO wd in cnetWeekDays)
                {
                    UIProcessManager.CreateWeekDay(wd);
                }

                DialogResult = DialogResult.OK;
                ////CNETInfoReporter.Hide();
                if (_isThisEdit)
                {
                    SystemMessage.ShowModalInfoMessage("Package detail successfully Updated!", "MESSAGE");
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Package detail successfully saved!", "MESSAGE");
                }
                this.Close();
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.No;
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving package detail. Detail: " + ex.Message, "ERROR");
                return;
            }
        }
        private void ResetFields()
        {
            deStartDatePostingDetail.EditValue = CurrentTime;
            deEndDatePostingDetail.EditValue = CurrentTime;
            teCodePostingDetail.Text = String.Empty;
            if (string.IsNullOrWhiteSpace(teCodePostingDetail.Text))
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Unable to generate packge detail code!", "ERROR");
                this.Close();
            }
            wdceWeekdaysPostingDetail.WeekDays = WeekDays.EveryDay;
            teAllowancePostingDetail.Text = "0.0";
            tePricePostingDetail.Text = "0.0";

        }

        public void CheckAndDisableWeekDay(int? package)
        {
            if (package != null && package > 0)
            {
                List<PostingScheduleDTO> _postingScheduleList = UIProcessManager.GetPostingScheduleByPackageHeaderId(package.Value);

                WeekDays weekDays = new WeekDays();

                foreach (PostingScheduleDTO day in _postingScheduleList)
                {
                    if (day.RhythmValue == 2)
                    {
                        weekDays |= WeekDays.Monday;
                    }

                    if (day.RhythmValue == 4)
                    {
                        weekDays |= WeekDays.Tuesday;
                    }

                    if (day.RhythmValue == 8)
                    {
                        weekDays |= WeekDays.Wednesday;
                    }

                    if (day.RhythmValue == 16)
                    {
                        weekDays |= WeekDays.Thursday;
                    }

                    if (day.RhythmValue == 32)
                    {
                        weekDays |= WeekDays.Friday;
                    }

                    if (day.RhythmValue == 64)
                    {
                        weekDays |= WeekDays.Saturday;
                    }

                    if (day.RhythmValue == 1)
                    {
                        weekDays |= WeekDays.Sunday;
                    }

                    wdceWeekdaysPostingDetail.WeekDays = weekDays;
                }
                foreach (CheckEdit checkEdit in wdceWeekdaysPostingDetail.Controls)
                {
                    if (!checkEdit.Checked)
                    {
                        checkEdit.Enabled = false;
                    }
                }
            }
        }

        public void validateArrivalAndDepartureDate()
        {
            //_invalidControls.Count=0;
            if (deStartDatePostingDetail.IsModified || deEndDatePostingDetail.IsModified)
            {
                List<ValidationInfo> validationInfos = new List<ValidationInfo>
            {
                new ValidationInfo(deEndDatePostingDetail, CompareControlOperator.LessOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deStartDatePostingDetail,
                    IsValidated=true
                },
                new ValidationInfo(deStartDatePostingDetail, CompareControlOperator.GreaterOrEqual,
                    conditionOperator: ConditionOperator.IsNotBlank)
                {
                    Control = deEndDatePostingDetail,
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

        private void bbiReset_ItemClick(object sender, ItemClickEventArgs e)
        {
            ResetFields();
        }
        private void bbiClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }
        private void bbiSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            Save();
        }

        private void deStartDatePostingDetail_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deStartDatePostingDetail.EditValue;
            DateTime dtEnDateTime = deEndDatePostingDetail.DateTime;
            if (dtSDateTime > dtEnDateTime) deEndDatePostingDetail.DateTime = dtSDateTime.AddDays(1);
        }

        private void deEndDatePostingDetail_EditValueChanged(object sender, EventArgs e)
        {
            DateTime dtSDateTime = (DateTime)deStartDatePostingDetail.EditValue;
            DateTime dtEnDateTime = deEndDatePostingDetail.DateTime;

            if (dtSDateTime > dtEnDateTime) validateArrivalAndDepartureDate();
        }

        private void frmPackageDetail_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion



    }
}