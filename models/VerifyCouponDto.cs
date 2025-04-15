using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class VerifyCouponDto
    {
        [Required(ErrorMessage = "Coupon code is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Code must be between 3 and 20 characters.")]
        public string Code { get; set; } =string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "Total must be greater than 0.")]
        public float CartAmount { get; set; }

    }
}