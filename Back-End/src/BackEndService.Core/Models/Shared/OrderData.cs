using System.Collections.Generic;

namespace BackEndService.Core.Models.Shared
{
    public class OrderItem
    {
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderData
    {
        public List<OrderItem> Items { get; set; } = new();
        public decimal Total { get; set; }
        public string? Currency { get; set; }
    }
}


