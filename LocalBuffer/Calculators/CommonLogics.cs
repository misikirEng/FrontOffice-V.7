using ProcessManager;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET_V7_Domain.Misc.PmsDTO;
using CNET_V7_Domain.Misc.PmsView;
using DevExpress.XtraEditors; 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNET_V7_Domain.Misc; 
using DevExpress.Utils;
using System.Windows.Forms;

namespace CNET.FrontOffice_V._7.PMS.Common_Classes
{
    public class CommonLogics
    {
        public static bool Validate_PMSPOS_License()
        {
            try
            {
                //FiscalPrinters.GetInstance();
                string[] splitted;
                string machineTIN = "";
                switch (POS_Settings.printerType.ToLower())
                {
                    case "datecs_fmp_350":
                    case "datecs_fp_700":
                    case "datecs_fp60_new":
                        machineTIN = Datecs_New.Get_Device_TIN();
                        break;
                    case "datecs_fp_60":
                        splitted = FiscalPrinters.datecs.GetTinAndMrc();
                        if (splitted != null && splitted.Count() > 0)
                            machineTIN = splitted[0];
                        break;
                    case "daisy_fx_1300":
                        machineTIN = Daisy_Fx1300.Get_TIN();
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

                if (string.IsNullOrEmpty(machineTIN))
                {
                    XtraMessageBox.Show("Failed To Read TIN From Fiscal Printer!", "CNET ERP V7", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (LocalBuffer.LocalBuffer.CompanyConsigneeData.Tin.Substring(0, 10) != machineTIN.Substring(0, 10))
                {
                    XtraMessageBox.Show("Fiscal Printer TIN Is Not The Same As TIN In The Database!", "CNET ERP V7", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                var response = UIProcessManager.Validate_POS_License(CNETConstantes.PMS_POS, machineTIN, POS_Settings.fiscalprinterMRC);

                if (response == null)
                {
                    XtraMessageBox.Show("Failed To Check POS License!", "CNET ERP V7", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!response.Success)
                {
                    XtraMessageBox.Show("Invalid POS License!", "CNET ERP V7", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Failed To Check POS License!\n" + ex.Message, "CNET ERP V7", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public static TaxDTO GetApplicableTax(int regVoucher, int voucherDef, int? guestCode, int? articleCode)
        {

            VoucherDTO oc = UIProcessManager.GetVoucherById(regVoucher);

            int? companyCode = oc.Consignee2 == null ? null : oc.Consignee2;


            TaxDTO tax = null;
            TaxDTO taxToReturn = new TaxDTO()
            {
                Id = -1,
                Category = 0,
                Description = "",
                Amount = 0,
                Remark = ""
            };

            ConfigurationDTO config = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c =>
                c.Reference == voucherDef.ToString() && c.Attribute.ToLower() == "tax priority");
            if (config == null)
            {
                taxToReturn.Remark = "Unable to get tax priority setting! voucher Def: " + voucherDef;
                return taxToReturn;

            }
            if (config.CurrentValue.ToLower() == "voucher")
            {
                ConfigurationDTO voucherTaxType = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c =>
                c.Reference == voucherDef.ToString() && c.Attribute.ToLower() == "voucher tax type");
                if (voucherTaxType != null)
                {
                    tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.Description == voucherTaxType.CurrentValue);
                }

                if (tax != null)
                {
                    taxToReturn.Id = tax.Id;
                    taxToReturn.Category = tax.Category;
                    taxToReturn.Description = tax.Description;
                    taxToReturn.Amount = tax.Amount;
                    taxToReturn.Remark = null;
                }
                else
                {
                    taxToReturn.Remark = "Voucher's Tax Type is not defined! Voucher Def: " + voucherDef;
                }

                return taxToReturn;
            }
            else if (config.CurrentValue.ToLower() == "consignee")
            {
                taxToReturn.Remark = "Voucher Tax Priority should be set to Voucher not Consignee Please Fix Your Setting";
                /*
                if (!string.IsNullOrEmpty(companyCode))
                {
                    GSLTax gslTax = LocalBuffer.LocalBuffer.GSLTaxBufferList.FirstOrDefault(g => g.reference == companyCode);
                    if (gslTax != null)
                    {
                        tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.code == gslTax.tax);

                    }
                }
                else if (!string.IsNullOrEmpty(guestCode))
                {
                    GSLTax gslTax = LocalBuffer.LocalBuffer.GSLTaxBufferList.FirstOrDefault(g => g.reference == guestCode);
                    if (gslTax != null)
                    {
                        tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.code == gslTax.tax);

                    }
                }
                

                //if tax for both consignee is not defined, take the vouchers tax type
                if (tax == null)
                {
                    Configuration voucherTaxType = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c =>
                    c.reference == voucherDef.ToString() && c.attribute.ToLower() == "voucher tax type");
                    if (voucherTaxType != null)
                    {
                        tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.description == voucherTaxType.currentValue);
                    }
                }

                if (tax != null)
                {
                    taxToReturn.code = tax.code;
                    taxToReturn.category = tax.category;
                    taxToReturn.description = tax.description;
                    taxToReturn.amount = tax.amount;
                    taxToReturn.remark = null;
                }
                else
                {
                    taxToReturn.remark = "Consignee tax type is not defined! Voucher Def: " + voucherDef;
                }
                */
                return taxToReturn;
            }
            else if (config.CurrentValue.ToLower() == "article")
            {
                if (articleCode != null && articleCode > 0)
                {
                    ArticleDTO articleddt = UIProcessManager.GetArticleById(articleCode.Value);
                    if (articleddt != null)
                    {
                        if (articleddt.DefaultTax != null)
                        {

                            tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.Id == articleddt.DefaultTax);
                        }
                        else
                        {
                            GsltaxDTO gslTax = UIProcessManager.GetGSLTaxByReference(articleCode.Value);
                            if (gslTax != null)
                            {
                                tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.Id == gslTax.Tax);

                            }

                        }

                    }
                    else
                    {
                        taxToReturn.Remark = "Article Can't be found " + articleCode.Value;

                    }

                }

                if (tax != null)
                {
                    taxToReturn.Id = tax.Id;
                    taxToReturn.Category = tax.Category;
                    taxToReturn.Description = tax.Description;
                    taxToReturn.Amount = tax.Amount;
                    taxToReturn.Remark = null;
                }
                else
                {
                    taxToReturn.Remark = "Article Tax Type is not defined! Voucher Def: " + voucherDef;
                }

                return taxToReturn;
            }
            else
            {
                //take the voucher's tax type
                ConfigurationDTO voucherTaxType = LocalBuffer.LocalBuffer.ConfigurationBufferList.FirstOrDefault(c =>
                c.Reference == voucherDef.ToString() && c.Attribute.ToLower() == "voucher tax type");
                if (voucherTaxType != null)
                {
                    tax = LocalBuffer.LocalBuffer.TaxBufferList.FirstOrDefault(t => t.Description == voucherTaxType.CurrentValue);
                }


                if (tax != null)
                {
                    taxToReturn.Id = tax.Id;
                    taxToReturn.Category = tax.Category;
                    taxToReturn.Description = tax.Description;
                    taxToReturn.Amount = tax.Amount;
                    taxToReturn.Remark = null;
                }
                else
                {
                    taxToReturn.Remark = "Voucher's Tax Type is not defined! Voucher Def: " + voucherDef;
                }

                return taxToReturn;

            }



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
                    .FirstOrDefault(r => r.Currency == currency);
                if (CNETLibrary != null)
                    exchangeRate =
                        CNETLibrary
                            .Buying;
            }
            return exchangeRate;
        }

       


        public static void ChargeAtCheckin(int regCode, DateTime date, int rateHeader, int? consignee, int Consigneeunit, bool showDialog = true)
        {
            try
            {
                //get applicable tax
                int? accArticle = null;
                var rateCodeHeader = UIProcessManager.GetRateCodeHeaderById(rateHeader);
                if (rateCodeHeader != null)
                {
                    accArticle = rateCodeHeader.Article;
                }
                else
                {
                    return;
                }

                if (accArticle == null)
                {
                    return;
                }

                TaxDTO tax = GetApplicableTax(regCode, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, consignee== null?null:consignee.Value, accArticle.Value);
                if (!string.IsNullOrEmpty(tax.Remark))
                {
                    return;
                }

                List<DailyRoomChargeDTO> dailyRoomChargeList = new List<DailyRoomChargeDTO>();
                DailyRoomChargeDTO dailyRoomCharge = UIProcessManager.GetDailyRoomChargePostingByRegistration(regCode, date, CNETConstantes.REGISTRATION_VOUCHER, CNETConstantes.CHECKED_IN_STATE, tax,null,null);
                if (dailyRoomCharge != null)
                {
                    dailyRoomChargeList.Add(dailyRoomCharge);
                }

                // check In Charge
                foreach (LineItemDetails itemD in dailyRoomCharge.lineItemList)
                {
                    if (itemD.lineItems.UnitAmount < 0)
                    {
                        SystemMessage.ShowModalInfoMessage("Couldn't Room Charge b/c " + itemD.articleName + " Price Can't be less than Zero !!" + Environment.NewLine + "Please Check your rate and package !!", "ERROR");
                       ////CNETInfoReporter.Hide();
                        return;
                    }
                }
                if (dailyRoomChargeList.Count > 0)
                {


                    PostRoomCharge(dailyRoomChargeList, date, LocalBuffer.LocalBuffer.CurrentDevice, Consigneeunit, false, showDialog);
                }

            }
            catch (Exception ex)
            {

            }
        }
        public static void ChargeAtEarlyCheckin(int regCode, DateTime date, int consignee, int ConsigneeUnit, int? EarlyCheckInArticle, bool showDialog = true)
        {
            try
            { 
                if (EarlyCheckInArticle != null && EarlyCheckInArticle>0)
                {
                    return;
                }

                TaxDTO tax = GetApplicableTax(regCode, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, consignee, EarlyCheckInArticle.Value);
                if (!string.IsNullOrEmpty(tax.Remark))
                {
                    return;
                }

                List<DailyRoomChargeDTO> dailyRoomChargeList = new List<DailyRoomChargeDTO>();
                DailyRoomChargeDTO dailyRoomCharge = UIProcessManager.GetDailyRoomChargePostingByRegistration( regCode, date, CNETConstantes.REGISTRATION_VOUCHER, CNETConstantes.CHECKED_IN_STATE, tax, null, EarlyCheckInArticle);
                if (dailyRoomCharge != null)
                {
                    dailyRoomChargeList.Add(dailyRoomCharge);
                }

                // check In Charge
                foreach (LineItemDetails itemD in dailyRoomCharge.lineItemList)
                {
                    if (itemD.lineItems.UnitAmount < 0)
                    {
                        SystemMessage.ShowModalInfoMessage("Couldn't Room Charge b/c " + itemD.articleName + " Price Can't be less than Zero !!" + Environment.NewLine + "Please Check your rate and package !!", "ERROR");
                       ////CNETInfoReporter.Hide();
                        return;
                    }
                }
                if (dailyRoomChargeList.Count > 0)
                {
                    PostEarlyCheckinRoomCharge(dailyRoomChargeList, date, LocalBuffer.LocalBuffer.CurrentDevice, ConsigneeUnit, showDialog);
                }

            }
            catch (Exception ex)
            {

            }
        }
        public static bool PostEarlyCheckinRoomCharge(List<DailyRoomChargeDTO> allDailyRoomCharges, DateTime currentTime, DeviceDTO device, int organizationunitdef, bool showDialog = true)
        {
            try
            {
                bool isSavingS = true;
                string regVoConcat = "";
                if (allDailyRoomCharges == null || allDailyRoomCharges.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("nothing to post!", "ERROR");
                    return false;
                }

               // Progress_Reporter.Show_Progress("Saving room charge posting...");

                //check workflow
                ActivityDefinitionDTO workflow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED,CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER).FirstOrDefault();

                if (workflow == null)
                {
                   ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please Define Workflow of PREPARED for Daily Room Charge Voucher", "ERROR");
                    return false;
                }

                //check workflow
                ActivityDefinitionDTO workflowRegVoucher = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOMCHARGE,CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workflowRegVoucher == null)
                {
                   ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please Define Workflow of ROOM CHARGE MADE  for Registration Voucher", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(workflow.Id).FirstOrDefault(r => r.Role == userRoleMapper.Role );//  && r.NeedsPassCode);
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


                int totalTask = allDailyRoomCharges.Count;
                int count = 0;
                foreach (DailyRoomChargeDTO dr in allDailyRoomCharges)
                {
                    count = count + 1;

                   // Progress_Reporter.Show_Progress("Saving room charge posting", string.Format("{0} of {1}", count, totalTask), count, totalTask);


                    if (dr.lineItemList.Count == 0)
                    {
                        XtraMessageBox.Show("No Line Item is detected. Posting process is aborted.", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    List<int> dList = UIProcessManager.GetDailyRoomVoucherByReg(dr.registrationId,dr.dailyRoomChargeVoucher.IssuedDate);
                  
                    if ((dList == null || dList.Count == 0))
                    {
                        List<LineItemDetails> LineItemDetailses = new List<LineItemDetails>();
                      
                        VoucherDTO vo = dr.dailyRoomChargeVoucher;
 
                       string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER,0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value,  false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                        if (!string.IsNullOrEmpty( currentVoCode))
                        {
                            vo.Code = currentVoCode;
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                            isSavingS = false;
                            return false;
                        }

                        LineItemDetailses = dr.lineItemList;
                        VoucherFinalDTO voucherFinal = dr.VoFinal;


                        ArticleDTO accArticle = null;
                        foreach (var li in LineItemDetailses)
                        {
                            ArticleDTO a = UIProcessManager.GetArticleById(li.lineItems.Article);
                            if (a.Preference == LocalBuffer.LocalBuffer.ACCOMODATION_PREFERENCE_CODE)
                            {
                                accArticle = a;
                                break;
                            }
                        }

                        if (accArticle == null)
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to find accomodation article", "ERROR");
                            isSavingS = false;
                            return false;
                        }

                        TaxDTO tax = null;
                        tax = CommonLogics.GetApplicableTax(dr.registrationId, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, voucherFinal.voucher.Consignee1, accArticle.Id);

                        if (!string.IsNullOrEmpty(tax.Remark))
                        {
                            SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                            isSavingS = false;
                            return false;
                        }

                        if (voucherFinal != null && voucherFinal.voucher != null)
                            vo.GrandTotal = voucherFinal.voucher.GrandTotal;

                        vo.LastState = workflow.State.Value;
                        vo.OriginConsigneeUnit = organizationunitdef;
                        // vo.code = UIProcessManager.GetCurrentIdByDevice("Voucher", device, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER.ToString(), CNETConstantes.VOUCHER_COMPONENET);
                        VoucherBuffer voucherBuffer = new VoucherBuffer();
                        voucherBuffer.Voucher = vo;
                        voucherBuffer.TaxTransactions = new List<TaxTransactionDTO>();
                        if (voucherFinal.taxTransactions != null)
                        { 
                            TaxTransactionDTO _taxTransaction = new TaxTransactionDTO();
                            foreach (var taxTran in voucherFinal.taxTransactions)
                            {
                                _taxTransaction = new TaxTransactionDTO();
                                _taxTransaction = taxTran;
                                _taxTransaction.Tax = tax.Id;
                                _taxTransaction.Voucher = vo.Id;
                                voucherBuffer.TaxTransactions.Add(_taxTransaction);
                             }
                        }

                        //if(voucherFinal.voucherValues != null)
                        //{
                        //    voucherFinal.voucher.SubTotal = 0;
                        //    voucherFinal.voucher.Discount = 0;
                        //    voucherFinal.voucher.AdditionalCharge = 0; 
                        //}


                        voucherBuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
                        TransactionReferenceBuffer TRBuffer = new TransactionReferenceBuffer();
                        dr.transactionReference.Referring = vo.Id;
                        dr.transactionReference.Value = 0;
                        dr.transactionReference.RelationType = CNETConstantes.DEFAULT_WINDOW;
                        TRBuffer.TransactionReference = dr.transactionReference;
                        TRBuffer.ReferencedActivity =null;

                        voucherBuffer.TransactionReferencesBuffer.Add(TRBuffer);
                        voucherBuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                        voucherBuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;
                        voucherBuffer.Voucher.Note = "Early Check-In Charge";

                        voucherBuffer.LineItemsBuffer = new List<LineItemBuffer>();

                        foreach (var items in LineItemDetailses)
                        {
                            LineItemBuffer LineItembuffer = new LineItemBuffer();
                            LineItembuffer.LineItem = items.lineItems;
                            LineItembuffer.LineItem.Voucher = vo.Id;
                            LineItembuffer.LineItem.CalculatedCost = 0;
                            LineItembuffer.LineItem.Tax = tax.Id;
                            LineItembuffer.LineItem.ObjectState = null;
                            if (items.lineItemValueFactor != null)
                            {
                                decimal? add = items.lineItemValueFactor.Where(x => !x.IsDiscount.Value) != null ? items.lineItemValueFactor.Where(x => !x.IsDiscount.Value).ToList().Sum(a => a.Amount) : 0;
                                decimal? dis = items.lineItemValueFactor.Where(x => x.IsDiscount.Value) != null ? items.lineItemValueFactor.Where(x => x.IsDiscount.Value).ToList().Sum(a => a.Amount) : 0;

                                LineItembuffer.LineItem.AddCharge = add;
                                LineItembuffer.LineItem.Discount = dis;
                            }
                            voucherBuffer.LineItemsBuffer.Add(LineItembuffer);
                        }

                        voucherBuffer.Voucher.Remark = dr.transactionReference.Referenced.ToString();
                        voucherBuffer.Activity = ActivityLogManager.SetupActivity( currentTime, workflow.Id,CNETConstantes.PMS_Pointer,"");
                        voucherBuffer.TransactionCurrencyBuffer = null;

                       ResponseModel<VoucherBuffer> isSaved = UIProcessManager.CreateVoucherBuffer(voucherBuffer);
                        if (isSaved == null || !isSaved.Success )
                        {
                            SystemMessage.ShowModalInfoMessage("Fail Saving !! " +Environment.NewLine + isSaved.Message, "Error");
                        }
                        else
                        {
                            isSavingS = false;
                        }

                    }
                    else
                    {
                        regVoConcat += dr.dailyRoomChargeVoucher.IssuedDate.ToString("d") + ",";
                    }


                }
                if (isSavingS)
                {
                    if (showDialog)
                        SystemMessage.ShowModalInfoMessage("Early Check-In charged succuessfully!", "MESSAGE");
                    //  PopulateRoomAndTax(postType);
                  //  MasterPageForm.LoadRoomChargesBuffer();
                }
                else
                {
                    if (showDialog)
                        SystemMessage.ShowModalInfoMessage("Early Check-In is not charged!", "ERROR");
                }

               ////CNETInfoReporter.Hide();
                return isSavingS;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("ERROR! " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
               ////CNETInfoReporter.Hide();
                return false;
            }
        }

        public static bool PostRoomCharge(List<DailyRoomChargeDTO> allDailyRoomCharges, DateTime currentTime, DeviceDTO device, int organizationunitdef, bool isLateCheckout = false, bool showDialog = true)
        {
            try
            {
                bool isSavingS = true;
                string regVoConcat = "";
                if (allDailyRoomCharges == null || allDailyRoomCharges.Count == 0)
                {
                    SystemMessage.ShowModalInfoMessage("nothing to post!", "ERROR");
                    return false;
                }

               // Progress_Reporter.Show_Progress("Saving room charge posting...");
                //check workflow
                ActivityDefinitionDTO workflow = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_PREPARED, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER).FirstOrDefault();

                if (workflow == null)
                {
                   ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please Define Workflow of PREPARED for Daily Room Charge Voucher", "ERROR");
                    return false;
                }

                //check workflow
                ActivityDefinitionDTO workflowRegVoucher = UIProcessManager.GetActivityDefinitionBydescriptionandreference(CNETConstantes.LU_ACTIVITY_DEFINATION_ROOMCHARGE, CNETConstantes.REGISTRATION_VOUCHER).FirstOrDefault();

                if (workflowRegVoucher == null)
                {
                   ////CNETInfoReporter.Hide();
                    SystemMessage.ShowModalInfoMessage("Please Define Workflow of ROOM CHARGE MADE  for Registration Voucher", "ERROR");
                    return false;
                }

                //Check Activity Previlage
                var userRoleMapper = LocalBuffer.LocalBuffer.UserRoleMapperBufferList.FirstOrDefault(r => r.User == LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id);
                if (userRoleMapper != null)
                {
                    var roleActivity = UIProcessManager.GetRoleActivityByactivityDefinition(workflow.Id).FirstOrDefault(r => r.Role == userRoleMapper.Role  && r.NeedsPassCode);
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


                int totalTask = allDailyRoomCharges.Count;
                int count = 0;
                foreach (DailyRoomChargeDTO dr in allDailyRoomCharges)
                {
                    count = count + 1;

                   // Progress_Reporter.Show_Progress("Saving room charge posting", string.Format("{0} of {1}", count, totalTask), count, totalTask);


                    if (dr.lineItemList.Count == 0)
                    {
                        XtraMessageBox.Show("No Line Item is detected. Posting process is aborted.", "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    List<int> dList = UIProcessManager.GetDailyRoomVoucherByReg(dr.registrationId,dr.dailyRoomChargeVoucher.IssuedDate);

                    if (isLateCheckout || (dList == null || dList.Count == 0))
                    {
                        List<LineItemDetails> LineItemDetailses = new List<LineItemDetails>();
                
                        VoucherDTO vo = dr.dailyRoomChargeVoucher;



                        string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER,0, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value,  false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                        if (!string.IsNullOrEmpty(currentVoCode))
                        {
                            vo.Code = currentVoCode;
                        }
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("There is a problem on id setting!!!", "ERROR");
                            isSavingS = false;
                            return false;
                        }

                        LineItemDetailses = dr.lineItemList;
                        VoucherFinalDTO voucherFinal = dr.VoFinal;


                        ArticleDTO accArticle = null;
                        foreach (var li in LineItemDetailses)
                        {
                          ArticleDTO a = UIProcessManager.GetArticleById(li.lineItems.Article);
                            if (a == null)
                            {
                                SystemMessage.ShowModalInfoMessage("Unable to find article", "ERROR");
                                isSavingS = false;
                                return false;
                            }
                            if (a.Preference == LocalBuffer.LocalBuffer.ACCOMODATION_PREFERENCE_CODE)
                            {
                                accArticle = a;
                                break;
                            }
                        }

                        if (accArticle == null)
                        {
                            SystemMessage.ShowModalInfoMessage("Unable to find accomodation article", "ERROR");
                            isSavingS = false;
                            return false;
                        }

                        TaxDTO tax = null;
                        tax = CommonLogics.GetApplicableTax(dr.registrationId, CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, voucherFinal.voucher.Consignee1, accArticle.Id);

                        if (!string.IsNullOrEmpty(tax.Remark))
                        {
                            SystemMessage.ShowModalInfoMessage(tax.Remark, "ERROR");
                            isSavingS = false;
                            return false;
                        }

                        if (voucherFinal != null && voucherFinal.voucher != null)
                            vo.GrandTotal = voucherFinal.voucher.GrandTotal;

                        vo.LastState = workflow.State.Value;

                        VoucherBuffer voucherBuffer = new VoucherBuffer();

                        voucherBuffer.Voucher = vo; 
                        voucherBuffer.TaxTransactions = new List<TaxTransactionDTO>();
                        if (voucherFinal.taxTransactions != null)
                        {
                            TaxTransactionDTO _taxTransaction = new TaxTransactionDTO();
                            foreach (var taxTran in voucherFinal.taxTransactions)
                            {
                                _taxTransaction = new TaxTransactionDTO();
                                _taxTransaction = taxTran;
                                _taxTransaction.Tax = tax.Id;
                                _taxTransaction.Voucher = vo.Id;
                                voucherBuffer.TaxTransactions.Add(_taxTransaction);
                            }
                        }
                        VoucherDTO RegVoucher = UIProcessManager.GetVoucherById(dr.registrationId);
                        if( RegVoucher != null ) 
                        {
                            voucherBuffer.Voucher.OriginConsigneeUnit = RegVoucher.OriginConsigneeUnit;
                        }
                        //if (voucherFinal.voucherValues != null)
                        //{
                        //    voucherFinal.voucher.SubTotal = 0;
                        //    voucherFinal.voucher.Discount = 0;
                        //    voucherFinal.voucher.AdditionalCharge = 0;
                        //}


                        voucherBuffer.TransactionReferencesBuffer = new List<TransactionReferenceBuffer>();
                        TransactionReferenceBuffer TRBuffer = new TransactionReferenceBuffer();
                        dr.transactionReference.Referring = vo.Id;
                        dr.transactionReference.Value = 0;
                        dr.transactionReference.RelationType = CNETConstantes.DEFAULT_WINDOW;
                        TRBuffer.TransactionReference = dr.transactionReference;
                        TRBuffer.ReferencedActivity = null;
                        voucherBuffer.TransactionReferencesBuffer.Add(TRBuffer);
                     
                        if (isLateCheckout)
                            voucherBuffer.Voucher.Note = "Late Check-out";

                        voucherBuffer.LineItemsBuffer = new List<LineItemBuffer>();
                        foreach (var items in LineItemDetailses)
                        {
                            LineItemBuffer LineItembuffer = new LineItemBuffer();
                            LineItembuffer.LineItem = items.lineItems;
                            LineItembuffer.LineItem.Voucher = vo.Id;
                            LineItembuffer.LineItem.CalculatedCost = 0;
                            LineItembuffer.LineItem.Tax = tax.Id;
                            LineItembuffer.LineItem.ObjectState = null;

                            if (items.lineItemValueFactor != null)
                            {
                                decimal? add = items.lineItemValueFactor.Where(x => !x.IsDiscount.Value) != null ? items.lineItemValueFactor.Where(x => !x.IsDiscount.Value).ToList().Sum(a => a.Amount) : 0;
                                decimal? dis = items.lineItemValueFactor.Where(x => x.IsDiscount.Value) != null ? items.lineItemValueFactor.Where(x => x.IsDiscount.Value).ToList().Sum(a => a.Amount) : 0;

                                LineItembuffer.LineItem.AddCharge = add;
                                LineItembuffer.LineItem.Discount = dis;
                            }
                            voucherBuffer.LineItemsBuffer.Add(LineItembuffer);
                        }

                        //voucherBuffer.Voucher.OriginConsigneeUnit = 

                        voucherBuffer.Activity = ActivityLogManager.SetupActivity(currentTime, workflow.Id, CNETConstantes.PMS_Pointer, "");
                        voucherBuffer.Voucher.Remark = dr.transactionReference.Referenced.ToString();


                        voucherBuffer.Voucher.LastUser = LocalBuffer.LocalBuffer.CurrentLoggedInUser.Id;
                        voucherBuffer.Voucher.LastDevice = LocalBuffer.LocalBuffer.CurrentDevice.Id;

                        voucherBuffer.TransactionCurrencyBuffer = null;

                        ResponseModel<VoucherBuffer> isSaved = UIProcessManager.CreateVoucherBuffer(voucherBuffer);
                        if (isSaved != null && isSaved.Success)
                        {
                            //SystemMessage.ShowModalInfoMessage("Room Charge Saved. " + Environment.NewLine + isSaved.Message);
                            //update Id Setting
                            UIProcessManager.IdGenerater("Voucher", CNETConstantes.DAILY_ROOM_CHARGE_VOUCHER, 1, LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value, false, LocalBuffer.LocalBuffer.CurrentDevice.Id);
                            isSavingS = true;

                        } 
                        else
                        {
                            SystemMessage.ShowModalInfoMessage("Fail Saving !! " + Environment.NewLine + isSaved.Message, "Error");
                        }
                        
                    }
                    else
                    {
                        regVoConcat += dr.dailyRoomChargeVoucher.IssuedDate.ToString("d") + ",";
                    }
                }
                if (!isLateCheckout && regVoConcat != "")
                {
                    SystemMessage.ShowModalInfoMessage("This registration already posted for " + regVoConcat.TrimEnd(',') + " date(s)!!!", "ERROR");
                    //  PopulateRoomAndTax(postType);
                }
                else
                {
                    if (isSavingS)
                    {
                        if (showDialog)
                            SystemMessage.ShowModalInfoMessage("Room charged succuessfully!", "MESSAGE");
                        //  PopulateRoomAndTax(postType);
                        //MasterPageForm.LoadRoomChargesBuffer();
                    }
                    else
                    {
                        if (showDialog)
                            SystemMessage.ShowModalInfoMessage("Room is not charged!", "ERROR");
                    }
                }

               ////CNETInfoReporter.Hide();

                return isSavingS;


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("ERROR! " + ex.Message, "CNET_v2016", MessageBoxButtons.OK, MessageBoxIcon.Error);
               ////CNETInfoReporter.Hide();
                return false;
            }
        }

 
        public static bool IsRoomOccupied(int regCode, int? roomId, int roomTypeCode, DateTime startDate, DateTime endDate, DateTime CurrentTime,int consigneeunit)
        {
            try
            {
               // Progress_Reporter.Show_Progress("Validating Room Occupancy...");
                bool flag = false;



                if (roomId != null)
                {
                    RoomDetailDTO rdDetail = UIProcessManager.GetRoomDetailById(roomId.Value);
                    if (rdDetail == null)
                    {
                       ////CNETInfoReporter.Hide();
                        //XtraMessageBox.Show("Unable to get room detail", "CNET ERP v2016", MessageBoxButtons.OK,
                        //                           MessageBoxIcon.Warning);
                        return false;
                    }


                    //    List<VwRegistrationDocumentViewDTO> regDetails = UIProcessManager.GetRegistrationDocumentViewByRoom(rdDetail.code).Where(rd => rd.roomType == rdDetail.roomType).ToList();

                    List<VwRegistrationDocumentViewDTO> regDetails = UIProcessManager.GetRegistrationDocumentViewByStartdateEnddateStateandConsigneeUnit(startDate, endDate, null, consigneeunit);

                    if (regDetails == null)
                    {
                       ////CNETInfoReporter.Hide();
                        XtraMessageBox.Show("Unable to validate room validation for the given date", "CNET ERP v2016", MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                        return false;
                    }



                    var filterd = regDetails.Where(r => r.Date.Value.Date >= CurrentTime.Date &&
                        r.Id != regCode && (roomId == null || r.Room == roomId) &&
                        r.LastState != CNETConstantes.CHECKED_OUT_STATE && r.LastState != CNETConstantes.OSD_CANCEL_STATE && ((
                        startDate.Date >= r.StartDate.Value.Date && startDate.Date < r.EndDate.Value.Date) ||
                        (endDate.Date > r.StartDate.Value.Date && startDate.Date < r.EndDate.Value.Date))
                        ).ToList();



                    if (filterd.Count > 0)
                    {
                        //filter out registrations in the room sharing relations

                        List<RelationDTO> relations = UIProcessManager.SelectAllRelation();
                        RelationDTO regrelation = relations.FirstOrDefault(r => r.RelationType == CNETConstantes.LK_ROOM_SHARE && r.ReferringObject == regCode && r.RelationLevel == 1);
                        if (regrelation != null)
                        {
                            List<RelationDTO> masterroomrelation = relations.Where(x => x.RelationType == CNETConstantes.LK_ROOM_SHARE && x.ReferencedObject == regrelation.ReferencedObject).ToList();
                            List<int> referringsInRelation = masterroomrelation.Select(r => r.ReferringObject).ToList();
                            List<int> referencesInRelation = masterroomrelation.Select(r => r.ReferencedObject).ToList();
                            filterd = filterd.Where(r => !referringsInRelation.Contains(r.Id ) && !referencesInRelation.Contains(r.Id)).ToList();

                        }
                        else
                        {
                            List<RelationDTO> masterroomrelation = relations.Where(x => x.RelationType == CNETConstantes.LK_ROOM_SHARE && x.ReferencedObject == regCode).ToList();
                            if (masterroomrelation != null)
                            {
                                List<int> referringsInRelation = masterroomrelation.Select(r => r.ReferringObject).ToList();
                                List<int> referencesInRelation = masterroomrelation.Select(r => r.ReferencedObject).ToList();
                                filterd = filterd.Where(r => !referringsInRelation.Contains(r.Id) && !referencesInRelation.Contains(r.Id)).ToList();

                            }

                        }


                        /*    .Where(r => (r.referenceObject == regCode || r.referringObject == regCode) && r.relationLevel == "1").ToList();
                       


                        List<Relation> relations = UIProcessManager.SelectAllRelation().Where(r => (r.referenceObject == regCode || r.referringObject == regCode) && r.relationLevel == "1").ToList();
                        if (relations != null && relations.Count > 0)
                        {
                            List<string> referringsInRelation = relations.Select(r => r.referringObject).ToList();
                            List<string> referencesInRelation = relations.Select(r => r.referenceObject).ToList();
                            filterd = filterd.Where(r => !referringsInRelation.Contains(r.code) && !referencesInRelation.Contains(r.code)).ToList();
                        }
                        */
                    }

                    if (filterd.Count > 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }

               ////CNETInfoReporter.Hide();
                return flag;
            }
            catch (Exception ex)
            {
               ////CNETInfoReporter.Hide();
                return false;

            }

        }

 
        public static string GetCustomerNote(int consigneeCode)
        {
            string vNote = null;
            List<VoucherDTO> vochList = UIProcessManager.GetVoucherBydefinitionandconsignee1andremark(CNETConstantes.MESSAGE, consigneeCode, "GuestNote");
            if (vochList != null && vochList.Count > 0)
            {
                vNote = vochList.LastOrDefault().Note;
            }

            return vNote;
        }

        public static bool CreateCustomerNote(int consigneeCode, string note)
        {
            try
            {
                DateTime? CurrentTime = UIProcessManager.GetServiceTime();
                if (CurrentTime == null) return false;
 
                string currentVoCode = UIProcessManager.IdGenerater("Voucher", CNETConstantes.MESSAGE, 0,LocalBuffer.LocalBuffer.CurrentDeviceConsigneeUnit.Value,  false, LocalBuffer.LocalBuffer.CurrentDevice.Id);

                if (string.IsNullOrEmpty(currentVoCode))
                {
                    return false;
                }
                else
                {
                    VoucherDTO vo = new VoucherDTO
                    {

                        Code = currentVoCode,
                        Type = CNETConstantes.TRANSACTIONTYPENORMALTXN,
                        Definition = CNETConstantes.MESSAGE,
                        Consignee1 = consigneeCode,
                        IssuedDate = CurrentTime.Value,
                        Year = CurrentTime.Value.Year,
                        Month = CurrentTime.Value.Month,
                        Day = CurrentTime.Value.Day,
                        IsIssued = true,
                        IsVoid = false,
                        GrandTotal = 0,
                        Period = LocalBuffer.LocalBuffer.GetPeriodCode(CurrentTime.Value),
                        Note = note,
                        Remark = "GuestNote"
                    };

                    UIProcessManager.CreateVoucher(vo);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }
}
