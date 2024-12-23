using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7.PMS.Contracts;
using CNET.FrontOffice_V._7;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Domain.ArticleSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET.POS.Common.Models;
using CNET.POS.Settings;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET.FrontOffice_V._7.Validation;
using CNET.FP.Tool;
using System.Diagnostics;
using CNET_V7_Domain;
using CNET.Progress.Reporter;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Misc;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmManualCharge : UILogicBase
    {
        #region Declaration
        private bool UseCompany = false;
        private List<ArticleDTO> _AllextraArticles = null;
        private List<ArticleDTO> _roomChargeArticles = null;
        private List<ArticleDTO> _packageArticles = null;
        private List<ArticleDTO> _extraArticles = null;
        List<ValueDTO> articlePriceList = null;
        private decimal currentExRate = 1;
        private ChargeType currentChargeType;
        private int currentExtraVoucherType = CNETConstantes.CREDITSALES;
        private VoucherDTO voucher = null;
        private VoucherFinalDTO voFinal = null;
        private string chargeId = null, extraId = null;
        private List<ManualChargeVM> _manualChargeDtoList = new List<ManualChargeVM>();
        private int? _voucherExt = null;
        private string _currentFSNumber = string.Empty;
        //private Voucher_UI _voucherUIRoomCharge = new Voucher_UI(CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
        //private Voucher_UI _voucherUIExtraCredit = new Voucher_UI(CNETConstantes.CREDITSALES);
        //private Voucher_UI _voucherUIExtraCash = new Voucher_UI(CNETConstantes.CASH_SALES);
        //private VoucherSetting _currentSetting = null;
        private VoucherFinalCalculator voucherCalculator = new VoucherFinalCalculator();
        private CurrencyDTO defCurrency = null;
        private string[] _allChargeTypes = { "Room Charge", "Package Charge", "Extra Charge" };
        private string[] _allRoomChargeTypes = { "Room Charge", "Package Charge" };
        private string[] _allExtrachargeTypes = { "Extra Charge" };
        private string[] _PackagechargeTypes = { "Package Charge" };
        private bool isRoomCharge = true;
        private string customerTIN = "";
        private string GuestTIN = "";
        private bool _isTinMandatory = false;
        private string _companyCode = "";
        private List<Print_Item> _printItems = new List<Print_Item>();
        private Consignee customerDto = new Consignee();
        private decimal _additionalChargeAmt = 0;
        private bool _isAdditionalInPercent = false;
        private int _roundCalculatorDigit = 2;
        PMSVoucherSetting _currentSetting { get; set; }
        #endregion

        //////////////// CONSTRUCTOR /////////////////
        public frmManualCharge()
        {
            InitializeComponent();
            InitializeUI();

        }

        #region Properties 

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


        private RegistrationListVMDTO _regExt;
        public RegistrationListVMDTO RegExt
        {
            get
            {
                return _regExt;
            }
            set
            {
                _regExt = value;

            }
        }


        public bool IsLostCardFee { get; set; }
        public string LostCardArticleCode { get; set; }
        public bool IsRoomMove { get; set; }
        public decimal RoomMoveAmount { get; set; }
        public string RoomMoveArticle { get; set; }


        #endregion

        #region Methods

        public void InitializeUI()
        {
            //Currency
            lukCurrency.Properties.Columns.Add(new LookUpColumnInfo("Description", "Name"));
            lukCurrency.Properties.DisplayMember = "Description";
            lukCurrency.Properties.ValueMember = "Id";

            //Article
            GridColumn col = gv_articleViewGrid.Columns.AddVisible("Id", "Id");
            col.Visible = false;
            col = gv_articleViewGrid.Columns.AddVisible("LocalCode", "Code");
            col.Visible = true;
            col.Width = 20;
            col = gv_articleViewGrid.Columns.AddVisible("Name", "Name");
            col.Visible = true;
            sLuk_article.Properties.ValueMember = "Id";
            sLuk_article.Properties.DisplayMember = "Name";

            sLuk_article.EditValueChanged += sLuk_article_EditValueChanged;


            //Amount
            luk_amount.Properties.DisplayMember = "NewValue";
            luk_amount.Properties.ValueMember = "NewValue";

            //Windows
            luk_window.Properties.Columns.Add(new LookUpColumnInfo("Description", "Window"));
            luk_window.Properties.DisplayMember = "Description";
            luk_window.Properties.ValueMember = "Id";

            comboTranType.Enabled = false;
        }

        private void SetupChargeTypeItems()
        {
            combo_chargeType.Properties.Items.Clear();

            if (isRoomCharge && gv_addedLineItems.RowCount > 0)
            {
                combo_chargeType.Properties.Items.AddRange(_allRoomChargeTypes);
            }
            else if (!isRoomCharge && gv_addedLineItems.RowCount > 0)
            {
                if (RegExt.NoPost == true)
                {
                    combo_chargeType.Properties.Items.AddRange(_PackagechargeTypes);

                }
                else
                {
                    combo_chargeType.Properties.Items.AddRange(_allExtrachargeTypes);
                }
            }
            else
            {
                if (RegExt.NoPost == true)
                {
                    combo_chargeType.Properties.Items.AddRange(_allRoomChargeTypes);
                }
                else
                {
                    combo_chargeType.Properties.Items.AddRange(_allChargeTypes);
                }
            }
        }

        private void CalculateTotal(decimal exAmount)
        {
            try
            {
                if (luk_amount.EditValue != null && !string.IsNullOrWhiteSpace(luk_amount.EditValue.ToString()))
                {

                    decimal amount = Convert.ToDecimal(luk_amount.EditValue.ToString());
                    if (amount < 0 && currentChargeType != ChargeType.ROOM_CHARGE) amount = 0;
                    int qty = Convert.ToInt32(spEdit_qty.EditValue.ToString());
                    decimal total = Math.Round(amount * qty * exAmount, 2);
                    teTotal.Text = string.Format("{0:f2}", total);


                }
                else
                {
                    teTotal.Text = "0.0";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error! DETAIL::" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                teTotal.Text = "0.0";
            }
        }

        private void CalculateLineItem(int articleId, int quantity, decimal uAmount, decimal tAmount, string articleName)
        {
            try
            {



                // Progress_Reporter.Show_Progress("Adding line item", "Please Wait...");
                if (voucher == null)
                {
                    voucher = new VoucherDTO();
                    voucher.Code = teNumber.Text;
                    voucher.Type = CNETConstantes.TRANSACTIONTYPENORMALTXN;//what type of voucher this would?
                    if (UseCompany)
                    {
                        voucher.Consignee1 = RegExt.CompanyId;
                    }
                    else
                    {
                        voucher.Consignee1 = RegExt.GuestId;
                    }
                    voucher.IsIssued = true;
                    voucher.IsVoid = false;


                }

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null) return;

                voucher.IssuedDate = CurrentTime.Value;
                voucher.Year = CurrentTime.Value.Date.Year;
                voucher.Month = CurrentTime.Value.Date.Month;
                voucher.Day = CurrentTime.Value.Date.Day;

                if (currentChargeType == ChargeType.EXTRA_CHARGE)
                {
                    voucher.Definition = currentExtraVoucherType;
                    if (voucher.Definition == CNETConstantes.CREDITSALES)
                    {
                        POS_Settings.Voucher_Definition = CNETConstantes.CREDITSALES;
                        PMSVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.CREDITSALES);
                    }
                    else
                    {
                        POS_Settings.Voucher_Definition = CNETConstantes.CASH_SALES;
                        PMSVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.CASH_SALES);
                    }
                }
                else
                {
                    voucher.Definition = CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER;
                    POS_Settings.Voucher_Definition = CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER;
                    PMSVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
                }




                TaxDTO tax = CommonLogics.GetApplicableTax(RegExt.Id, voucher.Definition, voucher.Consignee1, articleId);
                if (!string.IsNullOrEmpty(tax.Remark))
                {
                    SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                    return;
                }

                lciVAT.Text = tax.Description + " (" + (Math.Round(tax.Amount, 2)) + "%)";


                ManualChargeVM dto = new ManualChargeVM()
                {
                    Description = articleName
                };

                //Line Item Calculator
                //Line Item Detail Calculator 
                LineItemDTO lineItem = new LineItemDTO();
                lineItem.Article = articleId;
                //lineItem.Voucher = voucher.Code;
                lineItem.Quantity = quantity;
                lineItem.UnitAmount = uAmount * currentExRate;
                lineItem.TotalAmount = tAmount;
                lineItem.Tax = tax.Id;
                lineItem.ObjectState = null;
                //New Line Item Object
                LineItemBuffer newLineItem = new LineItemBuffer();
                newLineItem.LineItem = lineItem;

                bool isServiceChargeIncluded = true;
                if (voucher.Definition == CNETConstantes.CREDITSALES || voucher.Definition == CNETConstantes.CASH_SALES)
                {
                    isServiceChargeIncluded = false;
                }
                NewLineItemCaculator lineItemCalculator = new NewLineItemCaculator();
                LineItemDetailPMS liDetail = lineItemCalculator.LineItemDetailCalculatorVoucher(new VoucherBuffer() { Voucher = voucher }, newLineItem.LineItem, RegExt.Id, null, null, null, null, false, isServiceChargeIncluded, false, false);
                if (liDetail != null)
                {
                    LineItemDTO li = liDetail.lineItem;

                    if (li == null)
                    {
                        ////CNETInfoReporter.Hide();
                        return;
                    }

                    dto.Code = li.Article;
                    dto.Quantity = Convert.ToDecimal(li.Quantity);
                    dto.tax = li.Tax == null ? 0 : li.Tax.Value;
                    dto.taxableAmount = li.TaxableAmount.Value;
                    dto.taxAmount = li.TaxAmount == null ? 0 : voucherCalculator.RoundValue(li.TaxAmount.Value, 2);
                    dto.unitAmt = li.UnitAmount == null ? 0 : voucherCalculator.RoundValue(li.UnitAmount, 2);
                    dto.totalAmount = li.TotalAmount == null ? 0 : voucherCalculator.RoundValue(li.TotalAmount, 2);

                    dto.LineItem = li;
                    dto.LineItem.ObjectState = null;
                    dto.LineItemValueFactor = liDetail.lineItemValueFactor;

                }

                dto.LiDetails = new LineItemDetails()
                {
                    lineItems = liDetail.lineItem,
                    lineItemValueFactor = liDetail.lineItemValueFactor
                };

                //for already added article, remove the old one
                var addedDto = _manualChargeDtoList.FirstOrDefault(d => d.Code == dto.Code);
                if (addedDto != null)
                {
                    _manualChargeDtoList.Remove(addedDto);
                }

                _manualChargeDtoList.Add(dto);
                gc_addedLineItems.DataSource = _manualChargeDtoList;
                gv_addedLineItems.RefreshData();

                //Calculate Grand Total
                CalculatGrandTotal(_manualChargeDtoList);

                ResetArticleArea();

                //add or remove charge type items based on the type of selected charge type and the number of items in the grid
                SetupChargeTypeItems();

                ////CNETInfoReporter.Hide();
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in adding line item. DETAIL::" + ex.Message, "ERROR");
                ////CNETInfoReporter.Hide();
            }
        }

        private void CalculatGrandTotal(List<ManualChargeVM> addedCharges)
        {
            List<LineItemDetails> lineItems = new List<LineItemDetails>();
            foreach (var dto in addedCharges)
            {
                lineItems.Add(dto.LiDetails);
            }
            //voFinal = UIProcessManager.CalculatedVoucher(voucher, lineItems, false, null);
            voFinal = voucherCalculator.VoucherCalculation(voucher, lineItems);
            _currentSetting = null;

            if (voFinal != null)
            {
                decimal? subtotal = voFinal.voucher == null ? 0 : voFinal.voucher.SubTotal;
                decimal? additionalCharge = voFinal.voucher == null ? 0 : voFinal.voucher.AddCharge;
                decimal? discount = voFinal.voucher == null ? 0 : voFinal.voucher.Discount;
                decimal vat = addedCharges.Sum(a => a.taxAmount);

                decimal grandTotal = voFinal.voucher == null ? 0 : voFinal.voucher.GrandTotal;



                teSubtotal.Text = subtotal == null ? "0.0" : Math.Round(subtotal.Value, 2).ToString();
                te_additionalCharge.Text = additionalCharge == null ? "0.0" : Math.Round(additionalCharge.Value, 2).ToString();
                teDiscount.Text = discount == null ? "0.0" : Math.Round(discount.Value, 2).ToString();
                teVat.Text = Math.Round(vat, 2).ToString();
                te_grandTotal.Text = Math.Round(grandTotal, 2).ToString();

                POS_Settings.TotalDiscount = discount == null ? 0 : discount.Value;
                POS_Settings.TotalServiceCharge = additionalCharge == null ? 0 : additionalCharge.Value;
            }
        }

        public bool InitializeData()
        {
            try
            {
                if (RegExt == null) return false;

                teRegistration.Text = RegExt.Registration;
                teRoom.Text = RegExt.Room;
                beiGuest.EditValue = RegExt.GuestId;
                beiCompany.EditValue = RegExt.Company;

                //Check Voucher Setting
                //bool roomChargeFlag = _voucherUIRoomCharge.IsVoucherCreated();
                //if (!roomChargeFlag) return false;


                //bool extraFlagCredit = _voucherUIExtraCredit.IsVoucherCreated();
                //if (!extraFlagCredit) return false;

                //bool extraFlagCash = _voucherUIExtraCash.IsVoucherCreated();
                //if (!extraFlagCash) return false;

                // Progress_Reporter.Show_Progress("Initializing manual charge", "Please Wait...");


                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return false;

                }
                if (RegExt.Departure.Date < CurrentTime.Value.Date)
                {
                    ////CNETInfoReporter.Hide();
                    XtraMessageBox.Show("You can't make manual charge for over-due outs!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (RegExt.NoPost == true)
                {
                    combo_chargeType.Properties.Items.Clear();

                    combo_chargeType.Properties.Items.AddRange(_allRoomChargeTypes);
                }


                teDate.Text = CurrentTime.Value.ToShortDateString();

                //TIN Number

                if (RegExt.CompanyId != null)
                {
                    ConsigneeDTO Company = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExt.CompanyId);
                    if (Company != null)
                    {
                        customerTIN = Company.Tin;
                        beiCompanyTIN.EditValue = customerTIN;
                    }
                }


                if (RegExt.GuestId != null)
                {
                    ConsigneeDTO Guest = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == RegExt.GuestId);
                    if (Guest != null)
                    {
                        GuestTIN = Guest.Tin;
                    }

                }

                //Read TIN Mandatory Configuration
                var tinConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == LocalBuffer.LocalBuffer.CurrentDevice.Id.ToString() && c.Attribute.ToLower() == "company tin mandatory");
                if (tinConfig != null && !string.IsNullOrEmpty(tinConfig.CurrentValue))
                {
                    _isTinMandatory = Convert.ToBoolean(tinConfig.CurrentValue);
                    beiMandatoryTIN.EditValue = _isTinMandatory;
                }

                //currency
                List<CurrencyDTO> currencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;
                lukCurrency.Properties.DataSource = currencyList;
                if (currencyList != null)
                {
                    defCurrency = currencyList.FirstOrDefault(c => c.IsDefault == true);
                    if (defCurrency != null)
                    {
                        lukCurrency.EditValue = defCurrency.Id;

                        //Get Exchange Rate
                        currentExRate = CommonLogics.GetLatestExchangeRate(defCurrency.Id);
                    }
                }

                //populate window lookups
                List<SystemConstantDTO> windowsList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.SPLIT_WINDOWS && l.IsActive).ToList();
                if (windowsList != null && windowsList.Count != 0)
                {
                    luk_window.Properties.DataSource = windowsList;
                    luk_window.EditValue = CNETConstantes.DEFAULT_WINDOW;

                }



                GetPreferenceSetting();







                //voucher extension
                SetVoucherExtentions(CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);

                //for Lost-Card fee, calculate lineitem and  disable some buttons
                if (IsLostCardFee)
                {
                    btn_add.Enabled = false;
                    btn_remove.Enabled = false;
                    combo_chargeType.Enabled = false;
                    combo_chargeType.SelectedIndex = 2;
                    comboTranType.Enabled = true;
                    lukCurrency.Enabled = false;
                    teRemark.Text = "Lost card fee";
                    if (!string.IsNullOrEmpty(LostCardArticleCode))
                    {
                        int lostarticlecode = 0;

                        Int32.TryParse(LostCardArticleCode.ToLower(), out lostarticlecode);
                        ArticleDTO _lostCardArticle = UIProcessManager.GetArticleById(lostarticlecode);
                        if (_lostCardArticle == null)
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Please set lost card article!", "ERROR");
                            return false;
                        }
                        decimal amount = -1;
                        //get default price
                        //decimal amount = -1;
                        //articlePriceList = UIProcessManager.GetValueByArticle(lostarticlecode);
                        //if (articlePriceList != null)
                        //{
                        //    var defPrice = articlePriceList.FirstOrDefault(p => p.IsDefault);
                        //    if (defPrice != null)
                        //        amount = defPrice.priceValue;
                        //}


                        if (_lostCardArticle.DefaultValue != null)
                            amount = _lostCardArticle.DefaultValue.Value;
                        if (amount <= 0)
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Please set price of Lost-Card article!", "ERROR");
                            return false;
                        }
                        CalculateLineItem(lostarticlecode, 1, amount, amount, _lostCardArticle.Name);

                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Please set lost card article!", "ERROR");
                        return false;
                    }
                }
                else if (IsRoomMove)
                {
                    combo_chargeType.Enabled = false;
                    combo_chargeType.SelectedIndex = 1;
                    comboTranType.Enabled = true;
                    lukCurrency.Enabled = false;
                    teRemark.Text = "charge for moving to new room type";

                    //room charge Articles. This is the default articles
                    _roomChargeArticles = UIProcessManager.GetArticleByGSLType(CNETConstantes.SERVICES).Where(a => a.IsActive && a.Preference == LocalBuffer.LocalBuffer.ACCOMODATION_PREFERENCE_CODE).ToList();

                    if (_roomChargeArticles != null && _roomChargeArticles.Count > 0 && useassignedArticleRoomCharge && assignedArticleRoomChargePreference != null && assignedArticleRoomChargePreference.Count > 0)
                        _roomChargeArticles = _roomChargeArticles.Where(x => assignedArticleRoomChargePreference.Contains(x.Preference)).ToList();

                    sLuk_article.Properties.DataSource = _roomChargeArticles;
                    gv_articleViewGrid.RefreshData();


                    if (!string.IsNullOrEmpty(RoomMoveArticle))
                    {
                        int Roommovearticlecode = 0;

                        Int32.TryParse(RoomMoveArticle.ToLower(), out Roommovearticlecode);
                        sLuk_article.EditValue = RoomMoveArticle;

                        ArticleDTO roomMoveArticle = UIProcessManager.GetArticleById(Roommovearticlecode);
                        if (roomMoveArticle == null)
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("Please set Room Move article!", "ERROR");
                            return false;
                        }

                        //get default price
                        decimal amount = RoomMoveAmount;

                        if (amount <= 0)
                        {
                            ////CNETInfoReporter.Hide();
                            SystemMessage.ShowModalInfoMessage("There is no price to charge!", "ERROR");
                            return false;
                        }

                        CalculateLineItem(Roommovearticlecode, 1, amount, amount, roomMoveArticle.Name);

                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please set Room Move article!", "ERROR");
                        return false;
                    }
                }
                else
                {

                    //room charge Articles. This is the default articles
                    _roomChargeArticles = UIProcessManager.GetArticleByGSLType(CNETConstantes.SERVICES).Where(a => a.IsActive && a.Preference == LocalBuffer.LocalBuffer.ACCOMODATION_PREFERENCE_CODE).ToList();


                    if (_roomChargeArticles != null && _roomChargeArticles.Count > 0 && useassignedArticleRoomCharge)
                    {
                        if (assignedArticleRoomChargePreference != null && assignedArticleRoomChargePreference.Count > 0)
                        {
                            _roomChargeArticles = _roomChargeArticles.Where(x => assignedArticleRoomChargePreference.Contains(x.Preference)).ToList();
                        }
                        else
                            _roomChargeArticles = new List<ArticleDTO>();
                    }
                    sLuk_article.Properties.DataSource = _roomChargeArticles;
                    gv_articleViewGrid.RefreshData();

                    //package articles
                    _packageArticles = UIProcessManager.GetArticleByGSLType(CNETConstantes.PRODUCT).Where(a => a.IsActive && a.Preference == LocalBuffer.LocalBuffer.PACKAGE_PEREFERENCE).ToList();


                    if (_packageArticles != null && _packageArticles.Count > 0 && useassignedArticleRoomCharge)
                    {
                        if (assignedArticleRoomChargePreference != null && assignedArticleRoomChargePreference.Count > 0)
                            _packageArticles = _packageArticles.Where(x => assignedArticleRoomChargePreference.Contains(x.Preference)).ToList();
                        else
                            _packageArticles = new List<ArticleDTO>();
                    }

                    _extraArticles = new List<ArticleDTO>();
                    //extra articles
                    _AllextraArticles = UIProcessManager.GetArticleByGSLType(CNETConstantes.PRODUCT).Where(a => a.IsActive && a.Preference != LocalBuffer.LocalBuffer.PACKAGE_PEREFERENCE).ToList();
                    if (_AllextraArticles == null)
                        _AllextraArticles = new List<ArticleDTO>();


                    _AllextraArticles.AddRange(UIProcessManager.GetArticleByGSLType(CNETConstantes.SERVICES).Where(a => a.IsActive && a.Preference != LocalBuffer.LocalBuffer.ACCOMODATION_PREFERENCE_CODE).ToList());

                    _extraArticles.AddRange(_AllextraArticles);

                    if (comboTranType.SelectedIndex == 0)
                    {
                        if (_AllextraArticles != null && _AllextraArticles.Count > 0 && useassignedArticleCreditSales)
                        {
                            if (assignedArticleCreditSalesPreference != null && assignedArticleCreditSalesPreference.Count > 0)
                            {
                                _extraArticles = _AllextraArticles.Where(x => assignedArticleCreditSalesPreference.Contains(x.Preference)).ToList();
                            }
                            else
                                _extraArticles = new List<ArticleDTO>();
                        }
                        else
                        {
                            _extraArticles = new List<ArticleDTO>();

                            if (_AllextraArticles != null && _AllextraArticles.Count > 0)
                                _extraArticles.AddRange(_AllextraArticles);
                        }

                    }
                    else
                    {
                        if (_AllextraArticles != null && _AllextraArticles.Count > 0 && useassignedArticleCashSales)
                        {
                            if (assignedArticleCashSalesPreference != null && assignedArticleCashSalesPreference.Count > 0)
                            {
                                _extraArticles = _AllextraArticles.Where(x => assignedArticleCashSalesPreference.Contains(x.Preference)).ToList();
                            }
                            else
                                _extraArticles = new List<ArticleDTO>();
                        }
                        else
                        {
                            _extraArticles = new List<ArticleDTO>();

                            if (_AllextraArticles != null && _AllextraArticles.Count > 0)
                                _extraArticles.AddRange(_AllextraArticles);
                        }

                    }

                }
                ////CNETInfoReporter.Hide();
                return true;

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing manual charge. DETAIL:: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ////CNETInfoReporter.Hide();
                return false;
            }
        }


        bool useassignedArticleRoomCharge { get; set; }
        bool useassignedArticleCashSales { get; set; }
        bool useassignedArticleCreditSales { get; set; }
        List<int> assignedArticleRoomChargePreference { get; set; }
        List<int> assignedArticleCashSalesPreference { get; set; }
        List<int> assignedArticleCreditSalesPreference { get; set; }

        List<PreferenceAccessDTO> AssignedPreferenceAccess { get; set; }
        private void GetPreferenceSetting()
        {
            try
            {
                ConfigurationDTO useassignedArticle = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Reference == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER.ToString() && x.Attribute == "Use Only Assigned Article");
                if (useassignedArticle != null && !string.IsNullOrEmpty(useassignedArticle.CurrentValue))
                    useassignedArticleRoomCharge = bool.Parse(useassignedArticle.CurrentValue);

                useassignedArticle = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Reference == CNETConstantes.CASH_SALES.ToString() && x.Attribute == "Use Only Assigned Article");
                if (useassignedArticle != null && !string.IsNullOrEmpty(useassignedArticle.CurrentValue))
                    useassignedArticleCashSales = bool.Parse(useassignedArticle.CurrentValue);


                useassignedArticle = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Reference == CNETConstantes.CREDITSALES.ToString() && x.Attribute == "Use Only Assigned Article");
                if (useassignedArticle != null && !string.IsNullOrEmpty(useassignedArticle.CurrentValue))
                    useassignedArticleCreditSales = bool.Parse(useassignedArticle.CurrentValue);


                if (useassignedArticleRoomCharge)
                {
                    AssignedPreferenceAccess = UIProcessManager.GetPreferenceAccessByvoucherDfnanddevice(CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, LocalBuffer.LocalBuffer.CurrentDevice.Id);
                    if (AssignedPreferenceAccess != null && AssignedPreferenceAccess.Count > 0)
                        assignedArticleRoomChargePreference = AssignedPreferenceAccess.Select(x => x.Preference).ToList();
                }

                if (useassignedArticleCashSales)
                {
                    AssignedPreferenceAccess = UIProcessManager.GetPreferenceAccessByvoucherDfnanddevice(CNETConstantes.CASH_SALES, LocalBuffer.LocalBuffer.CurrentDevice.Id);
                    if (AssignedPreferenceAccess != null && AssignedPreferenceAccess.Count > 0)
                        assignedArticleCashSalesPreference = AssignedPreferenceAccess.Select(x => x.Preference).ToList();
                }

                if (useassignedArticleCreditSales)
                {
                    AssignedPreferenceAccess = UIProcessManager.GetPreferenceAccessByvoucherDfnanddevice(CNETConstantes.CREDITSALES, LocalBuffer.LocalBuffer.CurrentDevice.Id);
                    if (AssignedPreferenceAccess != null && AssignedPreferenceAccess.Count > 0)
                        assignedArticleCreditSalesPreference = AssignedPreferenceAccess.Select(x => x.Preference).ToList();
                }

            }
            catch (Exception io)
            {
                XtraMessageBox.Show("GetPreferenceAccess Error " + Environment.NewLine + io.Message, "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AddArticlePrice(string value)
        {
            if (value != null && !string.IsNullOrWhiteSpace(value))
            {
                //Pricedescription,priceValue
                ValueDTO userDefPrice = new ValueDTO()
                {
                    NewValue = Convert.ToDecimal(value)
                };
                if (articlePriceList == null)
                {
                    articlePriceList = new List<ValueDTO>() { userDefPrice };
                }

                if (!articlePriceList.Any(p => p.NewValue == userDefPrice.NewValue))
                {

                    articlePriceList.Add(userDefPrice);
                    luk_amount.Properties.DataSource = articlePriceList;
                    luk_amount.EditValue = articlePriceList.LastOrDefault().NewValue;


                }



            }
        }

        private string GenerateIdByChargeType(ChargeType chargeType, int value, int voucherDef)
        {
            try
            {
                string currentVoCode = null;
                if (chargeType == ChargeType.PACKAGE_CHARGE || chargeType == ChargeType.ROOM_CHARGE)
                {
                    currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, value, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (!string.IsNullOrEmpty(currentVoCode))
                    {
                        teNumber.Text = currentVoCode;
                        chargeId = currentVoCode;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting For " + CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER + " !!!", "ERROR");
                        this.Close();
                    }
                }
                else if (chargeType == ChargeType.EXTRA_CHARGE)
                {
                    currentVoCode = UIProcessManager.IdGenerater("Voucher", voucherDef, value, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (!string.IsNullOrEmpty(currentVoCode))
                    {
                        teNumber.Text = currentVoCode;
                        extraId = currentVoCode;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem on id setting For " + voucherDef + " !!!", "ERROR");

                        this.Close();
                    }
                }

                return currentVoCode;
            }
            catch (Exception ex)
            {
                return null;
            }



        }

        private void ResetArticleArea()
        {
            sLuk_article.EditValue = null;
            spEdit_qty.EditValue = 1;
            teTotal.Text = "0.0";
            luk_amount.EditValue = null;
        }

        private void ResetAll()
        {
            ResetArticleArea();

            if (defCurrency != null)
            {
                lukCurrency.EditValue = defCurrency.Id;
            }


            teSubtotal.Text = "0.0";
            te_additionalCharge.Text = "0.0";
            teVat.Text = "0.0";
            te_grandTotal.Text = "0.0";
            teDiscount.Text = "0.0";
            teRemark.Text = "";
            teReference.Text = "";

            luk_window.EditValue = CNETConstantes.DEFAULT_WINDOW;

            //Remove Line Items
            _manualChargeDtoList.Clear();
            gc_addedLineItems.DataSource = null;
            gv_addedLineItems.RefreshData();

            //add or remove charge type items based on the type of selected charge type and the number of items in the grid
            SetupChargeTypeItems();
            combo_chargeType.EditValue = "Room Charge";

            GenerateIdByChargeType(ChargeType.ROOM_CHARGE, 0, -1);
            comboTranType.Enabled = false;

        }



        private void PopulateCustomerDTO()
        {
            customerDto = new Consignee();
            if (RegExt == null) return;
            if (RegExt.CompanyId != null)
            {
                if (UseCompany)
                {
                    if (string.IsNullOrEmpty(customerTIN) && !_isTinMandatory)
                    {
                        customerDto.Name = RegExt.Company;
                        customerDto.TIN = GuestTIN;
                        customerDto.ConsigneeType = "Guest";
                    }
                    else
                    {
                        customerDto.Code = _companyCode;
                        customerDto.Name = RegExt.Company;
                        customerDto.TIN = customerTIN;
                        customerDto.ConsigneeType = "Company";
                    }

                }
                else
                {
                    customerDto.Name = RegExt.Guest;
                    customerDto.TIN = GuestTIN;
                    customerDto.ConsigneeType = "Guest";
                }

            }
            else
            {
                customerDto.Name = RegExt.Guest;
                customerDto.TIN = GuestTIN;
                customerDto.ConsigneeType = "Guest";

            }

        }


        private List<Print_Item> GetPrintItems()
        {
            if (voFinal == null) return null;
            if (voFinal.lineItemDetails == null || voFinal.lineItemDetails.Count == 0)
            {
                XtraMessageBox.Show("unable to find lineitem details for the current transaction.", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;

            }
            List<Print_Item> printItemList = new List<Print_Item>();

            foreach (var item in voFinal.lineItemDetails)
            {
                Print_Item pItem = new Print_Item();
                pItem.ID = item.lineItems.Article;
                ArticleDTO article = UIProcessManager.GetArticleById(item.lineItems.Article);
                if (article != null)
                {
                    pItem.Name = article.Name;
                }
                else
                {
                    pItem.Name = "Unknown";
                }
                pItem.UnitPrice = Math.Round(item.lineItems.UnitAmount, _roundCalculatorDigit);
                pItem.Quantity = Convert.ToDecimal(item.lineItems.Quantity);
                pItem.taxId = item.lineItems.Tax == null ? 1 : item.lineItems.Tax.Value;
                var disount = item.lineItemValueFactor.FirstOrDefault(l => l.IsDiscount.Value);
                var serviceCharge = item.lineItemValueFactor.FirstOrDefault(l => !l.IsDiscount.Value);
                //pItem.DiscountAmount = disount == null ? 0 : Math.Round(disount.Amount, _roundCalculatorDigit)  ;
                //pItem.ServiceChargeAmount = serviceCharge == null ? 0 : Math.Round(serviceCharge.amount, _roundCalculatorDigit);
                //pItem.IsDiscountPercent = true;
                //pItem.IsServiceChargePercent = true;
                printItemList.Add(pItem);
            }


            return printItemList;

        }


        #endregion

        #region Event Handlers

        private void sLuk_article_EditValueChanged(object sender, EventArgs e)
        {

            if (sLuk_article.EditValue != null && !string.IsNullOrWhiteSpace(sLuk_article.EditValue.ToString()))
            {
                ValueDTO DefPrice = null;
                List<ArticleDTO> articledatasource = (List<ArticleDTO>)sLuk_article.Properties.DataSource;
                int articleCode = Convert.ToInt32(sLuk_article.EditValue);
                ArticleDTO selected = articledatasource.FirstOrDefault(x => x.Id == articleCode);
                articlePriceList = UIProcessManager.GetValueByArticle(articleCode);
                if (selected != null && selected.DefaultValue != null && selected.DefaultValue > 0)
                {
                    DefPrice = new ValueDTO()
                    {
                        NewValue = Convert.ToDecimal(selected.DefaultValue)
                    };
                    articlePriceList.Add(DefPrice);
                }
                if (articlePriceList != null)
                {
                    articlePriceList = articlePriceList.OrderBy(p => p.Priority).ToList();
                    luk_amount.Properties.DataSource = articlePriceList;
                }
            }
        }

        private void Close_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void frmManualCharge_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                Close();
            }
            if (IsLostCardFee)
                GenerateIdByChargeType(ChargeType.EXTRA_CHARGE, 0, CNETConstantes.CREDITSALES);
            else
                GenerateIdByChargeType(ChargeType.ROOM_CHARGE, 0, -1);
        }

        private void combo_chargeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UseCompany = false;
            ComboBoxEdit view = (ComboBoxEdit)sender;
            //Room Charge
            if (view.SelectedIndex == 0)
            {
                comboTranType.Enabled = false;
                sLuk_article.Properties.DataSource = _roomChargeArticles;
                gv_articleViewGrid.RefreshData();
                currentChargeType = ChargeType.ROOM_CHARGE;
                isRoomCharge = true;
                if (chargeId == null)
                {
                    GenerateIdByChargeType(ChargeType.ROOM_CHARGE, 0, -1);
                }
                else
                {
                    teNumber.Text = chargeId;
                }

                SetVoucherExtentions(CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);

            }

            //Package Charge
            else if (view.SelectedIndex == 1)
            {
                comboTranType.Enabled = false;
                sLuk_article.Properties.DataSource = _packageArticles;
                gv_articleViewGrid.RefreshData();
                currentChargeType = ChargeType.PACKAGE_CHARGE;
                isRoomCharge = true;
                if (chargeId == null)
                {
                    GenerateIdByChargeType(ChargeType.PACKAGE_CHARGE, 0, -1);
                }
                else
                {
                    teNumber.Text = chargeId;
                }

                SetVoucherExtentions(CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
            }
            //Extra Charge
            else if (view.SelectedIndex == 2)
            {
                comboTranType.Enabled = true;
                _extraArticles = new List<ArticleDTO>();
                //  _extraArticles.AddRange(_AllextraArticles);


                if (comboTranType.SelectedIndex == 0)
                {
                    if (_AllextraArticles != null && _AllextraArticles.Count > 0 && useassignedArticleCreditSales)
                    {
                        if (assignedArticleCreditSalesPreference != null && assignedArticleCreditSalesPreference.Count > 0)
                        {
                            _extraArticles = _AllextraArticles.Where(x => assignedArticleCreditSalesPreference.Contains(x.Preference)).ToList();
                        }
                        else
                            _extraArticles = new List<ArticleDTO>();
                    }
                    else
                    {
                        _extraArticles = new List<ArticleDTO>();

                        if (_AllextraArticles != null && _AllextraArticles.Count > 0)
                            _extraArticles.AddRange(_AllextraArticles);
                    }
                    GenerateIdByChargeType(ChargeType.EXTRA_CHARGE, 0, CNETConstantes.CREDITSALES);
                    SetVoucherExtentions(CNETConstantes.CREDITSALES);

                }
                else
                {
                    if (_AllextraArticles != null && _AllextraArticles.Count > 0 && useassignedArticleCashSales)
                    {
                        if (assignedArticleCashSalesPreference != null && assignedArticleCashSalesPreference.Count > 0)
                        {
                            _extraArticles = _AllextraArticles.Where(x => assignedArticleCashSalesPreference.Contains(x.Preference)).ToList();
                        }
                        else
                            _extraArticles = new List<ArticleDTO>();
                    }
                    else
                    {
                        _extraArticles = new List<ArticleDTO>();

                        if (_AllextraArticles != null && _AllextraArticles.Count > 0)
                            _extraArticles.AddRange(_AllextraArticles);
                    }

                    GenerateIdByChargeType(ChargeType.EXTRA_CHARGE, 0, CNETConstantes.CASH_SALES);
                    SetVoucherExtentions(CNETConstantes.CASH_SALES);
                }
                sLuk_article.Properties.DataSource = _extraArticles;
                gv_articleViewGrid.RefreshData();
                currentChargeType = ChargeType.EXTRA_CHARGE;
                isRoomCharge = false;

                if (!string.IsNullOrEmpty(RegExt.Company))
                {
                    if (XtraMessageBox.Show("Do u want to Charge to the Company", "CNETERP_V6", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
                    {
                        UseCompany = true;
                    }
                }
            }
            //if (!string.IsNullOrEmpty(RegExt.Company))
            //{
            //    if (XtraMessageBox.Show("Do u want to Charge to the Company", "CNETERP_V6", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
            //    {
            //        UseCompany = true;
            //    }
            //}
        }
        public void SetVoucherExtentions(int voucherDef)
        {
            layoutControlItem11.Text = "Reference";
            VoucherExtensionDefinitionDTO vExt = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(ve => ve.Index == 0 && ve.VoucherDefinition == voucherDef && ve.Type == CNETConstantes.VOUCHER_RELATION_TYPE_VOUCHER_EXT);

            if (vExt == null)
            {
                teReference.Enabled = false;
                return;

            }
            else
            {
                teReference.Enabled = true;
            }
            _voucherExt = vExt.Id;
            string label = string.IsNullOrEmpty(vExt.Descritpion) ? "Reference" : vExt.Descritpion;
            if (vExt.IsMandatory)
            {
                layoutControlItem11.Text = label + " *";
            }
            else
            {
                layoutControlItem11.Text = label;
            }


        }


        private void lukCurrency_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit view = (LookUpEdit)sender;
            if (view.EditValue != null && !string.IsNullOrWhiteSpace(view.EditValue.ToString()))
            {
                currentExRate = CommonLogics.GetLatestExchangeRate(Convert.ToInt32(view.EditValue));
                CalculateTotal(currentExRate);

            }
        }


        private void luk_amount_EditValueChanged(object sender, EventArgs e)
        {
            CalculateTotal(currentExRate);
        }


        private void spEdit_qty_EditValueChanged(object sender, EventArgs e)
        {
            CalculateTotal(currentExRate);
        }

        private void luk_amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (luk_amount.EditValue != null && luk_amount.EditValue != null)
                {
                    try
                    {
                        Convert.ToDecimal(luk_amount.EditValue.ToString());
                        AddArticlePrice(luk_amount.EditValue.ToString());

                    }
                    catch (Exception ex)
                    {
                        //string is added
                        AddArticlePrice("0");
                    }
                }
            }
        }

        private void luk_amount_Leave(object sender, EventArgs e)
        {
            if (luk_amount.EditValue != null && luk_amount.EditValue != null)
            {
                try
                {
                    Convert.ToDecimal(luk_amount.EditValue.ToString());
                    AddArticlePrice(luk_amount.EditValue.ToString());

                }
                catch (Exception ex)
                {
                    //string is added
                    AddArticlePrice("0");
                }
            }
        }



        private void btn_add_Click(object sender, EventArgs e)
        {

            List<Control> controls = new List<Control>
            {
                luk_amount,
                teTotal
            };

            IList<Control> invalidControls = CustomValidationRule.Validate(controls);

            if (invalidControls.Count > 0)
            {
                return;

            }

            if (sLuk_article.EditValue == null || string.IsNullOrWhiteSpace(sLuk_article.EditValue.ToString()))
            {
                XtraMessageBox.Show("Selected article is empty!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (teTotal.Text == "0.00")
            {
                XtraMessageBox.Show("Total value is zero or empty", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            if (teTotal.EditValue != null && !string.IsNullOrEmpty(teTotal.EditValue.ToString()))
            {
                if (teTotal.EditValue != null && Convert.ToDecimal(teTotal.EditValue.ToString()) <= 0)
                {
                    XtraMessageBox.Show("Total value is zero or less than zero", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
            }
            else
            {
                XtraMessageBox.Show("Total value is null or empty ", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CalculateLineItem(Convert.ToInt32(sLuk_article.EditValue), Convert.ToInt32(spEdit_qty.EditValue.ToString()), Convert.ToDecimal(luk_amount.EditValue.ToString()), Convert.ToDecimal(teTotal.Text), sLuk_article.Text);


        }

        private void gv_addedLineItems_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void comboTranType_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit view = sender as ComboBoxEdit;
            if (view == null) return;

            if (view.SelectedIndex == 0)
            {
                currentExtraVoucherType = CNETConstantes.CREDITSALES;
                GenerateIdByChargeType(ChargeType.EXTRA_CHARGE, 0, CNETConstantes.CREDITSALES);
                SetVoucherExtentions(CNETConstantes.CREDITSALES);

                if (_AllextraArticles != null && _AllextraArticles.Count > 0 && useassignedArticleCreditSales)
                {
                    if (assignedArticleCreditSalesPreference != null && assignedArticleCreditSalesPreference.Count > 0)
                    {
                        _extraArticles = _AllextraArticles.Where(x => assignedArticleCreditSalesPreference.Contains(x.Preference)).ToList();
                    }
                    else
                        _extraArticles = new List<ArticleDTO>();
                }
                else
                {
                    _extraArticles = new List<ArticleDTO>();

                    if (_AllextraArticles != null && _AllextraArticles.Count > 0)
                        _extraArticles.AddRange(_AllextraArticles);
                }
            }
            else
            {
                currentExtraVoucherType = CNETConstantes.CASH_SALES;
                GenerateIdByChargeType(ChargeType.EXTRA_CHARGE, 0, CNETConstantes.CASH_SALES);
                SetVoucherExtentions(CNETConstantes.CASH_SALES);

                if (_AllextraArticles != null && _AllextraArticles.Count > 0 && useassignedArticleCashSales)
                {
                    if (assignedArticleCashSalesPreference != null && assignedArticleCashSalesPreference.Count > 0)
                    {
                        _extraArticles = _AllextraArticles.Where(x => assignedArticleCashSalesPreference.Contains(x.Preference)).ToList();
                    }
                    else
                        _extraArticles = new List<ArticleDTO>();
                }
                else
                {
                    _extraArticles = new List<ArticleDTO>();

                    if (_AllextraArticles != null && _AllextraArticles.Count > 0)
                        _extraArticles.AddRange(_AllextraArticles);
                }

            }

            sLuk_article.Properties.DataSource = _extraArticles;

        }


        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                PMSDataLogger.LogMessage("frmManualCharge", "Save Click Manual Charge Form");
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();

                if (voFinal == null) return;

                if (layoutControlItem11.Text.Contains("*") && string.IsNullOrWhiteSpace(teReference.Text))
                {
                    SystemMessage.ShowModalInfoMessage("Reference Field is Required!", "ERROR");
                    return;
                }

                if (Convert.ToDecimal(te_grandTotal.Text) <= 0)
                {
                    SystemMessage.ShowModalInfoMessage("Grand Total Can't be less than zero", "ERROR");
                    return;
                }

                Progress_Reporter.Show_Progress("Saving Voucher", "Please Wait...");
                string vouchercode = "";

                #region Voucher
                if (currentChargeType == ChargeType.ROOM_CHARGE || currentChargeType == ChargeType.PACKAGE_CHARGE)
                {
                    voucher.Remark = teRemark.Text;
                    voucher.Definition = CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER;
                    vouchercode = GenerateIdByChargeType(ChargeType.ROOM_CHARGE, 1, -1);
                }
                else
                {
                    if (comboTranType.SelectedIndex == 0)
                    {
                        voucher.Definition = CNETConstantes.CREDITSALES;
                        vouchercode = GenerateIdByChargeType(ChargeType.EXTRA_CHARGE, 1, CNETConstantes.CREDITSALES);
                    }
                    else
                    {
                        voucher.Definition = CNETConstantes.CASH_SALES;
                        vouchercode = GenerateIdByChargeType(ChargeType.EXTRA_CHARGE, 1, CNETConstantes.CASH_SALES);
                    }

                    //check no-post priviledge
                    var previlage = UIProcessManager.GetRegistrationPrivllegeByvoucher(RegExt.Id);
                    if (previlage != null)
                    {
                        if (previlage.Nopost != null && previlage.Nopost)
                        {
                            SystemMessage.ShowModalInfoMessage("This registration has NO-POST Privilege", "ERROR");
                            Progress_Reporter.Close_Progress();
                            return;
                        }
                    }

                }

                if (vouchercode == null)
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!! For (" + voucher.Definition + ")", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }
                else
                {
                    voucher.Code = vouchercode;
                }

                #region Check Workflow


                int? adCode = null; int? lastState = null; int? adRegVoucher = null;

                if (currentChargeType == ChargeType.ROOM_CHARGE || currentChargeType == ChargeType.PACKAGE_CHARGE)
                {
                    //check workflow
                    var workflowRegVoucher = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOMCHARGE, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                    if (workflowRegVoucher == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please Define Workflow of ROOM CHARGE MADE  for Registration Voucher", "ERROR");
                        return;
                    }
                    else
                    {
                        adRegVoucher = workflowRegVoucher.Id;
                    }


                }

                if (voucher.Definition == CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER)
                {

                    var workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, voucher.Definition).FirstOrDefault();

                    if (workFlow != null)
                    {

                        adCode = workFlow.Id;
                        lastState = workFlow.State;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Please define workflow of PREPARED for Daily Room Charge Voucher ", "ERROR");
                        return;
                    }



                    //Check Activity Previlage
                    var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                    if (userRoleMapper != null)
                    {
                        var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                        if (roleActivity != null)
                        {
                            frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                            frmNeedPass.ShowDialog();
                            if (!frmNeedPass.IsAutenticated)
                            {
                                ////CNETInfoReporter.Hide();
                                return;
                            }

                        }

                    }

                }
                else
                {
                    var workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, voucher.Definition).FirstOrDefault();

                    if (workFlow != null)
                    {

                        adCode = workFlow.Id;
                        lastState = workFlow.State;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        if (voucher.Definition == CNETConstantes.CASH_SALES)
                            SystemMessage.ShowModalInfoMessage("Please define workflow of Prepared for Cash Sales Voucher ", "ERROR");
                        else
                            SystemMessage.ShowModalInfoMessage("Please define workflow of Prepared for Credit Sales Voucher ", "ERROR");
                        return;
                    }


                    //Check Activity Previlage
                    var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                    if (userRoleMapper != null)
                    {
                        var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(adCode.Value).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
                        if (roleActivity != null)
                        {
                            frmNeedPassword frmNeedPass = new frmNeedPassword(true);

                            frmNeedPass.ShowDialog();
                            if (!frmNeedPass.IsAutenticated)
                            {
                                ////CNETInfoReporter.Hide();
                                return;
                            }

                        }

                    }
                }

                #endregion

                if (currentChargeType == ChargeType.EXTRA_CHARGE)
                {

                    //Check TIN Mandatory
                    if (!string.IsNullOrEmpty(_companyCode))
                    {
                        if (_isTinMandatory && string.IsNullOrEmpty(customerTIN))
                        {
                            ////CNETInfoReporter.Hide();
                            XtraMessageBox.Show("Customer TIN is mandatory!", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    #region Print to fisical printer
                    /*

                         POS_Settings.IsError = true;
                         POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                         POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                         if (voucher.Definition == CNETConstantes.CREDITSALES)
                         {
                             POS_Settings.Voucher_Definition = voucher.Definition;
                             POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);
                         }
                         else
                         {
                             POS_Settings.Voucher_Definition = voucher.Definition;
                             POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);
                         }


                         // FiscalPrinters FP = new FiscalPrinters();
                         FiscalPrinters.GetInstance();
                         if (!POS_Settings.IsError)
                         {
                             XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                             return;
                         }
                         PopulateCustomerDTO();

                         //getting voucher setting values for Credit sales voucher
                         PMSVoucherSetting.GetCurrentVoucherSetting(voucher.Definition);

                         //prepare to print items
                         // Progress_Reporter.Show_Progress("Preparing Print Items");
                         _printItems = GetPrintItems();
                         if (_printItems == null)
                         {
                             ////CNETInfoReporter.Hide();
                             FiscalPrinters.DisposeInstance();
                             return;
                         }
                         bool validatePMSLicense = CommonLogics.Validate_PMSPOS_License();
                         if (!validatePMSLicense)
                             return;

                         string vouchecode = GenerateIdByChargeType(ChargeType.EXTRA_CHARGE, 1, voucher.Definition);
                         if (vouchecode == null)
                         {
                             SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                             ////CNETInfoReporter.Hide();
                             return;
                         }
                         else
                         {
                             voucher.Code = vouchecode;
                         }

                         // _currentFSNumber = ReadFSForPrinting();

                         bool isprinted = false;
                         //for Cash Sales, don't print the guest's info
                         if (voucher.Definition == CNETConstantes.CASH_SALES)
                         {
                             isprinted = FiscalPrinters.PrintOperation(CNETConstantes.CASH_SALES, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, voucher.Code,
                                                 null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucher.IssuedDate, 0);
                         }
                         else
                         {
                             isprinted = FiscalPrinters.PrintOperation(CNETConstantes.CREDITSALES, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, voucher.Code,
                                                 null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucher.IssuedDate, 0, RegExt.Room, RegExt.Registration);
                         }

                         if (!isprinted)
                         {
                             XtraMessageBox.Show("Unable to make fisical print!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                             return;
                         }
                         //reset values
                         POS_Settings.TotalDiscount = 0;
                         POS_Settings.TotalServiceCharge = 0;
                         */
                    #endregion
                }


                voucher.IssuedDate = CurrentTime.Value;
                voucher.AddCharge = voFinal.voucher.AddCharge;
                voucher.Discount = voFinal.voucher.Discount;
                voucher.SubTotal = voFinal.voucher.SubTotal;
                voucher.GrandTotal = voFinal.voucher.GrandTotal;
                voucher.LastState = lastState == null ? CNETConstantes.OSD_PREPARED_STATE : lastState.Value;

                VoucherBuffer voucherbuffer = new VoucherBuffer();
                voucherbuffer.Voucher = voucher;
                if (currentChargeType == ChargeType.EXTRA_CHARGE)
                {
                    voucherbuffer.Voucher.FsNumber = POS_Settings.CurrentFSNo;
                    voucherbuffer.Voucher.Mrc = POS_Settings.fiscalprinterMRC;
                }
                voucherbuffer.Voucher.OriginConsigneeUnit = RegExt.ConsigneeUnit;
                voucherbuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
                if (voucher.Definition != CNETConstantes.CASH_SALES)
                {
                    TransactionReferenceBuffer TRBuffer = new TransactionReferenceBuffer();
                    TRBuffer.TransactionReference = new TransactionReferenceDTO()
                    {
                        ReferencingVoucherDefn = voucher.Definition,
                        Referenced = RegExt.Id,
                        ReferencedVoucherDefn = CNETConstants.REGISTRATION_VOUCHER,
                        Value = voFinal.voucher.GrandTotal,
                        RelationType = luk_window.EditValue == null ? CNETConstantes.DEFAULT_WINDOW : Convert.ToInt32(luk_window.EditValue)
                    };
                    TRBuffer.ReferencedActivity = null;

                    //if value == 1 -> Fisical Printed ... else if == 1 -> not printed
                    if (currentChargeType != ChargeType.EXTRA_CHARGE)
                        TRBuffer.TransactionReference.Value = 0;
                    voucherbuffer.TransactionReferencesBuffer.Add(TRBuffer);
                }

                if (_voucherExt != null && !string.IsNullOrWhiteSpace(teReference.Text))
                    voucherbuffer.Voucher.Extension1 = teReference.Text;


                #region Tax Transaction

                voucherbuffer.TaxTransactions = new List<TaxTransactionDTO>();
                foreach (var tax in voFinal.taxTransactions)
                {
                    voucherbuffer.TaxTransactions.Add(tax);
                }

                #endregion

                voucherbuffer.TransactionCurrencyBuffer = new CNET_V7_Domain.Domain.Transaction.TransactionCurrencyBuffer();

                #region Save Transaction Currency
                //TransactionCurrencyDTO currentTranCurrency = new TransactionCurrencyDTO();
                //currentTranCurrency.Rate = 1;
                //currentTranCurrency.Total = voucher.GrandTotal;
                //if (defCurrency != null)
                //    currentTranCurrency.Currency = defCurrency.Id;
                //else
                //    currentTranCurrency.Currency = Convert.ToInt32(lukCurrency.EditValue);

                //currentTranCurrency.Amount = voucher.GrandTotal;

                voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency = new TransactionCurrencyDTO();
                voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Rate = 1;
                voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Total = voucher.GrandTotal;
                if (defCurrency != null)
                    voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Currency = defCurrency.Id;
                else
                    voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Currency = Convert.ToInt32(lukCurrency.EditValue);

                voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Amount = voucher.GrandTotal;

                #endregion

                voucherbuffer.Voucher.Note = teRemark.EditValue == null || string.IsNullOrWhiteSpace(teRemark.EditValue.ToString()) ? "Manual Charge" : "Manual Charge ( " + teRemark.EditValue.ToString() + " )";

                voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime.Value, adCode.Value, CNETConstantes.PMS_Pointer);


                voucherbuffer.LineItemsBuffer = new List<LineItemBuffer>();

                LineItemBuffer lineItemBuffer = new LineItemBuffer();

                foreach (ManualChargeVM dto in _manualChargeDtoList)
                {
                    lineItemBuffer = new LineItemBuffer();
                    lineItemBuffer.LineItem = dto.LineItem;
                    lineItemBuffer.LineItem.ObjectState = null;
                    if (dto.LineItemValueFactor != null && dto.LineItemValueFactor.Count > 0)
                    {
                        var service = dto.LineItemValueFactor.Where(x => !x.IsDiscount.Value).ToList();
                        var disc = dto.LineItemValueFactor.Where(x => x.IsDiscount.Value).ToList();

                        if (service != null && service.Count > 0)
                            lineItemBuffer.LineItem.AddCharge = service.Sum(x => x.Amount);
                        if (disc != null && disc.Count > 0)
                            lineItemBuffer.LineItem.Discount = service.Sum(x => x.Amount);
                    }
                    voucherbuffer.LineItemsBuffer.Add(lineItemBuffer);

                    // foreach (var lvf in dto.LineItemValueFactor)
                }

                //Payment Method
                if (currentChargeType == ChargeType.EXTRA_CHARGE)
                {
                    PMSDataLogger.LogMessage("frmManualCharge", "EXTRA CHARGE Manual Charge Form");
                    if (voucherbuffer.Voucher.Definition == CNETConstantes.CASH_SALES)
                    {
                        PMSDataLogger.LogMessage("frmManualCharge", "Cash EXTRA CHARGE Manual Charge Form");
                        voucherbuffer.Voucher.PaymentMethod = CNETConstantes.PAYMENTMETHODSCASH;
                    }
                    else
                    {
                        PMSDataLogger.LogMessage("frmManualCharge", "Credit EXTRA CHARGE Manual Charge Form");
                        voucherbuffer.Voucher.PaymentMethod = CNETConstantes.PAYMENTMETHODSCREDIT;
                    }


                    #region

                    PMSDataLogger.LogMessage("frmManualCharge", "Check Printer Manual Charge Form");
                    POS_Settings.IsError = true;
                    POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                    POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                    if (voucher.Definition == CNETConstantes.CREDITSALES)
                    {
                        POS_Settings.Voucher_Definition = voucher.Definition;
                        POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);
                    }
                    else
                    {
                        POS_Settings.Voucher_Definition = voucher.Definition;
                        POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);
                    }


                    // FiscalPrinters FP = new FiscalPrinters();
                    FiscalPrinters.GetInstance();
                    if (!POS_Settings.IsError)
                    {
                        PMSDataLogger.LogMessage("frmManualCharge", "Unable to connect with fisical printer !! Manual Charge Form");
                        XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    PopulateCustomerDTO();

                    //getting voucher setting values for Credit sales voucher
                    PMSVoucherSetting.GetCurrentVoucherSetting(voucher.Definition);

                    //prepare to print items
                    // Progress_Reporter.Show_Progress("Preparing Print Items");
                    _printItems = GetPrintItems();
                    if (_printItems == null)
                    {
                        ////CNETInfoReporter.Hide();
                        FiscalPrinters.DisposeInstance();
                        return;
                    }
                    bool validatePMSLicense = CommonLogics.Validate_PMSPOS_License();
                    if (!validatePMSLicense)
                    {
                        PMSDataLogger.LogMessage("frmManualCharge", "Fail to validate Pos License !! Manual Charge Form");
                        return;
                    }
                    #endregion
                }

                voucherbuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                voucherbuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                PMSDataLogger.LogMessage("frmManualCharge", "Saving " + voucherbuffer.Voucher.Code + " !! Manual Charge Form");
                ResponseModel<VoucherBuffer> isSaved = UIProcessManager.CreateVoucherBuffer(voucherbuffer);

                #endregion

                if (isSaved != null && isSaved.Success)
                {
                    PMSDataLogger.LogMessage("frmManualCharge", "Saving Successful !! Manual Charge Form");
                    //ad for registration voucher
                    if (adRegVoucher != null)
                    {
                        ActivityDTO activityRegVoucher = ActivityLogManager.SetupActivity(CurrentTime.Value, adRegVoucher.Value, CNETConstantes.PMS_Pointer, "Voucher => " + voucher.Code + "  Amount => " + voucher.GrandTotal);
                        activityRegVoucher.Reference = RegExt.Id;
                        UIProcessManager.CreateActivity(activityRegVoucher);
                    }
                    if (currentChargeType == ChargeType.EXTRA_CHARGE)
                    {
                        Progress_Reporter.Show_Progress("Printing Recipt", "Please Wait...");
                        #region Print to fisical printer

                        /*
                                                    POS_Settings.IsError = true;
                                                    POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                                                    POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                                                    if (voucher.Definition == CNETConstantes.CREDITSALES)
                                                    {
                                                        POS_Settings.Voucher_Definition = voucher.Definition;
                                                        POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);
                                                    }
                                                    else
                                                    {
                                                        POS_Settings.Voucher_Definition = voucher.Definition;
                                                        POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);
                                                    }


                                                    // FiscalPrinters FP = new FiscalPrinters();
                                                    FiscalPrinters.GetInstance();
                                                    if (!POS_Settings.IsError)
                                                    {
                                                        XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                        return;
                                                    }
                                                    PopulateCustomerDTO();

                                                    //getting voucher setting values for Credit sales voucher
                                                    PMSVoucherSetting.GetCurrentVoucherSetting(voucher.Definition);

                                                    //prepare to print items
                                                    // Progress_Reporter.Show_Progress("Preparing Print Items");
                                                    _printItems = GetPrintItems();
                                                    if (_printItems == null)
                                                    {
                                                        ////CNETInfoReporter.Hide();
                                                        FiscalPrinters.DisposeInstance();
                                                        return;
                                                    }
                                                    bool validatePMSLicense = CommonLogics.Validate_PMSPOS_License();
                                                    if (!validatePMSLicense)
                                                        return;
                                                    */
                        //string vouchecode = GenerateIdByChargeType(ChargeType.EXTRA_CHARGE, 1, voucher.Definition);
                        //if (vouchecode == null)
                        //{
                        //    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                        //    ////CNETInfoReporter.Hide();
                        //    return;
                        //}
                        //else
                        //{
                        //    voucher.Code = vouchecode;
                        //}

                        // _currentFSNumber = ReadFSForPrinting();

                        POS_Settings.TotalServiceCharge = isSaved.Data.Voucher.AddCharge;
                        POS_Settings.TotalDiscount = isSaved.Data.Voucher.Discount;

                        bool isprinted = false;
                        //for Cash Sales, don't print the guest's info
                        if (voucher.Definition == CNETConstantes.CASH_SALES)
                        {
                            PMSDataLogger.LogMessage("frmManualCharge", "Printing CASH SALES Voucher " + isSaved.Data.Voucher.Code + " Manual Charge Form");
                            PMSDataLogger.LogMessage("frmManualCharge", "Total Service Charge " + POS_Settings.TotalServiceCharge );
                            PMSDataLogger.LogMessage("frmManualCharge", "Total Discount " + POS_Settings.TotalDiscount);
                            isprinted = FiscalPrinters.PrintOperation(CNETConstantes.CASH_SALES, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, voucher.Code,
                                                null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucher.IssuedDate, 0);
                        }
                        else
                        {
                            PMSDataLogger.LogMessage("frmManualCharge", "Printing Credit SALES Voucher " + isSaved.Data.Voucher.Code + " Manual Charge Form");
                            PMSDataLogger.LogMessage("frmManualCharge", "Total Service Charge " + POS_Settings.TotalServiceCharge);
                            PMSDataLogger.LogMessage("frmManualCharge", "Total Discount " + POS_Settings.TotalDiscount);
                            isprinted = FiscalPrinters.PrintOperation(CNETConstantes.CREDITSALES, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, voucher.Code,
                                                null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucher.IssuedDate, 0, RegExt.Room, RegExt.Registration);
                        }
                        Progress_Reporter.Close_Progress();

                        if (!isprinted)
                        {
                            PMSDataLogger.LogMessage("frmManualCharge", "Fail Printing Voucher " + isSaved.Data.Voucher.Code + " Manual Charge Form");

                            var dele = UIProcessManager.DeleteVoucherObjects(isSaved.Data.Voucher.Id);

                            if (dele != null && dele.Data)
                                PMSDataLogger.LogMessage("frmManualCharge", "Manual Deleted Charge Voucher " + isSaved.Data.Voucher.Code + " Manual Charge Form");
                            else
                                PMSDataLogger.LogMessage("frmManualCharge", "Delete Fail Manual Charge Voucher " + isSaved.Data.Voucher.Code + " Manual Charge Form");


                            XtraMessageBox.Show("Unable to make fisical print!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            VoucherDTO voucher = UIProcessManager.Patch_FS_No(isSaved.Data.Voucher.Id, POS_Settings.CurrentFSNo, POS_Settings.fiscalprinterMRC);

                            if (voucher == null)
                                PMSDataLogger.LogMessage("frmFrontOfficePOS", "Fail to Patch " + isSaved.Data.Voucher.Code + " FS." + POS_Settings.CurrentFSNo + " MRC." + POS_Settings.fiscalprinterMRC);
                        }
                        //reset values
                        POS_Settings.TotalDiscount = 0;
                        POS_Settings.TotalServiceCharge = 0;

                        #endregion
                    }
                    if (IsLostCardFee)
                        XtraMessageBox.Show("Lost Card is charged!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        XtraMessageBox.Show("Manual charge is saved!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ResetAll();
                    if (IsLostCardFee)
                    {
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    PMSDataLogger.LogMessage("frmManualCharge", "Fail to save !! Manual Charge Form" + isSaved.Message);
                    SystemMessage.ShowModalInfoMessage("Fail to Save Transaction !!" + Environment.NewLine + isSaved.Message, "ERROR");
                }
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in saving voucher. DETAIL::" + ex.Message, "ERROR");
                ////CNETInfoReporter.Hide();
            }
            Progress_Reporter.Close_Progress();
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            try
            {
                //get selected row
                ManualChargeVM selected = gv_addedLineItems.GetFocusedRow() as ManualChargeVM;
                if (selected != null)
                {
                    _manualChargeDtoList.Remove(selected);

                    gc_addedLineItems.DataSource = _manualChargeDtoList;
                    gv_addedLineItems.RefreshData();

                    //add or remove charge type items based on the type of selected charge type and the number of items in the grid
                    SetupChargeTypeItems();

                    //Calculate Grand Total
                    CalculatGrandTotal(_manualChargeDtoList);
                }

            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in removing line item. DETAIL::" + ex.Message, "ERROR");
            }
        }

        private void bbiRateSummary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmRateSummery _frmRateSummery = new frmRateSummery();
            _frmRateSummery.RegExtension = RegExt;
            _frmRateSummery.ShowDialog(this);
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ResetAll();
        }


        #endregion





    }

    enum ChargeType
    {
        ROOM_CHARGE = 0,
        PACKAGE_CHARGE = 1,
        EXTRA_CHARGE = 2
    }
}
