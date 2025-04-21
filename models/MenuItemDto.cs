using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class MenuItemDto
    {

        [Required(ErrorMessage = "RestaurantId is required.")]
        public Guid RestaurantId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        public Guid Category { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        public string ImageUrl { get; set; } =  string.Empty;

        [Required(ErrorMessage = "IsCustomizable must be specified.")]
        public bool IsCustomizable { get; set; }

        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public List<FoodItemVariantRequest>? Variants { get; set; }
    }
    

    }