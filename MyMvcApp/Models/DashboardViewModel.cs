using System;
using System.Collections.Generic;

namespace MyMvcApp.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public int TodayOrders { get; set; }
        public List<OrderStatusCount> OrderStatusCounts { get; set; }
        public List<CargoStatusCount> CargoStatusCounts { get; set; }
        public List<OrderSummary> LatestOrders { get; set; }
    }

    public class OrderStatusCount
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class CargoStatusCount
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class OrderSummary
    {
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}