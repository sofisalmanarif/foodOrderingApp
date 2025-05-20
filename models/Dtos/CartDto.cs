using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models.Dtos
{
    public class CartDto
    {
        public Guid ItemId { get; set; }
        public Guid RestaurantId { get; set; }

        public Guid? VariantId { get; set; } // optional

        public int Quantity { get; set; }
    }
}