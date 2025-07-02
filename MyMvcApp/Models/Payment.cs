using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMvcApp.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order? Order { get; set; }  // Nullable yapıldı

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [StringLength(50)]
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;  // Varsayılan atandı

        [StringLength(50)]
        [Required]
        public string Status { get; set; } = string.Empty;         // Varsayılan atandı
    }
}
