using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models.requists
{
    public class CartRequist
    {
           public  Guid? CartId { get; set; }
           [Required]
            public Guid ItemId { get; set; }

            public Guid? VariantId { get; set; } // optional

            public int Quantity { get; set; }
        
    }
}