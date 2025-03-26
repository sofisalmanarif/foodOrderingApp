using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace foodOrderingApp.models
{
    public class MenuItemVarient
    {
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }

        public string? Size { get; set; }
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}