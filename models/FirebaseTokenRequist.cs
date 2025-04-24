using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class FirebaseTokenRequist
    {
        [Required(ErrorMessage = "Firebase Token is required.")]
        public string FirebaseToken { get; set; } = string.Empty;
    }
}