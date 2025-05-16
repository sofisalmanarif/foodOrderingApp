
using foodOrderingApp.models;

namespace foodOrderingApp.interfaces
{
    public interface IOrderRepository
    {
        Task<Order> Add(OrderDto newOrder,Guid userId);
        IEnumerable<Order> GetOrders(Guid restaurantOwnerId, int pageSize,int pageNumber);
        IEnumerable<Order> GetAcceptedOrders(Guid restaurantOwnerId, int pageSize, int pageNumber);

        Task<string> ProcessOrder(Guid orderId);
        Object OrderDetails( Guid orderId);
        List<dynamic> MyOrders(Guid userId);

        object UserOrderHistory(Guid userId);
        IEnumerable<Order> RestaurnatOrderHistory(Guid restaurantOwnerId,int? pageSize,int? pageNumber,DateOnly? ofDate,DateOnly? fromDate,DateOnly? toDate);

    }
}