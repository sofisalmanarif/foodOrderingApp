using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cashfreepg.Api;
using foodOrderingApp.data;
using foodOrderingApp.models;

namespace foodOrderingApp. repositories.dashboard
{
    public class RestaurantDashboard
    {
        private readonly AppDbContext _context;
        public RestaurantDashboard(AppDbContext context)
        {
            _context=context;
            
        }
        public object GetRestaurntDashboard(Guid restaurantownerId){
            Restaurant existingRestaurant = _context.Restaurants.FirstOrDefault<Restaurant>(r=>r.OwnerId == restaurantownerId)!;
            if(existingRestaurant == null){
                throw new KeyNotFoundException("Restaurant not found");
            }
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Sunday
            var startOfNextWeek = startOfWeek.AddDays(7);

            var startOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var startOfYear = new DateTime(today.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfNextYear = startOfYear.AddYears(1);


            //orders
            int todaysOrders = _context.Orders
                .Count(o => o.RestaurantId == existingRestaurant.Id &&
                            o.CreatedAt >= today && o.CreatedAt < tomorrow);

            int thisWeeksOrders = _context.Orders
                .Count(o => o.RestaurantId == existingRestaurant.Id &&
                            o.CreatedAt >= startOfWeek && o.CreatedAt < startOfNextWeek);

            int thisMonthsOrders = _context.Orders
                .Count(o => o.RestaurantId == existingRestaurant.Id &&
                            o.CreatedAt >= startOfMonth && o.CreatedAt < startOfNextMonth);

            int thisYearsOrders = _context.Orders
                .Count(o => o.RestaurantId == existingRestaurant.Id &&
                            o.CreatedAt >= startOfYear && o.CreatedAt < startOfNextYear);



            //Revenue
            decimal todaysRevenue = _context.Orders
            .Where(o => o.RestaurantId == existingRestaurant.Id &&
                        o.CreatedAt >= today && o.CreatedAt < tomorrow)
            .Sum(o => o.TotalPrice);

            decimal thisWeeksRevenue = _context.Orders
            .Where(o => o.RestaurantId == existingRestaurant.Id &&
                        o.CreatedAt >= startOfWeek && o.CreatedAt < startOfNextWeek)
            .Sum(o => o.TotalPrice);

            decimal thisMonthsRevenue = _context.Orders
           .Where(o => o.RestaurantId == existingRestaurant.Id &&
                       o.CreatedAt >= startOfMonth && o.CreatedAt < startOfNextMonth)
           .Sum(o => o.TotalPrice);

            decimal thisYearsRevenue = _context.Orders
            .Where(o => o.RestaurantId == existingRestaurant.Id &&
                        o.CreatedAt >= startOfYear && o.CreatedAt < startOfNextWeek)
            .Sum(o => o.TotalPrice);

            var topItems = _context.OrderItem
                .GroupBy(oi => oi.ItemId)
                .Select(g => new
                {
                    ItemId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .Join(_context.MenuItems,
                    oi => oi.ItemId,
                    mi => mi.Id,
                    (oi, mi) => new
                    {
                        mi.Id,
                        mi.Name,
                        mi.ImageUrl,
                        // mi.Variants,
                        oi.TotalQuantity
                    })
                .ToList();



            var stats = new[]
                            {
                                new { Label = "Today", Orders = todaysOrders, Revenue = todaysRevenue },
                                new { Label = "This Week", Orders = thisWeeksOrders, Revenue = thisWeeksRevenue },
                                new { Label = "This Month", Orders = thisMonthsOrders, Revenue = thisMonthsRevenue },
                                new { Label = "This Year", Orders = thisYearsOrders, Revenue = thisYearsRevenue }
                            };
            return new{stats, topItems };


        }
    }
}