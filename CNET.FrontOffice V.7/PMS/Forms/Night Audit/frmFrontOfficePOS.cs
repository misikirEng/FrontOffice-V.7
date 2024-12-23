using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.Text.RegularExpressions;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using DevExpress.XtraBars;
using CNET_V7_Domain.Domain.TransactionSchema;
using ProcessManager;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.POS.Common.Models;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET.POS.Settings;
using CNET.FrontOffice_V._7.PMS.DTO;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using System.Diagnostics;
using DevExpress.XtraGauges.Core.Model;
using CNET.FP.Tool;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET_V7_Domain;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using CNET.Progress.Reporter;
using CNET_V7_Domain.Domain.Transaction;
using DevExpress.CodeParser;
using System.Reflection.Metadata.Ecma335;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Misc;

namespace CNET.FrontOffice_V._7.Night_Audit
{
    public partial class frmFrontOfficePOS : UILogicBase
    {
        private Consignee customerDto = null;
        private VoucherBuffer voucherbuffer;
        private VoucherFinalDTO voFinal = new VoucherFinalDTO();
        private List<VwLineItemDetailViewDTO> lineItemDetail = new List<VwLineItemDetailViewDTO>();
        private RoomDetailDTO rd = null;
        private List<Print_Item> _printItems = null;
        private int _roundCalculatorDigit = 2;
        private decimal _vat = 0;
        private decimal _vatAmt = 0;
        private decimal _additionalChargeAmt = 0;
        private bool _isAdditionalInPercent = false;
        private string _currentVoucherCode = string.Empty;
        public bool _isThereAnyError = false;
        private bool isNetOff = false;
        private int _activityDefCheckout;
        private bool _isDirectBill = false;
        private bool _isAccEnabled = false;
        private string statusMessage = "WARNING: this customer has no TIN number!";
        private bool _isTinMandatory = false;
        private bool _isSummerized = true;

        #region Properties

        private RegistrationListVMDTO regExtension;
        internal RegistrationListVMDTO RegExtension
        {
            get { return regExtension; }
            set
            {
                regExtension = value;

            }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        //private string _currentFsNumber;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private decimal taxRate { get; set; }
        private decimal ServiceCharge { get; set; }
        private decimal Discount { get; set; }
        private decimal GrandTotal { get; set; }
        private decimal SubTotal { get; set; }

        private int _window = -1;
        public int Window
        {
            get
            {
                return _window;
            }
            set
            {
                _window = value;
            }
        }

        public bool isCheckOut { get; set; }
        public DateTime CurrentTime { get; set; }

        public bool IsReinstate { get; set; }
        internal VoucherBuffer VoucherBuffer
        {
            get { return voucherbuffer; }
            set
            {

                voucherbuffer = value;

                if (_isThereAnyError) return;


            }
        }

        #endregion

        //**************************** CONSTRUCTOR *********************************//
        public frmFrontOfficePOS(bool isDirectBill = false)
        {
            try
            {
                InitializeComponent();

                PMSDataLogger.LogMessage("frmFrontOfficePOS", "Opening FrontOfficePOS Form for Receipt");
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

                InitializeUI();
                _isDirectBill = isDirectBill;
                if (!isDirectBill)
                {

                    var configDefaultBilType = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_DefaultCheckOutPaymentType);

                    if (configDefaultBilType != null && !string.IsNullOrEmpty(configDefaultBilType.CurrentValue) && configDefaultBilType.CurrentValue == "Cash_Sales")
                        cbeTransactionType.EditValue = "Cash Sales";
                    else
                        cbeTransactionType.EditValue = "Credit Sales";

                    //if (POS_Settings.Voucher_Definition == CNETConstantes.CASH_SALES)
                    //    cbeTransactionType.EditValue = "Cash Sales";
                    //else
                    //    cbeTransactionType.EditValue = "Credit Sales";

                }
                else
                {
                    cbeTransactionType.EditValue = "Credit Sales";
                    cbeTransactionType.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing front office pos. DETAIL::" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /*************** METHODS **************************/
        #region Methods

        private void InitializeUI()
        {
            // Payment type
            lePayment.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            lePayment.Properties.DisplayMember = "Description";
            lePayment.Properties.ValueMember = "Id";



            //Windows
            lukWindow.Properties.Columns.Add(new LookUpColumnInfo("Description", "Window"));
            lukWindow.Properties.DisplayMember = "Description";
            lukWindow.Properties.ValueMember = "Id";

            //Fiscal Bill Type
            repoLukFiscalBillType.Columns.Add(new LookUpColumnInfo("Column", "Fiscal Bill Type"));
            repoLukFiscalBillType.DisplayMember = "Column";
            repoLukFiscalBillType.ValueMember = "Column";

            repoArticleEdit.EditValueChanged += RepoArticleEdit_EditValueChanged;
            repoQtyEdit.EditValueChanged += RepoQtyEdit_EditValueChanged;

            ribbonPageGroup1.Visible = true;
        }

        private bool InitializeData()
        {
            try
            {
                if (RegExtension == null || VoucherBuffer == null) return false;

                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null)
                {
                    return false;
                }

                deDate.EditValue = CurrentTime.Value;

                // Progress_Reporter.Show_Progress("Initializing Data");



                //Read TIN Mandatory Configuration
                var tinConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == LocalBuffer.LocalBuffer.CurrentDevice.Id.ToString() && c.Attribute.ToLower() == "company tin mandatory");
                if (tinConfig != null && !string.IsNullOrEmpty(tinConfig.CurrentValue))
                {
                    _isTinMandatory = Convert.ToBoolean(tinConfig.CurrentValue);
                    beiMandatoryTIN.EditValue = _isTinMandatory;
                }

                teRegistration.Text = RegExtension.Registration;
                //Populate Customer DTO
                PopulateCustomerDTO(RegExtension.CompanyId);


                teRoom.Text = RegExtension.Room;

                //get current authorized user 


                // get configurations values
                GetVoucherSettingValues(CNETConstantes.CHECK_OUT_BILL_VOUCHER);

                //Check Accounting Configuration
                var configAccounting = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_EnableJournalization);
                if (configAccounting != null && Convert.ToBoolean(configAccounting.CurrentValue))
                {
                    cbeTransactionType.EditValue = "Credit Sales";
                    cbeTransactionType.Enabled = false;
                    _isAccEnabled = true;
                }


                //Transaction Type
                List<SystemConstantDTO> paymentList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == "Payment Methods").ToList();
                if (paymentList != null)
                {
                    lePayment.Properties.DataSource = (paymentList.OrderByDescending(c => c.IsDefault).ToList());
                    // Set default a record that has signed as IsDefault
                    SystemConstantDTO payLookup = paymentList.FirstOrDefault(c => c.IsDefault);
                    if (payLookup != null)
                    {
                        lePayment.EditValue = (payLookup.Id);
                        // POSSettingCache.defultPaymnet = payLookup.description.ToString();
                    }
                }

                teVoucherNo.Text = GetCurrentId(0);
                //Window
                //populate window lookups
                List<SystemConstantDTO> windowsList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.SPLIT_WINDOWS && l.IsActive).ToList();
                if (windowsList != null && windowsList.Count != 0)
                {
                    windowsList.Insert(0, new SystemConstantDTO() { Id = -1, Description = "All" });
                    lukWindow.Properties.DataSource = windowsList;
                    if (Window > 0)
                    {
                        var luk = windowsList.FirstOrDefault(l => l.Value == Window.ToString());
                        if (luk != null)
                        {
                            lukWindow.EditValue = luk.Id;
                        }
                        else
                        {
                            lukWindow.EditValue = windowsList[0].Id;
                        }
                    }
                    else
                    {
                        lukWindow.EditValue = windowsList[0].Id;

                    }

                }

                if (IsReinstate)
                {
                    lukWindow.Enabled = false;
                }

                //workflow
                if (!CheckWorkflow())
                {
                    ////CNETInfoReporter.Hide();
                    return false;
                }

                //Calculate Summerized Voucher
                CalculateVoFinal();

                //Populate Fiscal Bill Type
                string[] fiscalBillTypes = { "Summary", "Long Detail", "Summary Edit" };
                repoLukFiscalBillType.DataSource = fiscalBillTypes;

                var configDefaultBilType = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_DefaultFiscalBillType);
                if (configDefaultBilType != null && !string.IsNullOrEmpty(configDefaultBilType.CurrentValue))
                {
                    beiFiscalBillType.EditValue = configDefaultBilType.CurrentValue;
                }
                else
                {
                    beiFiscalBillType.EditValue = "Summary";
                }





                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing front office POS. DETAIL::" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void CalculateVoFinal()
        {
            try
            {
                // Progress_Reporter.Show_Progress("Getting Summerized Voucher", "Please Wait...");

                if (isCheckOut)
                {
                    RateCodeHeaderDTO rateCodeHeader = UIProcessManager.GetRateCodeHeaderById(RegExtension.RateCodeHeader.Value);

                    if (rateCodeHeader != null)
                    {
                        voFinal = SummarizeDailyRoomChargeWithCreditInvoice(RegExtension.Id,
                       RegExtension.GuestId, RegExtension.CompanyId, rateCodeHeader, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, false);
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("No Rate Code found for this registration!", "MESSAGE");
                        ////CNETInfoReporter.Hide();
                    }

                    if (voFinal == null)
                    {
                        SystemMessage.ShowModalInfoMessage("No transaction found for these guest!", "MESSAGE");
                        ////CNETInfoReporter.Hide();
                        this.Close();
                    }
                    else
                        RoundCalculator(voFinal);

                    //round calculator and text editors setter

                }

                ////CNETInfoReporter.Hide();


            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in calculating summerized voucher. Detail:: " + ex.Message, "ERROR");
            }
        }

        private bool PopulateDetailData()
        {
            gcFrontOfficePOS.DataSource = null;
            gvFrontOfficePOS.RefreshData();
            if (_printItems != null) _printItems.Clear();
            lineItemDetail.Clear();
            try
            {
                int? Window = (lukWindow.EditValue == null || Convert.ToInt32(lukWindow.EditValue) == -1) ? null : Convert.ToInt32(lukWindow.EditValue);
                // Progress_Reporter.Show_Progress("Populating Detail Data", "Please Wait...");
                var gLedger = UIProcessManager.GetGuestLedger(RegExtension.Id, RegExtension.Arrival, RegExtension.Departure, RegExtension.Room, Window == 0 ? null : Window);
                if (gLedger != null)
                {
                    foreach (var rc in gLedger.RoomCharges)
                    {
                        if (rc.tranRefValue != 1)
                        {
                            VwLineItemDetailViewDTO lineItem = new VwLineItemDetailViewDTO();
                            lineItem.Code = rc.voucherCode;
                            lineItem.Name = string.Format("{0} [{1}]", rc.date.ToString("dd/MM/yyyy"), rc.roomNo);
                            lineItem.Quantity = 1;
                            lineItem.UnitAmount = rc.amount;
                            lineItem.TotalAmount = rc.amount;
                            lineItemDetail.Add(lineItem);
                        }
                    }
                }

                gcFrontOfficePOS.DataSource = lineItemDetail;
                gcFrontOfficePOS.RefreshDataSource();
                gvFrontOfficePOS.RefreshData();



                //get print items
                List<VwLineItemDetailViewDTO> dtoList = gvFrontOfficePOS.DataSource as List<VwLineItemDetailViewDTO>;
                if (dtoList != null && dtoList.Count > 0)
                {
                    if (_printItems != null) _printItems.Clear();
                    _printItems = GetPrintItems(dtoList, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get print Items", "ERROR");
                }

                if (_printItems == null || _printItems.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get print Items", "ERROR");
                }
                if (_printItems.Sum(x => x.UnitPrice) <= 0)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get print Items Price !!", "ERROR");
                }



                ////CNETInfoReporter.Hide();
                return true;

            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in populating detail data. DETAIL:: " + ex.Message, "ERROR");
                return false;
            }
        }

        private bool PopulateSummaizedData()
        {
            gcFrontOfficePOS.DataSource = null;
            gvFrontOfficePOS.RefreshData();
            if (_printItems != null) _printItems.Clear();
            lineItemDetail.Clear();

            try
            {
                gcFrontOfficePOS.DataSource = null;
                if (lineItemDetail != null)
                    lineItemDetail.Clear();
                //if from check out
                // Progress_Reporter.Show_Progress("Getting line items..");
                if (isCheckOut)
                {

                    if (voFinal != null)
                    {
                        if (voFinal.lineItemDetails == null)
                        {
                            SystemMessage.ShowModalInfoMessage("No line item detail for this registration", "ERROR");
                            ////CNETInfoReporter.Hide();
                            return false;
                        }
                        List<LineItemDetails> lineItemDetails = voFinal.lineItemDetails;
                        foreach (var le in lineItemDetails)
                        {
                            if (le != null)
                            {

                                VwLineItemDetailViewDTO lineItem = new VwLineItemDetailViewDTO();
                                lineItem.Code = le.lineItems.Article.ToString();
                                lineItem.Name = "Accomodation";
                                lineItem.Quantity = 1;
                                if (le.lineItems.UnitAmount != null)
                                {
                                    lineItem.UnitAmount = Math.Round(le.lineItems.UnitAmount, 2);
                                    if (le.lineItems.TotalAmount != null)
                                        lineItem.TotalAmount = Math.Round(le.lineItems.TotalAmount, 2);
                                }
                                lineItemDetail.Add(lineItem);

                            }
                        }


                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("No transaction found for these guest!", "MESSAGE");
                        ////CNETInfoReporter.Hide();
                        return false;
                    }
                }
                else
                {
                    lineItemDetail = UIProcessManager.GetLineItemDetailByVoucher(RegExtension.Id).ToList();
                }

                gcFrontOfficePOS.DataSource = lineItemDetail;
                gcFrontOfficePOS.RefreshDataSource();
                gvFrontOfficePOS.RefreshData();


                //get print items
                List<VwLineItemDetailViewDTO> dtoList = gvFrontOfficePOS.DataSource as List<VwLineItemDetailViewDTO>;
                if (dtoList != null && dtoList.Count > 0)
                {
                    if (_printItems != null) _printItems.Clear();
                    _printItems = GetPrintItems(dtoList, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get print Items", "ERROR");
                }

                if (_printItems == null || _printItems.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get print Items", "ERROR");
                }
                if (_printItems.Sum(x => x.UnitPrice) <= 0)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get print Items Price !!", "ERROR");
                }

                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Exception is occurred. Detail: " + ex.Message, "ERROR");
                ////CNETInfoReporter.Hide();
                return false;
            }
        }

        private VoucherFinalDTO SummarizeDailyRoomChargeWithCreditInvoice(int regCode, int? guest, int? customer, RateCodeHeaderDTO ratecode, int voucherDef, bool printOption)
        {
            try
            {
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null) return null;
                VoucherFinalDTO voFinal = null;
                if (lukWindow.EditValue == null) return null;
                List<VwDailyChargePostingViewDTO> dailyCharge = null;
                if (IsReinstate && lukWindow.EditValue.ToString() == "ALL")
                    dailyCharge = UIProcessManager.GetCheckOutDetailViewByVoucher(regCode, voucherDef, null);

                else
                    dailyCharge = UIProcessManager.GetCheckOutDetailViewByVoucher(regCode, voucherDef, Convert.ToInt32(lukWindow.EditValue));

                if (dailyCharge == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to find any Daily Room Charge!", "ERROR");
                    return null;
                }

                if (ratecode == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to find the rate header", "ERROR");
                    return null;
                }

                //read voucher setting


                PMSVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.CHECK_OUT_BILL_VOUCHER);
                VoucherDTO Voucher = new VoucherDTO();
                Voucher.Code = teVoucherNo.Text;
                Voucher.Definition = voucherDef;
                Voucher.Consignee1 = customer ?? guest;
                Voucher.IssuedDate = CurrentTime.Value;
                Voucher.Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime.Value);

                double discount = 0;
                if (lukWindow.EditValue == null || lukWindow.Text.ToLower() == "all" || Convert.ToInt32(lukWindow.EditValue) == -1)
                {
                    discount = Convert.ToDouble(UIProcessManager.GetDiscount(regCode, CNETConstantes.CREDIT_NOTE_VOUCHER, null));
                }
                else
                {
                    discount = Convert.ToDouble(UIProcessManager.GetDiscount(regCode, CNETConstantes.CREDIT_NOTE_VOUCHER, Convert.ToInt32(lukWindow.EditValue)));
                }

                var additionlist = dailyCharge.Where(x => x.additionalCharge != null).ToList();


                double serviceCharge = additionlist == null ? 0 : Convert.ToDouble(additionlist.Sum(x => x.additionalCharge.Value));



                TaxDTO tax = CommonLogics.GetApplicableTax(RegExtension.Id, voucherDef, RegExtension.GuestId, ratecode.Article);
                if (!string.IsNullOrEmpty(tax.Remark))
                {
                    SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                    return null;
                }

                //calculate line item
                LineItemDTO lineItem = new LineItemDTO();
                //lineItem.Voucher = Voucher.code;
                lineItem.Quantity = 1;
                lineItem.Article = ratecode.Article;
                lineItem.UnitAmount = dailyCharge.Sum(x => x.unitAmount.Value);
                lineItem.Tax = tax.Id;
                lineItem.ObjectState = null;
                //LineItemDTO newLineItem = new LineItemDTO();
                //newLineItem.lineitem = lineItem;

                var vfdList = UIProcessManager.SelectAllValueFactorDefinition();
                ValueFactorDefinitionDTO discVFD = null;
                ValueFactorDefinitionDTO additionalVFD = null;
                if (discount > 0)
                {
                    discVFD = vfdList.FirstOrDefault(v => v.Type == CNETConstantes.DISCOUNT && v.GslType == CNETConstantes.SERVICES);
                    if (discVFD == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Please define Disount value factor definition for Services", "ERROR");
                        return null;
                    }
                }

                if (serviceCharge > 0)
                {
                    additionalVFD = vfdList.FirstOrDefault(v => v.Type == CNETConstantes.ADDTIONAL_CHARGE && v.GslType == CNETConstantes.SERVICES);
                    if (additionalVFD == null)
                    {
                        SystemMessage.ShowModalInfoMessage("Please define Additional Charge value factor definition for Services", "ERROR");
                        return null;
                    }
                }



                //note: price is already extracted
                LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(new VoucherBuffer() { Voucher = Voucher }, lineItem, RegExtension.Id, discVFD == null ? null : discVFD.Id, additionalVFD == null ? null : additionalVFD.Id, serviceCharge, discount, true, false, false, false);
                LineItemDetails liDetails = new LineItemDetails()
                {
                    lineItems = liDetail.lineItem,
                    lineItemValueFactor = liDetail.lineItemValueFactor
                };
                List<LineItemDetails> liDetailsList = new List<LineItemDetails>();
                liDetailsList.Add(liDetails);
                voFinal = new VoucherFinalCalculator().VoucherCalculation(Voucher, liDetailsList);
                return voFinal;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in summarizing daily room charge voucher. Detail:: " + ex.Message, "ERROR");
                return null;
            }
        }

        private void RoundCalculator(VoucherFinalDTO voFinal)
        {
            try
            {

                // get tax rate
                if (cbeTransactionType.SelectedIndex == 0)
                {
                    taxRate = GetTaxRate(CNETConstantes.CHECK_OUT_BILL_VOUCHER, voFinal.lineItemDetails[0].lineItems.Article);
                }
                else
                {
                    taxRate = GetTaxRate(CNETConstantes.CHECK_OUT_BILL_VOUCHER, voFinal.lineItemDetails[0].lineItems.Article);
                }

                //Discount = Math.Round(UIProcessManager.GetDiscount(RegExtension.Registration, CNETConstantes.CREDIT_NOTE_VOUCHER), 2);
                Discount = Math.Round(voFinal.voucher.Discount, 2);
                ServiceCharge = Math.Round(Convert.ToDecimal(voFinal.voucher.AddCharge), 2);
                decimal vatAmt = (voFinal.voucher.SubTotal + ServiceCharge - Discount) * (Math.Round(taxRate / 100, 2));
                SubTotal = Math.Round(Convert.ToDecimal(voFinal.voucher.SubTotal), 2);
                GrandTotal = voFinal.voucher.SubTotal + ServiceCharge - Discount + vatAmt;


                teVAT.Text = String.Format("{0:F2}", Math.Round(Convert.ToDecimal(vatAmt), _roundCalculatorDigit).ToString());
                tediscount.Text = String.Format("{0:F2}", Math.Round(Convert.ToDecimal(Discount), _roundCalculatorDigit).ToString());
                teSerCharge.Text = String.Format("{0:F2}", Math.Round(Convert.ToDecimal(ServiceCharge), _roundCalculatorDigit).ToString());
                teSubTotal.Text = String.Format("{0:F2}", Math.Round(Convert.ToDecimal(SubTotal), _roundCalculatorDigit).ToString());
                teGrandTotal.Text = String.Format("{0:F2}", Math.Round(Convert.ToDecimal(GrandTotal), _roundCalculatorDigit).ToString());

                POS_Settings.TotalDiscount = Convert.ToDecimal(voFinal.voucher.Discount);
                POS_Settings.TotalServiceCharge = ServiceCharge;

            }
            catch (Exception)
            {
                //  throw;
            }
        }

        public decimal GetTaxRate(int voucherDef, int articleCode)
        {
            TaxDTO tax = CommonLogics.GetApplicableTax(RegExtension.Id, voucherDef, RegExtension.GuestId, articleCode);
            if (!string.IsNullOrEmpty(tax.Remark))
            {
                SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                this.Close();
            }
            return (decimal)tax.Amount;
        }

        public string GetCurrentId(int generationType)
        {

            string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.CHECK_OUT_BILL_VOUCHER, generationType, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(currentVoCode))
            {
                return currentVoCode;
            }
            return "";
        }



        private List<Print_Item> GetPrintItems(List<VwLineItemDetailViewDTO> lineItemDetail, int voucherDef)
        {
            TaxDTO tax = null;
            tax = CommonLogics.GetApplicableTax(RegExtension.Id, voucherDef, RegExtension.GuestId, lineItemDetail[0].Article);

            if (!string.IsNullOrEmpty(tax.Remark))
            {
                SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                return null;
            }

            List<Print_Item> printItemList = new List<Print_Item>();

            foreach (VwLineItemDetailViewDTO item in lineItemDetail)
            {
                Print_Item pItem = new Print_Item();
                pItem.ID = item.Article;
                pItem.Name = item.Name;
                if (item.UnitAmount == null) pItem.UnitPrice = 0;
                else pItem.UnitPrice = item.UnitAmount.Value;
                pItem.UnitPrice = pItem.UnitPrice < 0 ? 0.01m : pItem.UnitPrice;
                pItem.Quantity = (decimal)item.Quantity;
                pItem.taxId = tax.Id;
                //pItem.DiscountAmount = POSSettingCache.TotalDiscount;
                //pItem.ServiceChargeAmount = ServiceCharge;

                printItemList.Add(pItem);
            }


            return printItemList;

        }

        #region Save Current Voucher

        private VoucherBuffer SaveCurrentVoucher()
        {
            VoucherBuffer Savedbuffer = null;
            // Progress_Reporter.Show_Progress("Saving Voucher", "Please Wait..", 1, 6);
            DateTime? currentTime = UIProcessManager.GetServiceTime();
            if (currentTime == null)
            {
                CurrentTime = DateTime.Now;
            }
            else
            {
                CurrentTime = currentTime.Value;
            }
            if (string.IsNullOrEmpty(_currentVoucherCode)) return null;

            VoucherBuffer voucherbuffer = new VoucherBuffer();

            if (voFinal.voucher == null)
                voucherbuffer = new VoucherBuffer();
            else
                voucherbuffer.Voucher = voFinal.voucher;

            voucherbuffer.Voucher.Type = CNETConstantes.TRANSACTIONTYPENORMALTXN;
            voucherbuffer.Voucher.Definition = CNETConstantes.CHECK_OUT_BILL_VOUCHER;
            voucherbuffer.Voucher.Type = CNETConstantes.TRANSACTIONTYPENORMALTXN;
            voucherbuffer.Voucher.LastState = CNETConstantes.OSD_PREPARED_STATE;
            voucherbuffer.Voucher.Code = _currentVoucherCode;

            if (customerDto.Code == null)
                voucherbuffer.Voucher.Consignee1 = RegExtension.GuestId;
            else
                voucherbuffer.Voucher.Consignee1 = customerDto.ID;


            voucherbuffer.Voucher.IssuedDate = CurrentTime;
            voucherbuffer.Voucher.Year = CurrentTime.Year;
            voucherbuffer.Voucher.Day = CurrentTime.Day;
            voucherbuffer.Voucher.Month = CurrentTime.Month;
            voucherbuffer.Voucher.IsIssued = true;
            voucherbuffer.Voucher.IsVoid = false;
            voucherbuffer.Voucher.GrandTotal = GrandTotal;
            voucherbuffer.Voucher.Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime);
            voucherbuffer.Voucher.Note = "check_out";
            voucherbuffer.Voucher.OriginConsigneeUnit = regExtension.ConsigneeUnit;
            voucherbuffer.Voucher.FsNumber = POS_Settings.CurrentFSNo;
            voucherbuffer.Voucher.Mrc = POS_Settings.fiscalprinterMRC;

            int? paymentmethod = null;

            if (lePayment.EditValue != null && !string.IsNullOrEmpty(lePayment.EditValue.ToString()))
                paymentmethod = Convert.ToInt32(lePayment.EditValue);

            voucherbuffer.Voucher.PaymentMethod = paymentmethod;



            voucherbuffer.LineItemsBuffer = new List<LineItemBuffer>();
            LineItemBuffer lineItemBuffer = new LineItemBuffer();
            //  LineItem 
            if (voFinal != null && voFinal.lineItemDetails != null)
            {
                LineItemDetails lDetails = voFinal.lineItemDetails.FirstOrDefault();
                lineItemBuffer.LineItem = lDetails.lineItems;
                lineItemBuffer.LineItem.ObjectState = null;
                if (lDetails.lineItemValueFactor != null && lDetails.lineItemValueFactor.Count > 0)
                {
                    var addvalue = lDetails.lineItemValueFactor.Where(x => x.IsDiscount == false);

                    if (addvalue != null && addvalue.Count() > 0)
                    {
                        lineItemBuffer.LineItem.AddCharge = addvalue.Sum(x => x.Amount);
                    }
                    var discvalue = lDetails.lineItemValueFactor.Where(x => x.IsDiscount == true);

                    if (discvalue != null && discvalue.Count() > 0)
                    {
                        lineItemBuffer.LineItem.Discount = discvalue.Sum(x => x.Amount);
                    }

                }

                lineItemBuffer.LineItemValueFactors = new List<LineItemValueFactorDTO>();
                lineItemBuffer.LineItemValueFactors.AddRange(lDetails.lineItemValueFactor);


                voucherbuffer.LineItemsBuffer.Add(lineItemBuffer);
            }


            //saving Transaction Currency
            var defCurrency = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.IsDefault);
            if (defCurrency != null)
            {
                voucherbuffer.TransactionCurrencyBuffer = new TransactionCurrencyBuffer();
                voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency = new TransactionCurrencyDTO()
                {
                    Currency = defCurrency.Id,
                    Rate = 1,
                    Amount = voucherbuffer.Voucher.GrandTotal,
                    Total = voucherbuffer.Voucher.GrandTotal

                };

            }
            else
            {
                voucherbuffer.TransactionCurrencyBuffer = null;
            }

            voucherbuffer.TaxTransactions = new List<TaxTransactionDTO>();
            if (voFinal.taxTransactions != null && voFinal.taxTransactions.Count > 0)
            {
                foreach (var taxTransaction in voFinal.taxTransactions)
                    voucherbuffer.TaxTransactions.Add(taxTransaction);

            }
            voucherbuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
            TransactionReferenceBuffer TrBuffer = new TransactionReferenceBuffer();
            TrBuffer.TransactionReference = new TransactionReferenceDTO
            {
                ReferencingVoucherDefn = voucherbuffer.Voucher.Definition,
                Referenced = RegExtension.Id,
                ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                Value = voucherbuffer.Voucher.GrandTotal,
            };
            TrBuffer.ReferencedActivity = null;
            voucherbuffer.TransactionReferencesBuffer.Add(TrBuffer);
            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Setup Activity For Checkout.");
            voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, _activityDefCheckout, CNETConstantes.PMS_Pointer);
            voucherbuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
            voucherbuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;

            var data = UIProcessManager.CreateVoucherBuffer(voucherbuffer);
            if (data == null || !data.Success)
            {
                SystemMessage.ShowModalInfoMessage("Fail Saving !! " + Environment.NewLine + data.Message, "Error");
                return null;
            }
            else
            {
                return data.Data;
            }
        }


        #endregion

        private void Reset()
        {
            teGrandTotal.Text = "";
            teSubTotal.Text = "";
            teSerCharge.Text = "";
            teVAT.Text = "";
            tediscount.Text = "";
        }

        public void GetVoucherSettingValues(int voDef)
        {
            string lastSettingInfoWithError = "";
            var configurationList = LocalBuffer.LocalBuffer.ConfigurationBufferList.Where(c => c.Reference == voDef.ToString()).ToList();
            if (configurationList != null && configurationList.Count <= 0)
            {
                XtraMessageBox.Show("There is no setting values for the current voucher type", "CNET ERP",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                //foreach (var config in configurationList)
                //{
                //    lastSettingInfoWithError = config.Attribute;
                //    switch (config.Attribute)
                //    {
                //        case "Value is Tax Inclusive":
                //            LocalBuffer.LocalBuffer.valuesAreTaxInclusive = Convert.ToBoolean(config.currentValue);
                //            break;
                //        case "print without preview":
                //            LocalBuffer.LocalBuffer.printWithoutPreview = Convert.ToBoolean(config.currentValue);
                //            break;
                //        case "additional charge value":
                //            _additionalChargeAmt = Convert.ToDecimal(config.currentValue);
                //            _isAdditionalInPercent = true;
                //            break;
                //        case "additional charge type":
                //            LocalBuffer.LocalBuffer.AdditionalChargeType = config.currentValue;

                //            break;
                //        case "flexible additional charge":
                //            LocalBuffer.LocalBuffer.Additionalchargeflexible = Convert.ToBoolean(config.currentValue);
                //            break;

                //        case "Round Digit Total":
                //            _roundCalculatorDigit = Convert.ToInt32(config.currentValue);
                //            break;
                //    }
                //}
            }
            catch (Exception ex)
            {

                XtraMessageBox.Show("Since  voucher settings for this voucher are not properly settled" + "\n" + "some functionalities may not work properly!" + "\n" + ex.Message + ":" + "\n" + lastSettingInfoWithError, "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void PopulateCustomerDTO(int? companyCode)
        {
            customerDto = new Consignee();

            //  if (companyCode == null) return;

            if (RegExtension == null) return;

            if (companyCode != null)
            {
                ConsigneeDTO customer = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == companyCode);
                if (customer != null)
                {
                    if (customer != null)
                    {
                        if (customer.Code != null)
                        {
                            customerDto.ID = customer.Id;
                            customerDto.Code = customer.Code;
                            customerDto.Name = customer.FirstName;
                            customerDto.TIN = customer.Tin;
                            customerDto.ConsigneeType = "Company";
                            teTIN.Text = customerDto.TIN;
                            beiCompanyName.EditValue = customer.FirstName;

                        }
                    }

                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get Company information!", "ERROR");
                    this.Close();
                    return;
                }

                if (string.IsNullOrEmpty(customerDto.TIN))
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get Company's TIN Number!", "ERROR");
                    this.Close();
                    return;
                }
            }
            else
            {
                if (RegExtension.GuestId != null)
                {
                    customerDto.ID = RegExtension.GuestId.Value;
                    customerDto.Name = RegExtension.Guest;
                    customerDto.ConsigneeType = "Guest";
                }
            }


            if (string.IsNullOrEmpty(customerDto.TIN))
                statusLabel.Text = statusMessage;
            teConsignee.Text = customerDto.Name;
        }

        private bool CheckWorkflow()
        {
            ActivityDefinitionDTO _checkoutSalesWorkFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.CHECK_OUT_BILL_VOUCHER).FirstOrDefault();

            if (_checkoutSalesWorkFlow != null)
            {
                _activityDefCheckout = _checkoutSalesWorkFlow.Id;
                return true;
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("Please define workflow for Check Out Bill Voucher ", "ERROR");
                return false;
            }
            return true;
        }

        private bool CheckTINStatus(string tinNumber)
        {
            if (string.IsNullOrEmpty(tinNumber)) return true;
            int length = tinNumber.Trim().Length;
            switch (POS_Settings.printerType.ToLower())
            {
                case "datecs_fp_60":
                    if (length != 10)
                    {
                        SystemMessage.ShowModalInfoMessage("TIN Number length must be 10 digit", "ERROR");
                        return false;
                    }
                    break;
                case "datecs_fp_700":
                case "datecs_fp60_new":
                    if (length == 10 || length == 12 || length == 13)
                    {

                        return true;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("TIN Number length must be 10 digit", "ERROR");
                        return false;
                    }
                default:
                    return true;
            }

            return true;
        }


        #endregion


        /*************** EVENT HANDLERS **************************/
        #region Event Handlers

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                PMSDataLogger.LogMessage("frmFrontOfficePOS", "Checking Out " + voucherbuffer.Voucher.Code + " Registration !! ");

                PMSDataLogger.LogMessage("frmFrontOfficePOS", "Print Button Clicked FrontOfficePOS Form for Receipt");
                //Check TIN Mandatory
                if (regExtension.CompanyId != null)
                {
                    if (_isTinMandatory && string.IsNullOrEmpty(customerDto.TIN))
                    {
                        ////CNETInfoReporter.Hide();
                        XtraMessageBox.Show("Customer TIN is mandatory!", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // POSSettingCache .voucherDefinationCode =
                _currentVoucherCode = GetCurrentId(0);
                if (string.IsNullOrEmpty(_currentVoucherCode))
                {
                    SystemMessage.ShowModalInfoMessage("Unable to generate ID!", "ERROR");
                    return;
                }
                //_currentFsNumber = ReadFSForPrinting();

                //if (!POSSettingCache.printTinAsCommercial && !CheckTINStatus(cutomerDto.TINNumber))
                //{
                //    this.Close();
                //    return;

                //}


                //get non-printed list
                var nonPrintedTranfList = UIProcessManager.GetCheckOutDetailViewByPrintStatus(RegExtension.Id, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, Convert.ToInt32(lukWindow.EditValue), 1);
                if (!IsReinstate && (nonPrintedTranfList == null || nonPrintedTranfList.Count == 0))
                {
                    SystemMessage.ShowModalInfoMessage("All Room Charges have been Printed!", "ERROR");
                    return;
                }

                List<int> tranRefList = nonPrintedTranfList.Select(t => t.TranId).ToList();



                POS_Settings.IsError = true;
                POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                //POS_Settings.Voucher_Definition = CNETConstantes.CHECK_OUT_BILL_VOUCHER; ;
                POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                // FiscalPrinters FP = new FiscalPrinters();
                FiscalPrinters.GetInstance();

                if (!POS_Settings.IsError)
                {
                    XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool validatePMSLicense = CommonLogics.Validate_PMSPOS_License();
                if (!validatePMSLicense)
                    return;


                //get print items
                List<VwLineItemDetailViewDTO> dtoList = gvFrontOfficePOS.DataSource as List<VwLineItemDetailViewDTO>;
                if (dtoList != null && dtoList.Count > 0)
                {
                    if (_printItems != null) _printItems.Clear();
                    _printItems = GetPrintItems(dtoList, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get print Items", "ERROR");
                    return;
                }

                if (_printItems == null || _printItems.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get print Items", "ERROR");
                    return;
                }
                if (_printItems.Sum(x => x.UnitPrice) <= 0)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get print Items Price !!", "ERROR");
                    return;
                }


                //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Printing Receipt FrontOfficePOS Form");
                //bool isprinted = FiscalPrinters.PrintOperation(_printItems, POSSettingCache.printerType, new List<CustomerDTO>() { cutomerDto }, 0, _currentVoucherCode, "",
                //     "", LocalBuffer.LocalBuffer.CurrentLoggedInUser.userName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, RegExtension.Room, RegExtension.Registration);
               // Progress_Reporter.Show_Progress("Printing Recipt", "Please Wait.....");
                /*
                                bool isprinted = FiscalPrinters.PrintOperation(POS_Settings.Voucher_Definition, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, _currentVoucherCode,
                                                    null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucherbuffer.Voucher.IssuedDate, 0, RegExtension.Room, RegExtension.Registration);



                 */

                //bool isprinted = FiscalPrinters.PrintOperation(_printItems, POSSettingCache.printerType, POSSettingCache.defultPaymnet, new List<CustomerDTO>() { cutomerDto},
                //    0, _currentVoucherCode, "", "", CurrentUser.userName, Discount, ServiceCharge, RegExtension.Room, Voucher.code);
                // Progress_Reporter.Show_Progress("Saving Voucher.", "Please Wait...");

                //if (isprinted)
                //{

                //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Printed Done Receipt FrontOfficePOS Form");
                ////change the transaction Reference value flag to 1 for printed vouchers
                //try
                //{
                //    Progress_Reporter.Show_Progress("Updating Transaction Reference", "Please Wait.....");
                //    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Updating Transaction Reference ");
                //    foreach (var tRefCode in tranRefList)
                //    {
                //        TransactionReferenceDTO tr = UIProcessManager.GetTransactionReferenceById(tRefCode);
                //        if (tr == null) continue;
                //        tr.Value = 1;
                //        UIProcessManager.UpdateTransactionReference(tr);
                //    }

                //}
                //catch
                //{
                //    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Error Updating Transaction Reference ");

                //}


                Progress_Reporter.Show_Progress("Saving Checkout Bill", "Please Wait.....");
                PMSDataLogger.LogMessage("frmFrontOfficePOS", "Saving Checkout Bill FrontOfficePOS Form");
                VoucherBuffer isVoucherSaved = SaveCurrentVoucher();

                if (isVoucherSaved == null)
                {
                    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Saving Checkout Bill Fail FrontOfficePOS Form");
                    XtraMessageBox.Show("Checkout Voucher is not saved!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    POS_Settings.TotalServiceCharge = isVoucherSaved.Voucher.AddCharge;
                    POS_Settings.TotalDiscount = isVoucherSaved.Voucher.Discount;

                    Progress_Reporter.Show_Progress("Printing Recipt", "Please Wait.....");
                    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Done Saving Checkout Bill FrontOfficePOS Form");

                    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Printing Receipt FrontOfficePOS Form");
                    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Total Service Charge" + POS_Settings.TotalServiceCharge);
                    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Total Discount" + POS_Settings.TotalDiscount);

                    bool isprinted = true;

                    if (LocalBuffer.LocalBuffer.PMSRecieptIsCheckout)
                        isprinted = FiscalPrinters.PrintOperation(POS_Settings.Voucher_Definition, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, _currentVoucherCode,
                                         null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucherbuffer.Voucher.IssuedDate, 0, RegExtension.Room, RegExtension.Registration);
                    if (!isprinted)
                    {
                        PMSDataLogger.LogMessage("frmFrontOfficePOS", "Printed Fail Receipt FrontOfficePOS Form");
                        XtraMessageBox.Show("Voucher is not printed", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ////CNETInfoReporter.Hide();


                        PMSDataLogger.LogMessage("frmFrontOfficePOS", "Delete Checkout Voucher " + isVoucherSaved.Voucher.Code + " FrontOfficePOS Form");

                        var dele = UIProcessManager.DeleteVoucherObjects(isVoucherSaved.Voucher.Id);

                        if (dele != null && dele.Data)
                            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Delete Success Checkout Voucher " + isVoucherSaved.Voucher.Code + " FrontOfficePOS Form");
                        else
                            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Delete Fail Checkout Voucher " + isVoucherSaved.Voucher.Code + " FrontOfficePOS Form");

                        DialogResult = DialogResult.No;
                        this.Close();
                        return;
                    }
                    else
                    {


                        PMSDataLogger.LogMessage("frmFrontOfficePOS", "Printed Done Receipt FrontOfficePOS Form");
                        //change the transaction Reference value flag to 1 for printed vouchers
                        try
                        {
                            Progress_Reporter.Show_Progress("Updating Transaction Reference", "Please Wait.....");
                            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Updating Transaction Reference ");
                            foreach (var tRefCode in tranRefList)
                            {
                                TransactionReferenceDTO tr = UIProcessManager.GetTransactionReferenceById(tRefCode);
                                if (tr == null) continue;
                                tr.Value = 1;
                                UIProcessManager.UpdateTransactionReference(tr);
                            }

                        }
                        catch
                        {
                            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Error Updating Transaction Reference ");

                        }



                        _currentVoucherCode = GetCurrentId(1);

                        if (LocalBuffer.LocalBuffer.PMSRecieptIsCheckout)
                        {
                            VoucherDTO voucher = UIProcessManager.Patch_FS_No(isVoucherSaved.Voucher.Id, POS_Settings.CurrentFSNo, POS_Settings.fiscalprinterMRC);

                            if (voucher == null)
                                PMSDataLogger.LogMessage("frmFrontOfficePOS", "Fail Checkout Patch FrontOfficePOS Form");
                        }
                    }
                }




                //get non-printed list for all windows
                var allNonPrintedTranfList = UIProcessManager.GetCheckOutDetailViewByPrintStatus(RegExtension.Id, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, null, 1);
                if (allNonPrintedTranfList == null || allNonPrintedTranfList.Count == 0)
                {


                    //Update Voucher 
                    GuestLedgerDTO gLedger = UIProcessManager.GetGuestLedger(RegExtension.Id,
                    RegExtension.Arrival.Date, RegExtension.Departure.Date, RegExtension.Room, null);

                    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Checking Guest ledger FrontOfficePOS Form");
                    if (gLedger != null && gLedger.RemainingBalance > 1)
                    {
                        if (!_isDirectBill)
                        {
                            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Registration is not Direct Bill FrontOfficePOS Form");
                            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Giving Choice to Change CHECKED_OUT_STATE");
                            DialogResult dr = XtraMessageBox.Show("There is unpaid remaining balance! Do you want to change to checkout state?", "Check-Out", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (dr == System.Windows.Forms.DialogResult.Yes)
                            {
                                PMSDataLogger.LogMessage("frmFrontOfficePOS", "Seleted Yes to CHECKED_OUT_STATE");
                                voucherbuffer.Voucher.Extension1 = null;
                                voucherbuffer.Voucher.LastState = CNETConstantes.CHECKED_OUT_STATE;
                                PMSDataLogger.LogMessage("frmFrontOfficePOS", "Setup Activity For Checkout.");
                                voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, _activityDefCheckout, CNETConstantes.PMS_Pointer, "Non Direct Bill CheckOut");
                            }
                            else
                            {
                                PMSDataLogger.LogMessage("frmFrontOfficePOS", "Seleted No to CHECKED_OUT_STATE");
                                return;
                            }
                        }
                        else
                        {
                            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Set CHECKED_OUT_STATE in Registration Object");
                            voucherbuffer.Voucher.Extension1 = null;
                            voucherbuffer.Voucher.LastState = CNETConstantes.CHECKED_OUT_STATE;
                            PMSDataLogger.LogMessage("frmFrontOfficePOS", "Setup Activity For Checkout.");
                            voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, _activityDefCheckout, CNETConstantes.PMS_Pointer, "Direct Bill CheckOut");
                        }
                    }
                    else
                    {
                        PMSDataLogger.LogMessage("frmFrontOfficePOS", "Set LastObjectState CHECKED_OUT_STATE FrontOfficePOS Form");
                        voucherbuffer.Voucher.Extension1 = null;
                        voucherbuffer.Voucher.LastState = CNETConstantes.CHECKED_OUT_STATE;
                        PMSDataLogger.LogMessage("frmFrontOfficePOS", "Setup Activity For Checkout.");
                        voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, _activityDefCheckout, CNETConstantes.PMS_Pointer, "CheckOut");
                    }
                    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Transaction References Buffer ReferencedActivity FrontOfficePOS Form");
                    if (voucherbuffer.TransactionReferencesBuffer != null && voucherbuffer.TransactionReferencesBuffer.Count > 0)
                        voucherbuffer.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                    //voucherbuffer.Activity = ActivityLogManager.SetupActivity(CurrentTime, _activityDefCheckout, CNETConstantes.PMS_Pointer, "CheckOut");
                    PMSDataLogger.LogMessage("frmFrontOfficePOS", "Updating Registration Voucher FrontOfficePOS Form");


                    voucherbuffer.TransactionCurrencyBuffer = null;


                    ResponseModel<VoucherBuffer> isVoucherUpdated = UIProcessManager.UpdateVoucherBuffer(voucherbuffer);



                    if (isVoucherUpdated != null && isVoucherUpdated.Success)
                    {
                        PMSDataLogger.LogMessage("frmFrontOfficePOS", voucherbuffer.Voucher.Code + " Voucher " + voucherbuffer.Voucher.LastState + " STATE with Receipt Done");
                        //Update Room to Dirty State
                        List<RoomTypeDTO> pseudoRoomList = UIProcessManager.GetRoomTypeByispseudoRoomType(true);
                        List<int> pseudoRooms = new List<int>();
                        if (pseudoRoomList != null)
                            pseudoRooms = pseudoRoomList.Select(p => p.Id).ToList();
                        rd = UIProcessManager.GetRoomDetailById(RegExtension.RoomCode.Value);
                        if (rd != null)
                        {

                            //Get Activity Definition For Dirty
                            ActivityDefinitionDTO adDirty = UIProcessManager.GetActivityDefinitionBycomponetanddescription(CNETConstantes.PMS_Pointer, CNETConstantes.Dirty).FirstOrDefault();
                            if (adDirty != null)
                            {
                                if (pseudoRooms != null && !pseudoRooms.Contains(rd.RoomType))
                                {
                                    rd.LastState = adDirty.Id;
                                    UIProcessManager.UpdateRoomDetail(rd);
                                    logActiviy(rd);
                                }
                            }
                        }

                        if (voucherbuffer.Voucher.LastState == CNETConstantes.CHECKED_OUT_STATE)
                            XtraMessageBox.Show("State Successfully Changed To Check-Out!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);


                        //Synchronize 
                        this.Close();
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        PMSDataLogger.LogMessage("frmFrontOfficePOS", voucherbuffer.Voucher.Code + " Voucher Update CHECKED OUT STATE with Receipt Fail");
                        PMSDataLogger.LogMessage("frmFrontOfficePOS", "Voucher Update Fail !!" + isVoucherUpdated.Message);
                        XtraMessageBox.Show("Registration voucher is not updated. please try again." + Environment.NewLine + isVoucherUpdated.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    XtraMessageBox.Show("State is not changed. Please check-Out all windows!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    DialogResult = DialogResult.Ignore;
                }
                if (_printItems != null)
                    _printItems.Clear();
                ////CNETInfoReporter.Hide();
                //}

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Exception is occurred. please try again. DETAIL:: " + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ////CNETInfoReporter.Hide();

            }
            Progress_Reporter.Close_Progress();
        }
        private void logActiviy(RoomDetailDTO rmd)
        {
            try
            {
                ActivityDTO act = new ActivityDTO();
                act.ActivityDefinition = rmd.LastState.Value;
                act.TimeStamp = CurrentTime.ToLocalTime();
                act.Year = CurrentTime.Year;
                act.Month = CurrentTime.Month;
                act.Day = CurrentTime.Day;
                act.Reference = rmd.Id;
                act.Platform = "1";
                act.User = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                act.Device = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                act.ConsigneeUnit = regExtension.ConsigneeUnit;
                act.Remark = "Event Checkout";
                UIProcessManager.CreateActivity(act);
            }
            catch (Exception e) { }
        }
        private void frmFrontOfficePOS_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        private void cbeTransactionType_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit view = sender as ComboBoxEdit;
            if (view == null) return;
            if (view.SelectedIndex == 0)
            {
                POS_Settings.Voucher_Definition = CNETConstantes.CASH_SALES;
                POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                teVoucherNo.Text = GetCurrentId(0);
                if (string.IsNullOrEmpty(teVoucherNo.Text))
                {
                    SystemMessage.ShowModalInfoMessage("Unable to generate ID!", "ERROR");
                    bbiPrint.Enabled = false;
                }
                lePayment.EditValue = CNETConstantes.PAYMENTMETHODSCASH;
                lePayment.Properties.ReadOnly = false;
            }
            else
            {
                POS_Settings.Voucher_Definition = CNETConstantes.CREDITSALES;
                POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                teVoucherNo.Text = GetCurrentId(0);
                if (string.IsNullOrEmpty(teVoucherNo.Text))
                {
                    SystemMessage.ShowModalInfoMessage("Unable to generate ID!", "ERROR");
                    bbiPrint.Enabled = false;
                }
                lePayment.EditValue = CNETConstantes.PAYMENTMETHODSCREDIT;
                lePayment.Properties.ReadOnly = true;
            }

            //get Setting Values
            GetVoucherSettingValues(POS_Settings.Voucher_Definition);

            //check workflow for the selected transactiontype
            if (!CheckWorkflow())
            {
                bbiPrint.Enabled = false;
            }
            else
            {
                bbiPrint.Enabled = true;
            }

        }

        private void lePayment_EditValueChanged(object sender, EventArgs e)
        {
            //POSSettingCache.defultPaymnet = lePayment.Text;
        }


        private void gvFrontOfficePOS_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.Caption == "SN")
                e.Value = e.ListSourceRowIndex + 1;
        }

        private void bbiShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_isSummerized)
            {

                if (!PopulateSummaizedData())
                {
                    bbiPrint.Enabled = false;
                }
                else
                {
                    bbiPrint.Enabled = true;
                }

            }
            else
            {
                if (!PopulateDetailData())
                {
                    bbiPrint.Enabled = false;
                }
                else
                {
                    bbiPrint.Enabled = true;
                }
            }
        }

        private void bbiViewFolio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (lukWindow.EditValue == null) return;
                int selectedWindow = Convert.ToInt32(lukWindow.EditValue);
                SystemConstantDTO window = null;
                if (selectedWindow != -1)
                    window = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Id == selectedWindow);
                frmFolio _frmFolio = new frmFolio();
                _frmFolio.RegistrationExt = RegExtension;
                _frmFolio.DefaultWindow = window == null ? -1 : Convert.ToInt32(window.Value);
                _frmFolio.ShowDialog();
            }
            catch (Exception ex) { }
        }

        private void beiFiscalBillType_EditValueChanged(object sender, EventArgs e)
        {
            BarEditItem view = sender as BarEditItem;
            if (view == null) return;
            string selected = view.EditValue.ToString();
            if (selected == "Summary")
            {
                _isSummerized = true;
                gcolArticle.OptionsColumn.AllowEdit = false;
                gcolQuantity.OptionsColumn.AllowEdit = false;

                if (!PopulateSummaizedData())
                {
                    bbiPrint.Enabled = false;
                }
                else
                {
                    bbiPrint.Enabled = true;
                }


            }
            else if (selected == "Long Detail")
            {
                _isSummerized = false;
                gcolArticle.OptionsColumn.AllowEdit = false;
                gcolQuantity.OptionsColumn.AllowEdit = false;

                if (!PopulateDetailData())
                {
                    bbiPrint.Enabled = false;
                }
                else
                {
                    bbiPrint.Enabled = true;
                }

            }
            else if (selected == "Summary Edit")
            {
                _isSummerized = true;
                gcolArticle.OptionsColumn.AllowEdit = true;
                gcolQuantity.OptionsColumn.AllowEdit = true;

                if (!PopulateSummaizedData())
                {
                    bbiPrint.Enabled = false;
                }
                else
                {
                    bbiPrint.Enabled = true;
                }

            }
            else
            {
                //nothing
            }
        }

        private void RepoQtyEdit_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit view = sender as SpinEdit;

            if (view.EditValue == null) return;

            int value = Convert.ToInt32(view.EditValue.ToString());
            VwLineItemDetailViewDTO dto = gvFrontOfficePOS.GetFocusedRow() as VwLineItemDetailViewDTO;
            if (dto != null)
            {
                dto.Quantity = value;
                dto.UnitAmount = Math.Round(dto.TotalAmount.Value / value, 2);
                gvFrontOfficePOS.RefreshData();

            }
        }

        private void RepoArticleEdit_EditValueChanged(object sender, EventArgs e)
        {
            TextEdit view = sender as TextEdit;

            if (view.EditValue == null) return;

            string value = view.EditValue.ToString();

            VwLineItemDetailViewDTO dto = gvFrontOfficePOS.GetFocusedRow() as VwLineItemDetailViewDTO;
            if (dto != null)
            {
                // dto.article = dto;
                dto.Name = value;
                gvFrontOfficePOS.RefreshData();
            }
        }


        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                customerDto = null;
                VoucherBuffer = null;
                voFinal = null;
                RegExtension = null;

                if (lineItemDetail != null)
                {
                    lineItemDetail.Clear();
                    lineItemDetail = null;

                }

                rd = null;
                if (_printItems != null)
                {
                    _printItems.Clear();
                    _printItems = null;
                }
            }
            base.Dispose(disposing);
        }

        private void beiFiscalBillType_ItemClick(object sender, ItemClickEventArgs e)
        {

        }



    }
}