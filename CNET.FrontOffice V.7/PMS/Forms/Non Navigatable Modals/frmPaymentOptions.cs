using CNET.ERP.Client.Common.UI;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmPaymentOptions : UILogicBase
    {

        private int _defPaymentMethod;
        private int _defCreditCardType;
        private int _defCurrency;
        List<SystemConstantDTO> creditCardTypes { get; set; }
        //private RegistrationExtension registrationExt = null;
        private ActivityDefinitionDTO workflow = null;

        private NonCashVM _nonCashVM = null;
        public NonCashVM NonCashVM
        {
            get
            {
                return _nonCashVM;

            }
            set
            {
                _nonCashVM = value;

            }
        }

        //  private NonCashTransaction _nonCashTran = null;

        /** Properties **/
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

        //******************* CONSTRUCTOR ********************//
        public frmPaymentOptions()
        {
            InitializeComponent();

            InitializeUI();
        }

        #region Helper Methods

        private void InitializeUI()
        {
            //Payment Method
            cacPayment.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            cacPayment.Properties.DisplayMember = "Description";
            cacPayment.Properties.ValueMember = "Id";

            //Credit Types
            cacCreditTypes.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            cacCreditTypes.Properties.DisplayMember = "Description";
            cacCreditTypes.Properties.ValueMember = "Id";

            //Currency
            lukCurrency.Properties.Columns.Add(new LookUpColumnInfo("Description", "Name"));
            lukCurrency.Properties.DisplayMember = "Description";
            lukCurrency.Properties.ValueMember = "Id";

            lcgCreditCardDetail.Visibility = LayoutVisibility.Never;
        }

        private bool InitializeData()
        {
            try
            {
                if (RegExt == null)
                {
                    SystemMessage.ShowModalInfoMessage("Please Select a Registration!", "ERROR");
                    return false;
                }
                CreateCommisionArticle();
                if (currentTime != null)
                {
                    deExpDate.DateTime = currentTime.Value;
                }
                teGuest.Text = RegExt.Guest;
                teRegNum.Text = RegExt.Registration;
                teRoom.Text = RegExt.Room;

                //// Progress_Reporter.Show_Progress("Initialize Data...", "Please Wait...");

                //Payment Methods
                List<SystemConstantDTO> paymentList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Id == CNETConstantes.PAYMENTMETHODSCASH ).ToList();
                if (paymentList != null)
                {
                    paymentList = paymentList.Where(
                            r =>
                                r.Id == CNETConstantes.PAYMENTMETHODSCASH ||
                                r.Id == CNETConstantes.PAYMNET_METHOD_CREDITCARD).ToList();
                    cacPayment.Properties.DataSource = (paymentList.OrderByDescending(c => c.IsDefault).ToList());
                    //  var paymentDefault = paymentList.FirstOrDefault(c => c.isDefault);
                    //  if (paymentDefault != null)
                    // {
                    //     cacPayment.EditValue = (paymentDefault.code);
                    //    _defPaymentMethod = paymentDefault.code;
                    //  }

                    if (_nonCashVM != null)
                    {
                        txtAmount.Text = _nonCashVM.ReceivedAmount.ToString();
                        cacPayment.EditValue = _nonCashVM.PaymentTypeCode;
                        _defPaymentMethod = _nonCashVM.PaymentTypeCode.Value;
                        cacPayment.Properties.ReadOnly = true;
                    }
                }

                //Credit Card Types

                creditCardTypes = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.CREDIT_CARD_TYPES && l.IsActive).ToList();
                if (creditCardTypes != null)
                {
                    cacCreditTypes.Properties.DataSource = creditCardTypes;
                    var creditTypeDef = creditCardTypes.FirstOrDefault(l => l.IsDefault);
                    if (creditTypeDef != null)
                    {
                        cacCreditTypes.EditValue = (creditTypeDef.Id);
                        _defCreditCardType = creditTypeDef.Id;
                    }
                }

                //Check Workflow
                workflow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.CASH_SALES).FirstOrDefault();

                if (workflow == null)
                {
                    ////CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Please Define Workflow of PREPARED for Credit Note", "Payment Options", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }


                //currency
                List<CurrencyDTO> currencyList = LocalBuffer.LocalBuffer.CurrencyBufferList;
                lukCurrency.Properties.DataSource = currencyList;
                if (currencyList != null)
                {
                    var defCurrency = currencyList.FirstOrDefault(c => c.IsDefault == true);
                    if (defCurrency != null)
                    {
                        lukCurrency.EditValue = defCurrency.Id;
                        _defCurrency = defCurrency.Id;

                    }
                }



                //get voucher extension transactions
                //VoucherDTO voExtTranList = UIProcessManager.GetVoucherExtTransViewListByVoucher(RegExt.Registration);
                //if (voExtTranList != null && voExtTranList.Count > 0)
                //{
                //    foreach (var vot in voExtTranList)
                //    {
                //        var voExtCRSNo  = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(vx => vx.type == CNETConstantes.VOUCHEREXTENSIONDEFINITIONCRSNO && vx.voucherDefn == CNETConstantes.REGISTRATION_VOUCHER);
                //        var voExtApprovalCode = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(vx => vx.type == CNETConstantes.VOUCHEREXTENSIONDEFINITIONAPPROVALCODE && vx.voucherDefn == CNETConstantes.REGISTRATION_VOUCHER);
                //        var voExtApprovalAmt = LocalBuffer.LocalBuffer.VoucherExtensionBufferList.FirstOrDefault(vx => vx.type == CNETConstantes.VOUCHEREXTENSIONDEFINITIONAPPAMT && vx.voucherDefn == CNETConstantes.REGISTRATION_VOUCHER);

                //        if (voExtCRSNo != null && vot.voExtensionCode == voExtCRSNo.code)
                //        {
                //            teCRSNo.Text = vot.number;
                //        }

                //        if (voExtApprovalCode != null && vot.voExtensionCode == voExtApprovalCode.code)
                //        {
                //            teApprovalCode.Text = vot.number;

                //        }

                //        if (voExtApprovalAmt != null && vot.voExtensionCode == voExtApprovalAmt.code)
                //        {
                //            teApprovalAmt.Text = vot.number;
                //        }

                //    }
                //}

                //get non-cash transaction
                /* _nonCashTran = UIProcessManager.GetNonCashTransaction(new Voucher() { code = RegExt.Registration }).FirstOrDefault();
                 if (_nonCashTran != null)
                 {
                     deExpDate.EditValue = _nonCashTran.maturityDate;
                     teCreditCardNo.EditValue = _nonCashTran.number;

                     if (creditCardTypes != null)
                     {
                         var savedCreditType = creditCardTypes.FirstOrDefault(l => l.code == _nonCashTran.paymentProcesser);
                         if (savedCreditType != null)
                         {
                             cacCreditTypes.EditValue = savedCreditType.code;
                         }
                     }

                 }*/

                if (RegExt.Payment == CNETConstantes.PAYMNET_METHOD_CREDITCARD)
                {
                    cacPayment.EditValue = CNETConstantes.PAYMNET_METHOD_CREDITCARD;
                }

                ////CNETInfoReporter.Hide();

                return true;
            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in initializing form. DETAIL:: " + ex.Message, "Payment Options", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }


        private void CreateCommisionArticle()
        {
            CommisssionArticle = UIProcessManager.GetArticleByname("Commission Item");
            if (CommisssionArticle == null)
            {
                string itemToSave = UIProcessManager.IdGenerater("Article", CNETConstantes.ITEM, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);


                if (itemToSave != null)
                {
                    string ItemCode = itemToSave;
                    PreferenceDTO ItemPref = LocalBuffer.LocalBuffer.PreferenceBufferList.FirstOrDefault(x => x.SystemConstant == CNETConstantes.ITEM);
                    if (ItemPref == null)
                    {
                        ItemPref = new PreferenceDTO
                        {
                            SystemConstant = CNETConstantes.ITEM,
                            //Reference = CNETConstantes.ITEM,
                            Description = "Item",
                            Index = 0,
                            IsActive = true
                        };
                        PreferenceDTO savedPref = UIProcessManager.CreatePreference(ItemPref);
                        if (savedPref == null)
                        {
                            XtraMessageBox.Show("Failed to Create Category for Commision Item", "CNETERP_V6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            LocalBuffer.LocalBuffer.LoadPreference();
                            ItemPref.Id = savedPref.Id;
                        }
                    }
                    CommisssionArticle = new ArticleDTO
                    {
                        LocalCode = ItemCode,
                        GslType = CNETConstantes.ITEM,
                        Name = "Commission Item",
                        Preference = ItemPref.Id,
                        Uom = CNETConstantes.UNITOFMEASURMENTPCS,
                        IsActive = true


                    };
                    ArticleDTO saved = UIProcessManager.CreateArticle(CommisssionArticle);

                    CommisssionArticleTax = new GsltaxDTO
                    {
                        Reference = saved.Id,
                        Tax = CNETConstantes.VAT
                    };

                    UIProcessManager.CreateGSLTax(CommisssionArticleTax);


                    SystemConstantDTO ItemActive = LocalBuffer.LocalBuffer.ObjectStateDefinitionBufferList.FirstOrDefault(x => x.Category == "Article");

                    ObjectStateDTO CommisssionArticleState = new ObjectStateDTO
                    {
                        Reference = saved.Id,
                        ObjectStateDefinition = ItemActive.Id
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



        #endregion

        #region Event Handlers

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (cacPayment.EditValue == null || string.IsNullOrEmpty(cacPayment.EditValue.ToString()))
                {
                    SystemMessage.ShowModalInfoMessage("Please Choose Payment Type!", "ERROR");
                    return;
                }

                if (lukCurrency.EditValue == null || string.IsNullOrEmpty(lukCurrency.EditValue.ToString()))
                {
                    SystemMessage.ShowModalInfoMessage("Please Choose Currency!", "ERROR");
                    return;
                }

                // Progress_Reporter.Show_Progress("Saving Payment Options", "Please Wait...");

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null)
                {
                    ////CNETInfoReporter.Hide();
                    return;
                }

                if (Convert.ToInt32(cacPayment.EditValue) == CNETConstantes.PAYMNET_METHOD_CREDITCARD)
                {

                    if (string.IsNullOrEmpty(teCreditCardNo.Text))
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Credit Card Number is required!", "ERROR");
                        return;
                    }
                    VoucherDTO voucherDTO = UIProcessManager.GetVoucherById(RegExt.Id);


                    voucherDTO.PaymentMethod = Convert.ToInt32(cacPayment.EditValue);
                    if (UIProcessManager.UpdateVoucher(voucherDTO) == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Payment Option is not saved!", "ERROR");
                        return;
                    }
                    if (!string.IsNullOrEmpty(txtAmount.Text))
                    {
                        NonCashVM.ReceivedAmount = txtAmount.Text;
                    }
                    else
                    {
                        txtAmount.Text = "0";
                    } 

                  //  bool isNonCashTranSaved = true;
                    // if (_nonCashTran != null)
                    // {
                    //  _nonCashTran.paymentProcesser = cacCreditTypes.EditValue == null ? "" : cacCreditTypes.EditValue.ToString();
                    //  _nonCashTran.maturityDate = deExpDate.DateTime;
                    // _nonCashTran.number = teCreditCardNo.Text;
                    // isNonCashTranSaved = UIProcessManager.UpdateNonCashTransaction(_nonCashTran);
                    //  }
                    //  else
                    //{
                    //NonCashTransactionDTO nonCahTrans = new NonCashTransactionDTO();
                    //nonCahTrans.Voucher = RegExt.Id;
                    //nonCahTrans.Consignee = RegExt.GuestId;
                    //nonCahTrans.PaymentMethod = Convert.ToInt32(cacPayment.EditValue);
                    //nonCahTrans.PaymentProcessor = cacCreditTypes.EditValue == null ? null : Convert.ToInt32(cacCreditTypes.EditValue);
                    //nonCahTrans.Index = 1;
                    //nonCahTrans.IssueDate = currentTime.Value;
                    //nonCahTrans.MaturityDate = deExpDate.EditValue != null ? Convert.ToDateTime(deExpDate.EditValue) : currentTime;
                    //nonCahTrans.Number = teCreditCardNo.Text;
                    //nonCahTrans.Currency = lukCurrency.EditValue == null ? null : Convert.ToInt32(lukCurrency.EditValue);
                    //nonCahTrans.Amount = Convert.ToDecimal(txtAmount.Text);
                    //nonCahTrans.Executed = true;
                    //// isNonCashTranSaved = UIProcessManager.CreateNonCashTransaction(nonCahTrans);

                    //// if (isNonCashTranSaved)
                    //// {
                    //SavedNonCashTransaction = nonCahTrans;
                    // }
                    // }

                    //delete all voucher extension transactions
                    // UIProcessManager.DeleteVoucherExtensionTransaction(new Voucher() { code = RegExt.Registration });


                    //save voucher extension and voucher extension transactions
                    //if (!string.IsNullOrEmpty(teCRSNo.Text))
                    //{
                    //    bool flag = CommonLogics.SaveVoucherExtensionWithTrans(CNETConstantes.REGISTRATION_VOUCHER,
                    //        CNETConstantes.VOUCHEREXTENSIONDEFINITIONCRSNO, RegExt.Registration, teCRSNo.Text);
                    //}

                    //if (!string.IsNullOrEmpty(teApprovalCode.Text))
                    //{
                    //    bool flag = CommonLogics.SaveVoucherExtensionWithTrans(CNETConstantes.REGISTRATION_VOUCHER,
                    //        CNETConstantes.VOUCHEREXTENSIONDEFINITIONAPPROVALCODE, RegExt.Registration, teApprovalCode.Text);
                    //}

                    //if (!string.IsNullOrEmpty(teApprovalAmt.Text))
                    //{
                    //    bool flag = CommonLogics.SaveVoucherExtensionWithTrans(CNETConstantes.REGISTRATION_VOUCHER,
                    //        CNETConstantes.VOUCHEREXTENSIONDEFINITIONAPPAMT, RegExt.Registration, teApprovalAmt.Text);
                    //}

                    // RegistrationList.SynchronizeRegistration(RegExt.Registration);
                    ////CNETInfoReporter.Hide();
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    XtraMessageBox.Show("Payment option is saved!", "Payment Options", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();

                }
                else
                {

                    VoucherDTO voucherDTO = UIProcessManager.GetVoucherById(RegExt.Id);
                    voucherDTO.PaymentMethod = Convert.ToInt32(cacPayment.EditValue);
                    if (UIProcessManager.UpdateVoucher(voucherDTO) == null)
                    {
                        ////CNETInfoReporter.Hide();
                        SystemMessage.ShowModalInfoMessage("Payment Option is not saved!", "ERROR");
                        return;
                    }
                    else
                    {
                        ////CNETInfoReporter.Hide();
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        XtraMessageBox.Show("Payment option is saved!", "Payment Options", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                }




            }
            catch (Exception ex)
            {
                ////CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in saving payment options. DETAIL:: " + ex.Message, "Payment Options", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        DateTime? currentTime = UIProcessManager.GetServiceTime();

        private void bbiReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            teApprovalAmt.Text = "";
            teApprovalCode.Text = "";
            teCRSNo.Text = "";
            teCreditCardNo.Text = "";
            if (currentTime != null)
            {
                deExpDate.DateTime = currentTime.Value;
            }

            cacCreditTypes.EditValue = _defCreditCardType;
            cacPayment.EditValue = _defPaymentMethod;
            lukCurrency.EditValue = _defCurrency;
        }

        private void cacPayment_EditValueChanged(object sender, EventArgs e)
        {
            int pay = Convert.ToInt32(cacPayment.EditValue);
            if (pay == CNETConstantes.PAYMENTMETHODSCASH || pay == CNETConstantes.PAYMENTMETHODS_DIRECT_BILL)
            {
                lcgCreditCardDetail.Visibility = LayoutVisibility.Never;

            }
            else
            {
                lcgCreditCardDetail.Visibility = LayoutVisibility.Always;
            }
        }

        private void frmPaymentOptions_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion

        private void cacCreditTypes_EditValueChanged(object sender, EventArgs e)
        {
            if (cacCreditTypes.EditValue != null && !string.IsNullOrEmpty(cacCreditTypes.EditValue.ToString()))
            {
                SystemConstantDTO SelectedcreditCard = creditCardTypes.FirstOrDefault(x => x.Id == Convert.ToInt32(cacCreditTypes.EditValue));
                if (SelectedcreditCard.Value != null && !string.IsNullOrEmpty(SelectedcreditCard.Value))
                {
                    decimal ComissionPercent = Convert.ToDecimal(SelectedcreditCard.Value);

                    decimal CommissionAmount = ((Convert.ToDecimal(_nonCashVM.ReceivedAmount) * ComissionPercent) / 100);

                    decimal AmountAfterComission = 0;
                    TaxDTO tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(x => x.Id == CommisssionArticleTax.Tax);
                    if (tax != null)
                    {

                        AmountAfterComission = (Convert.ToDecimal(_nonCashVM.ReceivedAmount) + ((1 + ((decimal)tax.Amount / 100)) * CommissionAmount));
                    }
                    else
                    {
                        AmountAfterComission = (Convert.ToDecimal(_nonCashVM.ReceivedAmount) + CommissionAmount);
                    }
                    txtAmount.Text = AmountAfterComission.ToString();

                }
                else
                {
                    XtraMessageBox.Show("Check Credit Card Value !!", "Payment Options", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            else
            {
                txtAmount.Text = _nonCashVM.ReceivedAmount.ToString();
            }
        }





        public ArticleDTO CommisssionArticle { get; set; }

        public GsltaxDTO CommisssionArticleTax { get; set; }
    }
}
