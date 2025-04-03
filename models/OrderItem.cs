using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        public Guid? VariantId { get; set; } 

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }

        // public MenuItem? MenuItem {get;set;}
        // ✅ Navigation Property for MenuItem
        [JsonIgnore]
        public MenuItem? MenuItem { get; set; }

        // ✅ Navigation Property for Variant (if applicable)
        [JsonIgnore]
        public MenuItemVarient? Variant { get; set; }
    }
}