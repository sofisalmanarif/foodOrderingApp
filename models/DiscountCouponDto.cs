using System.ComponentModel.DataAnnotations;
using foodOrderingApp.Models;

namespace foodOrderingApp.models
{
    public class DiscountCouponDto
    {
        [Required(ErrorMessage = "Coupon code is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Code must be between 3 and 20 characters.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "ValidTill date is required.")]
        [DataType(DataType.Date)]
        public DateOnly ValidTill { get; set; }

        [Required(ErrorMessage = "Discount type is required.")]
        public DiscountType Type { get; set; }

        [Required(ErrorMessage = "Discount value is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount must be greater than 0.")]
        public float DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum order value cannot be negative.")]
        public float? MinOrderValue { get; set; } = 160;
    }
}
