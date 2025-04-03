using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using Microsoft.EntityFrameworkCore;

namespace foodOrderingApp.repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;

        }
        public Order Add(OrderDto newOrder, Guid userId)
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

            Order order = new Order()
            {
                UserId = userId,
                RestaurantId = newOrder.RestaurantId,
                AddressId = newOrder.AddressId,
                OrderItems = newOrder.OrderItems,
                TotalPrice = totalPrice


            };
            _context.Orders.Add(order); _context.Orders.Add(order);
            _context.SaveChanges();
            return new Order();
        }

        public IEnumerable<Order> GetOrders(Guid restaurantOwnerId)
        {
            return _context.Orders.Include(o => o.OrderItems)
    .Where(o => o.Restaurant != null && o.Restaurant.OwnerId == restaurantOwnerId)
    .ToList();
        }

        public object OrderDetails(Guid orderId)
        {
            //    var order = _context.Orders
            //                     .Include(o => o.User)
            //                     .Include(o => o.Address)
            //                     .Include(o => o.OrderItems)
            //                         .ThenInclude(oi => oi.MenuItem)  // ✅ Include MenuItem (if applicable)
            //                     .Include(o => o.OrderItems)
            //                         .ThenInclude(oi => oi.Variant)  // ✅ Include Variant (if applicable)
            //                     .FirstOrDefault(o => o.Id == orderId);

            var order = _context.Orders
    .Where(o => o.Id == orderId)
    .Include(o=>o.User)
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
            VariantName = oi.Variant.Size
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

            if (order == null)
            {
                throw new KeyNotFoundException("No Order Found");
            }
            return order;
        }

        public string ProcessOrder(Guid orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Invalid Order Id");
            }
            if (order.GetOrderStatus() == Order.OrderStatus.Delivered)
            {
                return "Order is Already Delivered";
            }
            if (order.GetOrderStatus() == Order.OrderStatus.Pending)
            {
                order.SetOrderStatus(Order.OrderStatus.Processing);
            }
            else if (order.GetOrderStatus() == Order.OrderStatus.Processing)
            {
                order.SetOrderStatus(Order.OrderStatus.Shipped);
            }
            else if (order.GetOrderStatus() == Order.OrderStatus.Shipped)
            {
                order.SetOrderStatus(Order.OrderStatus.Delivered);
            }
            _context.Update(order);
            _context.SaveChanges();
            return $"Order is {order.GetOrderStatus()}";
        }
    }
}