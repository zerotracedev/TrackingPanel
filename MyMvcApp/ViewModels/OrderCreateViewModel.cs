using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyMvcApp.Models;

namespace MyMvcApp.ViewModels
{
    public class OrderCreateViewModel
    {
        public int? Id { get; set; }
        [Required]
        public int CustomerID { get; set; }
        public List<Customer> Customers { get; set; } = new();
        [Required]
        public DateTime OrderDate { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();
        public string? ShippingAddress { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class OrderItemViewModel
    {
        [Required]
        public int ProductID { get; set; }
        public List<Product> Products { get; set; } = new();
        [Required]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}