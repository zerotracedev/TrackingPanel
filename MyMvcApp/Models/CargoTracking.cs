using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMvcApp.Models
{
    public class CargoTracking
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int? OrderId { get; set; }

        [Required]
        [StringLength(100)]
        public string TrackingNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        public Order Order { get; set; }
    }
}
