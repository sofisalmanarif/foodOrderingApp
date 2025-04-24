using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class CategoryRequistModel
    {
        [Required(ErrorMessage = "Category Name is required.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Category Image is required.")]
        public IFormFile? Photo { get; set; }

    }
}