using Restoran.Models;

namespace Restoran.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public OrderStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class CreateOrderDto
    {
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateOrderItemDto
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
