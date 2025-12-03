using System.ComponentModel.DataAnnotations;
using Restoran.Models;

namespace Restoran.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public OrderStatus Status { get; set; }
        public string SpecialRequirements { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? SentToKitchenAt { get; set; }
        public DateTime? ReadyAt { get; set; }
        public DateTime? ServedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public decimal Total { get; set; }
    }

    public class CreateOrderDto
    {
        [Required]
        public int TableId { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        public string SpecialRequirements { get; set; } = string.Empty;

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    public class UpdateOrderDto
    {
        public string? SpecialRequirements { get; set; }
        public string? CustomerName { get; set; }
        public List<CreateOrderItemDto>? OrderItems { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string SpecialRequirements { get; set; } = string.Empty;
    }

    public class CreateOrderItemDto
    {
        [Required]
        public int MenuItemId { get; set; }

        [Required]
        [Range(1, 20)]
        public int Quantity { get; set; }

        public string SpecialRequirements { get; set; } = string.Empty;
    }
}
