
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace foodOrderingApp.models
{
    public class Restaurant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string RestaurantName { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Phone]
        public string RestaurantPhone { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false; 
        public bool IsActive {get;set;}
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User? Owner { get; set; }
        [JsonIgnore]
        public List<MenuItem>? MenuItems { get; set; }


    }
}