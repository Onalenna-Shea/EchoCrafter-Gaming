namespace EchoCrafter.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
        public required string CustomerName { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public decimal Total { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int OrderId { get; set; }
    }
}
