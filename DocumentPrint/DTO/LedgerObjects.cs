using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentPrint.DTO
{
    public class LedgerObjects
    {
        public string CustomerName { get; set; }
        public int? ConsigneeId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyTin { get; set; }
        public string RegistrationNumber { get; set; }
        public string Plan { get; set; }
        public string FsNo { get; set; }
        public string TINNo { get; set; }
        public string HeaderText { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public string SubTotal { get; set; }
        public string Discount { get; set; }
        public string ServiceCharge { get; set; }
        public string Vat { get; set; }
        public string GrandTotal { get; set; }
        public string TotalOtherBill { get; set; }
        public string TotalPaid { get; set; }
        public string TotalCredit { get; set; }
        public string Refund { get; set; }
        public string User { get; set; }
        public string RemainingBalance { get; set; }
        public bool SetWaterMark { get; set; }
        public int? ConsigneeUnit { get; set; }
    }
}
