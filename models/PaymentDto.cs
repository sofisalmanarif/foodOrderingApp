using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class PaymentDto
    {
      
            [Required(ErrorMessage = "Amount is required")]
            [Range(0.01, float.MaxValue, ErrorMessage = "Amount must be greater than zero")]
            public float Amount { get; set; }
    }
}