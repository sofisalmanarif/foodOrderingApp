using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cashfreepg.Api;
using FirebaseAdmin.Messaging;
using foodOrderingApp.data;
using foodOrderingApp.models;

namespace foodOrderingApp.repositories.dashboard
{

   public enum SatsOf
    {
        Today,
        Week,
        Month,
        Year,
    }
    public class RestaurantDashboard
    {
        private readonly AppDbContext _context;
        public RestaurantDashboard(AppDbContext context)
        {
            _context = context;

        }
        public object GetRestaurntDashboard(Guid restaurantownerId, SatsOf statsOf)
        {
            Restaurant existingRestaurant = _context.Restaurants.FirstOrDefault<Restaurant>(r => r.OwnerId == restaurantownerId)!;
            if (existingRestaurant == null)
            {
                throw new KeyNotFoundException("Restaurant not found");
            }

            int menuItemsCount = _context.MenuItems.Where(i=>i.RestaurantId ==existingRestaurant.Id).Count();

            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Sunday
            var startOfNextWeek = startOfWeek.AddDays(7);

            var startOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var startOfYear = new DateTime(today.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfNextYear = startOfYear.AddYears(1);
            var daysInMonth = Enumerable.Range(0, (startOfNextMonth - startOfMonth).Days)
                    .Select(offset => startOfMonth.AddDays(offset))
                    .ToList();

            var daysOfWeek = Enumerable.Range(0, 7)
                    .Select(offset => startOfWeek.AddDays(offset))
                    .ToList();


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
                            category_name =mi!.Category!.Name!,
                            total_orders =oi.TotalQuantity
                        })
                    .ToList();

            if (statsOf ==SatsOf.Today){
                int ordersCount = _context.Orders
               .Count(o => o.RestaurantId == existingRestaurant.Id &&
                           o.CreatedAt >= today && o.CreatedAt < tomorrow);

                decimal revenue = _context.Orders
                    .Where(o => o.RestaurantId == existingRestaurant.Id &&
                                o.CreatedAt >= today && o.CreatedAt < tomorrow)
                    .Sum(o => o.TotalPrice);

                // Query actual order data grouped by hour
                var rawHourlyStats = _context.Orders
                    .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow)
                    .GroupBy(o => o.CreatedAt.Hour)
                    .Select(g => new
                    {
                        Hour = g.Key,
                        OrderCount = g.Count(),
                        TotalRevenue = g.Sum(o => o.TotalPrice)
                    })
                    .ToList();

                // Ensure all 24 hours are included
                var chartData = Enumerable.Range(0, 24)
                    .Select(hour =>
                    {
                        var stat = rawHourlyStats.FirstOrDefault(s => s.Hour == hour);
                        return new
                        {
                            Hour = hour,
                            OrderCount = stat?.OrderCount ?? 0,
                            TotalRevenue = stat?.TotalRevenue ?? 0
                        };
                    })
                    .ToList();

                    return new { ordersCount, revenue, menuItemsCount, chartData, topItems};

            }

            if(statsOf==SatsOf.Week){
                int ordersCount = _context.Orders
                .Count(o => o.RestaurantId == existingRestaurant.Id &&
                            o.CreatedAt >= startOfWeek && o.CreatedAt < startOfNextWeek);

                decimal revenue = _context.Orders
                    .Where(o => o.RestaurantId == existingRestaurant.Id &&
                            o.CreatedAt >= startOfWeek && o.CreatedAt < startOfNextWeek)
                    .Sum(o => o.TotalPrice);

                var rawStats = _context.Orders
                    .Where(o => o.CreatedAt >= startOfWeek && o.CreatedAt < startOfWeek.AddDays(7))
                    .GroupBy(o => o.CreatedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        OrderCount = g.Count(),
                        TotalRevenue = g.Sum(o => o.TotalPrice)
                    })
                    .ToList();

                var chartData = daysOfWeek
                    .Select(day =>
                    {
                        var stat = rawStats.FirstOrDefault(s => s.Date == day);
                        return new
                        {
                            Date = day,
                            OrderCount = stat?.OrderCount ?? 0,
                            TotalRevenue = stat?.TotalRevenue ?? 0
                        };
                    })
                    .ToList();

                return new { ordersCount, revenue, menuItemsCount, chartData, topItems };

            }

            if(statsOf  ==SatsOf.Month){
                int ordersCount = _context.Orders
                .Count(o => o.RestaurantId == existingRestaurant.Id &&
                            o.CreatedAt >= startOfMonth && o.CreatedAt < startOfNextMonth);

                decimal revenue = _context.Orders
                    .Where(o => o.RestaurantId == existingRestaurant.Id &&
                            o.CreatedAt >= startOfMonth && o.CreatedAt < startOfNextMonth)
                    .Sum(o => o.TotalPrice);

                //30 days per day order count and revenur
                var dailyorderStats = _context.Orders
                    .Where(o => o.CreatedAt >= startOfMonth && o.CreatedAt < startOfNextMonth)
                    .GroupBy(o => o.CreatedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        OrderCount = g.Count(),
                        TotalRevenue = g.Sum(o => o.TotalPrice)
                    })
                    .ToList();


                // Join with all days to ensure missing days have 0
                var chartData = daysInMonth.Select(date =>
                    {
                        var stat = dailyorderStats.FirstOrDefault(s => s.Date == date);
                        return new
                        {
                            Date = date,
                            OrderCount = stat?.OrderCount ?? 0,
                            TotalRevenue = stat?.TotalRevenue ?? 0
                        };
                    }).ToList();

                return new { ordersCount, revenue, menuItemsCount, chartData, topItems };

            }

            if (statsOf ==SatsOf.Year){

                int ordersCount = _context.Orders
                    .Count(o => o.RestaurantId == existingRestaurant.Id &&
                                o.CreatedAt >= startOfYear && o.CreatedAt < startOfNextYear);
                decimal revenue = _context.Orders
                    .Where(o => o.RestaurantId == existingRestaurant.Id &&
                        o.CreatedAt >= startOfYear && o.CreatedAt < startOfNextWeek)
                    .Sum(o => o.TotalPrice);

                var rawMonthlyStats = _context.Orders
                    .Where(o => o.CreatedAt >= startOfYear && o.CreatedAt < startOfNextYear)
                    .GroupBy(o => o.CreatedAt.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        OrderCount = g.Count(),
                        TotalRevenue = g.Sum(o => o.TotalPrice)
                    })
                    .ToList();

                var chartData = Enumerable.Range(1, 12)
                    .Select(month =>
                    {
                        var stat = rawMonthlyStats.FirstOrDefault(x => x.Month == month);
                        return new
                        {
                            Month = month,
                            OrderCount = stat?.OrderCount ?? 0,
                            TotalRevenue = stat?.TotalRevenue ?? 0
                        };
                    })
                    .ToList();

                return new { ordersCount, revenue, menuItemsCount, chartData, topItems };

            }
            return new{};      

        }
    }
}