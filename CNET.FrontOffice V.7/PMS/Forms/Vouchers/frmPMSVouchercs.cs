using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CNET.ERP.Client.Common.UI;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using CNET.FrontOffice_V._7;
using DevExpress.XtraGrid.Columns;
using System.Globalization;
using System.Text.RegularExpressions;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET.FrontOffice_V._7.Validation;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET.POS.Common.Models;
using CNET.FP.Tool;
using CNET.POS.Settings;
using System.Diagnostics;
using DevExpress.XtraReports.ReportGeneration;
using CNET.FrontOffice_V._7.Forms.State_Change;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using DevExpress.XtraCharts;
using CNET.FrontOffice_V._7.Non_Navigatable_Modals;
using CNET_V7_Domain.Misc;
using CNET.Mobile.Payments;
using CNET_V7_Domain.Domain.PmsSchema;
using DevExpress.CodeParser;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.XtraRichEdit.Import.OpenXml;

namespace CNET.FrontOffice_V._7.Forms.Vouchers
{
    public partial class frmPMSVouchercs : UILogicBase
    {

        private string customerTIN = "";
        private List<Print_Item> _printItems = new List<Print_Item>();
        private Consignee customerDto = new Consignee();
        private decimal _additionalChargeAmt = 0;
        private bool _isAdditionalInPercent = false;
        private int _roundCalculatorDigit = 2;
        ArticleDTO CommisssionArticle { get; set; }
        GsltaxDTO CommisssionArticleTax { get; set; }

        private frmForeignExTransaction frmForeignTrans = null;


        private int? _voucherExt = null;

        private decimal exRate = 0.00m;

        private DeviceDTO device = null;
        private UserDTO currentUser = null;


        private decimal discount = 0;

        private int? defPaymentType = null;
        private int? defCurrency = null;

        private SystemConstantDTO _windowLookup = null;

        private ActivityDefinitionDTO workflow = null;

        /************ PROPERTIES *****************/

        VoucherBuffer voucherbuffer = new VoucherBuffer();
        public int VoucherType { get; set; }
        public bool IsFromSplitWindow { get; set; }

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

        private RegistrationListVMDTO registrationExt;
        //  private string _currentFSNumber;

        internal RegistrationListVMDTO RegistrationExt
        {
            get { return registrationExt; }
            set
            {
                registrationExt = value;

            }
        }

        private int _currentWindow = -1;
        public int CurrentWindow
        {
            get
            {
                return _currentWindow;
            }
            set
            {
                _currentWindow = value;
            }
        }

        /// /////////////////////////////// CONSTRUCTOR //////////////////////////////////
        public frmPMSVouchercs(int type)
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            VoucherType = type;

            InitializeUI();

        }
        private void cbeTransactionType_EditValueChanged(object sender, EventArgs e)
        {
            ComboBoxEdit view = sender as ComboBoxEdit;
            if (view == null) return;
            if (view.SelectedIndex == 0)
            {
                POS_Settings.Voucher_Definition = CNETConstantes.CASH_SALES;
                POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                // teVoucherNo.Text = GetCurrentId(0);
                //if (string.IsNullOrEmpty(teVoucherNo.Text))
                //{
                //    SystemMessage.ShowModalInfoMessage("Unable to generate ID!", "ERROR");
                //    bbiPrint.Enabled = false;
                //}
                lePaymentType.EditValue = CNETConstantes.PAYMENTMETHODSCASH;
                lePaymentType.Properties.ReadOnly = false;
            }
            else
            {
                POS_Settings.Voucher_Definition = CNETConstantes.CREDITSALES;
                POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                //teVoucherNo.Text = GetCurrentId(0);
                //if (string.IsNullOrEmpty(teVoucherNo.Text))
                //{
                //    SystemMessage.ShowModalInfoMessage("Unable to generate ID!", "ERROR");
                //    bbiPrint.Enabled = false;
                //}
                lePaymentType.EditValue = CNETConstantes.PAYMENTMETHODSCREDIT;
                lePaymentType.Properties.ReadOnly = true;
            }

            //get Setting Values
            // GetVoucherSettingValues(POS_Settings.Voucher_Definition);

            //check workflow for the selected transactiontype
            //if (!CheckWorkflow())
            //{
            //    bbiPrint.Enabled = false;
            //}
            //else
            //{
            //    bbiPrint.Enabled = true;
            //}

        }

        #region Helper Methods

        private void InitializeUI()
        {
            //Currency
            cacCurrency.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
            cacCurrency.Properties.DisplayMember = "Description";
            cacCurrency.Properties.ValueMember = "Id";

            //Customer List
            GridColumn columnCompany = cacCustomer.Properties.View.Columns.AddField("Id");
            columnCompany.Visible = false;
            columnCompany = cacCustomer.Properties.View.Columns.AddField("Code");
            columnCompany.Visible = true;
            columnCompany = cacCustomer.Properties.View.Columns.AddField("FirstName");
            columnCompany.Caption = "First Name";
            columnCompany.Visible = true;
            columnCompany = cacCustomer.Properties.View.Columns.AddField("SecondName");
            columnCompany.Caption = "Middle Name";
            columnCompany.Visible = true;
            columnCompany = cacCustomer.Properties.View.Columns.AddField("BioId");
            columnCompany.Caption = "Id Number";
            columnCompany.Visible = true;
            cacCustomer.Properties.DisplayMember = "FirstName";
            cacCustomer.Properties.ValueMember = "Id";

            //Payment Type
            lePaymentType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            lePaymentType.Properties.DisplayMember = "Description";
            lePaymentType.Properties.ValueMember = "Id";

            //Windows
            luk_window.Properties.Columns.Add(new LookUpColumnInfo("Description", "Window"));
            luk_window.Properties.DisplayMember = "Description";
            luk_window.Properties.ValueMember = "Id";

            if (VoucherType == CNETConstantes.CASHRECIPT)
            {
                rpgForignCurrency.Visible = true;
            }
            else if (VoucherType == CNETConstantes.CREDIT_NOTE_VOUCHER)
            {
                locPercent.Visibility = LayoutVisibility.Always;
            }
        }

        DateTime? CurrentTime = UIProcessManager.GetServiceTime();

        private bool InitializeData()
        {
            try
            {
                voucherbuffer = new VoucherBuffer();
                if (registrationExt == null) return false;

                device = LocalBuffer.LocalBuffer.CurrentDevice;
                currentUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser;

                // Progress_Reporter.Show_Progress("Loading Data. Please Wait...");



                string currentVoCode = UIProcessManager.IdGenerater("Voucher", VoucherType, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    teVoucherNo.Text = currentVoCode;
                }
                else
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    return false;
                }

                //Set Title Based on the current voucher type
                SetTitleByType();


                //for refund initialize printer
                if (VoucherType == CNETConstantes.REFUND)
                {
                    PMSVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.REFUND);
                    voucherbuffer.Voucher.Definition = CNETConstantes.REFUND;
                    POS_Settings.Voucher_Definition = CNETConstantes.REFUND;
                    POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);
                    POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                    POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                    FiscalPrinters fpSetup = new FiscalPrinters();
                    CNET.POS.Settings.POS_Settings.System_Constants_Buffer = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList;
                    //  bool isFpOpened = fpSetup.open(RegistrationExt.GuestId);
                    if (!POS_Settings.IsError)
                    {
                        ////CNETInfoReporter.Hide();
                        XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                //Check Workflow
                workflow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, VoucherType).FirstOrDefault();

                if (workflow == null)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please Define Workflow of PREPARED for " + (VoucherType == CNETConstantes.CREDIT_NOTE_VOUCHER ? "Credit Note" : this.Text), "ERROR");
                    return false;
                }



                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(workflow.Id).FirstOrDefault(r => r.Role == userRoleMapper.Role && r.NeedsPassCode);
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

                //CommisionArticle
                CreateCommisionArticle();


                //Setting Voucher Extensions
                SetVoucherExtentions();

                //Discount
                discount = Math.Round(UIProcessManager.GetDiscount(RegistrationExt.Id, CNETConstantes.CREDIT_NOTE_VOUCHER, null), 2);



                //set purpose text for cash reciept
                if (VoucherType == CNETConstantes.CASHRECIPT)
                {
                    PMSVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.CASHRECIPT);
                    voucherbuffer.Voucher.Definition = CNETConstantes.CASHRECIPT;
                    lciCustomerLookUp.Visibility = LayoutVisibility.Always;
                    lciCustomerText.Visibility = LayoutVisibility.Never;
                    if (RegistrationExt.lastState == CNETConstantes.SIX_PM_STATE || RegistrationExt.lastState == CNETConstantes.GAURANTED_STATE || RegistrationExt.lastState == CNETConstantes.OSD_WAITLIST_STATE || RegistrationExt.lastState == CNETConstantes.OSD_PENDING_STATE)
                    {
                        mePurpose.Text = @"Advance Payment for " + RegistrationExt.Registration + " Room = " + RegistrationExt.Room;
                    }
                    else if (RegistrationExt.lastState == CNETConstantes.CHECKED_IN_STATE)
                    {
                        mePurpose.Text = @"Payment  for " + RegistrationExt.Registration + " Room = " + RegistrationExt.Room;
                    }
                    else if (RegistrationExt.lastState == CNETConstantes.CHECKED_OUT_STATE)
                    {
                        mePurpose.Text = @"Settlement for " + RegistrationExt.Registration + " Room = " + RegistrationExt.Room;
                    }
                    else
                    {
                        mePurpose.Text = this.Text;
                    }

                }
                else if (VoucherType == CNETConstantes.REFUND)
                {
                    voucherbuffer.Voucher.Definition = CNETConstantes.REFUND;
                    mePurpose.Text = @"Refund for " + RegistrationExt.Registration + " Room = " + RegistrationExt.Room;
                    statusLabel.Text = "Note: Refund amount should be tax inclusive";
                    statusLabel.Visible = true;
                    lciCustomerLookUp.Visibility = LayoutVisibility.Never;
                    lciCustomerText.Visibility = LayoutVisibility.Always;
                }
                else
                {
                    lciCustomerLookUp.Visibility = LayoutVisibility.Never;
                    lciCustomerText.Visibility = LayoutVisibility.Always;
                }

                if (CurrentTime == null) return false;
                deDate.DateTime = CurrentTime.Value;


                //populate customer list for only cash reciept
                #region Populate Customer list 
                if (VoucherType == CNETConstantes.CASHRECIPT)
                {
                    List<ConsigneeDTO> CompanyandGuestList = new List<ConsigneeDTO>();
                    CompanyandGuestList.AddRange(LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist);
                    CompanyandGuestList.AddRange(LocalBuffer.LocalBuffer.AllGuestConsigneeViewlist);

                    cacCustomer.Properties.DataSource = (CompanyandGuestList.GroupBy(x => x.Id).Select(x => x.First()).ToList());

                }

                #endregion

                if (RegistrationExt.CompanyId != null)
                {

                    cacCustomer.EditValue = RegistrationExt.CompanyId;
                }
                else
                {
                    cacCustomer.EditValue = RegistrationExt.GuestId;
                }
                teRegistration.Text = RegistrationExt.Registration;
                teCustomer.Text = RegistrationExt.Guest;


                List<SystemConstantDTO> paymentList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.PAYMENT_METHODS && l.IsActive && l.Id != CNETConstantes.PAYMENTMETHODS_DIRECT_BILL).ToList();
                lePaymentType.Properties.DataSource = (paymentList.OrderByDescending(c => c.IsDefault).ToList());

                // Set default a record that has signed as IsDefault
                SystemConstantDTO payLookup = paymentList.FirstOrDefault(c => c.IsDefault);
                if (payLookup != null)
                {
                    lePaymentType.EditValue = (payLookup.Id);
                    defPaymentType = payLookup.Id;
                }

                //set the registration's payment method
                VoucherDTO regExt = UIProcessManager.GetVoucherById(RegistrationExt.Id);
                if (regExt != null)
                {
                    if (regExt.PaymentMethod != CNETConstantes.PAYMENTMETHODS_DIRECT_BILL)
                        lePaymentType.EditValue = regExt.PaymentMethod;
                    else
                        lePaymentType.EditValue = CNETConstantes.PAYMENTMETHODSCASH;
                }


                List<CurrencyDTO> currencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;
                if (currencyList != null)
                {
                    cacCurrency.Properties.DataSource = (currencyList.OrderByDescending(c => c.IsDefault).ToList());
                    var currency = currencyList.FirstOrDefault(c => c.IsDefault);
                    if (currency != null)
                    {
                        cacCurrency.EditValue = (currency.Id);
                        defCurrency = currency.Id;
                    }
                }

                //populate window lookups
                List<SystemConstantDTO> windowsList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.SPLIT_WINDOWS && l.IsActive).ToList();
                if (windowsList != null && windowsList.Count != 0)
                {
                    windowsList.Insert(0, new SystemConstantDTO() { Id = 0, Description = "All" });
                    luk_window.Properties.DataSource = windowsList;
                    if (CurrentWindow > 0)
                    {
                        _windowLookup = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(l => l.Category == CNETConstantes.SPLIT_WINDOWS && l.Value == CurrentWindow.ToString());

                        if (_windowLookup != null)
                        {
                            luk_window.EditValue = _windowLookup.Id;
                        }
                        else
                        {
                            luk_window.EditValue = 0;
                        }

                    }
                    else
                    {
                        luk_window.EditValue = 0;
                    }

                }

                if (IsFromSplitWindow)
                    luk_window.Enabled = false;

                if (registrationExt.Currency != null)
                    cacCurrency.EditValue = registrationExt.Currency;





                if (LocalBuffer.LocalBuffer.PMSRecieptIsCheckout || RegistrationExt.AuthorizeDirectBill)
                {
                    lcTransactionType.Visibility = LayoutVisibility.Never;
                    lcCRVFiscalGrouping.Visibility = LayoutVisibility.Never;
                    this.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height - lcCRVFiscalGrouping.Size.Height);
                }
                else
                {
                    lcTransactionType.Visibility = LayoutVisibility.Always;
                    lcCRVFiscalGrouping.Visibility = LayoutVisibility.Always;
                    bbiSave.Caption = "Print";

                    if (registrationExt.PaymentDesc == "Direct Bill")
                    {
                        cbeTransactionType.EditValue = "Credit Sales";
                        cbeTransactionType.Enabled = false;
                    }
                    else
                    {
                        var configDefaultBilType = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_DefaultCheckOutPaymentType);

                        if (configDefaultBilType != null && !string.IsNullOrEmpty(configDefaultBilType.CurrentValue) && configDefaultBilType.CurrentValue == "Cash_Sales")
                            cbeTransactionType.EditValue = "Cash Sales";
                        else
                            cbeTransactionType.EditValue = "Credit Sales";
                    }
                }



                ////CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                SystemMessage.ShowModalInfoMessage("Error in initializing data. Detail:  " + ex.Message, "ERROR");
                ////CNETInfoReporter.Hide();
                return false;
            }
        }

        private void CreateCommisionArticle()
        {
            CommisssionArticle = UIProcessManager.GetArticleByname("Commission Item");
            if (CommisssionArticle == null)
            {
                string currentVoCode = UIProcessManager.IdGenerater("Article", CNETConstantes.ITEM, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    string ItemlocalCode = currentVoCode;
                    int ItemNewId = 0;
                    PreferenceDTO ItemPref = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(x => x.SystemConstant == CNETConstantes.ITEM && x.SystemConstant == CNETConstantes.ARTICLE);
                    if (ItemPref == null)
                    {
                        ItemPref = new PreferenceDTO
                        {
                            SystemConstant = CNETConstantes.ARTICLE,
                            // Reference = CNETConstantes.ITEM,
                            Description = "Item",
                            Index = 0,
                            IsActive = true
                        };
                        PreferenceDTO Preferencesaved = UIProcessManager.CreatePreference(ItemPref);
                        if (Preferencesaved == null)
                        {
                            XtraMessageBox.Show("Failed to Create Category for Commision Item", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            LocalBuffer.LocalBuffer.LoadPreference();
                            ItemPref.Id = Preferencesaved.Id;
                        }
                    }
                    CommisssionArticle = new ArticleDTO
                    {
                        LocalCode = ItemlocalCode,
                        GslType = CNETConstantes.ITEM,
                        Name = "Commission Item",
                        Preference = ItemPref.Id,
                        Uom = CNETConstantes.UNITOFMEASURMENTPCS,
                        IsActive = true
                    };
                    ArticleDTO ItemSaved = UIProcessManager.CreateArticle(CommisssionArticle);

                    CommisssionArticleTax = new GsltaxDTO
                    {
                        Reference = ItemSaved.Id,
                        Tax = CNETConstantes.VAT
                    };

                    UIProcessManager.CreateGSLTax(CommisssionArticleTax);


                    SystemConstantDTO Sate = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault();

                    ObjectStateDTO CommisssionArticleState = new ObjectStateDTO
                    {
                        Reference = ItemSaved.Id,
                        ObjectStateDefinition = Sate.Id
                    };
                    UIProcessManager.CreateObjectState(CommisssionArticleState);
                }
            }
            else
            {
                CommisssionArticleTax = UIProcessManager.GetGSLTaxByReference(CommisssionArticle.Id);
                // CommisssionArticleTax = UIProcessManager.GetGSLTaxByReference(CommisssionArticle.code);
                if (CommisssionArticleTax == null)
                {
                    CommisssionArticleTax = new GsltaxDTO
                    {
                        Reference = CommisssionArticle.Id,
                        Tax = CNETConstantes.VAT
                    };
                    UIProcessManager.CreateGSLTax(CommisssionArticleTax);
                }
            }
        }

        private void SetTitleByType()
        {
            if (VoucherType == CNETConstantes.CASHRECIPT)
            {
                this.Text = @"Cash Receipt";
            }
            else if (VoucherType == CNETConstantes.CREDIT_NOTE_VOUCHER)
            {
                this.Text = @"Rebate";
                //show Net off check box
                ceNetoff.Checked = true;
                ceNetoff.Visible = true;
                lc_netOff.Visibility = LayoutVisibility.Always;
            }
            else if (VoucherType == CNETConstantes.PAID_OUT_VOUCHER)
            {
                this.Text = @"Paid Out";
            }

            else if (VoucherType == CNETConstantes.REFUND)
            {
                this.Text = @"Fiscal Refund";
            }


        }


        public void SetVoucherExtentions()
        {
            VoucherExtensionDefinitionDTO vExt = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(ve => ve.Index == 0 && ve.VoucherDefinition == VoucherType &&
                ve.Type == CNETConstantes.VOUCHER_RELATION_TYPE_VOUCHER_EXT);

            if (vExt == null)
            {
                teReference.Enabled = false;
                return;

            }
            //if (vExt.ExDataType == "String" || vExt.ExDataType == "string")
            //{
            _voucherExt = vExt.Id;
            string label = string.IsNullOrEmpty(vExt.Descritpion) ? "Reference" : vExt.Descritpion;
            if (vExt.IsMandatory != null && vExt.IsMandatory)
            {
                lc_reference.Text = label + " *";
            }
            else
            {
                lc_reference.Text = label;
            }
            // }
        }

        private void Reset()
        {

            string currentVoCode = UIProcessManager.IdGenerater("Voucher", VoucherType, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(currentVoCode))
            {
                teVoucherNo.Text = currentVoCode;
            }
            else
            {
                SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                this.Close();
            }


            meRemark.Text = "";
            mePurpose.Text = "";
            teAmount.Text = "0.0";
            //default payment type
            lePaymentType.EditValue = defPaymentType;
            cacCurrency.EditValue = defCurrency;
        }


        private void PopulateCustomerDTO()
        {
            customerDto = new Consignee();
            if (RegistrationExt == null) return;

            if (RegistrationExt.CompanyId != null)
            {
                ConsigneeDTO customer = LocalBuffer.LocalBuffer.AllCustomerConsigneeViewlist.FirstOrDefault(x => x.Id == RegistrationExt.CompanyId);
                if (customer != null)
                {
                    if (customer != null)
                    {
                        if (customer.Id != null)
                        {
                            customerDto.Code = customer.Code;
                            customerDto.Name = customer.FirstName;
                            customerDto.TIN = customer.Tin;
                            customerDto.ConsigneeType = "Company";
                        }
                    }

                }
                else
                {
                    customerDto.Name = RegistrationExt.Guest;
                    customerDto.ConsigneeType = "Guest";
                }
            }
            else
            {
                customerDto.Name = RegistrationExt.Guest;
                customerDto.ConsigneeType = "Guest";
            }

        }


        private List<Print_Item> GetPrintItems(string voucherCode, int articleId, decimal unitAmount, int taxId)
        {

            List<Print_Item> printItemList = new List<Print_Item>();

            Print_Item pItem = new Print_Item();

            pItem.ID = articleId;
            pItem.Name = string.IsNullOrEmpty(mePurpose.Text) ? "Refund" : mePurpose.Text;
            pItem.UnitPrice = Math.Round(unitAmount, _roundCalculatorDigit);
            pItem.Quantity = 1;
            pItem.taxId = taxId;
            pItem.lineItemValue = 1;
            pItem.TotalPrice = Math.Round(unitAmount, _roundCalculatorDigit);
            pItem.isValueDiscount = true;
            pItem.isValuePercent = true;
            printItemList.Add(pItem);



            return printItemList;

        }
        private List<Print_Item> GetPrintCRVItems(List<VwLineItemDetailViewDTO> lineItemDetail, int voucherDef)
        {
            TaxDTO tax = null;
            tax = CommonLogics.GetApplicableTax(registrationExt.Id, voucherDef, registrationExt.GuestId, lineItemDetail[0].Article);

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

        //private bool SaveRMSFiscalTransaction(string voucherCode)
        //{
        //    bool isSaved = true;
        //    string FsNum = "";
        //    var voucherFiscalTransactionList = new List<CNET.ERP2016.SQLDataAccess.FiscalExtension>();
        //    var voucherFiscalTransaction = new CNET.ERP2016.SQLDataAccess.FiscalExtension();
        //    try
        //    {

        //        #region FS and RFNumber

        //        string FSNo = _currentFSNumber;
        //        switch (POSSettingCache.printerType.ToLower())
        //        {
        //            case "datecs_fp_60":

        //                switch (POSSettingCache.voucherDefinationCode)
        //                {
        //                    case CNETConstantes.CHECK_OUT_BILL_VOUCHER:
        //                    case CNETConstantes.CASH_SALES:
        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;
        //                        break;
        //                    case CNETConstantes.CREDITSALES:

        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;

        //                        break;
        //                    case CNETConstantes.REFUND:

        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;


        //                        break;
        //                }
        //                voucherFiscalTransaction.code = "";
        //                voucherFiscalTransaction.voucher = voucherCode;
        //                voucherFiscalTransaction.remark = "Datecs FS";


        //                if (UIProcessManager.CreateFiscalExtension(voucherFiscalTransaction))
        //                {
        //                    isSaved = true;
        //                }
        //                else
        //                {
        //                    isSaved = false;
        //                }
        //                break;
        //            case "datecs_fmp_350":
        //            case "datecs_fp_700":
        //            case "datecs_fp60_new":

        //                FsNum = FiscalPrinters.datecsNew.GetLastFiscalNumber();

        //                switch (POSSettingCache.voucherDefinationCode)
        //                {
        //                    case CNETConstantes.CHECK_OUT_BILL_VOUCHER:
        //                    case CNETConstantes.CASH_SALES:
        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;


        //                        break;
        //                    case CNETConstantes.CREDITSALES:
        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;

        //                        break;
        //                    case CNETConstantes.REFUND:

        //                        FsNum = FiscalPrinters.datecsNew.GetLastRefundNumber();
        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;

        //                        break;
        //                }
        //                voucherFiscalTransaction.code = "";
        //                voucherFiscalTransaction.voucher = voucherCode;
        //                voucherFiscalTransaction.remark = POSSettingCache.printerType;

        //                if (UIProcessManager.CreateFiscalExtension(voucherFiscalTransaction))
        //                { isSaved = true; }
        //                else
        //                {
        //                    isSaved = false;
        //                }
        //                break;
        //            case "daisy_fx_1300":

        //                //  FsNum = FiscalPrinters.daisy.GetLastFiscalNumber();

        //                switch (POSSettingCache.voucherDefinationCode)
        //                {
        //                    case CNETConstantes.CHECK_OUT_BILL_VOUCHER:
        //                    case CNETConstantes.CASH_SALES:
        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;


        //                        break;
        //                    case CNETConstantes.CREDITSALES:
        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;

        //                        break;
        //                    case CNETConstantes.REFUND:


        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;

        //                        break;
        //                }
        //                voucherFiscalTransaction.code = "";
        //                voucherFiscalTransaction.voucher = voucherCode;
        //                voucherFiscalTransaction.remark = " ";

        //                if (UIProcessManager.CreateFiscalExtension(voucherFiscalTransaction))
        //                { isSaved = true; }
        //                else
        //                {
        //                    isSaved = false;
        //                }
        //                break;
        //            case "bmc_th34ej":
        //                switch (POSSettingCache.voucherDefinationCode)
        //                {
        //                    case CNETConstantes.CHECK_OUT_BILL_VOUCHER:
        //                    case CNETConstantes.CASH_SALES:
        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;


        //                        break;
        //                    case CNETConstantes.CREDITSALES:
        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;

        //                        break;
        //                    case CNETConstantes.REFUND:


        //                        voucherFiscalTransaction.fsNo = FSNo;
        //                        voucherFiscalTransaction.mrsNo = POSSettingCache.fiscalprinterMRC;

        //                        break;
        //                }
        //                voucherFiscalTransaction.code = "";
        //                voucherFiscalTransaction.voucher = voucherCode;
        //                voucherFiscalTransaction.remark = " ";

        //                if (UIProcessManager.CreateFiscalExtension(voucherFiscalTransaction))
        //                { isSaved = true; }
        //                else
        //                {
        //                    isSaved = false;
        //                }
        //                break;

        //        }

        //        #endregion




        //        return isSaved;
        //    }
        //    catch (Exception)
        //    {

        //        return false;
        //    }



        //}

        //public string ReadFSForPrinting()
        //{
        //    string CurrentFSNo = string.Empty;
        //    try
        //    {
        //       // Progress_Reporter.Show_Progress("Reading FS No from Fiscal Printer", "Please Wait..", 3,
        //            6);
        //        string FsNum = "";
        //        int no = 0;
        //        switch (POSSettingCache.printerType.ToLower())
        //        {
        //            case "datecs_fp_60":
        //                FsNum = FiscalPrinters.datecs.GetLastFiscalNumber();

        //                switch (POSSettingCache.voucherDefinationCode)
        //                {
        //                    case CNETConstantes.CASH_SALES:
        //                        CurrentFSNo = Regex.Split(FsNum, ",")[0];
        //                        no = Convert.ToInt32(CurrentFSNo) + 1;
        //                        CurrentFSNo = no.ToString();
        //                        CurrentFSNo = CurrentFSNo.PadLeft(8,
        //                            '0');

        //                        break;
        //                    case CNETConstantes.CREDITSALES:
        //                        CurrentFSNo = Regex.Split(FsNum, ",")[0];
        //                        no = Convert.ToInt32(CurrentFSNo) + 1;
        //                        CurrentFSNo = no.ToString();
        //                        CurrentFSNo = CurrentFSNo.PadLeft(8,
        //                            '0');

        //                        break;
        //                    case CNETConstantes.REFUND:

        //                        CurrentFSNo = Regex.Split(FsNum, ",")[2];
        //                        no = Convert.ToInt32(CurrentFSNo) + 1;
        //                        CurrentFSNo = no.ToString();
        //                        CurrentFSNo = CurrentFSNo.PadLeft(8,
        //                            '0');

        //                        break;
        //                }


        //                break;
        //            case "datecs_fmp_350":
        //            case "datecs_fp_700":
        //            case "datecs_fp60_new":
        //                FsNum = FiscalPrinters.datecsNew.GetLastFiscalNumber();

        //                switch (POSSettingCache.voucherDefinationCode)
        //                {
        //                    case CNETConstantes.CASH_SALES:
        //                        CurrentFSNo = Regex.Split(FsNum, "\t")[4];
        //                        no = Convert.ToInt32(CurrentFSNo) + 1;
        //                        CurrentFSNo = no.ToString();
        //                        CurrentFSNo = CurrentFSNo.PadLeft(8,
        //                            '0');

        //                        break;
        //                    case CNETConstantes.CREDITSALES:
        //                        CurrentFSNo = Regex.Split(FsNum, "\t")[4];
        //                        no = Convert.ToInt32(CurrentFSNo) + 1;
        //                        CurrentFSNo = no.ToString();
        //                        CurrentFSNo = CurrentFSNo.PadLeft(8,
        //                            '0');

        //                        break;
        //                    case CNETConstantes.REFUND:
        //                        FsNum = FiscalPrinters.datecsNew.GetLastRefundNumber();
        //                        CurrentFSNo = (Regex.Split(FsNum, "\t")[4]);
        //                        no = Convert.ToInt32(CurrentFSNo) + 1;
        //                        CurrentFSNo = no.ToString();
        //                        CurrentFSNo = CurrentFSNo.PadLeft(8,
        //                            '0');

        //                        break;
        //                }
        //                break;
        //            case "bmc_th34ej":
        //                FsNum = BMC.Get_Last_Fiscal_Number();

        //                switch (POSSettingCache.voucherDefinationCode)
        //                {
        //                    case CNETConstantes.CASH_SALES:
        //                        CurrentFSNo = FsNum;
        //                        no = Convert.ToInt32(CurrentFSNo) + 1;
        //                        CurrentFSNo = no.ToString();
        //                        CurrentFSNo = CurrentFSNo.PadLeft(8,
        //                            '0');

        //                        break;
        //                    case CNETConstantes.CREDITSALES:
        //                        CurrentFSNo = FsNum;
        //                        no = Convert.ToInt32(CurrentFSNo) + 1;
        //                        CurrentFSNo = no.ToString();
        //                        CurrentFSNo = CurrentFSNo.PadLeft(8,
        //                            '0');

        //                        break;
        //                    case CNETConstantes.REFUND:
        //                        CurrentFSNo = FsNum;
        //                        no = Convert.ToInt32(CurrentFSNo) + 1;
        //                        CurrentFSNo = no.ToString();
        //                        CurrentFSNo = CurrentFSNo.PadLeft(8,
        //                            '0');

        //                        break;
        //                }
        //                break;
        //        }

        //        return CurrentFSNo;
        //    }
        //    catch (Exception)
        //    {

        //        //throw;
        //    }

        //    return CurrentFSNo;

        //}



        #endregion


        #region Event Handlers

        decimal OriginalAmount = 0;
        private async void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime? CurrentTime = UIProcessManager.GetServiceTime();
            if (CurrentTime == null) return;
            DateTime _currenDateTime = CurrentTime.Value;
            //Progress_Reporter.Show_Progress("Saving..");

            try
            {
                List<Control> controls = new List<Control>
                {
                    deDate, teAmount, teTotalAmount,cacCurrency
                };
                if (lePaymentType.EditValue != null && Convert.ToInt32(lePaymentType.EditValue) != CNETConstantes.PAYMENTMETHODSMobileMoney)
                {
                    if (lc_reference.Text.Contains("*"))
                    {
                        controls.Add(teReference);
                    }
                }
                IList<Control> invalidControls = CustomValidationRule.Validate(controls);
                if (invalidControls.Count > 0)
                {
                    //CNETInfoReporter.Hide();
                    return;
                }
                decimal Total = string.IsNullOrEmpty(teTotalAmount.Text) ? 0 : Convert.ToDecimal(teTotalAmount.Text);
                if (Total == 0)
                {
                    //CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Invalid Input", "ERROR");
                    return;
                }


                OriginalAmount = Total;
                //Check Reference Number
                if (teReference.EditValue != null && !string.IsNullOrEmpty(teReference.EditValue.ToString()))
                {
                    var configValidateReference = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c => c.Reference == CNETConstantes.PMS_Pointer.ToString() && c.Attribute == CNETConstantes.PMS_SETTING_ValidateReference);
                    if (configValidateReference != null)
                    {
                        bool flag = Convert.ToBoolean(configValidateReference.CurrentValue);
                        if (flag)
                        {
                            //validate refrence
                            var voExtTranList = UIProcessManager.GetVoucherByExtension1(teReference.EditValue.ToString());
                            if (voExtTranList != null && voExtTranList.Count > 0)
                            {
                                ////CNETInfoReporter.Hide();
                                SystemMessage.ShowModalInfoMessage("The External Reference Number is already exist!", "ERROR");
                                return;
                            }
                        }
                    }

                }


                if (string.IsNullOrWhiteSpace(teTotalAmount.Text))
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please enter value!", "ERROR");
                    return;
                }

                TaxDTO tax = null;
                if (VoucherType == CNETConstantes.REFUND || VoucherType == CNETConstantes.CREDIT_NOTE_VOUCHER)
                {
                    tax = CommonLogics.GetApplicableTax(RegistrationExt.Id, VoucherType, RegistrationExt.GuestId, null);

                    if (!string.IsNullOrEmpty(tax.Remark))
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                        return;
                    }

                }

                if (!LocalBuffer.LocalBuffer.PMSRecieptIsCheckout && VoucherType == CNETConstantes.CASHRECIPT && !RegistrationExt.AuthorizeDirectBill)
                {
                   

                    if (cbeTransactionType.SelectedIndex == 0)
                        POS_Settings.Voucher_Definition = CNETConstantes.CASH_SALES;
                    else
                        POS_Settings.Voucher_Definition = CNETConstantes.CREDITSALES;

                    POS_Settings.IsError = true;
                    POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                    POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                    POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);



                    // FiscalPrinters FP = new FiscalPrinters();
                    FiscalPrinters.GetInstance();
                    if (!POS_Settings.IsError)
                    {
                        XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        PMSDataLogger.LogMessage("frmPMSVoucher", "Unable to connect with fisical printer !!");

                        return;
                    }

                    bool validatePMSLicense = CommonLogics.Validate_PMSPOS_License();
                    if (!validatePMSLicense)
                        return;

                }
                bool isSaved = true;



                string currentVoCode = UIProcessManager.IdGenerater("Voucher", VoucherType, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (!string.IsNullOrEmpty(currentVoCode))
                {
                    teVoucherNo.Text = currentVoCode;
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                    ////CNETInfoReporter.Hide();
                    return;
                }



                voucherbuffer.Voucher.Code = teVoucherNo.Text;
                voucherbuffer.Voucher.Definition = VoucherType;
                voucherbuffer.Voucher.Type = CNETConstantes.TRANSACTIONTYPENORMALTXN;
                voucherbuffer.Voucher.IssuedDate = _currenDateTime;
                voucherbuffer.Voucher.Year = _currenDateTime.Year;
                voucherbuffer.Voucher.Month = _currenDateTime.Month;
                voucherbuffer.Voucher.Day = _currenDateTime.Day;
                voucherbuffer.Voucher.IsVoid = false;
                voucherbuffer.Voucher.Period = LocalBuffer.LocalBuffer.GetPeriodCode(_currenDateTime);
                voucherbuffer.Voucher.Remark = meRemark.Text;
                voucherbuffer.Voucher.LastState = workflow.State.Value;



                //if (VoucherType == CNETConstantes.CASHRECIPT)
                //{
                if (cacCustomer.EditValue != null && !string.IsNullOrEmpty(cacCustomer.EditValue.ToString()))
                {
                    int cons = 0;

                    int.TryParse(cacCustomer.EditValue.ToString(), out cons);
                    voucherbuffer.Voucher.Consignee1 = cons;
                }
                //}
                //else
                //{
                //    voucherbuffer.Voucher.Consignee1 = RegistrationExt.GuestId;
                //    voucherbuffer.Voucher.Consignee2 = RegistrationExt.CompanyId;
                //}



                if (workflow.IssuingEffect)
                {
                    voucherbuffer.Voucher.IsIssued = true;
                }



                //if net off is checked, save the vat included value
                decimal grandTotal = string.IsNullOrEmpty(teTotalAmount.Text) ? 0 : Convert.ToDecimal(teTotalAmount.Text);
                if (grandTotal == 0)
                {
                    ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Invalid Input", "ERROR");
                    return;
                }

                // for paid-out put avoid -ve value entry
                if (VoucherType == CNETConstantes.PAID_OUT_VOUCHER)
                {
                    grandTotal = Math.Abs(grandTotal);
                }

                decimal subTotal = grandTotal;


                if (VoucherType == CNETConstantes.CREDIT_NOTE_VOUCHER)
                {
                    if (grandTotal + discount < 0)
                    {
                        ////CNETInfoReporter.Hide();
                        XtraMessageBox.Show("Overall discount is less than 0. ", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (ceNetoff.Checked)
                    {
                        decimal taxRate = (decimal)tax.Amount;
                        // subTotal = (grandTotal - (Math.Round(grandTotal * (taxRate / 100), 2)));
                        subTotal = (grandTotal / (1 + (taxRate / 100)));

                    }
                }
                else if (VoucherType == CNETConstantes.REFUND)
                {
                    decimal taxRate = (decimal)tax.Amount;
                    subTotal = (grandTotal / (1 + (taxRate / 100)));

                }

                voucherbuffer.Voucher.SubTotal = Math.Abs(subTotal);
                voucherbuffer.Voucher.GrandTotal = Math.Round(subTotal, 2);

                //save non cash transaction
                #region Non cash Transaction 

                frmNonCashPayments frmNonCash = null;
                if (lePaymentType.EditValue != null && Convert.ToInt32(lePaymentType.EditValue) != CNETConstantes.PAYMENTMETHODSCASH && Convert.ToInt32(lePaymentType.EditValue) != CNETConstantes.PAYMENTMETHODSCREDIT && Convert.ToInt32(lePaymentType.EditValue) != CNETConstantes.PAYMENTMETHODSMobileMoney)
                {
                    frmNonCash = new frmNonCashPayments();
                    frmNonCash.Show_Non_Cash_Payment_Form(RegistrationExt.Registration, voucherbuffer.Voucher.GrandTotal, RegistrationExt.Guest, _currenDateTime, Convert.ToInt32(lePaymentType.EditValue));

                    if (frmNonCash.Non_Cash_Payment == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Unable to save non-cash transaction", "ERROR");
                        return;
                    }
                    else
                    {
                        if (lePaymentType.EditValue != null && Convert.ToInt32(lePaymentType.EditValue) == CNETConstantes.PAYMNET_METHOD_CREDITCARD)
                        {
                            teAmount.Text = frmNonCash.Non_Cash_Payment.payment_Amount.ToString();
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
                        voucherbuffer.Voucher.Currency = Convert.ToInt32(cacCurrency.EditValue);
                    }
                }
                else if (lePaymentType.EditValue != null && Convert.ToInt32(lePaymentType.EditValue) == CNETConstantes.PAYMENTMETHODSMobileMoney)
                {
                    POS_Settings.Voucher_Definition = CNETConstantes.CASHRECIPT;
                    if (cacCustomer.EditValue != null)
                    {
                        customerDto = new Consignee();
                        customerDto.ID = Convert.ToInt32(cacCustomer.EditValue.ToString());
                        customerDto.Code = cacCustomer.EditValue.ToString();
                        //  POSSettingCache.SelectedCustomer.Name = cacCustomer.Text;


                    }

                    if (frmMobilePayments == null)
                        frmMobilePayments = new frmPaymentMethods(POS_Settings.Voucher_Definition);

                    if (frmMobilePayments.Check_Mobile_Payment(voucherbuffer, voucherbuffer.Voucher.Code, customerDto != null ? customerDto.ID : null, false, false))
                    {
                        frmMobilePayments.ShowDialog(this);
                        if (frmMobilePayments.payment_Successful)
                        {
                            var paymentProcessor = UIProcessManager.Get_ConsigneeUnit_By_Code(frmMobilePayments.Mobile_Transaction.PaymentProcessorConsigneeUnit.ToString());

                            voucherbuffer.Voucher.PaymentMethod = CNETConstantes.PAYMENTMETHODSMobileMoney;
                            voucherbuffer.Voucher.PaymentProcessor = paymentProcessor != null ? paymentProcessor.Id : null;
                            voucherbuffer.Voucher.Payer = frmMobilePayments.Mobile_Transaction.ConsigneeID;
                            voucherbuffer.Voucher.IsIncoming = true;
                            voucherbuffer.Voucher.PaymentIssueDate = frmMobilePayments.Mobile_Transaction.IssueDate;
                            voucherbuffer.Voucher.PaymentMaturityDate = frmMobilePayments.Mobile_Transaction.MaturityDate;
                            voucherbuffer.Voucher.PaymentRefNumber = frmMobilePayments.Mobile_Transaction.PaymentReference;
                            voucherbuffer.Voucher.PaymentAmount = voucherbuffer.Voucher.GrandTotal;
                            voucherbuffer.Voucher.PaymentStatus = CNETConstantes.Payment_Status_Executed;
                            voucherbuffer.Voucher.Currency = Convert.ToInt32(cacCurrency.EditValue);
                            //voucherbuffer.Voucher.Currency = Buffers.Currency_Buffer.FirstOrDefault(x => x.IsDefault) != null ? Buffers.Currency_Buffer.FirstOrDefault(x => x.IsDefault).Id : null;

                            //if (POS_Settings.Selected_Consignee != null)
                            //    voucherbuffer.Voucher.Consignee1 = POS_Settings.Selected_Consignee.ID;

                            POS_Settings.Selected_Payment_Type = new Payment_Method
                            {
                                Id = CNETConstantes.PAYMENTMETHODSMobileMoney,
                                Name = "Mobile",
                                Value = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == CNETConstantes.PAYMENTMETHODSMobileMoney).Value
                            };


                            // MobileNonCashTransaction = frmMobilePayments.Non_Cash_Transaction;
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("There is a problem with Mobile Payment !!", "ERROR");
                            ////CNETInfoReporter.Hide();
                            return;
                        }
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("There is a problem with Mobile Payment !!", "ERROR");
                        ////CNETInfoReporter.Hide();
                        return;
                    }
                }

                #endregion


                #region  for Refund

                if (VoucherType == CNETConstantes.REFUND)
                {
                    decimal taxRate = (decimal)tax.Amount;
                    grandTotal = subTotal;

                }


                #endregion


                #region  for CASHRECIPT

                if (VoucherType == CNETConstantes.CASHRECIPT && !LocalBuffer.LocalBuffer.PMSRecieptIsCheckout && !RegistrationExt.AuthorizeDirectBill)
                {

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

                    voucherbuffer.TaxTransactions = new List<TaxTransactionDTO>();
                    if (voFinal.taxTransactions != null && voFinal.taxTransactions.Count > 0)
                    {
                        foreach (var taxTransaction in voFinal.taxTransactions)
                            voucherbuffer.TaxTransactions.Add(taxTransaction);

                    }

                    // decimal taxRate = (decimal)tax.Amount;
                    grandTotal = voFinal.voucher.GrandTotal;


                    voucherbuffer.Voucher.SubTotal = voFinal.voucher.SubTotal;
                    voucherbuffer.Voucher.GrandTotal = voFinal.voucher.GrandTotal;

                }
                #endregion




                /* Printer Fiscal
                                #region Printer Fiscal for Refund

                                if (VoucherType == CNETConstantes.REFUND)
                                {
                                    decimal taxRate = (decimal)tax.Amount;
                                    grandTotal = subTotal;

                                    POS_Settings.IsError = true;
                                    POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                                    POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                                    POS_Settings.Voucher_Definition = CNETConstantes.REFUND;
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

                                    _printItems = GetPrintItems(voucherbuffer.Voucher.Code, 1, subTotal, tax.Id);


                                    //print operation
                                    //POSSettingCache.defultPaymnet = "Cash Sales";
                                    //POSSettingCache.defaultTransactionType = "Cash Sales";
                                    //POSSettingCache.voucherType = "Refund";
                                    //POSSettingCache.SelectedSalesType = "Cash Sales";
                                    //POSSettingCache.stationType = "Cash Sales";
                                    POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                                    //  _currentFSNumber = ReadFSForPrinting();
                                    PopulateCustomerDTO();


                                    POS_Settings.TotalServiceCharge = voucherbuffer.Voucher.AddCharge;
                                    POS_Settings.TotalDiscount = voucherbuffer.Voucher.Discount;
                                    PMSDataLogger.LogMessage("frmPMSVoucher", "Total Service Charge " + POS_Settings.TotalServiceCharge);
                                    PMSDataLogger.LogMessage("frmPMSVoucher", "Total Discount " + POS_Settings.TotalDiscount);

                                    bool isprinted = FiscalPrinters.PrintOperation(CNETConstantes.REFUND, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, voucherbuffer.Voucher.Code,
                                        null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucherbuffer.Voucher.IssuedDate, grandTotal, RegistrationExt.Room, RegistrationExt.Registration);

                                    //      bool isprinted = FiscalPrinters.PrintOperation(_printItems, POSSettingCache.printerType, new List<Consignee>() { customerDto }, 0, voucher.code, "",
                                    //"", LocalBuffer.LocalBuffer.CurrentLoggedInUser.userName, POSSettingCache.TotalDiscount, POSSettingCache.TotalServiceCharge, RegistrationExt.Room, RegistrationExt.Registration);

                                    //bool isprinted = FiscalPrinters.PrintOperation(_printItems, POSSettingCache.printerType, new List<CustomerDTO>() { customerDto }, 0, POSSettingCache.defultPaymnet, voucher.code,
                                    //    "", LocalBuffer.LocalBuffer.CurrentLoggedInUser.userName, POSSettingCache.TotalDiscount, POSSettingCache.TotalServiceCharge, RegistrationExt.Room, RegistrationExt.Registration);

                                    if (!isprinted)
                                    {
                                        XtraMessageBox.Show("Unable to make fisical print!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }

                                    voucherbuffer.Voucher.FsNumber = POS_Settings.CurrentFSNo;
                                    voucherbuffer.Voucher.Mrc = POS_Settings.fiscalprinterMRC;
                                    voucherbuffer.Voucher.GrandTotal = grandTotal;

                                    //reset values
                                    POS_Settings.TotalDiscount = 0;
                                    POS_Settings.TotalServiceCharge = 0;
                                }


                                #endregion


                                #region Printer Fiscal for CASHRECIPT

                                if (VoucherType == CNETConstantes.CASHRECIPT && !LocalBuffer.LocalBuffer.PMSRecieptIsCheckout)
                                {

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

                                    voucherbuffer.TaxTransactions = new List<TaxTransactionDTO>();
                                    if (voFinal.taxTransactions != null && voFinal.taxTransactions.Count > 0)
                                    {
                                        foreach (var taxTransaction in voFinal.taxTransactions)
                                            voucherbuffer.TaxTransactions.Add(taxTransaction);

                                    }

                                    decimal taxRate = (decimal)tax.Amount;
                                    grandTotal = voFinal.voucher.GrandTotal;


                                    if (cbeTransactionType.SelectedIndex == 0)
                                        POS_Settings.Voucher_Definition = CNETConstantes.CASH_SALES;
                                    else
                                        POS_Settings.Voucher_Definition = CNETConstantes.CREDITSALES;

                                    POS_Settings.IsError = true;
                                    POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                                    POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;
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


                                    List<VwLineItemDetailViewDTO> dtoList = gcCRVLineItem.DataSource as List<VwLineItemDetailViewDTO>;
                                    if (dtoList != null && dtoList.Count > 0)
                                    {
                                        if (_printItems != null) _printItems.Clear();
                                        _printItems = GetPrintCRVItems(dtoList, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
                                    }
                                    else
                                    {
                                        SystemMessage.ShowModalInfoMessage("Unable to get print Items", "ERROR");
                                        return;
                                    }

                                    //print operation
                                    //POSSettingCache.defultPaymnet = "Cash Sales";
                                    //POSSettingCache.defaultTransactionType = "Cash Sales";
                                    //POSSettingCache.voucherType = "Refund";
                                    //POSSettingCache.SelectedSalesType = "Cash Sales";
                                    //POSSettingCache.stationType = "Cash Sales";
                                    POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                                    //  _currentFSNumber = ReadFSForPrinting();
                                    PopulateCustomerDTO();


                                    POS_Settings.TotalServiceCharge = voucherbuffer.Voucher.AddCharge;
                                    POS_Settings.TotalDiscount = voucherbuffer.Voucher.Discount;
                                    PMSDataLogger.LogMessage("frmPMSVoucher", "Total Service Charge " + POS_Settings.TotalServiceCharge);
                                    PMSDataLogger.LogMessage("frmPMSVoucher", "Total Discount " + POS_Settings.TotalDiscount);

                                    bool isprinted = FiscalPrinters.PrintOperation(POS_Settings.Voucher_Definition, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, voucherbuffer.Voucher.Code,
                                        null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucherbuffer.Voucher.IssuedDate, voFinal.voucher.GrandTotal, RegistrationExt.Room, RegistrationExt.Registration);

                                    //      bool isprinted = FiscalPrinters.PrintOperation(_printItems, POSSettingCache.printerType, new List<Consignee>() { customerDto }, 0, voucher.code, "",
                                    //"", LocalBuffer.LocalBuffer.CurrentLoggedInUser.userName, POSSettingCache.TotalDiscount, POSSettingCache.TotalServiceCharge, RegistrationExt.Room, RegistrationExt.Registration);

                                    //bool isprinted = FiscalPrinters.PrintOperation(_printItems, POSSettingCache.printerType, new List<CustomerDTO>() { customerDto }, 0, POSSettingCache.defultPaymnet, voucher.code,
                                    //    "", LocalBuffer.LocalBuffer.CurrentLoggedInUser.userName, POSSettingCache.TotalDiscount, POSSettingCache.TotalServiceCharge, RegistrationExt.Room, RegistrationExt.Registration);

                                    if (!isprinted)
                                    {
                                        XtraMessageBox.Show("Unable to make fisical print!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }

                                    voucherbuffer.Voucher.FsNumber = POS_Settings.CurrentFSNo;
                                    voucherbuffer.Voucher.Mrc = POS_Settings.fiscalprinterMRC;
                                    voucherbuffer.Voucher.GrandTotal = voFinal.voucher.GrandTotal;

                                    //reset values
                                    POS_Settings.TotalDiscount = 0;
                                    POS_Settings.TotalServiceCharge = 0;
                                }
                                #endregion

                                */

                if (_voucherExt != null && !string.IsNullOrEmpty(teReference.Text))
                    voucherbuffer.Voucher.Extension1 = teReference.Text;



                voucherbuffer.Voucher.Note = mePurpose.Text;
                voucherbuffer.Voucher.OriginConsigneeUnit = RegistrationExt.ConsigneeUnit;
                voucherbuffer.TaxTransactions = new List<TaxTransactionDTO>();





                if (VoucherType == CNETConstantes.CREDIT_NOTE_VOUCHER || VoucherType == CNETConstantes.REFUND)
                {

                    decimal taxRate = (decimal)tax.Amount;
                    //Save Tax Transction
                    decimal taxAmt = 0;
                    if (VoucherType == CNETConstantes.CREDIT_NOTE_VOUCHER)
                    {
                        if (ceNetoff.Checked)
                        {
                            taxAmt = Math.Round(grandTotal - subTotal, 2);
                        }
                        else
                        {

                            taxAmt = Math.Round(grandTotal * ((taxRate / 100)), 2);
                        }
                    }
                    else
                    {
                        taxAmt = Math.Round(grandTotal - subTotal, 2);
                    }
                    voucherbuffer.TaxTransactions.ToList().Add(new TaxTransactionDTO()
                    {
                        TaxableAmount = subTotal,
                        Tax = tax.Id,
                        TaxAmount = taxAmt,
                        Remark = "Refund"
                    });
                }
                voucherbuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
                TransactionReferenceBuffer TrBuffer = new TransactionReferenceBuffer();
                TrBuffer.TransactionReference = new TransactionReferenceDTO();
                TrBuffer.TransactionReference.ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER;
                TrBuffer.TransactionReference.Referenced = RegistrationExt.Id;
                TrBuffer.TransactionReference.ReferencingVoucherDefn = voucherbuffer.Voucher.Definition;
                TrBuffer.TransactionReference.RelationType = luk_window.EditValue == null ? CNETConstantes.DEFAULT_WINDOW : Convert.ToInt32(luk_window.EditValue);
                if (teTotalAmount.Text != "")
                    TrBuffer.TransactionReference.Value = grandTotal;
                else
                    TrBuffer.TransactionReference.Value = 0;

                if (VoucherType == CNETConstantes.REFUND)
                    TrBuffer.TransactionReference.Value = 1; //means the bill is printed in fiscal printer
                TrBuffer.ReferencedActivity = null;
                voucherbuffer.TransactionReferencesBuffer.Add(TrBuffer);

                voucherbuffer.Activity = ActivityLogManager.SetupActivity(_currenDateTime, workflow.Id, CNETConstantes.VOUCHER_COMPONENET);

                if (voucherbuffer.Voucher.Definition == CNETConstantes.CREDIT_NOTE_VOUCHER)
                    voucherbuffer.Voucher.Remark = ceNetoff.Checked ? "netoff_on" : "netoff_off";


                // Create Transaction Currency 
                decimal unitAmount = string.IsNullOrEmpty(teAmount.Text) ? 0 : Convert.ToDecimal(teAmount.Text);
                decimal totalAmount = string.IsNullOrEmpty(teTotalAmount.Text) ? 0 : Convert.ToDecimal(teTotalAmount.Text);

                // for paid-out put avoid -ve value entry
                if (VoucherType == CNETConstantes.PAID_OUT_VOUCHER)
                {
                    unitAmount = Math.Abs(unitAmount);
                    totalAmount = Math.Abs(totalAmount);
                }

                if (frmForeignTrans != null && frmForeignTrans.AddedForeignExTranList.Count > 0)
                {

                    //foreach (var forex in frmForeignTrans.AddedForeignExTranList)
                    //{
                    //    TransactionCurrencyDTO tranCurrency = new TransactionCurrencyDTO();
                    //    tranCurrency.Currency = forex.CurrencyCode.Value;
                    //    tranCurrency.Amount = forex.Amount;
                    //    tranCurrency.Rate = forex.ExRate;
                    //    tranCurrency.Total = forex.TotalAmount;
                    //}

                    foreach (var forex in frmForeignTrans.AddedForeignExTranList)
                    {
                        voucherbuffer.TransactionCurrencyBuffer = new CNET_V7_Domain.Domain.Transaction.TransactionCurrencyBuffer();
                        voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency = new TransactionCurrencyDTO();
                        voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Currency = forex.CurrencyCode.Value;
                        voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Amount = forex.Amount;
                        voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Rate = forex.ExRate;
                        voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Total = forex.TotalAmount;
                    }
                }
                else
                {
                    voucherbuffer.TransactionCurrencyBuffer = new CNET_V7_Domain.Domain.Transaction.TransactionCurrencyBuffer();
                    voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency = new TransactionCurrencyDTO();
                    voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Currency = Convert.ToInt32(cacCurrency.EditValue);
                    voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Amount = unitAmount;
                    voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Rate = exRate;
                    voucherbuffer.TransactionCurrencyBuffer.TransactionCurrency.Total = totalAmount;
                }

                #region Non Cash Transaction

                /*
                if (lePaymentType.EditValue != null && Convert.ToInt32(lePaymentType.EditValue) != CNETConstantes.PAYMENTMETHODSMobileMoney && Convert.ToInt32(lePaymentType.EditValue) != CNETConstantes.PAYMENTMETHODSCASH)
                {
                    if (frmNonCash.Non_Cash_Payment != null)
                    {
                       // voucherbuffer.Voucher.paymen = frmNonCash.Non_Cash_Payment.executed;
                        voucherbuffer.Voucher.IsIncoming = frmNonCash.Non_Cash_Payment.is_Incoming;
                        voucherbuffer.Voucher.PaymentAmount = frmNonCash.Non_Cash_Payment.payment_Amount;
                        voucherbuffer.Voucher.PaymentIssueDate = frmNonCash.Non_Cash_Payment.payment_Date;
                        voucherbuffer.Voucher.PaymentMaturityDate = frmNonCash.Non_Cash_Payment.maturity_Date;
                        voucherbuffer.Voucher.PaymentRefNumber = frmNonCash.Non_Cash_Payment.payment_Reference;
                        voucherbuffer.Voucher.PaymentMethod = frmNonCash.Non_Cash_Payment.payment_Method;
                        voucherbuffer.Voucher.Currency = frmNonCash.Non_Cash_Payment.currency;
                    }
                    else
                        voucherbuffer.Voucher.PaymentMethod = CNETConstantes.PAYMENTMETHODSCASH;
                }
                else if (lePaymentType.EditValue != null && Convert.ToInt32(lePaymentType.EditValue) == CNETConstantes.PAYMENTMETHODSMobileMoney)
                {
                    //voucherbuffer.NonCashTransactions = new List<NonCashTransactionDTO>();
                    //voucherbuffer.NonCashTransactions.Add(MobileNonCashTransaction);
                }
                */
                #endregion

                int? paymentmet = null;

                if (lePaymentType.EditValue != null && !string.IsNullOrEmpty(lePaymentType.EditValue.ToString()))
                    paymentmet = Convert.ToInt32(lePaymentType.EditValue);



                voucherbuffer.Voucher.PaymentMethod = paymentmet;
                voucherbuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                voucherbuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                ResponseModel<VoucherBuffer> saved = UIProcessManager.CreateVoucherBuffer(voucherbuffer);

                if (saved != null && saved.Success)
                {
                    //Save Activity for The Registration Voucher
                    ActivityDTO activityReg = ActivityLogManager.SetupActivity(_currenDateTime, workflow.Id, CNETConstantes.PMS_Pointer, string.Format("{0} is made", this.Text));
                    activityReg.Reference = RegistrationExt.Id;
                    UIProcessManager.CreateActivity(activityReg);
                    /*
                                        if (frmNonCash != null)
                                        {
                                            if (Convert.ToInt32(lePaymentType.EditValue) == CNETConstantes.PAYMNET_METHOD_CREDITCARD && CommisssionArticle != null)
                                            {
                                                CreateAndPrintCreditCardComission(frmNonCash.Non_Cash_Payment.payment_Method);
                                            }
                                        }*/

                    #region Printer Fiscal for Refund

                    if (VoucherType == CNETConstantes.REFUND)
                    {
                        decimal taxRate = (decimal)tax.Amount;
                        grandTotal = subTotal;

                        POS_Settings.IsError = true;
                        POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                        POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                        POS_Settings.Voucher_Definition = CNETConstantes.REFUND;
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

                        _printItems = GetPrintItems(voucherbuffer.Voucher.Code, 1, subTotal, tax.Id);


                        //print operation
                        //POSSettingCache.defultPaymnet = "Cash Sales";
                        //POSSettingCache.defaultTransactionType = "Cash Sales";
                        //POSSettingCache.voucherType = "Refund";
                        //POSSettingCache.SelectedSalesType = "Cash Sales";
                        //POSSettingCache.stationType = "Cash Sales";
                        POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                        //  _currentFSNumber = ReadFSForPrinting();
                        PopulateCustomerDTO();


                        POS_Settings.TotalServiceCharge = voucherbuffer.Voucher.AddCharge;
                        POS_Settings.TotalDiscount = voucherbuffer.Voucher.Discount;
                        PMSDataLogger.LogMessage("frmPMSVoucher", "Total Service Charge " + POS_Settings.TotalServiceCharge);
                        PMSDataLogger.LogMessage("frmPMSVoucher", "Total Discount " + POS_Settings.TotalDiscount);

                        PMSDataLogger.LogMessage("frmPMSVoucher", "Printint REFUND " + POS_Settings.TotalServiceCharge);
                        bool isprinted = FiscalPrinters.PrintOperation(CNETConstantes.REFUND, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, voucherbuffer.Voucher.Code,
                            null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucherbuffer.Voucher.IssuedDate, grandTotal, RegistrationExt.Room, RegistrationExt.Registration);

                        //      bool isprinted = FiscalPrinters.PrintOperation(_printItems, POSSettingCache.printerType, new List<Consignee>() { customerDto }, 0, voucher.code, "",
                        //"", LocalBuffer.LocalBuffer.CurrentLoggedInUser.userName, POSSettingCache.TotalDiscount, POSSettingCache.TotalServiceCharge, RegistrationExt.Room, RegistrationExt.Registration);

                        //bool isprinted = FiscalPrinters.PrintOperation(_printItems, POSSettingCache.printerType, new List<CustomerDTO>() { customerDto }, 0, POSSettingCache.defultPaymnet, voucher.code,
                        //    "", LocalBuffer.LocalBuffer.CurrentLoggedInUser.userName, POSSettingCache.TotalDiscount, POSSettingCache.TotalServiceCharge, RegistrationExt.Room, RegistrationExt.Registration);

                        if (!isprinted)
                        {
                            PMSDataLogger.LogMessage("frmPMSVoucher", "Printint REFUND Fail !!");
                            XtraMessageBox.Show("Unable to make fisical print!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            PMSDataLogger.LogMessage("frmPMSVoucher", "Delete  REFUND Voucher " + saved.Data.Voucher.Code + " frmPMSVoucher Form");
                            UIProcessManager.DeleteVoucherObjects(saved.Data.Voucher.Id);
                            return;
                        }


                        voucherbuffer.Voucher.GrandTotal = grandTotal;

                        UIProcessManager.Patch_FS_No(saved.Data.Voucher.Id, POS_Settings.CurrentFSNo, POS_Settings.fiscalprinterMRC);


                        //reset values
                        POS_Settings.TotalDiscount = 0;
                        POS_Settings.TotalServiceCharge = 0;
                    }


                    #endregion


                    #region Printer Fiscal for CASHRECIPT

                    if (VoucherType == CNETConstantes.CASHRECIPT && !LocalBuffer.LocalBuffer.PMSRecieptIsCheckout && !RegistrationExt.AuthorizeDirectBill)
                    {
                        if (cbeTransactionType.SelectedIndex == 0)
                            POS_Settings.Voucher_Definition = CNETConstantes.CASH_SALES;
                        else
                            POS_Settings.Voucher_Definition = CNETConstantes.CREDITSALES;

                        POS_Settings.IsError = true;
                        POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                        POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                        POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);



                        // FiscalPrinters FP = new FiscalPrinters();
                        FiscalPrinters.GetInstance();
                        if (!POS_Settings.IsError)
                        {
                            XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            PMSDataLogger.LogMessage("frmPMSVoucher", "\"Unable to connect with fisical printer !!"); 


                            PMSDataLogger.LogMessage("frmPMSVoucher", "Delete CASH RECIPT Voucher " + saved.Data.Voucher.Code + " frmPMSVoucher Form");
                            UIProcessManager.DeleteVoucherObjects(saved.Data.Voucher.Id);
                            return;
                        }
                        bool validatePMSLicense = CommonLogics.Validate_PMSPOS_License();
                        if (!validatePMSLicense)
                        {
                           // XtraMessageBox.Show("License Error !!!", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            PMSDataLogger.LogMessage("frmPMSVoucher", "License Error !!");


                            PMSDataLogger.LogMessage("frmPMSVoucher", "Delete CASH RECIPT Voucher " + saved.Data.Voucher.Code + " frmPMSVoucher Form");
                            UIProcessManager.DeleteVoucherObjects(saved.Data.Voucher.Id);
                            return;
                        }

                        List<VwLineItemDetailViewDTO> dtoList = gcCRVLineItem.DataSource as List<VwLineItemDetailViewDTO>;
                        if (dtoList != null && dtoList.Count > 0)
                        {
                            if (_printItems != null) _printItems.Clear();
                            _printItems = GetPrintCRVItems(dtoList, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER);
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to get print Items", "ERROR");
                            return;
                        }


                        POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                        //  _currentFSNumber = ReadFSForPrinting();
                        PopulateCustomerDTO();


                        POS_Settings.TotalServiceCharge = voucherbuffer.Voucher.AddCharge;
                        POS_Settings.TotalDiscount = voucherbuffer.Voucher.Discount;
                        //PMSDataLogger.LogMessage("frmPMSVoucher", "Total Service Charge " + POS_Settings.TotalServiceCharge);
                        //PMSDataLogger.LogMessage("frmPMSVoucher", "Total Discount " + POS_Settings.TotalDiscount);
                        //PMSDataLogger.LogMessage("frmPMSVoucher", "Printint CASHRECIPT " + POS_Settings.TotalServiceCharge);

                        bool isprinted = FiscalPrinters.PrintOperation(POS_Settings.Voucher_Definition, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, voucherbuffer.Voucher.Code,
                            null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucherbuffer.Voucher.IssuedDate, voFinal.voucher.GrandTotal, RegistrationExt.Room, RegistrationExt.Registration);


                        if (!isprinted)
                        {

                            PMSDataLogger.LogMessage("frmPMSVoucher", "Printint CASHRECIPT Fail !!");
                            XtraMessageBox.Show("Unable to make fisical print!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);


                            PMSDataLogger.LogMessage("frmPMSVoucher", "Delete CASH RECIPT Voucher " + saved.Data.Voucher.Code + " frmPMSVoucher Form");
                            UIProcessManager.DeleteVoucherObjects(saved.Data.Voucher.Id);
                            return;
                        }
                        else
                        {


                        }


                        PMSDataLogger.LogMessage("frmPMSVoucher", "Update Fs No " + saved.Data.Voucher.Code +" "+ POS_Settings.CurrentFSNo+ " frmPMSVoucher Form");
                        UIProcessManager.Patch_FS_No(saved.Data.Voucher.Id, POS_Settings.CurrentFSNo, POS_Settings.fiscalprinterMRC);

                        //reset values
                        POS_Settings.TotalDiscount = 0;
                        POS_Settings.TotalServiceCharge = 0;
                    }
                    #endregion




                    try
                    {
                        DocumentPrint.ReportGenerator reportGenerator = new DocumentPrint.ReportGenerator();
                        reportGenerator.GetAttachementReport(saved.Data.Voucher.Id);
                    }
                    catch (Exception ex)
                    {

                    }
                    XtraMessageBox.Show(this.Text + " is successfully saved", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();

                    if (frmForeignTrans != null && frmForeignTrans.AddedForeignExTranList.Count > 0)
                    {
                        //ReportGenerator rg = new ReportGenerator();
                        //decimal total = frmForeignTrans.AddedForeignExTranList.Sum(d => d.TotalAmount);
                        //rg.GenerateForeginCurrency(frmForeignTrans.AddedForeignExTranList, voucher.code, voucher.grandTotal, total, false);
                    }
                    //XtraMessageBox.Show(this.Text + " is successfully saved", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    frmForeignTrans = null;

                    if (RegistrationExt.lastState == CNETConstantes.SIX_PM_STATE)
                    {
                        DialogResult dr = XtraMessageBox.Show("Do you want to change this registration to Guaranteed reservation?", "CNET_v2016", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == System.Windows.Forms.DialogResult.Yes)
                        {

                            frmConformation frmConform = new frmConformation();
                            frmConform.RegExtension = RegistrationExt;
                            if (frmConform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                DialogResult = DialogResult.OK;
                            }

                        }

                    }
                    if (workflow.IssuingEffect)
                    {
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        XtraMessageBox.Show("This voucher is not issued!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        DialogResult = DialogResult.Cancel;
                    }


                    this.Close();
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Saving not successful" + Environment.NewLine + saved.Message, "ERROR");
                    PMSDataLogger.LogMessage("frmPMSVoucher", "Saving Voucher Fail !! " + saved.Data.Voucher.Code + " frmPMSVoucher Form");
                    PMSDataLogger.LogMessage("frmPMSVoucher", "Saving Voucher Fail !! " + Environment.NewLine + saved.Message + " frmPMSVoucher Form");
                }
                ////CNETInfoReporter.Hide();
                if (VoucherType == CNETConstantes.CREDIT_NOTE_VOUCHER)
                {
                    chkPercent.Checked = false;
                    txtPercent.EditValue = "0";
                }
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("ERROR! error in saving " + this.Text + ". DETAIL:: " + ex.Message);
            }
        }


        private void CalculateCRVAccomodationLineItem(decimal unitAmount)
        {
            RateCodeHeaderDTO rateCodeHeader = UIProcessManager.GetRateCodeHeaderById(registrationExt.RateCodeHeader.Value);
            if (rateCodeHeader == null)
            {
                SystemMessage.ShowModalInfoMessage("Unable to find the rate header", "ERROR");
                return;
            }

            TaxDTO tax = CommonLogics.GetApplicableTax(registrationExt.Id, CNETConstantes.CASHRECIPT, registrationExt.GuestId, rateCodeHeader.Article);
            if (!string.IsNullOrEmpty(tax.Remark))
            {
                SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                return;
            
            }



            //calculate line item
            LineItemDTO lineItem = new LineItemDTO();
            //lineItem.Voucher = Voucher.code;
            lineItem.Quantity = 1;
            lineItem.Article = rateCodeHeader.Article;
            lineItem.UnitAmount = Convert.ToDecimal(unitAmount);
            lineItem.Tax = tax.Id;
            lineItem.ObjectState = null;

           List<ValueFactorDefinitionDTO> ArticleServiceCharge = new NewLineItemCaculator().GetValueFactorDefnsFromSetting(voucherbuffer.Voucher, lineItem, "service");
            if (ArticleServiceCharge != null && ArticleServiceCharge.Count > 0)
            {
                ValueFactorDefinitionDTO service = ArticleServiceCharge.FirstOrDefault();
                if (PMSVoucherSetting.ValueIsTaxInclusive == true)
                {
                    tax = CommonLogics.GetApplicableTax(registrationExt.Id, voucherbuffer.Voucher.Definition, voucherbuffer.Voucher.Consignee1, lineItem.Article);
                    if (!string.IsNullOrEmpty(tax.Remark))
                    {
                        XtraMessageBox.Show(tax.Remark, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    unitAmount = unitAmount / (1 + Convert.ToDecimal(tax.Amount) / 100);//Extract tax


                    if (service.IsPercent)
                        unitAmount = unitAmount / (1 + Convert.ToDecimal(service.Value) / 100);//Extract Servicecharge
                    else
                        unitAmount = unitAmount - service.Value;//Extract Servicecharge


                    unitAmount = unitAmount * (1 + Convert.ToDecimal(tax.Amount) / 100);//Return tax 

                    lineItem.UnitAmount = Convert.ToDecimal(unitAmount);
                }
                else
                {

                    if (service.IsPercent)
                        unitAmount = unitAmount / (1 + Convert.ToDecimal(service.Value) / 100);//Extract Servicecharge
                    else
                        unitAmount = unitAmount - service.Value;//Extract Servicecharge

                    lineItem.UnitAmount = Convert.ToDecimal(unitAmount);
                }
            }


            //note: price is already extracted
            LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(voucherbuffer, lineItem, registrationExt.Id, null, null, null, null, false, false, false, false);
            LineItemDetails liDetails = new LineItemDetails()
            {
                lineItems = liDetail.lineItem,
                lineItemValueFactor = liDetail.lineItemValueFactor
            };
            List<LineItemDetails> liDetailsList = new List<LineItemDetails>();
            liDetailsList.Add(liDetails);
            voFinal = new VoucherFinalCalculator().VoucherCalculation(voucherbuffer.Voucher, liDetailsList);



            if (voFinal.lineItemDetails == null)
            {
                SystemMessage.ShowModalInfoMessage("No line item detail for this registration", "ERROR");
                ////CNETInfoReporter.Hide();
                return;
            }
            List<VwLineItemDetailViewDTO> lineItemDetail = new List<VwLineItemDetailViewDTO>();
            List<LineItemDetails> lineItemDetails = voFinal.lineItemDetails;
            foreach (var le in lineItemDetails)
            {
                if (le != null)
                {

                    VwLineItemDetailViewDTO lineIte = new VwLineItemDetailViewDTO();
                    lineIte.Code = le.lineItems.Article.ToString();
                    lineIte.Name = "Accomodation";
                    lineIte.Quantity = 1;
                    if (le.lineItems.UnitAmount != null)
                    {
                        lineIte.UnitAmount = Math.Round(le.lineItems.UnitAmount, 2);
                        if (le.lineItems.TotalAmount != null)
                            lineIte.TotalAmount = Math.Round(le.lineItems.TotalAmount, 2);
                    }
                    lineIte.AddCharge = le.lineItems.AddCharge;
                    lineIte.Discount = le.lineItems.Discount;
                    lineItemDetail.Add(lineIte);
                }
            }

            if (voFinal.taxTransactions != null)
                voucherbuffer.TaxTransactions = voFinal.taxTransactions;




            txtServicecharge.Text = voFinal.voucher.AddCharge.ToString("N2");
            txtTax.Text = voFinal.taxTransactions.Sum(x => x.TaxAmount)?.ToString("N2");
            txtGrandTotal.Text = voFinal.voucher.GrandTotal.ToString("N2");
            gcCRVLineItem.DataSource = lineItemDetail;
        }

        VoucherFinalDTO voFinal = null;
        private void CreateAndPrintCreditCardComission(int paymentProcesser)
        {
            Decimal Comissionvalue = Convert.ToDecimal(Convert.ToDecimal(teTotalAmount.Text)) - OriginalAmount;

            if (Comissionvalue > 0)
            {
                decimal TaxableAmount = 0;
                decimal TaxAmount = 0;
                decimal GrandAmount = 0;

                SystemConstantDTO SelectedcreditCard = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == paymentProcesser);
                if (SelectedcreditCard.Value != null && !string.IsNullOrEmpty(SelectedcreditCard.Value) && Convert.ToDecimal(SelectedcreditCard.Value) > 0)
                {
                    decimal ComissionPercent = Convert.ToDecimal(SelectedcreditCard.Value);

                    decimal AmountAfterComission = ((Convert.ToDecimal(OriginalAmount) * ComissionPercent) / 100);
                    TaxableAmount = AmountAfterComission;
                    TaxDTO tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(x => x.Id == CommisssionArticleTax.Tax);
                    if (tax != null)
                    {
                        TaxAmount = (((decimal)tax.Amount / 100) * AmountAfterComission);
                        GrandAmount = TaxableAmount + TaxAmount;
                    }
                }

                VoucherBuffer voucherbuffer = new VoucherBuffer();

                string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.CREDITSALES, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);


                voucherbuffer.Voucher.Code = currentVoCode;
                voucherbuffer.Voucher.Definition = CNETConstantes.CREDITSALES;
                voucherbuffer.Voucher.Type = CNETConstantes.TRANSACTIONTYPENORMALTXN;
                voucherbuffer.Voucher.IssuedDate = CurrentTime.Value;
                voucherbuffer.Voucher.Year = CurrentTime.Value.Year;
                voucherbuffer.Voucher.Month = CurrentTime.Value.Month;
                voucherbuffer.Voucher.Day = CurrentTime.Value.Day;
                voucherbuffer.Voucher.IsVoid = false;
                voucherbuffer.Voucher.IsIssued = true;
                voucherbuffer.Voucher.Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime.Value);
                voucherbuffer.Voucher.LastState = workflow.State.Value;
                voucherbuffer.Voucher.Consignee1 = RegistrationExt.GuestId;
                voucherbuffer.Voucher.GrandTotal = GrandAmount;
                voucherbuffer.Voucher.SubTotal = TaxableAmount;
                voucherbuffer.Voucher.Discount = 0;
                voucherbuffer.Voucher.AddCharge = 0;
                voucherbuffer.Voucher.Note = "Credit Card Commission";




                voucherbuffer.LineItemsBuffer = new List<LineItemBuffer>();
                LineItemBuffer lineItemBuffer = new LineItemBuffer();

                lineItemBuffer.LineItem = new LineItemDTO
                {
                    Article = CommisssionArticle.Id,
                    UnitAmount = TaxableAmount,
                    Quantity = 1,
                    Uom = CommisssionArticle.Uom,
                    TotalAmount = TaxableAmount,
                    TaxableAmount = TaxableAmount,
                    TaxAmount = TaxAmount,
                    Tax = CommisssionArticleTax.Tax,
                    CalculatedCost = 0,
                    ObjectState = null
                };

                voucherbuffer.LineItemsBuffer.Add(lineItemBuffer);


                voucherbuffer.TaxTransactions = new List<TaxTransactionDTO>();

                TaxTransactionDTO taxTransaction = new TaxTransactionDTO
                {
                    Tax = CommisssionArticleTax.Tax,
                    TaxableAmount = TaxableAmount,
                    TaxAmount = TaxAmount,
                };
                voucherbuffer.TaxTransactions.Add(taxTransaction);


                voucherbuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
                TransactionReferenceBuffer TRBuffer = new TransactionReferenceBuffer();
                TRBuffer.TransactionReference = new TransactionReferenceDTO
                {
                    ReferencingVoucherDefn = CNETConstantes.CREDITSALES,
                    Referenced = RegistrationExt.Id,
                    ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                    Value = GrandAmount

                };
                TRBuffer.ReferencedActivity = null;
                voucherbuffer.TransactionReferencesBuffer.Add(TRBuffer);


                voucherbuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                voucherbuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                voucherbuffer.TransactionCurrencyBuffer = null;

                ResponseModel<VoucherBuffer> vouchersaved = UIProcessManager.CreateVoucherBuffer(voucherbuffer);
                if (vouchersaved != null && vouchersaved.Success)
                {

                    bool CreditCardIssuesReceipt = false;
                    ConfigurationDTO CreditCardIssuesReceiptConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Reference == LocalBuffer.LocalBuffer.CurrentDevice.Id.ToString() && x.Attribute == "Credit Card Issues Receipt");
                    if (CreditCardIssuesReceiptConfig != null)
                    {
                        CreditCardIssuesReceipt = Convert.ToBoolean(CreditCardIssuesReceiptConfig.CurrentValue);
                    }
                    if (CreditCardIssuesReceipt)
                    {
                        List<Print_Item> printItemList = new List<Print_Item>();
                        Print_Item pItem = new Print_Item();
                        pItem.ID = CommisssionArticle.Id;
                        pItem.Name = CommisssionArticle.Name;
                        pItem.UnitPrice = Math.Round(TaxableAmount, _roundCalculatorDigit);
                        pItem.Quantity = 1;
                        pItem.taxId = CommisssionArticleTax.Tax;
                        pItem.isValueDiscount = true;
                        pItem.isValuePercent = true;
                        printItemList.Add(pItem);

                        PrintCreditCardCommission(printItemList, vouchersaved.Data.Voucher);
                    }
                }
                else
                {
                    SystemMessage.ShowModalInfoMessage("Saving not successful" + Environment.NewLine + vouchersaved.Message, "ERROR");
                }
            }
        }

        private async void PrintCreditCardCommission(List<Print_Item> printItemList, VoucherDTO voucher)
        {



            bool PrintCreditCardCommission = false;
            ConfigurationDTO PrintCreditCardCommissionConfig = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Reference == LocalBuffer.LocalBuffer.CurrentDevice.Id.ToString() && x.Attribute == "Print Credit Card Commission");
            if (PrintCreditCardCommissionConfig != null)
            {
                PrintCreditCardCommission = Convert.ToBoolean(PrintCreditCardCommissionConfig.CurrentValue);
            }



            //print operation
            POS_Settings.IsError = true;
            POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
            POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;

            POS_Settings.Voucher_Definition = CNETConstantes.CREDITSALES;
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

            FiscalPrinters fpSetup = new FiscalPrinters();
            CNET.POS.Settings.POS_Settings.System_Constants_Buffer = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList;
            //  bool isFpOpened = fpSetup.OpenFisicalPrinter(RegistrationExt.Consignee);
            if (!POS_Settings.IsError)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
            }


            PopulateCustomerDTO();



            bool isprinted = FiscalPrinters.PrintOperation(CNETConstantes.CREDITSALES, _printItems, POS_Settings.printerType, null, 0, voucher.Code,
                null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, voucher.IssuedDate, voucher.GrandTotal, null, RegistrationExt.Registration);

            if (!isprinted)
            {
                XtraMessageBox.Show("Unable to make fisical print for credit card commission!", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //  return;
            }
            if (PrintCreditCardCommission)
            {
                try
                {
                    DocumentPrint.ReportGenerator reportGenerator = new DocumentPrint.ReportGenerator();
                    reportGenerator.GetAttachementReport(voucher.Id);
                }
                catch (Exception ex)
                {

                }
            }
        }
        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset();
        }

        private void lePaymentType_KeyDown(object sender, KeyEventArgs e)
        {
            LookUpEdit edit = sender as LookUpEdit;
            if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                edit.ClosePopup();
                edit.EditValue = null;

            }
            e.Handled = true;
        }

        private void cacCurrency_EditValueChanged(object sender, EventArgs e)
        {
            exRate = CommonLogics.GetLatestExchangeRate(Convert.ToInt32(cacCurrency.EditValue));
            te_ExRate.EditValue = exRate;
            if (!string.IsNullOrEmpty(teAmount.Text))
            {
                if (VoucherType == CNETConstantes.CASHRECIPT)
                    teTotalAmount.Text = Math.Abs(Math.Round((exRate * Convert.ToDecimal(teAmount.Text.ToString())), 2)).ToString();
                else
                    teTotalAmount.Text = Math.Round((exRate * Convert.ToDecimal(teAmount.Text.ToString())), 2).ToString();
            }

            if (!LocalBuffer.LocalBuffer.PMSRecieptIsCheckout && !string.IsNullOrEmpty(teTotalAmount.Text) && !RegistrationExt.AuthorizeDirectBill)
            {
                decimal unitamount = 0;

                decimal.TryParse(teTotalAmount.Text, out unitamount);

                if (unitamount > 0)
                    CalculateCRVAccomodationLineItem(unitamount);
            }
        }

        private void teAmount_Click(object sender, EventArgs e)
        {
            teAmount.Text = string.Empty;
        }

        private void frmPMSVouchercs_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }


        private void bbiForegin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (frmForeignTrans == null)
                frmForeignTrans = new frmForeignExTransaction(teVoucherNo.Text, Convert.ToDecimal(teTotalAmount.Text));


            if (frmForeignTrans.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                statusLabel.Text = "Foreign currency transaction is attached";
                statusLabel.Visible = true;
            }
            else
            {
                frmForeignTrans = null;
                statusLabel.Text = "";
                statusLabel.Visible = false;
            }
        }

        private void luk_window_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit view = sender as LookUpEdit;
            if (view == null || view.EditValue == null) return;

            //Get Remaining Balance
            if (VoucherType == CNETConstantes.CASHRECIPT || VoucherType == CNETConstantes.PAID_OUT_VOUCHER || VoucherType == CNETConstantes.REFUND)
            {
                int window = Convert.ToInt32(view.EditValue);


                GuestLedgerDTO gLedger = null;
                if (window > 0)
                {
                    gLedger = UIProcessManager.GetGuestLedger(RegistrationExt.Id, RegistrationExt.Arrival.Date, RegistrationExt.Departure.Date, RegistrationExt.Room, window);
                }
                else
                {
                    gLedger = UIProcessManager.GetGuestLedger(RegistrationExt.Id, RegistrationExt.Arrival.Date, RegistrationExt.Departure.Date, RegistrationExt.Room, null);
                }
                if (gLedger != null)
                {
                    if (VoucherType == CNETConstantes.PAID_OUT_VOUCHER)
                    {
                        teAmount.Text = Math.Abs(gLedger.RemainingBalance).ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        teAmount.Text = gLedger.RemainingBalance.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else
                {

                    teAmount.Text = "0.00";
                }

            }
            else if (VoucherType == CNETConstantes.CREDIT_NOTE_VOUCHER)
            {
                int window = Convert.ToInt32(view.EditValue);
                GuestLedgerDTO gLedger = null;
                if (window > 0)
                {
                    gLedger = UIProcessManager.GetGuestLedger(RegistrationExt.Id,
                    RegistrationExt.Arrival.Date, RegistrationExt.Departure.Date, RegistrationExt.Room, window);
                }
                else
                {
                    gLedger = UIProcessManager.GetGuestLedger(RegistrationExt.Id,
                    RegistrationExt.Arrival.Date, RegistrationExt.Departure.Date, RegistrationExt.Room, null);
                }
                if (gLedger != null)
                {
                    RebateAmount = gLedger.RemainingBalance;//.ToString(CultureInfo.InvariantCulture);
                    teAmount.Text = RebateAmount.ToString(CultureInfo.InvariantCulture);
                    txtTotalAmount.Text = RebateAmount.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    RebateAmount = 0;
                    txtTotalAmount.Text = "0.00";
                    teAmount.Text = "0.00";
                }
            }


        }
        #endregion
        decimal RebateAmount { get; set; }
        private void chkPercent_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPercent.Checked)
            {
                lcTotalAmount.Visibility = LayoutVisibility.Always;
                lcPercentmamount.Visibility = LayoutVisibility.Always;
                lciAmount.Visibility = LayoutVisibility.Never;
                teAmount.Enabled = false;
                teAmount.Text = RebateAmount.ToString(CultureInfo.InvariantCulture);
                txtTotalAmount.Text = RebateAmount.ToString(CultureInfo.InvariantCulture);
                txtPercent.EditValue = null;
                txtPercent.EditValue = "0";

            }
            else
            {
                lcTotalAmount.Visibility = LayoutVisibility.Never;
                lcPercentmamount.Visibility = LayoutVisibility.Never;
                lciAmount.Visibility = LayoutVisibility.Always;
                teAmount.Enabled = true;

                txtPercent.EditValue = null;
                txtPercent.EditValue = "0";
                teAmount.Text = RebateAmount.ToString(CultureInfo.InvariantCulture);
                txtTotalAmount.Text = RebateAmount.ToString(CultureInfo.InvariantCulture);
            }
        }


        private void txtPercent_EditValueChanging(object sender, ChangingEventArgs e)
        {
            Decimal newValue;
            if (e.NewValue != null)
            {
                if (Decimal.TryParse(e.NewValue.ToString(), out newValue))
                {
                    if (newValue < 0 || newValue > 100)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        decimal value = RebateAmount * Convert.ToDecimal(newValue / 100);
                        teAmount.Text = value.ToString();
                    }
                }
                else
                {
                    decimal value = RebateAmount * Convert.ToDecimal(newValue / 100);
                    teAmount.Text = value.ToString();
                }
            }
            else
            {
                teAmount.Text = "0";
            }
        }
        frmPaymentMethods frmMobilePayments { get; set; }
        private void btnMobilePayment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //POSSettingCache.voucherDefinationCode = CNETConstantes.CASHRECIPT;
            //if (cacCustomer.EditValue != null)
            //{
            //    POSSettingCache.SelectedCustomer = new CustomerDTO();
            //    POSSettingCache.SelectedCustomer.Code = cacCustomer.EditValue.ToString();
            //    POSSettingCache.SelectedCustomer.Name = cacCustomer.Text;
            //}

            //if (frmMobilePayments == null)
            //    frmMobilePayments = new frmPaymentMethods();

            //if (frmMobilePayments.Check_Mobile_Payment(1, "CRV-23030-22", POSSettingCache.SelectedCustomer != null ? POSSettingCache.SelectedCustomer.Code : null))
            //{
            //    frmMobilePayments.ShowDialog(this);
            //    if (frmMobilePayments.payment_Successful)
            //    {
            //        NonCashTransaction NonCashTransaction = frmMobilePayments.Non_Cash_Transaction;
            //        if (POSSettingCache.SelectedCustomer != null)
            //        {
            //        }
            //    }
            //}
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
                e.DisplayText = (e.RowHandle + 1).ToString();
        }
    }
}