using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class Cart
    {
        public Guid Id { get; set; } =  Guid.NewGuid();

        public Guid UserId { get; set; } 

        public List<CartItem> CartItems { get; set; } = new();
    }

    public class CartItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        public Guid CartId { get; set; }

        public Guid ItemId { get; set; } 

        public Guid? VariantId { get; set; } 
        public int Quantity { get; set; } = 1;
        public Cart Cart { get; set; }
        public MenuItemVarient? Variant { get; set; }
        public MenuItem Item{get;set;}
    }
}