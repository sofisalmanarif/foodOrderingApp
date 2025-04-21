using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
   public  enum AddressType{
        Restaurant,
        Home,
        Work
    }
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

        public Guid RefId { get; set; }
        public AddressType AddressType { get; set; } = AddressType.Home;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User? User { get; set; }
        // [JsonIgnore]
        // public Restaurant? Restaurant {get;set;}
    }
  
    }