using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class Address
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Area { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
        public string ShopNumber { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User? User { get; set; }
    }
}