////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	POSPart\frmNonCashPaymentOption.cs
//Developers: Minale Mekanehiwot
// summary:	non cash payment option class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using CNET.ERP.Client.Common.UI;
using CNET.VoucherControl;
//using CNETIconAndImagePovider;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using CNET.FrontOffice_V._7;
using DevExpress.XtraGrid.Columns;
using CNET.FrontOffice_V._7;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET.FrontOffice_V._7.APICommunication;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET.FrontOffice_V._7.Validation;

namespace CNET.VoucherControl
{
    /// <summary>   Non cash payment option. </summary>
    public partial class frmNonCashPaymentOption : UILogicBase
    {

        private bool _isCreditCard = false;

        private NonCashVM _nonCashVM = null;
        public DateTime CurrentTime { get; set; }

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

        private NonCashTransactionDTO _savedNonCashTrans;
        public NonCashTransactionDTO SavedNonCashTransaction
        {
            get
            {
                return _savedNonCashTrans;

            }
            set
            {
                _savedNonCashTrans = value;
            }
        }


        public RegistrationListVMDTO RegExt { get; set; }



        ///  Constructor. 
        public frmNonCashPaymentOption()
        {
            //   this._type = type;
            InitializeComponent();

            InitializeUI();

        }



        #region Helper Methods

        private void InitializeUI()
        {
            txtReceiveNumber.Focus();

            GridColumn columnCompany = cmbReceivedFrom.Properties.View.Columns.AddField("Id");
            columnCompany.Visible = true;
            columnCompany = cmbReceivedFrom.Properties.View.Columns.AddField("name");
            columnCompany.Visible = true;
            columnCompany = cmbReceivedFrom.Properties.View.Columns.AddField("idNumber");
            columnCompany.Visible = true;
            cmbReceivedFrom.Properties.DisplayMember = "name";
            cmbReceivedFrom.Properties.ValueMember = "Id";

            cmbReceiveBank.Properties.DisplayMember = "Name";
            cmbReceiveBank.Properties.ValueMember = "Id";
            cmbReceiveBank.Properties.TreeList.ParentFieldName = "OrganizationUnitDefinitionParent";
            cmbReceiveBank.Properties.TreeList.KeyFieldName = "OrganizationUnitDefinitionCode";


            //Credit Types
            cacCreditCardType.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            cacCreditCardType.Properties.DisplayMember = "Description";
            cacCreditCardType.Properties.ValueMember = "Id";

            layoutControlItem13.Visibility = LayoutVisibility.Never;
            layoutControlItem14.Visibility = LayoutVisibility.Never;
        }

        private bool InitializeData()
        {
            try
            {
                if (RegExt == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get selected Registration!", "ERROR");
                    return false;
                }

                DateTime? currentTime = UIProcessManager.GetServiceTime();
                if (currentTime == null) return false;
                CurrentTime = currentTime.Value;

                dtReciveMaturity.Properties.MinValue = CurrentTime;
                dtReceivedDate.Properties.MinValue = CurrentTime;
                dtReciveMaturity.DateTime = CurrentTime.Date;

                if (NonCashVM == null)
                {
                    SystemMessage.ShowModalInfoMessage("Unable to get non-cash payment info!", "ERROR");
                    return false;
                }

                teReferenceNum.Text = NonCashVM.ReferenceNum;
                tePaymentMethod.Text = NonCashVM.PaymentTypeDesc;
                teCurrency.Text = NonCashVM.CurrencyDesc;
                teReceivedAmount.Text = NonCashVM.ReceivedAmount;

                dtReceivedDate.DateTime = NonCashVM.ReceivedDate;

                // Progress_Reporter.Show_Progress("Initializing Data", "Please Wait...");

                if (NonCashVM.ConsigneeList != null)
                {
                    cmbReceivedFrom.Properties.DataSource = NonCashVM.ConsigneeList.GroupBy(x => x.Id).Select(x => x.First()).ToList();
                    cmbReceivedFrom.Refresh();

                }
                cmbReceivedFrom.EditValue = NonCashVM.SelectedConsignee;

                if (NonCashVM.PaymentTypeCode != CNETConstantes.PAYMNET_METHOD_CREDITCARD)
                {


                    List<BankInformation> bankInformationList = new List<BankInformation>();
                    List<VwConsigneeViewDTO> listOfOrganizationUnitDefinition = LocalBuffer.ConsigneeViewBufferList.Where(x => x.GslType == CNETConstantes.BANK).ToList();

                    BankInformation bank = null;
                    if (listOfOrganizationUnitDefinition != null)
                    {
                        string orgUnitDefinitionCode = "";
                        List<ConsigneeUnitDTO> organizationUnitList = null;


                        foreach (var pov in listOfOrganizationUnitDefinition)
                        {
                            BankInformation parentBank = new BankInformation();
                            parentBank.OrganizationUnitDefinitionCode = pov.Id;
                            parentBank.OrganizationUnitDefinitionParent = null;
                            parentBank.FirstName = pov.FirstName;
                            parentBank.Id = pov.Id;
                            bankInformationList.Add(parentBank);

                            organizationUnitList = UIProcessManager.GetConsigneeUnitByconsignee(pov.Id).ToList();

                            if (organizationUnitList != null && organizationUnitList.Count > 0)
                            {
                                foreach (ConsigneeUnitDTO organizationUnitDefinition in organizationUnitList)
                                {
                                    bank = new BankInformation();
                                    bank.FirstName = organizationUnitDefinition.Name;
                                    bank.Id = organizationUnitDefinition.Id;
                                    if (organizationUnitDefinition.ParentId != null)
                                    {
                                        bank.OrganizationUnitDefinitionParent = parentBank.OrganizationUnitDefinitionCode;
                                    }
                                    else
                                    {
                                        bank.OrganizationUnitDefinitionParent = organizationUnitDefinition.ParentId;
                                    }
                                    bank.OrganizationUnitDefinitionCode = organizationUnitDefinition.Id;
                                    bankInformationList.Add(bank);
                                }
                            }

                        }
                        bankInformationList.Add(new BankInformation() { Code = "", FirstName = "", OrganizationUnitDefinitionParent = null, OrganizationUnitDefinitionCode = null });
                    }

                    cmbReceiveBank.Properties.TreeList.DataSource = bankInformationList;

                }
                else
                {
                    //for credit card display credit card no. and credit card type

                    layoutControlItem11.Visibility = LayoutVisibility.Never;
                    layoutControlItem13.Visibility = LayoutVisibility.Always;
                    layoutControlItem14.Visibility = LayoutVisibility.Always;

                    txtReceiveNumber.Enabled = false;


                    _isCreditCard = true;
                    cacCreditCardType.Enabled = false;
                    dtReciveMaturity.Enabled = false;

                    // PopulateCreditCardInfo();


                }

                // CNETInfoReporter.Hide();
                return true;
            }
            catch (Exception ex)
            {
                // CNETInfoReporter.Hide();
                SystemMessage.ShowModalInfoMessage("Error in initializing form! Detail: " + ex.Message, "ERROR");
                return false;
            }
        }

        private bool IsIncorrectNodeSelected(TreeListLookUpEdit tlLookUpEdit)
        {
            TreeList tl = tlLookUpEdit.Properties.TreeList;

            TreeListNode focusedNode = tl.FocusedNode;
            if (focusedNode.HasChildren)
            {
                return true;
            }
            else
                return false;


        }

        private void PopulateCreditCardInfo(NonCashTransactionDTO nonCashTran)
        {
            List<SystemConstantDTO> creditCardTypes = LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.CREDIT_CARD_TYPES && l.IsActive).ToList();
            cacCreditCardType.Properties.DataSource = creditCardTypes;

            //get non-cash transaction
            //   var _nonCashTran = UIProcessManager.GetNonCashTransaction(new Voucher() { code = RegExt.Registration }).FirstOrDefault();
            dtReciveMaturity.DateTime = nonCashTran.MaturityDate.Value;
            txtReceiveNumber.Text = nonCashTran.Number;
            teReceivedAmount.Text = nonCashTran.Amount.ToString();
            if (creditCardTypes != null)
            {
                var savedCreditType = creditCardTypes.FirstOrDefault(l => l.Code == nonCashTran.PaymentMethod);
                if (savedCreditType != null)
                {
                    cacCreditCardType.EditValue = savedCreditType.Id;
                }
            }


        }


        #endregion

        #region Event Handlers

        private void bbiClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show("Do you want to abort saving non-cash transaction detail?", "CNET_v2016", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnOk_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Progress_Reporter.Show_Progress("Saving non-cash transaction..");
            try
            {
                List<Control> controls = new List<Control>()
                {
                    txtReceiveNumber,
                    tePaymentMethod,
                    teCurrency,
                    cmbReceivedFrom,
                    teReferenceNum
                };

                if (!_isCreditCard)
                {
                    controls.Add(cmbReceiveBank);
                }

                IList<Control> invalidControls = CustomValidationRule.Validate(controls);

                if (invalidControls.Count > 0)
                {
                    // CNETInfoReporter.Hide();
                    return;

                }

                if (!_isCreditCard && cmbReceiveBank.EditValue == null)
                {
                    // CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Please select bank first !", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_isCreditCard && cacCreditCardType.EditValue == null)
                {
                    // CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Please select Credit Card Type first !", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if (!string.IsNullOrEmpty(txtReceiveNumber.Text))
                {
                    if (NonCashVM.PaymentTypeCode == CNETConstantes.PAYMNET_METHOD_CHECK)
                    {
                        List<NonCashTransactionDTO> ncashTrxnlist = UIProcessManager.GetNonCashTransactionByPaymentMethodandNumber(CNETConstantes.PAYMNET_METHOD_CHECK, txtReceiveNumber.Text);
                        if (ncashTrxnlist != null)
                        {
                            // CNETInfoReporter.Hide();
                            XtraMessageBox.Show("You can't enter duplicate check number !", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }


                }
                else
                {
                    // CNETInfoReporter.Hide();
                    XtraMessageBox.Show("Please set number first !", "CNET_V2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }



                NonCashTransactionDTO nonCashTrans = new NonCashTransactionDTO();
                nonCashTrans.Amount = Convert.ToDecimal(teReceivedAmount.Text);
                nonCashTrans.Consignee = Convert.ToInt32(cmbReceivedFrom.EditValue);
                nonCashTrans.Currency = NonCashVM.CurrencyCode;
                nonCashTrans.IssueDate = dtReceivedDate.DateTime;
                nonCashTrans.MaturityDate = dtReciveMaturity.DateTime;
                nonCashTrans.Number = txtReceiveNumber.Text;
                nonCashTrans.PaymentMethod = NonCashVM.PaymentTypeCode;
                nonCashTrans.Remark = txtReceivedRemark.Text;
                nonCashTrans.IsIncoming = true;
                nonCashTrans.PaymentProcessor = _isCreditCard ? Convert.ToInt32(cacCreditCardType.EditValue) : Convert.ToInt32(cmbReceiveBank.EditValue);
                nonCashTrans.Rate = NonCashVM.CurrencyRate;
                nonCashTrans.Executed = true;

                SavedNonCashTransaction = nonCashTrans;

                DialogResult = DialogResult.OK;
                this.Close();


                // CNETInfoReporter.Hide();

            }
            catch (Exception ex)
            {
                // CNETInfoReporter.Hide();
                XtraMessageBox.Show("Error in saving non-cash transaction. DETAIL:: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void cmbReceiveBank_QueryCloseUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel = IsIncorrectNodeSelected(sender as TreeListLookUpEdit))
                cmbReceiveBank.EditValue = "";
        }


        private void frmNonCashPaymentOption_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }


        #endregion





        private class BankInformation : VwConsigneeViewDTO
        {
            private int? organizationUnitDefinitionParent;
            private int? organizationUnitDefinitionCode;

            public int? OrganizationUnitDefinitionParent
            {
                get { return organizationUnitDefinitionParent; }
                set { organizationUnitDefinitionParent = value; }
            }

            public int? OrganizationUnitDefinitionCode
            {
                get { return organizationUnitDefinitionCode; }
                set { organizationUnitDefinitionCode = value; }
            }
        }


        private void beAddPaymentOption_Click(object sender, EventArgs e)
        {
            frmPaymentOptions frmPaymentOptions = new frmPaymentOptions();
            frmPaymentOptions.RegExt = RegExt;
            frmPaymentOptions.NonCashVM = NonCashVM;

            if (frmPaymentOptions.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtReceiveNumber.Text = "";
                cacCreditCardType.EditValue = null;
                PopulateCreditCardInfo(frmPaymentOptions.SavedNonCashTransaction);
            }
        }






    }

}


