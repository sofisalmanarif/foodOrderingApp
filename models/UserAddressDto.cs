using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class UserAddressDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Area { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;

        public Guid RefId { get; set; }
        public string AddressType { get; set; } =string.Empty ;
    }
}