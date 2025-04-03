using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using Microsoft.EntityFrameworkCore;

namespace foodOrderingApp. repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context =context;
            
        }
        public Order Add(OrderDto newOrder,Guid userId)
        {
            if (!newOrder.OrderItems.Any()){
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

            Order order = new Order(){
                UserId =userId,
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
            return _context.Orders.Include(o=>o.OrderItems)
    .Where(o => o.Restaurant != null && o.Restaurant.OwnerId == restaurantOwnerId)
    .ToList();
        }

        public string ProcessOrder(Guid orderId)
        {
            var order = _context.Orders.Find(orderId);
            if(order ==null){
                throw new KeyNotFoundException("Invalid Order Id");
            }
            if(order.GetOrderStatus() == Order.OrderStatus.Deliverd){
                return "Order is Already Delivered";
            }
            if(order.GetOrderStatus() ==Order.OrderStatus.Pending){
                order.SetOrderStatus(Order.OrderStatus.Processing);
            }
             else if (order.GetOrderStatus() == Order.OrderStatus.Processing)
            {
                order.SetOrderStatus(Order.OrderStatus.Shipped);
            }
            else if (order.GetOrderStatus() == Order.OrderStatus.Shipped)
            {
                order.SetOrderStatus(Order.OrderStatus.Deliverd);
            }
            _context.Update(order);
            _context.SaveChanges();
            return $"Order is {order.GetOrderStatus()}";
        }
    }
}