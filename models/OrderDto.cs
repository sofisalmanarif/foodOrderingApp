
using System.ComponentModel.DataAnnotations;


namespace foodOrderingApp.models
{
    public class OrderDto
    {
        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        public Guid AddressId { get; set; }


        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }


}