
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IOrderRepository
    {
        Order Add(OrderDto newOrder,Guid userId);
        IEnumerable<Order> GetOrders(Guid restaurantOwnerId);
        IEnumerable<Order> GetAcceptedOrders(Guid restaurantOwnerId);

        string ProcessOrder(Guid orderId);
        Object OrderDetails( Guid orderId);
        List<dynamic> MyOrders(Guid userId);

        object UserOrderHistory(Guid userId);
        IEnumerable<Order> RestaurnatOrderHistory(Guid restaurantOwnerId);

    }
}