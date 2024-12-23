using CNET.ERP.Client.Common.UI.Library;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using CNET.ERP.ResourceProvider;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraScheduler;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET.FrontOffice_V._7.Validation;

namespace CNET.FrontOffice_V._7.Forms
{
    public partial class frmPackageHeader : XtraForm//UILogicBase

    //public partial class frmPackageHeader : XtraForm
    {
        private Boolean _isEditMode;
        private PackageHeaderView _editedPackageHeader;
        private List<PostingScheduleDTO> _postingScheduleList = new List<PostingScheduleDTO>();
        private int _postRhytLukDesc;

        private int _defGroup;
        private int _defType;
        private int _defRateApp;
        private int _defPostRhy;
        private int _defCalcRule;
        private int _defCurrency;

        //properties
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

        internal PackageHeaderView EditedPackageHeader
        {
            get { return _editedPackageHeader; }
            set
            {
                _editedPackageHeader = value;

                _isEditMode = true;
            }
        }

        /************************ CONSTRUCTOR **********************************/
        public frmPackageHeader()
        {
            InitializeComponent();
            InitializeUI();
            ApplyIcons();

            leHotel.Properties.DisplayMember = "Name";
            leHotel.Properties.ValueMember = "Id";
            leHotel.Properties.DataSource = LocalBuffer.LocalBuffer.HotelBranchBufferList.Select(x => new { x.Id, x.Name }).ToList();

        }
        public int selectedhotel { get; set; }

        #region Helper Methods 

        private void ApplyIcons()
        {
            Image Image = Provider.GetImage("New", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiNew.Glyph = Image;
            bbiNew.LargeGlyph = Image;

            Image = Provider.GetImage("Save", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiSave.Glyph = Image;
            bbiSave.LargeGlyph = Image;

            Image = Provider.GetImage("Delete", ProviderType.APPLICATIONICON, PictureSize.Dimension_32X32);

            bbiDelete.Glyph = Image;
            bbiDelete.LargeGlyph = Image;
        }

        public void InitializeUI()
        {
            Utility.AdjustRibbon(lciRibbonHolder);
            Utility.AdjustForm(this);
            Size = new Size(500, 500);
            Location = new Point(450, 150);

            //group
            cacGroup.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacGroup.Properties.DisplayMember = "Description";
            cacGroup.Properties.ValueMember = "Id";


            cacArticle.Properties.DisplayMember = "Name";
            cacArticle.Properties.ValueMember = "Id";

            //article
            GridColumn column = cacArticle.Properties.View.Columns.AddField("Id");
            column.Visible = false;
            column = cacArticle.Properties.View.Columns.AddField("LocalCode");
            column.Caption = "Code";
            column.Visible = true;
            column = cacArticle.Properties.View.Columns.AddField("Name");
            column.Visible = true;

            //currency
            cacCurrency.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacCurrency.Properties.DisplayMember = "Description";
            cacCurrency.Properties.ValueMember = "Id";

            //type
            cacType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacType.Properties.DisplayMember = "Description";
            cacType.Properties.ValueMember = "Id";

            //rate apperance
            cacRateAppearance.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacRateAppearance.Properties.DisplayMember = "Description";
            cacRateAppearance.Properties.ValueMember = "Id";

            //posting Rhythm
            cacPostingRhythm.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacPostingRhythm.Properties.DisplayMember = "Description";
            cacPostingRhythm.Properties.ValueMember = "Id";

            //calculation rule
            cacCalculationRule.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacCalculationRule.Properties.DisplayMember = "Description";
            cacCalculationRule.Properties.ValueMember = "Id";

        }

        public bool InitializeData()
        {
            try
            {


                // Progress_Reporter.Show_Progress("Initializing Data");

                #region Populate Lookups

                //group 
                List<LookupDTO> _packageGroup = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.PACKAGE_GROUP).ToList();
                cacGroup.Properties.DataSource = _packageGroup;

                if (_packageGroup != null)
                {
                    var defualtGroup = _packageGroup.FirstOrDefault(c => c.IsDefault);
                    if (defualtGroup != null)
                    {
                        cacGroup.EditValue = (defualtGroup.Id);
                        _defGroup = defualtGroup.Id;
                    }
                }

                //type
                List<LookupDTO> _type = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.PACKAGE_TYPE).ToList();
                cacType.Properties.DataSource = _type;

                if (_type != null)
                {
                    var defType = _type.FirstOrDefault(c => c.IsDefault);
                    if (defType != null)
                    {
                        _defType = defType.Id;
                        cacType.EditValue = (defType.Id);
                    }
                }

                //rate apperance
                List<LookupDTO> _rateAppearance = LocalBuffer.LocalBuffer.LookUpBufferList.Where(l => l.Type == CNETConstantes.PACKAGE_RATE_APPEARANCE).ToList();
                cacRateAppearance.Properties.DataSource = _rateAppearance;

                if (_rateAppearance != null)
                {
                    var defRateApp = _rateAppearance.FirstOrDefault(c => c.IsDefault);
                    if (defRateApp != null)
                    {
                        cacRateAppearance.EditValue = (defRateApp.Id);
                        _defRateApp = defRateApp.Id;
                    }
                }

                //posting rhytm
                List<SystemConstantDTO> _postingRhythm = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.PACKAGE_POSTING_RHYTHM).ToList();
                cacPostingRhythm.Properties.DataSource = _postingRhythm;
                if (_postingRhythm != null)
                {
                    var defPostRhy = _postingRhythm.FirstOrDefault(c => c.IsDefault);
                    if (defPostRhy != null)
                    {
                        _defPostRhy = defPostRhy.Id;
                        cacPostingRhythm.EditValue = (defPostRhy.Id);
                    }
                }

                //calculation rule
                List<SystemConstantDTO> _calculationRule = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.PACKAGE_CALCULATION_RATE).ToList();
                cacCalculationRule.Properties.DataSource = _calculationRule;
                if (_calculationRule != null)
                {
                    var defCalcRule = _calculationRule.FirstOrDefault(c => c.IsDefault);
                    if (defCalcRule != null)
                    {
                        cacCalculationRule.EditValue = (defCalcRule.Id);
                        _defCalcRule = defCalcRule.Id;
                    }
                }

                //currency
                List<CurrencyDTO> _currencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;
                if (_currencyList != null)
                {
                    cacCurrency.Properties.DataSource = (_currencyList.OrderByDescending(c => c.IsDefault).ToList());
                    var currency = _currencyList.FirstOrDefault(c => c.IsDefault);
                    if (currency != null)
                    {
                        cacCurrency.EditValue = (currency.Id);
                        _defCurrency = currency.Id;

                    }
                }


                List<ArticleDTO> articles = UIProcessManager.GetArticleByGSLType(CNETConstantes.PRODUCT).Where(a => a.IsActive && a.Preference == LocalBuffer.LocalBuffer.PACKAGE_PEREFERENCE).ToList();
                foreach (ArticleDTO art in articles)
                {
                    if (art.Preference > 0)
                    {
                        PreferenceDTO pref = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(p => p.Id == art.Preference);

                        art.Preference = pref == null ? 0 : pref.Id;
                    }
                }

                cacArticle.Properties.DataSource = articles;

                #endregion

                if (EditedPackageHeader != null)
                {
                    #region Populate Edited Pkg Header

                    PackageHeaderDTO pkgHeader = UIProcessManager.GetPackageHeaderById(EditedPackageHeader.Id);
                    if (pkgHeader == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Unable to get package header", "ERROR");
                        return false;
                    }

                    teCode.Text = pkgHeader.Id.ToString();
                    teDescription.Text = pkgHeader.Description;
                    cacArticle.EditValue = pkgHeader.Article;
                    cacGroup.EditValue = pkgHeader.PakageGroup;
                    cacRateAppearance.EditValue = pkgHeader.RateApperance;
                    cacPostingRhythm.EditValue = pkgHeader.PostingRhythm;
                    cacCalculationRule.EditValue = pkgHeader.CalculationRule;
                    cacCurrency.EditValue = pkgHeader.CurrencyPreference;
                    cacType.EditValue = pkgHeader.Type;
                    teFormula.Text = pkgHeader.Formula;
                    ceSaleSeparate.EditValue = pkgHeader.SellSeparet;
                    meRamark.Text = pkgHeader.Remark;

                    if (pkgHeader.PostingRhythm != null)
                    {
                        var luk = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == pkgHeader.PostingRhythm);
                        _postRhytLukDesc = luk == null ? 0 : luk.Id;
                    }

                    _postingScheduleList = UIProcessManager.GetPostingScheduleByPackageHeaderId(EditedPackageHeader.Id);
                    if (_postingScheduleList != null && _postingScheduleList.Count > 0)
                    {
                        switch (_postRhytLukDesc)
                        {
                            case CNETConstantes.CUSTOM_POSTING_BASED_ON_STAY:
                                foreach (PostingScheduleDTO schedule in _postingScheduleList)
                                {
                                    foreach (CheckedListBoxItem box in ccbeCustomPosting.Properties.GetItems())
                                    {
                                        if (schedule.RhythmValue != null && schedule.RhythmValue.Value == int.Parse(box.Value.ToString()))
                                        {
                                            box.CheckState = CheckState.Checked;
                                            break;
                                        }
                                    }
                                }

                                break;

                            case CNETConstantes.POST_ON_CERTAIN_NIGHTS_OF_THE_WEEK:

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

                                    wdcePostOnCertainNights.WeekDays = weekDays;
                                }

                                break;

                            case CNETConstantes.POST_ON_EVERY_X_NIGHTS_STARTING_NIGHT_Y:
                                var pSch = _postingScheduleList.FirstOrDefault();
                                teX.Text = pSch.RhythmValue.ToString();
                                teY.Text = pSch.Remark;

                                break;
                        }
                    }

                    #endregion

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

        private void ResetFields()
        {
            teDescription.Text = String.Empty;
            teFormula.Text = "";
            meRamark.Text = "";
            cacArticle.EditValue = "";
            cacGroup.EditValue = _defGroup;
            cacCurrency.EditValue = _defCurrency;
            cacType.EditValue = _defType;
            cacRateAppearance.EditValue = _defRateApp;
            cacPostingRhythm.EditValue = _defPostRhy;
            cacCalculationRule.EditValue = _defCalcRule;
            ceSaleSeparate.CheckState = CheckState.Unchecked;
            wdcePostOnCertainNights.WeekDays = WeekDays.EveryDay;
            teX.Text = String.Empty;
            teY.Text = String.Empty;
            ccbeCustomPosting.Properties.Items.Clear();

        }

        public void OnSave()
        {
            try
            {
                List<Control> controls = new List<Control>
                {
                     cacArticle,
                     cacRateAppearance,
                     teDescription,
                     cacPostingRhythm,
                     cacCurrency,
                     leHotel
                };
                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                    return;

                var ph = new PackageHeaderDTO
                {
                    Description = teDescription.Text,
                    PakageGroup = (cacGroup.EditValue == null || string.IsNullOrEmpty(cacGroup.EditValue.ToString())) ? null : Convert.ToInt32(cacGroup.EditValue),
                    Type = cacType.EditValue == null ? null : Convert.ToInt32(cacType.EditValue.ToString()),
                    Article = Convert.ToInt32(cacArticle.EditValue.ToString()),
                    CurrencyPreference = Convert.ToInt32(cacCurrency.EditValue.ToString()),
                    RateApperance = cacRateAppearance.EditValue == null ? null : Convert.ToInt32(cacRateAppearance.EditValue.ToString()),
                    PostingRhythm = (cacPostingRhythm.EditValue == null || string.IsNullOrEmpty(cacPostingRhythm.EditValue.ToString())) ? null : Convert.ToInt32(cacPostingRhythm.EditValue),
                    CalculationRule = cacCalculationRule.EditValue == null ? null : Convert.ToInt32(cacCalculationRule.EditValue.ToString()),
                    Formula = teFormula.Text,
                    Remark = meRamark.Text,
                    Consigneeunit = selectedhotel,
                    SellSeparet = ceSaleSeparate.Checked
                };

                // Progress_Reporter.Show_Progress("Saving Package Header");

                PackageHeaderDTO Savedpackage = null;

                if (_isEditMode)
                {
                    ph.Id = EditedPackageHeader.Id;
                    if (UIProcessManager.UpdatePackageHeader(ph) == null)
                    {
                        DialogResult = DialogResult.No;
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Unable to update package header!", "ERROR");
                        return;
                    }

                    List<PostingScheduleDTO> PostingScheduleList = UIProcessManager.GetPostingScheduleBypackageHeader(ph.Id);
                    if (PostingScheduleList != null)
                        PostingScheduleList.ForEach(x => UIProcessManager.DeletePostingScheduleById(x.Id));


                }
                else
                {
                    Savedpackage = UIProcessManager.CreatePackageHeader(ph);
                    if (Savedpackage == null)
                    {
                        DialogResult = DialogResult.No;
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Unable to save package header!", "ERROR");
                        return;
                    }
                }


                #region saving Posting Schedules

                List<PostingScheduleDTO> postingSchedules = new List<PostingScheduleDTO>();
                switch (Convert.ToInt32(cacPostingRhythm.EditValue))
                {
                    case CNETConstantes.CUSTOM_POSTING_BASED_ON_STAY:

                        postingSchedules.AddRange(from CheckedListBoxItem box in ccbeCustomPosting.Properties.GetItems()
                                                  where box.CheckState == CheckState.Checked
                                                  select new PostingScheduleDTO
                                                  {
                                                      PackageHeader = Savedpackage.Id,
                                                      RhythmValue = Convert.ToInt16(box.Value)
                                                  });

                        break;

                    case CNETConstantes.POST_ON_CERTAIN_NIGHTS_OF_THE_WEEK:

                        WeekDays weekDays = wdcePostOnCertainNights.WeekDays;
                        // 1
                        if (weekDays.HasFlag(WeekDays.Monday))
                        {
                            PostingScheduleDTO ps = new PostingScheduleDTO
                            {
                                PackageHeader = Savedpackage.Id,
                                RhythmValue = 2
                            };

                            postingSchedules.Add(ps);
                        }
                        // 2
                        if (weekDays.HasFlag(WeekDays.Tuesday))
                        {
                            PostingScheduleDTO ps = new PostingScheduleDTO
                            {
                                PackageHeader = Savedpackage.Id,
                                RhythmValue = 4
                            };

                            postingSchedules.Add(ps);
                        }
                        // 3
                        if (weekDays.HasFlag(WeekDays.Wednesday))
                        {
                            PostingScheduleDTO ps = new PostingScheduleDTO
                            {
                                PackageHeader = Savedpackage.Id,
                                RhythmValue = 8
                            };

                            postingSchedules.Add(ps);
                        }
                        // 4
                        if (weekDays.HasFlag(WeekDays.Thursday))
                        {
                            PostingScheduleDTO ps = new PostingScheduleDTO
                            {
                                PackageHeader = Savedpackage.Id,
                                RhythmValue = 16
                            };

                            postingSchedules.Add(ps);
                        }
                        // 5
                        if (weekDays.HasFlag(WeekDays.Friday))
                        {
                            PostingScheduleDTO ps = new PostingScheduleDTO
                            {
                                PackageHeader = Savedpackage.Id,
                                RhythmValue = 32
                            };

                            postingSchedules.Add(ps);
                        }
                        // 6
                        if (weekDays.HasFlag(WeekDays.Saturday))
                        {
                            PostingScheduleDTO ps = new PostingScheduleDTO
                            {
                                PackageHeader = Savedpackage.Id,
                                RhythmValue = 64
                            };

                            postingSchedules.Add(ps);
                        }
                        // 7
                        if (weekDays.HasFlag(WeekDays.Sunday))
                        {
                            PostingScheduleDTO ps = new PostingScheduleDTO
                            {
                                PackageHeader = Savedpackage.Id,
                                RhythmValue = 1
                            };

                            postingSchedules.Add(ps);
                        }

                        break;

                    case CNETConstantes.POST_ON_EVERY_X_NIGHTS_STARTING_NIGHT_Y:

                        PostingScheduleDTO postingSchedule = new PostingScheduleDTO
                        {
                            PackageHeader = Savedpackage.Id,
                            RhythmValue = Convert.ToInt32(teX.Text),
                            Remark = teY.Text
                        };

                        postingSchedules.Add(postingSchedule);

                        break;
                }

                foreach (PostingScheduleDTO ps in postingSchedules)
                {
                    UIProcessManager.CreatePostingSchedule(ps);
                }

                #endregion

                DialogResult = DialogResult.OK;
                ////CNETInfoReporter.Hide();
                if (_isEditMode)
                {
                    SystemMessage.ShowModalInfoMessage("Package header successfully Updated!", "MESSAGE");
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Package header successfully saved!", "MESSAGE");
                }
                this.Close();

            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.No;
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in saving package header. Detail: " + ex.Message, "ERROR");
            }
        }

        #endregion

        #region Event Handlers 

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnSave();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetFields();
        }

        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void cacPostingRhythm_EditValueChanged(object sender, EventArgs e)
        {
            if (cacPostingRhythm.EditValue == null || string.IsNullOrWhiteSpace(cacPostingRhythm.EditValue.ToString()))
            {
                return;
            }

            lciForPostOnCertainNights.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcgForPostOnEveryXNights.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcgForCustomPosting.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            switch (Convert.ToInt32(cacPostingRhythm.EditValue))
            {
                case CNETConstantes.CUSTOM_POSTING_BASED_ON_STAY:

                    lcgForCustomPosting.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    break;

                case CNETConstantes.POST_ON_CERTAIN_NIGHTS_OF_THE_WEEK:

                    lciForPostOnCertainNights.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    break;


                case CNETConstantes.POST_ON_EVERY_X_NIGHTS_STARTING_NIGHT_Y:

                    lcgForPostOnEveryXNights.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    break;
            }
        }

        private void cacArticle_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacCurrency_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacGroup_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacType_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacRateAppearance_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacPostingRhythm_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacCalculationRule_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void frmPackageHeader_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
            leHotel.EditValue = selectedhotel;
        }

        #endregion


    }
}
