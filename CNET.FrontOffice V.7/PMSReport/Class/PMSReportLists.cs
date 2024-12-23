using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSReport
{
    public class PMSReportLists
    {
        
    }

    public class FiscalReconciliationReport
    {
        public string ReportItems { get; set; }
        public List<CashSales> CashSalesList { get; set; }
        public List<CreditSales> CreditSalesList { get; set; }
        public List<CheckOutSales> CheckOutSalesList { get; set; }
        public List<RefundSales> RefundSalesList { get; set; }
        public List<FiscalReconciliation> FiscalReconciliation { get; set; }
    }

    public class FiscalReconciliation
    {
        public string Description { get; set; }
        public string Value { get; set; }
    }
    public class CheckOutSales
    {
        public string code { get; set; }
        public string Date { get; set; }
        public string ServiceCharge { get; set; }
        public string Discount { get; set; }
        public string Tax { get; set; }
        public string GrandTotal { get; set; }
        public string UserName { get; set; }
    }

    public class RefundSales
    {
        public string code { get; set; }
        public string Date { get; set; }
        public string ServiceCharge { get; set; }
        public string Discount { get; set; }
        public string Tax { get; set; }
        public string GrandTotal { get; set; }
        public string UserName { get; set; }
    }

    public class CashSales
    {
        public string code { get; set; }
        public string Date { get; set; }
        public string ServiceCharge { get; set; }
        public string Discount { get; set; }
        public string Tax { get; set; }
        public string GrandTotal { get; set; }
        public string UserName { get; set; }
    }

    public class CreditSales
    {
        public string code { get; set; }
        public string Date { get; set; }
        public string ServiceCharge { get; set; }
        public string Discount { get; set; }
        public string Tax { get; set; }
        public string GrandTotal { get; set; }
        public string UserName { get; set; }
    }
    public class NationalityReport
    {
        public string Name { get; set; }
        public string Continent { get; set; }
        public string Nationality { get; set; }
        public string PoliticalName { get; set; }
        public int NoOfCustomer { get; set; }
        public decimal Amount { get; set; }

    }


    public class ForeignExchangeReport
    {
        public string Voucher { get; set; }
        public DateTime  Date { get; set; }
        public string PreparedBy { get; set; }
        public decimal GrandTotal { get; set; }


        public List<ForeignExDetail> ForeignExDetailList { get; set; }
        
    }

    public class ForeignExDetail
    {
        public string Empty { get; set; }
        public string Currency { get; set; }
        public string Abbrivation { get; set; }
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public decimal AmountInBirr { get; set; }
    }

    public class ForexSummary
    {
        public string User { get; set; }
        public List<Tuple<string,decimal>> Transactions { get; set; }
    }

    public class SummaryOfSummary
    {
        public DateTime Date { set; get; }
        public decimal RoomRevenue { set; get; }
        public decimal Package { set; get; }
        public decimal ServiceCharge { set; get; }
        public decimal Vat { set; get; }
        public decimal RoomTotal { set; get; }
        public decimal POSCharge { set; get; }
        public decimal TodayTotal { set; get; }
        public decimal BBF { set; get; }
        public decimal toDateTotal { set; get; }
        public decimal Payment { set; get; }
        public decimal Discount { set; get; }
        public decimal Paidout { set; get; }
        public decimal BCF { set; get; }
        public decimal Outstanding { set; get; }
    }

    public class RoomPOSCharges
    {   
        public string RegNo { get; set; }
        public string VoucherID { get; set; }
        public string Guest { get; set; }
        public string Company { get; set; }
        public string Note { get; set; }
        public string RoomType { get; set; }
        public string Room { get; set; }
        public decimal Amount { get; set; }
    }

    public class LineItemTransactionReports
    {
        public string VoucherID { get; set; }
        public string RegNo { get; set; }
        public string CustomerName { get; set; }
        public DateTime IssuedDate { get; set; }
        public string RoomNo { get; set; }
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal VAT { get; set; }
        public decimal GrandTotal { get; set; }
        public string LastOperation { get; set; }
        public string LastOperator { get; set; }
        public string Device { get; set; }
    }

    public class NonLineItemTransactionReports
    {
        public string VoucherID { get; set; }
        public string RegNo { get; set; }
        public string CustomerName { get; set; }
        public DateTime IssuedDate { get; set; }
        public string RoomNo { get; set; }
        public string Note { get; set; }
        public decimal GrandTotal { get; set; }
        public string LastOperation { get; set; }
        public string LastOperator { get; set; }
        public string Device { get; set; }
    }

    public class CRVTransactionReports
    {
        public string VoucherID { get; set; }
        public string RegNo { get; set; }
        public string CustomerName { get; set; }
        public DateTime IssuedDate { get; set; }
        public string RoomNo { get; set; }
        public string Note { get; set; }
        public string OtherReference { get; set; }
        public decimal GrandTotal { get; set; }
        public string LastOperation { get; set; }
        public string LastOperator { get; set; }
        public string Device { get; set; }
    }

    public class OccupancySummary
    {
        public int SN { get; set; }
        public string RoomType { get; set; }
        public int Rooms { get; set; }
        public int Occupied { get; set; }
        public int Vacant { get; set; }
        public decimal Occupancy { get; set; }
        public decimal MTD { get; set; }
        public decimal YTD { get; set; }
        public decimal LastYearDate { get; set; }
        public decimal LastYearMonth { get; set; }
        public decimal LastYear { get; set; }
        public decimal ADR { get; set; }
    }

    public class CollectorAnalysis
    {
        public int SN { get; set; }
        public string Cashier { get; set; }
        public decimal FromGuest { get; set; }
        public decimal AdvancedDeposit { get; set; }
        public decimal FromCredit { get; set; }
        public decimal CashRecieved { get; set; }
        public decimal Total { get; set; }
    }

    public class SalesCenters
    {
        public int SN { get; set; }
        public bool IsPrevious { get; set; }
        public string MachineName { get; set; }
        public decimal Cash { get; set; }
        public decimal Room { get; set; }
        public decimal CityLedger { get; set; }
        public decimal TotalToday { get; set; }
        public decimal MonthToDate { get; set; }
        public decimal YearToDate { get; set; }
        public decimal LastYearDate { get; set; }
        public decimal LastYearMonth { get; set; }
        public decimal LastYear { get; set; }
    }

    public class IncomeAnalysis
    {
        public int SN { get; set; }
        public bool IsPrevious { get; set; }
        public string Particulars { get; set; }
        public decimal TotalToday { get; set; }
        public decimal MonthTodate { get; set; }
        public decimal YearToDate { get; set; }
        public decimal LastYearDate { get; set; }
        public decimal LastYearMonth { get; set; }
        public decimal LastYear { get; set; }
    }

    public class ResidentSummary
    {
        public int SN { get; set; }
        public string Particulars { get; set; }
        public decimal TotalToday { get; set; }
        public decimal MonthTodate { get; set; }
        public decimal YearToDate { get; set; }
        public decimal LastYearDate { get; set; }
        public decimal LastYearMonth { get; set; }
        public decimal LastYear { get; set; }
    }


    public class DailyBusinessReport
    {
        public string ReportItems { get; set; }
        public List<ResidentSummary> dailyResidentSummaryList { get; set; }
        public List<IncomeAnalysis> incomeAnalysis { get; set; }
        public List<SalesCenters> salesCenters { get; set; }
        public List<CollectorAnalysis> collectorAnalysis { get; set; }
        public List<OccupancySummary> occupancySummary { get; set; }
    }

    public class RoomIncomeReport
    {
        public string RoomType { get; set; }
        public string Registration { get; set; }
        public DateTime Date { get; set; }
        public string RoomNo { get; set; }
        public string CustomerName { get; set; }
        public string RateType { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal VAT { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
    }

    public class CashDropDocuments
    {
        public string Documents { get; set; }
        public List<SalesDocumentList> SalesDocuments { get; set; }
        public List<PaymentDocumentList> PaymentDocuments { get; set; }
    }

    public class SalesDocumentList
    {
        public int SN { get; set; }
        public string VoucherID { get; set; }
        public DateTime Date { get; set; }
        public string RoomNo { get; set; }
        public string RegNumber { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string Reference { get; set; }
        public decimal Cash { get; set; }
        public decimal Allowance { get; set; }
        public decimal RoomCharge { get; set; }
        public decimal CityLedger { get; set; }
       
    }

    public class DailySalesDocument
    {
        public string VoucherID { get; set; }
        public string Customer { get; set; }
        public string RoomNo { get; set; }
        public decimal Cash { get; set; }
        public decimal RoomPosCharge { get; set; }
        public decimal CityLedger { get; set; }
    }

    public class PaymentDocumentList
    {
        public int SN { get; set; }
        public string VoucherID { get; set; }
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }
        public string ReceivedFrom { get; set; }

        public string RegNumber { get; set; }
        public string RoomNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentNumber { get; set; }
        public string CompanyName { get; set; }
        
        public decimal Amount { get; set; }
    }

    public class PackageReport
    {
        public string RoomType { set; get; }
        public string Reg_No { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string PackageGroup { set; get; }
        public string PackageType { set; get; }
        public string Adult { set; get; }
        public string Child { set; get; }
        public string UnitPackage { set; get; }
        public string TotalPackage { set; get; }
    }

    public class PoliceReport
    {
        public int SN { get; set; }
        public string Reg_No { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Guestcode { set; get; }
        public string Gender { set; get; }
        public string Nationality { set; get; }
        public DateTime DOB { set; get; }
        public string ID_Description { set; get; }
        public string ID_Number { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string PurposeOfTravel { get; set; }
    }
    public class PoliceReportGovernment
    {
        public int Code { get; set; }
        public string tin { set; get; }
        public string branch { set; get; }
        public string hotel { set; get; }
        public string room { set; get; }
        public string company { set; get; }
        public string guestName { set; get; }
        public string gender { set; get; }
        public string nationality { set; get; }
        public DateTime doB { set; get; }
        public string idType { set; get; }
        public string idNumber { set; get; }
        public DateTime arrivalDate { set; get; }
        public DateTime departureDate { set; get; }
        public string purposeOfTravel { get; set; }
        public DateTime reportDate { get; set; }
        public string registeredBy { set; get; }
        public int pax { set; get; }
        public string image { set; get; }
        public string imagename { set; get; }
        public string remark { set; get; } 
    }
    public class CheckoutReportColumn
    {
        public int SN { get; set; }
        public string VoucherID { get; set; }
        public DateTime Date { get; set; }
        public string RegNo { set; get; }
        public string Room { set; get; }
        public int NoOfNight { set; get; }
        public string Customer { set; get; }
        public string Company { set; get; }
        public string User { set; get; }
        public string PaymentType { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal VAT { get; set; }
        public decimal GrandTotal { get; set; }

    }

    public class TrialBalance
    {
        public string Group { set; get; }
        public string Description { set; get; }
        public string Balance { set; get; }
    }

    public class CashierSummaryReportByUser
    {
        //public string Voucher { set; get; }
        public string User { set; get; }
        public string Currency { set; get; }
        public string PaymentMethod { set; get; }
        public string VoucherType { set; get; }
        public string CurrencyAmount { set; get; }
        public string Rate { set; get; }
        public string etbTotal { set; get; }
    }

    public class CashierSummaryReportBySummary
    {
        //public string Voucher { set; get; }
        public string Currency { set; get; }
        public string PaymentMethod { set; get; }
        public string VoucherType { set; get; }
        public string CurrencyAmount { set; get; }
        public string Rate { set; get; }
        public string etbTotal { set; get; }
    }


    public class DepositLedger
    {
        public string Reg_No { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public DateTime? ArrivalDate { set; get; }
        public DateTime? DepartureDate { set; get; }
        public string Reg_Type { set; get; }
        public string PaymentType { set; get; }
        public DateTime? LastPaidOn { set; get; }
        public string DepositBalance { set; get; }
    }

    public class CashReceiptReport
    {
        public string Reg_No { set; get; }
        public string VoucherNo { set; get; }
        public string Room { set; get; }
        public DateTime IssueDate { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string Note { set; get; }
        public string GrandTotal { set; get; }
    }

    public class PaidoutReport
    {
        public string Reg_No { set; get; }
        public string VoucherNo { set; get; }
        public string Room { set; get; }
        public DateTime IssueDate { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string Note { set; get; }
        public string GrandTotal { set; get; }
    }

    public class RebateReport
    {
        public string Reg_No { set; get; }
        public string VoucherNo { set; get; }
        public string Room { set; get; }
        public DateTime IssueDate { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string Note { set; get; }
        public string GrandTotal { set; get; }
    }

    public class CreditCardsOfTheDay
    {
        public string Reg_No { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string PaymentMethod { set; get; }
        public string PaymentProcessor { set; get; }
        public string Number { set; get; }
        public DateTime MaturityDate { set; get; }
        public string etbAmount { set; get; }
    }

    public class CheckReportOfTheDay
    {
        public string Reg_No { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string PaymentMethod { set; get; }
        public string PaymentProcessor { set; get; }
        public string Number { set; get; }
        public DateTime MaturityDate { set; get; }
        public string Amount { set; get; }
    }

    public class CancellationReport
    {
        public string Reg_No { set; get; }
        public string Room { set; get; }
        public string RoomCount { set; get; }
        public string RoomType { set; get; }
        public string Company { set; get; }
        public string Guest { set; get; }
        public string Adult { set; get; }
        public string Child { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string RateCode { set; get; }
        public string RateAmount { set; get; }
        public string PaymentType { set; get; }
        public string User { set; get; }
        public string ActualRTC { set; get; }
        public string MarketCode { set; get; }
    }

    public class NoShowReport
    {
        public string Reg_No { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string Reg_State { set; get; }
        public string Reg_Type { set; get; }
        public string PaymentType { set; get; }
        public string MarketCode { set; get; }
    }

   

    public class ArrivalsList
    {
        public string Reg_No { set; get; }
        public string RoomType { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string ID_Number { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string Res_Type { set; get; }
        public string PaymentMethod { set; get; }
        public string Market { set; get; }
    }

    public class ArrivedList
    {
        public string Reg_No { set; get; }
        public string RoomType { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string ID_Number { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string Res_Type { set; get; }
        public string PaymentMethod { set; get; }
        public string Market { set; get; }
    }


    public class UnassignedReservations
    {
        public string SN { set; get; }
        public string Reg_No { set; get; }
        public string RoomType { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string State { get; set; }

        public string Color { get; set; }
    }


    public class BillTransferReport
    {
        public string ToRegistration { get; set; }
        public string ToGuest { get; set; }
        public string ToRoom { get; set; }
        public string ToRoomType { get; set; }
        public string ToState { get; set; }

        public List<TransferFrom> TransferFromList { get; set; }

    }

    public class TransferFrom
    {
        public string VoucherCode { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime IssuedDate { get; set; }
        public string VoucherType { get; set; }
        public string TransferedBy { get; set; }
        public DateTime TransferedDate { get; set; }
        public string FromRegistration { get; set; }
        public string FromGuest { get; set; }
        public string FromRoom { get; set; }
        public string FromRoomType { get; set; }
        public string FromStatus { get; set; }
    }
    

    public class StayOvers
    {
        public string Reg_No { set; get; }
        public string RoomType { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string ID_Number { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string Res_Type { set; get; }
        public string PaymentMethod { set; get; }
        public string Market { set; get; }
    }

    public class DueOuts
    {
        public string Reg_No { set; get; }
        public string RoomType { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string ID_Number { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string Res_Type { set; get; }
        public string PaymentMethod { set; get; }
        public string Market { set; get; }
    }

    public class DepartedList
    {
        public string Reg_No { set; get; }
        public string RoomType { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string ID_Number { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string Res_Type { set; get; }
        public string PaymentMethod { set; get; }
        public string Market { set; get; }
    }

    public class PostmasterInHouseList
    {
        public string Reg_No { set; get; }
        public string RoomType { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string ID_Number { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string Res_Type { set; get; }
        public string PaymentMethod { set; get; }
        public string Market { set; get; }
    }

    public class RoomMoved
    {
        public string Reg_No { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public string PreviousRoom { set; get; }
        public string PreviousRoomType { set; get; }
        public string CurrentRoom { set; get; }
        public string CurrentRoomType { set; get; }
        public string RoomCount { set; get; }
        public string Adult { set; get; }
        public string Child { set; get; }
        public string RateCode { set; get; }
        public string RateAmount { set; get; }
        public string User { set; get; }
        public string ActualRTC { set; get; }
    }

    public class DetailDailySalesTransactions
    {
        public string Reg_No { set; get; }
        public string Guest { set; get; }
        public string SubTotal { set; get; }
        public string Discount { set; get; }
        public string AdditionalCharge { set; get; }
        public string Tax { set; get; }
        public string GrandTotal { set; get; }
        public string UserName { set; get; }
        public string DeviceName { set; get; }
        public string Type { set; get; }
    }

    public class PickUPOrDropOff
    {
        public string Reg_No { set; get; }
        public string Guest { set; get; }
        public string Room { set; get; }
        public string RoomType { set; get; }
        public string Company { set; get; }
        public string TransactionType { set; get; }
        public string Station { set; get; }
        public string Carrier { set; get; }
        public string TransportationNo { set; get; }
        public DateTime? TravelTimestamp { set; get; }
    }

    public class RateCheckReport
    {
        public string Reg_No { set; get; }
        public string Room { set; get; }
        public string Guest { set; get; }
        public string Company { set; get; }
        public int Adult { set; get; }
        public int Child { set; get; }
        public string RateCodeHeader { set; get; }
        public string RateCodeAmount { set; get; }
        public string RateAmount { set; get; }
        public string Variance { set; get; }
        public string Currency { set; get; }
        public DateTime ArrivalDate { set; get; }
        public DateTime DepartureDate { set; get; }
        public string RoomType { set; get; }
        public string RTC { set; get; }
        public string Reg_State { set; get; }
    }

    public class RateAdjustmentReport
    {
        public string Reg_No { set; get; }
        public string Guest { set; get; }
        public DateTime? ArrivalDate { set; get; }
        public DateTime? DepartureDate { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public string isPercent { set; get; }
        public string Amount { set; get; }
        public string Value { set; get; }
        public string Reason { set; get; }
    }

    public class ReportDTO
    {
        public string reportCode { get; set; }
        public string reportDesc { get; set; }
        public string reportParent { get; set; }
    }

    public class ManagerialFlashReportFields
    {
        public int SN { get; set; }
        public string ReportItem { get; set; }
        public string CurrentDate { get; set; }
        public string CurrentMonth { get; set; }
        public string CurrentYear { get; set; }
        public string LastYearToday { get; set; }
        public string LastYearThisMonth { get; set; }
        public string LastYearNow { get; set; }
    }

    #region House Keeping
    public class DiscripancyReport
    {
        public string RoomNo { get; set; }
        public string RoomType { get; set; }
        public string RoomStatus { get; set; }
        public string HkStatus { get; set; }
        public string FoStatus { get; set; }
        public string ResStatus { get; set; }
        public string FoPerson { get; set; }
        public string HkPerson { get; set; }
        public string Discrepancy { get; set; }
        public string Date { get; set; }
    }


    public class HKStatusReport
    {
        public string StatusName { get; set; }
        public List<HKStatusDetailReport> StatusDetailList { get; set; }
        public List<HKStatusSummaryReport> StatusSummaryList { get; set; }

    }

    public class HKStatusSummaryReport
    {
        public int TotalRooms { get; set; }
        public int CleanRooms { get; set; }
        public int DirtyRooms { get; set; }
        public int PickupRooms { get; set; }
        public int OooRooms { get; set; }
        public int OosRooms { get; set; }
        public int InspectedRooms { get; set; }
        public int VacantRooms { get; set; }
        public int OccupiedRooms { get; set; }
    }

    public class HKStatusDetailReport
    {
        public int SN { get; set; }
        public string Floor { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public string RoomStatus { get; set; }
        public string HKStatus { get; set; }


    }

    public class HKTaskAssignment
    {
        public string TaskDate { get; set; }
        public string Task { get; set; }
        public string IsAuto { get; set; }
        public string TotalSheets { get; set; }
        public string TotalCredits { get; set; }
    }

    public class HKActivityReport
    {
        public string Activity { get; set; }
        public string RoomNumber { get; set; }
        public string Date { get; set; }
        public string User { get; set; }
        public string DeviceName { get; set; }
        public string Action { get; set; }
    }
    #endregion 

}
