using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CNET.Client.Common;
using CNET.ERP.Client.Common.UI;


using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

using System.Text.RegularExpressions;
using CNET.Client.Common.UserControls.Data.Data_Manager;

using CNET.Client.Common.UserControls.Data.Data_Cache;
using CNET.ERP.Client.UI_Logic.PMS.Forms.Non_Navigatable_Modals;
using DevExpress.XtraBars;
using POSSetting;
using FPTool;
using CNET.ERP.Client.UI_Logic.PMS.Forms.CommonClass;
using DocumentPrint;
using DevExpress.XtraGrid.Views.Grid;

namespace CNET.ERP.EventManagement.Modals
{
    public partial class frmFrontOfficePOS : XtraForm
    {
        Generated_ID generatedId { get; set; }
        DateTime CurrentTime = DateTime.Now;
        private VoucherFinal voFinal = new VoucherFinal();
        public Voucher EventVoucher { get; set; }
        public string EventOwnerName { get; set; }
        public string EventOwnerCode { get; set; }
        public string EventCode { get; set; }

        public List<EventRequirementList> EventRequirementList { get; set; }
        public List<EventRequirementList> EventRequirementSummaryList { get; set; }

        public frmFrontOfficePOS()
        {
            InitializeComponent();
            CheckWorkflow();
            CurrentTime = UIProcessManager.GetDataTime().Timestamp;
            deDate.EditValue = CurrentTime;

            string VoucherID = GetCurrentId(0);
            if (string.IsNullOrEmpty(VoucherID))
            {
                XtraMessageBox.Show("Unable to generate ID!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            teVoucherNo.EditValue = VoucherID;

            cbeTransactionType.EditValue = "Cash Sales";

        }
        public string GetCurrentId(int generationType)
        {

            Device device = LoginPage.Authentication.DeviceObject;
            Generated_ID id = UIProcessManager.IdGenerater("Voucher", device,
                CNETConstantes.CHECK_OUT_BILL_VOUCHER.ToString(),
                CNETConstantes.VOUCHER_COMPONENET, generationType);
            if (id != null)
            {
                generatedId = id;
                return id.GeneratedNewId;
            }
            return "";
        }
        CustomerDTO cutomerDto { get; set; }
        private void frmFrontOfficePOS_Load(object sender, EventArgs e)
        {
            if (EventRequirementList != null)
            {
                GetVoucherFinal();
                PopulateVoucherFinal();
                EventVoucher = UIProcessManager.SelectVoucher(EventCode);
                EventOwnerCode = EventVoucher.consignee;
                teConsignee.EditValue = EventOwnerName;


                if (!string.IsNullOrEmpty(EventVoucher.consignee))
                {
                    vw_VoucherConsignee consignee = LoginPage.Authentication.AllVoucherConsigneeBufferList.FirstOrDefault(x => x.code == EventVoucher.consignee);
                    if (EventVoucher.consignee != null)
                    {
                        cutomerDto = new CustomerDTO()
                         {
                             Code = EventVoucher.consignee,
                             Name = consignee.name,
                             TINNumber = (consignee.IdentficationType == CNETConstantes.EMPLOYEE_IDENTIFICATION_TIN || consignee.IdentficationType == CNETConstantes.ORGANIZATION_IDENTIFICATION_TYPETIN) ? consignee.idNumber : "",
                             Componenet = CNETConstantes.ORGANIZATION,
                         };
                        if (consignee.IdentficationType == CNETConstantes.EMPLOYEE_IDENTIFICATION_TIN || consignee.IdentficationType == CNETConstantes.ORGANIZATION_IDENTIFICATION_TYPETIN)
                            teTIN.EditValue = consignee.idNumber;
                    }

                }

            }
        }

        string _activityDefCheckout { get; set; }
        string _objectstateCheckout { get; set; }

        private bool CheckWorkflow()
        {
            vw_WorkFlowByReferenceView _checkoutSalesWorkFlow = UIProcessManager.GetWorkFlowsByreference(CNETConstantes.CHECK_OUT_BILL_VOUCHER.ToString(),
               CNETConstantes.VOUCHER_COMPONENET).Where(w => w.description == CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED)
               .FirstOrDefault();

            if (_checkoutSalesWorkFlow != null)
            {

                _activityDefCheckout = _checkoutSalesWorkFlow.code;
                _objectstateCheckout = _checkoutSalesWorkFlow.objectStateDefinition;
                return true;
            }
            else
            {
               XtraMessageBox.Show("Please define workflow for Check Out Bill Voucher ", "ERROR",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            } 
            return true;
        }
        public string SaveActivity()
        {
            string activtycode = "";
            User currentUser = LoginPage.Authentication.GetAuthorizedUser();
            Device device = LoginPage.Authentication.DeviceObject;
            var orgUnit = LoginPage.Authentication.OrganizationUnitBufferList.FirstOrDefault(o => o.reference == device.code);
            var oud = orgUnit == null ? "" : orgUnit.organizationUnitDefinition;

            Activity activity = new Activity()
            {
                reference = teVoucherNo.Text,
                organizationUnitDef = oud,
                startTimStamp = CurrentTime,
                endTimStamp = CurrentTime,
                year = CurrentTime.Year,
                month = CurrentTime.Month,
                date = CurrentTime.Day,
                device = device.code,
                activitiyDefinition = _activityDefCheckout,
                component = CNETConstantes.PMS,
                user = currentUser == null ? null  : currentUser.code
            };
            activtycode = UIProcessManager.CreateActivity(activity);
            return activtycode;
        }
      
        public void GetVoucherFinal()
        {
            Voucher voucher = new Voucher();
            voucher.code = teVoucherNo.Text;
            voucher.voucherDefinition = CNETConstantes.CHECK_OUT_BILL_VOUCHER;
            voucher.consignee = "";
            voucher.IssuedDate = CurrentTime;
            voucher.period = UIProcessManager.getPeriodCode(CurrentTime);

            //read voucher setting
            Voucher_UI voucherUI = new Voucher_UI(CNETConstantes.CHECK_OUT_BILL_VOUCHER);
            bool flag = voucherUI.IsVoucherCreated();
            if (!flag)
            {
                return;
            }

            GetLineItemlist();
            liDetailsList = new List<LineItemDetails>();
            foreach (LineItem lineItem in LineItemlist)
            {

                NewLineitemObj newLineItem = new NewLineitemObj();
                newLineItem.lineitem = lineItem;
                
                //note: price is already extracted
                LineItemDetailNew liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(voucherUI.Vouchersetting, new NewVoucherBuffer() { voucher = voucher }, newLineItem.lineitem, null, null, null, null, null, true, false, false, false);
                LineItemDetails liDetails = new LineItemDetails()
                {
                    lineItems = liDetail.lineItem,
                    lineItemValueFactor = liDetail.lineItemValueFactor
                };
                liDetailsList.Add(liDetails);
            }

            voFinal = new VoucherFinalCalculator().VoucherCalculation(voucherUI.Vouchersetting, voucher, liDetailsList);

            gcFrontOfficePOS.DataSource = EventRequirementSummaryList;
        }

        List<LineItemDetails> liDetailsList { get; set; }


    
        List<LineItem> LineItemlist { get; set; }
        public List<LineItem> GetLineItemlist()
        {
            LineItemlist = new List<LineItem>();
            EventRequirementSummaryList = new List<EventRequirementList>();

            foreach (EventRequirementList Event in EventRequirementList)
            {
                GSLTax tax = LoginPage.Authentication.GSLTaxBufferList.FirstOrDefault(x => x.reference == Event.articlecode);
                LineItem CheckLineitem = LineItemlist.FirstOrDefault(x => x.article == Event.articlecode);

                if (CheckLineitem == null)
                {
                    EventRequirementList EventRequirement = new EventRequirementList()
                        {
                            articlecode = Event.articlecode,
                            articlename = Event.articlename,
                            quantity = Event.quantity, 
                            unitamount = Event.unitamount
                        };
                    EventRequirementSummaryList.Add(EventRequirement);
                    LineItem li = new LineItem()
                    {
                        article = Event.articlecode,
                        quantity = Event.quantity,
                        unitAmt = Event.unitamount,
                        tax = tax.tax,
                        UOM = CNETConstantes.UNITOFMEASURMENT 

                    };
                    LineItemlist.Add(li);

                }
                else
                {
                    EventRequirementSummaryList.FirstOrDefault(x => x.articlecode == Event.articlecode).quantity += Event.quantity;
                    LineItemlist.FirstOrDefault(x=> x.article == Event.articlecode).quantity += Event.quantity;
                }
                if (EventRequirementSummaryList != null)
                EventRequirementSummaryList.ForEach(x=> x.totalamount = (x.quantity * x.unitamount));
            }
            return LineItemlist;
        }

        private void cbeTransactionType_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit view = sender as ComboBoxEdit;
            if (view == null) return;
            if (view.SelectedIndex == 0)
            {
                POSSettingCache.SelectedSalesType = "Cash Sales";
                POSSettingCache.voucherDefinationCode = CNETConstantes.CHECK_OUT_BILL_VOUCHER;
                POSSettingCache.voucherType = "Cash Sales";
                POSSettingCache.SaleType = "Cash Sales";
                POSSettingCache.defaultTransactionType = "Cash Sales";
                POSSettingCache.stationType = "Cash Sales";
                POSSettingCache.SelectedPaymentType = "Cash";
                POSSettingCache.PaymentType = "Cash";
                POSSettingCache.GetPOSsetting();
                //lePayment.Properties.ReadOnly = false;
            }
            else
            {
                POSSettingCache.SelectedSalesType = "Credit Sales";
                POSSettingCache.stationType = "Credit Sales";
                POSSettingCache.SaleType = "Credit Sales";
                POSSettingCache.voucherDefinationCode = CNETConstantes.CHECK_OUT_BILL_VOUCHER;
                POSSettingCache.voucherType = "Credit Sales";
                POSSettingCache.defaultTransactionType = "Credit Sales";
                POSSettingCache.SelectedPaymentType = "Credit Sales";
                POSSettingCache.PaymentType = "Credit Sales";
                POSSettingCache.GetPOSsetting();
                //lePayment.Properties.ReadOnly = true;
            }
        }


        public void PopulateVoucherFinal()
        {
            decimal Vatamount = 0;
            if (voFinal.taxTransactions != null && voFinal.taxTransactions.Count > 0)
            {
                TaxTransaction VATTaxTransaction = voFinal.taxTransactions.FirstOrDefault(x => x.taxType == CNETConstantes.VAT);
                if (VATTaxTransaction != null)
                {
                    Vatamount = VATTaxTransaction.taxAmount;
                }
            }


            teSubTotal.EditValue =voFinal.voucherValues == null ? "0:00": voFinal.voucherValues.subTotal.Value.ToString("N2");
            teSerCharge.Text = voFinal.voucherValues == null ? "0:00" : voFinal.voucherValues.additionalCharge.Value.ToString("N2");
            tediscount.Text = voFinal.voucherValues == null ? "0:00" : voFinal.voucherValues.discount.Value.ToString("N2");
            teVAT.Text = Vatamount.ToString("N2");
            teGrandTotal.Text = voFinal.voucher.grandTotal.ToString("N2");
        }
        private void bbiClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (Print())
                {
                    bool saved = SaveEventCheckout();

                    if (saved)
                        XtraMessageBox.Show("saved successfully", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        XtraMessageBox.Show("Saving Failed", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    XtraMessageBox.Show("Printing Failed", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception io)
            {
                XtraMessageBox.Show("bbiPrint_ItemClick"+Environment.NewLine+io.Message);
            }
            this.Close();
        }

        private bool SaveEventCheckout()
        {
            bool save = false;
            try
            {
                // voFinal.voucher
                voFinal.voucher.code = teVoucherNo.Text;
                voFinal.voucher.component = CNETConstantes.PMS;
                voFinal.voucher.IssuedDate = CurrentTime;
                voFinal.voucher.IsIssued = true;
                voFinal.voucher.IsVoid = false;
                voFinal.voucher.year = CurrentTime.Year;
                voFinal.voucher.month = CurrentTime.Month;
                voFinal.voucher.date = CurrentTime.Day;
                voFinal.voucher.type = CNETConstantes.TRANSACTIONTYPENORMALTXN;
                voFinal.voucher.LastObjectState = _objectstateCheckout;
                save = UIProcessManager.CreateVoucher(voFinal.voucher);

                if (save)
                {

                    voFinal.lineItemDetails.ForEach(x => x.lineItems.voucher = teVoucherNo.Text);

                    foreach (LineItemDetails LineItemDetail in voFinal.lineItemDetails)
                    {
                        string lineitemcode = UIProcessManager.CreateLineItem(new List<LineItem> { LineItemDetail.lineItems });
                        if (!string.IsNullOrEmpty(lineitemcode))
                        {
                            foreach (LineItemValueFactor ValueFactor in LineItemDetail.lineItemValueFactor)
                            {
                                ValueFactor.lineItem = lineitemcode;
                                UIProcessManager.CreateLineItemValueFactor(ValueFactor);
                            }
                        }

                    }

                    voFinal.voucherValues.voucher = teVoucherNo.Text;
                    UIProcessManager.CreateVoucherValues(voFinal.voucherValues);

                    voFinal.taxTransactions.ForEach(x => x.voucher = teVoucherNo.Text);
                    UIProcessManager.CreateTaxTransaction(voFinal.taxTransactions);

                    VoucherNote voNote = new VoucherNote()
                    {
                        voucher = teVoucherNo.Text,
                        note = "check_out",
                        code = string.Empty
                    };
                    UIProcessManager.CreateVoucherNote(voNote);

                    string activitycode = SaveActivity();
                    if (!string.IsNullOrEmpty(activitycode))
                    {
                        voFinal.voucher.lastActivity = activitycode;
                        UIProcessManager.UpdateVoucher(voFinal.voucher);
                    }
                    if (EventVoucher != null)
                    {
                        EventVoucher.LastObjectState = CNETConstantes.OSD_EVENTCHECKEDOUT;
                        UIProcessManager.UpdateVoucher(EventVoucher);
                    }

                    ReportGenerator reportGenerator = new ReportGenerator();
                    reportGenerator.GenerateAttachmentReport(teVoucherNo.Text);
                }
                else
                    MessageBox.Show("Failed to save Voucher!!", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception io)
            {
                XtraMessageBox.Show("Save " + Environment.NewLine + io.Message);
            }
            return save;
        }

        private List<PrintItem> _printItems { get; set; }
        private bool Print()
        {
            bool isprinted = false;
            try
            {


                FpSetup fpSetup = new FpSetup();
                bool isFpOpened = fpSetup.OpenFisicalPrinter(cutomerDto.Code);
                if (!isFpOpened)
                {
                    CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!fpSetup.CheckPrinterStatusForPrinting())
                {
                    XtraMessageBox.Show("Printer is Out Of Order", "CNET_ERP2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CNETInfoReporter.Hide();
                    FiscalPrinters.DisposeInstance();
                    return false;
                }

                List<LineItemDetail_View> dtoList = gvFrontOfficePOS.DataSource as List<LineItemDetail_View>;
                if (voFinal.lineItemDetails != null && voFinal.lineItemDetails.Count > 0)
                {
                    if (_printItems != null) _printItems.Clear();
                    _printItems = GetPrintItems(voFinal.lineItemDetails);
                }
                else
                {
                    XtraMessageBox.Show("Unable to get print Items", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                isprinted = FiscalPrinters.PrintOperation(_printItems, POSSettingCache.printerType, new List<CustomerDTO>() { cutomerDto }, 0, teVoucherNo.Text, "",
                    "", LoginPage.Authentication.CurrentLoggedInUser.userName, voFinal.voucherValues.discount.Value, voFinal.voucherValues.additionalCharge.Value);

            }
            catch (Exception io)
            {
                XtraMessageBox.Show("Print "+Environment.NewLine+io.Message);
            }
            return isprinted;
        }
        private List<PrintItem> GetPrintItems(List<LineItemDetails> lineItemDetail)
        {
            List<PrintItem> printItemList = new List<PrintItem>();
            foreach (LineItemDetails item in lineItemDetail)
            {

                LineItemValueFactor LineItemDiscount = item.lineItemValueFactor.FirstOrDefault(x => x.isDiscount);
                LineItemValueFactor LineItemServiceCharge = item.lineItemValueFactor.FirstOrDefault(x => !x.isDiscount);
                EventRequirementList EventRequirement = EventRequirementList.FirstOrDefault(x => x.articlecode == item.lineItems.article);

                PrintItem pItem = new PrintItem();
                pItem.code = item.lineItems.article;
                pItem.Name = EventRequirement.articlename;
                if (item.lineItems.unitAmt == null) pItem.UnitPrice = 0;
                else pItem.UnitPrice = item.lineItems.unitAmt.Value;
                pItem.UnitPrice = pItem.UnitPrice < 0 ? 0.01m : pItem.UnitPrice;
                pItem.Quantity = item.lineItems.quantity;
                pItem.taxId = item.lineItems.tax.Value;
                pItem.DiscountAmount = LineItemDiscount == null ? 0 : LineItemDiscount.amount;
                pItem.ServiceChargeAmount = LineItemServiceCharge == null ? 0 : LineItemServiceCharge.amount;
                printItemList.Add(pItem);
            }
            return printItemList;
        }

        private void gvFrontOfficePOS_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.Caption == "SN")
                e.DisplayText = (e.RowHandle + 1).ToString();
        }


    }
}