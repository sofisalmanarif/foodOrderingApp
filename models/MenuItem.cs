using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace foodOrderingApp.models
{
    public class MenuItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Restaurant is required")]
        public Guid RestaurantId { get; set; } 

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Image is Required")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is Required")]
        public Guid CategoryId { get; set; }
        public List<MenuItemVarient>? Variants { get; set; }

        public bool IsCustomizable { get; set; } = true;

        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
    }
}
