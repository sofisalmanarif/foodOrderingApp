using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using foodOrderingApp.data;
using Microsoft.EntityFrameworkCore;

namespace foodOrderingApp.repositories
{
    public class Filters
    {
        private readonly AppDbContext _context;
        public Filters(AppDbContext context)
        {
            _context = context;
        }
        public object Search(string query)
        {
            var dishes = _context.MenuItems.Include(m => m.Category)
           .Where(m => m.Name.ToLower().Contains(query) || m.Category != null && m.Category.Name.ToLower().Contains(query))
           .Select(m => new { m.Id, m.Name, m.RestaurantId, m.ImageUrl })
           .Distinct()
           .ToList();

            var restaurants = _context.Restaurants
                        .Where(r =>
                            r.IsVerified
                            &&
                            (
                                r.RestaurantName.ToLower().Contains(query)
                                ||
                                (r.MenuItems != null && r.MenuItems.Any(m =>
                                    m.Name.ToLower().Contains(query)
                                    || (m.Category != null && m.Category.Name.ToLower().Contains(query))
                                ))
                            )
                        )
                        .Select(r => new
                        {
                            r.RestaurantName,
                            r.Id,
                            r.ImageUrl,
                            r.IsActive
                        })
                        .Distinct()
                        .ToList();

            return new { dishes, restaurants };


        }


    }
}