
using System.ComponentModel.DataAnnotations;


namespace foodOrderingApp.models
{
    public class OrderDto
    {
        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        public Guid AddressId { get; set; }

        /// <summary>
        /// Payment method: cash on delivery (cod) or online.
        /// </summary>
        [Required]
        [EnumDataType(typeof(Order.PaymentMethod))]
        public Order.PaymentMethod PaymentMethod { get; set; } = Order.PaymentMethod.cod;

        /// <summary>
        /// Optional: For online payments, the payment gateway may return a Transaction Id.
        /// </summary>
        public string? PaymentTransactionId { get; set; }


        /// <summary>
        /// List of items ordered. At least one item is required.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one order item must be provided.")]
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }


}