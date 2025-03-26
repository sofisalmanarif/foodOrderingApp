using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class Order
    {
        public enum OrderStatus
        {
            Pending,
            Processing,
            Completed,
            Cancelled
        }
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        public Guid AddressId { get; set; }

        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = OrderStatus.Pending.ToString();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Helper to Get Enum
        public OrderStatus GetOrderStatus()
        {
            return Enum.Parse<OrderStatus>(Status);
        }

        // Helper to Set Enum
        public void SetOrderStatus(OrderStatus status)
        {
            Status = status.ToString();
        }
    }
}