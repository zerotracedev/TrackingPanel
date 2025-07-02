using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMvcApp.Models
{
    // public class Order
    // {
    //     public int Id { get; set; }

    //     public string? OrderNumber { get; set; }

    //     [Required]
    //     public int CustomerID { get; set; }
    //     public Customer Customer { get; set; }

    //     [Required]
    //     public DateTime OrderDate { get; set; }

    //     [Required]
    //     [Column(TypeName = "decimal(18,2)")]
    //     public decimal TotalAmount { get; set; }

    //     public string? ShippingAddress { get; set; }
    //     public string? PaymentMethod { get; set; }
    //     public string? Status { get; set; }

    //     public string? PhoneNumber { get; set; }
    //     public string? Email { get; set; }

    //     public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public class Order
    {
        public int Id { get; set; }

        public string? OrderNumber { get; set; }

        [Required]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

}

}