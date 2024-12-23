using CNET.ERP.Client.Common.UI; 
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using ERP.EventManagement;
using ERP.EventManagement.DTO;
using ProcessManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.EventManagement.Modals
{
    public partial class frmCashReceipt : Form
    {
        #region Declaration
        public int? _adPrepared { get; set; }
        public int? _obPrepared { get; set; }
        public int? defCurrency { get; set; }
        DateTime CurrentDate { get; set; }
        private int? _voucherExt { get; set; }
        VoucherDTO EventVo { get; set; }

        #endregion

        #region Constractor
        public frmCashReceipt(List<EventConsgineeDTO> ConsigneeList, VoucherDTO EventVoucher, Double Grandtotal)
        {
            InitializeComponent();

            EventVo = EventVoucher;
            //Progress_Reporter.Show_Progress("Getting data", "Please Wait...");
            var workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.CASHRECIPT);

            if (workFlow != null)
            {
                _adPrepared = workFlow.FirstOrDefault().Id;
                _obPrepared = workFlow.FirstOrDefault().State;
            }
            else
            {
                XtraMessageBox.Show("Please define workflow of PREPARED for Cash Recipt Voucher ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SetVoucherExtentions();


            leCurrency.Properties.DataSource = (LocalBuffer.LocalBuffer.CurrencyBufferList.OrderByDescending(c => c.IsDefault).ToList());
            leCurrency.Properties.DisplayMember = "Description";
            leCurrency.Properties.ValueMember = "Id";
            if (LocalBuffer.LocalBuffer.CurrencyBufferList != null)
            {
                var currency = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(c => c.IsDefault);
                if (currency != null)
                {
                    leCurrency.EditValue = (currency.Id);

                    defCurrency = currency.Id;
                }
            }



            //device = LoginPage.Authentication.DeviceObject;
            //currentUser = LoginPage.Authentication.GetAuthorizedUser();
            //Generated_ID id = UIProcessManager.IdGenerater("Voucher", device,
            //       CNETConstantes.CASHRECIPT.ToString(),
            //       CNETConstantes.VOUCHER_COMPONENET, 0);

            string id = UIProcessManager.IdGenerater("Voucher", CNETConstantes.CASHRECIPT, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(id))
            {
                txtVoucherCode.Text = id;
            }
            else
            {
                XtraMessageBox.Show("There is a problem on id setting! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }


            //if (id != null)
            //{
            //    txtVoucherCode.Text = id.GeneratedNewId;
            //}
            //else
            //{
            //    XtraMessageBox.Show("There is a problem on CASH RECIPT id setting!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    this.Close();
            //    return;
            //}

            txtEventCode.Text = EventVoucher.Code;


            sleCustomer.Properties.DataSource = ConsigneeList;
            sleCustomer.Properties.DisplayMember = "name";
            sleCustomer.Properties.ValueMember = "id";
            sleCustomer.EditValue = EventVoucher.Consignee1;

            leCustomer.DataSource = ConsigneeList;
            leCustomer.DisplayMember = "name";
            leCustomer.ValueMember = "id";


            List<SystemConstantDTO> paymentList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.PAYMENT_METHODS && l.IsActive && l.Id != CNETConstantes.PAYMENTMETHODS_DIRECT_BILL).ToList();
            lePaymentType.Properties.DataSource = (paymentList.OrderByDescending(c => c.IsDefault).ToList());
            lePaymentType.Properties.DisplayMember = "Description";
            lePaymentType.Properties.ValueMember = "Id";
            SystemConstantDTO cashpayment = paymentList.FirstOrDefault(x => x.Description.ToLower() == "cash");
            if (cashpayment != null)
            {
                lePaymentType.EditValue = cashpayment.Id;
            }
            else
            {
                lePaymentType.EditValue = paymentList.FirstOrDefault().Id;
            }
            CurrentDate = UIProcessManager.GetServiceTime().Value;
            deIssuedDate.EditValue = CurrentDate;
            PopulatePaymentHistory();
        }
        #endregion

        #region Methods
        public void SetVoucherExtentions()
        {
            VoucherExtensionDefinitionDTO vExt = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(ve => ve.Index == 0 && ve.VoucherDefinition == CNETConstantes.CASHRECIPT &&
                ve.Type == CNETConstantes.VOUCHER_RELATION_TYPE_VOUCHER_EXT);

            if (vExt == null)
            {
                txtReference.Enabled = false;
                return;

            }
            if (vExt.ExDataType == "String" || vExt.ExDataType == "string")
            {
                _voucherExt = vExt.Id;
                string label = string.IsNullOrEmpty(vExt.Descritpion) ? "Reference" : vExt.Descritpion;
                if (vExt.IsMandatory)
                {
                    lc_reference.Text = label + " *";
                }
                else
                {
                    lc_reference.Text = label;
                }
            }
        }

        public void PopulatePaymentHistory()
        {
            //Progress_Reporter.Show_Progress("Getting Cash Reciving Payment History data", "Please Wait...");
            List<TransactionReferenceDTO> Payments = UIProcessManager.GetTransactionReferenceByreferenced(EventVo.Id);
            if (Payments != null && Payments.Count > 0)
            {
                VoucherDTO vou = null;
                List<VoucherDTO> PaymentVoucherList = new List<VoucherDTO>();
                foreach (TransactionReferenceDTO tran in Payments)
                {
                    vou = UIProcessManager.GetVoucherById(tran.Referring.Value);
                    if (vou != null && vou.Definition == CNETConstantes.CASHRECIPT)
                    {
                        PaymentVoucherList.Add(vou);
                    }
                }
                gcCashReceiptHistory.DataSource = PaymentVoucherList;
            }
            //Progress_Reporter.Close_Progress();
        }

        public static decimal GetLatestExchangeRate(int currency)
        {
            int? defaultCurrency = null;
            decimal exchangeRate = 1;
            var firstOrDefault = UIProcessManager.SelectAllCurrency().FirstOrDefault(r => r.IsDefault);
            if (firstOrDefault != null)
            {
                defaultCurrency = firstOrDefault.Id;
            }
            if (currency == defaultCurrency)
            {
                exchangeRate = 1;
            }
            else
            {
                var CNETLibrary = UIProcessManager.SelectAllExchangeRate()
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefault(r => r.FromCurrency == currency && (r.ToCurrency == defaultCurrency || r.ToCurrency == 0));
                if (CNETLibrary != null)
                    exchangeRate =
                        CNETLibrary
                            .Buying;
            }
            return exchangeRate;
        }

        private void leCurrency_EditValueChanged(object sender, EventArgs e)
        {
            decimal exRate = GetLatestExchangeRate(Convert.ToInt32(leCurrency.EditValue.ToString()));
            txtRate.EditValue = exRate;
            if (!string.IsNullOrEmpty(txtAmount.Text))
            {
                txtTotalAmount.Text = Math.Abs(Math.Round((exRate * Convert.ToDecimal(txtAmount.Text.ToString())), 2)).ToString();
            }
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            VoucherBuffer voucherbuffer = new VoucherBuffer();
            if (txtTotalAmount.EditValue == null || string.IsNullOrEmpty(txtTotalAmount.EditValue.ToString()))
            {
                XtraMessageBox.Show("Enter Amount First ! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Decimal tottal = Convert.ToDecimal(txtTotalAmount.EditValue.ToString());
                if (tottal == 0)
                {
                    XtraMessageBox.Show("Amount must be greater than Zero  ! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
            }



            if (lc_reference.Text.Contains("*") && (txtReference.EditValue == null || string.IsNullOrEmpty(txtReference.EditValue.ToString())))
            {
                XtraMessageBox.Show("Please Enter " + lc_reference.Text + " First ! ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }






            //Progress_Reporter.Show_Progress("Saving Voucher....", "Please Wait....");
            CurrentDate = UIProcessManager.GetServiceTime().Value;

            string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.CASHRECIPT, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(currentVoCode))
            {
                txtVoucherCode.Text = currentVoCode;
            }
            else
            {
                MessageBox.Show("There is a problem on id setting!!!", "ERROR");
                ////CNETInfoReporter.Hide();
                return;
            }
            voucherbuffer.Voucher.Code = txtVoucherCode.Text;
            voucherbuffer.Voucher.Definition = CNETConstantes.CASHRECIPT;
            voucherbuffer.Voucher.Type = CNETConstantes.TRANSACTIONTYPENORMALTXN;
            voucherbuffer.Voucher.IssuedDate = CurrentDate;
            voucherbuffer.Voucher.Year = CurrentDate.Year;
            voucherbuffer.Voucher.Month = CurrentDate.Month;
            voucherbuffer.Voucher.Day = CurrentDate.Day;
            voucherbuffer.Voucher.IsVoid = false;
            voucherbuffer.Voucher.Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentDate);
            voucherbuffer.Voucher.LastState = _obPrepared.Value;

            if (sleCustomer.EditValue != null && !string.IsNullOrEmpty(sleCustomer.EditValue.ToString()))
            {
                int cons = 0;

                int.TryParse(sleCustomer.EditValue.ToString(), out cons);
                voucherbuffer.Voucher.Consignee1 = cons;
            }

            voucherbuffer.Voucher.IsIssued = true;

            decimal grandTotal = string.IsNullOrEmpty(txtTotalAmount.Text) ? 0 : Convert.ToDecimal(txtTotalAmount.Text);
            if (grandTotal == 0)
            {
                ////CNETInfoReporter.Hide();
                MessageBox.Show("Invalid Input", "ERROR");
                return;
            }


            voucherbuffer.Voucher.SubTotal = Math.Abs(grandTotal);
            voucherbuffer.Voucher.GrandTotal = Math.Round(grandTotal, 2);



            //save non cash transaction
            frmNonCashPayments frmNonCash = null;
            if (lePaymentType.EditValue != null && Convert.ToInt32(lePaymentType.EditValue.ToString()) != CNETConstantes.PAYMENTMETHODSCASH)
            {
                frmNonCash = new frmNonCashPayments();
                frmNonCash.Show_Non_Cash_Payment_Form(txtVoucherCode.Text, voucherbuffer.Voucher.GrandTotal, sleCustomer.Text, CurrentDate, Convert.ToInt32(lePaymentType.EditValue));

                //frmNonCash = new frmNonCashPayments();
                //NonCashDTO ncVM = new NonCashDTO()
                //{
                //    ReferenceNum = txtVoucherCode.Text,
                //    SelectedConsignee = sleCustomer.EditValue != null ? sleCustomer.EditValue.ToString() : string.Empty,
                //    CurrencyCode = leCurrency.EditValue != null ? leCurrency.EditValue.ToString() : string.Empty,
                //    CurrencyDesc = leCurrency.Text,
                //    PaymentTypeCode = lePaymentType.EditValue != null ? lePaymentType.EditValue.ToString() : string.Empty,
                //    PaymentTypeDesc = lePaymentType.Text,
                //    ReceivedAmount = txtTotalAmount.Text,
                //    ReceivedDate = CurrentDate,
                //    ConsigneeList = (List<EventConsgineeDTO>)sleCustomer.Properties.DataSource,
                //    CurrencyRate = Convert.ToDecimal(txtRate.Text)
                //};

                //frmNonCash.NonCashVM = ncVM;
                //frmNonCash.CashRecivingVoucher = txtEventCode.Text;
                if (frmNonCash.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                else
                {
                    if (lePaymentType.EditValue != null && Convert.ToInt32(lePaymentType.EditValue) == CNETConstantes.PAYMNET_METHOD_CREDITCARD)
                    {
                        txtAmount.Text = frmNonCash.Non_Cash_Payment.payment_Amount.ToString();
                    }
                    //var paymentProcessor = UIProcessManager.Get_ConsigneeUnit_By_Code(frmMobilePayments.Mobile_Transaction.PaymentProcessorConsigneeUnit.ToString());
                    voucherbuffer.Voucher.PaymentMethod = Convert.ToInt32(lePaymentType.EditValue);
                    voucherbuffer.Voucher.PaymentProcessor = frmNonCash.Non_Cash_Payment.payment_Processor /* paymentProcessor != null ? paymentProcessor.Id : null*/;
                    voucherbuffer.Voucher.Payer = voucherbuffer.Voucher.Consignee1;
                    voucherbuffer.Voucher.IsIncoming = true;
                    voucherbuffer.Voucher.PaymentIssueDate = frmNonCash.Non_Cash_Payment.payment_Date;
                    voucherbuffer.Voucher.PaymentMaturityDate = frmNonCash.Non_Cash_Payment.maturity_Date;
                    voucherbuffer.Voucher.PaymentRefNumber = frmNonCash.Non_Cash_Payment.payment_Reference;
                    voucherbuffer.Voucher.PaymentAmount = voucherbuffer.Voucher.GrandTotal;
                    voucherbuffer.Voucher.PaymentStatus = CNETConstantes.Payment_Status_Executed;
                    voucherbuffer.Voucher.Currency = frmNonCash.Non_Cash_Payment.currency;
                }
            }



            //Check Reference Number


            if (txtReference.EditValue != null && !string.IsNullOrEmpty(txtReference.EditValue.ToString()))
            {
                var configValidateReference = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_ValidateReference);
                if (configValidateReference != null)
                {
                    bool flag = Convert.ToBoolean(configValidateReference.CurrentValue);
                    if (flag)
                    {
                        //validate refrence
                        var voExtTranList = UIProcessManager.GetVoucherByExtension1(txtReference.EditValue.ToString());
                        if (voExtTranList != null && voExtTranList.Count > 0)
                        {
                            //////Progress_Reporter.Close_Progress();
                            XtraMessageBox.Show("The External Reference Number is already exist!", "ERROR");
                            return;
                        }
                    }
                }

            }

            if (_voucherExt != null && !string.IsNullOrEmpty(txtReference.Text))
                voucherbuffer.Voucher.Extension1 = txtReference.Text;

            voucherbuffer.Voucher.OriginConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
            voucherbuffer.TaxTransactions = new List<TaxTransactionDTO>();


            voucherbuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
            TransactionReferenceBuffer TrBuffer = new TransactionReferenceBuffer();
            TrBuffer.TransactionReference = new TransactionReferenceDTO();
            TrBuffer.TransactionReference.ReferencedVoucherDefn = EventVo.Definition;
            TrBuffer.TransactionReference.Referenced = EventVo.Id;
            TrBuffer.TransactionReference.ReferencingVoucherDefn = voucherbuffer.Voucher.Definition;
            TrBuffer.TransactionReference.RelationType = null;
            TrBuffer.TransactionReference.Value = grandTotal;
            TrBuffer.ReferencedActivity = null;
            voucherbuffer.TransactionReferencesBuffer.Add(TrBuffer);


            voucherbuffer.Activity = SetupActivity(CurrentDate, _adPrepared.Value, CNETConstantes.VOUCHER_COMPONENET);

            int? paymentmet = null;

            if (lePaymentType.EditValue != null && !string.IsNullOrEmpty(lePaymentType.EditValue.ToString()))
                paymentmet = Convert.ToInt32(lePaymentType.EditValue);



            voucherbuffer.Voucher.PaymentMethod = paymentmet;
            voucherbuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
            voucherbuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;

            ResponseModel<VoucherBuffer> saved = UIProcessManager.CreateVoucherBuffer(voucherbuffer);

            if (saved != null && saved.Success)
            {
                XtraMessageBox.Show("Saved Successfully", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try
                {
                    DocumentPrint.ReportGenerator reportGenerator = new DocumentPrint.ReportGenerator();
                    reportGenerator.GetAttachementReport(saved.Data.Voucher.Id);
                }
                catch (Exception ex)
                {

                }
                txtAmount.EditValue = 0;
                PopulatePaymentHistory();
                leCurrency.EditValue = defCurrency;
                txtReference.EditValue = null;
            }
            else
            {
                XtraMessageBox.Show("This voucher is not issued!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


            //Progress_Reporter.Close_Progress();
        }
        public static ActivityDTO SetupActivity(DateTime serverTimeStamp, int activityDefCode, int compCode, string remark = "", int? userCode = null)
        {
            //bool isExist = IsExistActDefinitionCode(activityDefCode);

            //if (!isExist) return null;

            UserDTO currentUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;
            DeviceDTO device = LocalBuffer.LocalBuffer.CurrentDevice;

            ActivityDTO activity = new ActivityDTO()
            {
                ConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit,
                TimeStamp = serverTimeStamp,
                Year = serverTimeStamp.Year,
                ActivityDefinition = activityDefCode,
                Month = serverTimeStamp.Month,
                Day = serverTimeStamp.Day,
                Device = device.Id,
                Pointer = compCode,
                Platform = "1",
                User = userCode == null ? currentUser.Id : userCode.Value,
                Remark = remark
            };

            return activity;
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void gvCashReceiptHistory_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.Caption == "SN")
                e.DisplayText = (e.RowHandle + 1).ToString();
        }
        #endregion
    }


    public class NonCashVM
    {
        public string ReferenceNum { get; set; }
        public string ReceivedAmount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyDesc { get; set; }
        public string PaymentTypeCode { get; set; }
        public string PaymentTypeDesc { get; set; }
        public string SelectedConsignee { get; set; }
        public decimal CurrencyRate { get; set; }
        public List<ConsigneeDTO> ConsigneeList { get; set; }
    }
}
