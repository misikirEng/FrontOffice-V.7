using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CNET_V7_Domain.Domain.TransactionSchema;
using CNET_V7_Domain.Domain.PmsSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.AccountingSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Misc.CommonTypes;

namespace DocumentPrint.DTO
{
    public class VoucherPrintModel
    {
        #region Print Settings
        public int PrintCount { get; set; }
        public bool IsIssued { get; set; }
        public bool IsPreview { get; set; }
        public bool IsVoucherPrinted { get; set; }
        public string printDialogueConsineeCode { get; set; }
        public bool PrintQuantitySum { get; set; }
        public string documentname { get; set; }
        public string attachmentPath { get; set; }
        public string printDialogueDocumentname { get; set; }
        public int VoucherId { get; set; }
        public int voucherDefinition { get; set; }
        public string defaultPrinter { get; set; }
        public string PrintWaterMark { get; set; }
        public string DateFormat { get; set; }
        public bool PrintWithoutPreview { get; set; }
        public uint MaxLineItem { get; set; }
        public bool PrintSeasonalmessage { get; set; }
        public uint MaxNoOfPrinting { get; set; }
        public uint NoOfLineItemPerPage { get; set; }
        public string VoucherOrientation { get; set; }
        public bool MergreItemCodeAndDescription { get; set; }
        public bool EnableQtyConversion { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public string devicecode { get; set; }
        public string Type { get; set; }
        public bool IsForced { get; set; }
        public bool PrintRemoteDistribution { get; set; }
        public bool PrintCopyDistribution { get; set; }
        public bool PrintCatalogueAutomatically { get; set; }
        public bool PrintArticleCode { get; set; }
        public bool PrintArticlePicture { get; set; }
        public string PrintingMethod { get; set; }
        public string VoucherUserOrientation { get; set; }
        public bool UseDarkerLines { get; set; }
        public bool PrintAncestorReference { get; set; }
        public bool PrintAncestorExtension { get; set; }
        public bool PrintimmediateReference { get; set; }
        public bool PrintSum { get; set; }
        public bool PrintAmountInWord { get; set; }
        public string PaperSize { get; set; }
        public bool PrintJournal { get; set; }
        public string PrintSpecification { get; set; }
        public bool PrintProductionDate { get; set; }
        public bool PrintConsigneeCode { get; set; }
        public string TemplateDocument { get; set; }
        public bool PrintExpiryDate { get; set; }
        public bool PrintBatch { get; set; }
        public string PrintValues { get; set; } = "All";
        public bool EnablePaymentOptions { get; set; }
        public bool EnableBridgeWeight = false;
        public string RoundDigitTotal { get; set; }
        public string RoundDigitQuantity { get; set; }
        public string RoundDigitUnitPrice { get; set; }
        public string SortLineItem { get; set; }
        public bool EnableTerm { get; set; }
        public string termsPath { get; set; }
        public bool PrintBankInfo { get; set; }
        public short NoOfCopies { get; set; }
        public string PaperType { get; set; }
        public bool PrintArticleVolume { get; set; }
        public bool Hasendweightzero { get; set; }
        public int IntRoundTotalDigit { get; set; }
        public string PrintReferenceActivity { get; set; }
        #endregion

        #region CompanyInformation
        public string CompanyName { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyWeb { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPOBox { get; set; }
        public string CompanyKifleKetema { get; set; }
        public string CompanyHouseNo { get; set; }
        public string CompanyKebele { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyStreet { get; set; }
        public string CompanyMobile { get; set; }
        public string TINNo { get; set; }
        public string VATNo { get; set; }
        public string CompanyBranch { get; set; }
        public string logoPath { get; set; }
        public List<string> WorkflowCode { get; set; }
        public List<string> ActivityDefDesc { get; set; }
        public List<string> Voucheroperators { get; set; }
        public List<string> ActivityDate { get; set; }
        public List<bool> WorkflowManual { get; set; }
        public List<string> WorkflowVariables { get; set; }
        public NonCashTransactionInformationPrint NonCashPayment { get; set; }
        public Dictionary<string, List<ArticleSpecification>> Specification = new Dictionary<string, List<ArticleSpecification>>();
        public Dictionary<string, List<LineItemSerialCode>> SerialCode = new Dictionary<string, List<LineItemSerialCode>>();
        public Dictionary<string, List<LineItemConversionValues>> LineItemConversion = new Dictionary<string, List<LineItemConversionValues>>();
        #endregion

        #region ConsigneeInformation
        public string ConsigneeCode { get; set; }
        public string ConsigneeMobile { get; set; }
        public string ConsigneeFax { get; set; }
        public string ConsigneWeb { get; set; }
        public string ConsigneeEmail { get; set; }
        public string ConsigneeKifleketema { get; set; }
        public string ConsigneeHouseNo { get; set; }
        public string ConsigneeKebele { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeCountry { get; set; }
        public string ConsigneeStreet { get; set; }
        public string ConsigneeContactName { get; set; }
        public string ConsigneePOBox { get; set; }
        public string ConsigneeWoreda { get; set; }
        public string ConsigneeRef { get; set; }
        public string ConsigneeTel { get; set; }
        public string VATCertificate { get; set; }
        public string TINCertificate { get; set; }
        public string ConsigneeBranch { get; set; }
        #endregion

        #region Voucher Information
        public string DateString { get; set; }
        public bool IsVoid { get; set; }
        public string VoucherString { get; set; }
        public string mAddressExtended { get; set; }
        public string Consignee { get; set; }
        public string Consignee2 { get; set; }
        public string CopyDescriptionString { get; set; }
        public List<string> CopyDescription { get; set; }
        public List<string> OrganizationDepartment { get; set; }
        public List<string> WorkFlowLookupCode { get; set; }
        public List<string> WorkFlowObjectStateDescription { get; set; }
        public List<bool> HasIssuingEffect { get; set; }
        public string WorkFlowObjectStateDescriptionString { get; set; }
        public string PaymentMethodString { get; set; }
        public string paymentOptionTitle { get; set; }
        public string VoucheroperatorsString { get; set; }
        public string OrganizationDepartmentString { get; set; }
        public string OtherConsignee { get; set; }
        public string VoucherRemark { get; set; }
        public string IssuedDate { get; set; }
        public bool DocumentBrowser { get; set; }
        public string footerString { get; set; }
        public decimal withHoldingAmount { get; set; }
        public string SourceStore { get; set; }
        public string DestinationStore { get; set; }
        public string storeString { get; set; }
        public string cart { get; set; }
        public string fsNo { get; set; }
        public string mrsNo { get; set; }
        public string voucherExtentionString { get; set; }
        public List<string> VoucherExtensionTransactionDescription { get; set; }
        public List<string> VoucherExtensionTransactionNumber { get; set; }
        public List<string> DistrbutionPrinterList { get; set; }
        public string transactionReferenceString { get; set; }
        public VoucherValues voucherValues { get; set; }
        public List<VoucherInformation> VoucherInformation { get; set; }
        //public List<ActitityAndWorkFlow /*vw_WorkFlowByReferenceView*/> AllWorkFlow { get; set; }   
        public List<VoucherTermDTO> TermListView { get; set; }
        public List<ArticleObjsPrint> ListLineItemObj { get; set; }
        public List<JournalDetailObjPrint> JournalDetObj { get; set; }
        public List<ActivityDetail> Activities { get; set; }
        public List<TaxDTO> TaxBuffer { get; set; } = new List<TaxDTO>();
        public VoucherBuffer attachment { get; set; }
        public List<VoucherConsigneeListDTO> ConsigneeDetail = new List<VoucherConsigneeListDTO>();
        public List<ConsigneeUnitDTO> ConsigneeUnitBuffer = new List<ConsigneeUnitDTO>();
        public string voucherExtensionString { get; set; }
        #endregion
        #region voucher Valuees
        public decimal SubTotal { get; set; }
        public decimal AdditionalCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal taxTotal { get; set; }
        public string taxString { get; set; }
        public string GrandTotalInWords { get; set; }
        #endregion
        #region Clearance
        public string CompName { get; set; }
        public string DocNo { get; set; }
        public DateTime issudate { get; set; }

        public string fullname { get; set; }
        public DateTime DateOfEmployment { get; set; }
        public string jobTitle { get; set; }
        public string Deprt { get; set; }
        public string ReasonOfTerm { get; set; }
        public DateTime DateOfTerm { get; set; }
        public string Note { get; set; }
        public List<OtherConsigneeDetail> OtherConsigneeDetail { get; set; }
        #endregion
    }
    public class OtherConsigneeDetail
    {
        public int consignee { get; set; }
        public string requiredGSlDesc { get; set; } = string.Empty;
        public string consigneeFullName { get; set; } = string.Empty;
        public string consigneTin { get; set; } = string.Empty;
    }
    public class PayrollHeaderDTO
    {
        public int SN { get; set; }
        public string PayrollTransferCode { get; set; }
        public string PayrollTransferDate { get; set; }
        public string BankName { get; set; }
        public string PayAmount { get; set; }
        public string PayAmountText { get; set; }
        public string CompanyAccount { get; set; }
        public string Period { get; set; }
        public string EmployeeDepartment { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeAccount { get; set; }
        public string EmployeeAccountBranch { get; set; }
        public string NetAmount { get; set; }
    }
    public class overtimeLineItemDetail
    {
        public int Sn { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string Hrate { get; set; }
        public string rate { get; set; }
        public string hours { get; set; }
        public string total { get; set; }

    }
    public class clearanceDeptDTO
    {
        public int SN { get; set; }
        public string workUnit { get; set; }
        public string workUnitCode { get; set; }
        public string user { get; set; }
        public string userName { get; set; }
        public string Date { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityUser { get; set; }
        public string ActivityUserName { get; set; }
        public string Signature { get; set; }

    }
    public class LeaveVoucherView
    {
        public int Sn { get; set; }
        public string datetime { get; set; }
        public string weekDay { get; set; }
        public string Session { get; set; }
        public string DayPortion { get; set; }
        public decimal total { get; set; }
    }
    public class LeaveAllocationView
    {
        public int Sn { get; set; }
        public string description { get; set; }
        public string year { get; set; }
        public string startDate { get; set; }
        public string endeDate { get; set; }
        public string allocatopn { get; set; }
    }
    public class LeaveDefinitionDTO
    {
        public bool Checked { get; set; }
        public string LineItem { get; set; }
        public decimal Quantity { get; set; }
        public decimal Remaining { get; set; }
        public DateTime ValidUntil { get; set; }
        public string Voucher { get; set; }
        public int year { get; set; }
        public decimal Requested { get; set; }
        internal decimal unaccountedTotal { get; set; }
        public decimal ToDeductFrom { get; set; }

    }
    public class PrintedActivities
    {
        public string Description { get; set; }
        public string UserName { get; set; }
        public bool IsManual { get; set; }
        public string ActivityCode { get; set; }
        public DateTime? TimeStap { get; set; }

    }
    public class LetterReportDto
    {
        public string lblDocNo { get; set; }
        public string docNo { get; set; }
        public string date { get; set; }
        public string lblDate { get; set; }
        public string lblTo { get; set; }
        public string vouchernote { get; set; }
        public string lblHeader { get; set; }
        public bool isEnglish { get; set; }
        public List<string> department { get; set; }
    }
    public class NonCashTransactionInformationPrint
    {

        public string BankName { get; set; }
        public string PaymentProcessor { get; set; }
        public string Branch { get; set; }
        public string PaymentNumber { get; set; }
        public string PaymentDescription { get; set; }
        public DateTime MaturityDate { get; set; }



    }
    public class JournalDetailObjPrint
    {
        public string Account { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
    public class ArticleObjsPrint
    {
        public int sn { get; set; }
        public string Article { get; set; }
        public string Description { get; set; }
        public string LineItemCode { get; set; }
        public string LineItemRemark { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; }
        public string Catagory { get; set; }
        public decimal UnitAmnt { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? productionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string LineItemNote { get; set; }
        public decimal LineItemWeight { get; set; }
        public PhysicalDimensionPrint PhysicalDimension { get; set; }
        public bool IsBatchItem { get; set; }
        public Dictionary<string, List<ArticleSpecification>> Specification = new Dictionary<string, List<ArticleSpecification>>();
        public Dictionary<string, List<LineItemSerialCode>> SerialCode = new Dictionary<string, List<LineItemSerialCode>>();
        public Dictionary<string, List<LineItemConversionValues>> LineItemConversion = new Dictionary<string, List<LineItemConversionValues>>();
    }
    public class ArticleSpecificationPrint
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
        public string Type { get; set; }
    }
    public class PhysicalDimensionPrint
    {
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string Description { get; set; }
    }
    public class LineItemSerialCodePrint
    {
        public string Code { get; set; }
        public string Value { get; set; }
        public string Definition { get; set; }
        public DateTime? BatchProductionDate { get; set; }
        public DateTime? BatchExpiryDate { get; set; }
    }
    public class LineItemConversionValuesPrint
    {
        public string code { get; set; }
        public string uom { get; set; }
        public string UOMLookupDescription { get; set; }
        public double Quantity { get; set; }
        public decimal UnitAmount { get; set; }
    }

    public class ArticleSpecification
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
        public string Type { get; set; }
    }
    public class PhysicalDimension
    {
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string Description { get; set; }
    }
    public class LineItemSerialCode
    {
        public string Code { get; set; }
        public string Value { get; set; }
        public string Definition { get; set; }
        public DateTime? BatchProductionDate { get; set; }
        public DateTime? BatchExpiryDate { get; set; }
    }
    public class LineItemConversionValues
    {
        public string code { get; set; }
        public string uom { get; set; }
        public string UOMLookupDescription { get; set; }
        public double Quantity { get; set; }
        public decimal UnitAmount { get; set; }
    }
    public class VoucherValues
    {
        public List<TaxTransactionDTO> VoucherTax { get; set; }
        public decimal SubTotal { get; set; }
        public decimal AdditionalCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal taxTotal { get; set; }
        public string taxString { get; set; }
        public decimal GrandTotal { get; set; }
        public string GrandTotalInWords { get; set; }
        public string Remark { get; set; }
    }
    public class RoomChargeDetail
    {
        public string RegistationNumber { get; set; }
        public string RoomNumber { get; set; }
    }
    public class VoucherInformation
    {

        public string SourceStore { get; set; }
        public string DestinationStore { get; set; }
        public string VoucherName { get; set; }
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public bool IsLineItem { get; set; }
        public bool IsVoid { get; set; }
        public bool IsIssued { get; set; }
        public bool IsPreview { get; set; }
        public DateTime IsssueDate { get; set; }
        public string Consignee { get; set; }
        public string Consignee2 { get; set; }
        public string VoucherNote { get; set; }
        public int VoucherDefinition { get; set; }
        public string Cart { get; set; }
        public string OtherConsignee { get; set; }
        public string Type { get; set; }
        public string VoucherRemark { get; set; }
        public RoomChargeDetail RoomPOSChargeDetail { get; set; }
        public bool isPosCharge { get; set; }
        public string LastObjectState { get; set; }
    }
    public class NonCashTransactionInformation
    {

        public string BankName { get; set; }
        public string PaymentProcessor { get; set; }
        public string Branch { get; set; }
        public string PaymentNumber { get; set; }
        public string PaymentDescription { get; set; }
        public DateTime MaturityDate { get; set; }

    }
    public class ConsigneeInformation
    {
        public string ConsigneeCode { get; set; }
        public string ConsigneeMobile { get; set; }
        public string ConsigneeFax { get; set; }
        public string ConsigneWeb { get; set; }
        public string ConsigneeEmail { get; set; }
        public string ConsigneeKifleketema { get; set; }
        public string ConsigneeHouseNo { get; set; }
        public string ConsigneeKebele { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeCountry { get; set; }
        public string ConsigneeStreet { get; set; }
        public string ConsigneeContactName { get; set; }
        public string ConsigneePOBox { get; set; }
        public string ConsigneeWoreda { get; set; }
        public string ConsigneeRef { get; set; }
        public string ConsigneeTel { get; set; }
        public string VATCertificate { get; set; }
        public string TINCertificate { get; set; }
        public string IdDescription { get; set; }
    }
    public class DistrbutionObj
    {
        public string DistrbutionDepartment { get; set; }
        public string PrinterList { get; set; }
        public string CopyDiscription { get; set; }
    }
    public class VoucherExtensionObj
    {
        public string code { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
    public class ActitityAndWorkFlow
    {
        public string ActivityDefination { get; set; }
        public string IsManual { get; set; }
        public int Index { get; set; }
        public string VoucherOperator { get; set; }
        public string OSDDescription { get; set; }
        DateTime OperationTime { get; set; }
        public string ActivityName { get; set; }
    }
    public class PCPRGridDTO
    {

        public int SN { get; set; }
        public string PCPVCode { get; set; }
        public string PayTo { get; set; }
        public DateTime Date { get; set; }
        public string Purpose { get; set; }
        public decimal PCPRAmount { get; set; }
    }

    public class HeaderDTO
    {
        
        public int PrintCount { get; set; }
        public string consigneeTitle { get; set; }
        public string companyAddress { get; set; }
        public string companyName { get; set; }
        public string TinNo { get; set; }
        public string VatNo { get; set; }
        public string VoucheroperatorsString { get; set; } 
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string RefNo { get; set; }
        public decimal grandTotal { get; set; }
        public decimal withHoldingAmount { get; set; }
        public decimal incomeAmount { get; set; }
        public string amount_in_word { get; set; }
        public string IssueDate { get; set; }
        public string payment_method { get; set; }
        public bool PrintJournal { get; set; }
        public string logoPath { get; set; }
        public string BackImage { get; set; }
        public bool isVoid { get; set; }
        public bool isIssued { get; set; }
        public string waterMark { get; set; }
        public string paperType { get; set; }
        public string VoucherOrientation { get; set; }
        public uint MaxNoOfPrinting { get; set; }
        public string defaultPrinter { get; set; }
        public short NoOfCopies { get; set; }
        public string PaperSize { get; set; }
        public bool PrintCopyDistribution { get; set; }
        public string OrganizationDepartmentString { get; set; }
        public string CopyDistributionString { get; set; } 
        
        public string voucherType { get; set; }
        public string vouchernote { get; set; }
        public string consignee { get; set; }
        public string remark { get; set; }
        public string voucherExtensionString { get; set; }
        public int voucherDefinition { get; set; }
        public int NoOfLineItemPerPage { get; set; }
        public List<string> ActivityDefDesc { get; set; }
        public List<string> Voucheroperators { get; set; }
        public List<string> ActivityDate { get; set; }
        public List<bool> isManual { get; set; }
        public List<PCPRGridDTO> PCPRGridList { get; set; }
        public List<JournalDetailObjPrint> JournalDetail { get; set; }
        public List<ActivityDefinitionDTO> activityDefinitionBuffer { get; set; }
    }
    public class BankDepositDTO
    {
        public string logoPath { get; set; }
        public string bank { get; set; }
        public string AccountHolder { get; set; }
        public string AccountType { get; set; }
        public string AccountNo { get; set; }
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string TinNo { get; set; }
        public string VatNo { get; set; }
        public string VoucheroperatorsString { get; set; }
        public string copyDistribution { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherDefnition { get; set; }
        public string VoucherDefnitionName { get; set; }
        public string RefNo { get; set; }
        public decimal amount { get; set; }
        public decimal withHoldingAmount { get; set; }
        public decimal grandTotal { get; set; }
        public string amount_in_word { get; set; }
        public DateTime IssueDate { get; set; }
        public string payment_method { get; set; }
        public bool PrintJournal { get; set; }
        public List<LineItemDTO> denominationList { get; set; }

    }
    public class LineItemDTO
    {
        public string remark { get; set; }
        public decimal? calculatedCost { get; set; }
        public decimal? taxAmount { get; set; }
        public int? tax { get; set; }
        public decimal taxableAmount { get; set; }
        public decimal? totalAmount { get; set; }
        public string totalAmountString { get; set; }
        public string coinString { get; set; }
        public decimal? coin { get; set; }
        public string note { get; set; }
        public string UOM { get; set; }
        public string quantityString { get; set; }
        public decimal quantity { get; set; }
        public decimal? unitAmt { get; set; }
        public string article { get; set; }
        public string voucher { get; set; }
        public string code { get; set; }
    }
    public class VoucherValuesPrint
    {
        public List<TaxTransactionDTO> VoucherTax { get; set; }
        public decimal SubTotal { get; set; }
        public decimal AdditionalCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal taxTotal { get; set; }
        public string taxString { get; set; }
        public decimal GrandTotal { get; set; }
        public string GrandTotalInWords { get; set; }
        public string Remark { get; set; }
    }
    public class ConsigneeInformationPrint
    {
        public string ConsigneeCode { get; set; }
        public string ConsigneeMobile { get; set; }
        public string ConsigneeFax { get; set; }
        public string ConsigneWeb { get; set; }
        public string ConsigneeEmail { get; set; }
        public string ConsigneeKifleketema { get; set; }
        public string ConsigneeHouseNo { get; set; }
        public string ConsigneeKebele { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeCountry { get; set; }
        public string ConsigneeStreet { get; set; }
        public string ConsigneeContactName { get; set; }
        public string ConsigneePOBox { get; set; }
        public string ConsigneeWoreda { get; set; }
        public string ConsigneeRef { get; set; }
        public string ConsigneeTel { get; set; }
        public string VATCertificate { get; set; }
        public string TINCertificate { get; set; }
        public string IdDescription { get; set; }
    }
    public class PrintCommonObj
    {
        //public List<Attachment> Attachment = new List<Attachment>();
        //public List<Identification> Identification = new List<Identification>();
        //public Organization organization { get; set; }
        //public PrintCommonObj()
        //{
        //    Attachment = new List<Attachment>();
        //    Identification = new List<Identification>();
        //}
    }
    public class ArticleObjs
    {
        public int sn { get; set; }
        public string Article { get; set; }
        public string Description { get; set; }
        public string LineItemCode { get; set; }
        public string LineItemRemark { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; }
        public string Catagory { get; set; }
        public decimal UnitAmnt { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? productionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        //public LineItemNote LineItemNote { get; set; }
        //public LineitemWeight LineItemWeight { get; set; }
        public PhysicalDimension PhysicalDimension { get; set; }
        public bool IsBatchItem { get; set; }
        public Dictionary<string, List<ArticleSpecification>> Specification = new Dictionary<string, List<ArticleSpecification>>();
        public Dictionary<string, List<LineItemSerialCode>> SerialCode = new Dictionary<string, List<LineItemSerialCode>>();
        public Dictionary<string, List<LineItemConversionValues>> LineItemConversion = new Dictionary<string, List<LineItemConversionValues>>();
    }


    public enum DocumentType
    {
        Voucher = 0,
        Grid = 1,
        Check = 2,
        ForexTransaction = 3,
        Label = 4,
        PickUp = 5,
        LeaveVoucher = 6,
        OvertimeVoucher = 7,
        LeaveAllocation = 8,
        PayrollSlip = 9,

        PromotionSlip = 10,

        FieldworkVoucher = 11,
        NewEmployeeAnnouncement = 12,
        Clearance = 13,
        AnnualSupportAgreement = 14,
        VoucherDesignTemplate = 15,

    }
}
