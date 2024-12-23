using CNET.FrontOffice_V._7;
using ProcessManager;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using System.Reflection.Metadata.Ecma335;

namespace ERP.EventManagement
{
    public partial class frmNonCashPayments : XtraForm
    {
        public class NonCashPayment
        {
            public int payment_Processor { get; set; }
            public int payment_Method { get; set; }
            public bool is_Incoming { get; set; }
            public DateTime payment_Date { get; set; }
            public DateTime maturity_Date { get; set; }
            public string payment_Reference { get; set; }
            public decimal payment_Amount { get; set; }
            public bool executed { get; set; }
            public int currency { get; set; }
        }
        public string Credit_CRSNo { get; set; }
        public string Credit_ApprovalCode { get; set; }
        public string Credit_ApprovalAmt { get; set; }


        public NonCashPayment Non_Cash_Payment { get; set; }
        private int? payment_Consignee_Unit;
        public frmNonCashPayments()
        {
            InitializeComponent();
            sleCurrency.Properties.DataSource = LocalBuffer.LocalBuffer.CurrencyBufferList;
        }

        public void Show_Non_Cash_Payment_Form(string voucher_Code, decimal grand_Total, string consignee_Name, DateTime receieved_Date, int payment_Method)
        {
            try
            {
                Non_Cash_Payment = null;
                payment_Consignee_Unit = null;
                var payment_Const = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == payment_Method);
                if (payment_Const != null)
                {
                    txtPaymentMethod.Text = payment_Const.Description;
                    //sleBank.Properties.DataSource = UIProcessManager.GetConsigneeViewByGslType(payment_Method);
                    //payment_Consignee_Unit = payment_Const.ParentId;
                }
                //sleBank.Properties.DataSource = UIProcessManager.GetConsigneeViewByGslType(CNETConstantes.BANK);



                if (payment_Method == CNETConstantes.PAYMNET_METHOD_CREDITCARD)
                {

                    //var CardList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(x => x.Category == CNETConstantes.CREDIT_CARD_TYPES);

                    //cacCreditCardType.Properties.DataSource = CardList;

                    //cacCreditCardType.Properties.DisplayMember = "Description";
                    //cacCreditCardType.Properties.ValueMember = "Id";


                    ////lblApprovalAmt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    ////lblApprovalcode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    ////lblCRSNo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    //lblCreditCard.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    lblBank.Text = "Credit Card";
                    lblMaturityDate.Text = "Expiry Date";
                    lblNumber.Text = "Credit Card Number";
                }

                GetBankInformation(payment_Method);
                cmbReceiveBank.Properties.DisplayMember = "Name";
                cmbReceiveBank.Properties.ValueMember = "Id";
                cmbReceiveBank.Properties.TreeList.ParentFieldName = "OrganizationUnitDefinitionParent";
                cmbReceiveBank.Properties.TreeList.KeyFieldName = "OrganizationUnitDefinitionId";
                cmbReceiveBank.Properties.TreeList.DataSource = BankInformationlist;
                cmbReceiveBank.Properties.TreeList.ExpandAll();

                payment_Consignee_Unit = payment_Const.Id;
                txtReference.Text = voucher_Code;
                txtConsignee.Text = consignee_Name;
                txtAmount.Text = grand_Total.ToString();
                deRecieved.EditValue = receieved_Date;
                deExpiryDate.EditValue = receieved_Date;

                Reset_Control();

                this.ShowDialog();

            }
            catch { }
        }
        private void Reset_Control()
        {
            try
            {
                if (LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(x => x.IsDefault) != null)
                    sleCurrency.EditValue = LocalBuffer.LocalBuffer.CurrencyBufferList.FirstOrDefault(x => x.IsDefault).Id;
                else
                    sleCurrency.EditValue = null;
                cmbReceiveBank.EditValue = null;
                deExpiryDate.EditValue = deRecieved.EditValue;
                txtNumber.Text = "";
                txtRemark.Text = "";
            }
            catch { }
        }
        private bool Validate()
        {
            try
            {
                var validation_Status = 0;
                dxErrorProvider1.ClearErrors();

                if (payment_Consignee_Unit == CNETConstantes.PAYMNET_METHOD_CREDITCARD)
                {
                    if (cmbReceiveBank.EditValue == null || string.IsNullOrEmpty(cmbReceiveBank.EditValue.ToString()))
                    {
                        dxErrorProvider1.SetError(cmbReceiveBank, "Please Select Credit Card Type", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                        validation_Status = 1;
                    }
                }
                else
                {
                    if (cmbReceiveBank.EditValue == null || string.IsNullOrEmpty(cmbReceiveBank.EditValue.ToString()))
                    {
                        dxErrorProvider1.SetError(cmbReceiveBank, "Please Select Bank Branch", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                        validation_Status = 1;
                    }
                }

                if (string.IsNullOrEmpty(txtNumber.Text))
                {
                    dxErrorProvider1.SetError(txtNumber, "Please Enter Number", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                    validation_Status = 1;
                }
                if (sleCurrency.EditValue == null || string.IsNullOrEmpty(sleCurrency.EditValue.ToString()))
                {
                    dxErrorProvider1.SetError(sleCurrency, "Please Select Currency", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                    validation_Status = 1;
                }
                if (deExpiryDate.EditValue == null)
                {
                    dxErrorProvider1.SetError(deExpiryDate, "Please Select Expiry Date", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                    validation_Status = 1;
                }
                else
                {
                    if (Convert.ToDateTime(deRecieved.EditValue) > Convert.ToDateTime(deExpiryDate.EditValue))
                    {
                        dxErrorProvider1.SetError(deExpiryDate, "Please Select Correct Expiry Date", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
                        validation_Status = 1;
                    }
                }
                return validation_Status == 0;
            }
            catch { return false; }
        }

        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Reset_Control();
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Validate())
            {
                Non_Cash_Payment = new NonCashPayment
                {
                    payment_Method = payment_Consignee_Unit.Value,
                    is_Incoming = true,
                    payment_Date = Convert.ToDateTime(deRecieved.EditValue),
                    maturity_Date = Convert.ToDateTime(deExpiryDate.EditValue),
                    payment_Reference = txtNumber.Text,
                    payment_Amount = Convert.ToDecimal(txtAmount.Text),
                    executed = true,
                    currency = (int)sleCurrency.EditValue,
                    payment_Processor = (int)cmbReceiveBank.EditValue,
                };


                if (!string.IsNullOrEmpty(teCRSNo.Text))
                    Credit_CRSNo = teCRSNo.Text;
                else
                    Credit_CRSNo = null;


                if (!string.IsNullOrEmpty(teApprovalCode.Text))
                    Credit_ApprovalCode = teApprovalCode.Text;
                else
                    Credit_ApprovalCode = null;


                if (!string.IsNullOrEmpty(teApprovalAmt.Text))
                    Credit_ApprovalAmt = teApprovalAmt.Text;
                else
                    Credit_ApprovalAmt = null;


                this.Close();
            }
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Non_Cash_Payment = null;
            this.Close();
        }

        private bool IsIncorrectNodeSelected(TreeListLookUpEdit tlLookUpEdit)
        {
            TreeList tl = tlLookUpEdit.Properties.TreeList;

            TreeListNode focusedNode = tl.FocusedNode;

            if (focusedNode == null)
                return false;

            if (focusedNode.HasChildren)
            {
                return true;
            }
            else
                return false;


        }
        List<BankInformation> BankInformationlist { get; set; }

        public List<BankInformation> GetBankInformation(int payment_Method)
        {
            BankInformationlist = new List<BankInformation>();

            var Bankdata = UIProcessManager.GetConsigneeViewByGslType(CNETConstantes.BANK);

            if (Bankdata != null && Bankdata.Count > 0)
            {

                if (payment_Method == CNETConstantes.PAYMNET_METHOD_CREDITCARD)
                    Bankdata = Bankdata.Where(x => x.FirstName == "Bank Credit Card").ToList();
                else
                    Bankdata = Bankdata.Where(x => x.FirstName != "Bank Credit Card").ToList();


                if (Bankdata != null && Bankdata.Count > 0)
                {

                    BankInformation bank = null;
                    foreach (var pov in Bankdata)
                    {
                        BankInformation parentBank = new BankInformation();
                        parentBank.OrganizationUnitDefinitionId = pov.Id;
                        parentBank.OrganizationUnitDefinitionParent = null;
                        parentBank.Name = pov.FirstName;
                        parentBank.Id = pov.Id;
                        BankInformationlist.Add(parentBank);

                        var BankBranchlist = UIProcessManager.GetConsigneeUnitByconsigneeandtype(pov.Id, CNETConstantes.Org_Unit_Type_Branch);

                        if (BankBranchlist != null && BankBranchlist.Count > 0)
                        {
                            foreach (var ou in BankBranchlist)
                            {
                                bank = new BankInformation();
                                bank.Name = ou.Name;
                                if (ou.ParentId == null)
                                {
                                    bank.OrganizationUnitDefinitionParent = parentBank.Id;
                                }
                                else
                                {
                                    bank.OrganizationUnitDefinitionParent = ou.ParentId.Value;
                                }
                                bank.OrganizationUnitDefinitionId = ou.Id;
                                bank.Id = ou.Id;
                                BankInformationlist.Add(bank);
                            }
                        }
                    }
                }
            }
            return BankInformationlist;
        }

        private void cmbReceiveBank_QueryCloseUp(object sender, CancelEventArgs e)
        {
            if (e.Cancel = IsIncorrectNodeSelected(sender as TreeListLookUpEdit))
                cmbReceiveBank.EditValue = "";
        }
    }

    public class BankInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private int? organizationUnitDefinitionParent;

        private int organizationUnitDefinitionId;

        public int? OrganizationUnitDefinitionParent
        {
            get { return organizationUnitDefinitionParent; }
            set { organizationUnitDefinitionParent = value; }
        }

        public int OrganizationUnitDefinitionId
        {
            get { return organizationUnitDefinitionId; }
            set { organizationUnitDefinitionId = value; }
        }
    }
}
