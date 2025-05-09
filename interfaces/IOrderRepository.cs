
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IOrderRepository
    {
        Task<Order> Add(OrderDto newOrder,Guid userId);
        IEnumerable<Order> GetOrders(Guid restaurantOwnerId);
        IEnumerable<Order> GetAcceptedOrders(Guid restaurantOwnerId);

        Task<string> ProcessOrder(Guid orderId);
        Object OrderDetails( Guid orderId);
        List<dynamic> MyOrders(Guid userId);

        object UserOrderHistory(Guid userId);
        IEnumerable<Order> RestaurnatOrderHistory(Guid restaurantOwnerId);

    }
}