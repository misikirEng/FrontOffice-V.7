using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.Mobile.Payments.Models
{
    public class MobilePaymentDTO
    {
        public string PaymentReference { get; set; }
        public int PaymentProcessorConsigneeId { get; set; }
        public int PaymentProcessorConsigneeUnit { get; set; }
        public string PaymenetProcessorName { get; set; }
        public int? ConsigneeID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime MaturityDate { get; set; }
    }
}
