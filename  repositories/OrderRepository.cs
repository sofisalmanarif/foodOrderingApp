using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.services;
using Microsoft.EntityFrameworkCore;

namespace foodOrderingApp.repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly FirebaseService _fireBaseService;
        public OrderRepository(AppDbContext context, FirebaseService firebaseService)
        {
            _context = context;
            _fireBaseService = firebaseService;


        }
        public async Task<Order> Add(OrderDto newOrder, Guid userId)
        {
            if (!newOrder.OrderItems.Any())
            {
                throw new ArgumentException("Order must contain at least one item.");
            }

            decimal totalPrice = 0;


            // Validate and calculate price
            foreach (var item in newOrder.OrderItems)
            {
                var menuItem = _context.MenuItems
                    .Include(m => m.Variants)
                    .FirstOrDefault(m => m.Id == item.ItemId && m.RestaurantId == newOrder.RestaurantId);

                if (menuItem == null)
                    throw new KeyNotFoundException($"MenuItem with ID {item.ItemId} not found.");

                totalPrice += (item.VariantId != null
                                ? menuItem.Variants?.FirstOrDefault(v => v.Id == item.VariantId)?.Price
                                : menuItem.Price) * item.Quantity ?? throw new KeyNotFoundException("Invalid variant or item price.");
            }

            Order order;
            if (newOrder.PaymentMethod != Order.PaymentMethod.cod && !string.IsNullOrWhiteSpace(newOrder.PaymentTransactionId))
            {
                order = new Order()
                {
                    UserId = userId,
                    RestaurantId = newOrder.RestaurantId,
                    AddressId = newOrder.AddressId,
                    OrderItems = newOrder.OrderItems,
                    paymentMethod = newOrder.PaymentMethod,
                    PaymentTransactionId = newOrder.PaymentTransactionId,
                    paymentStatus = Order.PaymentStatus.paid,
                    TotalPrice = totalPrice,

                };

            }

            else
            {
                order = new Order()
                {
                    UserId = userId,
                    RestaurantId = newOrder.RestaurantId,
                    AddressId = newOrder.AddressId,
                    OrderItems = newOrder.OrderItems,
                    TotalPrice = totalPrice,

                };
            }
            _context.Orders.Add(order);
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == newOrder.RestaurantId);
            if (restaurant == null)
                throw new KeyNotFoundException("Restaurant not found.");
            //use pushnotification
            var res = _context.FirebaseTokens.FirstOrDefault(f => f.UserId == restaurant.OwnerId);
            if (res != null)
            {
                await _fireBaseService.SendPushNotification(res.FirebaseToken, "New Order ", $"Your have a new order {order.Id}");
            }
            _context.SaveChanges();
            return order;

        }

        public IEnumerable<Order> GetAcceptedOrders(Guid restaurantOwnerId)
        {
            return _context.Orders.Include(o => o.OrderItems)
    .Where(o => o.Restaurant != null && o.Restaurant.OwnerId == restaurantOwnerId && (o.Status == Order.OrderStatus.Shipped || o.Status == Order.OrderStatus.Processing))
    .ToList();
        }

        public IEnumerable<Order> GetOrders(Guid restaurantOwnerId)
        {
            return _context.Orders.Include(o => o.OrderItems)
    .Where(o => o.Restaurant != null && o.Restaurant.OwnerId == restaurantOwnerId && o.Status == Order.OrderStatus.Pending)
    .ToList();
        }

        public List<dynamic> MyOrders(Guid userId)
        {
            var orders = _context.Orders
                .Where(o => o.UserId == userId && o.Status != Order.OrderStatus.Delivered)
                .Select(o => new
                {
                    o.Id,
                    o.TotalPrice,
                    o.Status,
                    o.CreatedAt,
                    OrderItems = o.OrderItems.Select(oi => new
                    {
                        MenuItemName = oi.MenuItem.Name,
                        MenuItemImage = oi.MenuItem.ImageUrl,
                        VariantName = oi.Variant.Size
                    }).ToList()
                })
                .ToList<dynamic>();

            if (!orders.Any())
            {
                throw new KeyNotFoundException("No Orders Found");
            }

            return orders;
        }



        public object OrderDetails(Guid orderId)
        {
            //    var order = _context.Orders
            //                     .Include(o => o.User)
            //                     .Include(o => o.Address)
            //                     .Include(o => o.OrderItems)
            //                         .ThenInclude(oi => oi.MenuItem)  
            //                     .Include(o => o.OrderItems)
            //                         .ThenInclude(oi => oi.Variant)  
            //                     .FirstOrDefault(o => o.Id == orderId);

            var orders = _context.Orders
            .Where(o => o.Id == orderId)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
            .Select(o => new
            {
                o.Id,
                o.TotalPrice,
                o.Status,
                o.CreatedAt,
                OrderItems = o.OrderItems.Select(oi => new
                {
                    MenuItemName = oi.MenuItem.Name,
                    MenuItemImage = oi.MenuItem.ImageUrl,
                    VariantName = oi.Variant.Size,
                    quantity = oi.Quantity
                }).ToList(),
                User = new
                {
                    o.User.Id,
                    o.User.Name,
                    o.User.Email,
                    o.User.Phone
                },
                Address = new
                {
                    o.Address.Id,
                    o.Address.Area,
                    o.Address.City,
                    o.Address.Landmark
                }
            })
            .FirstOrDefault();

            if (orders == null)
            {
                throw new KeyNotFoundException("No Order Found");
            }
            return orders;
        }

        public async Task<string> ProcessOrder(Guid orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Invalid Order Id");
            }
            if (order.Status == Order.OrderStatus.Delivered)
            {
                return "Order is Already Delivered";
            }
            if (order.Status == Order.OrderStatus.Pending)
            {
                order.Status = Order.OrderStatus.Processing;
            }
            else if (order.Status == Order.OrderStatus.Processing)
            {
                order.Status = Order.OrderStatus.Shipped;
            }
            else if (order.Status == Order.OrderStatus.Shipped)
            {
                order.Status = Order.OrderStatus.Delivered;
            }

            var res = _context.FirebaseTokens.FirstOrDefault(f => f.UserId == order.UserId);
            if (res != null)
            {
                await _fireBaseService.SendPushNotification(res.FirebaseToken, "Order Status", $"Your order is {order.Status}");
            }

            _context.Update(order);
            _context.SaveChanges();
            return $"Order is {order.Status}";
        }

        public IEnumerable<Order> RestaurnatOrderHistory(
       Guid restaurantOwnerId,
       int? pageSize,
       int? pageNumber,
       DateOnly? ofDate,
       DateOnly? fromDate,
       DateOnly? toDate)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Restaurant != null
                            && o.Restaurant.OwnerId == restaurantOwnerId
                            && o.Status == Order.OrderStatus.Delivered);

            // Convert DateOnly to UTC DateTime 
            if (ofDate.HasValue)
            {
                var utcOfDate = DateTime.SpecifyKind(ofDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                query = query.Where(o => o.CreatedAt.Date == utcOfDate.Date);
            }

            if (fromDate.HasValue)
            {
                var utcFromDate = DateTime.SpecifyKind(fromDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                query = query.Where(o => o.CreatedAt.Date >= utcFromDate.Date);
            }

            if (toDate.HasValue)
            {
                var utcToDate = DateTime.SpecifyKind(toDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
                query = query.Where(o => o.CreatedAt.Date <= utcToDate.Date);
            }

            if (pageSize.HasValue && pageNumber.HasValue)
            {
                query = query
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return query.ToList();
        }




        public object UserOrderHistory(Guid userId)
        {
            var order = _context.Orders
            .Where(o => o.UserId == userId && o.Status == Order.OrderStatus.Delivered)
            .Select(o => new
            {
                o.Id,
                o.TotalPrice,
                o.Status,
                o.CreatedAt,
                OrderItems = o.OrderItems.Select(oi => new
                {
                    MenuItemName = oi.MenuItem.Name,
                    MenuItemImage = oi.MenuItem.ImageUrl,
                    VariantName = oi.Variant.Size
                }).ToList(),
            })
            .FirstOrDefault();

            if (order == null)
            {
                throw new KeyNotFoundException("No Order Found");
            }
            return order;
        }
    }
}