using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.ArticleSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Misc.CommonTypes;
using ProcessManager;
using System.Diagnostics;
using DevExpress.Mvvm.POCO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DocumentPrint.DTO
{
    public class PrintDocumentVoucher
    {
        private bool articleNew = true;
        string Objectstatedefination = "";

        public PrintDocumentVoucher()
        {
        }

        public byte[] logoPic { get; set; }
        public string logoPicUrl { get; set; }
        private bool FirstTime = true;
        public static bool DocumentBrowser = true;
        
        ArticleSpecificationPrint Spec = new ArticleSpecificationPrint();
        LineItemSerialCodePrint SrCode = new LineItemSerialCodePrint();
        List<ArticleSpecificationPrint> Specification = new List<ArticleSpecificationPrint>();
        List<LineItemSerialCodePrint> Serialcode = new List<LineItemSerialCodePrint>();
        List<LineItemConversionValuesPrint> LineItemConversion = new List<LineItemConversionValuesPrint>();
        VoucherValues VoucherValues = new VoucherValues();
        List<JournalDetailObjPrint> JournalDetObj = new List<JournalDetailObjPrint>();
        ConsigneeInformationPrint ConsigneeRecord = new ConsigneeInformationPrint();
        NonCashTransactionInformationPrint NonCashPayment = new NonCashTransactionInformationPrint();
        //  VoucherInformationPrint VoucherInfo = new VoucherInformationPrint();
        public string Language { get; set; }
        public bool IsFirstTime { get; set; }
        public bool Hasendweightzero { get; set; }
        public bool IsOnTab { get; set; }
        public async Task<VoucherPrintModel> PrintLineItemVoucher(VoucherDetailDTO rdatasource)
        {
            VoucherPrintModel voucherPrint = new VoucherPrintModel();
            var NonListDataSource = rdatasource.VoucherHeader;
            var LineItemList = rdatasource.VoucherDetail;
            var activityList = rdatasource.ActivityDetail;
            Dictionary<string, List<LineItemConversionValues>> ConversionObj = new Dictionary<string, List<LineItemConversionValues>>();
           ArticleObjsPrint LineItemObj = new ArticleObjsPrint();
            List<ArticleObjsPrint> ListLineItemObj = new List<ArticleObjsPrint>();
            //var configBuffer = await _sharedHelpers.GetFilterDynamicData<List<ConfigurationDTO>>("Configuration/filter", new Dictionary<string, string>() { { "reference", NonListDataSource.DefinitionId.ToString() } });
            var configBuffer =  DocumentPrintSetting.ConfigurationDTOList.Where(x=> x.Reference == NonListDataSource.DefinitionId.ToString());
            var ConsigneeUnitBuffer = DocumentPrintSetting.CompanyBranchList;

            var taxTransBuffer = UIProcessManager.GetTaxTransactionByVoucher(rdatasource.VoucherHeader.Id);
            
            var rDistribution = DocumentPrintSetting.DistributionList.Where(x=> x.SystemConstant ==  NonListDataSource.DefinitionId && x.Type == 1578);
            
            var systemConstantBuffer = DocumentPrintSetting.SystemConstantList;
            #region companyInformation
            ConsigneeDTO rOrganization = DocumentPrintSetting.CompanyConsigneeDTO;

            voucherPrint.CompanyName = rOrganization?.FirstName;
            //var companyIdentification = rIdentification.Where(x => x.reference == rOrganization?.code).ToList();
            voucherPrint.TINNo = rOrganization.Tin;
            voucherPrint.VATNo = DocumentPrintSetting.CompanyVATNumber;
            //voucherPrint.VATNo = rOrganization.;
            var headOffice = ConsigneeUnitBuffer.Where(x => x.Abrivation == "HO").FirstOrDefault();
            if (headOffice == null) headOffice = new ConsigneeUnitDTO();
            int? country = null;             
            int? subCity = null;      
            int? city = null;
            string wereda = null;

            if(headOffice.Country !=null) country = headOffice.Country;
            if (headOffice.Subcity !=null)  subCity = headOffice.Subcity;
            if (headOffice.Wereda != null)  wereda = headOffice.Wereda;
            if (headOffice.HouseNumber !=null) voucherPrint.CompanyHouseNo = headOffice.HouseNumber;
            if (headOffice.City !=null) city = headOffice.City;

             voucherPrint.CompanyTel = (!string.IsNullOrEmpty(headOffice.Phone1) && !string.IsNullOrEmpty(headOffice.Phone2))  ?
                headOffice.Phone1 +" / " +headOffice.Phone2 : string.IsNullOrEmpty(headOffice.Phone1)? "" : headOffice.Phone1;

            if (headOffice.Website !=null) voucherPrint.CompanyWeb = headOffice.Website;
            if (headOffice.PoBox !=null)  voucherPrint.CompanyPOBox = headOffice.PoBox;
                //  if (headOffice.fax)
                //voucherPrint.CompanyFax = compAdd.value;
                // break;
           if (headOffice.Email !=null) voucherPrint.CompanyEmail = headOffice.Email;
            string mtextToPrint = "";
            try
            {
                if (subCity != null) { mtextToPrint += subCity + ", "; }
                if (wereda != null)
                {
                    mtextToPrint += "Wereda " + wereda + ", ";
                }
                if (voucherPrint.CompanyHouseNo != null)
                {
                    mtextToPrint += voucherPrint?.CompanyHouseNo;
                }
            }
            catch
            {
            }
            voucherPrint.CompanyAddress = mtextToPrint;
            voucherPrint.ConsigneeUnitBuffer = ConsigneeUnitBuffer;
            #endregion
            #region voucher Information and Voucher values
            if (voucherPrint?.DateFormat?.ToLower() == "longdate")
            {
                voucherPrint.IssuedDate = string.Format("{0:G}", NonListDataSource.IssuedDate);
            }
            else if (voucherPrint?.DateFormat?.ToLower() == "mediumdate")
            {
                voucherPrint.IssuedDate = NonListDataSource.IssuedDate.ToString("dd-MMM-yyyy");
            }
            else
            {
                voucherPrint.IssuedDate = NonListDataSource.IssuedDate.ToShortDateString();
            }

            NumberToEnglish numToEng = new NumberToEnglish();
            var currencyBuffer = new List<CurrencyDTO>();
            var currency = new CurrencyDTO();
            if (NonListDataSource.CurrencyDescription !=null) {
                currency.Description = NonListDataSource.CurrencyDescription;
                currency.IsDefault = true;
            }
            else
            {
                currency.Description = "Birr";
                currency.IsDefault = true;
            }
            currencyBuffer.Add(currency);
            VoucherValues voucherValue = new VoucherValues()
            {
                SubTotal = NonListDataSource.SubTotal,
                Remark = NonListDataSource.Remark,
                AdditionalCharge = NonListDataSource.AddCharge,
                GrandTotal = decimal.Parse(String.Format("{0:n2}", NonListDataSource.GrandTotal)),
                GrandTotalInWords = numToEng.changeCurrencyToWords(String.Format("{0:n2}", NonListDataSource.GrandTotal)),//, currencyBuffer),
                Discount = NonListDataSource.Discount,
            };
          
            voucherValue.VoucherTax = taxTransBuffer?.Where(x => x.Voucher == NonListDataSource.Id).ToList();
            decimal WithHoldingTax = 0;
            if (voucherValue != null)
            {

                List<TaxTransactionDTO> taxTransactionList = voucherValue.VoucherTax;
                decimal VAT = 0;
                decimal TaxableVAT = 0;
                var taxBuffer = DocumentPrintSetting.TaxList;
                voucherPrint.TaxBuffer = taxBuffer;
                TaxDTO TypeName = new TaxDTO();
                string taxString = "";
                for (int m = 0; m <= taxTransactionList.Count() - 1; m++)
                {
                    if (taxTransactionList[m].Tax == 6)
                    {
                        WithHoldingTax = Math.Round((decimal)taxTransactionList[m].TaxAmount, 2);

                    }
                    else
                    {
                        TypeName = taxBuffer.Where(x => x.Id == voucherValue.VoucherTax[m].Tax).FirstOrDefault();
                        if (TypeName != null)
                        {
                            taxString = TypeName.Description;
                            if (TypeName.Amount != 0)
                            {
                                taxString += " (" + Math.Round(TypeName.Amount!=null?(decimal)TypeName.Amount:0, 2) + "%)";
                            }

                            VAT = Math.Round(taxTransactionList[m].TaxAmount!=null?(decimal)taxTransactionList[m].TaxAmount:0, 2);
                            TaxableVAT = Math.Round(taxTransactionList[m].TaxableAmount!=null?(decimal)taxTransactionList[m].TaxableAmount:0, 2);
                        }
                    }
                }
        
                voucherPrint.SubTotal = voucherValue.SubTotal;
                voucherPrint.AdditionalCharge = voucherValue.AdditionalCharge;
                voucherPrint.Discount = voucherValue.Discount;
                voucherPrint.taxString = taxString;
                if (TypeName != null)
                {
                    try
                    {
                        voucherPrint.taxTotal = (decimal)(taxTransBuffer.Where(x => x.Voucher == NonListDataSource.Id).FirstOrDefault() != null ? taxTransBuffer.Where(x => x.Voucher == NonListDataSource.Id).FirstOrDefault().TaxableAmount : 0);

                    }
                    catch { voucherPrint.taxTotal = (decimal)0.00; }
                    }
                voucherPrint.voucherValues = voucherValue;
                voucherPrint.GrandTotal = voucherValue.GrandTotal;
                voucherPrint.GrandTotalInWords = voucherValue.GrandTotalInWords;
            }
            if (WithHoldingTax != 0)
            {
                //voucherPrint.withHoldingTax = WithHoldingTax;
            }
            #endregion
            voucherPrint.VoucherId = NonListDataSource.Id;
            voucherPrint.VoucherString = NonListDataSource.Code;
            voucherPrint.IsVoid = NonListDataSource.IsVoid;
            voucherPrint.IsIssued = NonListDataSource.IsIssued;
            voucherPrint.printDialogueDocumentname = DocumentPrintSetting.SystemConstantList?.FirstOrDefault(i =>  i.Id == NonListDataSource.DefinitionId)?.Description;
            voucherPrint.documentname = DocumentPrintSetting.SystemConstantList?.FirstOrDefault(i => i.Id == NonListDataSource.DefinitionId)?.Description;
            voucherPrint.voucherDefinition = NonListDataSource.DefinitionId;

            #region copy and department
            voucherPrint.CopyDescription = new List<string>();
            voucherPrint.OrganizationDepartment = new List<string>();
            voucherPrint.DistrbutionPrinterList = new List<string>();
            if (rDistribution != null)
            {
                DistributionDTO distributionDTO = new DistributionDTO();
                foreach (DistributionDTO objdistribution in rDistribution)
                {
                    voucherPrint.CopyDescription.Add(objdistribution.Description);
                    voucherPrint.DistrbutionPrinterList.Add(objdistribution.Remark);
                    voucherPrint.OrganizationDepartment.Add(objdistribution.Destination != null?ConsigneeUnitBuffer.Where(x=>x.Id==objdistribution.Destination).FirstOrDefault()?.Name:"");
                }
                if (voucherPrint.CopyDescription != null)
                {
                    string coppyDstributionString = "";
                    string CopyDepartmentString = "";
                    var index = 0;
                    bool mOC = false;
                    bool mFC = false;
                    bool mSC = false;
                    bool mTC = false;
                    bool mFrC = false;
                    bool mFiC = false;
                    if (voucherPrint.CopyDescription.Contains("Orignal copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("Orignal copy");
                        coppyDstributionString += "Original Copy -" + voucherPrint.OrganizationDepartment[index];
                    }
                    if (voucherPrint.CopyDescription.Contains("1st copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("1st copy");
                        if (mOC == true)
                        {
                            coppyDstributionString += " , 1st Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 1st copy -" + voucherPrint.OrganizationDepartment[index];
                        }
                        mFC = true;
                    }
                    if (voucherPrint.CopyDescription.Contains("2nd copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("2nd copy");
                        if (mFC == true)
                        {
                            coppyDstributionString += " , 2nd Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 2nd Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        mSC = true;
                    }
                    if (voucherPrint.CopyDescription.Contains("3rd copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("3rd copy");
                        if (mOC == true | mFC == true | mSC == true)
                        {
                            coppyDstributionString += " , 3rd Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 3rd Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        mTC = true;
                    }
                    if (voucherPrint.CopyDescription.Contains("4th copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("4th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true)
                        {
                            coppyDstributionString += " , 4th Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 4th Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        mFrC = true;
                    }
                    if (voucherPrint.CopyDescription.Contains("5th copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("5th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true | mFrC == true)
                        {
                            coppyDstributionString += " , 5th Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += "5th Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        mFiC = true;

                    }
                    if (voucherPrint.CopyDescription.Contains("6th copy"))
                    {
                        index = voucherPrint.CopyDescription.IndexOf("6th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true | mFrC == true | mFiC == true)
                        {
                            coppyDstributionString += "  , 6th Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 6th Copy - " + voucherPrint.OrganizationDepartment[index];
                        }
                    }
                    voucherPrint.CopyDescriptionString = coppyDstributionString;
                }
                if (voucherPrint.OrganizationDepartment != null)
                {
                    #region Watermark

                    voucherPrint.Activities = rdatasource.ActivityDetail;
                    int GetCount = GetPrintCount(rdatasource.ActivityDetail);
                    voucherPrint.IsVoucherPrinted = false; //GetCount.Item1;
                    voucherPrint.PrintCount =  GetCount;
                    string mDistTo = null;
                    if (voucherPrint.PrintWaterMark == "Standard")
                    {
                        if (voucherPrint.IsVoid==true)
                        {

                        }
                        else if (voucherPrint.IsPreview==true)
                        {

                        }
                        else if (voucherPrint.IsIssued!=true)
                        {

                        }
                        else
                        {
                            if (voucherPrint.PrintCount >= 0)
                            {
                                if (voucherPrint.PrintCopyDistribution == true)
                                {
                                    if (voucherPrint.PrintCount == 0)
                                    {
                                        if (voucherPrint.CopyDescription != null)
                                        {
                                            if (voucherPrint.CopyDescription.Count > 0)
                                            {
                                                if (voucherPrint.CopyDescription.Contains("Orignal copy"))
                                                {
                                                    int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("1st copy"));
                                                    mDistTo = voucherPrint.OrganizationDepartment[index];                                                }
                                            }
                                        }
                                    }
                                    if (voucherPrint.PrintCount == 1)
                                    {
                                        if (voucherPrint.CopyDescription != null)
                                        {
                                            if (voucherPrint.CopyDescription.Count > 0)
                                            {
                                                if (voucherPrint.CopyDescription.Contains("1st copy"))
                                                {
                                                    int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("1st copy"));
                                                    mDistTo = voucherPrint.OrganizationDepartment[index];
                                                }
                                            }
                                        }
                                    }
                                    else if (voucherPrint.PrintCount == 2)
                                    {
                                        if (voucherPrint.CopyDescription != null)
                                        {
                                            if (voucherPrint.CopyDescription.Count > 0)
                                            {
                                                if (voucherPrint.CopyDescription.Contains("2nd copy"))
                                                {
                                                    int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("2nd copy"));
                                                    mDistTo = voucherPrint.OrganizationDepartment[index];
                                                }
                                            }
                                        }
                                    }
                                    else if (voucherPrint.PrintCount == 3)
                                    {
                                        if (voucherPrint.CopyDescription != null)
                                        {
                                            if (voucherPrint.CopyDescription.Count > 0)
                                            {
                                                if (voucherPrint.CopyDescription.Contains("3rd copy"))
                                                {
                                                    int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("3rd copy"));
                                                    mDistTo = voucherPrint.OrganizationDepartment[index];
                                                }
                                            }
                                        }
                                    }
                                    else if (voucherPrint.PrintCount == 4)
                                    {
                                        if (voucherPrint.CopyDescription != null)
                                        {
                                            if (voucherPrint.CopyDescription.Count > 0)
                                            {
                                                if (voucherPrint.CopyDescription.Contains("4th copy"))
                                                {
                                                    int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("4th copy"));
                                                    mDistTo = voucherPrint.OrganizationDepartment[index];
                                                }
                                            }
                                        }
                                    }
                                    else if (voucherPrint.PrintCount == 5)
                                    {
                                        if (voucherPrint.CopyDescription != null)
                                        {
                                            if (voucherPrint.CopyDescription.Count > 0)
                                            {
                                                if (voucherPrint.CopyDescription.Contains("5th copy"))
                                                {
                                                    int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("5th copy"));
                                                    mDistTo = voucherPrint.OrganizationDepartment[index];
                                                }
                                            }
                                        }
                                    }
                                    else if (voucherPrint.PrintCount == 6)
                                    {
                                        if (voucherPrint.CopyDescription != null)
                                        {
                                            if (voucherPrint.CopyDescription.Count > 0)
                                            {
                                                if (voucherPrint.CopyDescription.Contains("6th copy"))
                                                {
                                                    int index = voucherPrint.CopyDescription.FindIndex(x => x.StartsWith("6th copy"));
                                                    mDistTo = voucherPrint.OrganizationDepartment[index];
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    voucherPrint.OrganizationDepartmentString = mDistTo;
                    #endregion
                }
            }
            #endregion




            #region print setting
            string attchmentUl = null;
            voucherPrint.attachmentPath = attchmentUl;
            //voucherPrint.IsForced = Isforced;
            var voucherDefinition = NonListDataSource.Definition.ToString();
            voucherPrint.Type = "Template Type 1";
            List<ConfigurationDTO> value =  configBuffer.ToList();
            foreach (var va in value)
            {
                voucherPrint.PrintValues = "All";
                switch (va.Attribute.ToString())
                {
                    case "Default Printer":
                        voucherPrint.defaultPrinter = va.CurrentValue.ToString();
                        break;
                    case "Print Quantity Sum":
                        voucherPrint.PrintQuantitySum = bool.Parse(va.CurrentValue);
                        break;
                    case "Date Format":
                        voucherPrint.DateFormat = va.CurrentValue.ToString();
                        break;
                    case "Merge Item code and Description":
                        voucherPrint.MergreItemCodeAndDescription = bool.Parse(va.CurrentValue.ToString());
                        break;
                    case "Max Line Item":
                        voucherPrint.MaxLineItem = uint.Parse(va.CurrentValue.ToString());
                        break;
                    case "Print Seasonal message":
                        voucherPrint.PrintSeasonalmessage = bool.Parse(va.CurrentValue.ToString());
                        break;
                    case "Max No Of Printing":
                        voucherPrint.MaxNoOfPrinting = uint.Parse(va.CurrentValue.ToString());
                        break;
                    case "No Of Line Item Per Page":
                        voucherPrint.NoOfLineItemPerPage = uint.Parse(va.CurrentValue.ToString());
                        break;
                    case "Voucher Orientation":
                        voucherPrint.VoucherOrientation = va.CurrentValue.ToString();
                        break;
                    case "Print LineItem Conversion":
                        voucherPrint.EnableQtyConversion = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Remote Distribution":
                        voucherPrint.PrintRemoteDistribution = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Copy Distribution":
                        voucherPrint.PrintCopyDistribution = bool.Parse(va.CurrentValue.ToLower());
                        break;
                    case "Print Catalogue Automatically":
                        voucherPrint.PrintCatalogueAutomatically = bool.Parse(va.CurrentValue.ToString());
                        break;
                    case "Print Article Code":
                        voucherPrint.PrintArticleCode = bool.Parse(va.CurrentValue.ToString());
                        break;
                    case "Print Article Picture":
                        voucherPrint.PrintArticlePicture = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Article Volume":
                        voucherPrint.PrintArticleVolume = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Journal":
                        voucherPrint.PrintJournal = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Sum. Art. Phy. dim.":
                        voucherPrint.PrintSum = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Specification":
                        voucherPrint.PrintSpecification = va.CurrentValue.ToString();
                        break;
                    case "Print Values":
                        voucherPrint.PrintValues = va.CurrentValue.ToString();
                        break;
                    case "Paper Size":
                        //  voucherPrint.PaperSize = "A4";
                        voucherPrint.PaperSize = va.CurrentValue.ToString();
                        break;
                    case "Print Immediate Reference":
                        voucherPrint.PrintimmediateReference = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Ancestor Reference":
                        voucherPrint.PrintAncestorReference = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Ancestor Extension":
                        voucherPrint.PrintAncestorExtension = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Water mark":
                        voucherPrint.PrintWaterMark = va.CurrentValue.ToString();
                        break;
                    case "Print Without Preview":
                        voucherPrint.PrintWithoutPreview = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Amount In Word":
                        voucherPrint.PrintAmountInWord = bool.Parse(va.CurrentValue);
                        break;
                    case "Sort Line Item":
                        voucherPrint.SortLineItem = va.CurrentValue.ToString();
                        break;
                    case "No Of Copies":
                        voucherPrint.NoOfCopies = short.Parse(va.CurrentValue.ToString());
                        break;
                    case "Paper Type":
                        voucherPrint.PaperType = va.CurrentValue.ToString();
                        break;
                    case "Use Darker Lines":
                        voucherPrint.UseDarkerLines = bool.Parse(va.CurrentValue.ToString());
                        break;
                    case "Enable Payment Option":
                        voucherPrint.EnablePaymentOptions = bool.Parse(va.CurrentValue.ToString());
                        break;
                    case "Voucher User Orientation":
                        voucherPrint.VoucherUserOrientation = va.CurrentValue.ToString();
                        break;
                    case "Top":
                        if (string.IsNullOrEmpty(va.CurrentValue))
                        {
                            voucherPrint.Top = 0;
                        }
                        else
                        {
                            voucherPrint.Top = Convert.ToInt16(va.CurrentValue);
                        }
                        break;
                    case "Bottom":
                        if (string.IsNullOrEmpty(va.CurrentValue))
                        {
                            voucherPrint.Bottom = 0;
                        }
                        else
                        {
                            voucherPrint.Bottom = Convert.ToInt16(va.CurrentValue);
                        }
                        break;
                    case "Left":
                        if (string.IsNullOrEmpty(va.CurrentValue))
                        {
                            voucherPrint.Left = 0;
                        }
                        else
                        {
                            voucherPrint.Left = Convert.ToInt16(va.CurrentValue);
                        }
                        break;
                    case "Right":
                        if (string.IsNullOrEmpty(va.CurrentValue))
                        {
                            voucherPrint.Right = 0;
                        }
                        else
                        {
                            voucherPrint.Right = Convert.ToInt16(va.CurrentValue);
                        }
                        break;
                    case "Type":
                        voucherPrint.Type = va.CurrentValue.ToString();
                        break;
                    case "Round Digit Total":
                        voucherPrint.RoundDigitTotal = "N" + Convert.ToInt32(va.CurrentValue);
                        voucherPrint.IntRoundTotalDigit = Convert.ToInt32(va.CurrentValue);
                        break;
                    case "Round Digit Quantity":
                        voucherPrint.RoundDigitQuantity = "N" + Convert.ToInt32(va.CurrentValue);
                        break;
                    case "Round Digit Unit Price":
                        voucherPrint.RoundDigitUnitPrice = "N" + Convert.ToInt32(va.CurrentValue);
                        break;
                    case "Enable Weight Bridge":
                        voucherPrint.EnableBridgeWeight = bool.Parse(va.CurrentValue);
                        break;
                    case "Enable Term":
                        voucherPrint.EnableTerm = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Bank Info":
                        voucherPrint.PrintBankInfo = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Batch":
                        voucherPrint.PrintBatch = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Expiry Date":
                        voucherPrint.PrintExpiryDate = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Production Date":
                        voucherPrint.PrintProductionDate = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Consignee Code":
                        voucherPrint.PrintConsigneeCode = bool.Parse(va.CurrentValue);
                        break;
                    case "Print Activity Reference":
                        voucherPrint.PrintReferenceActivity = va.CurrentValue;
                        break;
                    case "Template Document":
                        voucherPrint.TemplateDocument = va.CurrentValue;
                        break;

                }
            }
            #endregion

            List<OtherConsigneeDetail> OtherConsList = new List<OtherConsigneeDetail>();

            try
            {
                 if (NonListDataSource.Consignee1Id != null)
                {
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = (int)NonListDataSource.Consignee1Id,
                        consigneeFullName = NonListDataSource.Consignee1FullName,
                        requiredGSlDesc = NonListDataSource.Consignee1PrefDesc,
                        consigneTin = NonListDataSource.Tin,
                    };
                    OtherConsList.Add(Cons);
                }  
                if (NonListDataSource.Consignee2Id != null)
                {
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = (int)NonListDataSource.Consignee2Id,
                        consigneeFullName = NonListDataSource.Consignee2FullName,
                    };
                    OtherConsList.Add(Cons);
                }
                if (NonListDataSource.Consignee3Id != null)
                {
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = (int)NonListDataSource.Consignee3Id,
                        consigneeFullName = NonListDataSource.Consignee3FullName,
                    };
                    OtherConsList.Add(Cons);
                }  
                if (NonListDataSource.Consignee4Id != null)
                {
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = (int)NonListDataSource.Consignee4Id,
                        consigneeFullName = NonListDataSource.Consignee4FullName,
                    };
                    OtherConsList.Add(Cons);
                }    
                if (NonListDataSource.Consignee5Id != null)
                {
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = (int)NonListDataSource.Consignee5Id,
                        consigneeFullName = NonListDataSource.Consignee5FullName,
                    };
                    OtherConsList.Add(Cons);
                }   
                if (NonListDataSource.Consignee6Id != null)
                {
                    var Cons = new OtherConsigneeDetail()
                    {
                        consignee = (int)NonListDataSource.Consignee6Id,
                        consigneeFullName = NonListDataSource.Consignee6FullName,
                    };
                    OtherConsList.Add(Cons);
                }
            }
            catch { }
            voucherPrint.OtherConsigneeDetail = OtherConsList;
                if (LineItemList != null)
                {
                    ArticleSpecification Spec = new ArticleSpecification();
                   LineItemSerialCode SrCode = new LineItemSerialCode();
                   LineItemConversionValues LICv = new LineItemConversionValues();
                    List<ArticleSpecification> Specification = new List<ArticleSpecification>();
                    List<LineItemSerialCode> Serialcode = new List<LineItemSerialCode>();
                    List<LineItemConversionValues> LineItemConversion = new List<LineItemConversionValues>();
                    Dictionary<string, List<ArticleSpecification>> SpecObj = new Dictionary<string, List<ArticleSpecification>>();
                    Dictionary<string, List<LineItemSerialCode>> SerialObj = new Dictionary<string, List<LineItemSerialCode>>();
                foreach (var objectADD in LineItemList)
                {
                    if (objectADD !=null )
                    {
                        if (ConversionObj.ContainsKey(objectADD.LineItemId.ToString()))
                        {
                            LineItemConversion = ConversionObj[objectADD.LineItemId.ToString()];
                            if (LineItemConversion.Any(c => c.code == objectADD.LineItemId.ToString()))
                            {
                                LICv = new LineItemConversionValues();
                                LICv.code = objectADD.LineItemId.ToString();
                                LICv.uom = objectADD?.Uom?.ToString();
                                LICv.UnitAmount = objectADD.UnitAmount!=null?(decimal)objectADD.UnitAmount:0;
                                LICv.Quantity = objectADD.Quantity!=null?(double)objectADD.Quantity:0;
                                LICv.UOMLookupDescription = systemConstantBuffer.Where(x => x.Id == objectADD.Uom)?.FirstOrDefault()?.Description;
                                LineItemConversion.Add(LICv);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(objectADD.LineItemId.ToString()))
                            {
                                LineItemConversion = new List<LineItemConversionValues>();
                                LICv = new LineItemConversionValues();
                                LICv.code = objectADD.LineItemId.ToString();
                                LICv.uom = objectADD.Uom?.ToString();
                                LICv.UnitAmount = objectADD.UnitAmount != null ? (decimal)objectADD.UnitAmount : 0;
                                LICv.Quantity = objectADD.Quantity != null ? (double)objectADD.Quantity : 0;
                                LICv.UOMLookupDescription = systemConstantBuffer.Where(x => x.Id == objectADD.Uom)?.FirstOrDefault()?.Description; ;
                                LineItemConversion.Add(LICv);
                                ConversionObj.Add(objectADD.LineItemId.ToString(), LineItemConversion);
                                LineItemObj.LineItemConversion = ConversionObj;
                            }
                        }

                        if (!ListLineItemObj.Any(l => l.LineItemCode == objectADD.LineItemId.ToString()))
                        {


                            LineItemObj.LineItemCode = objectADD.LineItemId.ToString();


                            LineItemObj.Article = objectADD.ArticleCode;


                            LineItemObj.Description = objectADD.ArticleName;


                            LineItemObj.Quantity = Math.Round(objectADD.Quantity!=null?(decimal) objectADD.Quantity:0,2);


                            LineItemObj.UnitAmnt = objectADD.UnitAmount!=null?(decimal)objectADD.UnitAmount:0;

                            LineItemObj.UOM = systemConstantBuffer.Where(x => x.Id == objectADD?.Uom)?.FirstOrDefault()?.Description;


                            LineItemObj.TotalAmount = objectADD.TotalAmount!=null? Convert.ToDecimal(objectADD.TotalAmount):0;
                            ListLineItemObj.Add(LineItemObj);
                            LineItemObj = new ArticleObjsPrint();
                        }
                    }
                } 

            }
            var sn = 1;
            if (ListLineItemObj?.Count>0)
            {
                foreach (var lineItem in ListLineItemObj)
                {
                    lineItem.sn = sn;
                    lineItem.Quantity = Math.Round(lineItem.Quantity,2 );
                    sn++;
                }
              
            }
            voucherPrint.OtherConsigneeDetail = OtherConsList;
            voucherPrint.ListLineItemObj = ListLineItemObj;
            voucherPrint.JournalDetObj = JournalDetObj;
             if (activityList != null)
            {
                List<string> dateList = new List<string>();
                List<string> opList = new List<string>();
                List<string> actList = new List<string>();
                foreach (var val in activityList)
                {
                    //if (val.isPrint.HasValue && val.isPrint.Value)
                    //{
                    opList.Add(val.UserName);
                    string dt = val.ActivityDate!=null? val.ActivityDate?.ToString("dd-MM-yyyy"):"";
                    dateList.Add(dt);
                    actList.Add(val.ActivityDefDesc != null ? val.ActivityDefDesc : "");
                    // }

                }
                voucherPrint.Voucheroperators = opList;
                voucherPrint.ActivityDate = dateList;
                voucherPrint.ActivityDefDesc = actList;
                bool isPrintActivitiyReferenceSet = (!string.IsNullOrEmpty(voucherPrint.PrintReferenceActivity) && !voucherPrint.PrintReferenceActivity.ToLower().Equals("notapplicable")) ? true : false;
                if (!string.IsNullOrEmpty(voucherPrint.SortLineItem))
                {
                    List<string> SortedLineItem = new List<string>();
                    List<string> SortedArticle = new List<string>();
                    List<string> SortedUOM = new List<string>();
                    List<decimal> SortedQuantity = new List<decimal>();
                    List<decimal> SortedUnitPrice = new List<decimal>();
                    List<string> SortedCategory = new List<string>();
                    List<string> SortedLineItemCode = new List<string>();
                    List<decimal> SortedTotalAmount = new List<decimal>();
                    List<string> SortedLineItemRemark = new List<string>();
                    //List<LineitemWeight> SortedLineItemWeight = new List<LineitemWeight>();
                    //List<LineItemNote> SortedLineItemNote = new List<LineItemNote>();
                    string[] array = { };
                    List<ArticleObjsPrint> SortedArticlesObj = new List<ArticleObjsPrint>();
                    if (voucherPrint.SortLineItem.ToLower() == "asentered")
                    {
                        array = ListLineItemObj.Select(c => c.LineItemCode).ToArray();
                        SortedLineItem = ListLineItemObj.Select(c => c.LineItemCode).ToList();
                        SortedLineItem.Sort();
                        for (int i = 0; i < SortedLineItem.Count; i++)
                        {
                            int index = Array.IndexOf(array, SortedLineItem[i]);
                            SortedArticle.Add(ListLineItemObj[index].Description);
                            SortedUOM.Add(ListLineItemObj[index].UOM);
                            SortedQuantity.Add(ListLineItemObj[index].Quantity);
                            SortedUnitPrice.Add(ListLineItemObj[index].UnitAmnt);
                            SortedTotalAmount.Add(ListLineItemObj[index].TotalAmount);
                            SortedLineItemCode.Add(ListLineItemObj[index].Article);
                            SortedCategory.Add(ListLineItemObj[index].Catagory);
                            //SortedLineItemWeight.Add(ListLineItemObj[index].LineItemWeight);
                            //SortedLineItemNote.Add(ListLineItemObj[index].LineItemNote);
                            SortedLineItemRemark.Add(ListLineItemObj[index].LineItemRemark);
                        }
                        for (int i = 0; i < SortedArticle.Count; i++)
                        {
                            //LineItemObj = newArticleObjsPrint();
                            //LineItemObj.LineItemNote = new LineItemNote();
                            //LineItemObj.LineItemNote = SortedLineItemNote[i];
                            //LineItemObj.LineItemWeight = new LineitemWeight();
                            //LineItemObj.LineItemWeight = SortedLineItemWeight[i];
                            LineItemObj.LineItemCode = SortedLineItem[i];
                            LineItemObj.Article = SortedLineItemCode[i];
                            LineItemObj.Description = SortedArticle[i];
                            LineItemObj.Quantity = SortedQuantity[i];
                            LineItemObj.UOM = SortedUOM[i];
                            LineItemObj.Catagory = SortedCategory[i];
                            LineItemObj.UnitAmnt = SortedUnitPrice[i];
                            LineItemObj.TotalAmount = SortedTotalAmount[i];
                            LineItemObj.LineItemRemark = SortedLineItemRemark[i];
                            if (ListLineItemObj.FirstOrDefault(lc => lc.LineItemCode == LineItemObj.LineItemCode).SerialCode.ContainsKey(LineItemObj.LineItemCode))
                            {
                                Dictionary<string, List<LineItemSerialCode>> Dictionary = new Dictionary<string, List<LineItemSerialCode>>();
                                Dictionary.Add(LineItemObj.LineItemCode, ListLineItemObj.FirstOrDefault(lc => lc.LineItemCode == LineItemObj.LineItemCode).SerialCode[LineItemObj.LineItemCode]);
                                LineItemObj.SerialCode = Dictionary;
                            }
                            if (ListLineItemObj.FirstOrDefault(lc => lc.LineItemCode == LineItemObj.LineItemCode).Specification.ContainsKey(LineItemObj.LineItemCode))
                            {
                                LineItemObj.Specification.Add(LineItemObj.LineItemCode, ListLineItemObj.FirstOrDefault(lc => lc.LineItemCode == LineItemObj.LineItemCode).Specification[LineItemObj.LineItemCode]);
                            }
                            if (ListLineItemObj.FirstOrDefault(lc => lc.LineItemCode == LineItemObj.LineItemCode).LineItemConversion.ContainsKey(LineItemObj.LineItemCode))
                            {
                                LineItemObj.LineItemConversion.Add(LineItemObj.LineItemCode, ListLineItemObj.FirstOrDefault(lc => lc.LineItemCode == LineItemObj.LineItemCode).LineItemConversion[LineItemObj.LineItemCode]);
                            }
                            SortedArticlesObj.Add(LineItemObj);
                        }
                    }
                    else if (voucherPrint.SortLineItem.ToLower() == "bynamealphabetically")
                    {
                        array = ListLineItemObj.Select(c => c.Description).ToArray();
                        SortedLineItem = ListLineItemObj.Select(c => c.Description).ToList();
                        SortedLineItem.Sort();
                        for (int i = 0; i < SortedLineItem.Count; i++)
                        {
                            int index = Array.IndexOf(array, SortedLineItem[i]);
                            SortedArticle.Add(ListLineItemObj[index].LineItemCode);
                            SortedUOM.Add(ListLineItemObj[index].UOM);
                            SortedQuantity.Add(ListLineItemObj[index].Quantity);
                            SortedUnitPrice.Add(ListLineItemObj[index].UnitAmnt);
                            SortedTotalAmount.Add(ListLineItemObj[index].TotalAmount);
                            SortedLineItemCode.Add(ListLineItemObj[index].Article);
                            SortedCategory.Add(ListLineItemObj[index].Catagory);
                            //SortedLineItemWeight.Add(ListLineItemObj[index].LineItemWeight);
                            //SortedLineItemNote.Add(ListLineItemObj[index].LineItemNote);
                            SortedLineItemRemark.Add(ListLineItemObj[index].LineItemRemark);
                        }

                        for (int i = 0; i < SortedArticle.Count; i++)
                        {
                            //LineItemObj = newArticleObjsPrint();
                            //LineItemObj.LineItemNote = SortedLineItemNote[i];
                            //LineItemObj.LineItemWeight = SortedLineItemWeight[i];
                            LineItemObj.LineItemCode = SortedArticle[i];
                            LineItemObj.Article = SortedLineItemCode[i];
                            LineItemObj.Description = SortedLineItem[i];
                            LineItemObj.Quantity = SortedQuantity[i];
                            LineItemObj.UOM = SortedUOM[i];
                            LineItemObj.Catagory = SortedCategory[i];
                            LineItemObj.UnitAmnt = SortedUnitPrice[i];
                            LineItemObj.TotalAmount = Convert.ToDecimal(SortedTotalAmount[i]);
                            LineItemObj.LineItemRemark = SortedLineItemRemark[i];
                            if (ListLineItemObj[i].SerialCode.ContainsKey(LineItemObj.LineItemCode))
                            {
                                Dictionary<string, List<LineItemSerialCode>> Dictionary = new Dictionary<string, List<LineItemSerialCode>>();
                                Dictionary.Add(LineItemObj.LineItemCode, ListLineItemObj[i].SerialCode[LineItemObj.LineItemCode]);
                                LineItemObj.SerialCode = Dictionary;
                            }
                            if (ListLineItemObj[i].Specification.ContainsKey(LineItemObj.LineItemCode))
                            {
                                LineItemObj.Specification.Add(LineItemObj.LineItemCode, ListLineItemObj[i].Specification[LineItemObj.LineItemCode]);
                            }
                            if (ListLineItemObj[i].LineItemConversion.ContainsKey(LineItemObj.LineItemCode))
                            {
                                LineItemObj.LineItemConversion.Add(LineItemObj.LineItemCode, ListLineItemObj[i].LineItemConversion[LineItemObj.LineItemCode]);
                            }
                            SortedArticlesObj.Add(LineItemObj);
                        }

                    }
                }
                voucherPrint.ListLineItemObj = ListLineItemObj;
                //voucherPrint.DocumentBrowser = IsDocumentBrowser;
                voucherPrint.printDialogueConsineeCode = voucherPrint.ConsigneeCode;

                voucherPrint.storeString = "";

                string mTextToPrint = "";

                if (NonListDataSource.SourceStoreDescription != null)
                {
                    mTextToPrint = NonListDataSource.SourceStoreDescription;
                }
                if (NonListDataSource.DestinationStoreDescription != null)
                {
                    mTextToPrint += "=>";
                    mTextToPrint += NonListDataSource.DestinationStoreDescription;
                }
                if (!string.IsNullOrWhiteSpace(mTextToPrint))
                {
                    voucherPrint.storeString = mTextToPrint;
                }
                voucherPrint.VoucherRemark = NonListDataSource?.Remark;
                if (NonListDataSource?.Cart!=null) {
                    voucherPrint.cart = NonListDataSource?.Cart.ToString();
                }

            }
            if (!voucherPrint.PrintAmountInWord)
            {
                voucherPrint.GrandTotalInWords = "";
            }
            string oppText = "";

            if (voucherPrint?.VoucherUserOrientation == "Horizontal")
            {
                if (activityList != null && activityList.Count > 0)
                {
                    var opereratorString = new List<string>();
                    var operarators = voucherPrint.Voucheroperators;
                    var activityDesc = voucherPrint.ActivityDefDesc;
                    var activityDate = voucherPrint.ActivityDate;
                    for (int z = 0; z < activityList.Count; z++)
                    {
                        oppText += voucherPrint.ActivityDefDesc[z]; ;
                        oppText += " by ";
                        oppText += voucherPrint.Voucheroperators[z];
                        oppText += " on " + voucherPrint.ActivityDate[z];
                       oppText += "__________";
                       oppText += "       ";
                    }

                }
            }
            voucherPrint.VoucheroperatorsString = oppText;
            if (voucherPrint?.ActivityDate != null)
            {
                foreach (var dt in voucherPrint.ActivityDate)
                {
                    if (string.IsNullOrWhiteSpace(voucherPrint?.DateString))
                    {
                        voucherPrint.DateString = dt;
                    }
                    else
                    {
                        voucherPrint.DateString = voucherPrint.DateString + "," + dt;
                    }
                }
            }
            voucherPrint.fsNo = NonListDataSource.FsNumber;
            voucherPrint.mrsNo = NonListDataSource.Mrc;
            var VoucherExtensionTransactionDescription = new List<string>();
            var VoucherExtensionTransactionNumber = new List<string>();
            if (!string.IsNullOrEmpty(voucherPrint.fsNo))
            {
                if (NonListDataSource.DefinitionId == 121) //CNETConstantes.REFUND)
                {
                    VoucherExtensionTransactionDescription.Add("RF No.");
                }
                else
                {
                    VoucherExtensionTransactionDescription.Add("FS No.");
                }
                VoucherExtensionTransactionNumber.Add(voucherPrint.fsNo);
            }
            if (!string.IsNullOrEmpty(voucherPrint.mrsNo))
            {
                VoucherExtensionTransactionDescription.Add("MRC No.");
                VoucherExtensionTransactionNumber.Add(voucherPrint.mrsNo);
            }

            var voucherExtensionString = "";
            if (VoucherExtensionTransactionDescription.Count > 0)
            {
                try
                {
                    for (var i = 0; i < VoucherExtensionTransactionDescription.Count; i++)
                    {
                        voucherExtensionString += VoucherExtensionTransactionDescription[i] + "  " + VoucherExtensionTransactionNumber[i] + "  ";
                    }

                }
                catch
                {

                }
            }
            voucherPrint.voucherExtensionString = voucherExtensionString;
            voucherPrint.VoucherExtensionTransactionDescription = VoucherExtensionTransactionDescription;
            voucherPrint.VoucherExtensionTransactionNumber = VoucherExtensionTransactionNumber;
            return voucherPrint;
        }

        private int GetPrintCount(List<ActivityDetail> Activities)
        {
            int printcount = 0;
            if (Activities != null && DocumentPrintSetting.PrintActivityDefinitionDTO != null && Activities.Count > 0)
            {
                List<ActivityDetail> Printcountactivity = Activities.Where(x => x.ActivityDescId == DocumentPrintSetting.PrintActivityDefinitionDTO.Id).ToList();

                if (Printcountactivity != null)
                    printcount = Printcountactivity.Count();
            }
            return printcount;
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap  = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }


        public async Task<HeaderDTO> PrintNoneLineItemVoucher(VoucherDetailDTO rdatasource)
        {
            #region Header
            HeaderDTO voucherPrint = new HeaderDTO();
            var NonListDataSource = rdatasource.VoucherHeader;
            var LineItemList = rdatasource.VoucherDetail;
            var activityList = rdatasource.ActivityDetail;
            Dictionary<string, List<LineItemConversionValues>> ConversionObj = new Dictionary<string, List<LineItemConversionValues>>();
            ArticleObjsPrint LineItemObj = new ArticleObjsPrint();
            List<ArticleObjsPrint> ListLineItemObj = new List<ArticleObjsPrint>();
            List<ConfigurationDTO> configBuffer = DocumentPrintSetting.ConfigurationDTOList.Where(x => x.Reference == NonListDataSource.DefinitionId.ToString()).ToList();
            var taxTransBuffer = UIProcessManager.GetTaxTransactionByVoucher(rdatasource.VoucherHeader.Id);

            var rDistribution = DocumentPrintSetting.DistributionList.Where(x => x.SystemConstant == NonListDataSource.DefinitionId && x.Type == 1578);

            var systemConstantBuffer = DocumentPrintSetting.SystemConstantList;
            var branches = DocumentPrintSetting.CompanyBranchList;
            #endregion

            #region companyInformation
            ConsigneeDTO rOrganization = DocumentPrintSetting.CompanyConsigneeDTO;// await _sharedHelpers.GetCompany();
            voucherPrint.companyName = rOrganization?.FirstName;
            //var companyIdentification = rIdentification.Where(x => x.reference == rOrganization?.code).ToList();
            voucherPrint.TinNo = rOrganization.Tin;
            // var branches = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string>() { { "Consignee", rOrganization.Id.ToString() } });
            voucherPrint.VatNo = DocumentPrintSetting.CompanyVATNumber;
            var headOffice = branches.Where(x => x.Abrivation == "HO")?.FirstOrDefault();
            if (headOffice == null) headOffice = new ConsigneeUnitDTO();
            string mtextToPrint = "";
            if (headOffice.AddressLine1 != null)
            { mtextToPrint += "Tel " + headOffice.AddressLine1 + ", "; }
            if (headOffice.PoBox != null)
            {
                mtextToPrint += "P.O.Box " + headOffice.PoBox + ", ";
            }
            voucherPrint.companyAddress = mtextToPrint;
            #endregion

            int GetCount = 0;// GetPrintCount(voucherPrint.Activities); 
            voucherPrint.PrintCount = GetCount;

            #region print setting
            var voucherDefinition = NonListDataSource.Definition.ToString();
            List<ConfigurationDTO> value = configBuffer;
            string DateFormat = "";
            bool EnablePaymentOptions = false;
            bool PrintAncestorReference = false;
            foreach (var va in value)
            {
                switch (va.Attribute.ToString())
                {
                    case "Default Printer":
                        voucherPrint.defaultPrinter = va.CurrentValue.ToString();
                        break;
                    case "No Of Copies":
                        voucherPrint.NoOfCopies = short.Parse(va.CurrentValue.ToString());
                        break;
                    case "Max No Of Printing":
                        voucherPrint.MaxNoOfPrinting = uint.Parse(va.CurrentValue.ToString());
                        break;
                    case "Date Format":
                        DateFormat = va.CurrentValue.ToString();
                        break;
                    case "Print Journal":
                        voucherPrint.PrintJournal = bool.Parse(va.CurrentValue ?? "false");
                        break;
                    case "Enable Payment Option":
                        EnablePaymentOptions = bool.Parse(va.CurrentValue ?? "false");
                        break;
                    case "Print Ancestor Reference":
                        PrintAncestorReference = bool.Parse(va.CurrentValue ?? "false");
                        break;
                    case "Print Water mark":
                        voucherPrint.waterMark = va.CurrentValue.ToString();
                        break;
                    case "Paper Type":
                        voucherPrint.paperType = va.CurrentValue.ToString();
                        break;
                    case "Voucher Orientation":
                        voucherPrint.VoucherOrientation = va.CurrentValue.ToString();
                        break;
                    case "Paper Size":
                        voucherPrint.PaperSize = va.CurrentValue.ToString();
                        break;
                    case "Print Copy Distribution":
                        voucherPrint.PrintCopyDistribution = bool.Parse(va.CurrentValue ?? "false");
                        break;
                    case "No Of Line Item Per Page":
                        voucherPrint.NoOfLineItemPerPage = int.Parse(va.CurrentValue ?? "0");
                        break;
                }
            }
            #endregion

            #region voucher Information and Voucher values
            if (DateFormat.ToLower() == "longdate")
            {
                voucherPrint.IssueDate = string.Format("{0:G}", NonListDataSource.IssuedDate);
            }
            else if (DateFormat.ToLower() == "mediumdate")
            {
                voucherPrint.IssueDate = NonListDataSource.IssuedDate.ToString("dd-MMM-yyyy");
            }
            else
            {
                voucherPrint.IssueDate = NonListDataSource.IssuedDate.ToShortDateString();
            }
            if (NonListDataSource.Consignee1Id != null)
            {
                voucherPrint.consignee = NonListDataSource.Consignee1FullName;
                voucherPrint.consigneeTitle = NonListDataSource.Consignee1PrefDesc;
                if (NonListDataSource.DefinitionId == 197 | NonListDataSource.DefinitionId == 200)
                {
                    voucherPrint.consigneeTitle = "Received From: ";
                }
                else if (NonListDataSource.DefinitionId == 210
                    || NonListDataSource.DefinitionId == 196
                    || NonListDataSource.DefinitionId == 195 ||
                     NonListDataSource.DefinitionId == 205)
                {
                    voucherPrint.consigneeTitle = "Pay To: ";
                }
                else if (NonListDataSource.DefinitionId == 194)
                {
                    voucherPrint.consigneeTitle = "Debit To: ";
                }
                else if (NonListDataSource.DefinitionId == 344)
                {
                    voucherPrint.consigneeTitle = "Credit To: ";
                }
                else if (NonListDataSource.DefinitionId == 121)
                {
                    voucherPrint.consigneeTitle = "Refund To: ";
                }
                else
                {
                    voucherPrint.consigneeTitle = "To: ";
                }
            }


            var taxTransactionList = taxTransBuffer?.Where(x => x.Voucher == NonListDataSource.Id).ToList();

            decimal withholdingTax = 0;
            decimal IncomeTax = 0;
            decimal newAmt = NonListDataSource.GrandTotal;
            if (taxTransactionList != null && taxTransactionList.Count > 0)
            {
                try
                {
                    withholdingTax = (decimal)taxTransactionList.FirstOrDefault(x => x.Tax == 6).TaxAmount;//WITHOLDING
                }
                catch
                {
                    withholdingTax = 0;
                }
                try
                {
                    IncomeTax = (decimal)taxTransactionList.FirstOrDefault(x => x.Tax == 5).TaxAmount;//Income
                }
                catch { IncomeTax = 0; }
                if (NonListDataSource.DefinitionId == 196 || NonListDataSource.DefinitionId == 197)
                {
                    newAmt = (NonListDataSource.GrandTotal - withholdingTax - IncomeTax);
                }
                else
                {
                    newAmt = NonListDataSource.GrandTotal;
                }
            }
            NumberToEnglish numToEng = new NumberToEnglish();
            var currencyBuffer = new List<CurrencyDTO>();
            var currency = new CurrencyDTO();
            if (NonListDataSource.CurrencyDescription != null)
            {
                currency.Description = NonListDataSource.CurrencyDescription;
                currency.IsDefault = true;
            }
            else
            {
                currency.Description = "Birr";
                currency.IsDefault = true;
            }
            currencyBuffer.Add(currency);
            voucherPrint.VoucherId = NonListDataSource.Id;
            voucherPrint.VoucherCode = NonListDataSource.Code;
            voucherPrint.amount_in_word = numToEng.changeCurrencyToWords(String.Format("{0:n2}", newAmt));
            voucherPrint.grandTotal = decimal.Parse(String.Format("{0:n2}", newAmt));
            voucherPrint.withHoldingAmount = withholdingTax;
            voucherPrint.incomeAmount = IncomeTax;
            voucherPrint.vouchernote = NonListDataSource.Note;
            voucherPrint.voucherDefinition = NonListDataSource.DefinitionId;
            voucherPrint.voucherType = NonListDataSource.Definition;
            if (EnablePaymentOptions)
            {
                if (NonListDataSource.PaymentMethod != null)
                {
                    var payme = systemConstantBuffer.FirstOrDefault(x => x.Id == NonListDataSource.PaymentMethod);
                    if (NonListDataSource.PaymentMethod != null)
                        voucherPrint.payment_method = payme.Description;
                    else voucherPrint.payment_method = "Cash";
                }
                else
                    voucherPrint.payment_method = "Cash";
                // voucherPrint.payment_method = NonListDataSource.PaymentProcessingName;
            }
            voucherPrint.isIssued = NonListDataSource.IsIssued;
            voucherPrint.isVoid = NonListDataSource.IsVoid;

            #endregion

            #region transaction References
            //List<TransactionReferenceDTO> TransactionRefBycode = UIProcessManager.GetTransactionReferenceByreferring(NonListDataSource.Id);
            //List<TransactionReferenceDTO> refList = new List<TransactionReferenceDTO>();
            //if (TransactionRefBycode != null && TransactionRefBycode.Count != 0)
            //{
            //    if (PrintAncestorReference)
            //    {
            //        foreach (TransactionReferenceDTO TrF in TransactionRefBycode)
            //        {
            //            if (TrF.Referenced != null)
            //            {
            //                List<TransactionReferenceDTO> AncesestorRef = UIProcessManager.GetTransactionReferenceByreferring(TrF.Referenced.Value);
            //                if (AncesestorRef?.Count > 0)
            //                    refList.AddRange(AncesestorRef);

            //            }
            //        }
            //    }
            //}
            //if (TransactionRefBycode != null)
            //    refList.AddRange(TransactionRefBycode);
            //if (refList.Count > 0)
            //{
            //    var refText = "";
            //    foreach (var TrRef in refList)
            //    {
            //        refText += TrRef.Referenced + ",";
            //    }
            //    refText.TrimEnd(',');
            //}
            voucherPrint.RefNo = NonListDataSource.Extension1;
            #endregion

            #region copy and department
            var CopyDescription = new List<string>();
            var OrganizationDepartment = new List<string>();
            var DistrbutionPrinterList = new List<string>();
            if (rDistribution != null)
            {
                DistributionDTO distributionDTO = new DistributionDTO();
                foreach (DistributionDTO objdistribution in rDistribution)
                {
                    CopyDescription.Add(objdistribution.Description);
                    DistrbutionPrinterList.Add(objdistribution.Remark);
                    OrganizationDepartment.Add(objdistribution.Destination != null ? branches.Where(x => x.Id == objdistribution.Destination).FirstOrDefault()?.Name : "");
                }
                if (CopyDescription != null)
                {
                    string coppyDstributionString = "";
                    string CopyDepartmentString = "";
                    var index = 0;
                    bool mOC = false;
                    bool mFC = false;
                    bool mSC = false;
                    bool mTC = false;
                    bool mFrC = false;
                    bool mFiC = false;
                    if (CopyDescription.Contains("Orignal copy"))
                    {
                        index = CopyDescription.IndexOf("Orignal copy");
                        coppyDstributionString += "Original Copy -" + OrganizationDepartment[index];
                    }
                    if (CopyDescription.Contains("1st copy"))
                    {
                        index = CopyDescription.IndexOf("1st copy");
                        if (mOC == true)
                        {
                            coppyDstributionString += " , 1st Copy - " + OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 1st copy -" + OrganizationDepartment[index];
                        }
                        mFC = true;
                    }
                    if (CopyDescription.Contains("2nd copy"))
                    {
                        index = CopyDescription.IndexOf("2nd copy");
                        if (mFC == true)
                        {
                            coppyDstributionString += " , 2nd Copy - " + OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 2nd Copy - " + OrganizationDepartment[index];
                        }
                        mSC = true;
                    }
                    if (CopyDescription.Contains("3rd copy"))
                    {
                        index = CopyDescription.IndexOf("3rd copy");
                        if (mOC == true | mFC == true | mSC == true)
                        {
                            coppyDstributionString += " , 3rd Copy - " + OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 3rd Copy - " + OrganizationDepartment[index];
                        }
                        mTC = true;
                    }
                    if (CopyDescription.Contains("4th copy"))
                    {
                        index = CopyDescription.IndexOf("4th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true)
                        {
                            coppyDstributionString += " , 4th Copy - " + OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 4th Copy - " + OrganizationDepartment[index];
                        }
                        mFrC = true;
                    }
                    if (CopyDescription.Contains("5th copy"))
                    {
                        index = CopyDescription.IndexOf("5th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true | mFrC == true)
                        {
                            coppyDstributionString += " , 5th Copy - " + OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += "5th Copy - " + OrganizationDepartment[index];
                        }
                        mFiC = true;

                    }
                    if (CopyDescription.Contains("6th copy"))
                    {
                        index = CopyDescription.IndexOf("6th copy");
                        if (mOC == true | mFC == true | mSC == true | mTC == true | mFrC == true | mFiC == true)
                        {
                            coppyDstributionString += "  , 6th Copy - " + OrganizationDepartment[index];
                        }
                        else
                        {
                            coppyDstributionString += " 6th Copy - " + OrganizationDepartment[index];
                        }
                    }
                    voucherPrint.CopyDistributionString = coppyDstributionString;
                }
                //if (OrganizationDepartment != null)
                //{
                //    #region Watermark
                //    //var GetCount = GetPrintCount(voucherPrint, true);
                //    IsVoucherPrinted = false; //GetCount.Item1;
                //    PrintCount = 0;// GetCount.Item2;
                //    string mDistTo = null;
                //    if (PrintWaterMark == "Standard")
                //    {
                //        if (IsVoid == true)
                //        {
                //        }
                //        else if (IsPreview == true)
                //        {
                //        }
                //        else if (IsIssued != true)
                //        {
                //        }
                //        else
                //        {
                //            if (PrintCount >= 0)
                //            {
                //                if (PrintCopyDistribution == true)
                //                {
                //                    if (PrintCount == 0)
                //                    {
                //                        if (CopyDescription != null)
                //                        {
                //                            if (CopyDescription.Count > 0)
                //                            {
                //                                if (CopyDescription.Contains("Orignal copy"))
                //                                {
                //                                    int index = CopyDescription.FindIndex(x => x.StartsWith("1st copy"));
                //                                    mDistTo = OrganizationDepartment[index];
                //                                }
                //                            }
                //                        }
                //                    }
                //                    if (PrintCount == 1)
                //                    {
                //                        if (CopyDescription != null)
                //                        {
                //                            if (CopyDescription.Count > 0)
                //                            {
                //                                if (CopyDescription.Contains("1st copy"))
                //                                {
                //                                    int index = CopyDescription.FindIndex(x => x.StartsWith("1st copy"));
                //                                    mDistTo = OrganizationDepartment[index];
                //                                }
                //                            }
                //                        }
                //                    }
                //                    else if (PrintCount == 2)
                //                    {
                //                        if (CopyDescription != null)
                //                        {
                //                            if (CopyDescription.Count > 0)
                //                            {
                //                                if (CopyDescription.Contains("2nd copy"))
                //                                {
                //                                    int index = CopyDescription.FindIndex(x => x.StartsWith("2nd copy"));
                //                                    mDistTo = OrganizationDepartment[index];
                //                                }
                //                            }
                //                        }
                //                    }
                //                    else if (PrintCount == 3)
                //                    {
                //                        if (CopyDescription != null)
                //                        {
                //                            if (CopyDescription.Count > 0)
                //                            {
                //                                if (CopyDescription.Contains("3rd copy"))
                //                                {
                //                                    int index = CopyDescription.FindIndex(x => x.StartsWith("3rd copy"));
                //                                    mDistTo = OrganizationDepartment[index];
                //                                }
                //                            }
                //                        }
                //                    }
                //                    else if (PrintCount == 4)
                //                    {
                //                        if (CopyDescription != null)
                //                        {
                //                            if (CopyDescription.Count > 0)
                //                            {
                //                                if (CopyDescription.Contains("4th copy"))
                //                                {
                //                                    int index = CopyDescription.FindIndex(x => x.StartsWith("4th copy"));
                //                                    mDistTo = OrganizationDepartment[index];
                //                                }
                //                            }
                //                        }
                //                    }
                //                    else if (PrintCount == 5)
                //                    {
                //                        if (CopyDescription != null)
                //                        {
                //                            if (CopyDescription.Count > 0)
                //                            {
                //                                if (CopyDescription.Contains("5th copy"))
                //                                {
                //                                    int index = CopyDescription.FindIndex(x => x.StartsWith("5th copy"));
                //                                    mDistTo = OrganizationDepartment[index];
                //                                }
                //                            }
                //                        }
                //                    }
                //                    else if (PrintCount == 6)
                //                    {
                //                        if (CopyDescription != null)
                //                        {
                //                            if (CopyDescription.Count > 0)
                //                            {
                //                                if (CopyDescription.Contains("6th copy"))
                //                                {
                //                                    int index = CopyDescription.FindIndex(x => x.StartsWith("6th copy"));
                //                                    mDistTo = OrganizationDepartment[index];
                //                                }
                //                            }
                //                        }
                //                    }
                //                    else
                //                    {
                //                    }
                //                }
                //                else
                //                {
                //                }
                //            }
                //            else
                //            {

                //            }
                //        }
                //    }
                //    OrganizationDepartmentString = mDistTo;
                //    #endregion
                //}
            }
            #endregion

            #region operators
            string oppText = "";
            if (activityList != null && activityList.Count > 0)
            {
                List<string> dateList = new List<string>();
                List<bool> isManual = new List<bool>();
                List<string> opList = new List<string>();
                List<string> actList = new List<string>();
                foreach (var val in activityList)
                {
                    if (val.IsPrint.HasValue && val.IsPrint.Value)
                    {
                        isManual.Add(val.IsManual ?? false);
                        opList.Add(val.UserName);
                        string dt = val.ActivityDate != null ? val.ActivityDate?.ToString("dd-MM-yyyy") : "";
                        dateList.Add(dt);
                        actList.Add(val.ActivityDefDesc != null ? val.ActivityDefDesc : "");
                    }
                }
                voucherPrint.Voucheroperators = opList;
                voucherPrint.ActivityDate = dateList;
                voucherPrint.ActivityDefDesc = actList;
                voucherPrint.isManual = isManual;
                //for (int z = 0; z < activityList.Count; z++)
                //{
                //    if (activityList[z].IsPrint != null && activityList[z].IsPrint.Value)
                //    {
                //        oppText += z+1 + ") " + voucherPrint.ActivityDefDesc[z];
                //        oppText += " By ";
                //        oppText += voucherPrint.Voucheroperators[z];
                //        oppText += " on " + voucherPrint.ActivityDate[z];
                //        oppText += "____________" + Environment.NewLine;
                //    }
                //}
                int count = 0;
                foreach (var val in activityList)
                {
                    if (val.IsPrint.HasValue && val.IsPrint.Value&& val.ActivityDate != null && val.ActivityDescId >0 && val.ActivityDefDesc != null)
                    {
                        count++;
                        string dt = val.ActivityDate != null ? val.ActivityDate?.ToString("dd-MM-yyyy") : "";
                        oppText += count + ") " +( val.ActivityDefDesc != null ? val.ActivityDefDesc : "");
                        oppText += " By ";
                        oppText += val.UserName;
                        oppText += " on " + dt;
                        oppText += "____________" + Environment.NewLine;

                        
                    }
                }


            }


            voucherPrint.VoucheroperatorsString = oppText;
            #endregion


            var VoucherExtensionTransactionDescription = new List<string>();

            var VoucherExtensionTransactionNumber = new List<string>();

            if (!string.IsNullOrEmpty(rdatasource.VoucherHeader.FsNumber))
            {
                if (NonListDataSource.DefinitionId == 121) //CNETConstantes.REFUND)
                {
                    VoucherExtensionTransactionDescription.Add("RF No.");
                }
                else
                {
                    VoucherExtensionTransactionDescription.Add("FS No.");
                }
                VoucherExtensionTransactionNumber.Add(rdatasource.VoucherHeader.FsNumber);
            }

            if (!string.IsNullOrEmpty(rdatasource.VoucherHeader.Mrc))
            {
                VoucherExtensionTransactionDescription.Add("MRC No.");
                VoucherExtensionTransactionNumber.Add(rdatasource.VoucherHeader.Mrc);
            }

            var voucherExtensionString = "";
            if (VoucherExtensionTransactionDescription.Count > 0)
            {
                try
                {
                    for (var i = 0; i < VoucherExtensionTransactionDescription.Count; i++)
                    {
                        voucherExtensionString += VoucherExtensionTransactionDescription[i] + "  " + VoucherExtensionTransactionNumber[i] + "  ";
                    }
                }
                catch
                {

                }
            }
            voucherPrint.voucherExtensionString = voucherExtensionString;
            return voucherPrint;
        }
    }
}

