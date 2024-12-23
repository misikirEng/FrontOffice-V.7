using CNET.ERP.Client.Common.UI;
using CNET.ERP.Client.UI_Logic.PMS.Forms.CommonClass;
using CNET.FrontOffice_V._7;
using ProcessManager;
using CNET.FrontOffice_V._7.PMS.Common_Classes;
using CNET.Progress.Reporter;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using DevExpress.Mvvm.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNET_V7_Domain.Misc;
using DevExpress.ClipboardSource.SpreadsheetML;

namespace CNET.FrontOffice_V._7.Forms.Non_Navigatable_Modals
{
    public partial class frmSplitItem : DevExpress.XtraEditors.XtraForm
    {
        List<LineItemDTO> lineItems = null;
        List<VoucherBuffer> splittedVoucherList = null;
        private List<LineItemDetails> lineItemDetails;
        private VoucherBuffer retrivedVoucherBuffer;

        private string currentBasicActivityDefn;

        private int? adBreakVoucher = null;


        public VoucherDTO CurrentVoucher { get; set; }
        public RegistrationListVMDTO RegistrationExt { get; set; }

        private DateTime CurrentTime { get; set; }

        /****** constructor ****/
        public frmSplitItem()
        {
            InitializeComponent();

            InitializeUI();

        }

        #region Helper Methods

        private void InitializeUI()
        {
            //Split By
            lukSplitBy.Properties.Columns.Add(new LookUpColumnInfo("SplitBy", "Split By"));
            lukSplitBy.Properties.DisplayMember = "SplitBy";
            lukSplitBy.Properties.ValueMember = "Value";
        }

        private bool InitializeData()
        {
            try
            {
                if (CurrentVoucher == null) return false;
                if (RegistrationExt == null) return false;

                DateTime? cDate = UIProcessManager.GetServiceTime();
                if (cDate == null)
                {
                    return false;
                }
                CurrentTime = cDate.Value;

                Progress_Reporter.Show_Progress("Initializing Split Voucher ...", "Please Wait.......");

                //check workflow

                ActivityDefinitionDTO workFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_BREAK, CurrentVoucher.Definition).FirstOrDefault();

                if (workFlow != null)
                {

                    adBreakVoucher = workFlow.Id;
                }
                else
                {
                    Progress_Reporter.Close_Progress();
                    var voucDef = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(vd => vd.Id == CurrentVoucher.Definition);
                    string vouchDefName = voucDef == null ? "Current Voucher" : voucDef.Description;
                    SystemMessage.ShowModalInfoMessage("Please define workflow of BREAK for " + vouchDefName, "ERROR");
                    return false;
                }




                teDate.Text = CurrentVoucher.IssuedDate.ToShortDateString();
                teVoucherNo.Text = CurrentVoucher.Code;
                teCustomer.Text = RegistrationExt.Guest;
                teCurrentAmount.Text = Math.Round(Math.Abs(CurrentVoucher.GrandTotal), 2).ToString();
                if (CurrentVoucher.Definition == CNETConstantes.CREDIT_NOTE_VOUCHER)
                {
                    //net off
                    bool isNetoff = true;
                    if (CurrentVoucher.Remark == "netoff_off")
                    {
                        isNetoff = false;
                    }

                    if (isNetoff)
                    {

                        TaxDTO tax = CommonLogics.GetApplicableTax(RegistrationExt.Id, CNETConstantes.CREDIT_NOTE_VOUCHER, RegistrationExt.GuestId, null);
                        if (!string.IsNullOrEmpty(tax.Remark))
                        {
                            SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                            return false;
                        }
                        decimal taxRate = (decimal)tax.Amount;
                        decimal subtotal = Math.Round(Math.Abs(CurrentVoucher.GrandTotal) * ((taxRate / 100) + 1), 2);
                        CurrentVoucher.GrandTotal = subtotal;
                        teCurrentAmount.Text = Math.Round(subtotal, 2).ToString();
                    }
                }
                rgFactor.SelectedIndex = 0;


                //get lineItems
                lineItems = UIProcessManager.GetLineItemByvoucher(CurrentVoucher.Id);

                //read voucher setting
                PMSVoucherSetting.GetCurrentVoucherSetting(CurrentVoucher.Definition);
                //Voucher_UI _voucherUIExtra = new Voucher_UI(CurrentVoucher.voucherDefinition);
                //bool _isVoucherCreated = _voucherUIExtra.IsVoucherCreated();
                //if (!_isVoucherCreated)
                //{
                //    Progress_Reporter.Close_Progress();
                //    return false;
                //}
                //_currentSetting = _voucherUIExtra.Vouchersetting;

                //populate split-by lookup
                List<SplitByHolder> splitByHolder = new List<SplitByHolder>();
                if (lineItems != null && lineItems.Count > 0)
                {
                    splitByHolder.Add(new SplitByHolder()
                    {
                        SplitBy = "Quantitiy",
                        Value = 1
                    });
                }
                splitByHolder.Add(new SplitByHolder()
                {
                    SplitBy = "Amount",
                    Value = 2
                });
                lukSplitBy.Properties.DataSource = splitByHolder;
                lukSplitBy.EditValue = 2;


                Progress_Reporter.Close_Progress();
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error in initializing split voucher. DETAIL:: " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Progress_Reporter.Close_Progress();
                return false;
            }

        }


        private async Task<VoucherBuffer> PrepareVoucherBufferObj(int voucherCode)
        {
            retrivedVoucherBuffer = new VoucherBuffer();
            Progress_Reporter.Show_Progress("Selecting Voucher Object. . . ", "Please Wait.......");


            var response = UIProcessManager.GetVoucherBufferById(voucherCode);

            retrivedVoucherBuffer = response.Data;

            //#region LineItemObjBuffer
            //Progress_Reporter.Show_Progress("Loading LineItem(s). . . ", "Please Wait . . . ", 2, 20);
            //List<LineItem> lineItemsV = UIProcessManager.GetLineItem(currentVoucher).ToList();
            //int l = 1;
            //lineItemDetails = new List<LineItemDetails>();
            //foreach (var lin in lineItemsV)
            //{
            //    string step = "";
            //    if (l == 1)
            //    {
            //        step = "1st";
            //    }
            //    else if (l == 2)
            //    {
            //        step = "2nd";
            //    }
            //    else if (l == 3)
            //    {
            //        step = "3rd";
            //    }
            //    else if (l > 3)
            //    {
            //        step = l + "th";
            //    }
            //    Progress_Reporter.Show_Progress(String.Format("Loading {0} LineItem  Detail. . . ", step), "Please Wait . . .(" + lineItemsV.Count + " LineItems Found!)", l, lineItemsV.Count);
            //    NewLineitemObj lineitemBufferEdit = new NewLineitemObj();
            //    lineitemBufferEdit.lineitem = lin;
            //    lineitemBufferEdit.OriginalReferedPrice = lin.unitAmt.Value;
            //    lineitemBufferEdit.OriginalReferedQuantity = lin.quantity;
            //    lineitemBufferEdit.lineitemValueFactors = UIProcessManager.GetLineItemValueFactor(lin).ToList();
            //    lineitemBufferEdit.serialCodes = UIProcessManager.GetSerialCodeListByLineItem(lin.code).ToList();
            //    lineitemBufferEdit.lifespans = UIProcessManager.LifeSpanSelectByReference(lin.code).ToList();
            //    lineitemBufferEdit.lineitemReference = UIProcessManager.GetLineItemReference(lin).FirstOrDefault();
            //    lineitemBufferEdit.lineitemWeight = UIProcessManager.GetLineitemWeightByLineitem(lin.code).FirstOrDefault();
            //    lineitemBufferEdit.LineItemConversions = UIProcessManager.SelectLineitemConversionByLineitem(lin.code).ToList();
            //    lineitemBufferEdit.value = UIProcessManager.GetValueByArticle(lin.article).FirstOrDefault();
            //    lineitemBufferEdit.schedules = UIProcessManager.GetScheduleByReference(lin.code);
            //    lineitemBufferEdit.ArticleCode = lin.article;
            //    Article art = UIProcessManager.SelectArticle(lin.article);
            //    if (art != null)
            //    {
            //        lineitemBufferEdit.ArticleName = art.name;
            //        var pref = Authentication.PreferenceBufferList.FirstOrDefault(p => p.code == art.preference);
            //        if (pref != null)
            //        {
            //            lineitemBufferEdit.Catagory = pref.description;
            //        }
            //    }
            //    lineitemBufferEdit.UnitPrice = lin.unitAmt.Value;
            //    lineitemBufferEdit.TaxAmount = lin.taxAmount.Value;
            //    lineitemBufferEdit.TotalAmount = lin.totalAmount.Value;
            //    lineitemBufferEdit.Quantity = lin.quantity;
            //    LineItemDetails linDet = new LineItemDetails()
            //    {
            //        lineItems = lin,
            //        lineItemValueFactor = lineitemBufferEdit.lineitemValueFactors
            //    };
            //    lineItemDetails.Add(linDet);

            //    if (retrivedVoucherBuffer.newLineitemObjs == null)
            //        retrivedVoucherBuffer.newLineitemObjs = new List<NewLineitemObj>();
            //    retrivedVoucherBuffer.newLineitemObjs.Add(lineitemBufferEdit);
            //    l++;
            //}
            //#endregion

            //#region PopulateVoucherBufferObject
            //Progress_Reporter.Show_Progress("Populating LineItem Value Factor. . . ", "Please Wait . . . ", 3, 21);
            //retrivedVoucherBuffer.voucher = currentVoucher;
            //Progress_Reporter.Show_Progress("Loading and Populating Voucher Value. . . ", "Please Wait . . . ", 4, 21);
            //retrivedVoucherBuffer.voucherValue = UIProcessManager.GetVoucherValue(currentVoucher);
            //Progress_Reporter.Show_Progress("Loading and Populating Tax Transactions. . . ", "Please Wait . . . ", 5, 21);
            //retrivedVoucherBuffer.taxTransactions = UIProcessManager.GetTaxTransactionByVoucher(voucherCode).ToList();
            //Progress_Reporter.Show_Progress("Loading and Populating Transaction Reference. . . ", "Please Wait . . . ", 6, 21);
            //retrivedVoucherBuffer.transactionReferences = UIProcessManager.GetTransactionReferenceByVoucherCode(voucherCode);
            //Progress_Reporter.Show_Progress("Loading and Populating Store Transaction. . . ", "Please Wait . . . ", 7, 21);
            //retrivedVoucherBuffer.storeTransaction = UIProcessManager.GetStoreTransaction(currentVoucher).FirstOrDefault();
            //Progress_Reporter.Show_Progress("Loading and Populating Voucher Extension Transaction. . . ", "Please Wait . . . ", 8, 21);
            //retrivedVoucherBuffer.voucherExtensionTransactions = UIProcessManager.SelectAllVoucherExtensionTransaction().Where(y => y.voucher == voucherCode).ToList();
            //Progress_Reporter.Show_Progress("Loading and Populating Closed Relation. . . ", "Please Wait . . . ", 9, 21);
            //retrivedVoucherBuffer.closedRelation = UIProcessManager.GetClosedRelationByVouher(voucherCode).ToList();
            //Progress_Reporter.Show_Progress("Loading and Populating Non Cash Transaction. . . ", "Please Wait . . . ", 10, 21);
            //retrivedVoucherBuffer.noneCashTransaction = UIProcessManager.GetNonCashTransaction(currentVoucher).ToList();
            //Progress_Reporter.Show_Progress("Loading and Populating Voucher Terms. . . ", "Please Wait . . . ", 11, 21);
            //retrivedVoucherBuffer.voucherTerm = UIProcessManager.GetVoucherTermsByVouchercode(currentVoucher).ToList();
            //Progress_Reporter.Show_Progress("Loading and Populating Voucher Note. . . ", "Please Wait . . . ", 12, 21);
            //retrivedVoucherBuffer.voucherNote = UIProcessManager.GetVoucherNote(currentVoucher);
            //Progress_Reporter.Show_Progress("Loading and Populating Voucher Account. . . ", "Please Wait . . . ", 13, 21);
            //retrivedVoucherBuffer.voucherAccount = UIProcessManager.GetVoucherAccount(currentVoucher).ToList();
            //Progress_Reporter.Show_Progress("Loading and Populating Cart Transaction. . . ", "Please Wait . . . ", 14, 21);
            //List<CartTransaction> cartTransactionList = await UIProcessManager.SelectAllCartTransaction();
            //retrivedVoucherBuffer.cartTransaction = cartTransactionList.FirstOrDefault(k => k.reference == voucherCode);
            //Progress_Reporter.Show_Progress("Loading and Populating Removed Items. . . ", "Please Wait . . . ", 15, 21);
            //retrivedVoucherBuffer.removedItems = UIProcessManager.selectRemovedItemsByVoucher(voucherCode);
            //Progress_Reporter.Show_Progress("Loading and Populating Activity. . . ", "Please Wait . . . ", 16, 21);
            //retrivedVoucherBuffer.activities = UIProcessManager.GetActivity(voucherCode).ToList();
            //Progress_Reporter.Show_Progress("Loading and Populating Other Consignee. . . ", "Please Wait . . . ", 17, 21);
            //retrivedVoucherBuffer.otherConsignee = UIProcessManager.GetOtherConsigneesListByVoucher(voucherCode);
            //Progress_Reporter.Show_Progress("Loading and Populating Preference Value Factor. . . ", "Please Wait . . . ", 18, 21);
            //retrivedVoucherBuffer.preferentialValueFactor = UIProcessManager.PreferenceValueFactorSelectAll().Where(pp => pp.voucher == voucherCode).ToList();
            //Progress_Reporter.Show_Progress("Loading and Populating Fiscal Extension. . . ", "Please Wait . . . ", 19, 21);
            //retrivedVoucherBuffer.fiscalextension = UIProcessManager.SelectAllFiscalExtension().FirstOrDefault(p => p.voucher == voucherCode);
            //Progress_Reporter.Show_Progress("Loading Transaction Currency", "Please Wait . . . ", 20, 21);
            //UIProcessManager.GetTransCurrencyByVoucher(voucherCode).FirstOrDefault();
            //Progress_Reporter.Show_Progress("Loading Voucher Object Completed!!", "Please Wait . . . ", 21, 21);
            //#endregion

            return retrivedVoucherBuffer;
        }

        private bool DeleteVoucherRelatedObjects(bool isLineItem)
        {
            //    try
            //    {
            //        //Delete Line Item Obj
            //        Progress_Reporter.Show_Progress("Deleting LineItem(s). . . ", "Please Wait. . . ", 1, 16);
            //        bool flag = true;
            //        if (isLineItem)
            //        {
            //            int tot = retrivedVoucherBuffer.LineItems.Count;
            //            int p = 1;
            //            foreach (var linObj in retrivedVoucherBuffer.LineItems)
            //            {
            //                if (linObj.lineitem != null && !string.IsNullOrEmpty(linObj.lineitem.code))
            //                {
            //                    string step = "";
            //                    if (p == 1)
            //                    {
            //                        step = "1st";
            //                    }
            //                    else if (p == 2)
            //                    {
            //                        step = "2nd";
            //                    }
            //                    else if (p == 3)
            //                    {
            //                        step = "3rd";
            //                    }
            //                    else if (p > 3)
            //                    {
            //                        step = p + "th";
            //                    }
            //                    Progress_Reporter.Show_Progress(string.Format("Deleting {0} LineItem. . .", step), "Please Wait. . . ", p, tot);
            //                    if (linObj.lineitemValueFactors != null && linObj.lineitemValueFactors.Count > 0)
            //                    {
            //                        flag = UIProcessManager.DeleteLineItemValueFactorByLineItem(linObj.lineitem);
            //                    }
            //                    for (int y = 0; y < linObj.lifespans.Count; y++)
            //                    {
            //                        if (linObj.lifespans[y].component == CNETConstantes.SERIAL_CODE_COMPONENT && linObj.serialCodes.Count == linObj.lifespans.Count)
            //                        {
            //                           flag =  UIProcessManager.DeleteLifespanByReference(linObj.serialCodes[y].code);
            //                        }
            //                        else
            //                        {
            //                          flag =   UIProcessManager.DeleteLifespanByReference(linObj.lineitem.code);
            //                        }
            //                    }
            //                    if (linObj.serialCodes != null && linObj.serialCodes.Count > 0)
            //                    {
            //                       flag =  UIProcessManager.DeleteSerialCodeByLineItem(linObj.lineitem.code);
            //                    }
            //                    if (linObj.lineitemReference != null && linObj.lineitemReference.referenced != null)
            //                    {
            //                       flag =  UIProcessManager.DeleteLineitemReference(linObj.lineitem.code);
            //                    }
            //                    if (linObj.lineitemNote != null && !string.IsNullOrEmpty(linObj.lineitemNote.note))
            //                    {
            //                        flag = UIProcessManager.DeleteLineItemNote(linObj.lineitem.code);
            //                    }
            //                    if (linObj.schedules != null && !string.IsNullOrEmpty(linObj.schedules.scheduledHeader))
            //                    {
            //                       flag =  UIProcessManager.DeleteScheduleByReference(linObj.lineitem.code);
            //                    }
            //                    if (linObj.lineitemWeight != null && !string.IsNullOrEmpty(linObj.lineitemWeight.articleWt.ToString()))
            //                    {
            //                       flag = UIProcessManager.DeleteLineitemWeightByLineitemcode(linObj.lineitem.code);
            //                    }
            //                    if (linObj.LineItemConversions != null && linObj.LineItemConversions.Count > 0)
            //                    {
            //                       flag =  UIProcessManager.DeleteLineitemConversionByLineitem(linObj.lineitem.code);
            //                    }
            //                }
            //                p++;
            //            }

            //            Progress_Reporter.Show_Progress("Deleting LineItem. . . ", "Please Wait. . .", 2, 16);
            //            if (retrivedVoucherBuffer.newLineitemObjs != null && retrivedVoucherBuffer.newLineitemObjs.Count > 0)
            //            {
            //                flag = UIProcessManager.DeleteLineitem(retrivedVoucherBuffer.voucher);
            //            }
            //        }

            //        Progress_Reporter.Show_Progress("Deleting Other Consignee. . . ", "Please Wait. . .", 3, 16);
            //        if (retrivedVoucherBuffer.otherConsignee != null && retrivedVoucherBuffer.otherConsignee.Count > 0)
            //        {
            //           flag =  UIProcessManager.DeleteOtherConsignees(retrivedVoucherBuffer.voucher);
            //        }

            //        Progress_Reporter.Show_Progress("Deleting Store Transaction. . . ", "Please Wait. . .", 4, 16);
            //        if (retrivedVoucherBuffer.storeTransaction != null && !string.IsNullOrEmpty(retrivedVoucherBuffer.storeTransaction.voucher))
            //        {
            //            flag = UIProcessManager.DeleteStoreTransaction(retrivedVoucherBuffer.voucher);
            //        }

            //        Progress_Reporter.Show_Progress("Deleting Activity. . . ", "Please Wait. . .", 5, 16);
            //        if (retrivedVoucherBuffer.activities != null && retrivedVoucherBuffer.activities.Count > 0)
            //        {
            //           flag =  UIProcessManager.DeleteActivityByReference(retrivedVoucherBuffer.voucher.code);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Cloase Relation. . . ", "Please Wait. . .", 6, 16);
            //        if (retrivedVoucherBuffer.closedRelation != null && retrivedVoucherBuffer.closedRelation.Count > 0)
            //        {
            //            flag = UIProcessManager.DeleteClosedRelation(retrivedVoucherBuffer.voucher.code);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Cart Transaction. . . ", "Please Wait. . .", 7, 16);
            //        if (retrivedVoucherBuffer.cartTransaction != null && !string.IsNullOrEmpty(retrivedVoucherBuffer.cartTransaction.reference))
            //        {
            //            flag = UIProcessManager.DeleteCartTransactions(retrivedVoucherBuffer.voucher);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Tax Transaction. . . ", "Please Wait. . .", 8, 16);
            //        if (retrivedVoucherBuffer.taxTransactions != null && retrivedVoucherBuffer.taxTransactions.Count > 0)
            //        {
            //            flag = UIProcessManager.DeleteTaxTransaction(retrivedVoucherBuffer.voucher);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Transaction References. . . ", "Please Wait. . .", 8, 16);
            //        if (retrivedVoucherBuffer.transactionReferences != null && retrivedVoucherBuffer.transactionReferences.Count > 0)
            //        {
            //            UIProcessManager.DeleteTransactionReference(retrivedVoucherBuffer.voucher.code);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Voucher Extension Transaction. . . ", "Please Wait. . .", 9, 16);
            //        if (retrivedVoucherBuffer.voucherExtensionTransactions != null && retrivedVoucherBuffer.voucherExtensionTransactions.Count > 0)
            //        {
            //            flag = UIProcessManager.DeleteVoucherExtensionTransaction(retrivedVoucherBuffer.voucher);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Voucher Value. . . ", "Please Wait. . .", 10, 16);
            //        if (retrivedVoucherBuffer.voucherValue != null && !string.IsNullOrEmpty(retrivedVoucherBuffer.voucherValue.voucher))
            //        {
            //            flag = UIProcessManager.DeleteVoucherValue(retrivedVoucherBuffer.voucher);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Cash Transaction. . . ", "Please Wait. . .", 11, 16);
            //        if (retrivedVoucherBuffer.noneCashTransaction != null && retrivedVoucherBuffer.noneCashTransaction.Count > 0)
            //        {
            //            flag = UIProcessManager.DeleteNonCashTransaction(retrivedVoucherBuffer.voucher);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Voucher Term. . . ", "Please Wait. . .", 12, 16);
            //        if (retrivedVoucherBuffer.voucherTerm != null && retrivedVoucherBuffer.voucherTerm.Count > 0)
            //        {
            //            flag = UIProcessManager.DeleteVoucherTermByVoucher(retrivedVoucherBuffer.voucher);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Voucher Note. . . ", "Please Wait. . .", 13, 16);
            //        if (retrivedVoucherBuffer.voucherNote != null && !string.IsNullOrEmpty(retrivedVoucherBuffer.voucherNote.voucher))
            //        {
            //            flag = UIProcessManager.DeleteVoucherNote(retrivedVoucherBuffer.voucher);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Voucher Account. . . ", "Please Wait. . .", 14, 16);
            //        if (retrivedVoucherBuffer.voucherAccount != null && retrivedVoucherBuffer.voucherAccount.Count > 0)
            //        {
            //            flag = UIProcessManager.DeleteVoucherAccount(retrivedVoucherBuffer.voucher);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Preference Value Factor. . . ", "Please Wait. . .", 15, 16);
            //        if (retrivedVoucherBuffer.preferentialValueFactor != null && retrivedVoucherBuffer.preferentialValueFactor.Count > 0)
            //        {
            //            flag = UIProcessManager.DeletePreferenceValueFactorByVoucher(retrivedVoucherBuffer.voucher.code);
            //        }

            //        Progress_Reporter.Show_Progress("Deleting Transaction Currency", "Please Wait. . .", 15, 16);
            //        if (retrivedVoucherBuffer.transactionCurrency != null)
            //        {
            //            flag = UIProcessManager.DeleteTransactionCurrencyByVoucher(retrivedVoucherBuffer.voucher);
            //        }
            //        Progress_Reporter.Show_Progress("Deleting Completed!!", "Please Wait. . .", 15, 16);

            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        return false;
            //    }

            return false;
        }

        //private VoucherBufferDTO updateVoucherCode(string voucherID, VoucherBufferDTO voucherBufferSave, bool isLineItem)
        //{
        //    if (voucherBufferSave.voucher != null)
        //    {
        //        voucherBufferSave.voucher.code = voucherID;
        //    }

        //    if (voucherBufferSave.voucherValue != null)
        //    {
        //        voucherBufferSave.voucherValue.voucher = voucherID;
        //    }

        //    if (voucherBufferSave.taxTransactions != null && voucherBufferSave.taxTransactions.Count > 0)
        //    {
        //        foreach (var taxTransaction in voucherBufferSave.taxTransactions)
        //        {
        //            taxTransaction.voucher = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.storeTransaction != null)
        //    {
        //        voucherBufferSave.storeTransaction.voucher = voucherID;
        //    }

        //    if (voucherBufferSave.voucherExtensionTransactions != null && voucherBufferSave.voucherExtensionTransactions.Count > 0)
        //    {
        //        foreach (var voucherExtensiontransaction in voucherBufferSave.voucherExtensionTransactions)
        //        {
        //            voucherExtensiontransaction.voucher = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.closedRelation != null)
        //    {
        //        foreach (var closedRelations in voucherBufferSave.closedRelation)
        //        {
        //            closedRelations.voucher = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.noneCashTransaction != null && voucherBufferSave.noneCashTransaction.Count > 0)
        //    {
        //        foreach (var noncashTransaction in voucherBufferSave.noneCashTransaction)
        //        {
        //            noncashTransaction.voucher = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.voucherTerm !=  null && voucherBufferSave.voucherTerm.Count > 0)
        //    {
        //        foreach (var voucherTerm in voucherBufferSave.voucherTerm)
        //        {
        //            voucherTerm.voucher = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.voucherNote != null)
        //    {
        //        voucherBufferSave.voucherNote.voucher = voucherID;
        //    }

        //    if (voucherBufferSave.transactionCurrency != null)
        //    {
        //        voucherBufferSave.transactionCurrency.voucher = voucherID;
        //    }

        //    if (voucherBufferSave.fiscalextension != null && voucherBufferSave.fiscalextension.fsNo != null && voucherBufferSave.fiscalextension.fsNo != "")
        //    {
        //        voucherBufferSave.fiscalextension.voucher = voucherID;
        //    }

        //    if (voucherBufferSave.voucherAccount != null && voucherBufferSave.voucherAccount.Count > 0)
        //    {
        //        foreach (var voucherAccount in voucherBufferSave.voucherAccount)
        //        {
        //            voucherAccount.voucher = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.cartTransaction != null)
        //    {
        //        voucherBufferSave.cartTransaction.reference = voucherID;
        //    }

        //    if (voucherBufferSave.removedItems != null && voucherBufferSave.removedItems.Count > 0)
        //    {
        //        foreach (var removedItem in voucherBufferSave.removedItems)
        //        {
        //            removedItem.voucher = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.activities != null && voucherBufferSave.activities.Count > 0)
        //    {
        //        foreach (var activity in voucherBufferSave.activities)
        //        {
        //            activity.reference = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.otherConsignee != null && voucherBufferSave.otherConsignee.Count > 0)
        //    {
        //        foreach (var otherConsignee in voucherBufferSave.otherConsignee)
        //        {
        //            otherConsignee.voucher = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.preferentialValueFactor != null && voucherBufferSave.preferentialValueFactor.Count > 0)
        //    {
        //        foreach (var prefrencevalueFactor in voucherBufferSave.preferentialValueFactor)
        //        {
        //            prefrencevalueFactor.voucher = voucherID;
        //        }
        //    }

        //    if (voucherBufferSave.transactionReferences != null && voucherBufferSave.transactionReferences.Count > 0)
        //    {
        //        foreach (var transRf in voucherBufferSave.transactionReferences)
        //        {
        //            transRf.referenced = voucherID;
        //        }
        //    }
        //    if (isLineItem && voucherBufferSave.newLineitemObjs != null)
        //    {
        //        //to be done.
        //        foreach (var lineItemObj in voucherBufferSave.newLineitemObjs)
        //        {
        //            lineItemObj.lineitem.voucher = voucherID;
        //        }
        //    }
        //    return voucherBufferSave;
        //}

        // Note: this method should be called after the orgional voucher buffer is mapped because retrivedVoucherBuffer will be modified 
        //       by the principle of POINTER

        // Note: VoucherValue, TaxTransaction and NewLineItemObject are already added to the newVoucher Buffer
        //private VoucherBufferDTO MapVoucherBuffer(VoucherBufferDTO newVoucher)
        //{
        //    string voucherCode = newVoucher.voucher.code;
        //    if (retrivedVoucherBuffer.fiscalextension != null)
        //    {
        //        newVoucher.fiscalextension = retrivedVoucherBuffer.fiscalextension;
        //        newVoucher.fiscalextension.voucher = voucherCode;
        //    }

        //    if (retrivedVoucherBuffer.transactionReferences != null)
        //    {
        //        newVoucher.transactionReferences = retrivedVoucherBuffer.transactionReferences;
        //        foreach (var tranRef in newVoucher.transactionReferences)
        //        {
        //            tranRef.referening = voucherCode;
        //            tranRef.value = newVoucher.voucher.grandTotal;
        //        }
        //    }

        //    if (retrivedVoucherBuffer.storeTransaction != null)
        //    {
        //        newVoucher.storeTransaction = retrivedVoucherBuffer.storeTransaction;
        //        newVoucher.storeTransaction.voucher = voucherCode;
        //    }

        //    if (retrivedVoucherBuffer.voucherExtensionTransactions != null)
        //    {
        //        newVoucher.voucherExtensionTransactions = retrivedVoucherBuffer.voucherExtensionTransactions;
        //        foreach (var vet in newVoucher.voucherExtensionTransactions)
        //        {
        //            vet.voucher = voucherCode;
        //        }

        //    }

        //    if (retrivedVoucherBuffer.closedRelation != null)
        //    {
        //        newVoucher.closedRelation = retrivedVoucherBuffer.closedRelation;
        //        foreach (var cr in newVoucher.closedRelation)
        //        {
        //            cr.voucher = voucherCode;
        //        }
        //    }

        //    if (retrivedVoucherBuffer.noneCashTransaction != null)
        //    {
        //        newVoucher.noneCashTransaction = retrivedVoucherBuffer.noneCashTransaction;
        //        foreach (var nct in newVoucher.noneCashTransaction)
        //        {
        //            nct.voucher = voucherCode;
        //        }
        //    }

        //    if (retrivedVoucherBuffer.voucherTerm != null)
        //    {
        //        newVoucher.voucherTerm = retrivedVoucherBuffer.voucherTerm;
        //        foreach (var vt in newVoucher.voucherTerm)
        //        {
        //            vt.voucher = voucherCode;
        //        }
        //    }

        //    if (retrivedVoucherBuffer.voucherNote != null)
        //    {
        //        newVoucher.voucherNote = retrivedVoucherBuffer.voucherNote;
        //        newVoucher.voucherNote.voucher = voucherCode;
        //    }


        //    if (retrivedVoucherBuffer.voucherAccount != null)
        //    {
        //        newVoucher.voucherAccount = retrivedVoucherBuffer.voucherAccount;
        //        foreach (var va in newVoucher.voucherAccount)
        //        {
        //            va.voucher = voucherCode;
        //        }
        //    }

        //    if (retrivedVoucherBuffer.cartTransaction != null)
        //    {
        //        newVoucher.cartTransaction = retrivedVoucherBuffer.cartTransaction;
        //        newVoucher.cartTransaction.reference = voucherCode;
        //    }


        //    if (retrivedVoucherBuffer.removedItems != null)
        //    {
        //        newVoucher.removedItems = retrivedVoucherBuffer.removedItems;
        //        foreach (var ri in newVoucher.removedItems)
        //        {
        //            ri.voucher = voucherCode;
        //        }
        //    }

        //    if (retrivedVoucherBuffer.activities != null)
        //    {
        //        newVoucher.activities = retrivedVoucherBuffer.activities;
        //        foreach (var activity in newVoucher.activities)
        //        {
        //            activity.reference = voucherCode;
        //        }
        //    }

        //    if (retrivedVoucherBuffer.otherConsignee != null)
        //    {
        //        newVoucher.otherConsignee = retrivedVoucherBuffer.otherConsignee;
        //        foreach (var oc in newVoucher.otherConsignee)
        //        {
        //            oc.voucher = voucherCode;
        //        }
        //    }

        //    if (retrivedVoucherBuffer.preferentialValueFactor != null)
        //    {
        //        newVoucher.preferentialValueFactor = retrivedVoucherBuffer.preferentialValueFactor;
        //        foreach (var p in newVoucher.preferentialValueFactor)
        //        {
        //            p.voucher = voucherCode;
        //        }
        //    }

        //    return newVoucher;
        //}

        //private NewLineitemObj updateLineitemObjs(string lineitemCode, NewLineitemObj currentSaveLineitemObj)
        //{
        //    if (currentSaveLineitemObj.lineitemValueFactors != null && currentSaveLineitemObj.lineitemValueFactors.Count > 0)
        //    {
        //        foreach (var valueFact in currentSaveLineitemObj.lineitemValueFactors)
        //        {
        //            valueFact.lineItem = lineitemCode;
        //        }
        //    }

        //    if (currentSaveLineitemObj.serialCodes != null && currentSaveLineitemObj.serialCodes.Count > 0)
        //    {
        //        foreach (var serialCde in currentSaveLineitemObj.serialCodes)
        //        {
        //            serialCde.lineItem = lineitemCode;
        //        }
        //    }
        //    if (currentSaveLineitemObj.lifespans != null && currentSaveLineitemObj.lifespans.Count > 0)
        //    {
        //        foreach (var lifespn in currentSaveLineitemObj.lifespans)
        //        {
        //            if (lifespn.component != CNETConstantes.SERIAL_CODE_COMPONENT)
        //            {
        //                lifespn.reference = lineitemCode;
        //            }
        //        }
        //    }
        //    if (currentSaveLineitemObj.lineitemReference != null && !string.IsNullOrEmpty(currentSaveLineitemObj.lineitemReference.referenced))
        //    {
        //        currentSaveLineitemObj.lineitemReference.lineItem = lineitemCode;
        //    }
        //    if (currentSaveLineitemObj.lineitemWeight != null && !string.IsNullOrEmpty(currentSaveLineitemObj.lineitemWeight.articleWt.ToString()))
        //    {
        //        currentSaveLineitemObj.lineitemWeight.lineitem = lineitemCode;
        //    }
        //    if (currentSaveLineitemObj.lineitemNote != null && !string.IsNullOrEmpty(currentSaveLineitemObj.lineitemNote.note))
        //    {
        //        currentSaveLineitemObj.lineitemNote.lineItem = lineitemCode;
        //    }
        //    if (currentSaveLineitemObj.LineItemConversions != null && currentSaveLineitemObj.LineItemConversions.Count > 0)
        //    {
        //        foreach (var linCo in currentSaveLineitemObj.LineItemConversions)
        //        {
        //            linCo.lineItem = lineitemCode;
        //        }
        //    }
        //    if (currentSaveLineitemObj.schedules != null && currentSaveLineitemObj.schedules.scheduledHeader != null && currentSaveLineitemObj.schedules.scheduledHeader != "")
        //    {
        //        currentSaveLineitemObj.schedules.reference = lineitemCode;
        //    }
        //    return currentSaveLineitemObj;
        //}

        private bool SaveVoucher(VoucherBuffer voucherBufferSave, bool directPreparation, bool isLineItem)
        {
            try
            {
                DateTime? currentTime = UIProcessManager.GetServiceTime();
                Progress_Reporter.Show_Progress("Mapping data ...", "Please Wait. . . ");
                //voucherBufferSave = MapVoucherBuffer(voucherBufferSave);

                if (directPreparation)
                {

                    string currentVoCode = UIProcessManager.IdGenerater("Voucher", voucherBufferSave.Voucher.Definition, 0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                    if (string.IsNullOrEmpty(currentVoCode))
                    {
                        SystemMessage.ShowModalInfoMessage("Unable to generate voucher Id!", "ERROR");
                        return false;
                    }

                    //string voucherGeneratedID = generatedId.GeneratedNewId;

                    //if (voucherGeneratedID != voucherBufferSave.voucher.code)
                    //{
                    //    voucherBufferSave = updateVoucherCode(voucherGeneratedID, voucherBufferSave, isLineItem);
                    //}
                    voucherBufferSave.Voucher.Id = 0;
                    voucherBufferSave.Voucher.Code = currentVoCode;
                    voucherBufferSave.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
                    TransactionReferenceBuffer TRBuffer = new TransactionReferenceBuffer();
                    TRBuffer.TransactionReference = new TransactionReferenceDTO()
                    {
                        ReferencingVoucherDefn = voucherBufferSave.Voucher.Definition,
                        ReferencedVoucherDefn = CNETConstantes.REGISTRATION_VOUCHER,
                        Referenced = RegistrationExt.Id

                    };
                    voucherBufferSave.LineItemsBuffer.ForEach(x => x.LineItem.Id = 0);
                    TRBuffer.ReferencedActivity = null;
                    voucherBufferSave.TransactionReferencesBuffer.Add(TRBuffer);
                    voucherBufferSave.TransactionCurrencyBuffer = retrivedVoucherBuffer.TransactionCurrencyBuffer;
                    voucherBufferSave.Activity = ActivityLogManager.SetupActivity(currentTime.Value, adBreakVoucher.Value, CNETConstantes.PMS_Pointer, "Voucher is breaked");

                    voucherBufferSave.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                    voucherBufferSave.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                    voucherBufferSave.TransactionCurrencyBuffer = null;

                    ResponseModel<VoucherBuffer> voucherSave = UIProcessManager.CreateVoucherBuffer(voucherBufferSave);

                    if (voucherSave != null && voucherSave.Success)
                        return true;
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Fail Saving !! " + Environment.NewLine + voucherSave.Message, "Error");
                        return false;
                    }

                }
                else
                {
                    retrivedVoucherBuffer.Voucher.Code = retrivedVoucherBuffer.Voucher.Code;
                    voucherBufferSave.Activity = ActivityLogManager.SetupActivity(currentTime.Value, adBreakVoucher.Value, CNETConstantes.PMS_Pointer, "Voucher is breaked");
                    voucherBufferSave.TransactionCurrencyBuffer = retrivedVoucherBuffer.TransactionCurrencyBuffer;
                    voucherBufferSave.TransactionReferencesBuffer = retrivedVoucherBuffer.TransactionReferencesBuffer;

                    if (voucherBufferSave.TransactionReferencesBuffer != null && voucherBufferSave.TransactionReferencesBuffer.Count > 0)
                        voucherBufferSave.TransactionReferencesBuffer.ToList().ForEach(x => x.ReferencedActivity = null);


                    voucherBufferSave.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                    voucherBufferSave.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                    voucherBufferSave.TransactionCurrencyBuffer = null;

                    ResponseModel<VoucherBuffer> voucherSave = UIProcessManager.UpdateVoucherBuffer(voucherBufferSave);

                    if (voucherSave != null && voucherSave.Success)
                    {
                        return true;
                    }
                    else
                    {
                        SystemMessage.ShowModalInfoMessage("Fail Saving !! " + Environment.NewLine + voucherSave.Message, "Error");
                        return false;
                    }
                }


            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion


        #region Event Handlers

        private void lukSplitBy_EditValueChanged(object sender, EventArgs e)
        {
            //if (lukSplitBy.EditValue.ToString() == "1")
            //{
            //    rgFactor.Enabled = false;
            //    teValue.Enabled = false;
            //}
            //else
            //{
            //    rgFactor.Enabled = true;
            //    teValue.Enabled = true;
            //}
        }

        private void bbiCancle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void bbiClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gc_splittedVouchers.DataSource = null;
            gv_splittedVouchers.RefreshData();
            splittedVoucherList = null;
            teValue.Text = "";
            teDate.Text = CurrentVoucher.IssuedDate.ToShortDateString();
            rgFactor.SelectedIndex = 0;
        }

        private void gv_splittedVouchers_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "SN")
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void bbiSplit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SplitVoucher splitVoucher = new SplitVoucher();
                decimal? value = null;
                bool? isPercent = null;
                int splitBy = Convert.ToInt32(lukSplitBy.EditValue); //splitBy = 1 -> quantity else if splitBy = 2 -> value
                isPercent = Convert.ToBoolean(rgFactor.SelectedIndex);

                if (!string.IsNullOrWhiteSpace(teValue.Text))
                {
                    value = Convert.ToDecimal(teValue.Text);
                    if (!isPercent.Value)
                    {
                        if (value <= 0)
                        {
                            SystemMessage.ShowModalInfoMessage("Value should be greater than 0!", "ERROR");
                            return;
                        }
                        if (lineItems != null && lineItems.Count > 0)
                        {
                            if (splitBy == 2)
                            {
                                LineItemDTO minLi = lineItems.OrderBy(l => l.UnitAmount).FirstOrDefault();
                                if (value > minLi.UnitAmount)
                                {
                                    SystemMessage.ShowModalInfoMessage("Over lineitem value! minimum value should be <= " + minLi.UnitAmount, "ERROR");
                                    return;
                                }

                            }
                            else if (splitBy == 1)
                            {
                                // it is quanitity
                                LineItemDTO minLi = lineItems.OrderBy(l => l.Quantity).FirstOrDefault();
                                if (value > (decimal)minLi.Quantity)
                                {
                                    SystemMessage.ShowModalInfoMessage("Over lineitem's quantity value! minimum value should be <= " + minLi.Quantity, "ERROR");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (value >= CurrentVoucher.GrandTotal)
                            {
                                SystemMessage.ShowModalInfoMessage("Over grand total value! minimum value should be < " + CurrentVoucher.GrandTotal, "ERROR");
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (value >= 100 || value <= 0)
                        {
                            SystemMessage.ShowModalInfoMessage("Percent value should be between 0 and 100!", "ERROR");
                            return;
                        }
                    }

                }



                Progress_Reporter.Show_Progress("Splitting Voucher ...", "Please Wait.......");

                //get lineitems of the voucher

                List<LineItemBuffer> nliList = new List<LineItemBuffer>();
                if (lineItems != null)
                {
                    foreach (var li in lineItems)
                    {

                        LineItemBuffer newLineItemObj = new LineItemBuffer();
                        newLineItemObj.LineItem = li;
                        nliList.Add(newLineItemObj);
                    }
                }

                if (!isPercent.Value && value != null && lineItems != null)
                {
                    value = (value.Value / CurrentVoucher.GrandTotal) * 100;
                    if (value > 100)
                    {
                        SystemMessage.ShowModalInfoMessage("Over grand total value! minimum value should be < " + CurrentVoucher.GrandTotal, "ERROR");
                        return;
                    }
                    isPercent = true;
                }

                splittedVoucherList = splitVoucher.SplitVouchers(new VoucherBuffer() { Voucher = CurrentVoucher, LineItemsBuffer = nliList },
                   Convert.ToInt32(spEdit_qty.EditValue), splitBy == 2 ? "value" : "quantity", isPercent, value, CurrentTime, RegistrationExt.Id);

                if (splittedVoucherList != null && splittedVoucherList.Count > 0)
                {
                    List<SplittedDTO> dtoList = new List<SplittedDTO>();
                    foreach (var s in splittedVoucherList)
                    {
                        SplittedDTO dto = new SplittedDTO();
                        if (s.Voucher == null) continue;
                        if (s.Voucher.Code == CurrentVoucher.Code)
                            dto.date = CurrentVoucher.IssuedDate.ToShortDateString();
                        else
                            dto.date = s.Voucher.IssuedDate.ToShortDateString();
                        dto.amount = s.Voucher.GrandTotal;
                        dto.code = s.Voucher.Code;

                        if (s.LineItemsBuffer != null)
                        {
                            dto.LineItems = new List<DailyLineItemDTO>();
                            foreach (var li in s.LineItemsBuffer)
                            {
                                DailyLineItemDTO dt = new DailyLineItemDTO();
                                dt.articleCode = li.LineItem.Article;
                                dt.Name = UIProcessManager.GetArticleById(li.LineItem.Article).Name;
                                dt.quantity = li.LineItem.Quantity;
                                dt.unitAmount = li.LineItem.UnitAmount;
                                dt.totalAmunt = li.LineItem.TotalAmount;
                                dto.LineItems.Add(dt);
                            }
                        }

                        dtoList.Add(dto);
                    }

                    gc_splittedVouchers.DataSource = dtoList;
                    gv_splittedVouchers.RefreshData();
                }

                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in spliting voucher. Detail: " + ex.Message, "ERROR");
            }
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (splittedVoucherList == null && splittedVoucherList.Count < 2)
                {
                    SystemMessage.ShowModalInfoMessage("No splitted vouchers!", "ERROR");
                    return;
                }


                Progress_Reporter.Show_Progress("Saving Voucher Split ...", "Please Wait.......");
                SystemConstantDTO vd = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.FirstOrDefault(x => x.Id == CurrentVoucher.Definition);
                bool Lineitem = true;
                if (vd.Value.ToLower() == "lineitem")
                    Lineitem = true;
                else
                    Lineitem = false;

                bool flag1 = true;
                bool flag2 = true;
                string v1 = "";
                string v2 = "";
                foreach (var voBuffer in splittedVoucherList)
                {
                    if (voBuffer.Voucher.Code == CurrentVoucher.Code)
                    {
                        v1 = string.Format("{0} ({1})", voBuffer.Voucher.Id, voBuffer.Voucher.GrandTotal);
                        PrepareVoucherBufferObj(voBuffer.Voucher.Id);
                        if (vd != null)
                        {


                            flag1 = SaveVoucher(voBuffer, false, Lineitem);
                        }

                        //Synchronize 
                        // CommonLogics.Synchronize(voBuffer.voucher.code, voBuffer.voucher.voucherDefinition.ToString(), CNETConstantes.VOUCHER_COMPONENET);

                    }
                    else
                    {
                        v2 = string.Format("{0} ({1})", voBuffer.Voucher.Id, voBuffer.Voucher.GrandTotal);
                        if (vd != null)
                        {
                            flag2 = SaveVoucher(voBuffer, true, Lineitem);
                        }

                        //Synchronize 
                        // CommonLogics.Synchronize(voBuffer.voucher.code, voBuffer.voucher.voucherDefinition.ToString(), CNETConstantes.VOUCHER_COMPONENET);
                    }



                }

                //save activity
                if (flag1 || flag2)
                {
                    DateTime? currentTime = UIProcessManager.GetServiceTime();
                    if (currentTime != null)
                    {
                        ActivityDTO activity = ActivityLogManager.SetupActivity(currentTime.Value, adBreakVoucher.Value, CNETConstantes.PMS_Pointer, "Voucher is breaked into " + v1 + " and " + v2);
                        activity.Reference = CurrentVoucher.Id;
                        UIProcessManager.CreateActivity(activity);
                    }
                }

                if (!flag1 && flag2)
                {
                    XtraMessageBox.Show("Origional Voucher is not saved!", "Voucher Split", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else if (flag1 && !flag2)
                {
                    XtraMessageBox.Show("New voucher is not saved!", "Voucher Split", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else if (!flag1 && !flag2)
                {
                    XtraMessageBox.Show("Voucher split is not successfull!", "Voucher Split", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else
                {
                    XtraMessageBox.Show("Voucher Split is Successfull!", "Voucher Split", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }


                Progress_Reporter.Close_Progress();
            }
            catch (Exception ex)
            {
                Progress_Reporter.Close_Progress();
                SystemMessage.ShowModalInfoMessage("Error in saving voucher split. Detail: " + ex.Message, "ERROR");

            }
        }

        private void rgFactor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rgFactor.SelectedIndex == 0)
            {
                lciFactor.Text = "Amount";
            }
            else
            {
                lciFactor.Text = "% Value";
            }
        }

        private void frmSplitItem_Load(object sender, EventArgs e)
        {
            if (!InitializeData())
            {
                this.Close();
            }
        }

        #endregion

        private class SplitByHolder
        {
            public int Value { get; set; }
            public string SplitBy { get; set; }
        }

        private class SplittedDTO
        {
            public int id { get; set; }
            public string code { get; set; }
            public string date { get; set; }
            public decimal amount { get; set; }

            public List<DailyLineItemDTO> LineItems { get; set; }
        }






    }


}
