using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace foodOrderingApp.models
{
    public class MenuItemRequest
    {


        [Required(ErrorMessage = "RestaurantId is required.")]
        public Guid RestaurantId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public Guid Category { get; set; }

        [Required]
        public IFormFile? Photo { get; set; }

        [Required]
        public bool IsCustomizable { get; set; }

        public decimal Price { get; set; }

        public string VariantsJson { get; set; } = string.Empty;
    }
    public class FoodItemVariantRequest
    {
        [Required(ErrorMessage = "Size is required.")]
        [MaxLength(20, ErrorMessage = "Size cannot exceed 20 characters.")]
        public string? Size { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

}
