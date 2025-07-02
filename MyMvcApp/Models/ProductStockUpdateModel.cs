using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models
{
    public class ProductStockUpdateModel
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int QuantityToAdd { get; set; }
    }
}