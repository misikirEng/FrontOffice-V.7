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
using CNET.POS.Common.Models;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET.POS.Settings;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using System.Diagnostics;
using DevExpress.XtraGauges.Core.Model;
using CNET.FP.Tool;
using CNET_V7_Domain.Misc.PmsView;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET_V7_Domain;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509; 
using CNET_V7_Domain.Domain.Transaction;
using DevExpress.CodeParser;
using System.Reflection.Metadata.Ecma335;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Misc;
using DevExpress.Printing.Core.PdfExport.Metafile;
using CNET_V7_Domain.Domain.SecuritySchema;
using DevExpress.XtraGrid.Views.Grid;
using CNET.FP.Tool.FP_Types;
using DevExpress.Diagram.Core.Shapes;
using static ERP.EventManagement.Modals.EventRequirement;

namespace ERP.EventManagement
{
    public partial class frmFrontOfficePOS : XtraForm
    {
        private Consignee customerDto = null;
        private VoucherBuffer voucherbuffer;
        private VoucherFinalDTO voFinal = new VoucherFinalDTO();
        private List<VwLineItemDetailViewDTO> lineItemDetail = new List<VwLineItemDetailViewDTO>();
        string _currentVoucherCode { get; set; }
        DateTime CurrentTime = DateTime.Now;
        public VoucherBuffer EventVoucher { get; set; }
        public string EventOwnerName { get; set; }
        public int? EventOwnerId { get; set; }
        public int EventId { get; set; }
        public string EventCode { get; set; }
        public List<EventRequirementView> EventRequirementList { get; set; }
        public List<EventLineItemData> EventLineItemList { get; set; }

        public List<LineItemDisplayDTO> EventLineItemDisplay { get; set; }

        #region Properties



        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Checking Out " + voucherbuffer.Voucher.Code + " Registration !! ");

                //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Print Button Clicked FrontOfficePOS Form for Receipt");

                _currentVoucherCode = GetCurrentId(0);
                if (string.IsNullOrEmpty(_currentVoucherCode))
                {
                    MessageBox.Show("Unable to generate ID!", "ERROR");
                    return;
                }



                POS_Settings.IsError = true;
                POS_Settings.Machine_Consginee_Unit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value;
                POS_Settings.Machine_ID = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                //POS_Settings.Voucher_Definition = CNETConstantes.CHECK_OUT_BILL_VOUCHER;
                POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);
 

                    // FiscalPrinters FP = new FiscalPrinters();
                    FiscalPrinters.GetInstance();

                if (!POS_Settings.IsError)
                {
                    XtraMessageBox.Show("Unable to connect with fisical printer", "CNET_2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool validatePMSLicense = Validate_PMSPOS_License();
                if (!validatePMSLicense)
                    return;


                //get print items 
                if (voFinal.lineItemDetails != null && voFinal.lineItemDetails.Count > 0)
                {
                    if (_printItems != null) _printItems.Clear();
                    _printItems = GetPrintItems(voFinal.lineItemDetails);
                }
                else
                {
                    MessageBox.Show("Unable to get print Items", "ERROR");
                    return;
                }

                if (_printItems == null || _printItems.Count == 0)
                {
                    MessageBox.Show("Unable to get print Items", "ERROR");
                    return;
                }
                if (_printItems.Sum(x => x.UnitPrice) <= 0)
                {
                    MessageBox.Show("Unable to get print Items Price !!", "ERROR");
                    return;
                }





                //Progress_Reporter.Show_Progress("Saving Checkout Bill", "Please Wait.....");
                //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Saving Checkout Bill FrontOfficePOS Form");
                VoucherBuffer isVoucherSaved = SaveCurrentVoucher();

                if (isVoucherSaved == null)
                {
                    //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Saving Checkout Bill Fail FrontOfficePOS Form");
                    XtraMessageBox.Show("Checkout Voucher is not saved!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    POS_Settings.TotalServiceCharge = isVoucherSaved.Voucher.AddCharge;
                    POS_Settings.TotalDiscount = isVoucherSaved.Voucher.Discount;

                    //Progress_Reporter.Show_Progress("Printing Recipt", "Please Wait.....");
                    //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Done Saving Checkout Bill FrontOfficePOS Form");

                    //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Printing Receipt FrontOfficePOS Form");
                    //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Total Service Charge" + POS_Settings.TotalServiceCharge);
                    //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Total Discount" + POS_Settings.TotalDiscount);


                    bool isprinted = FiscalPrinters.PrintOperation(POS_Settings.Voucher_Definition, _printItems, POS_Settings.printerType, new List<Consignee>() { customerDto }, 0, _currentVoucherCode,
                                     null, null, LocalBuffer.LocalBuffer.CurrentLoggedInUser.UserName, POS_Settings.TotalDiscount, POS_Settings.TotalServiceCharge, isVoucherSaved.Voucher.IssuedDate, 0);
                    if (!isprinted)
                    {
                        XtraMessageBox.Show("Voucher is not printed", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        var dele = UIProcessManager.DeleteVoucherObjects(isVoucherSaved.Voucher.Id);

                    }
                    else
                    {
                        _currentVoucherCode = GetCurrentId(1);
                        VoucherDTO voucher = UIProcessManager.Patch_FS_No(isVoucherSaved.Voucher.Id, POS_Settings.CurrentFSNo, POS_Settings.fiscalprinterMRC);
                    }
                }




                EventVoucher.Voucher.LastState = CNETConstantes.OSD_EVENTCHECKEDOUT;




                VoucherDTO isVoucherUpdated = UIProcessManager.UpdateVoucher(EventVoucher.Voucher);



                if (isVoucherUpdated != null)
                {
                    if (isVoucherUpdated.LastState == CNETConstantes.OSD_EVENTCHECKEDOUT)
                        XtraMessageBox.Show("State Successfully Changed To Check-Out!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //Synchronize 
                    this.Close();
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    //PMSDataLogger.LogMessage("frmFrontOfficePOS", voucherbuffer.Voucher.Code + " Voucher CHECKED OUT STATE with Receipt Fail");
                    XtraMessageBox.Show("Event voucher is not updated to Checkout !!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //Progress_Reporter.Close_Progress();
        }

        public static bool Validate_PMSPOS_License()
        {
            try
            {
                string[] splitted = new string[2];
                string machineTIN = "";
                try
                {
                    //FiscalPrinters.GetInstance();
                    switch (POS_Settings.printerType.ToLower())
                    {
                        case "datecs_fmp_350":
                        case "datecs_fp_700":
                        case "datecs_fp60_new":
                            machineTIN = Datecs_New.Get_Device_TIN();
                            break;
                        case "datecs_fp_60":
                            machineTIN = Datecs_FP60_Old.Read_TIN();
                            //if (splitted != null && splitted.Count() > 0)
                            //    machineTIN = splitted[0];
                            break;
                        case "daisy_fx_1300":
                            machineTIN = Daisy_Fx1300.Get_TIN();
                            if (machineTIN != null)
                                machineTIN = machineTIN.Substring(0, 10);
                            break;
                        case "galeb_gp_200":
                            splitted = GP_200.Read_ECR_Information();
                            if (splitted != null && splitted.Count() > 0)
                                machineTIN = splitted[0];
                            break;
                        case "bmc_th34ej":
                            splitted = BMC.GetTINAndMRC();
                            if (splitted != null && splitted.Count() > 0)
                                machineTIN = splitted[0];
                            break;
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Failed To Check POS License MRC and TIN !\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (string.IsNullOrEmpty(machineTIN))
                {
                    XtraMessageBox.Show("Failed To Read TIN From Fiscal Printer!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin != machineTIN)
                {
                    XtraMessageBox.Show("Fiscal Printer TIN Is Not The Same As TIN In The Database!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                try
                {
                    var response = UIProcessManager.Validate_POS_License(CNETConstantes.PMS_POS, machineTIN, POS_Settings.fiscalprinterMRC);
                    if (response == null)
                    {
                        XtraMessageBox.Show("Failed To Check POS License!", "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (!response.Success)
                    {
                        XtraMessageBox.Show("Invalid POS License!" + Environment.NewLine + machineTIN + Environment.NewLine + POS_Settings.fiscalprinterMRC, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Failed To Validate POS License!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Failed To Check POS License!\n" + ex.Message, "CNET ERP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
        #region Save Current Voucher

        private VoucherBuffer SaveCurrentVoucher()
        {
            VoucherBuffer Savedbuffer = null;
            // //Progress_Reporter.Show_Progress("Saving Voucher", "Please Wait..", 1, 6);
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
            voucherbuffer.Voucher.Consignee1 = customerDto.ID;


            voucherbuffer.Voucher.IssuedDate = CurrentTime;
            voucherbuffer.Voucher.Year = CurrentTime.Year;
            voucherbuffer.Voucher.Day = CurrentTime.Day;
            voucherbuffer.Voucher.Month = CurrentTime.Month;
            voucherbuffer.Voucher.IsIssued = true;
            voucherbuffer.Voucher.IsVoid = false;
            voucherbuffer.Voucher.GrandTotal = Convert.ToDecimal(teGrandTotal.Text);
            voucherbuffer.Voucher.Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime);
            voucherbuffer.Voucher.Note = "check_out";
            voucherbuffer.Voucher.OriginConsigneeUnit = LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit;
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
                Referenced = EventId,
                ReferencedVoucherDefn = CNETConstantes.EVENT_VOUCHER,
                Value = voucherbuffer.Voucher.GrandTotal,
            };
            TrBuffer.ReferencedActivity = null;
            voucherbuffer.TransactionReferencesBuffer.Add(TrBuffer);
            //PMSDataLogger.LogMessage("frmFrontOfficePOS", "Setup Activity For Checkout.");
            voucherbuffer.Activity = SetupActivity(CurrentTime, _activityDefCheckout, CNETConstantes.PMS_Pointer);
            voucherbuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
            voucherbuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;

            var data = UIProcessManager.CreateVoucherBuffer(voucherbuffer);
            if (data == null || !data.Success)
            {
                MessageBox.Show("Fail Saving !! " + Environment.NewLine + data.Message, "Error");
                return null;
            }
            else
            {
                return data.Data;
            }
        }


        #endregion
        //**************************** CONSTRUCTOR *********************************//
        public frmFrontOfficePOS()
        {
            InitializeComponent();
            CheckWorkflow();


            EventVoucherSetting.GetCurrentVoucherSetting(CNETConstantes.CHECK_OUT_BILL_VOUCHER);
            //Transaction Type

            List<SystemConstantDTO> paymentList = LocalBuffer.LocalBuffer.SystemConstantDTOBufferList.Where(l => l.Category == CNETConstantes.PAYMENT_METHODS && l.IsActive && l.Id != CNETConstantes.PAYMENTMETHODS_DIRECT_BILL).ToList();
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
            // Payment type
            //lePayment.Properties.Columns.Add(new LookUpColumnInfo("Description", "Payment Methods"));
            lePayment.Properties.DisplayMember = "Description";
            lePayment.Properties.ValueMember = "Id";


            CurrentTime = UIProcessManager.GetServiceTime().Value;
            deDate.EditValue = CurrentTime;

            string VoucherID = GetCurrentId(0);
            if (string.IsNullOrEmpty(VoucherID))
            {
                XtraMessageBox.Show("Unable to generate ID!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            teVoucherNo.EditValue = VoucherID;

            cbeTransactionType.EditValue = "Cash Sales";


        }

        /*************** METHODS **************************/
        #region Methods


        public string GetCurrentId(int generationType)
        {

            string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.CHECK_OUT_BILL_VOUCHER, generationType, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

            if (!string.IsNullOrEmpty(currentVoCode))
            {
                return currentVoCode;
            }
            return "";
        }
        private void frmFrontOfficePOS_Load(object sender, EventArgs e)
        {
            if (EventRequirementList != null)
            {

                ConfigurationDTO config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(x => x.Attribute == "Event Bill From");
                if (config != null && config.CurrentValue == "EventVoucher")
                {
                    beiBillFrom.EditValue = "Event Voucher";
                    beiBillFrom.Enabled = false;
                }
                else if (config != null && config.CurrentValue == "EventRequirementVoucher")
                {
                    beiBillFrom.EditValue = "Event Requirement Voucher";
                    beiBillFrom.Enabled = false;
                }
                else
                    beiBillFrom.EditValue = "Event Voucher";
                // GetVoucherFinal();
                PopulateVoucherFinal();
                var data = UIProcessManager.GetVoucherBufferById(EventId);
                if (data == null || !data.Success)
                {

                    XtraMessageBox.Show("Fail to get Event Voucher data !!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                EventVoucher = data.Data;
                EventOwnerId = EventVoucher.Voucher.Consignee1;
                teConsignee.EditValue = EventOwnerName;


                if (EventOwnerId != null)
                {
                    ConsigneeDTO consignee = LocalBuffer.LocalBuffer.ConsigneeViewBufferList.FirstOrDefault(x => x.Id == EventOwnerId);
                    if (EventOwnerId != null)
                    {
                        customerDto = new Consignee()
                        {
                            ID = consignee.Id,
                            Code = consignee.Code,
                            Name = consignee.FirstName + " " + consignee.SecondName + " " + consignee.ThirdName,
                            TIN = consignee.Tin,
                            IsPerson = string.IsNullOrEmpty(consignee.Tin) ? true :false
                        };
                        if (!string.IsNullOrEmpty(consignee.Tin))
                            teTIN.Text = consignee.Tin;
                    }

                }

            }
        }

        int _activityDefCheckout { get; set; }
        int? _objectstateCheckout { get; set; }

        private bool CheckWorkflow()
        {
            ActivityDefinitionDTO _checkoutSalesWorkFlow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.CHECK_OUT_BILL_VOUCHER).FirstOrDefault();


            if (_checkoutSalesWorkFlow != null)
            {

                _activityDefCheckout = _checkoutSalesWorkFlow.Id;
                _objectstateCheckout = _checkoutSalesWorkFlow.State;
                return true;
            }
            else
            {
                XtraMessageBox.Show("Please define workflow for Check Out Bill Voucher ", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
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


        public void GetVoucherFinal()
        {
            VoucherDTO voucher = new VoucherDTO();
            voucher.Code = teVoucherNo.Text;
            voucher.Definition = CNETConstantes.CHECK_OUT_BILL_VOUCHER;
            voucher.Consignee1 = null;
            voucher.IssuedDate = CurrentTime;
            voucher.Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime);

            liDetailsList = new List<LineItemDetails>();
            if (beiBillFrom.EditValue != null && beiBillFrom.EditValue == "Event Voucher")
                GetLineItemlistByEventVoucher(voucher);
            else
                GetLineItemlistByEventRequirement(voucher);

            //foreach (LineItemDTO lineItem in LineItemlist)
            //{
            //    // VwArticleViewDTO Art = ArticleList.FirstOrDefault(x => x.Id == lineItem.Article);
            //    //note: price is already extracted
            //    LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(new VoucherBuffer() { Voucher = voucher }, lineItem, lineItem.Voucher, null, null, 0, 0, true, false, false, false);
            //    LineItemDetails liDetails = new LineItemDetails()
            //    {
            //        // articleName = Art.Name,
            //        lineItems = liDetail.lineItem,
            //        lineItemValueFactor = liDetail.lineItemValueFactor
            //    };
            //    liDetailsList.Add(liDetails);
            //}
            EventLineItemDisplay = new List<LineItemDisplayDTO>();

            EventLineItemDisplay = liDetailsList.Select(x => new LineItemDisplayDTO()
            {
                ArticleId = x.lineItems.Article,
                ArticleName = x.articleName,
                Quantity = x.lineItems.Quantity,
                UnitPrice = x.lineItems.UnitAmount,
                AddCharge = x.lineItems.AddCharge,
                TaxAmount = x.lineItems.TaxAmount,
                TotalPrice = (x.lineItems.TaxableAmount.HasValue ? x.lineItems.TaxableAmount.Value : 0) + (x.lineItems.TaxAmount.HasValue ? x.lineItems.TaxAmount.Value : 0)

            }).ToList();

            voFinal = new VoucherFinalCalculator().VoucherCalculation(voucher, liDetailsList);

            gcFrontOfficePOS.DataSource = EventLineItemDisplay;

            decimal? taxamt = voFinal.taxTransactions == null ? 0 : voFinal.taxTransactions.Sum(x => x.TaxAmount);

            teVAT.Text = taxamt.Value.ToString("N2");
            tediscount.Text = voFinal.voucher.Discount.ToString("N2");
            teSerCharge.Text = voFinal.voucher.AddCharge.ToString("N2");
            teSubTotal.Text = voFinal.voucher.SubTotal.ToString("N2");
            teGrandTotal.Text = voFinal.voucher.GrandTotal.ToString("N2");

        }

        List<LineItemDetails> liDetailsList { get; set; }



        List<LineItemDTO> LineItemlist { get; set; }
        public List<LineItemDTO> GetLineItemlistByEventRequirement(VoucherDTO voucher)
        {
            LineItemlist = new List<LineItemDTO>();
            EventLineItemDisplay = new List<LineItemDisplayDTO>();

            foreach (EventRequirementView Event in EventRequirementList)
            {
                LineItemDTO CheckLineitem = LineItemlist.FirstOrDefault(x => x.Article == Event.Articleid);

                if (CheckLineitem == null)
                {
                    LineItemDTO li = new LineItemDTO()
                    {
                        Article = Event.Articleid,
                        Quantity = Event.Quantity,
                        UnitAmount = Event.UnitAmt,
                        Tax = Event.Tax,
                        Uom = CNETConstantes.UNITOFMEASURMENTPCS
                    };
                    //LineItemlist.Add(li);
                    LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(new VoucherBuffer() { Voucher = voucher }, li, li.Voucher, null, null, null, null, true, false, false, false);
                    LineItemDetails liDetails = new LineItemDetails()
                    {
                        articleName = Event.ArtName,
                        lineItems = liDetail.lineItem,
                        lineItemValueFactor = liDetail.lineItemValueFactor
                    };
                    liDetailsList.Add(liDetails);

                }
                else
                {
                    //LineItemlist.FirstOrDefault(x => x.Article == Event.Articleid).Quantity += Event.Quantity;

                    LineItemDTO li = LineItemlist.FirstOrDefault(x => x.Article == Event.Articleid);
                    li.Quantity += Event.Quantity;

                    LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(new VoucherBuffer() { Voucher = voucher }, li, li.Voucher, null, null, null, null, true, false, false, false);
                    LineItemDetails liDetails = new LineItemDetails()
                    {
                        articleName = Event.ArtName,
                        lineItems = liDetail.lineItem,
                        lineItemValueFactor = liDetail.lineItemValueFactor
                    };
                    liDetailsList.Add(liDetails);
                }
            }
            return LineItemlist;
        }
        public List<LineItemDTO> GetLineItemlistByEventVoucher(VoucherDTO voucher)
        {
            LineItemlist = new List<LineItemDTO>();

            foreach (EventLineItemData Event in EventLineItemList)
            {
                LineItemDTO CheckLineitem = LineItemlist.FirstOrDefault(x => x.Article == Event.Articleid);

                if (CheckLineitem == null)
                {
                    LineItemDTO li = new LineItemDTO()
                    {
                        Article = Event.Articleid,
                        Quantity = Event.Quantity,
                        UnitAmount = Event.UnitAmt,
                        Tax = Event.Tax,
                        Uom = CNETConstantes.UNITOFMEASURMENTPCS,
                        AddCharge= Event.AdditionalCharge,
                        Discount = Event.Discount
                    };
                  //  LineItemlist.Add(li);

                    LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(new VoucherBuffer() { Voucher = voucher }, li, li.Voucher, null, null, null, null, true, false, false, false);
                    LineItemDetails liDetails = new LineItemDetails()
                    {
                        articleName = Event.ArtName,
                        lineItems = liDetail.lineItem,
                        lineItemValueFactor = liDetail.lineItemValueFactor
                    };
                    liDetailsList.Add(liDetails);
                }
                else
                {
                    LineItemDTO li = LineItemlist.FirstOrDefault(x => x.Article == Event.Articleid);
                    li.Quantity += Event.Quantity;

                    LineItemDetailPMS liDetail = new NewLineItemCaculator().LineItemDetailCalculatorVoucher(new VoucherBuffer() { Voucher = voucher }, li, li.Voucher, null, null, null, null, true, false, false, false);
                    LineItemDetails liDetails = new LineItemDetails()
                    {
                        articleName = Event.ArtName,
                        lineItems = liDetail.lineItem,
                        lineItemValueFactor = liDetail.lineItemValueFactor
                    };
                    liDetailsList.Add(liDetails);
                }
            }
            return LineItemlist;
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
                    MessageBox.Show("Unable to generate ID!", "ERROR");
                    bbiPrint.Enabled = false;
                }
                lePayment.Properties.ReadOnly = false;
            }
            else
            {
                POS_Settings.Voucher_Definition = CNETConstantes.CREDITSALES;
                POS_Settings.Get_POS_Settings(LocalBuffer.LocalBuffer.ConfigurationBufferList);

                teVoucherNo.Text = GetCurrentId(0);
                if (string.IsNullOrEmpty(teVoucherNo.Text))
                {
                    MessageBox.Show("Unable to generate ID!", "ERROR");
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
        public void PopulateVoucherFinal()
        {
            decimal Vatamount = 0;
            if (voFinal.taxTransactions != null && voFinal.taxTransactions.Count > 0)
            {
                TaxTransactionDTO VATTaxTransaction = voFinal.taxTransactions.FirstOrDefault(x => x.Tax == CNETConstantes.VAT);
                if (VATTaxTransaction != null)
                {
                    Vatamount = VATTaxTransaction.TaxAmount == null ? 0 : VATTaxTransaction.TaxAmount.Value;
                }
            }


            teSubTotal.EditValue = voFinal.voucher.SubTotal.ToString("N2");
            teSerCharge.Text = voFinal.voucher.AddCharge.ToString("N2");
            tediscount.Text = voFinal.voucher.Discount.ToString("N2");
            teVAT.Text = Vatamount.ToString("N2");
            teGrandTotal.Text = voFinal.voucher.GrandTotal.ToString("N2");
        }
        private void bbiClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }




        private List<Print_Item> _printItems { get; set; }

        private List<Print_Item> GetPrintItems(List<LineItemDetails> lineItemDetail)
        {
            List<Print_Item> printItemList = new List<Print_Item>();
            foreach (LineItemDetails item in lineItemDetail)
            {

                LineItemValueFactorDTO LineItemDiscount = item.lineItemValueFactor.FirstOrDefault(x => x.IsDiscount.Value);
                LineItemValueFactorDTO LineItemServiceCharge = item.lineItemValueFactor.FirstOrDefault(x => !x.IsDiscount.Value);
                 
                LineItemDisplayDTO EventRequirement = EventLineItemDisplay.FirstOrDefault(x => x.ArticleId == item.lineItems.Article);

                Print_Item pItem = new Print_Item();
                pItem.ID = item.lineItems.Article;
                pItem.Name = EventRequirement.ArticleName;
                if (item.lineItems.UnitAmount == null) pItem.UnitPrice = 0;
                else pItem.UnitPrice = item.lineItems.UnitAmount;
                pItem.UnitPrice = pItem.UnitPrice < 0 ? 0.01m : pItem.UnitPrice;
                pItem.Quantity = item.lineItems.Quantity;
                pItem.taxId = item.lineItems.Tax.Value;

                if (item.lineItems.UnitAmount <= 0)
                {
                    XtraMessageBox.Show(EventRequirement.ArticleName + " Article Price can't be Zero !!", "CNET ERP",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                decimal value = 0;

                if (LineItemDiscount != null && LineItemServiceCharge != null)
                {
                    value = LineItemServiceCharge.Amount.Value - LineItemDiscount.Amount.Value;

                    if (value > 0)
                    {
                        pItem.lineItemValue = value;
                        pItem.isValueDiscount = false;
                    }
                    else if (value < 0)
                    {
                        pItem.lineItemValue = Math.Abs(value);
                        pItem.isValueDiscount = true;
                    }
                }
                else
                if (LineItemDiscount == null && LineItemServiceCharge != null)
                {

                    value = LineItemServiceCharge.Amount.Value;
                    pItem.lineItemValue = value;
                    pItem.isValueDiscount = false;
                }
                else
                if (LineItemDiscount != null && LineItemServiceCharge == null)
                {
                    value = LineItemDiscount.Amount.Value;
                    pItem.lineItemValue = value;
                    pItem.isValueDiscount = true;
                }

                //pItem.DiscountAmount = LineItemDiscount == null ? 0 : LineItemDiscount.Amount;
                //pItem.ServiceChargeAmount = LineItemServiceCharge == null ? 0 : LineItemServiceCharge.Amount;


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

        #endregion

        private void beiBillFrom_EditValueChanged(object sender, EventArgs e)
        {
            if(beiBillFrom.EditValue != null)
            {
                GetVoucherFinal();
            }
        }
    }
}