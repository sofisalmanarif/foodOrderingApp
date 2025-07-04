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
            Shipped,
            Delivered,
            Cancelled
        }

        public enum PaymentMethod{
            cod,
            online
        }
        public enum PaymentStatus
        {
            paid,
            unpaid
        }
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        public Guid AddressId { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal PayableAmount { get; set; }

        public string? DiscountCoupon { get; set; }
        public decimal Discount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public PaymentMethod paymentMethod {get;set;} =PaymentMethod.cod;
        public PaymentStatus paymentStatus { get; set; } = PaymentStatus.unpaid;

        public string PaymentTransactionId {get;set;} = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public Restaurant? Restaurant { get; set; }
        public User? User {get;set;}

        public Address? Address {get;set;}


    }
}