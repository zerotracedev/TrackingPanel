using System.Collections.Generic;

namespace MyMvcApp.Models
{
    public class PaymentIndexViewModel
    {
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public int TotalCount { get; set; }
        public int CompletedCount { get; set; }
        public int PendingCount { get; set; }
        public int FailedCount { get; set; }
    }
}
