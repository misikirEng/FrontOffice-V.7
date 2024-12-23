using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNET.Mobile.Payments.Models
{
    internal class PaymentProvider
    {
        public string name { get; set; }
        public string image_URL { get; set; }
        public int PaymentProcessorConsigneeId { get; set; } //payment processor
        public int PaymentProcessorConsigneeUnit { get; set; } //payment method
        public bool is_Synchronous { get; set; }
        public bool is_Two_Step { get; set; }
        public bool is_Customer_Init { get; set; }
    }
}
